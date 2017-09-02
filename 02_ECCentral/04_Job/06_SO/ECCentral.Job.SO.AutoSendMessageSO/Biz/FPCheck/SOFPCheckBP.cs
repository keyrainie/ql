using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Job.SO.AutoSendMessageSO.BusinessEntities.AutoAudit;
using ECCentral.Job.SO.AutoSendMessageSO.BusinessEntities.FPCheck;
using System.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.SO.AutoSendMessageSO.Dac.Common;
using ECCentral.Job.SO.AutoSendMessageSO.Utilities;
using ECCentral.Job.SO.AutoSendMessageSO.Dac.FPCheck;
using ECCentral.Job.SO.AutoSendMessageSO.Common;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.DataContracts;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.DataAccess;
using ECCentral.Job.SO.AutoSendMessageSO.Dac.AutoAudit;


namespace ECCentral.Job.SO.AutoSendMessageSO.Biz.FPCheck
{
    public class SOFPCheckBP
    {
        private const string CCPAYTYPE = "CREDITCARD_PAYTYPE";
        private const string INTERORDER = "PromotionIntel2009Q4";

        #region PreData List

        /// <summary>
        /// 自动审核一次最多提取的单数
        /// </summary>
        private static int TopCount = 2000;

        /// <summary>
        /// 是否进行可疑订单检测
        /// </summary>
        private static bool IsCheckKeYi = false;

        /// <summary>
        /// 是否进行串货检测
        /// </summary> 
        private static bool IsCheckChuanHuo = false;

        /// <summary>
        /// 是否进行炒货检测
        /// </summary>
        private static bool IsCheckChaoHuo = false;

        /// <summary>
        /// 是否进行重复订单检测
        /// </summary>
        private static bool IsCheckChongFu = false;

        /// <summary>
        /// 一个月内被物流拒收过单拒的客户编号
        /// </summary>
        private static List<int> AutoRMACustomerSysNos;

        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        /// <summary>
        /// 支付方式列表
        /// </summary>
        private static List<PayTypeEntity> payTypeList;


        ////恶意用户列表
        //public static List<DubiousCustomerEntity> spiteList;

        ////拒收用户列表
        //public static List<DubiousCustomerEntity> rejList;

        ////占库存用户列表
        //public static List<DubiousCustomerEntity> occupyList;

        //断货订单支持仓库
        private static List<string> OutStockList = new List<string>();
        #endregion

        //以下五项为JOB中调用WCF服务时所使用到的信息
        private static string UserDisplayName;
        private static string UserLoginName;
        private static string CompanyCode;
        private static string StoreCompanyCode;
        private static string StoreSourceDirectoryKey;

        /// <summary>
        ///  获取FPCheck检查项的具体明细项信息
        /// </summary>
        private static List<FPCheckItemEntity> FPCheckItemList;

        /// <summary>
        /// 获取FPCheck检查项信息
        /// </summary>
        private static List<FPCheckMasterEntity> FPCheckMasterList;

        /// <summary>
        /// 验证串货的产品编号列表
        /// </summary>
        private static List<string> ChuanHuoProductNoList;

        /// <summary>
        /// 验证串货的C3编号列表
        /// </summary>
        private static List<int> ChuanHuoC3SysNoList;


        /// <summary>
        /// 从配置文件获取为邮件发送JOB中调用WCF服务时所使用到的信息
        /// </summary>
        /// <returns></returns>
        public static void GetOprationUserInfo()
        {
            UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
        }

        #region Entry Point

        /// <summary>
        /// 获取一次审核最多的记录数
        /// </summary>
        /// <returns></returns>
        private static int GetFPCheckTopCount()
        {
            string tmpAuditUserSysNo = ConfigurationManager.AppSettings["FPCheckTopCount"];
            int topCount = int.Parse(tmpAuditUserSysNo);

            return (topCount > 0) ? topCount : 2000;
        }

