using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeProductInfoVM : ModelBase
    {
        public ShipTypeProductInfoVM()
        {
            this.PagingInfo = new PagingInfo();
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel { ChannelID = null, ChannelName = ResCommonEnum.Enum_All });

            this.ListShipTypeProductType = EnumConverter.GetKeyValuePairs<ShipTypeProductType>();
            this.ListProductRange = EnumConverter.GetKeyValuePairs<ProductRange>();


        }
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 配送方式-产品ID
        /// </summary>
        /// 
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }

        }
        /// <summary>
        /// 渠道编号
        /// </summary>
        public int? _companyCode;
        public int? CompanyCode
        {
            get { return _companyCode; }
            set { SetValue("CompanyCode", ref _companyCode, value); }

        }

        /// <summary>
        /// 商户
        /// </summary>
        private CompanyCustomer? _companyCustomer;

        public CompanyCustomer? CompanyCustomer
        {
            get { return _companyCustomer; }
            set { _companyCustomer = value; }
        }

        /// <summary>
        ///类型
        /// </summary>
        public ShipTypeProductType? _shipTypeProductType;
        [Validate(ValidateType.Required)]
        public ShipTypeProductType? ShipTypeProductType
        {
            get { return _shipTypeProductType; }
            set { SetValue("ShipTypeProductType", ref _shipTypeProductType, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string _description;
        [Validate(ValidateType.Required)]
        public string Description
        {
            get { return _description; }
            set { SetValue("Description", ref _description, value); }
        }

        /// <summary>
        /// 仓库
        /// </summary>
        public int? _wareHouse;
        public int? WareHouse
        {
            get { return _wareHouse; }
            set { SetValue("WareHouse", ref _wareHouse, value); }
        }
        /// <summary>
        ///区域编号
        /// </summary>
        public int? _areaSysNo;
        public int? AreaSysNo
        {
            get { return _areaSysNo; }
            set { SetValue("AreaSysNo", ref _areaSysNo, value); }
        }
        /// <summary>
        ///省份编号
        /// </summary>
        public int? _provinceSysNo;
        public int? ProvinceSysNo
        {
            get { return _provinceSysNo; }
            set { SetValue("ProvinceSysNo", ref _provinceSysNo, value); }
        }
        /// <summary>
        ///城市编号
        /// </summary>
        public int? _citySysNo;
        public int? CitySysNo
        {
            get { return _citySysNo; }
            set { SetValue("CitySysNo", ref _citySysNo, value); }
        }
        /// <summary>
        ///地区编号
        /// </summary>
        public int? _districtSysNo;
        public int? DistrictSysNo
        {
            get { return _districtSysNo; }
            set { SetValue("DistrictSysNo", ref _districtSysNo, value); }
        }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? _shippingType;
        [Validate(ValidateType.Required)]
        public int? ShippingType
        {
            get { return _shippingType; }
            set { SetValue("ShippingType", ref _shippingType, value); }
        }

        /// <summary>
        /// 商品范围
        /// </summary>
        public ProductRange? _productRange;
        [Validate(ValidateType.Required)]
        public ProductRange? ProductRange
        {
            get { return _productRange; }
            set { SetValue("ProductRange", ref _productRange, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        //public string _ProductID;
        //public string ProductID
        //{
        //    get { return _ProductID; }
        //    set { SetValue("ProductID", ref _ProductID, value); }
        //}

        /// <summary>
        /// 商品类别
        /// </summary>
        public int? _productType;
        public int? ProductType
        {
            get { return _productType; }
            set { SetValue("ProductType", ref _productType, value); }

        }
        /// <summary>
        /// list区域
        /// </summary>
        public List<UIWebChannel> ListWebChannel { get; set; }
        public List<KeyValuePair<ShipTypeProductType?, string>> ListShipTypeProductType { get; set; }
        public List<KeyValuePair<ProductRange?, string>> ListProductRange { get; set; }
        public List<ProductVM> _listProductInfo;
        [Validate(ValidateType.Required)]
        public List<ProductVM> ListProductInfo
        {
            get { return _listProductInfo; }
            set { SetValue("ListProductInfo", ref _listProductInfo, value); }
        }
        public List<CategoryVM> _listCategoryInfo;
        public List<CategoryVM> ListCategoryInfo
        {
            get { return _listCategoryInfo; }
            set { SetValue("ListCategoryInfo", ref _listCategoryInfo, value); }
        }
    }
}
