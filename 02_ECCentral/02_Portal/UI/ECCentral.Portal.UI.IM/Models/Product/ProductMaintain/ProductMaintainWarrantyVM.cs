using System;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainWarrantyVM : ModelBase
    {
        public int HostWarrantyDay
        {
            get { return Convert.ToInt32(HostWarrantyDayMonth) * 30 + Convert.ToInt32(HostWarrantyDayDay); }
            set
            {
                if (value > 45)
                {
                    HostWarrantyDayMonth = (value / 30).ToString();
                    HostWarrantyDayDay = (value % 30).ToString();
                }
                else
                {
                    HostWarrantyDayMonth = 0.ToString();
                    HostWarrantyDayDay = value.ToString();
                }
            }
        }

        public int PartWarrantyDay
        {
            get { return Convert.ToInt32(PartWarrantyDayMonth) * 30 + Convert.ToInt32(PartWarrantyDayDay); }
            set
            {
                if (value > 45)
                {
                    PartWarrantyDayMonth = (value / 30).ToString();
                    PartWarrantyDayDay = (value % 30).ToString();
                }
                else
                {
                    PartWarrantyDayMonth = 0.ToString();
                    PartWarrantyDayDay = value.ToString();
                }
            }
        }

        private String _hostWarrantyDayMonth;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductWarrantyHostWarrantyDayMonthEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductWarrantyHostWarrantyDayMonthInValid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String HostWarrantyDayMonth
        {
            get { return _hostWarrantyDayMonth; }
            set { SetValue("HostWarrantyDayMonth", ref _hostWarrantyDayMonth, value); }
        }

        private String _hostWarrantyDayDay;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductWarrantyHostWarrantyDayDayEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductWarrantyHostWarrantyDayDayInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String HostWarrantyDayDay
        {
            get { return _hostWarrantyDayDay; }
            set { SetValue("HostWarrantyDayDay", ref _hostWarrantyDayDay, value); }
        }

        private String _partWarrantyDayMonth;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductWarrantyPartWarrantyDayMonthEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductWarrantyPartWarrantyDayMonthInValid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String PartWarrantyDayMonth
        {
            get { return _partWarrantyDayMonth; }
            set { SetValue("PartWarrantyDayMonth", ref _partWarrantyDayMonth, value); }
        }

        private String _partWarrantyDayDay;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductWarrantyPartWarrantyDayDayEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.Regex, "^[1-9]*[1-9][0-9]*$|^0$", ErrorMessageResourceName = "ProductMaintain_ProductWarrantyPartWarrantyDayDayInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String PartWarrantyDayDay
        {
            get { return _partWarrantyDayDay; }
            set { SetValue("PartWarrantyDayDay", ref _partWarrantyDayDay, value); }
        }

        private String _warranty;

        public String Warranty
        {
            get { return _warranty; }
            set { SetValue("Warranty", ref _warranty, value); }
        }

        private String _servicePhone;

        public String ServicePhone
        {
            get { return _servicePhone; }
            set { SetValue("ServicePhone", ref _servicePhone, value); }
        }

        private String _serviceInfo;

        public String ServiceInfo
        {
            get { return _serviceInfo; }
            set { SetValue("ServiceInfo", ref _serviceInfo, value); }
        }

        private OfferVATInvoice _offerVATInvoice;

        public OfferVATInvoice OfferVATInvoice
        {
            get { return _offerVATInvoice; }
            set { SetValue("OfferVATInvoice", ref _offerVATInvoice, value); }
        }

        private WarrantyShow _warrantyShow;

        public WarrantyShow WarrantyShow
        {
            get { return _warrantyShow; }
            set { SetValue("WarrantyShow", ref _warrantyShow, value); }
        }

        public ProductVM MainPageVM
        {
            get { return ((Views.ProductMaintain)CPApplication.Current.CurrentPage).VM; }
        }

        public bool HasItemWarrantyMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemWarrantyMaintain); }
        }
    }
}
