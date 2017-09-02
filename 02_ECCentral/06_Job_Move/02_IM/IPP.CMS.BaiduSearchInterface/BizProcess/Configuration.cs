/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Phoebe Zhang (Phoebe.F.Zhang@Newegg.com)
 *  Date:    2009-05-20 17:35:39
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{
    /// <summary>
    /// Summary description for Configuration.
    /// </summary>
    public class Configuration
    {
        #region class definition for InternalConfiguration
        /// <summary>
        /// this is the adapter class that implemented the actual property interfaces.
        /// all properties are instance properties.
        /// for refactoring's purpose, configuration class is not necessarily in the form of an adapter class. 
        /// </summary>
        private class InternalConfiguration : ConfigurationBase
        {
            #region section name in the configuration file
            private static readonly string SECTIONNAME_MAIL_CONFIGURATION = "Configuration\\Mail.config";
            private static readonly string SECTIONNAME_AUCTIONL_CONFIGURATION = "Configuration\\Auction.config";
            private static readonly string SECTIONNAME_HBX_CONFIGURATION = "Configuration\\HBX.config";
            private static readonly string SECTIONNAME_COMMON_CONFIGURATION = "Configuration\\Common.config";
            private static readonly string SECTIONNAME_FINANCE_CONFIGURATION = "Configuration\\Finance.config";
            #endregion

            static private InternalConfiguration Config;

            public static InternalConfiguration GetInstance()
            {
                if (Config == null)
                {
                    Config = new InternalConfiguration();
                }
                return Config;
            }

            public MailConfiguration MailConfiguration
            {
                get
                {
                    return (MailConfiguration)GetFromCache(false, SECTIONNAME_MAIL_CONFIGURATION, typeof(MailConfiguration));
                }
            }

            public AuctionConfiguration AuctionConfiguration
            {
                get
                {
                    return (AuctionConfiguration)GetFromCache(false, SECTIONNAME_AUCTIONL_CONFIGURATION, typeof(AuctionConfiguration));
                }
            }

            public HBXConfiguration HBXConfiguration
            {
                get
                {
                    return (HBXConfiguration)GetFromCache(false, SECTIONNAME_HBX_CONFIGURATION, typeof(HBXConfiguration));
                }
            }

            public CommonConfiguration CommonConfiguration
            {
                get
                {
                    return (CommonConfiguration)GetFromCache(false, SECTIONNAME_COMMON_CONFIGURATION, typeof(CommonConfiguration));
                }
            }

            public FinanceConfiguration FinanceConfiguration
            {
                get
                {
                    return (FinanceConfiguration)GetFromCache(false, SECTIONNAME_FINANCE_CONFIGURATION, typeof(FinanceConfiguration));
                }
            }
        }

        #endregion

        private static InternalConfiguration Config;

        static Configuration()
        {
            Config = InternalConfiguration.GetInstance();
        }

        public static MailConfiguration MailConfiguration
        {
            get
            {
                return Config.MailConfiguration;
            }
        }

        public static AuctionConfiguration AuctionConfiguration
        {
            get
            {
                return Config.AuctionConfiguration;
            }
        }

        public static HBXConfiguration HBXConfiguration
        {
            get
            {
                return Config.HBXConfiguration;
            }
        }
        public static CommonConfiguration CommonConfiguration
        {
            get
            {
                return Config.CommonConfiguration;
            }
        }
        public static FinanceConfiguration FinanceConfiguration
        {
            get
            {
                return Config.FinanceConfiguration;
            }
        }
    }

    #region Serialize XmlNode info in config files --added by seanqu 2006/06/06

    #region MailConfiguration Class

    /// <summary>
    /// Mail configuration class
    /// </summary>
    [XmlRoot("MailConfiguration")]
    public class MailConfiguration
    {
        public MailConfiguration()
        {
        }

        [XmlElement("MailCharset")]
        public string MailCharset;

        [XmlElement("MailFrom")]
        public string MailFrom;

        [XmlElement("MailFromName")]
        public string MailFromName;

        [XmlElement("MailServer")]
        public string MailServer;

        [XmlElement("MailUserName")]
        public string MailUserName;

        [XmlElement("MailUserPassword")]
        public string MailUserPassword;

        [XmlElement("MailLord")]
        public string MailLord;
    }

    #endregion

    #region AuctionConfiguration Class
    /// <summary>
    /// Auction Configuration Class
    /// </summary>
    [XmlRoot("AuctionConfiguration")]
    public class AuctionConfiguration
    {
        public AuctionConfiguration()
        {
        }

        [XmlElement("BidInfoXmlPath")]
        public string BidInfoXmlPath;

        [XmlElement("IsAuction")]
        public string IsAuction;

        [XmlElement("DelayTime")]
        public string DelayTime;

        [XmlElement("AuctionBeginTime")]
        public string AuctionBeginTime;

        [XmlElement("AuctionEndTime")]
        public string AuctionEndTime;

        [XmlElement("AuctionAdmin")]
        public string AuctionAdmin;
    }
    #endregion

    #region HBXConfiguration Class
    /// <summary>
    /// HBX Configuration Class
    /// </summary>
    [XmlRoot("HBXConfiguration")]
    public class HBXConfiguration
    {
        public HBXConfiguration()
        {
        }

        [XmlElement("HBX_Required")]
        public string HBX_Required;

        [XmlElement("HBX_JSLocation")]
        public string HBX_JSLocation;

        [XmlElement("HBX_InternalUseOnlyGN")]
        public string HBX_InternalUseOnlyGN;

        [XmlElement("HBX_GroupName")]
        public string HBX_GroupName;

        [XmlElement("HBX_ACCT")]
        public string HBX_ACCT;

        [XmlElement("HBX_EACCT")]
        public string HBX_EACCT;

        [XmlElement("HBX_CACCT")]
        public string HBX_CACCT;

        [XmlElement("HBX_Store_Default")]
        public string HBX_Store_Default;

        [XmlElement("HBX_Store_Hardware")]
        public string HBX_Store_Hardware;

        [XmlElement("HBX_Store_DigitalProduct")]
        public string HBX_Store_DigitalProduct;

        [XmlElement("HBX_Store_Attachment")]
        public string HBX_Store_Attachment;

        [XmlElement("HBX_Store_SoundDevice")]
        public string HBX_Store_SoundDevice;

        [XmlElement("HBX_Store_ITLife")]
        public string HBX_Store_ITLife;

        [XmlElement("HBX_Store_BrandSale")]
        public string HBX_Store_BrandSale;
    }
    #endregion

    #region CommonConfiguration Class
    /// <summary>
    /// Common Configuration Class
    /// </summary>
    [XmlRoot("CommonConfiguration")]
    public class CommonConfiguration
    {
        public CommonConfiguration()
        {
        }

        [XmlElement("PMCollection")]
        public string PMCollection;

        [XmlElement("FMCollection")]
        public string FMCollection;

        [XmlElement("IPPrintCollection")]
        public string IPPrintCollection;

        [XmlElement("FMPrintCollection")]
        public string FMPrintCollection;


        [XmlElement("RMAHandlerNext")]
        public string RMAHandlerNext;

        [XmlElement("RMAandCCMail")]
        public string RMAandCCMail;

        [XmlElement("DayReportMail")]
        public string DayReportMail;

        [XmlElement("DayNegativeMail")]
        public string DayNegativeMail;

        //Add by tomato 2006-10-30 投訴系統
        [XmlElement("ComplainType")]
        public string ComplainType;

        [XmlElement("ComplainSourceType")]
        public string ComplainSourceType;

        [XmlElement("ReplySourceType")]
        public string ReplySourceType;

        [XmlElement("ComplainMailFrom")]
        public string ComplainMailFrom;

        [XmlElement("ComplainMailBcc")]
        public string ComplainMailBcc;
        //Add by tomato 2006-10-30 投訴系統

        // Add by tomato 2006-12-11 [CS]IPP投訴管理系統添加統計功能應用的需求 Begin
        [XmlElement("ComplainResponsibleDept")]
        public string ComplainResponsibleDept;
        // Add by tomato 2006-12-11 [CS]IPP投訴管理系統添加統計功能應用的需求 End

        //Add by tomato 2006-11-30 Partly Receive
        [XmlElement("PartlyReceiveReason")]
        public string PartlyReceiveReason;

        [XmlElement("InStockCC")]
        public string InStockCC;
        //Add by tomato 2006-11-30 Partly Receive

        // Add by tomato 2006-12-11 供應商管理 Begin
        [XmlElement("VendorFilePath")]
        public string VendorFilePath;

        [XmlElement("VendorURLPath")]
        public string VendorURLPath;

        [XmlElement("VendorAgentLevel")]
        public string VendorAgentLevel;
        // Add by tomato 2006-12-11 供應商管理 End

        // Add by tomato 2006-12-19 價格審核 Begin
        [XmlElement("PriceVerifyEmail")]
        public string PriceVerifyEmail;
        // Add by tomato 2006-12-19 價格審核 End

        [XmlElement("VendorVerifyEmail")]
        public string VendorVerifyEmail;

        [XmlElement("CategoryVerifyEmail")]
        public string CategoryVerifyEmail;

        [XmlElement("ExpiredSOMailList")]
        public string ExpiredSOMailList;

        [XmlElement("OpenCheckQtyLog")]
        public Boolean OpenCheckQtyLog;

        //ＲＭＡ數據同步開關
        [XmlElement("OpenRMADataTran")]
        public Boolean OpenRMADataTran;

        // Add by tomato 2006-02-01 [PM]PIM后臺追加
        [XmlElement("PIMURL")]
        public string PIMURL;

        // Add by tomato 2007-04-03 WarehouseNumber
        [XmlElement("WarehouseNumber")]
        public string WarehouseNumber;

        // Add by Robin 2007-06-01  圖片稍后即來的圖片URL
        [XmlElement("ProductInitializePic")]
        public string ProductInitializePic;

        // Add by Shadow 2007-08-03 JD價格導入IPP
        [XmlElement("UpFilePath_ImportJDPrice")]
        public string UpFilePath_ImportJDPrice;

        [XmlElement("UpFilePath_ImportAstraZenecaCustomer")]
        public string UpFilePath_ImportAstraZenecaCustomer;

        [XmlElement("SVRemindMailToPMList")]
        public string SVRemindMailToPMList;

        [XmlElement("SVRemindMailToCCList")]
        public string SVRemindMailToCCList;

        [XmlElement("IsSendSVRemindMail")]
        public string IsSendSVRemindMail;

        [XmlElement("OutboundCCMail")]
        public string OutboundCCMail;
        //Add by Robin 2007-11-05 產品價格修改TL 的郵件通知
        [XmlElement("AuditProductPrice1")]
        public string AuditProductPrice1;

        //Add by Robin 2007-11-05 產品價格修改PM主管的郵件通知
        [XmlElement("AuditProductPrice2")]
        public string AuditProductPrice2;

        //Add by Robin 2007-12-05 實庫圖片拍攝Item 的Newegg Support 通知郵件
        [XmlElement("SendMailToPs")]
        public string SendMailToPs;

        //Add by Robin 2007-12-07 更新，新增地區郵件發送地址
        [XmlElement("SendMailForAddOrUpdateArea")]
        public string SendMailForAddOrUpdateArea;

        [XmlElement("FetchByOneSelf_Warehouse")]
        public string FetchByOneSelf_Warehouse;
        [XmlElement("FetchByOneSelf_Station")]
        public string FetchByOneSelf_Station;

        [XmlElement("SendMailForNewVendorToPM")]
        public string SendMailForNewVendorToPM;

        [XmlElement("ProductQuestionUserSysnoList")]
        public string ProductQuestionUserSysnoList;

        /// <summary>
        /// 新蛋快遞物流配送方式列表
        /// </summary>
        [XmlElement("NeweggShipType")] // Kevin 2008-3-5
        public string NeweggShipType;

        /// <summary>
        /// 自提配送方式列表
        /// </summary>
        [XmlElement("SelfFetchShipType")] // Kevin 2008-3-5
        public string SelfFetchShipType;

        /// <summary>
        /// 投訴即將超期提醒郵件列表 
        /// </summary>
        [XmlElement("ComplainOutdatedAlertMailList")]  // Kevin 2008-2-26
        public string ComplainOutdatedAlertMailList;

        /// <summary>
        /// 自動RMA處理系統財務確認人員 
        /// </summary>
        [XmlElement("RMAAuto")]  // Kevin 2008-4-15
        public string RMAAuto;

        //Addy by Andy 2008-01-24 退壞貨入庫審核人郵件地址
        [XmlElement("RMAReturnBadMailList")]
        public string RMAReturnBadMailList;
        [XmlElement("RMAReturnBadCCMailList")]
        public string RMAReturnBadCCMailList;
        //add by andy 2008-03-21 退二手品入庫抄送郵件地址
        [XmlElement("ReturnSecondHandsCCEmail")]
        public string ReturnSecondHandsCCEmail;

        [XmlElement("SendMailReportPerMonth")]
        public string SendMailReportPerMonth;

        [XmlElement("CustVATCertificateFilePath")]
        public string CustVATCertificateFilePath;

        [XmlElement("CustVATCertificateURLPath")]
        public string CustVATCertificateURLPath;

        [XmlElement("CSConfimComplainType")]
        public string CSConfimComplainType;

        [XmlElement("ProductComplainType")]
        public string ProductComplainType;

        [XmlElement("StockComplainType")]
        public string StockComplainType;

        [XmlElement("CSComplainType")]
        public string CSComplainType;

        [XmlElement("SaleComplainType")]
        public string SaleComplainType;

        [XmlElement("MarketComplainType")]
        public string MarketComplainType;

        [XmlElement("OtherComplainType")]
        public string OtherComplainType;
        [XmlElement("MSMQException")]
        public string MSMQException;
        [XmlElement("RMARevertOutDate")]
        public string RMARevertOutDate;
        [XmlElement("OZZOProductControllerMail")]
        public string OZZOProductControllerMail;

        [XmlElement("OutStockMailAddr")]
        public string OutStockMailAddr;

        [XmlElement("InventoryMailAddress")]
        public string InventoryMailAddress;

        [XmlElement("ReportOfPayListAddress")]
        public string ReportOfPayListAddress;    //PO應付款通知郵件

        [XmlElement("GiftRenewQtyAddress")]   //Daemon.SendGiftRenewQtyEmail
        public string GiftRenewQtyAddress;

        [XmlElement("GiftRenewQtyCCAddress")]
        public string GiftRenewQtyCCAddress;

        [XmlElement("SaleTop200Email")]     //StockManager.SendSaleTop200Email
        public string SaleTop200Email;
        /// <summary>
        /// ProductManager.GetInstance().SendLastWeekNewShowItem();
        /// ProductManager.GetInstance().SendLastWeekPurchaseItem();
        /// ProductManager.GetInstance().SendAllShowItem();
        /// </summary>
        [XmlElement("EOAWeeklyReportItemEmail")]//
        public string EOAWeeklyReportItemEmail;
        /// <summary>
        /// EIMS 采購單扣減銷售成本 生效時間
        /// </summary>
        [XmlElement("EIMSReduceSaleCostDate")]
        public string EIMSReduceSaleCostDate;
    }

    #endregion

    #region FinanceConfiguration Class
    /// <summary>
    /// Finance Configuration Class
    /// </summary>
    [XmlRoot("FinanceConfiguration")]
    public class FinanceConfiguration
    {
        public FinanceConfiguration()
        {
        }

        [XmlElement("UpFilePath_AutoConfirmPage")]
        public string UpFilePath_AutoConfirmPage;

        [XmlElement("MKTPointAlarmEmailAdd")]
        public string MKTPointAlarmEmailAdd;
        //PM積分報警
        [XmlElement("PMPointAlarmEmailAdd")]
        public string PMPointAlarmEmailAdd;

        //PM積分報警
        [XmlElement("PMPointAlarmEmailAddForRicoh")]
        public string PMPointAlarmEmailAddForRicoh;


        [XmlElement("ShippingFeePromationEmailAdd")]
        public string ShippingFeePromationEmailAdd;

        [XmlElement("KuLingReportEmailAdd")]
        public string KuLingReportEmailAdd;

        [XmlElement("PayListReportEmailAdd")]
        public string PayListReportEmailAdd;

        [XmlElement("IsSendPayListReportEmail")]
        public string IsSendPayListReportEmail;

        [XmlElement("InventoryWeeklyReportEmailAddress")]
        public string InventoryWeeklyReportEmailAddress;


    }
    #endregion

    #endregion
}
