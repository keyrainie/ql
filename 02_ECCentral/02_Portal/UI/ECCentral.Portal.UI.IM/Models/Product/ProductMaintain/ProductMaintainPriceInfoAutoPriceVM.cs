using System;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPriceInfoAutoPriceVM : ModelBase
    {
        private IsAutoAdjustPrice _isAutoAdjustPrice;

        public IsAutoAdjustPrice IsAutoAdjustPrice
        {
            get { return _isAutoAdjustPrice; }
            set { SetValue("IsAutoAdjustPrice", ref _isAutoAdjustPrice, value); }
        }

        private string _notAutoAdjustPriceShow;

        public string NotAutoAdjustPriceShow
        {
            get { return _notAutoAdjustPriceShow; }
            set { SetValue("NotAutoAdjustPriceShow", ref _notAutoAdjustPriceShow, value); }
        }

        private DateTime? _notAutoAdjustPriceBeginDate;

        public DateTime? NotAutoAdjustPriceBeginDate
        {
            get { return _notAutoAdjustPriceBeginDate; }
            set { SetValue("NotAutoAdjustPriceBeginDate", ref _notAutoAdjustPriceBeginDate, value); }
        }

        private DateTime? _notAutoAdjustPriceEndDate;

        public DateTime? NotAutoAdjustPriceEndDate
        {
            get { return _notAutoAdjustPriceEndDate; }
            set { SetValue("NotAutoAdjustPriceEndDate", ref _notAutoAdjustPriceEndDate, value); }
        }
    }
}
