using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    /// <summary>
    /// 供应商佣金信息
    /// </summary>
    public class VendorCommissionInfoVM : ModelBase
    {

        private int? commissionSysNo;

        public int? CommissionSysNo
        {
            get { return commissionSysNo; }
            set { base.SetValue("CommissionSysNo", ref commissionSysNo, value); }
        }

        public VendorCommissionInfoVM()
        {
            saleRuleEntity = new VendorStagedSaleRuleEntityVM();
        }
        /// <summary>
        /// 店租佣金
        /// </summary>
        private string rentFee;

        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string RentFee
        {
            get { return rentFee; }
            set { base.SetValue("RentFee", ref rentFee, value); }
        }

        /// <summary>
        /// 销售提成 - 阶梯设置
        /// </summary>
        private VendorStagedSaleRuleEntityVM saleRuleEntity;

        public VendorStagedSaleRuleEntityVM SaleRuleEntity
        {
            get { return saleRuleEntity; }
            set { base.SetValue("SaleRuleEntity", ref saleRuleEntity, value); }
        }

        /// <summary>
        /// 保底金额
        /// </summary>
        private string guaranteedAmt;

        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string GuaranteedAmt
        {
            get { return guaranteedAmt; }
            set { base.SetValue("GuaranteedAmt", ref guaranteedAmt, value); }
        }
        /// <summary>
        /// 订单提成
        /// </summary>
        private string orderCommissionAmt;

        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string OrderCommissionAmt
        {
            get { return orderCommissionAmt; }
            set { base.SetValue("OrderCommissionAmt", ref orderCommissionAmt, value); }
        }

        /// <summary>
        /// 配送费
        /// </summary>
        private string deliveryFee;

        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string DeliveryFee
        {
            get { return deliveryFee; }
            set { base.SetValue("DeliveryFee", ref deliveryFee, value); }
        }
    }
}
