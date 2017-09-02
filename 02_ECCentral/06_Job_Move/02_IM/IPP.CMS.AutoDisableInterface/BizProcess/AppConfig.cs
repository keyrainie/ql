/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Phoebe Zhang (Phoebe.F.Zhang@Newegg.com)
 *  Date:    2009-05-20 17:28:55
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System.Configuration;
using System.Collections;
using System;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{    /// <summary>
    /// Summary description for AppConfig.
    /// </summary>
    public class AppConfig
    {
        private AppConfig()
        {

        }

        public static string IPPVersion
        {
            get { return ConfigurationManager.AppSettings["IPPVersion"]; }
        }

        public static string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["ConnectionString"]; }
        }
        public static string ConnectionStringForSelect
        {
            get { return ConfigurationManager.AppSettings["ConnectionStringForSelect"]; }
        }
        // Add by Philo 2007-03-26
        public static string ConnectionReportString
        {
            get { return ConfigurationManager.AppSettings["ConnectionReportString"]; }
        }

        public static string ConnectionD2His01String
        {
            get { return ConfigurationManager.AppSettings["ConnectionD2His01String"]; }
        }

        // add by gordon 2008-02-05 [SAP]Interface from IPP to SAP
        public static string ConnectionSAPString
        {
            get { return ConfigurationManager.AppSettings["ConnectionSAPString"]; }
        }
        // add by gordon 2008-02-05 [SAP]Interface from IPP to SAP
        //add by andy 2008-09-02 
        public static string ConnectionOZZODBString
        {
            get { return ConfigurationManager.AppSettings["OZZODBConnectionString"]; }
        }
        public static string ErrorLogFolder
        {
            get { return ConfigurationManager.AppSettings["ErrorLogFolder"]; }
        }

        public static string SOAllocatedMap
        {
            get { return ConfigurationManager.AppSettings["SOAllocatedMap"]; }
        }
        public static string pre_return_url
        {
            get { return ConfigurationManager.AppSettings["pre_return_url"]; }
        }

        public static string MailDBConnectionString
        {
            get { return ConfigurationManager.AppSettings["MailConnectionString"]; }
        }

        #region 支付密鑰定義
        public static string payresultfromCcbKey
        {
            get { return ConfigurationManager.AppSettings["payresultfromCcbKey"]; }
        }
        public static string paypaypalipnKey
        {
            get { return ConfigurationManager.AppSettings["pay-paypal-ipnKey"]; }
        }
        public static string paypaypalpdtKey
        {
            get { return ConfigurationManager.AppSettings["pay-paypal-pdtKey"]; }
        }
        public static string NetPayAlipayUtilKey
        {
            get { return ConfigurationManager.AppSettings["NetPayAlipayUtilKey"].ToString(); }
        }
        public static string TenpayKey
        {
            get { return ConfigurationManager.AppSettings["TenpayKey"].ToString(); }
        }
        public static string NetPay99billKey
        {
            get { return ConfigurationManager.AppSettings["netpay99billkey"].ToString(); }
        }
        #endregion



        // Add by tomato 2006-12-11 供應商管理 Begin
        #region 供應商管理信息
        public static string VendorFilePath
        {
            get
            {
                return Configuration.CommonConfiguration.VendorFilePath;
            }
        }

        public static string VendorURLPath
        {
            get
            {
                return Configuration.CommonConfiguration.VendorURLPath;
            }
        }

        public static string VendorAgentLevel
        {
            get
            {
                return Configuration.CommonConfiguration.VendorAgentLevel;
            }
        }

        #endregion
        // Add by tomato 2006-12-11 供應商管理 End
        // Add by tomato 2006-12-19 價格審核 Begin
        public static string PriceVerifyEmail
        {
            get
            {
                return Configuration.CommonConfiguration.PriceVerifyEmail;
            }
        }
        // Add by tomato 2006-12-19 價格審核 End


        public static string VendorVerifyEmail
        {
            get
            {
                return Configuration.CommonConfiguration.VendorVerifyEmail;
            }
        }

        public static string CategoryVerifyEmail
        {
            get
            {
                return Configuration.CommonConfiguration.CategoryVerifyEmail;
            }
        }


        #region PM Configuration Info
        // delete by tomato 2007-04-18 測試去掉PMCollection配置項
        //public static string PMCollection
        //{
        //    get
        //    {
        //        //return ConfigurationManager.AppSettings["PMCollection"];
        //        return Configuration.CommonConfiguration.PMCollection;
        //    }
        //}

        public static string FMCollection
        {
            get
            {
                //return ConfigurationManager.AppSettings["FMCollection"];
                return Configuration.CommonConfiguration.FMCollection;
            }
        }

        #endregion

        #region RMA或CC人員名單配置信息
        public static string RMAHandlerNext
        {  //add by dawnstar
            get
            {
                return Configuration.CommonConfiguration.RMAHandlerNext;
            }
        }
        #endregion

        #region RMA和CC人員的郵箱地址,vip customer RMA Request后第3,7,12天發mail
        public static string RMAandCCMail
        {
            get
            {
                return Configuration.CommonConfiguration.RMAandCCMail;
            }

        }
        #endregion

        #region 日報表接收人員郵箱地址
        public static string DayReportMail
        {
            get
            {
                return Configuration.CommonConfiguration.DayReportMail;
            }
        }

        #endregion

        #region 每日已入庫未付款的負PO單接收郵箱地址
        public static string DayNegativeMail
        {
            get
            {
                return Configuration.CommonConfiguration.DayNegativeMail;
            }
        }

        #endregion
        #region POPrint Configuration Info
        public static string IPPrintCollection
        {
            get
            {
                return Configuration.CommonConfiguration.IPPrintCollection;
            }
        }

        public static string FMPrintCollection
        {
            get
            {
                return Configuration.CommonConfiguration.FMPrintCollection;
            }
        }

        #endregion

        #region Add by tomato 2006-11-30 Partly Receive
        public static string PartlyReceiveReason
        {
            get
            {
                return Configuration.CommonConfiguration.PartlyReceiveReason;
            }
        }
        public static string InStockCC
        {
            get
            {
                return Configuration.CommonConfiguration.InStockCC;
            }
        }
        #endregion

        // Add by tomato 2006-02-01 [PM]PIM后臺追加 Begin
        public static string PIMURL
        {
            get
            {
                return Configuration.CommonConfiguration.PIMURL;
            }
        }
        // Add by tomato 2006-02-01 [PM]PIM后臺追加 End

        #region Add by tomato 2006-10-30 投訴系統

        public static string ComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainType;
            }
        }

        public static string ComplainSourceType
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainSourceType;
            }
        }

        public static string ReplySourceType
        {
            get
            {
                return Configuration.CommonConfiguration.ReplySourceType;
            }
        }

        public static string ComplainMailFrom
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainMailFrom;
            }
        }

        public static string ComplainMailBcc
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainMailBcc;
            }
        }

        // Add by tomato 2006-12-11 [CS]IPP投訴管理系統添加統計功能應用的需求 Begin
        public static string ComplainResponsibleDept
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainResponsibleDept;
            }
        }
        // Add by tomato 2006-12-11 [CS]IPP投訴管理系統添加統計功能應用的需求 End
        #endregion



        #region Mail Configuration Info
        public static string MailCharset
        {
            get
            {
                return ConfigurationManager.AppSettings["MailCharset"];
                //return Configuration.MailConfiguration.MailCharset;
            }
        }

        public static string MailFrom
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailFrom"];
                return Configuration.MailConfiguration.MailFrom;
            }
        }

        public static string MailFromName
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailFromName"];
                return Configuration.MailConfiguration.MailFromName;
            }
        }

        public static string MailServer
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailServer"];
                return Configuration.MailConfiguration.MailServer;
            }
        }

        public static string MailUserName
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailUserName"];
                return Configuration.MailConfiguration.MailUserName;
            }
        }

        public static string MailUserPassword
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailUserPassword"];
                return Configuration.MailConfiguration.MailUserPassword;
            }
        }
        public static string MailLord
        {
            get
            {
                //return ConfigurationManager.AppSettings["MailLord"];
                return Configuration.MailConfiguration.MailLord;
            }
        }

        #endregion

        public static bool IsDaemon
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["IsDaemon"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// PM二手品管理專員的電子郵件地址。若有多個，相鄰2個電子郵件地址請用英文分號“;”分隔。
        /// 
        /// Add by Carl 2008.1.18.[WorkItem:3305][PM]新增二手品自動定價流程
        /// </summary>
        public static string PMSecondHandProductManageCommissionerEmail
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["PMSecondHandProductManageCommissionerEmail"];
                if (dd == null || dd.Trim() == "")
                    return "";
                else
                    return dd.Trim();
            }
        }
        /// <summary>
        /// 數據庫表 IPP3..Sys_User 里，標識系統用戶的SysNo。該用戶的創建是為了，標識“系統自動生成記錄或其它處理”所完成的作業。
        /// 這個值，在開發環境、QA環境、生產環境里，可能對應于不同的值；所以，需要外界傳入。
        /// 
        /// Add by Carl 2008.1.18.[WorkItem:3305][PM]新增二手品自動定價流程
        /// </summary>
        public static int SystemUserSysNo
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["SystemUserSysNo"];
                if (dd == null)
                    return AppConst.IntNull;
                try
                {
                    int intSystemUserSysNo = int.Parse(dd);
                    return intSystemUserSysNo;
                }
                catch
                {
                    return AppConst.IntNull;
                }
            }
        }
        public static bool IsIPP
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["IsIPP"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }

        public static string AdminEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminEmail"];
            }
        }

        public static string PMGroupEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["PMGroupEmail"];
            }
        }

        public static bool IsSendEMail
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["IsSendEMail"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }

        public static string SOMailTemplet
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderMailTemplet"];
            }
        }

        public static string NewSOMailTemplet
        {
            get
            {
                return ConfigurationManager.AppSettings["NewOrderMailTemplet"];
            }
        }

        public static string NewForthSOMailTemplet
        {
            get
            {
                return ConfigurationManager.AppSettings["NewForthOrderMailTemplet"];
            }
        }

        public static string CustomerGiftMailTemplet
        {
            get
            {
                return ConfigurationManager.AppSettings["CustomerGiftMailTemplet"];
            }
        }


        public static string PicturePath
        {
            get
            {
                return ConfigurationManager.AppSettings["PicturePath"];
            }
        }
        public static string OnlineRootPath
        {
            get
            {
                return ConfigurationManager.AppSettings["OnlineRootPath"];
            }
        }
        public static bool IsImportable
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["IsImportable"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }
        public static string ReviewBox
        {
            get
            {
                return ConfigurationManager.AppSettings["ReviewBox"];
            }
        }

        #region Auction Configuration Info
        /// <summary>
        /// add by erik at 2005-06-30
        /// </summary>
        public static string BidInfoXmlPath
        {
            get
            {
                //return ConfigurationManager.AppSettings["BidInfoXmlPath"];
                return Configuration.AuctionConfiguration.BidInfoXmlPath;
            }
        }

        /// <summary>
        /// 是否開啟拍賣功能
        /// KiddLiu 2005/07/02
        /// </summary>
        public static bool IsAuction
        {
            get
            {
                //string dd = ConfigurationManager.AppSettings["IsAuction"];
                string dd = Configuration.AuctionConfiguration.IsAuction;
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 拍賣成功后允許的最長確認時間
        /// </summary>
        public static string DelayTime
        {
            get
            {
                //return ConfigurationManager.AppSettings["DelayTime"];			
                return Configuration.AuctionConfiguration.DelayTime;
            }
        }

        /// <summary>
        /// 拍賣開始時間
        /// Kidd Liu 2006/07/16
        /// </summary>
        public static string AuctionBeginTime
        {
            get
            {
                //return ConfigurationManager.AppSettings["AuctionBeginTime"];			
                return Configuration.AuctionConfiguration.AuctionBeginTime;
            }
        }

        /// <summary>
        /// 拍賣結束時間
        /// Kidd Liu 2006/07/16
        /// </summary>
        public static string AuctionEndTime
        {
            get
            {
                //return ConfigurationManager.AppSettings["AuctionEndTime"];			
                return Configuration.AuctionConfiguration.AuctionEndTime;
            }
        }

        /// <summary>
        /// Author :   Johnnie.X.Zhou
        /// Time   :   09/16/2005
        /// 拍賣管理員
        /// </summary>
        public static string AuctionAdmin
        {
            get
            {
                //return ConfigurationManager.AppSettings["AuctionAdmin"];
                return Configuration.AuctionConfiguration.AuctionAdmin;
            }
        }

        #endregion

        public static string Adv
        {
            get
            {
                return ConfigurationManager.AppSettings["Adv"];
            }
        }

        public static string MailBestowPointRemark
        {
            get
            {
                return ConfigurationManager.AppSettings["MailBestowPointRemark"];
            }
        }

        public static bool IsIPCtrl
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["IsIPCtrl"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }

        public static int MaxPerOrder
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["MaxPerOrder"];
                int maxPerOrder = 999;
                if (dd != null)
                {
                    try
                    {
                        maxPerOrder = Convert.ToInt32(dd);
                    }
                    catch
                    { }
                }
                return maxPerOrder;
            }
        }

        public static string BetPicUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BetPicUrl"];
            }
        }
        //自動作廢訂單開關 
        public static bool AbandonExpiredSOControl
        {
            get
            {
                string dd = ConfigurationManager.AppSettings["AbandonExpiredSO"];
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }

        }

        #region HBX --- David Liu @ 2006-05-31

        /// <summary>
        /// 獲取 HBX 開關信息
        /// </summary>
        public static bool HBX_Required
        {
            get
            {
                //if(ConfigurationManager.AppSettings["HBX_Required"]=="1" || ConfigurationManager.AppSettings["HBX_Required"].ToLower()=="true")
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                if (Configuration.HBXConfiguration.HBX_Required == "1" || Configuration.HBXConfiguration.HBX_Required.ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return ConfigurationManager.AppSettings["HBX_Required"] ;
                //return Configuration.HBXConfiguration.HBX_Required == "1";
            }
        }

        /// <summary>
        /// 獲取 HBX Javascript 存放路徑
        /// </summary>
        public static string HBX_JSLocation
        {
            get
            {
                //return ConfigurationManager.AppSettings["HBX_JS_Location"];
                return Configuration.HBXConfiguration.HBX_JSLocation;
            }
        }


        public static string HBX_InternalUseOnlyGN
        {
            get
            {
                //return ConfigurationManager.AppSettings["HBX_InternalUseOnlyGN"];
                return Configuration.HBXConfiguration.HBX_InternalUseOnlyGN;
            }
        }

        /// <summary>
        /// 有待刪除
        /// </summary>
        public static string HBX_GroupName
        {
            get
            {
                return ConfigurationManager.AppSettings["HBX_GroupName"];
                //return Configuration.HBXConfiguration.HBX_GroupName;
            }
        }

        public static string HBX_Acct
        {
            get
            {
                //return ConfigurationManager.AppSettings["HBX_Acct"];
                return Configuration.HBXConfiguration.HBX_ACCT;
            }
        }

        public static string HBX_EAcct
        {
            get
            {
                //return ConfigurationManager.AppSettings["HBX_EAcct"];
                return Configuration.HBXConfiguration.HBX_EACCT;
            }
        }

        public static string HBX_CAcct
        {
            get
            {
                //return ConfigurationManager.AppSettings["HBX_CAcct"];
                return Configuration.HBXConfiguration.HBX_CACCT;
            }
        }

        //=======================================
        //Store Value Map

        //首頁
        public static string HBX_Store_Default
        {
            get
            {
                //return ConfigurationManager.AppSettings["Default.aspx"];
                return Configuration.HBXConfiguration.HBX_Store_Default;
            }
        }

        //電腦硬件
        public static string HBX_Store_Hardware
        {
            get
            {
                //return ConfigurationManager.AppSettings["Index.aspx?cid=01"];
                return Configuration.HBXConfiguration.HBX_Store_Hardware;
            }
        }

        //數碼產品
        public static string HBX_Store_DigitalProduct
        {
            get
            {
                //return ConfigurationManager.AppSettings["Index.aspx?cid=02"];
                return Configuration.HBXConfiguration.HBX_Store_DigitalProduct;
            }
        }

        //附件耗材
        public static string HBX_Store_Attachment
        {
            get
            {
                //return ConfigurationManager.AppSettings["Index.aspx?cid=04"];
                return Configuration.HBXConfiguration.HBX_Store_Attachment;
            }
        }

        //音頻設備
        public static string HBX_Store_SoundDevice
        {
            get
            {
                //return ConfigurationManager.AppSettings["SubIndex.aspx?subid=01"];
                return Configuration.HBXConfiguration.HBX_Store_SoundDevice;
            }
        }

        //IT時尚生活
        public static string HBX_Store_ITLife
        {
            get
            {
                //return ConfigurationManager.AppSettings["Index.aspx?cid=07"];
                return Configuration.HBXConfiguration.HBX_Store_ITLife;
            }
        }

        //品牌專賣
        public static string HBX_Store_BrandSale
        {
            get
            {
                //return ConfigurationManager.AppSettings["BrandMap.aspx"];
                return Configuration.HBXConfiguration.HBX_Store_BrandSale;
            }
        }
        //=======================================
        #endregion


        public static string UpFilePath_AutoConfirmPage
        {
            get
            {
                return Configuration.FinanceConfiguration.UpFilePath_AutoConfirmPage;
            }
        }

        public static string MKTPointAlarmEmailAdd
        {
            get
            {
                return Configuration.FinanceConfiguration.MKTPointAlarmEmailAdd;
            }
        }
        public static string PMPointAlarmEmailAdd
        {
            get
            {
                return Configuration.FinanceConfiguration.PMPointAlarmEmailAdd;
            }
        }

        public static string PMPointAlarmEmailAddForRicoh
        {
            get
            {
                return Configuration.FinanceConfiguration.PMPointAlarmEmailAddForRicoh;
            }
        }

        public static string ShippingFeePromationEmailAdd
        {
            get
            {
                return Configuration.FinanceConfiguration.ShippingFeePromationEmailAdd;
            }

        }
        public static string KuLingReportEmailAdd
        {
            get
            {
                return Configuration.FinanceConfiguration.KuLingReportEmailAdd;
            }
        }

        public static string PayListReportEmailAdd
        {
            get
            {
                return Configuration.FinanceConfiguration.PayListReportEmailAdd;
            }
        }

        public static bool IsSendPayListReportEmail
        {
            get
            {
                string dd = Configuration.FinanceConfiguration.IsSendPayListReportEmail;
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }
        }

        #region ExpiredSOMail List 作廢過期未支付訂單、提醒支付的結果，mail通知相關人員 --Cindy

        public static string ExpireSOMailList
        {
            get
            {
                return Configuration.CommonConfiguration.ExpiredSOMailList;
            }
        }

        #endregion

        public static bool OpenCheckQtyLog
        {
            get
            {
                return Configuration.CommonConfiguration.OpenCheckQtyLog;
            }
        }

        public static bool OpenRMADataTran
        {
            get
            {
                return Configuration.CommonConfiguration.OpenRMADataTran;
            }
        }
        // Add by tomato 2007-04-03 WarehouseNumber
        public static string WarehouseNumber
        {
            get
            {
                return Configuration.CommonConfiguration.WarehouseNumber;
            }
        }
        /// <summary>
        /// chester 獲取所有connectionStrings信息
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetConnectionStrings()
        {
            Hashtable hsconnectionStrings = new Hashtable();
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

            IEnumerator connectionStringsEnum = connectionStrings.GetEnumerator();

            int i = 0;
            string name = "";
            while (connectionStringsEnum.MoveNext())
            {
                name = connectionStrings[i].Name;
                hsconnectionStrings.Add(name, connectionStrings[name].ToString());
                i += 1;
            }
            return hsconnectionStrings;
        }

        //public static bool IsOutStockOpt
        //{
        //    get
        //    {
        //        string str = ConfigurationManager.AppSettings["IsOutStockOpt"];
        //        if (str == null || str.ToLower() != "true")
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        // Add by Robin 2007-06-01 圖片稍后即來的圖片URL
        public static string ProductInitializePic
        {
            get
            {
                return Configuration.CommonConfiguration.ProductInitializePic;
            }
        }

        public static string UpFilePath_ImportJDPrice
        {
            get
            {
                return Configuration.CommonConfiguration.UpFilePath_ImportJDPrice;
            }
        }

        //更新AZ用戶列表
        public static string UpFilePath_ImportAstraZenecaCustomer
        {
            get
            {
                return Configuration.CommonConfiguration.UpFilePath_ImportAstraZenecaCustomer;
            }
        }

        public static string InventoryWeeklyReportEmailAddress
        {
            get
            {
                return Configuration.FinanceConfiguration.InventoryWeeklyReportEmailAddress;
            }
        }

        public static string SVRemindMailToPMList
        {
            get
            {
                return Configuration.CommonConfiguration.SVRemindMailToPMList;
            }
        }
        public static string SVRemindMailToCCList
        {
            get
            {
                return Configuration.CommonConfiguration.SVRemindMailToCCList;
            }
        }
        public static bool IsSendSVRemindMail
        {
            get
            {
                string dd = Configuration.CommonConfiguration.IsSendSVRemindMail;
                if (dd == null || dd.ToLower() != "true")
                    return false;
                else
                    return true;
            }

        }

        public static string AuditProductPrice1
        {
            get
            {
                return Configuration.CommonConfiguration.AuditProductPrice1;
            }
        }

        public static string AuditProductPrice2
        {
            get
            {
                return Configuration.CommonConfiguration.AuditProductPrice2;
            }
        }
        public static string OutboundCCMail
        {
            get
            {
                return Configuration.CommonConfiguration.OutboundCCMail;
            }
        }

        #region  實庫圖片拍攝Item 的Newegg Support 通知郵件
        public static string SendMailToPs
        {
            get
            {
                return Configuration.CommonConfiguration.SendMailToPs;
            }
        }

        #endregion

        #region  更新，新增地區郵件發送地址
        public static string SendMailForAddOrUpdateArea
        {
            get
            {
                return Configuration.CommonConfiguration.SendMailForAddOrUpdateArea;
            }
        }
        public static string FetchByOneSelf_Warehouse
        {
            get
            {
                return Configuration.CommonConfiguration.FetchByOneSelf_Warehouse;
            }
        }
        public static string FetchByOneSelf_Station
        {
            get
            {
                return Configuration.CommonConfiguration.FetchByOneSelf_Station;
            }

        }

        #endregion

        /// <summary>
        /// 取把一周內新增的Vendor的列表需要發給那些PM
        /// 如果有多個PM，以;分隔
        /// </summary>
        public static string SendMailForNewVendorToPM
        {
            get
            {
                return Configuration.CommonConfiguration.SendMailForNewVendorToPM;
            }
        }

        public static string ProductQuestionUserSysnoList
        {
            get
            {
                return Configuration.CommonConfiguration.ProductQuestionUserSysnoList;
            }
        }

        /// <summary>
        /// 投訴即將超期提醒郵件列表 Kevin 2008-2-26
        /// </summary>
        public static string ComplainOutdatedAlertMailList
        {
            get
            {
                return Configuration.CommonConfiguration.ComplainOutdatedAlertMailList;
            }
        }

        /// <summary>
        /// 自動RMA處理系統財務確認人員 Kevin 2008-4-15
        /// </summary>
        public static string RMAAuto
        {
            get
            {
                return Configuration.CommonConfiguration.RMAAuto;
            }
        }

        //Add By Andy 2008-01-23 獲取RMA退壞貨入庫審核人郵箱
        public static string RMAReturnAuditMail
        {

            get { return Configuration.CommonConfiguration.RMAReturnBadMailList; }

        }
        //add by andy 2008-03-21 獲取退二手品入庫抄送郵件地址
        public static string ReturnSecondHandsCCEmailAddr
        {
            get { return Configuration.CommonConfiguration.ReturnSecondHandsCCEmail; }
        }
        public static string RMAReturnBadCCMailList
        {
            get { return Configuration.CommonConfiguration.RMAReturnBadCCMailList; }

        }

        public static string SendMailReportPerMonth
        {
            get
            {
                return Configuration.CommonConfiguration.SendMailReportPerMonth;
            }
        }

        public static string CustVATCertificateFilePath
        {
            get
            {
                return Configuration.CommonConfiguration.CustVATCertificateFilePath;
            }
        }

        public static string CustVATCertificateURLPath
        {
            get
            {
                return Configuration.CommonConfiguration.CustVATCertificateURLPath;
            }
        }

        public static string CSConfimComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.CSConfimComplainType;
            }
        }

        public static string SaleComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.SaleComplainType;
            }
        }

        public static string StockComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.StockComplainType;
            }
        }

        public static string ProductComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.ProductComplainType;
            }
        }

        public static string CSComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.CSComplainType;
            }
        }

        public static string MarketComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.MarketComplainType;
            }
        }

        public static string OtherComplainType
        {
            get
            {
                return Configuration.CommonConfiguration.OtherComplainType;
            }
        }
        public static string MSMQException
        {
            get
            {
                return Configuration.CommonConfiguration.MSMQException;
            }
        }
        public static string RMARevertOutDate
        {
            get
            {
                return Configuration.CommonConfiguration.RMARevertOutDate;
            }
        }

        public static string OZZOProductControllerMail
        {
            get { return Configuration.CommonConfiguration.OZZOProductControllerMail; }
        }

        public static string OutStockMailAddr
        {
            get { return Configuration.CommonConfiguration.OutStockMailAddr; }
        }
        public static string SelfFetchShipType
        {
            get { return Configuration.CommonConfiguration.SelfFetchShipType; }
        }

        public static string InventoryMailAddress
        {
            get { return Configuration.CommonConfiguration.InventoryMailAddress; }
        }

        public static string EIMSReduceSaleCostDate
        {
            get { return Configuration.CommonConfiguration.EIMSReduceSaleCostDate; }
        }
    }
}
