using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoAuditMemoVM : ModelBase
    {
        private String _pmMemo;

        public String PMMemo
        {
            get { return _pmMemo; }
            set { SetValue("PMMemo", ref _pmMemo, value); }
        }

        public String TLMemo { get; set; }

        public String PMDMemo { get; set; }
    }
}
