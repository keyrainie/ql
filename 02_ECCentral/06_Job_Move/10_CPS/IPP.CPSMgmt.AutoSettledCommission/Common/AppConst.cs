using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Common
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public class AppConst
    {
        #region CDK : contextData Key
        /*
         * CDK : contextData Key
         */
        public const string CDK_IPPSearchDatas = "CDK_IPPSearchDatas";

        public const string CDK_IPPUploadDatas = "CDK_IPPUploadDatas";
        
        public const string CDK_SAPWriteDatas = "CDK_SAPWriteDatas";

        /// <summary>
        /// 对账周期
        /// </summary>
        public const string CDK_ComparePeriod = "CDK_ComparePeriod";
        public const string CDK_ComparePeriodStr = "CDK_ComparePeriodStr";
        /// <summary>
        /// Compare Results【比较结果】
        /// </summary>
        public const string CDK_CR_IPPSearchToIPPUpload = "CDK_CR_IPPSearchToIPPUpload";
        public const string CDK_CR_IPPUploadToSAPWrite = "CDK_CR_IPPUploadToSAPWrite";

        #endregion CDK : contextData Key

        #region Xml File Name
        /*
         * Xml File Name
         */
        public const string XFN_Report_FincialPay = "Report_FincialPay.xml";
        #endregion Report File Name

        #region Report File Name
        /*
         * Report File Name
         */
        public const string RFN_FincialPay_MonthUploadToSAP = "ReportMonth_UploadToSAP";
        public const string RFN_FincialPay_DiffUploadToSAP = "ReportDifferent_UploadToSAP";
        public const string RFN_FincialPay_IPPAmtException = "ReportMonth_IPPAmtException";
        
        
        #endregion Report File Name

        #region Report File Name
        /*
         * Report File Name
         */
        public const int InitSize_General = 10;
        public const int InitSize_ValidateResultList = 10;
        public const int InitSize_FinancialPayDiff = 10;
        public const int InitSize_CompareFinancialPayResult = 50;
        #endregion Init List Size        

        /*
         * Data Format Style
         */
        public const string DFS_Decimal_General = "#0.00";
        public const string DFS_DateTime_General = "yyyy-MM-dd";
        public const string DFS_Empty_TD = "&nbsp;";

        public const string RowStyle_Error = "rowStyle_Error";
        public const string RowStyle_Normal = "rowStyle_Normal";
    }
}
