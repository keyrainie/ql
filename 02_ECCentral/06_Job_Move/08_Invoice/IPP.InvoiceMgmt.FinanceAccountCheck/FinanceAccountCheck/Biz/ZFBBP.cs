using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using IPPOversea.Invoicemgmt.ZFBAccountCheck.DAL;
using IPPOversea.Invoicemgmt.ZFBAccountCheck.Biz;
using System.Configuration;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.Data;
using IPPOversea.Invoicemgmt.Model;
using System.Diagnostics;



namespace IPPOversea.Invoicemgmt.ZFBAccountCheck.Biz
{
    public static class ZFBBP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);
        private static ILog log = LogerManger.GetLoger();
        private static List<ZFBDataEntity> lastInfo = new List<ZFBDataEntity>();
        #endregion

        public static void DoWork(AutoResetEvent are)
        {
            ZFBDA zfbDA = new ZFBDA();
            DateTime createStart, createEnd;

            #region 测试importData

            //zfbDA.ImportDataLength();
            //throw new Exception("测试");

            #endregion

            try
            {
                if (zfbDA.IsFirst())
                {
                    createStart = Settings.DefaultDate;
                    createEnd = createStart.AddDays(1);
                }
                else
                {
                    createStart = zfbDA.GetLastImportDate();
                    createEnd = createStart.AddDays(1);
                    lastInfo = zfbDA.GetLastTimeData(createStart);
                }

                OnShowInfo("createStart:" + createStart);
                OnShowInfo("createEnd:" + createEnd);

                if (!string.IsNullOrEmpty(Settings.Partner) && !string.IsNullOrEmpty(Settings.Key))
                {
                    ProcessData(createStart, createEnd, Settings.Partner, Settings.Key);
                }
                if (!string.IsNullOrEmpty(Settings.partnerAdd) && !string.IsNullOrEmpty(Settings.KeyAdd))
                {
                    ProcessData(createStart, createEnd, Settings.partnerAdd, Settings.KeyAdd);
                }

                OnShowInfo("开始同步PayedDate:" + DateTime.Now);

                var rowCount = 0;

                do
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    rowCount = zfbDA.SysncPayedDate();

                    OnShowInfo(string.Format("更新到{0}条数据..花费时间{1}ms", rowCount.ToString(), sw.ElapsedMilliseconds.ToString()));
                }
                while (rowCount > 0);

                OnShowInfo("同步PayedDate完毕:" + DateTime.Now);

                Console.ForegroundColor = ConsoleColor.Green;
                OnShowInfo("\r\n执行完毕");
                are.Set();
            }
            catch (Exception ex)
            {
                are.Set();
                throw ex;
            }
        }

        private static void ProcessData(DateTime createStart, DateTime createEnd, string partner, string key)
        {
            var xmlResult = new XmlDocument();
            string AttachInfo2;

            if (partner == "2088101149633121")
            {
                AttachInfo2 = "alipay_newegg2@newegg.com.cn";
            }
            else
            {
                AttachInfo2 = "alipay_newegg@newegg.com.cn";
            }

            for (int overTry = 1; overTry <= 3; )
            {
                xmlResult = GetResponseXml(createStart.ToString("yyyy-MM-dd HH:mm:ss"),
                    createEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                    partner,
                    key,
                    ref overTry);
            }

            if (xmlResult == null)
            {
                throw new Exception("尝试获取数据失败，停止本次job运行!");
            }

            CreateLog(xmlResult);
            CreateData(xmlResult, AttachInfo2);
        }

        private static void CreateData(XmlDocument xmlResult, string AttachInfo2)
        {
            var zfbDA = new ZFBDA();
            var entityList = new List<ZFBDataEntity>();
            var now = DateTime.Now;

            XmlNodeList nodeList = xmlResult.GetElementsByTagName("csv_data");
            if (nodeList.Count == 0)
            {
                throw new Exception("获取数据失败");
            }
            string strResult = nodeList[0].InnerXml;

            XmlNodeList nodeList01 = xmlResult.GetElementsByTagName("is_success");
            string flag = nodeList01[0].InnerXml;

            XmlNodeList nodeList02 = xmlResult.GetElementsByTagName("count");
            int countList = int.Parse(nodeList02[0].InnerXml);


            if (flag == "F")
            {
                throw new Exception("获取失败,is_success标识为F");
            }
            if (countList <= 0)
            {
                throw new Exception("当天没有交易记录");
            }

            //去掉左右冗余符号
            strResult = strResult.Replace("&lt;![CDATA[", "").Replace("]]&gt;", "").Replace("\n", "").Trim();
            string[] resultArray = strResult.Split(',');
            const int coulmnCount = 14;
            int rowCount = resultArray.Count() / coulmnCount - 1;

            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            //加列头
            for (int i = 0; i < coulmnCount; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = resultArray[i];
                table.Columns.Add(column);
            }
            //加行
            int index = coulmnCount;
            for (int j = 0; j < rowCount; j++)
            {
                row = table.NewRow();
                for (int i = 0; i < coulmnCount; i++)
                {
                    row[i] = resultArray[index++];
                }
                table.Rows.Add(row);
            }

            foreach (DataRow rowData in table.Rows)
            {
                try
                {
                    var soSysNo = Convert.ToInt32(rowData[0]);
                    var getPayTypeResult = zfbDA.GetPayType(soSysNo);
                    if (getPayTypeResult == null || getPayTypeResult.Count == 0)
                    {
                        throw new Exception("SO_Master中查找不到该订单信息");
                    }
                    PayTypeEntity paytypeEntity = getPayTypeResult[0];

                    entityList.Add(new ZFBDataEntity
                    {
                        SoSysNo = soSysNo,
                        PayTermsNo = paytypeEntity.PayTermsNo,
                        PayTerms = paytypeEntity.PayTerms,
                        PayedDate = Convert.ToDateTime(rowData[2]),
                        PayedAmt = Convert.ToDecimal(rowData[8]),
                        SerialNumber = rowData[3].ToString(),
                        OutOrderNo = rowData[4].ToString(),
                        PayedUserTag = rowData[5].ToString(),
                        PayedUserName = rowData[6].ToString(),
                        PayedUserNo = rowData[7].ToString(),
                        PartnerName = "支付宝",
                        TradeType = rowData[12].ToString(),
                        AttachInfo = rowData[11].ToString(),
                        AttachInfo2 = AttachInfo2,
                        InUser = "System",
                        InDate = now
                    });
                }
                catch (Exception ex)
                {
                    string[] strArray = rowData.ItemArray.Select(x =>
                    {
                        return x == null ? string.Empty : x.ToString();
                    }).ToArray<string>();
                    string details = string.Format("异常订单信息 SoSysNo:{0},InDate:{1},Error:{2},ErrorInfoSource:{3}",
                        rowData[0].ToString(),
                        rowData[2].ToString(),
                        ex.Message,
                        string.Join(",", strArray));
                    OnShowInfo(details);
                    SendMail("支付宝对账异常", details);

                    if (rowData[0].ToString().Length >= 9)
                    {
                        var longInfo = string.Format("支付宝过长订单号信息提醒 SoSysNo:{0},InDate:{1},ErrorInfoSource:{2}",
                            rowData[0].ToString(),
                            rowData[2].ToString(),
                            string.Join(",", strArray));
                        MailDA.SendEmail(Settings.LongOrderSysNoAlert, "支付宝过长订单号信息提醒", longInfo);
                    }
                    else if (ex.Message.Contains("SO_Master中查找不到该订单信息"))
                    {
                        var longInfo = string.Format("无法确认支付宝订单提醒 SoSysNo:{0},InDate:{1},ErrorInfoSource:{2}",
                            rowData[0].ToString(),
                            rowData[2].ToString(),
                            string.Join(",", strArray));
                        MailDA.SendEmail(Settings.LongOrderSysNoAlert, "无法确认支付宝订单提醒", longInfo);
                    }
                }
            }

            if (lastInfo != null && lastInfo.Count > 0)
            {
                entityList = entityList.Except(lastInfo, new ZFBEntityComparer()).ToList();
            }

            if (entityList == null || entityList.Count == 0)
            {
                throw new Exception("不存在可导入记录");
            }
            OnShowInfo("开始导入数据");
            try
            {
                //每一百条数据导入一次
                var entityMiddle = new List<ZFBDataEntity>();
                for (int i = 0; i < entityList.Count; i++)
                {
                    entityMiddle.Add(entityList[i]);
                    if ((i != 0 && (i + 1) % 100 == 0) || (i + 1) == entityList.Count)
                    {
                        zfbDA.CreateData(entityMiddle);
                        OnShowInfo(string.Format("导入了{0}条记录", (i + 1).ToString()));
                        entityMiddle.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                OnShowInfo(string.Format("导入失败:{0}", ex.Message));
            }
            OnShowInfo("导入完毕");
        }

        private static void CreateLog(XmlDocument xmlResult)
        {
            var zfbDA = new ZFBDA();
            var logEnity = new ZFBLogEntity();

            logEnity.ImportData = xmlResult.InnerXml;
            logEnity.PayTermsNo = 34;
            logEnity.PayTerms = "支付宝";
            logEnity.InUser = "System";
            logEnity.InDate = DateTime.Now;
            OnShowInfo("开始记录日志表");
            try
            {
                logEnity = zfbDA.CreateLogContent(logEnity);
                zfbDA.CreateLog(logEnity);
            }
            catch (Exception ex)
            {
                OnShowInfo(string.Format("记录日志表失败:{0}", ex.Message));
            }
            OnShowInfo("记录日志表完成");
        }

        public static void SendMail(string subject, string mailBody)
        {
            MailDA.SendEmail(Settings.EmailAddress, subject, mailBody);
        }

        private static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }

        private static XmlDocument GetResponseXml(string createStart, string createEnd, string partner, string key, ref int overTry)
        {
            XmlDocument rssDoc = new XmlDocument();

            //支付URL生成
            string aliay_url = AliPay.CreatUrl(
                Settings.GateWay,
                Settings.Service,
                partner,
                createStart,
                createEnd,
                Settings.TransCode,
                Settings.SignType,
                key,
                Settings.InputCharset
                );

            //跳转到支付宝页并解析XML
            try
            {
                OnShowInfo(string.Format("开始第{0}次获取数据", overTry));
                WebRequest myRequest = System.Net.WebRequest.Create(aliay_url);
                WebResponse myResponse = myRequest.GetResponse();
                Stream rssStream = myResponse.GetResponseStream();
                rssDoc.Load(rssStream);
                OnShowInfo("成功获取数据");
                overTry = 4;
            }
            catch (Exception ex)
            {
                OnShowInfo(string.Format("第{0}次获取数据出错:" + ex.Message, overTry));
                overTry++;
                Thread.Sleep(Settings.TryPeriod * 60 * 1000);
            }

            return rssDoc;
        }
    }

    public class ZFBEntityComparer : IEqualityComparer<ZFBDataEntity>
    {
        #region IEqualityComparer<ZFBDataEntity> Members

        public bool Equals(ZFBDataEntity x, ZFBDataEntity y)
        {
            return x.SoSysNo == y.SoSysNo
                && x.SerialNumber == y.SerialNumber;
        }

        public int GetHashCode(ZFBDataEntity obj)
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
