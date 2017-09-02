using System;
using System.Collections.Generic;
using System.Configuration;
using IPP.OrderMgmt.JobV31.AppEnum;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;
using IPP.OrderMgmt.JobV31.Dac.AutoAudit;
using IPP.OrderMgmt.JobV31.Dac.FPCheck;
using IPP.OrderMgmt.JobV31.Dac.Common;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.DataContracts;
using Newegg.Oversea.Framework.Contract;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.ServiceContracts;
using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.DataContracts;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;

namespace IPP.OrderMgmt.JobV31.Biz.AutoAudit
{
    public class SOAutoAuditBP
    {
        #region PreData List
        /// <summary>
        /// 恶意客户列表
        /// </summary>
        private static List<CustomerEntity> malevolenceCustomers;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        private static List<PayTypeEntity> payTypeList;
        /// <summary>
        /// 配送方式列表
        /// </summary>
        private static List<ShipTypeInfo> shipTypeList;
        /// <summary>
        /// 缺货的待审核且经过FP处理过的订单系统编号列表
        /// </summary>
        private static List<int> OOSOSysNos;
        /// <summary>
        /// 含有C3指定类产品的订单系统编号列表
        /// </summary>
        private static List<int> C3SOSysNos;
        /// <summary>
        /// 新蛋快递列表
        /// </summary>
        //private static List<int> NeweggShippingSysNos;
        /// <summary>
        /// 具有有效收款记录的订单号
        /// </summary>
        private static List<int> SOSysNosHasWhichSOIncomeInfo;
        /// <summary>
        /// 订单网络支付信息列表
        /// </summary>
        private static List<SONetPayEntity> SONetPayInfoList;
        /// <summary>
        /// 自动审单时审单人编号
        /// </summary>
        private static int AuditUserSysNo = 0;
        /// <summary>
        /// 自动审核一次最多提取的单数
        /// </summary>
        private static int TopCount = 2000;
        /// <summary>
        /// 允许通过款到发货自动审单的支付方式编号列表
        /// </summary>
        //private static List<int> AllowPIASOPayTypeSysNos;
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        private const string INTERORDER = "PromotionIntel2009Q4";

        //信用卡支付方式
        //private static List<int> SuspectPayTypeList = new List<int>();

        //以下五项为邮件发送JOB中调用WCF服务时所使用到的信息
        private static string UserDisplayName;
        private static string UserLoginName;
        private static string CompanyCode;
        private static string StoreCompanyCode;
        private static string StoreSourceDirectoryKey;

        //自动审单控制开关实体
        private static AutoAuditSwitchEntity AuditSwitch;

        //自动审单FP状态明细检查项开关实体
        private static FPSwitchEntity FPSwitch;

        //自动审单客户类型明细检查项开关实体
        private static CustomerTypeSwitchEntity CustomerTypeSwitch;


        //不能通过的优惠类型列表
        private static List<string> DispassPromotionTypes;

        //不能通过审单的地址关键字列表
        private static List<string> DispassReceiveAddressList;


        //不能通过审单的手机或电话关键字列表
        private static List<string> DispassReceiveCellPhoneOrPhoneList;
              
        
        //不能通过审单的收货联系人关键字列表
        private static List<string> DispassReceiveContactList;    

        //不能通过审单的支付方式编号列表
        private static List<int> DispassPayTypeSysNoList;

        //不能通过自动审单的配送方式编号列表
        private static List<int> DispassShipTypeSysNoList;


        //不能通过审单的C3类产品编号列表
        private static List<int> DispassC3ProcuctSysNoList;

        //不能通过的商品ID的SOSysNo列表
        private static List<int> DispassProcuctIDSOSysNoList;

        //获取会员等级及对应金额实体信息列表
        private static List<CustomerRankAmtLimitEntity> customerRankAmtList;




        /// <summary>
        /// 获取自动审单检查项信息（CS工具箱自动审单维护）
        /// </summary>
        private static List<CSTBOrderCheckMasterEntity> CSTBOrderCheckMasterList;

        /// <summary>
        /// 获取自动审单检查项具体明细项信息（CS工具箱自动审单明细项维护）
        /// </summary>
        private static List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemList;

        private static List<DateTime> CSNoWorkDayList;

        //节假日
        private static List<HolidayEntity> Holidays;

        #endregion

        private static void GetAutoAuditInfo()
        {
            UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
        }

