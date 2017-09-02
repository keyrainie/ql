using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SendMessage.Class;
using SendMessage.Entity;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace SendMessage
{
    public class SmsSender
    {
        public static void SendSmsMessage()
        {
            SendSmsMessage(null);
        }

        /// <summary>
        /// 发送手机短信
        /// </summary>
        public static void SendSmsMessage(JobContext context)
        {
            try
            {
                List<SmsEntity> smsList = null;
                SmsDA.WriteLog("发送短信任务开始");
                //获取待发送短信列表
                int topCount = Helper.TopCount;
                int priority = Helper.Priority;

                bool EnableTranslator = Helper.EnableTranslator;
                string sEncoder = Helper.SourceEncoder;
                string tEncoder = Helper.TargetEncoder;

                //晚上只取高有限级的短信
                if (Helper.IsInDayTime())
                {
                    priority = 100;
                }

                smsList = SmsDA.GetSMS2SendList(priority, topCount);

                SmsDA.WriteLog(string.Format("共找到待发送短信记录:{0}条!", smsList.Count));

                if (smsList.Count > 0)
                {
                    SmsDA.WriteLog("开始验证手机号码格式");
                    smsList.ForEach(x =>
                    {

                        x.CheckResult = Helper.IsCellPhoneNumber(x.CellNumber);
                        x.ProcessMessage = "手机号码格式验证失败!";
                    });

                    //if (EnableTranslator
                    //    || !string.Equals(sEncoder, tEncoder, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    SmsDA.WriteLog(string.Format("开始对短信内容进行编码转换，由{0}转为{1}", sEncoder, tEncoder));
                    //    smsList.ForEach(x =>
                    //    {
                    //        //在所有的短信内容后面加上【易捷网】
                    //        x.SMSContent = x.SMSContent + "【易捷网】";
                    //        x.SMSContent = Encoding.GetEncoding(tEncoder).GetString(Encoding.GetEncoding(sEncoder).GetBytes(x.SMSContent));
                    //    });
                    //}

                    //初始化发短信服务
                    //var smsService = new SendMessage.SmsService.CustomerServicePortTypeClient();
                    ///var smsService = new SendMessage.NetShopService.NetShopServicePortTypeClient();
                    //var smsService = new SmsServiceHttp();
                    //string Account = ConfigurationManager.AppSettings["Account"];
                    //string Password = ConfigurationManager.AppSettings["Password"];

                    string E_CompanyId = ConfigurationManager.AppSettings["E_CompanyId"];
                    string E_UserName = ConfigurationManager.AppSettings["E_UserName"];
                    string E_PassWord = ConfigurationManager.AppSettings["E_PassWord"];
                    Random rd = new Random();
                    Sms smsService = new Sms();
                    foreach (var x in smsList)
                    {
                        if (x.CheckResult)
                        {
                            string guid = buildGuid(x.SysNo);

                            string num =  string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), rd.Next(100, 999));
                            string result = smsService.CallSms(E_CompanyId, E_UserName, E_PassWord, x.SMSContent, x.CellNumber, num, "", "", "");
                            if ((!string.IsNullOrWhiteSpace(result)) && (result.ToLower() == "result=0"
                                || result.ToLower().IndexOf("&result=0&") > -1
                                || result.ToLower().IndexOf("=result=0&") > -1
                                || result.ToLower().Substring(0, 9) == "result=0&"
                                || result.ToLower().Substring(result.Length - 9, 9) == "&result=0"
                                || result.ToLower().Substring(result.Length - 9, 9) == "=result=0"))
                            {
                                x.ProcessMessage = "短信成功发出! 业务流水号：" + num + ", result=" + result;
                            }
                            else
                            {
                                x.CheckResult = false;
                                x.ProcessMessage = "短信发送失败! 业务流水号：" + num + ", result=" + result;
                            }
                        }
                    }

                    //更新发送结果
                    foreach (var x in smsList)
                    {
                        if (x.CheckResult)
                        {
                            SmsDA.UpdateResult(x);
                        }
                        else
                        {
                            SmsDA.UpdateRetryCount(x.SysNo);
                        }
                    }

                    StringBuilder rptsb = new StringBuilder();
                    foreach (var x in smsList)
                    {
                        rptsb.AppendFormat("记录编号:{0} - {1}\r\n", x.SysNo, x.ProcessMessage);
                        //SmsDA.WriteLog(string.Format("记录编号:{0} - {1}", x.SysNo,x.ProcessMessage));
                    }

                    SmsDA.WriteLog(string.Format("短信发送报告：{0}", rptsb.ToString()));

                    //邮件通知发送短信报告
                    SendReportMail(rptsb.ToString());
                }


            }
            catch (Exception ex)
            {
                string msg = string.Format("发送短信时出现异常：{0}\r\n{1}", ex.Message, ex.Source);
                SmsDA.WriteLog(msg);
                SendReportMail(msg);
                if (context != null)
                {
                    context.Message += msg + "\r\n";
                }
            }

            SmsDA.WriteLog("本次发送任务结束!");
        }

        private static string buildGuid(int sysno)
        {
            //源系统标识号(6位)+源系统交易日期(8位: YYYYMMDD)+集群编号(2位)+流水序号(10位)。
            //ü	源系统标识号：6位，见：应用系统编码列表，注意系统ID为STRING类型，允许包含字母。全行标识业务系统的唯
            //一编号，统一编制。
            //ü	源系统交易日期：为交易流程第一个发起系统的账务日期，按YYYYMMDD的格式编排。 
            //ü	集群编号：分多路部署应用的系统需设置集群编号，单路部署应用的系统默认为“1”,不足2位前补0。 
            //ü	流水序号：由源系统生成10位的数字序号；流水序号达到最大或者日期发生切换后,重新开始计数。
            //ü	业务流水号：唯一标识一笔业务的流水号，由交易发起方的系统产生，贯穿整个业务流程，反映一笔业务的唯一标
            //识，是端对端的。
            //ü	交易流水号：标识一笔交易的流水号，两个相邻系统之间传递，由前端系统产生，反映每次调用的唯一标识，后端
            //系统保存前端系统的交易流水号。
            return ConfigurationManager.AppSettings["CONSUMER_ID"] + DateTime.Now.ToString("yyyyMMdd") + "01" + sysno.ToString().PadLeft(10, '0');
        }

        private static void SendReportMail(string message)
        {
            string mailto = Helper.MailTo;
            string mailSubject = Helper.MailSubject;
            string msg = string.Format("短信发送报告:\r\n", message);
            SmsDA.SendMail(mailto, mailSubject, msg);
        }

        private static string GetServiceErrorMessage(int errCode)
        {
            string rst = "其他错误";
            /*
               接口返回码含义如下：
                -1：系统错误
                0：正常
                1：手机号码错误
                2：信息长度错误
                3：信息内容不合法
                4：其他错误
            */

            switch (errCode)
            {
                case -1:
                    { rst = "系统错误"; }
                    break;
                case 0:
                    { rst = "正常"; }
                    break;
                case 1:
                    { rst = "手机号码错误"; }
                    break;
                case 2:
                    { rst = "信息长度错误"; }
                    break;
                case 3:
                    { rst = "信息内容不合法"; }
                    break;
                default:
                    { rst = "其他错误"; }
                    break;
            }
            return rst;
        }
        private static string GetServiceErrorInfo(string rst)
        {
            string errorInfo = "";
            /*
               接口返回码含义如下：
                -1：系统错误
                0：正常
                1：手机号码错误
                2：信息长度错误
                3：信息内容不合法
                4：其他错误
            */

            switch (rst)
            {
                case "1":
                    { errorInfo = "没有需要取得的数据"; }
                    break;
                case "-2":
                    { errorInfo = "帐号/密码不正确"; }
                    break;
                case "-4":
                    { errorInfo = "余额不足"; }
                    break;
                case "-5":
                    { errorInfo = "数据格式错误"; }
                    break;
                case "-6":
                    { errorInfo = "参数有误"; }
                    break;
                case "-7":
                    { errorInfo = "权限受限"; }
                    break;
                case "-8":
                    { errorInfo = "流量控制错误"; }
                    break;
                case "-9":
                    { errorInfo = "扩展码权限错误"; }
                    break;
                case "-10":
                    { errorInfo = "内容长度长"; }
                    break;
                case "-11":
                    { errorInfo = "内部数据库错误"; }
                    break;
                case "-12":
                    { errorInfo = "序列号状态错误"; }
                    break;
                case "-13":
                    { errorInfo = "没有提交增值内容"; }
                    break;
                case "-14":
                    { errorInfo = "服务器写文件失败"; }
                    break;
                case "-15":
                    { errorInfo = "文件内容base64编码错误"; }
                    break;
                case "-16":
                    { errorInfo = "返回报告库参数错误"; }
                    break;
                case "-17":
                    { errorInfo = "没有权限"; }
                    break;
                case "-18":
                    { errorInfo = "上次提交没有等待返回不能继续提交"; }
                    break;
                case "-19":
                    { errorInfo = "禁止同时使用多个接口地址"; }
                    break;
                default:
                    { errorInfo = ""; }
                    break;
            }
            return errorInfo;
        }
    }
}
