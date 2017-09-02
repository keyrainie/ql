using System.Collections.Generic;
using System.Linq;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.Models;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComboQueryReqVM : ModelBase
    {
        public ComboQueryReqVM()
        {
            //修改UIWebChannelType.publicChennel 后放开

            //this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType. });

            this.ComboStatusList = EnumConverter.GetKeyValuePairs<ComboStatus>(EnumConverter.EnumAppendItemType.All);

            List<WebChannelVM> webChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                webChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            webChennelList.Insert(0, new WebChannelVM() { ChannelName = ResCommonEnum.Enum_All });
            this.WebChannelList = webChennelList;
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

        public List<int> SysNoList { get; set; }

        private string productSysNo;
        public string ProductSysNo
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

        /// <summary>
        /// 商品编号
        /// </summary>
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
        /// <summary>
        /// 规则编号
        /// </summary>
        private string rulesSysNo;
        [Validate(ValidateType.Interger)]
        public string RulesSysNo
        {
            get { return rulesSysNo; }
            set { SetValue("RulesSysNo", ref rulesSysNo, value); }
        }
        public List<WebChannelVM> WebChannelList { get; set; }
        public List<KeyValuePair<ComboStatus?, string>> ComboStatusList { get; set; }
    }
}