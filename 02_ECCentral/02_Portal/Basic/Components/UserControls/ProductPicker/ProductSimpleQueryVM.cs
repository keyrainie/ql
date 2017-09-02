using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public class ProductSimpleQueryVM : ModelBase
    {
        public ProductSimpleQueryVM()
        {
            this.ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            this.ProductTypeList = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
            this.SelectedProducts = new ObservableCollection<ProductVM>();
        }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private string _productSysNo;
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }

        private string _productID;
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                base.SetValue("ProductName", ref _productName, value);
            }
        }
        List<KeyValuePair<ProductStatus?, string>> _ProductStatusList;

        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList
        {
            get { return _ProductStatusList; }
            set
            {
                base.SetValue("ProductStatusList", ref _ProductStatusList, value);
            }
        }

        private ProductStatus? _Status;

        public ProductStatus? ProductStatus
        {
            get { return _Status; }
            set { base.SetValue("ProductStatus", ref _Status, value); }
        }
        List<KeyValuePair<ProductType?, string>> _ProductTypeList;

        public List<KeyValuePair<ProductType?, string>> ProductTypeList
        {
            get { return _ProductTypeList; }
            set
            {
                base.SetValue("ProductTypeList", ref _ProductTypeList, value);
            }
        }

        private ProductType? _ProductType;

        public ProductType? ProductType
        {
            get { return _ProductType; }
            set { base.SetValue("ProductType", ref _ProductType, value); }
        }



        private int? _IsConsign;

        public int? IsConsign
        {
            get { return _IsConsign; }
            set
            {
                base.SetValue("IsConsign", ref _IsConsign, value);
            }
        }

        private int? _C1SysNo;

        public int? C1SysNo
        {
            get { return _C1SysNo; }
            set { base.SetValue("C1SysNo", ref _C1SysNo, value); }
        }

        private int? _C2SysNo;

        public int? C2SysNo
        {
            get { return _C2SysNo; }
            set { base.SetValue("C2SysNo", ref _C2SysNo, value); }
        }

        private int? _C3SysNo;

        public int? C3SysNo
        {
            get { return _C3SysNo; }
            set { base.SetValue("C3SysNo", ref _C3SysNo, value); }
        }

        private int? _StockSysNo;

        public int? StockSysNo
        {
            get { return _StockSysNo; }
            set
            {
                base.SetValue("StockSysNo", ref _StockSysNo, value);
            }
        }

        private int? _VendorSysNo;
        public int? VendorSysNo
        {
            get { return _VendorSysNo; }
            set { SetValue("VendorSysNo", ref _VendorSysNo, value); }
        }
        private string _VendorName;
        public string VendorName
        {
            get { return _VendorName; }
            set
            {
                base.SetValue("VendorName", ref _VendorName, value);
            }
        }

        private bool _IsMoreConditionChecked;

        public bool IsMoreConditionChecked
        {
            get { return _IsMoreConditionChecked; }
            set
            {
                base.SetValue("IsMoreConditionChecked", ref _IsMoreConditionChecked, value);
                if (value)
                    MoreConditionPannelVisibility = Visibility.Visible;
                else
                    MoreConditionPannelVisibility = Visibility.Collapsed;
            }
        }
        private Visibility _MoreConditionPannelVisibility = Visibility.Collapsed;

        public Visibility MoreConditionPannelVisibility
        {
            get { return _MoreConditionPannelVisibility; }
            set { base.SetValue("MoreConditionPannelVisibility", ref _MoreConditionPannelVisibility, value); }
        }

        private ObservableCollection<ProductVM> _SelectedProducts;

        public ObservableCollection<ProductVM> SelectedProducts
        {
            get { return _SelectedProducts; }
            set { base.SetValue("SelectedProducts", ref _SelectedProducts, value); }
        }


        private int? brandSysNo;

        public int? BrandSysNo
        {
            get
            {
                return brandSysNo;
            }
            set
            {
                SetValue("BrandSysNo", ref brandSysNo, value);
            }
        }
    }
}
