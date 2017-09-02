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

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeProductQueryFilterVM:ModelBase
    {
        public ShipTypeProductQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel {ChannelID=null,ChannelName=ResCommonEnum.Enum_All });

            this.ListShipTypeProductType = EnumConverter.GetKeyValuePairs<ShipTypeProductType>();
            ListShipTypeProductType.Insert(0, new KeyValuePair<ShipTypeProductType?, string>(null, ResCommonEnum.Enum_All));
            this.ShipTypeProductType = this.ListShipTypeProductType[0].Key;

            this.ListProductRange = EnumConverter.GetKeyValuePairs<ProductRange>();
            ListProductRange.Insert(0, new KeyValuePair<ProductRange?, string>(null, ResCommonEnum.Enum_All));
            this.ProductRange = this.ListProductRange[0].Key;
       
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
        /// 商户
        /// </summary>
        private CompanyCustomer? _companyCustomer;

        public CompanyCustomer? CompanyCustomer
        {
            get { return _companyCustomer; }
            set { _companyCustomer = value; }
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
        ///类型
        /// </summary>
        public ShipTypeProductType? _shipTypeProductType;
        public ShipTypeProductType? ShipTypeProductType
        {
            get { return _shipTypeProductType; }
            set { SetValue("ShipTypeProductType", ref _shipTypeProductType, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string  _description;
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
        /// 收货区域
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
        public int? _districtSysNumber;
        public int? DistrictSysNumber
        {
            get { return _districtSysNumber; ; }
            set { SetValue("DistrictSysNumber", ref _districtSysNumber, value); }
        }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? _shippingType;
        public int? ShippingType
        {
            get { return _shippingType; }
            set { SetValue("ShippingType", ref _shippingType, value); }
        }

        /// <summary>
        /// 商品范围
        /// </summary>
        public ProductRange? _productRange;
        public ProductRange? ProductRange
        {
            get { return _productRange; }
            set { SetValue("ProductRange", ref _productRange, value); }
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string _productSysNo;
        public string ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string _productID;
        public string  ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }
        /// <summary>
        /// 商品类别C1
        /// </summary>
        public int? _category1;
        public int? Category1
        {
            get { return _category1; }
            set { SetValue("Category1", ref _category1, value); }

        }
        /// <summary>
        /// 商品类别C2
        /// </summary>
        public int? _category2;
        public int? Category2
        {
            get { return _category2; }
            set { SetValue("Category2", ref _category2, value); }

        }
        /// <summary>
        /// 商品类别C3
        /// </summary>
        public int? _category3;
        public int? Category3
        {
            get { return _category3; }
            set { SetValue("Category3", ref _category3, value); }

        }
        /// <summary>
        /// list区域
        /// </summary>
        public List<UIWebChannel> ListWebChannel { get; set; }
        public List<KeyValuePair<ShipTypeProductType?, string>> ListShipTypeProductType { get; set; }
        public List<KeyValuePair<ProductRange?, string>> ListProductRange { get; set; }
    }
}
