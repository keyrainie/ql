using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class OptionalAccessoriesVM : ModelBase
    {
        public OptionalAccessoriesVM()
        {
            this.Priority = "9999";
            this.IsDeactive = true;
            this.ComboTypeList = EnumConverter.GetKeyValuePairs<ComboType>();
            this.SaleRuleType = ComboType.Common;
            this.Items = new ObservableCollection<OptionalAccessoriesItemVM>();
            this.DisplayApproveMsg = new ObservableCollection<string>();
        }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
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
                if (value == ComboStatus.Active)
                {
                    IsActive = true;
                }
                else if (value == ComboStatus.WaitingAudit)
                {
                    IsWaitingAudit = true;
                }
                else
                {
                    IsDeactive = true;
                }
            }
        }

        private ComboStatus? targetStatus;
        public ComboStatus? TargetStatus
        {
            get
            {
                return targetStatus;
            }
            set
            {
                SetValue("TargetStatus", ref targetStatus, value);
            }
        }

        private ComboType? saleRuleType;
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

        public List<KeyValuePair<ComboType?, string>> ComboTypeList { get; set; }
        public ObservableCollection<OptionalAccessoriesItemVM> Items { get; set; }

        /// <summary>
        /// 快速选择时间
        /// </summary>
        //private List<CodeNamePair> quickTimeList;
        //public List<CodeNamePair> QuickTimeList
        //{
        //    get { return quickTimeList; }
        //    set { base.SetValue("QuickTimeList", ref quickTimeList, value); }
        //}

        /// <summary>
        /// 待审核规则的商品毛利Check信息，供页面显示
        /// </summary>
        public ObservableCollection<string> DisplayApproveMsg { get; set; }

        private bool isDeactive;
        public bool IsDeactive
        {
            get
            {
                return isDeactive;
            }
            set
            {
                SetValue("IsDeactive", ref isDeactive, value);
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                SetValue("IsActive", ref isActive, value);
            }
        }

        private bool isWaitingAudit;
        public bool IsWaitingAudit
        {
            get
            {
                return isWaitingAudit;
            }
            set
            {
                SetValue("IsWaitingAudit", ref isWaitingAudit, value);
            }
        }

        public bool HasOptionalAccessoriesApproveMaintain
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_OptionalAccessories_ApproveMaintain); }
        }
    }

    public class OptionalAccessoriesItemVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private int? productSysNo;
        [Validate(ValidateType.Required)]
        public int? ProductSysNo
        {
            get
            {
                return productSysNo;
            }
            set
            {
                SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                SetValue("ProductID", ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                SetValue("ProductName", ref productName, value);
            }
        }

        public int? MerchantSysNo { get; set; }

        private string merchantName;
        public string MerchantName
        {
            get
            {
                return merchantName;
            }
            set
            {
                SetValue("MerchantName", ref merchantName, value);
            }
        }


        private string quantity;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]*[1-9][0-9]*$", ErrorMessage = "数量必须是整数，且大于0")]
        public string Quantity
        {
            get
            {
                var dpdTmp = 0;
                if (!int.TryParse(quantity, out dpdTmp)) { dpdTmp = 1; }
                return dpdTmp.ToString();
            }
            set
            {
                SetValue("Quantity", ref quantity, value);
            }
        }

        private string discount;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(0|-[0-9]\d*)$", ErrorMessage = "折扣必须是整数，且小于等于0")]
        public string Discount
        {
            get
            {
                //var dpdTmp = 0m;
                //decimal.TryParse(discount, out dpdTmp);
                //return dpdTmp.ToString("0");
                return discount ?? "0";
            }
            set
            {
                SetValue("Discount", ref discount, value);
            }
        }

        private string discountPercent;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100的整数")]
        public string DiscountPercent
        {
            get
            {
                return string.IsNullOrEmpty(discountPercent) ? "100" : discountPercent;
            }
            set
            {
                SetValue("DiscountPercent", ref discountPercent, value);
            }
        }
        public string DiscountPercentDisplay
        {
            get
            {
                var dpdTmp = 0m;
                decimal.TryParse(discountPercent, out dpdTmp);
                dpdTmp = (dpdTmp > 0 ? dpdTmp / 100 : dpdTmp);
                return dpdTmp.ToString("P0");
            }
            set
            {
                SetValue("DiscountPercentDisplay", ref discountPercent, string.IsNullOrEmpty(value) ? "0" : value.Replace("%", ""));
            }
        }
        public decimal DiscountPercentVal
        {
            get
            {
                var dpdTmp = 0m;
                decimal.TryParse(DiscountPercent, out dpdTmp);
                dpdTmp = (dpdTmp > 1 ? dpdTmp / 100 : dpdTmp);
                return dpdTmp;
            }
        }

        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.MaxLength, 5)]
        public string Priority
        {
            get { return string.IsNullOrEmpty(priority) ? "9999" : priority; }
            set { SetValue("Priority", ref priority, value); }
        }

        private decimal? productUnitCost;
        public decimal? ProductUnitCost
        {
            get
            {
                return productUnitCost;
            }
            set
            {
                SetValue("ProductUnitCost", ref productUnitCost, value);
            }
        }

        private decimal prodcuctCurrentPrice;
        public decimal ProductCurrentPrice
        {
            get
            {
                return prodcuctCurrentPrice;
            }
            set
            {
                SetValue("ProductCurrentPrice", ref prodcuctCurrentPrice, value);
            }
        }

        private bool isMasterItemB;
        public bool IsMasterItemB
        {
            get
            {
                return isMasterItemB;
            }
            set
            {
                SetValue("IsMasterItemB", ref isMasterItemB, value);
            }
        }


    }
}
