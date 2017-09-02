using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainSalesAreaSelectVM : ModelBase
    {
        public ProductMaintainSalesAreaSelectVM()
        {
            Province = new ProvinceVM();
            City = new CityVM();
        }

        private ProvinceVM _province;

        public ProvinceVM Province
        {
            get { return _province; }
            set { SetValue("Province", ref _province, value); }
        }

        private CityVM _city;

        public CityVM City
        {
            get { return _city; }
            set { SetValue("City", ref _city, value); }
        }

        private int? _stockSysNo;

        public int? StockSysNo
        {
            get { return _stockSysNo; }
            set { SetValue("StockSysNo", ref _stockSysNo, value); }
        }

        private String _stockName;

        public String StockName
        {
            get { return _stockName; }
            set { SetValue("StockName", ref _stockName, value); }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ProductMaintainSalesAreaSelectVM)
            {
                var o = obj as ProductMaintainSalesAreaSelectVM;
                if(o.Province != null)
                {
                    if(o.Province.ProvinceSysNo==Province.ProvinceSysNo 
                        && o.City.CitySysNo == City.CitySysNo 
                        && o.StockSysNo == StockSysNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class ProvinceVM : ModelBase
    {
        private int _provinceSysNo;

        public int ProvinceSysNo
        {
            get { return _provinceSysNo; }
            set { SetValue("ProvinceSysNo", ref _provinceSysNo, value); }
        }

        private String _provinceName;

        public String ProvinceName
        {
            get { return _provinceName; }
            set { SetValue("ProvinceName", ref _provinceName, value); }
        }
    }

    public class CityVM : ModelBase
    {
        private int _citySysNo;

        public int CitySysNo
        {
            get { return _citySysNo; }
            set { SetValue("CitySysNo", ref _citySysNo, value); }
        }

        private String _cityName;

        public String CityName
        {
            get { return _cityName; }
            set { SetValue("CityName", ref _cityName, value); }
        }
    }
}
