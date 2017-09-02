using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeAreaPriceQueryFilterVM:ModelBase
    {
        public ShipTypeAreaPriceQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel { ChannelID = null, ChannelName = ResCommonEnum.Enum_All });
            this.VendorSysNo = 1;
            this.VendorName = "泰隆优选";
        }
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 配送方式-地区-价格ID
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
        /// 配送区域
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
        public int? _shipTypeSysNo;
        public int? ShipTypeSysNo
        {
            get { return _shipTypeSysNo; }
            set { SetValue("ShipTypeSysNo", ref _shipTypeSysNo, value); }
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? _vendorSysNo;
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set { SetValue("VendorSysNo", ref _vendorSysNo, value); }
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string _vendorName;
        public string VendorName
        {
            get { return _vendorName; }
            set { SetValue("VendorName", ref _vendorName, value); }
        }

        /// <summary>
        /// 商户
        /// </summary>
        public CompanyCustomer? _companyCustomer;
        public CompanyCustomer? CompanyCustomer
        {
            get { return _companyCustomer; }
            set { SetValue("CompanyCustomer", ref _companyCustomer, value); }
        }

        /// <summary>
        /// list区域
        /// </summary>
        public List<UIWebChannel> ListWebChannel { get; set; }
    }
}
