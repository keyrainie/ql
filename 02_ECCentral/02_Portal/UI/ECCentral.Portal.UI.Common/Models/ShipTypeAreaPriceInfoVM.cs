using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Common.Models
{
    public class ShipTypeAreaPriceInfoVM:ModelBase
    {
        public ShipTypeAreaPriceInfoVM()
        {
            this.ListWebChannel = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.ListWebChannel.Insert(0, new UIWebChannel { ChannelID = null, ChannelName = ResCommonEnum.Enum_All });
        }
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
        /// 商户
        /// </summary>
        private CompanyCustomer? _companyCustomer;

        public CompanyCustomer? CompanyCustomer
        {
            get { return _companyCustomer; }
            set { _companyCustomer = value; }
        }
        /// <summary>
        /// 配送区域
        /// </summary>
        public int? _areaSysNo;
        [Validate(ValidateType.Required)]
        public int? AreaSysNo
        {
            get { return _areaSysNo; }
            set { SetValue("AreaSysNo", ref _areaSysNo, value); }
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        public int? _shipTypeSysNo;
        [Validate(ValidateType.Required)]
        public int? ShipTypeSysNo
        {
            get { return _shipTypeSysNo; }
            set { SetValue("ShipTypeSysNo", ref _shipTypeSysNo, value); }
        }
        /// <summary>
        /// 本段起始重量
        /// </summary>
        public string _baseWeight;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string BaseWeight
        {
            get { return _baseWeight; }
            set { SetValue("BaseWeight", ref _baseWeight, value); }
        }
        /// <summary>
        /// 本段截至重量
        /// </summary>
        public string _topWeight;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string TopWeight
        {
            get { return _topWeight; }
            set { SetValue("TopWeight", ref _topWeight, value); }
        }
        /// <summary>
        /// 重量基本单位
        /// </summary>
        public string _unitWeight;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string UnitWeight
        {
            get { return _unitWeight; }
            set { SetValue("UnitWeight", ref _unitWeight, value); }
        }
        /// <summary>
        /// 价格基本单位
        /// </summary>
        public string _unitPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string UnitPrice
        {
            get { return _unitPrice; }
            set { SetValue("UnitPrice", ref _unitPrice, value); }
        }
        /// <summary>
        /// 本段最高价格
        /// </summary>
        public string _maxPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string MaxPrice
        {
            get { return _maxPrice; }
            set { SetValue("MaxPrice", ref _maxPrice, value); }
        }
        /// <summary>
        /// 渠道编号
        /// </summary>
        private string _channelID;
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
        /// <summary>
        /// list区域
        /// </summary>
        public List<UIWebChannel> ListWebChannel { get; set; }


        public List<int?> _areaSysNoList;
        public List<int?> AreaSysNoList
        {
            get { return _areaSysNoList; }
            set { SetValue("AreaSysNoList", ref _areaSysNoList, value); }
        }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? _vendorSysNo;
        [Validate(ValidateType.Required)]
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set { SetValue("VendorSysNo", ref _vendorSysNo, value); }
        }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string _vendorName;
        public string VendorName
        {
            get { return _vendorName; }
            set { SetValue("VendorName", ref _vendorName, value); }
        }
    }
}
