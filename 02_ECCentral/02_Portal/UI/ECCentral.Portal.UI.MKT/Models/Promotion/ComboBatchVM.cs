using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComboBatchVM : ModelBase
    {
        public ComboBatchVM()
        {
            this.ComboTypeList = EnumConverter.GetKeyValuePairs<ComboType>();
            this.SaleRuleType = ComboType.Common;

            this.StatusList = new List<KeyValuePair<ComboStatus?,string>>();
            this.StatusList.Insert(0, new KeyValuePair<ComboStatus?, string>(ComboStatus.Deactive, EnumConverter.GetDescription(ComboStatus.Deactive)));
            this.Status = ComboStatus.Deactive;

            this.Priority = "9999";
        }

        private string name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetValue("Name", ref name, value);
            }
        }

        private bool isShowName;
        /// <summary>
        /// 前台显示套餐名称
        /// </summary>
        public bool IsShowName
        {
            get
            {
                return isShowName;
            }
            set
            {
                SetValue("IsShowName", ref isShowName, value);
            }
        }

        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get
            {
                return priority;
            }
            set
            {
                SetValue("Priority", ref priority, value);
            }
        }

        private ComboStatus? status;
        public ComboStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status", ref status, value);
            }
        }

        private ComboType? saleRuleType;
        /// <summary>
        /// 类型
        /// </summary>
        public ComboType? SaleRuleType
        {
            get
            {
                return saleRuleType;
            }
            set
            {
                SetValue("SaleRuleType", ref saleRuleType, value);
            }
        }

        private string mqty;
        /// <summary>
        /// 主数量
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessageResourceName = "Validate_PlusInteger", ErrorMessageResourceType = typeof(ResComboSaleBatchMaintain))]
        [Validate(ValidateType.Required)]
        public string MQty
        {
            get
            {
                return mqty;
            }
            set
            {
                SetValue("MQty", ref mqty, value);
            }
        }

        private string mDiscount;
        /// <summary>
        /// 主单件折扣
        /// </summary>
        [Validate(ValidateType.Regex, @"^-\d+|0$", ErrorMessageResourceName = "Validate_NegativeInteger", ErrorMessageResourceType = typeof(ResComboSaleBatchMaintain))]
        [Validate(ValidateType.Required)]
        public string MDiscount
        {
            get
            {
                return mDiscount;
            }
            set
            {
                SetValue("MDiscount", ref mDiscount, value);
            }
        }

        private string discountRate;
        /// <summary>
        /// 次单件折扣率
        /// </summary>
        [Range(0.00, 1.00, ErrorMessageResourceName = "Validate_Between0And1", ErrorMessageResourceType = typeof(ResComboSaleBatchMaintain))]
        public string DiscountRate
        {
            get
            {
                return discountRate;
            }
            set
            {
                SetValue("DiscountRate", ref discountRate, value);
            }
        }

        private string discount;
        /// <summary>
        /// 次单件折扣
        /// </summary>
        [Validate(ValidateType.Regex, @"^-\d+|0$", ErrorMessageResourceName = "Validate_NegativeInteger", ErrorMessageResourceType = typeof(ResComboSaleBatchMaintain))]
        [Validate(ValidateType.Required)]
        public string Discount
        {
            get
            {
                return discount;
            }
            set
            {
                SetValue("Discount", ref discount, value);
            }
        }

        private string qty;
        /// <summary>
        /// 此数量
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessageResourceName = "Validate_PlusInteger", ErrorMessageResourceType = typeof(ResComboSaleBatchMaintain))]
        [Validate(ValidateType.Required)]
        public string Qty
        {
            get
            {
                return qty;
            }
            set
            {
                SetValue("Qty", ref qty, value);
            }
        }

        /// <summary>
        /// 主绑定商品
        /// </summary>
        public List<int> MasterItems { get; set; }

        /// <summary>
        /// 次绑定商品
        /// </summary>
        public List<int> Items { get; set; }


        public List<KeyValuePair<ComboType?, string>> ComboTypeList { get; set; }
        public List<KeyValuePair<ComboStatus?, string>> StatusList { get; set; }
    }
}
