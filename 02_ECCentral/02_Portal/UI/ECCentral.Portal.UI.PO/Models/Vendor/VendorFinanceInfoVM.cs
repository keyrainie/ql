using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorFinanceInfoVM : ModelBase
    {

        public VendorFinanceInfoVM()
        {
            payPeriodType = new VendorPayTermsItemInfoVM();
            financeRequestInfo = new VendorModifyRequestInfoVM();
            cooperateAmt = "0";
        }

        /// <summary>
        /// 开户行
        /// </summary>
        private string bankName;
        [Validate(ValidateType.Required)]
        public string BankName
        {
            get { return bankName; }
            set { base.SetValue("BankName", ref bankName, value); }
        }

        /// <summary>
        /// 税号
        /// </summary>
        private string taxNumber;

        public string TaxNumber
        {
            get { return taxNumber; }
            set { base.SetValue("TaxNumber", ref taxNumber, value); }
        }

        /// <summary>
        /// 财务联系人
        /// </summary>
        private string accountContact;
        [Validate(ValidateType.Required)]
        public string AccountContact
        {
            get { return accountContact; }
            set { base.SetValue("AccountContact", ref accountContact, value); }
        }

        /// <summary>
        /// 电话
        /// </summary>
        private string accountPhone;
        [Validate(ValidateType.Required)]
        public string AccountPhone
        {
            get { return accountPhone; }
            set { base.SetValue("AccountPhone", ref accountPhone, value); }
        }

        /// <summary>
        /// 财务联系人电子邮箱
        /// </summary>
        private string accountContactEmail;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Email)]
        public string AccountContactEmail
        {
            get { return accountContactEmail; }
            set { base.SetValue("AccountContactEmail", ref accountContactEmail, value); }
        }


        /// <summary>
        /// 账号
        /// </summary>
        private string accountNumber;
        [Validate(ValidateType.Required)]
        public string AccountNumber
        {
            get { return accountNumber; }
            set { base.SetValue("AccountNumber", ref accountNumber, value); }
        }

        /// <summary>
        /// 账期
        /// </summary>
        private string payPeriod;
        [Validate(ValidateType.Interger)]
        public string PayPeriod
        {
            get { return payPeriod; }
            set { base.SetValue("PayPeriod", ref payPeriod, value); }
        }

        /// <summary>
        /// 账期类型
        /// </summary>
        private VendorPayTermsItemInfoVM payPeriodType;

        public VendorPayTermsItemInfoVM PayPeriodType
        {
            get { return payPeriodType; }
            set { base.SetValue("PayPeriodType", ref payPeriodType, value); }
        }

        /// <summary>
        /// 结算方式
        /// </summary>
        private VendorSettlePeriodType? settlePeriodType;

        public VendorSettlePeriodType? SettlePeriodType
        {
            get { return settlePeriodType; }
            set { base.SetValue("SettlePeriodType", ref settlePeriodType, value); }
        }

        /// <summary>
        /// 是否自动审核
        /// </summary>
        private bool? isAutoAudit;

        public bool? IsAutoAudit
        {
            get { return isAutoAudit; }
            set { base.SetValue("IsAutoAudit", ref isAutoAudit, value); }
        }

        /// <summary>
        /// 是否自动审核 - 显示
        /// </summary>
        private string isAutoAuditDisplayString;

        public string IsAutoAuditDisplayString
        {
            get { return isAutoAuditDisplayString; }
            set { base.SetValue("IsAutoAuditDisplayString", ref isAutoAuditDisplayString, value); }
        }

        /// <summary>
        /// 是否代销
        /// </summary>
        private VendorConsignFlag? consignFlag;

        public VendorConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { base.SetValue("ConsignFlag", ref consignFlag, value); }
        }

        /// <summary>
        /// 合作生效日期
        /// </summary>
        private DateTime? cooperateValidDate;
        public DateTime? CooperateValidDate
        {
            get { return cooperateValidDate; }
            set { base.SetValue("CooperateValidDate", ref cooperateValidDate, value); }
        }

        /// <summary>
        /// 合作过期日期
        /// </summary>
        private DateTime? cooperateExpiredDate;
        public DateTime? CooperateExpiredDate
        {
            get { return cooperateExpiredDate; }
            set { base.SetValue("CooperateExpiredDate", ref cooperateExpiredDate, value); }
        }

        /// <summary>
        /// 合作金额
        /// </summary>
        private string cooperateAmt;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string CooperateAmt
        {
            get { return cooperateAmt; }
            set { base.SetValue("CooperateAmt", ref cooperateAmt, value); }
        }

        /// <summary>
        /// 累计金额
        /// </summary>
        private decimal? totalPOAmt;

        public decimal? TotalPOAmt
        {
            get { return totalPOAmt; }
            set { base.SetValue("TotalPOAmt", ref totalPOAmt, value); }
        }

        /// <summary>
        /// 财务信息审核状态
        /// </summary>
        private VendorModifyRequestStatus? verifyStatus;

        public VendorModifyRequestStatus? VerifyStatus
        {
            get { return verifyStatus; }
            set { base.SetValue("VerifyStatus", ref verifyStatus, value); }
        }

        private VendorModifyRequestInfoVM financeRequestInfo;

        public VendorModifyRequestInfoVM FinanceRequestInfo
        {
            get { return financeRequestInfo; }
            set { base.SetValue("FinanceRequestInfo", ref financeRequestInfo, value); }
        }
    }
}
