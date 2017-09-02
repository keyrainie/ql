/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Phoebe Zhang (Phoebe.F.Zhang@Newegg.com)
 *  Date:    2009-05-20 19:42:45
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{
    /// <summary>
    /// Summary description for AppConst.
    /// </summary>
    public class AppConst
    {
        #region 系統中判斷未賦值的判斷，只可以用于比較判斷，不能用于賦值

        public const string StringNull = "";

        public const int IntNull = -999999;
        public const decimal DecimalNull = -999999;
        public const float FloatNull = -999999;  //Add By Teracy
        public const float DoubleNull = -999999;  //Add By Teracy 


        public static DateTime DateTimeNull = DateTime.Parse("1900-01-01");

        #endregion

        public const string AllSelectString = "--- ALL ---";
        public const string DecimalToInt = "#########0"; //用于point的顯示，一般來說currentprice應該沒有分。
        public const string DecimalFormat = "#########0.00";
        public const string DecimalFormatWithGroup = "#,###,###,##0.00";
        public const string IntFormatWithGroup = "#,###,###,##0";
        public const string DecimalFormatWithCurrency = "￥#########0.00";
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateTimeFormatA = "HHmmss";
        public const string DateFormatA = "yyyyMMdd";
        public const string DateFormatLong = "yyyy-MM-dd HH:mm:ss";
        public const string DecimalFormatForPayRate = "#########0.000";//payrate支持小數點后三位
        public const int SysUserSysNo = 493;      //系統賬號的sysno

        //#region 市場活動
        ////空調類別
        //public const int AirConditionC3No=667;//966;
        //public const int AirConditionShipType = 62;

        //#endregion

        //3PL分攤新蛋快遞
        // public const int Shipping3PLForNewegg = 12;

        //用于顯示信息重復
        public const int InfoRepeat = -1;

        //用于表示子表已用到此記錄，不可刪除
        public const int CanNotDel = -1;

        //用于DataGrid中每頁的記錄數
        public const int PageSize = 50;

        //用于DataGrid中每頁按鈕的數目
        public const int PageButtonCount = 5;

        /// <summary>
        /// 積分和現金RMB轉換比例 Point = Cash*ExchangeRate
        /// </summary>
        public const decimal ExchangeRate = 10m;

        // 每張發票的最大行數
        public const int PageMaxLine = 14;

        // 每行名稱的最大長度
        public const int NameMaxLength = 30;

        // 地址的最大長度，超過長都會被截去一部分
        public const int AddressMaxLength = 100;

        // 商品價格的缺省值
        public const decimal DefaultPrice = 999999m;

        // 前臺生成訂單對應的銷售人員編號
        public const int NeweggSalesMan = 0;
        public const string NeweggSalesManName = "新蛋網";

        // 系統操作Log，默認ip地址
        public const string SysIP = "127.0.0.1";

        // 系統操作Log，默認User
        public const int SysUser = 0;

        //最多出價次數 KiddLiu 2005/07/01
        public const int MaxNumberOfBids = 3;

        //廣告的文件名 MulburryFu 2005/7/27
        public const string AdvAuctionCenter = "auction\\AdvAuctionCenter.htm";
        public const string AdvAuctionBulletin = "auction\\AdvAuctionBulletin.htm";
        public const string AdvAuctionRight = "auction\\AdvAuctionRight.htm";
        public const string AdvAuctionRule = "auction\\AdvAuctionRule.htm";

        // Auction V2.0 -- By DavidLiu 2005-09-10
        public const string AdvAuctionTopLeft = "auction\\AdvAuctionTopLeft.htm";
        public const string AdvAuctionNotice = "auction\\AdvAuctionNotice.htm";
        //外網BBS/BLOG宣傳新蛋網活動
        public const string AdvSpreadURL = "AdvSpreadURLContext.htm";

        // 逆向拍賣時默認降價幅度
        public const decimal DefaultDepInterval = 1m;

        //用戶瀏覽記錄Cookie名稱 added by seanqu 2006/05/08
        public const string COOKIE_BROWSED_PRODUCT = "BrowsedProductSysNoList";

        #region HBX所需Cookies
        public const string COOKIE_HBX_STORE_VALUE = "HBX_Store_Value";

        public const string COOKIE_HBX_ADD_PRODUCT_INFO = "HBX_Add_Product_Info";

        #endregion

        // KEY: 快錢返現比例 -- David Liu @ 2006-09-06
        // shadow 2007-02-02 修改5%為4% 
        public const string FastCashRebate_4_Percent = "FastCashRebate_4_Percent";
        public const string FastCashRebate_3_Percent = "FastCashRebate_3_Percent";
        public const decimal maxFastCashAmount = 200m;

        #region Topic (評論系統) 配置信息使用的 KEY

        // Topic (評論系統) 配置信息使用的 KEY -- David Liu @ 2006-10-13
        public const string Topic_CanPostTopic = "Topic_CanPostTopic";
        public const string Topic_CanPostReply = "Topic_CanPostReply";
        public const string Topic_PostTopic_CustomerRankLimit = "Topic_PostTopic_CustomerRankLimit";
        public const string Topic_PostReply_CustomerRankLimit = "Topic_PostReply_CustomerRankLimit";
        public const string Topic_Can_AutoSetDigest = "Topic_Can_AutoSetDigest";
        public const string Topic_Score_AutoSetDigest = "Topic_Score_AutoSetDigest";
        public const string Topic_PointBonus_AutoSetDigest = "Topic_PointBonus_AutoSetDigest";
        public const string Topic_SendPoint_BySetDigest = "Topic_SendPoint_BySetDigest";
        public const string Topic_PostTopic_Everyday_CountLimit = "Topic_PostTopic_Everyday_CountLimit";
        public const string Topic_PostReply_Everyday_CountLimit = "Topic_PostReply_Everyday_CountLimit";
        #endregion

        #region 添加RMA貨卡查詢起始時間
        public static DateTime RMA_Initializtion_DateTime = DateTime.Parse("2007-01-01");

        #endregion

        /// <summary>
        /// Holiday - 無法新蛋快遞配送（休息日）
        /// ---- David Liu @ 2006-11-16
        /// </summary>
        public const string UnableDeliveryHoliday = "UnableDeliveryHoliday";

        /// <summary>
        /// Sunday - 可以新蛋快遞配送（如長假休息日調整）
        /// ---- David Liu @ 2006-11-17
        /// </summary>
        public const string CanDeliverySunday = "CanDeliverySunday";

        /// <summary>
        /// 新蛋快遞一天一送
        /// </summary>
        public const string OneTimeOneDay = "OneTimeOneDay";
        /// <summary>
        /// 標準節假日
        /// </summary>
        public const string NormalHoliday = "NormalHoliday";
        /// <summary>
        /// 價格舉報Category 
        /// Shadow 2007-01-09
        /// </summary>
        public const string CategoryListForPriceCompare = "CategoryListForPriceCompare";

        /// <summary>
        /// 可配置類別結構（導航欄 )
        /// Alan 2007-01-22
        /// </summary>
        public const string CategoryCustomized = "CategoryCustomized";

        /// <summary>
        /// 配置外網新注冊用戶的合作網站
        /// Shadow 2007-01-23
        /// </summary>
        public const string WebNewUserFrom = "WebNewUserFrom";

        /// <summary>
        /// ACER商品sysno列表緩存
        /// alan 2007-08-01
        /// </summary>
        public const string AcerProductSysNoList = "AcerProductSysNoList";

        // ACER 優惠券，50/100面值的 PromotionMasterSysNo -- CACHE KEY 
        public const string AcerPromotion50SysNoKEY = "AcerPromotion50SysNo";
        public const string AcerPromotion100SysNoKEY = "AcerPromotion100SysNo";

        // ACER 第三期優惠券，100面值的  CACHE KEY 
        public const string AcerSummerPromotionSysNoKEY = "AcerSummerPromotionSysNo";

        // ACER 包藏愛心活動優惠券  CACHE KEY 
        public const string AcerAutumnNBPromotionSysNoKEY = "AcerAutumnNBPromotionSysNo";
        public const string AcerAutumnNBQuestionListKEY = "AcerAutumnNBQuestionList";
        public const string AcerAutumnNBGiftSysNoListKEY = "AcerAutumnNBGiftSysNoList";

        // ACER LCD活動優惠券號和問卷調查
        public const string AcerLcdRegPromotionSysNoKEY = "AcerLcdRegPromotionSysNo";
        public const string AcerLcdRegQuestionListKEY = "AcerLcdRegQuestionList";

        /// <summary>
        /// Acer 免費贈送的商品 SysNo 列表 -- Cache KEY
        /// </summary>
        public const string AcerGiftSysNoListKEY = "AcerGiftSysNoList";

        /// <summary>
        /// Acer 免費贈送錄取生的商品 SysNo 列表 -- Cache KEY
        /// </summary>
        public const string AcerStudentGiftSysNoListKEY = "AcerStudentGiftSysNoList";

        /// <summary>
        /// Acer 第二期優惠特殊商品 SysNo -- Cache KEY
        /// </summary>
        public const string AcerCouponProductSysNo_CouponOnlyKEY = "AcerCouponProductSysNo_CouponOnly";

        /// <summary>
        /// Acer 問卷調查列表 -- Cache KEY
        /// </summary>
        public const string AcerQuestionListKEY = "AcerQuestionList";
        public const string AcerCouponRegisterListKEY = "AcerCouponRegisterList";

        // Homepage ProductList TOP / UP/ Down -- CACHE KEY 
        public const string HomepageProductListTopKEY = "HomepageProductListTop";
        public const string HomepageProductListUpKEY = "HomepageProductListUp";
        public const string HomepageProductListDownKEY = "HomepageProductListDown";

        #region 會員專區
        //活躍會員排行
        public const string MemberArea_Top10ActiveCust = "MemberArea_Top10ActiveCust";
        public const string MemberArea_Top10TopicCust = "MemberArea_Top10TopicCust";
        public const string MemberArea_Top2UesfulTopicCust = "MemberArea_Top2UesfulTopicCust";
        public const string MemberArea_Top1RecommendCust = "MemberArea_Top1RecommendCust";

        #endregion
        /// <summary>
        /// 有效期最小值，單位：月
        /// </summary>
        public const int PointMinExpiring = 3;
        /// <summary>
        /// 有效期最大值，單位：月
        /// </summary>
        public const int PointMaxExpiring = 24;

        /// <summary>
        /// 廣告效果監測系統－Cookie.Value & 類型
        /// </summary>
        public const string Cookie_AdvEffectMonitor_CMP_Value = "Cookie_AdvEffectMonitor_CMP_Value";
        public const string AdvEffectMonitor_Type_Register = "用戶注冊Register";
        public const string AdvEffectMonitor_Type_PlaceOrder = "生成訂單PlaceOrder";

        //CPS廣告聯盟CookieName 
        public const string Cookie_CPS = "CPS";
        public const string Cookie_CPA = "CPA";

        //訂單作廢問卷調查
        public const string CancelSoReason = "CancelSoReason";

        #region 在代碼里寫死的小類 C3SysNo

        public const int C3_DigitalCamera_SysNo = 655;
        public const int C3_MobilePhone_SysNo = 651;
        public const int C3_StorageCard_SysNo = 663;
        public const int C3_LCD_SysNo = 582;
        public const int C3_Laptop_SysNo = 586;
        public const int C3_AcerAccessories_SysNo = 588;

        #endregion

        /// <summary>
        /// 分期付款銀行SysNo
        /// </summary>
        /// 
        #region Add by Mike Li 2008-09-19
        //中國銀行 
        public const int ChinaBankSysNo = 1;
        #endregion

        //民生
        public const int MingShengBankSysNo = 2;
        //招商
        public const int ZhaoShangBankSysNo = 3;

        /// <summary>
        /// Acer 專用快遞的 SysNo 1
        /// </summary>
        public const int AcerShipType = 24;

        /// <summary>
        /// Acer 專用快遞的 SysNo 2
        /// </summary>
        public const int AcerShipType2 = 23;

        /// <summary>
        /// AstraZeneca 專用快遞的 SysNo 
        /// </summary>
        public const int AstraZenecaShipType = 45;
        /// <summary>
        /// AstraZeneca EMS快遞的 SysNo 
        /// </summary>
        public const int AstraZenecaEMSShipType = 50;


        /// <summary>
        /// POS 機 -- 支付方式 SysNo
        /// </summary>
        public const int POS_PayType_SysNo = 20;

        /// <summary>
        /// 運費促銷四省市  ProvinceSysNo
        /// </summary>
        public const int AnHui = 1;
        public const int JiangShu = 1591;
        public const int ShangHai = 2621;
        public const int ZheJiang = 3225;


        /// <summary>
        /// 運費促銷開始時間
        /// </summary>
        public static DateTime ShippingFeePromotionDateTime = DateTime.Parse("2007-05-15 13:00:00 ");

        #region 分倉項目，各個倉庫的SysNo -- David Liu @ 2007-06-30
        public const int Warehouse_SH = 51; // shangHai
        public const int Warehouse_BJ = 52; // BeiJing
        public const int WareHouse_GZ = 53; // Guangzhou
        public const int WareHouse_CD = 54; // ChengDu
        public const int WareHouse_WH = 55; // ChengDu
        #endregion

        #region  配送員關聯配送方式-- Philo @ 2007-07-04
        public const int NewEggSD_SH = 21;
        public const int NewEggSD_BJ = 28;
        #endregion
        public const int RemindEmail_PM_FillPurchaseInfo = 0;
        public const int RemindEmail_PM_WaitingForStock = 1;
        public const int RemindEmail_CS_AuditSo = 2;
    }
}
