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

       
    }
}
