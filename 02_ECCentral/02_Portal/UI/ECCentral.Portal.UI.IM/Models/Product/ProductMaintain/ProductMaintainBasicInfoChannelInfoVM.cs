using System;
using System.Linq;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoChannelInfoVM : ModelBase
    {
        public ProductMaintainBasicInfoChannelInfoVM()
        {
            WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList();
        }

        public List<UIWebChannel> WebChannelList { get; set; }

        private int _sellerSysNo;

        public int SellerSysNo
        {
            get { return _sellerSysNo; }
            set { SetValue("SellerSysNo", ref _sellerSysNo, value); }
        }

        private String _sellerName;

        public String SellerName
        {
            get { return _sellerName; }
            set { SetValue("SellerName", ref _sellerName, value); }
        }

        private UIWebChannel _webChannel;

        public UIWebChannel WebChannel
        {
            get { return _webChannel; }
            set { SetValue("WebChannel", ref _webChannel, value); }
        }
    }
}