        /// <summary>
        /// 获取是否可疑订单的验证标识符
        /// </summary>
        /// <returns></returns>
        private static bool GetFPCheckKeYiFlag()
        {
            bool result = false;
            if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x => { return (x.CheckType == "KY" && x.Status == 0); }) != null)
            {
                result = true;
            }
            return result;
        }




        /// <summary>
        /// 获取是否串单的验证标识符
        /// </summary>
        /// <returns></returns>
        private static bool GetFPCheckChuanHuoFlag()
        {
            bool result = false;
            if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x => { return (x.CheckType == "CH" && x.Status == 0); }) != null)
            {
                result = true;
            }

            if (result)
            {
                ChuanHuoProductNoList = new List<string>();
                FPCheckItemList.FindAll(x =>
                {
                    return (x.ReferenceType == "PID" && x.Status == 0);
                }).ForEach(x =>
                {
                    ChuanHuoProductNoList.Add(x.ReferenceContent);
                });

                ChuanHuoC3SysNoList = new List<int>();
                FPCheckItemList.FindAll(x =>
                {
                    return (x.ReferenceType == "PC3" && x.Status == 0);
                }).ForEach(x =>
                {
                    ChuanHuoC3SysNoList.Add(Convert.ToInt32(x.Description.Trim()));
                });
            }

            return result;
        }



        /// <summary>
        /// 获取是否进行炒单验证的标识符
        /// </summary>
        /// <returns></returns>
        private static bool GetFPCheckChaoHuoFlag()
        {
            bool result = false;
            if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x =>
            {
                return (x.CheckType == "CC" && x.Status == 0);
            }) != null)
            {
                result = true;
            }
            return result;
        }




        /// <summary>
        /// 获取是否检测重复订单标识符
        /// </summary>
        /// <returns></returns>
        private static bool GetCheckChongFuFlag()
        {
            //判断是否进行炒货检测          
            bool result = false;
            if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x =>
            {
                return (x.CheckType == "CF" && x.Status == 0);
            }) != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 开始FP验证
        /// </summary>
        /// <param name="jobContext">Job运行上下文</param>
        public static void Check(JobContext jobContext)
        {
            GetOprationUserInfo();

            //提前取得所有支付方式的列表
            payTypeList = CommonDA.GetPayTypeList(CompanyCode);

            BizLogFile = jobContext.Properties["BizLog"];

            StartCheckAll(jobContext);
            //提前取得一次验单的最多数目
            TopCount = GetFPCheckTopCount();
            //提取验证的订单列表
            List<SOEntity4FPEntity> tmpSOENtityList = SOFPCheckDA.GetSOList4FPCheck(TopCount, CompanyCode);
            //无新单时退出
            if (null == tmpSOENtityList
                || tmpSOENtityList.Count == 0)
            {
                jobContext.Message += "没有附合条件的订单记录" + Environment.NewLine;
                WriteLog("没有附合条件的订单记录");
                return;
            }
            else
            {
                jobContext.Message += "本次提取订单记录" + tmpSOENtityList.Count + "条." + Environment.NewLine;
            }

            //提前获取断货支持仓库
            string outStockStr = ConfigurationManager.AppSettings["OUT_STOCK"];
            string[] outStockArray = outStockStr.Split(',');
            foreach (string os in outStockArray)
            {
                OutStockList.Add(os);
            }

            //提前取得一个月内被物流拒收过单拒的客户编号
            AutoRMACustomerSysNos = SOFPCheckDA.GetAutoRMACustomerSysNos(CompanyCode);


            //提取现有恶意用户列表
            //spiteList = SOFPCheckDA.GetDubiousCustomersByCat(2);

            //提取现有拒收用户列表
            //rejList = SOFPCheckDA.GetDubiousCustomersByCat(1);

            //提取现有占库存用户列表
            //occupyList = SOFPCheckDA.GetDubiousCustomersByCat(0);

            //提取FP检查项列表
            FPCheckMasterList = SOFPCheckDA.GetFPCheckMasterList(CompanyCode);

            //提取FP检查项列表明细项
            FPCheckItemList = SOFPCheckDA.GetFPCheckItemList(CompanyCode);

            //提前取得是否进行可疑订单检测是标识符
            IsCheckKeYi = GetFPCheckKeYiFlag();

            //提前取得是否进行串单检测标识符
            IsCheckChuanHuo = GetFPCheckChuanHuoFlag();

            //提前取得是否进行炒货订单检测标识符
            IsCheckChaoHuo = GetFPCheckChaoHuoFlag();

            //提前获取是否进行重复订单检测标识符
            IsCheckChongFu = GetCheckChongFuFlag();

            foreach (SOEntity4FPEntity x in tmpSOENtityList)
            {
                try
                {
                    string FromLinkSource = SOFPCheckDA.GetFromLinkSource(x.CustomerSysNo);
                    //阿斯利康用户、盛大用户、柯尼卡美能达用户、理光用户、Intel用户无需审核
                    if ((FromLinkSource != null && FromLinkSource == INTERORDER) ||
                        (x.CustomerID.StartsWith("AstraZeneca-")) ||
                        (x.CustomerID.StartsWith("Shanda")) ||
                        (x.CustomerID.StartsWith("konicaminolta")) ||
                        (x.CustomerID.StartsWith("Ricoh-")))
                    {
                        continue;
                    }
                    CheckSingle(x);
                }
                catch (Exception ex)
                {
                    jobContext.Message += string.Format("订单[{0}]出现异常:{1}\r\n", x.SysNo, ex.Message);
                }
            }

            EndCheckAll(jobContext);
        }





        /// <summary>
        /// 检查具体的一单
        /// </summary>
        /// <param name="soEntity4FPCheck"></param>
        private static void CheckSingle(SOEntity4FPEntity soEntity4FPCheck)
        {
            List<string> tmpResons = new List<string>();

            FPStatus tmpFPstatus = FPStatus.Normal;
            bool IsMarkRed = false; //是否飘红可疑标记

            if (IsCheckKeYi)
            {
                #region 检查疑单

                if (SOFPCheckDA.IsSpiteCustomer(soEntity4FPCheck.CustomerSysNo, CompanyCode))
                {

                    tmpFPstatus = FPStatus.Suspect;
                    tmpResons.Add("此用户是恶意用户，之前有过不良的购物记录");
                    InsertKFC(soEntity4FPCheck);
                }
                else
                {
                    // 如果是货到付款
                    bool isRej = false;
                    if (payTypeList.Exists(item => item.SysNo == soEntity4FPCheck.PayTypeSysNo && item.IsPayWhenRecv == 1))
                    {

                        if (CommonDA.IsNewCustomer(soEntity4FPCheck.CustomerSysNo))
                        {
                            if (SOFPCheckDA.IsRejectionCustomer(soEntity4FPCheck.ShippingAddress, soEntity4FPCheck.ReceiveCellPhone, soEntity4FPCheck.ReceivePhone, CompanyCode))
                            {
                                isRej = true;
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                            }
                            else if (soEntity4FPCheck.ReceivePhone.IndexOf(",") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split(',');
                                foreach (string sp in singlePhone)
                                {
                                    if (SOFPCheckDA.IsRejectionCustomer("", "", sp, CompanyCode))
                                    {
                                        isRej = true;
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                                        break;
                                    }
                                }
                            }
                            else if (soEntity4FPCheck.ReceivePhone.IndexOf("，") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split('，');
                                foreach (string sp in singlePhone)
                                {
                                    if (SOFPCheckDA.IsRejectionCustomer("", "", sp, CompanyCode))
                                    {
                                        isRej = true;
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (SOFPCheckDA.IsRejectionCustomer(soEntity4FPCheck.CustomerSysNo))
                            {
                                isRej = true;
                                tmpFPstatus = FPStatus.Suspect;
                                if (SOFPCheckDA.IsNewRejectionCustomerB(soEntity4FPCheck.CustomerSysNo, CompanyCode))
                                {
                                    tmpResons.Add("新用户，该用户之前有过拒收货物的订单记录，请谨慎处理");
                                }
                                else
                                {
                                    tmpResons.Add("该用户拒收订单的比例超过限度，请谨慎处理");
                                }
                            }
                        }

                        if (isRej != true)
                        {
                            // 是新客户并且没有通过手机验证
                            if (CommonDA.IsNewCustomer(soEntity4FPCheck.CustomerSysNo) && !CommonDA.IsTelPhoneCheck(soEntity4FPCheck.CustomerSysNo))
                            {

                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新客户");
                                tmpResons.Add("没有通过手机验证的货到付款订单");

                                if (soEntity4FPCheck.SOAmt > 500)
                                {
                                    tmpFPstatus = FPStatus.Suspect;//标为可疑单
                                    tmpResons.Add("订单金额在500元以上");
                                    IsMarkRed = true; //设置可疑信息飘红
                                }
                                else
                                {
                                    tmpFPstatus = FPStatus.Suspect;//标为可疑单
                                    tmpResons.Add("订单金额在0-500元");
                                }

                                if (SOQueryDA.GetSOCount4OneDay(soEntity4FPCheck.CustomerSysNo, CompanyCode) > 1)
                                {
                                    tmpFPstatus = FPStatus.Suspect;
                                    tmpResons.Add("一天之中存在多个不同的收货地址的订单信息");
                                    IsMarkRed = true;
                                }

                                // 将当前订单信息写入可疑表
                                // InsertKFC(x);
                            }
                            else if (AutoRMACustomerSysNos.Find(z => { return z == soEntity4FPCheck.CustomerSysNo; }) > 0)
                            {
                                //一个月内物流拒收过此用户的订单标为可疑单
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("一个月内物流拒收过此用户的订单");
                                IsMarkRed = true;
                            }
                        }
                        else
                            InsertKFC(soEntity4FPCheck);
                    }
                    else  //款到发货
                    {
                        if (SOFPCheckDA.IsNewOccupyStockCustomerA(soEntity4FPCheck.CustomerSysNo))
                        {
                            if (SOFPCheckDA.IsOccupyStockCustomer(soEntity4FPCheck.ShippingAddress, soEntity4FPCheck.ReceiveCellPhone, soEntity4FPCheck.ReceivePhone, CompanyCode))
                            {
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                InsertKFC(soEntity4FPCheck);
                            }
                            else if (soEntity4FPCheck.ReceivePhone.IndexOf(",") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split(',');
                                foreach (string sp in singlePhone)
                                {
                                    if (SOFPCheckDA.IsOccupyStockCustomer("", "", sp, CompanyCode))
                                    {
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                        InsertKFC(soEntity4FPCheck);
                                        break;
                                    }
                                }
                            }
                            else if (soEntity4FPCheck.ReceivePhone.IndexOf("，") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split('，');
                                foreach (string sp in singlePhone)
                                {
                                    if (SOFPCheckDA.IsOccupyStockCustomer("", "", sp, CompanyCode))
                                    {
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                        InsertKFC(soEntity4FPCheck);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (SOFPCheckDA.IsOccupyStockCustomer(soEntity4FPCheck.CustomerSysNo))
                            {
                                InsertKFC(soEntity4FPCheck);
                                tmpFPstatus = FPStatus.Suspect;
                                if (SOFPCheckDA.IsNewOccupyStockCustomerB(soEntity4FPCheck.CustomerSysNo))
                                {
                                    tmpResons.Add("新用户，该用户之前有过连续作废订单记录，请谨慎处理");
                                }
                                else
                                {
                                    tmpResons.Add("该用户之前有过连续作废订单记录，请谨慎处理");
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #region 获取订单的Item列表信息
            List<SOEntity4CheckEntity> tmpSingleSO = SOFPCheckDA.GetSingleSO4FPCheck(soEntity4FPCheck.SysNo);
            if (tmpSingleSO.Count == 0)
            {
                tmpResons.Add("空单");
            }
            #endregion

            if (IsCheckChuanHuo
                && tmpFPstatus != FPStatus.Suspect)//可疑优先于炒货,如果已经确认是可疑单,就不用再验证串货了
            {
                if (tmpSingleSO.Count > 0)
                {
                    string currentSOIPAddress = tmpSingleSO[0].IPAddress;
                    DateTime currentSOCreateTime = tmpSingleSO[0].CreateTime;


                    #region 检查串货订单
                    //检查PRD
                    foreach (SOEntity4CheckEntity item in tmpSingleSO)
                    {
                        //仅在遇到要求串货检查的Item时才去检测,减少数据库的访问次数
                        if (ChuanHuoProductNoList.Find(t => { return (t == item.ProductID); }) != null)
                        {
                            List<int> queryResultChuanHuoSOSysNosByProduct = SOFPCheckDA.GetChuanHuoSOSysNoListByProduct(item.ProductSysNo, currentSOIPAddress, currentSOCreateTime, CompanyCode);
                            if (queryResultChuanHuoSOSysNosByProduct.Count > 1)
                            {
                                tmpFPstatus = FPStatus.ChuanHuo;
                                break;
                            }
                        }
                    }

                    //检查C3小类
                    List<int> chuanHuoSOSysNosByC3 = (from a in ChuanHuoC3SysNoList
                                                      from b in tmpSingleSO
                                                      where a == b.C3SysNo
                                                      select a).ToList();

                    foreach (int c3No in chuanHuoSOSysNosByC3)
                    {
                        List<int> queryResultChuanHuoSOSysNosByC3 = SOFPCheckDA.GetChuanHuoSOSysNoListByC3(c3No, currentSOIPAddress, currentSOCreateTime, CompanyCode);
                        if (queryResultChuanHuoSOSysNosByC3.Count > 1)
                        {
                            tmpFPstatus = FPStatus.ChuanHuo;
                            break;
                        }
                    }

                    if (tmpFPstatus == FPStatus.ChuanHuo)
                    {
                        tmpResons.Add("串货订单");
                    }


                }

                    #endregion
            }

            if (IsCheckChongFu)
            {
                #region 检查重复订单
                if (tmpSingleSO.Count > 0)
                {
                    foreach (SOEntity4CheckEntity item in tmpSingleSO)
                    {
                        List<int> tmpDuplicateSOSysNos = SOFPCheckDA.GetDuplicatSOSysNoList(item.SOSysNo, item.ProductSysNo, item.CustomerSysNo, item.CreateTime, CompanyCode);
                        if (tmpDuplicateSOSysNos.Count > 1)
                        {
                            if (!tmpResons.Contains("重复订单"))
                            {
                                tmpResons.Add("重复订单");
                            }

                            StringBuilder tmpDuplicateSOSysNosb = new StringBuilder();
                            string tmpDuplicateSOSysNoString = string.Empty;

                            foreach (int y in tmpDuplicateSOSysNos)
                            {
                                tmpDuplicateSOSysNosb.Append(string.Format("{0},", y));
                                if (tmpDuplicateSOSysNosb.Length > 400)
                                    break;
                            }

                            tmpDuplicateSOSysNoString = tmpDuplicateSOSysNosb.ToString().TrimEnd(",".ToCharArray());

                            SOFPCheckDA.UpdateMarkException(tmpDuplicateSOSysNoString, item.ProductSysNo, tmpDuplicateSOSysNoString);
                        }
                    }
                }
                #endregion
            }

            if (IsCheckChaoHuo
                && (!string.IsNullOrEmpty(soEntity4FPCheck.ReceiveCellPhone) || !string.IsNullOrEmpty(soEntity4FPCheck.ReceivePhone))
                )
            {
                #region 检查炒货订单

                #region PreData
                int SysNoCount = 3;//订单数量 (默认需要大于的最小订单数量为1)
                int hoursLimit = 24; //需从配置表中读取

                FPCheckItemEntity FPCheckItemEntityTemp = FPCheckItemList.Find(t => { return (t.Description == "小时之内订单数量大于" && t.Status == 0); });
                if (FPCheckItemEntityTemp != null)
                {
                    if (FPCheckItemEntityTemp.ReferenceContent != string.Empty)
                    {
                        hoursLimit = Convert.ToInt32(FPCheckItemEntityTemp.ReferenceContent.Split('|')[0].ToString());
                        SysNoCount = Convert.ToInt32(FPCheckItemEntityTemp.ReferenceContent.Split('|')[1].ToString());
                    }
                }

                int? PointPromotionFlag = null;//判断是否进行订单优惠券积分的,检测标识符,赋任何值表示此条件有效;
                int? ShipPriceFlag = null;//判断是否进行订单中运费金额为0的,检测标识符,赋任何值表示此条件有效;
                int? IsVATFlag = null;//判断是否进行订单中勾选开具增值税发票的,检测标识符,赋任何值表示此条件有效;

                if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中使用蛋券/积分等优惠" && t.Status == 0); }) != null)
                {
                    PointPromotionFlag = 1;
                }
                if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中运费金额为0" && t.Status == 0); }) != null)
                {
                    ShipPriceFlag = 1;
                }
                if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中勾选开具增值税发票" && t.Status == 0); }) != null)
                {
                    IsVATFlag = 1;
                }
                #endregion

                List<int> queryResultForChaoHuoList = SOFPCheckDA.GetChaoHuoSOSysNoList(
                      soEntity4FPCheck.ReceiveCellPhone
                    , soEntity4FPCheck.ReceivePhone
                    , hoursLimit
                    , soEntity4FPCheck.SOCreateTime
                    , PointPromotionFlag
                    , ShipPriceFlag
                    , IsVATFlag
                    , CompanyCode
                    );

                if (queryResultForChaoHuoList.Count > SysNoCount)
                {
                    if (tmpResons.Contains("重复订单"))
                    {
                        //    tmpResons.Clear();
                        //    tmpResons.Add("重复订单");
                        //}
                        //else
                        //{
                        tmpResons.Clear();
                    }

                    //tmpResons.Add("炒货订单");
                    tmpFPstatus = FPStatus.ChaoHuo;

                    #region //收集炒货的订单号
                    int lenReasons = 0;

                    tmpResons.ForEach(x =>
                    {
                        lenReasons += x.Length;
                    }
                    );

                    lenReasons += tmpResons.Count;

                    StringBuilder tmpChaoHuoSOSysNosb = new StringBuilder();
                    string tmpChaoHuoSysNoString = string.Empty;
                    string ChaoHuoSysNo;

                    foreach (int x in queryResultForChaoHuoList)
                    {
                        ChaoHuoSysNo = string.Format("{0},", x);
                        if ((lenReasons + tmpChaoHuoSOSysNosb.Length + ChaoHuoSysNo.Length - 1) > 200)
                        {
                            break;
                        }
                        tmpChaoHuoSOSysNosb.Append(ChaoHuoSysNo);
                    }

                    tmpChaoHuoSysNoString = tmpChaoHuoSOSysNosb.ToString().TrimEnd(",".ToCharArray());

                    tmpResons.Add(tmpChaoHuoSysNoString);
                    #endregion

                    foreach (int sysNo in queryResultForChaoHuoList)
                    {
                        SOFPCheckDA.UpdateMarkFPStatus(sysNo, (int)tmpFPstatus, tmpChaoHuoSysNoString, false);
                    }
                }
                #endregion
            }

            #region 检查本地仓断货订单
            string localWH = CommonDA.GetlocalWHByAreaSysNo(soEntity4FPCheck.ReceiveAreaSysNo, CompanyCode);
            if (!string.IsNullOrEmpty(localWH)
                && OutStockList.Exists(os => os == localWH)
                && CommonDA.ExistsNotLocalWH(soEntity4FPCheck.SysNo, localWH, CompanyCode))
            {
                CommonDA.UpdateLocalWHMark(soEntity4FPCheck.SysNo, localWH, 1);
            }
            #endregion

            if (tmpFPstatus == FPStatus.Normal
                && tmpResons.Count == 0)
            {
                tmpResons.Add("正常单");
            }

            string temReson = string.Empty;
            foreach (string reson in tmpResons)
            {
                temReson += reson + ";";
            }
            temReson = temReson.TrimEnd(';');

            SOFPCheckDA.UpdateMarkFPStatus(soEntity4FPCheck.SysNo, (int)tmpFPstatus, temReson, IsMarkRed);

            WriteLog(string.Format("{0} {1}", soEntity4FPCheck.SysNo, temReson));
            GC.Collect();
        }

        #endregion Entry Point




        /// <summary>
        /// 本批全部检查结束
        /// </summary>
        /// <param name="entity"></param>
        private static void EndCheckAll(JobContext jobContext)
        {
            jobContext.Message += "本次检查结束:" + DateTime.Now.ToString() + Environment.NewLine;
            WriteLog("本次检查结束:" + DateTime.Now.ToString());
        }

        /// <summary>
        /// 本批全部检查结束
        /// </summary>
        /// <param name="entity"></param>
        private static void StartCheckAll(JobContext jobContext)
        {
            WriteLog("开始FP检查:" + DateTime.Now.ToString());
        }

        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

        private static void InsertKFC(SOEntity4FPEntity entity)
        {
            IMaintainKFCV31 service = ServiceBroker.FindService<IMaintainKFCV31>();

            KnownFraudCustomerV31 message = new KnownFraudCustomerV31
            {
                Body = new KnownFraudCustomerMsg
                {
                    BrowseInfo = entity.BrowseInfo,
                    CreateDate = DateTime.Now,
                    CreateUserName = "OrderJob",
                    CustomerSysNo = entity.CustomerSysNo,
                    EmailAddress = entity.EmailAddress,
                    FraudType = 1,
                    IPAddress = entity.IPAddress,
                    MobilePhone = entity.MobilePhone,
                    ShippingAddress = entity.ShippingAddress,
                    ShippingContact = entity.ShippingContact,
                    Status = 0,
                    Telephone = entity.Telephone
                },
                Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
                {
                    OperationUser = new OperationUser(UserDisplayName, UserLoginName, StoreSourceDirectoryKey, CompanyCode),
                    CompanyCode = CompanyCode,
                    StoreCompanyCode = StoreCompanyCode
                }
            };
            try
            {
                message = service.InsertKnownFraudCustomer(message);
            }
            finally
            {
                ServiceAdapterHelper.DealServiceFault(message);
            }

        }
       
       // #region
       // /// <summary>
       // /// 自动审核一次最多提取的单数
       // /// </summary>
       // private static int TopCount = 2000;

       // /// <summary>
       // /// 是否进行可疑订单检测
       // /// </summary>
       // private static bool IsCheckKeYi = false;

       // /// <summary>
       // /// 是否进行串货检测
       // /// </summary> 
       // private static bool IsCheckChuanHuo = false;

       // /// <summary>
       // /// 是否进行炒货检测
       // /// </summary>
       // private static bool IsCheckChaoHuo = false;

       // /// <summary>
       // /// 是否进行重复订单检测
       // /// </summary>
       // private static bool IsCheckChongFu = false;

       // /// <summary>
       // /// 一个月内被物流拒收过单拒的客户编号
       // /// </summary>
       // private static List<int> AutoRMACustomerSysNos;

       // /// <summary>
       // /// 业务日志文件
       // /// </summary>
       // private static string BizLogFile;

       // /// <summary>
       // /// 支付方式列表
       // /// </summary>
       // private static List<PayTypeEntity> payTypeList;

       // //断货订单支持仓库
       // private static List<string> OutStockList = new List<string>();
       // #endregion

       // //以下五项为JOB中调用WCF服务时所使用到的信息
       // private static string UserDisplayName;
       // private static string UserLoginName;
       // private static string CompanyCode;
       // private static string StoreCompanyCode;
       // private static string StoreSourceDirectoryKey;

       // /// <summary>
       // /// 从配置文件获取为邮件发送JOB中调用WCF服务时所使用到的信息
       // /// </summary>
       // /// <returns></returns>
       // public static void GetOprationUserInfo()
       // {
       //     UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
       //     UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
       //     CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
       //     StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
       //     StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
       // }


       // /// <summary>
       // ///  获取FPCheck检查项的具体明细项信息
       // /// </summary>
       // private static List<FPCheckItemEntity> FPCheckItemList;

       // /// <summary>
       // /// 获取FPCheck检查项信息
       // /// </summary>
       // private static List<FPCheckMasterEntity> FPCheckMasterList;

       // /// <summary>
       // /// 验证串货的产品编号列表
       // /// </summary>
       // private static List<string> ChuanHuoProductNoList;

       // /// <summary>
       // /// 验证串货的C3编号列表
       // /// </summary>
       // private static List<int> ChuanHuoC3SysNoList;


       // /// <summary>
       // /// 获取一次审核最多的记录数
       // /// </summary>
       // /// <returns></returns>
       // private static int GetFPCheckTopCount()
       // {
       //     string tmpAuditUserSysNo = ConfigurationManager.AppSettings["FPCheckTopCount"];
       //     int topCount = int.Parse(tmpAuditUserSysNo);

       //     return (topCount > 0) ? topCount : 2000;
       // }

       // /// <summary>
       // /// 获取是否可疑订单的验证标识符
       // /// </summary>
       // /// <returns></returns>
       // private static bool GetFPCheckKeYiFlag()
       // {
       //     bool result = false;
       //     if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x => { return (x.CheckType == "KY" && x.Status == 0); }) != null)
       //     {
       //         result = true;
       //     }
       //     return result;
       // }

       // /// <summary>
       // /// 获取是否串单的验证标识符
       // /// </summary>
       // /// <returns></returns>
       // private static bool GetFPCheckChuanHuoFlag()
       // {
       //     bool result = false;
       //     if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x => { return (x.CheckType == "CH" && x.Status == 0); }) != null)
       //     {
       //         result = true;
       //     }

       //     if (result)
       //     {
       //         ChuanHuoProductNoList = new List<string>();
       //         FPCheckItemList.FindAll(x =>
       //         {
       //             return (x.ReferenceType == "PID" && x.Status == 0);
       //         }).ForEach(x =>
       //         {
       //             ChuanHuoProductNoList.Add(x.ReferenceContent);
       //         });

       //         ChuanHuoC3SysNoList = new List<int>();
       //         FPCheckItemList.FindAll(x =>
       //         {
       //             return (x.ReferenceType == "PC3" && x.Status == 0);
       //         }).ForEach(x =>
       //         {
       //             ChuanHuoC3SysNoList.Add(Convert.ToInt32(x.Description.Trim()));
       //         });
       //     }

       //     return result;
       // }

       // /// <summary>
       // /// 获取是否进行炒货单验证的标识符
       // /// </summary>
       // /// <returns></returns>
       // private static bool GetFPCheckChaoHuoFlag()
       // {
       //     bool result = false;
       //     if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x =>
       //     {
       //         return (x.CheckType == "CC" && x.Status == 0);
       //     }) != null)
       //     {
       //         result = true;
       //     }
       //     return result;
       // }


       ///// <summary>
       ///// 获取是否检测重复订单标识符
       ///// </summary>
       ///// <returns></returns>
       // private static bool GetCheckChongFuFlag(){
       //     //判断是否进行炒货检测
       //     bool result = false;
       //     if (FPCheckMasterList.Count > 0 && FPCheckMasterList.Find(x =>
       //     {
       //         return (x.CheckType == "CF" && x.Status == 0);
       //     }) != null) {
       //         result = true;
       //     }
       //     return result;
       //  }
       // /// <summary>
       // /// 验证FP
       // /// </summary>
       // /// <param name="jobContext"></param>
       // public static void Check(JobContext jobContext) {
       //     GetOprationUserInfo();
       // //提前取得所有支付方式的列表
       //     payTypeList = CommonDA.GetPayTypeList(CompanyCode);

       //     BizLogFile = jobContext.Properties["BizLog"];

       //     StartCheckAll(jobContext);

       //     //提前取得一次验单的最多数目
       //     TopCount = GetFPCheckTopCount();
       //     //提取验证的订单列表
       //     List<SOEntity4FPEntity> tmpSOENtityList = SOFPCheckDA.GetSOList4FPCheck(TopCount, CompanyCode);
       //     if (null == tmpSOENtityList || tmpSOENtityList.Count == 0)
       //     {
       //         jobContext.Message += "没有附合条件的订单记录" + Environment.NewLine;
       //         WriteLog("没有附合条件的订单记录");
       //         return;
       //     }
       //     else {
       //         jobContext.Message += "本次提取订单记录" + tmpSOENtityList.Count + "条." + Environment.NewLine;
       //     }
            
       //     //提前获取断货支持仓库
       //     string outStockStr = ConfigurationManager.AppSettings["OUT_STOCK"];
       //     string[] outStockArray = outStockStr.Split(',');
       //     foreach(string os in outStockArray){
       //         OutStockList.Add(os);
       //     }
       //     //提前取得一个月内被物流拒收过单拒的客户编号
       //     AutoRMACustomerSysNos = SOFPCheckDA.GetAutoRMACustomerSysNos(CompanyCode);


       //     //提取现有恶意用户列表
       //     //spiteList = SOFPCheckDA.GetDubiousCustomersByCat(2);

       //     //提取现有拒收用户列表
       //     //rejList = SOFPCheckDA.GetDubiousCustomersByCat(1);

       //     //提取现有占库存用户列表
       //     //occupyList = SOFPCheckDA.GetDubiousCustomersByCat(0);

       //     //提取FP检查项列表
       //     FPCheckMasterList = SOFPCheckDA.GetFPCheckMasterList(CompanyCode);

       //     //提取FP检查项列表明细项
       //     FPCheckItemList = SOFPCheckDA.GetFPCheckItemList(CompanyCode);

       //     //提前取得是否进行可疑订单检测是标识符
       //     IsCheckKeYi = GetFPCheckKeYiFlag();

       //     //提前取得是否进行串单检测标识符
       //     IsCheckChuanHuo = GetFPCheckChuanHuoFlag();

       //     //提前取得是否进行炒货订单检测标识符
       //     IsCheckChaoHuo = GetFPCheckChaoHuoFlag();

       //     //提前获取是否进行重复订单检测标识符
       //     IsCheckChongFu = GetCheckChongFuFlag();

       //     //遍历验证订单的列表
       //     foreach(SOEntity4FPEntity  x in tmpSOENtityList){
       //         try {
       //             string FromLinkSource = SOFPCheckDA.GetFromLinkSource(x.CustomerSysNo);
       //             //阿斯利康用户、盛大用户、柯尼卡美能达用户、理光用户、Intel用户无需审核
       //             if ((FromLinkSource != null && FromLinkSource == INTERORDER) ||
       //                 (x.CustomerID.StartsWith("AstraZeneca-")) ||
       //                 (x.CustomerID.StartsWith("Shanda")) ||
       //                 (x.CustomerID.StartsWith("konicaminolta")) ||
       //                 (x.CustomerID.StartsWith("Ricoh-")))
       //               {
       //                   continue;
       //             }
       //             CheckSingle(x);
       //         }catch(Exception ex){
       //             jobContext.Message += string.Format("订单{0}出现异常:{1}\r\n", x.SysNo, ex.Message);
       //         }
                
       //     }
       //     EndCheckAll(jobContext);
       // }
       




       // /// <summary>
       // /// 检查具体的一单
       // /// </summary>
       // /// <param name="soEntity4FPCheck"></param>
       // private static void CheckSingle(SOEntity4FPEntity soEntity4FPCheck)
       // {
       //     List<string> tmpResons = new List<string>();

       //     FPStatus tmpFPstatus = FPStatus.Normal;
       //     bool IsMarkRed = false; //是否飘红可疑标记

       //     if (IsCheckKeYi)
       //     {
       //         #region 检查疑单

       //         if (SOFPCheckDA.IsSpiteCustomer(soEntity4FPCheck.CustomerSysNo, CompanyCode))
       //         {

       //             tmpFPstatus = FPStatus.Suspect;
       //             tmpResons.Add("此用户是恶意用户，之前有过不良的购物记录");
       //             InsertKFC(soEntity4FPCheck);
       //         }
       //         else
       //         {
       //             // 如果是货到付款
       //             bool isRej = false;
       //             if (payTypeList.Exists(item => item.SysNo == soEntity4FPCheck.PayTypeSysNo && item.IsPayWhenRecv == 1))
       //             {

       //                 if (CommonDA.IsNewCustomer(soEntity4FPCheck.CustomerSysNo))
       //                 {
       //                     if (SOFPCheckDA.IsRejectionCustomer(soEntity4FPCheck.ShippingAddress, soEntity4FPCheck.ReceiveCellPhone,
       //                         soEntity4FPCheck.ReceivePhone, CompanyCode))
       //                     {
       //                         isRej = true;
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
       //                     }
       //                     else if (soEntity4FPCheck.ReceivePhone.IndexOf(",") >= 0)
       //                     {
       //                         string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split(',');
       //                         foreach (string sp in singlePhone)
       //                         {
       //                             if (SOFPCheckDA.IsRejectionCustomer("", "", sp, CompanyCode))
       //                             {
       //                                 isRej = true;
       //                                 tmpFPstatus = FPStatus.Suspect;
       //                                 tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
       //                                 break;
       //                             }
       //                         }
       //                     }
       //                     else if (soEntity4FPCheck.ReceivePhone.IndexOf("，") >= 0)
       //                     {
       //                         string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split('，');
       //                         foreach (string sp in singlePhone)
       //                         {
       //                             if (SOFPCheckDA.IsRejectionCustomer("", "", sp, CompanyCode))
       //                             {
       //                                 isRej = true;
       //                                 tmpFPstatus = FPStatus.Suspect;
       //                                 tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
       //                                 break;
       //                             }
       //                         }
       //                     }
       //                 }
       //                 else
       //                 {
       //                     if (SOFPCheckDA.IsRejectionCustomer(soEntity4FPCheck.CustomerSysNo))
       //                     {
       //                         isRej = true;
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         if (SOFPCheckDA.IsNewRejectionCustomerB(soEntity4FPCheck.CustomerSysNo, CompanyCode))
       //                         {
       //                             tmpResons.Add("新用户，该用户之前有过拒收货物的订单记录，请谨慎处理");
       //                         }
       //                         else
       //                         {
       //                             tmpResons.Add("该用户拒收订单的比例超过限度，请谨慎处理");
       //                         }
       //                     }
       //                 }

       //                 //新用户而手机没有通过验证 
       //                 if (isRej != true)
       //                 {
       //                     if(CommonDA.IsNewCustomer(soEntity4FPCheck.CustomerSysNo)&&!CommonDA.IsTelPhoneCheck(soEntity4FPCheck.CustomerSysNo)){
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         tmpResons.Add("新客户");
       //                         tmpResons.Add("没有通过手机验证的货到付款订单");
       //                         if (soEntity4FPCheck.SOAmt > 500)
       //                         {
       //                             tmpFPstatus = FPStatus.Suspect;
       //                             tmpResons.Add("订单金额在500元以上");
       //                             IsMarkRed = true;
       //                         }
       //                         else {
       //                             tmpFPstatus = FPStatus.Suspect;
       //                             tmpResons.Add("订单金额在0—500元");
       //                         }
       //                         if(SOQueryDA.GetSOCount4OneDay(soEntity4FPCheck.CustomerSysNo,CompanyCode)>1){
       //                             tmpFPstatus = FPStatus.Suspect;
       //                             tmpResons.Add("一天之中存在多个不同的收货地址的订单信息");
       //                             IsMarkRed = true;
       //                         }
       //                     }
       //                     else if (AutoRMACustomerSysNos.Find(z => { return z == soEntity4FPCheck.CustomerSysNo; })>0) {
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         tmpResons.Add("一个月内物流拒收过此用户的订单");
       //                         IsMarkRed = true;  //设置可疑信息标红
       //                     }
       //                     else
       //                     InsertKFC(soEntity4FPCheck);
       //                 }


       //                 else
       //                     InsertKFC(soEntity4FPCheck);
       //             }
       //             else  //款到发货
       //             {
       //                 if (SOFPCheckDA.IsNewOccupyStockCustomerA(soEntity4FPCheck.CustomerSysNo))
       //                 {
       //                     if (SOFPCheckDA.IsOccupyStockCustomer(soEntity4FPCheck.ShippingAddress, soEntity4FPCheck.ReceiveCellPhone, soEntity4FPCheck.ReceivePhone, CompanyCode))
       //                     {
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
       //                         InsertKFC(soEntity4FPCheck);
       //                     }
       //                     else if (soEntity4FPCheck.ReceivePhone.IndexOf(",") >= 0)
       //                     {
       //                         string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split(',');
       //                         foreach (string sp in singlePhone)
       //                         {
       //                             if (SOFPCheckDA.IsOccupyStockCustomer("", "", sp, CompanyCode))
       //                             {
       //                                 tmpFPstatus = FPStatus.Suspect;
       //                                 tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
       //                                 InsertKFC(soEntity4FPCheck);
       //                                 break;
       //                             }
       //                         }
       //                     }
       //                     else if (soEntity4FPCheck.ReceivePhone.IndexOf("，") >= 0)
       //                     {
       //                         string[] singlePhone = soEntity4FPCheck.ReceivePhone.Split('，');
       //                         foreach (string sp in singlePhone)
       //                         {
       //                             if (SOFPCheckDA.IsOccupyStockCustomer("", "", sp, CompanyCode))
       //                             {
       //                                 tmpFPstatus = FPStatus.Suspect;
       //                                 tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
       //                                 InsertKFC(soEntity4FPCheck);
       //                                 break;
       //                             }
       //                         }
       //                     }
       //                 }
       //                 else
       //                 {
       //                     if (SOFPCheckDA.IsOccupyStockCustomer(soEntity4FPCheck.CustomerSysNo))
       //                     {
       //                         InsertKFC(soEntity4FPCheck);
       //                         tmpFPstatus = FPStatus.Suspect;
       //                         if (SOFPCheckDA.IsNewOccupyStockCustomerB(soEntity4FPCheck.CustomerSysNo))
       //                         {
       //                             tmpResons.Add("新用户，该用户之前有过连续作废订单记录，请谨慎处理");
       //                         }
       //                         else
       //                         {
       //                             tmpResons.Add("该用户之前有过连续作废订单记录，请谨慎处理");
       //                         }
       //                     }
       //                 }
       //             }
       //         }

       //         #endregion
       //     }


       //     #region 获取订单的Item列表信息
       //     List<SOEntity4CheckEntity> tmpSingleSO = SOFPCheckDA.GetSingleSO4FPCheck(soEntity4FPCheck.SysNo);
       //     if (tmpSingleSO.Count == 0)
       //     {
       //         tmpResons.Add("空单");
       //     }
       //     #endregion

       //     if (IsCheckChuanHuo
       //         && tmpFPstatus != FPStatus.Suspect)//可疑优先于炒货,如果已经确认是可疑单,就不用再验证串货了
       //     {
       //         if (tmpSingleSO.Count > 0)
       //         {
       //             string currentSOIPAddress = tmpSingleSO[0].IPAddress;
       //             DateTime currentSOCreateTime = tmpSingleSO[0].CreateTime;


       //             #region 检查串货订单
       //             //检查PRD
       //             foreach (SOEntity4CheckEntity item in tmpSingleSO)
       //             {
       //                 //仅在遇到要求串货检查的Item时才去检测,减少数据库的访问次数
       //                 if (ChuanHuoProductNoList.Find(t => { return (t == item.ProductID); }) != null)
       //                 {
       //                     List<int> queryResultChuanHuoSOSysNosByProduct = SOFPCheckDA.GetChuanHuoSOSysNoListByProduct(item.ProductSysNo, currentSOIPAddress, currentSOCreateTime, CompanyCode);
       //                     if (queryResultChuanHuoSOSysNosByProduct.Count > 1)
       //                     {
       //                         tmpFPstatus = FPStatus.ChuanHuo;
       //                         break;
       //                     }
       //                 }
       //             }

       //             //检查C3小类
       //             List<int> chuanHuoSOSysNosByC3 = (from a in ChuanHuoC3SysNoList
       //                                               from b in tmpSingleSO
       //                                               where a == b.C3SysNo
       //                                               select a).ToList();

       //             foreach (int c3No in chuanHuoSOSysNosByC3)
       //             {
       //                 List<int> queryResultChuanHuoSOSysNosByC3 = SOFPCheckDA.GetChuanHuoSOSysNoListByC3(c3No, currentSOIPAddress, currentSOCreateTime, CompanyCode);
       //                 if (queryResultChuanHuoSOSysNosByC3.Count > 1)
       //                 {
       //                     tmpFPstatus = FPStatus.ChuanHuo;
       //                     break;
       //                 }
       //             }

       //             if (tmpFPstatus == FPStatus.ChuanHuo)
       //             {
       //                 tmpResons.Add("串货订单");
       //             }
       //         }

       //             #endregion
       //     }

       //     if (IsCheckChongFu)
       //     {
       //         #region 检查重复订单
       //         if (tmpSingleSO.Count > 0)
       //         {
       //             foreach (SOEntity4CheckEntity item in tmpSingleSO)
       //             {
       //                 List<int> tmpDuplicateSOSysNos = SOFPCheckDA.GetDuplicatSOSysNoList(item.SOSysNo, item.ProductSysNo, item.CustomerSysNo, item.CreateTime, CompanyCode);
       //                 if (tmpDuplicateSOSysNos.Count > 1)
       //                 {
       //                     if (!tmpResons.Contains("重复订单"))
       //                     {
       //                         tmpResons.Add("重复订单");
       //                     }

       //                     StringBuilder tmpDuplicateSOSysNosb = new StringBuilder();
       //                     string tmpDuplicateSOSysNoString = string.Empty;

       //                     foreach (int y in tmpDuplicateSOSysNos)
       //                     {
       //                         tmpDuplicateSOSysNosb.Append(string.Format("{0},", y));
       //                         if (tmpDuplicateSOSysNosb.Length > 400)
       //                             break;
       //                     }

       //                     tmpDuplicateSOSysNoString = tmpDuplicateSOSysNosb.ToString().TrimEnd(",".ToCharArray());

       //                     SOFPCheckDA.UpdateMarkException(tmpDuplicateSOSysNoString, item.ProductSysNo, tmpDuplicateSOSysNoString);
       //                 }
       //             }
       //         }
       //         #endregion
       //     }

       //     if (IsCheckChaoHuo
       //         && (!string.IsNullOrEmpty(soEntity4FPCheck.ReceiveCellPhone) || !string.IsNullOrEmpty(soEntity4FPCheck.ReceivePhone))
       //         )
       //     {
       //         #region 检查炒货订单

       //         #region PreData
       //         int SysNoCount = 3;//订单数量 (默认需要大于的最小订单数量为1)
       //         int hoursLimit = 24; //需从配置表中读取

       //         FPCheckItemEntity FPCheckItemEntityTemp = FPCheckItemList.Find(t => { return (t.Description == "小时之内订单数量大于" && t.Status == 0); });
       //         if (FPCheckItemEntityTemp != null)
       //         {
       //             if (FPCheckItemEntityTemp.ReferenceContent != string.Empty)
       //             {
       //                 hoursLimit = Convert.ToInt32(FPCheckItemEntityTemp.ReferenceContent.Split('|')[0].ToString());
       //                 SysNoCount = Convert.ToInt32(FPCheckItemEntityTemp.ReferenceContent.Split('|')[1].ToString());
       //             }
       //         }

       //         int? PointPromotionFlag = null;//判断是否进行订单优惠券积分的,检测标识符,赋任何值表示此条件有效;
       //         int? ShipPriceFlag = null;//判断是否进行订单中运费金额为0的,检测标识符,赋任何值表示此条件有效;
       //         int? IsVATFlag = null;//判断是否进行订单中勾选开具增值税发票的,检测标识符,赋任何值表示此条件有效;

       //         if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中使用蛋券/积分等优惠" && t.Status == 0); }) != null)
       //         {
       //             PointPromotionFlag = 1;
       //         }
       //         if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中运费金额为0" && t.Status == 0); }) != null)
       //         {
       //             ShipPriceFlag = 1;
       //         }
       //         if (FPCheckItemList.Count > 0 && FPCheckItemList.Find(t => { return (t.Description == "订单中勾选开具增值税发票" && t.Status == 0); }) != null)
       //         {
       //             IsVATFlag = 1;
       //         }
       //         #endregion

       //         List<int> queryResultForChaoHuoList = SOFPCheckDA.GetChaoHuoSOSysNoList(
       //               soEntity4FPCheck.ReceiveCellPhone
       //             , soEntity4FPCheck.ReceivePhone
       //             , hoursLimit
       //             , soEntity4FPCheck.SOCreateTime
       //             , PointPromotionFlag
       //             , ShipPriceFlag
       //             , IsVATFlag
       //             , CompanyCode
       //             );

       //         if (queryResultForChaoHuoList.Count > SysNoCount)
       //         {
       //             if (tmpResons.Contains("重复订单"))
       //             {
       //                 //    tmpResons.Clear();
       //                 //    tmpResons.Add("重复订单");
       //                 //}
       //                 //else
       //                 //{
       //                 tmpResons.Clear();
       //             }

       //             //tmpResons.Add("炒货订单");
       //             tmpFPstatus = FPStatus.ChaoHuo;

       //             #region //收集炒货的订单号
       //             int lenReasons = 0;

       //             tmpResons.ForEach(x =>
       //             {
       //                 lenReasons += x.Length;
       //             }
       //             );

       //             lenReasons += tmpResons.Count;

       //             StringBuilder tmpChaoHuoSOSysNosb = new StringBuilder();
       //             string tmpChaoHuoSysNoString = string.Empty;
       //             string ChaoHuoSysNo;

       //             foreach (int x in queryResultForChaoHuoList)
       //             {
       //                 ChaoHuoSysNo = string.Format("{0},", x);
       //                 if ((lenReasons + tmpChaoHuoSOSysNosb.Length + ChaoHuoSysNo.Length - 1) > 200)
       //                 {
       //                     break;
       //                 }
       //                 tmpChaoHuoSOSysNosb.Append(ChaoHuoSysNo);
       //             }

       //             tmpChaoHuoSysNoString = tmpChaoHuoSOSysNosb.ToString().TrimEnd(",".ToCharArray());

       //             tmpResons.Add(tmpChaoHuoSysNoString);
       //             #endregion

       //             foreach (int sysNo in queryResultForChaoHuoList)
       //             {
       //                 SOFPCheckDA.UpdateMarkFPStatus(sysNo, (int)tmpFPstatus, tmpChaoHuoSysNoString, false);
       //             }
       //         }
       //         #endregion
       //     }

       //     #region 检查本地仓断货订单
       //     string localWH = CommonDA.GetlocalWHByAreaSysNo(soEntity4FPCheck.ReceiveAreaSysNo, CompanyCode);
       //     if (!string.IsNullOrEmpty(localWH)
       //         && OutStockList.Exists(os => os == localWH)
       //         && CommonDA.ExistsNotLocalWH(soEntity4FPCheck.SysNo, localWH, CompanyCode))
       //     {
       //         CommonDA.UpdateLocalWHMark(soEntity4FPCheck.SysNo, localWH, 1);
       //     }
       //     #endregion

       //     if (tmpFPstatus == FPStatus.Normal
       //         && tmpResons.Count == 0)
       //     {
       //         tmpResons.Add("正常单");
       //     }

       //     string temReson = string.Empty;
       //     foreach (string reson in tmpResons)
       //     {
       //         temReson += reson + ";";
       //     }
       //     temReson = temReson.TrimEnd(';');

       //     SOFPCheckDA.UpdateMarkFPStatus(soEntity4FPCheck.SysNo, (int)tmpFPstatus, temReson, IsMarkRed);

       //     WriteLog(string.Format("{0} {1}", soEntity4FPCheck.SysNo, temReson));
       //     GC.Collect();
       // }

       // private static void  StartCheckAll(JobContext  jobContext){
       //     WriteLog("开始FP检查" + DateTime.Now.ToString());
       // }

       // private static void EndCheckAll(JobContext  jobContext){
       //     jobContext.Message += "本次检查结束:" + DateTime.Now.ToString() + Environment.NewLine;
       //     WriteLog("本次检查结束:" + DateTime.Now.ToString());
       // }

      
       // private static void WriteLog(string content)
       // {
       //     Log.WriteLog(content, BizLogFile);
       //     Console.WriteLine(content);
       // }

       // private static void InsertKFC(SOEntity4FPEntity  entity){
       //     IMaintainKFCV31 service = ServiceBroker.FindService<IMaintainKFCV31>();
       //     KnownFraudCustomerV31 message = new KnownFraudCustomerV31
       //     {
       //         Body = new KnownFraudCustomerMsg
       //         {
       //             BrowseInfo = entity.BrowseInfo,
       //             CreateDate = DateTime.Now,
       //             CreateUserName = "OrderJob",
       //             CustomerSysNo = entity.CustomerSysNo,
       //             EmailAddress = entity.EmailAddress,
       //             FraudType = 1,
       //             IPAddress = entity.IPAddress,
       //             MobilePhone = entity.MobilePhone,
       //             ShippingAddress = entity.ShippingAddress,
       //             ShippingContact = entity.ShippingContact,
       //             Status = 0,
       //             Telephone = entity.Telephone
       //         },
       //         Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
       //         {
       //             OperationUser = new OperationUser(UserDisplayName, UserLoginName, StoreSourceDirectoryKey, CompanyCode),
       //             CompanyCode = CompanyCode,
       //             StoreCompanyCode=StoreCompanyCode
       //         }
       //     };
       //     try {
       //         message = service.InsertKnownFraudCustomer(message);
       //     }
       //     finally {
       //         ServiceAdapterHelper.DealServiceFault(message);
       //     }
       // }

    }
}