        #region Entry Point
        /// <summary>
        /// 自动审单
        /// </summary>
        /// <param name="bizLogFile">业务日志文件全名</param>
        public static void AuditSO(JobContext jobContext)
        {
            GetAutoAuditInfo();

            //获取自动审单检查项信息
            CSTBOrderCheckMasterList = SOQueryDA.GetCSTBOrderCheckMasterList(CompanyCode);

            //获取自动审单检查项具体的明细项信息
            CSTBOrderCheckItemList = SOQueryDA.GetCSTBOrderCheckItemList(CompanyCode);

            //获取非客服工作日列表
            CSNoWorkDayList = SOQueryDA.GetNoCSWorkDay(CompanyCode);

            Holidays = SOQueryDA.GetHolidays(CompanyCode);
    
         
            BizLogFile = jobContext.Properties["BizLog"];

            #region 自动审核暂停项被选中 则进行自动审核的暂停判断
            //如果存在 自动审核暂停时间项的检测  则进行自动审核暂停明细项的进一步判断（当前时间点属于自动审核暂停的时间段范围 则 立即暂停自动审核） 
            if (CSTBOrderCheckMasterList.Count > 0 && CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "AT" && x.Status == 0); }) != null)
            {
                //过滤,  获取有效停止自动审核时间段 列表信息
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTemp = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "SA" && x.Status == 0 && System.DateTime.Now < Convert.ToDateTime(x.Description)); });

                if (CSTBOrderCheckItemEntityTemp != null && CSTBOrderCheckItemEntityTemp.Count > 0)
                {
                    foreach (CSTBOrderCheckItemEntity oci in CSTBOrderCheckItemEntityTemp)
                    {
                        if (System.DateTime.Now > Convert.ToDateTime(oci.ReferenceContent))
                        {
                            jobContext.Message = "当前时间不属于自动审单时间";
                            WriteLog("当前时间不属于自动审单时间");
                            return;
                        }
                    }
                }
            }
            #endregion


            //从配置文件中获取自动审核一次最多提取的单数
            TopCount = GetAutoAuditTopCount();
            //获取本次审单的记录集
            List<SOQueryEntity> tmpSOList = SOQueryDA.GetSOList4Audit(TopCount,CompanyCode);
            if (tmpSOList.Count == 0)
            {
                jobContext.Message = "没有附合条件的记录";
                WriteLog("没有附合条件的记录");
                return;
            }
            jobContext.Message = string.Format("本次共提取{0}个订单", tmpSOList.Count);
            WriteLog(string.Format("本次共提取{0}个订单", tmpSOList.Count));

            //设置相应的审单检查项
            SetAuditSwitchStatus(); 
         
            //设置FP状态检查项明细项信息
            SetAuditFPStatus();

            //设置 客户类型检查明细项
            SetAuditCustomerType();


            //从配置文件中获取新蛋快递列表
            //NeweggShippingSysNos = GetNeweggExpressList();

            //自动审单时审单人编号
            AuditUserSysNo = GetAutoAuditUserSysNo();

            //允许通过自动审单的支付方式编号字串
            //AllowPIASOPayTypeSysNos = GetAllowPIASOPayTypeSysNos();

            //提前取得恶意用户的列表
            malevolenceCustomers = CommonDA.GetMalevolenceCustomers(CompanyCode);
            //提前取得缺货的待审核且经过FP处理过的订单系统编号
            OOSOSysNos = SOQueryDA.GetOOSSOSysNos();
            //提前取得含有C3指定类产品的订单系统编号
            C3SOSysNos = SOQueryDA.GetSOSysNosWhichHasC3Product(CompanyCode);
            //提前取得所有支付方式的列表
            payTypeList = CommonDA.GetPayTypeList(CompanyCode);
            //提前获取所有配送方式的列表
            shipTypeList = CommonDA.GetShipTypeList(CompanyCode);
            //提前取得订单的网络支付信息列表
            SONetPayInfoList = SOQueryDA.GetSONetPayInfoList(CompanyCode);
            //提前取得具有有效收款记录的订单号
            SOSysNosHasWhichSOIncomeInfo = SOQueryDA.GetSOSysNosHasWhichSOIncomeInfo(CompanyCode);
            //提前可疑支付方式
            //string ccPaytypeStr = ConfigurationManager.AppSettings["SUSPECT_PAYTYPE"];
            //string[] ccPaytypeArray = ccPaytypeStr.Split(',');
            //foreach (string cc in ccPaytypeArray)
            //{
            //    SuspectPayTypeList.Add(Int32.Parse(cc));
            //}

            foreach (SOQueryEntity x in tmpSOList)
            {
                try
                {
                    x.CompanyCode = CompanyCode;
                    x.AuditUserSysNo = AuditUserSysNo;
                    CheckRules(x);
                }
                catch (ApplicationException aex)
                {
                    WriteLog(aex.Message);
                }
            }

            EndAuditAll();
        }


        //设置 审单改观检查项 开关状态
        private static void SetAuditSwitchStatus( )
        {         
            AuditSwitch = new AutoAuditSwitchEntity();

            #region 填充开关状态

            if(CSTBOrderCheckMasterList.Count > 0)
            {
                //判断 关键字开关 是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "KW" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckKeyWords = true;
                }

                //判断 配送方式项开关 是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "ST" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckShipType = true;
                }

                //判断 支付方式开关 是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "PT" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckPayType = true;
                }


                //判断 增值税发票开关 是否打开 
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "VAT" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckVAT = true;
                }

                //判断 含有对应产品类别的产品的订单(C3类)开关 是否打开 
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "PC3" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckProductType = true;
                }

                //判断 FP状态开关 是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return ((x.CheckType == "FP") && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckFPStatus = true;
                }

                //判 断客户类型开关 是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "CT" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckCustomerType = true;
                }

                //判断 订单中使用了积分，蛋券，余额，礼品卡 是否打开  
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "PC" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckPointPromotion = true;
                }

                //判断 订单金额大于上对应登记上线 的开关是否 打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "AMT" && x.Status == 0); }) != null)
                {
                    AuditSwitch.IsCheckOrderAmt = true;
                }


                //判断 配送服务类型  开关是否打开
                if (CSTBOrderCheckMasterList.Find(x => { return (x.CheckType == "DT" && x.Status == 0); }) != null)
                {

                    AuditSwitch.IsCheckShipServiceType = true;
                }

            }
                 
            #endregion

            #region 填充不能通过自动审单相应列表信息

            //设置不能通过自动审单的优惠类型
            if (AuditSwitch.IsCheckPointPromotion)
            {
                DispassPromotionTypes = new List<string>();
                //过滤优惠类型
                List<CSTBOrderCheckItemEntity> PromotionTypes = CSTBOrderCheckItemList.FindAll(x => x.ReferenceType == "PC" && x.Status == 0);
                foreach (CSTBOrderCheckItemEntity PromotionType in PromotionTypes)
                {
                    DispassPromotionTypes.Add(PromotionType.ReferenceContent);
                }
            }

            //设置不能通过自动审单的关键字列表
            if (AuditSwitch.IsCheckKeyWords)
            {
                DispassReceiveAddressList = new List<string>();
                //过滤客户地址类型的关键字信息列表 (模糊匹配)   CA：客户地址
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempAddress = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "CA"  && x.Status == 0);});
                foreach (CSTBOrderCheckItemEntity itemAddress in CSTBOrderCheckItemEntityTempAddress)
                {
                    DispassReceiveAddressList.Add(itemAddress.ReferenceContent);
                }

                DispassReceiveCellPhoneOrPhoneList = new List<string>();
                //过滤客户电话类型的关键字 信息列表(精确匹配)   CP：客户电话
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempPhone = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "CP" && x.Status == 0);});
                foreach (CSTBOrderCheckItemEntity itemPhone in CSTBOrderCheckItemEntityTempPhone)
                {
                    DispassReceiveCellPhoneOrPhoneList.Add(itemPhone.ReferenceContent);
                }

                DispassReceiveContactList = new List<string>();
                //过滤联系人类型的关键字列表信息(模糊匹配)     CN：联系人
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempContact = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "CN" && x.Status == 0);});
                foreach (CSTBOrderCheckItemEntity itemContact in CSTBOrderCheckItemEntityTempContact)
                {
                    DispassReceiveContactList.Add(itemContact.ReferenceContent);
                }
            }

            //设置不能通过自动审单的配送方式列表
            if (AuditSwitch.IsCheckShipType)
            {
                DispassShipTypeSysNoList = new List<int>();
                //过滤配送方式列表信息(ST:配送方式类型)
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempShip = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "ST"  && x.Status == 0);});
                foreach (CSTBOrderCheckItemEntity itemShip in CSTBOrderCheckItemEntityTempShip)
                {
                    DispassShipTypeSysNoList.Add(Convert.ToInt32(itemShip.ReferenceContent));
                }   
            }

            //设置不能通过自动审单的支付方式列表
            if (AuditSwitch.IsCheckPayType)
            {
                DispassPayTypeSysNoList = new List<int>();
                //过滤支付方式列表信息(PT:支付方式类型)
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempPay = CSTBOrderCheckItemList.FindAll(x => { return (x.ReferenceType == "PT" && x.Status == 0); });
                foreach (CSTBOrderCheckItemEntity itemPay in CSTBOrderCheckItemEntityTempPay)
                    {
                        DispassPayTypeSysNoList.Add(Convert.ToInt32(itemPay.ReferenceContent));
                    }                              
            }

            //设置C3类产品信息列表
            if(AuditSwitch.IsCheckProductType)
            {
                DispassC3ProcuctSysNoList = SOQueryDA.GetSOSysNosWhichHasC3Product(CompanyCode);

                DispassProcuctIDSOSysNoList = SOQueryDA.GetSOSysNosWhichHasProductID(CompanyCode);
            }

            //设置 订单金额 检查项
            if(AuditSwitch.IsCheckOrderAmt)
            {
                customerRankAmtList = new List<CustomerRankAmtLimitEntity>();
                //过滤 订单金额 及对应的用户等级列表
                List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTempOrderRankAmt = CSTBOrderCheckItemList.FindAll(x => { return ((x.ReferenceType == "1AL"||x.ReferenceType == "2AL"||x.ReferenceType == "3AL"||x.ReferenceType == "4AL"||x.ReferenceType == "5AL") && x.Status == 0); });

                foreach (CSTBOrderCheckItemEntity OrderRankAmt in CSTBOrderCheckItemEntityTempOrderRankAmt)
                {              
                    CustomerRankAmtLimitEntity crle = new CustomerRankAmtLimitEntity();
                    if(OrderRankAmt.ReferenceType=="1AL")
                    {
                        crle.CustomerRank =3;
                    }
                    else if (OrderRankAmt.ReferenceType == "2AL")
                    {
                        crle.CustomerRank = 4;
                    }
                    else if (OrderRankAmt.ReferenceType == "3AL")
                    {
                        crle.CustomerRank = 5;
                    }
                    else if (OrderRankAmt.ReferenceType == "4AL")
                    {
                        crle.CustomerRank = 6;
                    }
                    else if (OrderRankAmt.ReferenceType == "5AL")
                    {
                        crle.CustomerRank = 7;
                    }  
                    crle.AmtLimit = Convert.ToDecimal(OrderRankAmt.ReferenceContent);
                    customerRankAmtList.Add(crle);
                }  
            }
           
            #endregion
                    
        }

        /// <summary>
        /// 设置订单类型 具体的检查项
        /// </summary>
        private static void SetAuditFPStatus()
        {
            FPSwitch = new FPSwitchEntity();
            #region 填充FP开关状态
            if(AuditSwitch.IsCheckFPStatus)
            {
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "FP" && x.ReferenceContent=="KY" && x.Status == 0); })!=null)
                {
                    FPSwitch.IsCheckKY = true;
                }
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CH" && x.Status == 0); }) != null)
                {
                    FPSwitch.IsCheckCH = true;
                }
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CC" && x.Status == 0); }) != null)
                {
                    FPSwitch.IsCheckCC = true;
                }
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CF" && x.Status == 0); }) != null)
                {
                    FPSwitch.IsCheckCF = true;
                }
            }
            #endregion
        }

        /// <summary>
        /// 设置客户类型 具体的检查项
        /// </summary>
        private static void SetAuditCustomerType()
        {
            CustomerTypeSwitch = new CustomerTypeSwitchEntity();
            #region 填充客户类型检查明细项开关
            if(AuditSwitch.IsCheckCustomerType)
            {
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "ZC" && x.Status == 0); }) != null)
                {
                    CustomerTypeSwitch.IsCheckCustomerZC = true;
                }
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "KY" && x.Status == 0); }) != null)
                {
                    CustomerTypeSwitch.IsCheckCustomerKY = true;
                }
                if (CSTBOrderCheckItemList.Count > 0 && CSTBOrderCheckItemList.Find(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "QZ" && x.Status == 0); }) != null)
                {
                    CustomerTypeSwitch.IsCheckCustomerQZ = true;
                }
            
            }
            #endregion
        }
        
        
        //private static List<int> GetAllowPIASOPayTypeSysNos()
        //{
        //    List<int> result = new List<int>();
        //    string tmpNEExpress = ConfigurationManager.AppSettings["AllowPayTypeSysNos"];
        //    string[] tmpExpressIds = tmpNEExpress.Split(",".ToCharArray());

        //    foreach (string id in tmpExpressIds)
        //    {
        //        result.Add(int.Parse(id));
        //    }


        //    return result;
        //}

        /// <summary>
        /// 获取一次审核最多的记录数
        /// </summary>
        /// <returns></returns>
        private static int GetAutoAuditTopCount()
        {
            string tmpAuditUserSysNo = ConfigurationManager.AppSettings["AutoAuditTopCount"];
            int topCount = int.Parse(tmpAuditUserSysNo);

            return (topCount > 0) ? topCount : 2000;
        }

        /// <summary>
        /// 获取自动审单时的用户编号
        /// </summary>
        /// <returns></returns>
        private static int GetAutoAuditUserSysNo()
        {
            string tmpAuditUserSysNo = ConfigurationManager.AppSettings["AuditUserSysNo"];
            return int.Parse(tmpAuditUserSysNo);
        }

        /// <summary>
        /// 从配置文件中获取新蛋快递的列表
        /// </summary>
        /// <returns></returns>
        //private static List<int> GetNeweggExpressList()
        //{
        //    List<int> result = new List<int>();
        //    string tmpNEExpress = ConfigurationManager.AppSettings["NeweggExpress"];
        //    string[] tmpExpressIds = tmpNEExpress.Split(",".ToCharArray());

        //    foreach (string id in tmpExpressIds)
        //    {
        //        result.Add(int.Parse(id));
        //    }

        //    return result;
        //}
        #endregion Entry Point

        /// <summary>
        /// 检查规则-公共部分
        /// </summary>
        /// <param name="entity"></param>
        private static void CheckRules(SOQueryEntity entity)
        {


            #region  订单金额为0
            if (entity.TotalAmount == 0)
            {
                ThrowException(entity, "订单金额为零，不能通过自动审核");
            }
            #endregion

            #region  配送区域不能为空
            if ((entity.ReceiveAreaSysNo == null) || (entity.ReceiveAreaSysNo <= 0))
            {
                ThrowException(entity, "配送区域无效，不能通过自动审核");
            }
            #endregion


            #region 以旧换新订单如果发票抬头与证件姓名不匹配-不能通过自动审核

            if (entity.SOType == 1)
            {
                int soSysNo = entity.SOSysNo.Value;
                string certificaterName = SOQueryDA.GetCertificaterNameBySOSysNo(soSysNo);
                if ((entity.IsVAT ?? 0) == 0)
                {
                    if (entity.ReceiveName != certificaterName)
                    {
                        ThrowException(entity, "以旧换新订单发票抬头与证件姓名不匹配");
                    }
                }
                else
                {
                    string companyName = SOQueryDA.GetVatCompanyNameBySOSysNo(soSysNo);
                    if (certificaterName != companyName)
                    {
                        ThrowException(entity, "以旧换新订单发票抬头与证件姓名不匹配");
                    }
                }
            }

            #endregion

            #region  Intel用户订单不能通过自动审核

            string FromLinkSource = SOFPCheckDA.GetFromLinkSource(entity.CustomerSysNo);
            if (FromLinkSource != null && FromLinkSource == INTERORDER)
            {
                ThrowException(entity, "Intel用户所下订单不能通过自动审核");
            }
            #endregion

            #region 订单备注,订单说明,客户备注有特殊要求 - 不能通过自动审核
            if (!string.IsNullOrEmpty(entity.Memo)
                || !string.IsNullOrEmpty(entity.Note)
                || !string.IsNullOrEmpty(entity.InvoiceNote))
            {
                ThrowException(entity, "订单备注,订单说明,客户备注有特殊要求");
            }
            #endregion

            #region 参加团购的订单 - 不能通过自动审核
            if (entity.IsWholeSale.HasValue
                && entity.IsWholeSale.Value == 1)
            {
                ThrowException(entity, "此订单为团购订单");
            }
            #endregion

            #region 信用卡支付标记为可疑单
            //if (SuspectPayTypeList.Exists(item => item == entity.PayTypeSysNo))
            //{
            //    ThrowException(entity, "信用卡支付,请确认信用卡信息.");
            //}
            #endregion

            #region 订单为:非待审核状态 - 不能通过自动审核
            //提取提单时已按此条件过滤
            #endregion

            #region 订单为:订单锁定 - 不能通过自动审核
            //提取提单时已按此条件过滤
            #endregion

            #region 订单为恶意用户下单 - 不能通过自动审核

            if (malevolenceCustomers.Count > 0
                && malevolenceCustomers.Find(x =>
                {
                    return x.SysNo == entity.CustomerSysNo;
                }) != null)
            {
                ThrowException(entity, "订单为恶意用户下单");
            }
            #endregion

            #region 产品无现货(即产品存在“虚库不足”，“待采购”字样) - 不能通过自动审核
            if (OOSOSysNos.Count > 0
                && OOSOSysNos.Find(x =>
                   {
                       return x == entity.SystemNumber;
                   }) != 0
                && entity.StockType=="NEG"
                && entity.ShippingType=="NEG"
                && entity.InvoiceType=="NEG")
            {
                ThrowException(entity, "产品无现货(即产品存在“虚库不足”，“待采购”字样)");
            }
            #endregion



            #region 订单中的信息与关键字匹配则 - 不能通过自动审核
         
            if (AuditSwitch.IsCheckKeyWords)
            {
                // 关键字(接收地址)
                if (entity.ReceiveAddress.Contains(DispassReceiveAddressList))            
                {
                    ThrowException(entity, "订单中收货地址信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(手机 精确匹配)
                if ( entity.ReceiveCellPhone.Contains(DispassReceiveCellPhoneOrPhoneList, "Accurate") )
                {
                    ThrowException(entity, "订单中的手机信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(电话 精确匹配)
                if (entity.ReceivePhone.Contains(DispassReceiveCellPhoneOrPhoneList, "Accurate"))
                {
                    ThrowException(entity, "订单中的电话信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(收货联系人)      
                if (entity.ReceiveContact.Contains(DispassReceiveContactList))            
                {
                    ThrowException(entity, "订单中收货联系人信息存在特殊关键字 - 不能通过自动审核");
                }                                    
            }
           

            #endregion

            #region 订单 的配送方式、支付方式 属于限定配送方式 - 不能通过自动审核

            if (entity.PayTypeSysNo==0)
            {
                ThrowException(entity, "订单的支付方式为空 - 不能通过自动审核");
            }
            else if (AuditSwitch.IsCheckPayType && DispassPayTypeSysNoList.Contains(entity.PayTypeSysNo))
            {
                ThrowException(entity, "订单的支付方式属于限定支付方式 - 不能通过自动审核");
            }


            if (entity.ShipTypeSysNo == 0)
            {
                ThrowException(entity, "订单的配送方式为空 - 不能通过自动审核");
            }
            else if (AuditSwitch.IsCheckShipType && DispassShipTypeSysNoList.Contains(entity.ShipTypeSysNo))
            {
                ThrowException(entity, "订单的配送方式属于限定配送方式 - 不能通过自动审核");
            }

           
            #endregion

            #region 根据FP状态判断订单是否可以通过自动审核
            if (AuditSwitch.IsCheckFPStatus)
            {
                if (FPSwitch.IsCheckCC
                 && entity.IsFPSO.HasValue
                 && entity.IsFPSO.Value == 3)
                {
                    ThrowException(entity, "订单为炒货订单，不能通过自动审核");
                }
                else if (FPSwitch.IsCheckKY
                    && entity.IsFPSO.HasValue
                    && entity.IsFPSO.Value == 1)
                {
                    ThrowException(entity, "订单为可疑订单，不能通过自动审核");
                }
                else if (FPSwitch.IsCheckCH
                    && entity.IsFPSO.HasValue
                    && entity.IsFPSO.Value == 2)
                {
                    ThrowException(entity, "订单为串货订单，不能通过自动审核");
                }
              
                if (FPSwitch.IsCheckCF
                   && entity.IsDuplicateOrder==1)
                {
                    ThrowException(entity, "订单为重复订单，不能通过自动审核");   
                }

            }       
            #endregion

            #region 根据订单的客户类型判断订单是否可以通过审核

            if(AuditSwitch.IsCheckCustomerType)
            {
                int fraudType = SOQueryDA.GetFraudType(entity.CustomerSysNo);  // 根据客户编号 获取 客户类型      0:正常  1: 可疑  2:欺诈 
                if (CustomerTypeSwitch.IsCheckCustomerZC  && fraudType == 0)
                {
                    ThrowException(entity, "正常用户类型，不能通过自动审核");
                }

                if (CustomerTypeSwitch.IsCheckCustomerKY  && fraudType == 1)
                {
                    ThrowException(entity, "可疑用户类型，不能通过自动审核");
                }
                if (CustomerTypeSwitch.IsCheckCustomerQZ  && fraudType == 2)
                {
                    ThrowException(entity, "欺诈用户类型，不能通过自动审核");
                }
          
            }

            #endregion

            #region 根据 自动审核检查项:增值税发票： 订单中含有增值税发票则不能通过自动审核。
            if (AuditSwitch.IsCheckVAT)
            {
                if(entity.IsVAT.HasValue && entity.IsVAT.Value==1)
                {
                     ThrowException(entity, "订单中含有增值税发票 - 不能通过自动审核");
                }               
            }
            #endregion

            #region 根据 自动审核检查项:订单中使用了积分，蛋券，余额，礼品卡 - 不能通过自动审核

            if (AuditSwitch.IsCheckPointPromotion)
            {
                //检查订单中使用蛋券超过50%
                if (DispassPromotionTypes.Contains("CP50")
                    && (entity.PromotionValue.HasValue && (entity.PromotionValue.Value / 1.0M) > (entity.SOAmt / 2.0M))
                    )
                {
                    ThrowException(entity, "单张订单金额中使用积分或者蛋券超过50%  - 不能通过自动审核");
                }
                //检查订单中使用了积分
                if (DispassPromotionTypes.Contains("PNT")
                    && entity.PointPay > 0)
                {
                    ThrowException(entity, "订单中使用了积分- 不能通过自动审核");
 
                }
                //检查订单中使用了余额
                if (DispassPromotionTypes.Contains("PRE")
                    && entity.PrepayAmt > 0.0M)
                {
                    ThrowException(entity, "订单中使用了余额- 不能通过自动审核");

                }
                //检查订单中使用了礼品卡
                if (DispassPromotionTypes.Contains("GIF")
                    && entity.GiftCardPay > 0.0M)
                {
                    ThrowException(entity, "订单中使用了礼品卡- 不能通过自动审核");

                }


            }    
            #endregion

            #region 含有对应产品类别的产品的订单(C3类)将不能通过自动审核

            if(AuditSwitch.IsCheckProductType)
            {
                if( DispassC3ProcuctSysNoList.Contains(entity.SystemNumber))
                    ThrowException(entity, "订单中有指定C3类产品 - 不能通过自动审核");
                if( DispassProcuctIDSOSysNoList.Contains(entity.SystemNumber))
                    ThrowException(entity, "订单中有指定产品ID - 不能通过自动审核");
            }
       
            #endregion
            
            #region 自动审单对配送时间按照一日一送、一日两送的规则进行检查


            if (entity.DeliveryDate.HasValue)
            {
                if (AuditSwitch.IsCheckShipServiceType)
                {
                    //过滤 产品类别列表信息 (PC3:产品类别)
                    List<CSTBOrderCheckItemEntity> CSTBOrderCheckItemEntityTemp = CSTBOrderCheckItemList.FindAll(x => { return ((x.ReferenceType == "DT11" || x.ReferenceType == "DT12") && x.Status == 0); });
                    if (CSTBOrderCheckItemEntityTemp != null && CSTBOrderCheckItemEntityTemp.Count > 0)
                    {
                        CSTBOrderCheckItemEntity csItem = CSTBOrderCheckItemEntityTemp.Find(item => item.ReferenceContent.Trim() == entity.ShipTypeSysNo.ToString());
                        if (csItem != null)
                        {
                            CheckDeliverDateV2(entity, csItem);
                        }
                    }
                }
            }
            #endregion

            if (payTypeList.Count > 0
                && payTypeList.Find(x =>
                {
                    return (x.SysNo == entity.PayTypeSysNo
                        && x.IsPayWhenRecv == 1
                        );
                }) != null)
            {
                //分支验证货到付款
                CheckCODSO(entity);
                PassCODSO(entity);
                EndAudit(entity);
            }
            else
            {
                //分支验证款到发货
                SONetPayEntity soNetPayinfo = CheckPIASO(entity);
                //对于不需要再次审核的记录做处理
                if (!soNetPayinfo.isReTry)
                {
                    PassPIASO(entity, soNetPayinfo);
                    EndAudit(entity);
                }

            }


        }

        private static void CheckDeliverDateV2(SOQueryEntity entity, CSTBOrderCheckItemEntity csItem)
        {
            //配送信息
            ShipTypeInfo shipType = shipTypeList.Find(x => x.SysNo == entity.ShipTypeSysNo);

            

            //配送类型是0,3的不检查
            if(shipType==null
               ||shipType.DeliveryType=="0"
               ||shipType.DeliveryType=="3"
               ||entity.SOType == 6
               ||entity.SOType == 7
               ||entity.SOType == 8)
                return;
            
            int DType=0;
            int.TryParse(shipType.DeliveryType,out DType);

            //节假日 没有配送方式的是指1和2
            List<HolidayEntity> holidays = Holidays.FindAll(x => x.ShipTypeSysNo == shipType.SysNo || !x.ShipTypeSysNo.HasValue);

            //时间节点
            List<TimeSpan> timepoint = ParseTimeSpot(csItem.Description);

            DeliveryIteration calculator = new DeliveryIteration(DateTime.Now, DType, holidays, timepoint, shipType.IntervalDays, null);

            calculator.Roll();


            DateTime latestDate = calculator.LatestDate;
            int finalSection = calculator.FinalSection;


            //比较
            if (entity.DeliveryDate < latestDate)
            {
                ThrowException(entity, "预约配送日期不满足要求，不能通过自动审核！");

            }
            else if (entity.DeliveryDate == latestDate)
            {

                if( entity.DeliveryTimeRange < finalSection )
                    ThrowException(entity, "预约配送日期不满足要求，不能通过自动审核！");
            }
            
        }

        private static void CheckDeliveryDate(SOQueryEntity entity, CSTBOrderCheckItemEntity csItem)
        {
            List<TimeSpan> dtSpotList = ParseTimeSpot(csItem.Description);
            TimeSpan ts = DateTime.Now.TimeOfDay;
            DayOfWeek dow = DateTime.Now.DayOfWeek;
            DateTime today = DateTime.Today;
            DateTime checkDay;
            ShipTypeInfo shipType = shipTypeList.Find(x => x.SysNo == entity.ShipTypeSysNo);
            int interval = 0;
            if (shipType != null && shipType.DeliveryType=="1")
            {
                interval = shipType.IntervalDays;
            }

            if (csItem.ReferenceType == "DT12")
            {
                if (dtSpotList.Count >= 2)
                {
                    if (((dow == DayOfWeek.Saturday) && (ts >= dtSpotList[1] )) || ((dow == DayOfWeek.Sunday) && (ts <= dtSpotList[1] )))
                    {
                        if (dow == DayOfWeek.Saturday)
                        {
                            checkDay = AdjustCheckDay(today.AddDays(2.0).Date);
                        }
                        else
                        {
                            checkDay = AdjustCheckDay(today.AddDays(1.0).Date);
                        }
                    }
                    else
                    {
                        if ((ts < dtSpotList[0]) || (ts >= dtSpotList[1]))
                        {
                            if (ts < dtSpotList[0])
                            {
                                checkDay = AdjustCheckDay(today.Date);
                            }
                            else
                            {
                                checkDay = AdjustCheckDay(today.AddDays(1.0).Date);
                            }

                            if ((entity.DeliveryDate < checkDay) || ((entity.DeliveryDate == checkDay) && (entity.DeliveryTimeRange == 1)))
                            {
                                ThrowException(entity, "预约配送日期不满足一日两送要求，不能通过自动审核！");
                            }

                            return;
                        }
                        else
                        {
                            checkDay = AdjustCheckDay(today.AddDays(1.0).Date);
                        }                        
                    }

                    if (entity.DeliveryDate < checkDay)
                    {
                        ThrowException(entity, "预约配送日期不满足一日两送要求，不能通过自动审核！");
                    }
                }
            }
            else if (csItem.ReferenceType == "DT11")
            {
                if (dtSpotList.Count >= 1)
                {
                    if (ts < dtSpotList[0])
                    {
                        checkDay = AdjustCheckDay(today.AddDays(1.0 + interval).Date);
                    }
                    else
                    {
                        checkDay = AdjustCheckDay(today.AddDays(2.0 + interval).Date);
                    }

                    if (entity.DeliveryDate < checkDay)
                    {
                        ThrowException(entity, "预约配送日期不满足一日一送要求，不能通过自动审核！");
                    }

                }

            }
        
        }

        private static DateTime AdjustCheckDay(DateTime checkDay)
        {
            DateTime result = checkDay;

            while (CSNoWorkDayList.Contains(result))
            {
                result = result.AddDays(1.0).Date;
            }

            return result;
        }

        private static List<TimeSpan> ParseTimeSpot(string timestring)
        {
            string[] tmpList = timestring.Split(',');
            List<TimeSpan> dtList = new List<TimeSpan>();

            if (tmpList != null)
            {
                for(int i=0; i<tmpList.Length; i++)
                  dtList.Add(Convert.ToDateTime(tmpList[i]).TimeOfDay);
            }

            return dtList;
        }

        #region Check COD
        /// <summary>
        /// 检查规则-货到付款
        /// </summary>
        /// <param name="entity"></param>
        private static void CheckCODSO(SOQueryEntity entity)
        {
            entity.AuditUserSysNo = 800;    //注意这里一定要是这个用户，Alan说有一个邮件专门根据这个用户来给货到付款的客户发短信

            #region 订单项为货到付款但快递不是“新蛋快递” - 不能通过自动审核
            //if (NeweggShippingSysNos.Count > 0
            //    && NeweggShippingSysNos.Find(x =>
            //    {
            //        return x == entity.ShipTypeSysNo;
            //    }) == 0)
            //{
            //    ThrowException(entity, "货到付款订单,但快递不是[新蛋快递]");
            //}
            #endregion
           
            #region 根据订单的客户等级及订单金额判断是否能够通过自动审核

            if(AuditSwitch.IsCheckOrderAmt)
            {
                if (entity.CustomerRank<4)//黄金级别标志位为 4
                {
                    CustomerRankAmtLimitEntity currentRank = customerRankAmtList.Find(x => { return x.CustomerRank == 3; });
                    if (currentRank != null && entity.SOAmt > currentRank.AmtLimit)
                    {                        
                            ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");                    
                    }
                }          
                else if (entity.CustomerRank >=4)
                {
                    //判断是否设置了同等级的 订单金额上线  没有设置 则判断是否设置了下一等级的订单金额上线 
                    CustomerRankAmtLimitEntity currentRank = customerRankAmtList.Find(x => { return x.CustomerRank ==  entity.CustomerRank; });
                    if (currentRank != null)
                    {
                        if (entity.SOAmt > currentRank.AmtLimit)
                        {
                            ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");
                        }
                    }
                    else if(entity.CustomerRank-1>=4)
                    {
                        CustomerRankAmtLimitEntity currentRankFirst = customerRankAmtList.Find(x => { return x.CustomerRank == entity.CustomerRank-1; });
                        if (currentRankFirst != null)
                        {
                            if (entity.SOAmt > currentRankFirst.AmtLimit)
                            {
                                ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");
                            }
                        }
                        else if (entity.CustomerRank - 2 >= 4)
                        {
                            CustomerRankAmtLimitEntity currentRankSecond = customerRankAmtList.Find(x => { return x.CustomerRank == entity.CustomerRank-2; });
                            if (currentRankSecond != null)
                            {
                                if (entity.SOAmt > currentRankSecond.AmtLimit)
                                {
                                    ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");
                                }
                            }
                            else if (entity.CustomerRank - 3 >= 4)
                            {
                                CustomerRankAmtLimitEntity currentRankThird = customerRankAmtList.Find(x => { return x.CustomerRank == entity.CustomerRank-3; });
                                if (currentRankThird != null)
                                {
                                    if (entity.SOAmt > currentRankThird.AmtLimit)
                                    {
                                        ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");
                                    }
                                }
                            }
                        }
                    }
                }            
       
            }                                
            #endregion
        }

        /// <summary>
        /// 一单货到付款通过
        /// </summary>
        /// <param name="entity"></param>
        /// <![CDATA[
        /// 1.将订单标记为：1-待出库；
        /// 2.将CheckShipping中的审核类型标记为 0-自动审核
        /// ]]>
        private static void PassCODSO(SOQueryEntity entity)
        {
            //entity.AuditUserSysNo = AuditUserSysNo;

            if (entity.SoSplitType == "1" || entity.SoSplitType == "2")
            {
                //SOAutoAuditMaintainDA.UpdateSO4PassAutoAudit(entity);
                AutoSplitSO(entity);
                SOAutoAuditMaintainDA.UpdateCheckShippingAuditTypeBySysNo(entity.SystemNumber, AuditType.Manual, "自动审单通过[货到付款], 自动拆分订单", entity.CompanyCode);
                SOAutoAuditMaintainDA.UpdateSO4AuditUserInfo(entity.SystemNumber, entity.AuditUserSysNo,entity.CompanyCode);
            }
            else
            {
                //更新时需要验证当前的SO状态
                int influencingCount1 = SOAutoAuditMaintainDA.UpdateSO4PassAutoAudit(entity);
                if (1 == influencingCount1)
                {
                    SOAutoAuditMaintainDA.UpdateCheckShippingAuditTypeBySysNo(entity.SystemNumber, AuditType.Auto, "自动审单通过[货到付款]", entity.CompanyCode);
                }
                else
                {
                    ThrowException(entity, "订单不存在或状态已不再是待审核状态");
                }
            }
        }
        #endregion Check COD

        #region Check PIA
        /// <summary>
        /// 检查规则-款到发货
        /// </summary>
        /// <param name="entity"></param>
        private static SONetPayEntity CheckPIASO(SOQueryEntity entity)
        {

            SONetPayEntity result;

            #region 款到发货订单,支付方式不在允许的列表中 - 不能通过自动审核
            //int tmpPayTypeSysNo = AllowPIASOPayTypeSysNos.Find(x => { return x == entity.PayTypeSysNo; });
            //if (tmpPayTypeSysNo == 0)
            //{
            //    ThrowException(entity, "款到发货订单,支付方式不在允许的列表中");
            //}

            #endregion

            #region 款到发货订单,不是网络支付类型的 - 不能通过自动审核
            PayTypeEntity payType = payTypeList.Find(x => { return x.SysNo == entity.PayTypeSysNo; });

            if (payType == null)
            {
                ThrowException(entity, string.Format("款到发货订单,不能自动审单的支付类型:[{0}]", entity.PayTypeSysNo));
            }

            if (payType.IsNet != 1)
            {
                ThrowException(entity, "款到发货订单,不是网络支付");
            }

            if (payType.SysNo != entity.PayTypeSysNo)
            {
                ThrowException(entity, "款到发货订单,订单中的付款方式和所选的付款方式不符");
            }

            #endregion

            SONetPayEntity soNetPayInfo = SONetPayInfoList.Find(x => { return x.SOSysNo == entity.SystemNumber; });
            result = soNetPayInfo;

            if (soNetPayInfo == null)
            {
                //ThrowException(entity, "款到发货订单,没有网络支付信息");
                //因为支付可能延迟，如果不存在网上支付，需要重新再跑这个订单
                soNetPayInfo = new SONetPayEntity();
                soNetPayInfo.isReTry = true;
                return soNetPayInfo;
            }

            // 团购订单NetPay审核可能在团购处理Job中已经处理。
            if (entity.SOType != 7) 
            {
                #region 订单金额 != 实收+余额支付 - 不能通过自动审核
                if (soNetPayInfo.PayAmount + entity.PrepayAmt + entity.GiftCardPay != (entity.CashPay + entity.PayPrice + entity.ShipPrice + entity.PremiumAmt + entity.DiscountAmt)) {
                    ThrowException(entity, "款到发货订单,未满足订单金额＝实收+余额支付+礼品卡支付");
                }
                #endregion

                #region 款到发货订单,NetPay信息为非Origin状态
                if (soNetPayInfo.Status != (int)AppEnum.NetPayStatus.Origin) {
                    ThrowException(entity, "款到发货订单,NetPay信息为非Origin状态");
                }
                #endregion
            }
            else if (SONetPayInfoList.Exists(x=>x.SOSysNo == entity.SystemNumber 
                                             && x.Status ==(int)AppEnum.NetPayStatus.Abandon))
            {
                ThrowException(entity, "团购订单，存在作废的NetPay.");
            }


            #region 款到发货订单,网上支付来源不为Bank和NoNeedPay，不能通过自动审核
            if (soNetPayInfo.Source != 0
                && soNetPayInfo.Source != 2)
            {
                ThrowException(entity, "款到发货订单,网上支付来源不为Bank和NoNeedPay");
            }

            #endregion

            #region 款到发货订单，验证网上支付金额是否足额
            decimal soendmoney = entity.GetReceivableAmt(); //因为网上支付不需要去零头
            if (soendmoney > soNetPayInfo.PayAmount)
            {
                ThrowException(entity, "款到发货订单,网上支付金额不足");
            }
            #endregion


            return result;
        }

        /// <summary>
        /// 一单款到发货通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soNetPayinfo"></param>
        private static void PassPIASO(SOQueryEntity entity, SONetPayEntity soNetPayinfo)
        {
            //soNetPayinfo.Status = (int)AppEnum.NetPayStatus.Verified;
            //soNetPayinfo.ApproveUserSysNo = AuditUserSysNo;
            if (entity.SOType != 7)
            {
                AuditNetPay(entity, soNetPayinfo.SysNo);
            }
            else
            {
                if (soNetPayinfo.Status == (int)AppEnum.NetPayStatus.Origin)
                {
                    AuditNetPay(entity, soNetPayinfo.SysNo);
                }
            }
            //int influencingCount = SOAutoAuditMaintainDA.UpdateNetPay(soNetPayinfo);

            //if (influencingCount > 0
            //     && (SOSysNosHasWhichSOIncomeInfo.Count == 0
            //     || !SOSysNosHasWhichSOIncomeInfo.Exists(x =>
            //          x == entity.SystemNumber
            //      )))
            //{
            //    CreateSOIncome(entity, soNetPayinfo);
            //}

            entity.AuditUserSysNo = AuditUserSysNo;
            if (entity.SoSplitType == "1" || entity.SoSplitType == "2")
            {
                //SOAutoAuditMaintainDA.UpdateSO4PassAutoAudit(entity);
                AutoSplitSO(entity);
                SOAutoAuditMaintainDA.UpdateCheckShippingAuditTypeBySysNo(entity.SystemNumber, AuditType.Manual, "自动审单通过，自动拆单.", entity.CompanyCode);
                SOAutoAuditMaintainDA.UpdateSO4AuditUserInfo(entity.SystemNumber, AuditUserSysNo,entity.CompanyCode);
            }
            else
            {
                //更新时需要验证当前的SO状态
                int influencingCount1 = SOAutoAuditMaintainDA.UpdateSO4PassAutoAudit(entity);
                if (1 == influencingCount1)
                {
                    SOAutoAuditMaintainDA.UpdateCheckShippingAuditTypeBySysNo(entity.SystemNumber, AuditType.Auto, "自动审单通过[款到发货]",entity.CompanyCode);
                }
                else
                {
                    ThrowException(entity, "订单不存在或状态已不再是待审核状态");
                }
            }
        }

        private static void AuditNetPay(SOQueryEntity entity, int netpaySysNo)
        {

            IMaintainNetPayV31 service = ServiceBroker.FindService<IMaintainNetPayV31>();

            BatchActionRequest<NetPayMessage> npMsg = new BatchActionRequest<NetPayMessage>()
            {
                Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
                {
                    OperationUser = new OperationUser(UserDisplayName, UserLoginName, StoreSourceDirectoryKey, CompanyCode),
                    CompanyCode = CompanyCode,
                    StoreCompanyCode = StoreCompanyCode
                }
                ,
                Body = new List<NetPayMessage>()
                {
                    new NetPayMessage()
                    {
                        SysNo = netpaySysNo,
                        SOSysNo = entity.SystemNumber,
                        IsForceCheck = false
                    }
                }
            };

            //npMsg.Header = ServiceAdapterHelper.GetCurrentMessageHeader(header);

            try
            {
                IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.DataContracts.BatchActionResponse<NetPayMessage> actionResult = service.BatchApprove(npMsg);
                ServiceAdapterHelper.DealServiceFault(actionResult.Body[0].Fault);
            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainNetPayV31>(service);
            }        
        }

        private static void AutoSplitSO(SOQueryEntity entity)
        {
            if (!entity.SOSysNo.HasValue)
            {
                return;
            }

            IMaintainSOV31 service = ServiceBroker.FindService<IMaintainSOV31>();
            try
            {


                SOV31 actionPara = new SOV31()
                {
                    Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
                    {
                        OperationUser = new OperationUser(UserDisplayName, UserLoginName, StoreSourceDirectoryKey, CompanyCode),
                        CompanyCode = CompanyCode,
                        StoreCompanyCode = StoreCompanyCode
                    }
                    ,
                    Body = new SOMsg
                    {
                        SOMaster = new SOMasterMsg { SystemNumber = entity.SOSysNo.Value }
                    }
                };

                SOListV31 result = new SOListV31();

                result = service.AutoSplitSONew(actionPara);

                if (actionPara.Faults != null
                    && actionPara.Faults.Count > 0)
                {
                    string tmpSendMessageException = actionPara.Faults[0].ErrorDescription + "\r\n" +
                       actionPara.Faults[0].ErrorDetail;
                    throw (new Exception(tmpSendMessageException));
                }
            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainSOV31>(service);
            }

        }

        /// <summary>
        /// 创建收款单记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soNetPayinfo"></param>
        private static void CreateSOIncome(SOQueryEntity entity, SONetPayEntity soNetPayinfo)
        {
            //SOIncomeEntity soIncomeInfo = new SOIncomeEntity();

            //soIncomeInfo.OrderType = (int)SOIncomeOrderType.SO;
            //soIncomeInfo.OrderSysNo = soNetPayinfo.SOSysNo;
            //soIncomeInfo.OrderAmt = entity.TotalAmount;
            //soIncomeInfo.IncomeStyle = (int)SOIncomeStyle.Advanced;
            //// SOIncome的收款=NetPay收款+预付款
            //soIncomeInfo.IncomeAmt = soNetPayinfo.PayAmount;
            //soIncomeInfo.PrepayAmt = Math.Max(entity.PrepayAmt, 0);
            //soIncomeInfo.IncomeUserSysNo = AuditUserSysNo;
            //soIncomeInfo.IncomeTime = DateTime.Now;
            //soIncomeInfo.Status = (int)SOIncomeStatus.Origin;
            //soIncomeInfo.GiftCardPayAmt = Math.Max(entity.GiftCardPay, 0);

            //SOAutoAuditMaintainDA.CreateSOIncome(soIncomeInfo);
        }
        #endregion Check PIA

        /// <summary>
        /// 未通过验证规则时的处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="message"></param>
        private static void ThrowException(SOQueryEntity entity, string message)
        {
            string tmpErrorMessage = string.Format("自动审单未通过{0}: - {1}", entity.SystemNumber, message);

            SOAutoAuditMaintainDA.UpdateCheckShippingAuditTypeBySysNo(entity.SystemNumber, AuditType.Manual, message,entity.CompanyCode);

            throw (new ApplicationException(tmpErrorMessage));
        }

        /// <summary>
        /// 一单审核结束
        /// </summary>
        /// <param name="entity"></param>
        private static void EndAudit(SOQueryEntity entity)
        {
            WriteLog(string.Format("自动审单通过:{0}", entity.SystemNumber));
        }

        /// <summary>
        /// 本批全部审核结束
        /// </summary>
        /// <param name="entity"></param>
        private static void EndAuditAll()
        {
            WriteLog("本次审单结束");
        }

        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }
    }
}
