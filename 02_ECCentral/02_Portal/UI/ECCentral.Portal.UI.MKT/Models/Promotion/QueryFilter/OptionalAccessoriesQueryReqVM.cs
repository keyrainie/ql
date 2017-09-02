using System.Collections.Generic;
using System.Linq;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class OptionalAccessoriesQueryReqVM : ModelBase
    {
        public OptionalAccessoriesQueryReqVM()
        {
            //this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType= UIWebChannelType.publicChennel });

            this.ComboStatusList = EnumConverter.GetKeyValuePairs<ComboStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private string sysNo;
        [Validate(ValidateType.Regex, @"^[0-9]*[1-9][0-9]*$", ErrorMessage = "规则编号必须是整数，且大于0")]
        public string SysNo
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

        public List<int> SysNoList { get; set; }

        private string productSysNo;
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { SetValue("ProductID", ref productID, value); }
        }

        private string merchantSysNo;
        public string MerchantSysNo
        {
            get
            {
                return merchantSysNo;
            }
            set
            {
                SetValue("MerchantSysNo", ref merchantSysNo, value);
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

        private int? pm;
        public int? PM
        {
            get
            {
                return pm;
            }
            set
            {
                SetValue("PM", ref pm, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private int optionalAccessoriesItemType;
        public int OptionalAccessoriesItemType
        {
            get
            {
                return optionalAccessoriesItemType;
            }
            set
            {
                SetValue("OptionalAccessoriesItemType", ref optionalAccessoriesItemType, value);
            }
        }

        private int? category1SysNo;
        public int? Category1SysNo
        {
            get
            {
                return category1SysNo;
            }
            set
            {
                SetValue("Category1SysNo", ref category1SysNo, value);
            }
        }

        private int? category2SysNo;
        public int? Category2SysNo
        {
            get
            {
                return category2SysNo;
            }
            set
            {
                SetValue("Category2SysNo", ref category2SysNo, value);
            }
        }

        private int? category3SysNo;
        public int? Category3SysNo
        {
            get
            {
                return category3SysNo;
            }
            set
            {
                SetValue("Category3SysNo", ref category3SysNo, value);
            }
        }

        //public List<UIWebChannel> WebChannelList { get; set; }
        public List<KeyValuePair<ComboStatus?, string>> ComboStatusList { get; set; }

        /// <summary>
        /// 审核权限
        /// </summary>
        public bool HasOptionalAccessoriesManagementEdit 
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_OptionalAccessories_ApproveMaintain); }
        }
    }
}