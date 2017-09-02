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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using ECCentral.BizEntity.RMA;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;

using System.Linq;

namespace ECCentral.Portal.UI.RMA.Models
{
    /// <summary>
    /// 送修未返还查询条件
    /// </summary>
    public class OutBoundNotReturnQueryVM : ModelBase
    {
        public OutBoundNotReturnQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //修改UIWebChannelType.publicChennel 后放开
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.publicChennel });
            this.RefundStatusList = EnumConverter.GetKeyValuePairs<RMARefundStatus>(EnumConverter.EnumAppendItemType.All);
            this.RevertStatusList = EnumConverter.GetKeyValuePairs<RMARevertStatus>(EnumConverter.EnumAppendItemType.All);
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
        }
        /// <summary>
        /// 送修时间范围开始
        /// </summary>
        public DateTime? outTimeFrom;
        public DateTime? OutTimeFrom
        {
            get { return outTimeFrom; }
            set { base.SetValue("OutTimeFrom", ref outTimeFrom, value); }
        }

        /// <summary>
        /// 送修时间范围结束
        /// </summary>
        public DateTime? outTimeTo;
        public DateTime? OutTimeTo
        {
            get { return outTimeTo; }
            set { base.SetValue("OutTimeTo", ref outTimeTo, value); }
        }
        /// <summary>
        /// 供应商系统编号
        /// </summary>

        public string vendorSysNo;
        [Validate(ValidateType.Interger)]
        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string sOSysNo;
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return sOSysNo; }
            set { base.SetValue("SOSysNo", ref sOSysNo, value); }
        }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string productSysNo;
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        /// <summary>
        /// 产品管理员系统编号
        /// </summary>
        public int? pmUserSysNo;
        public int? PMUserSysNo
        {
            get { return pmUserSysNo; }
            set { base.SetValue("PMUserSysNo", ref pmUserSysNo, value); }
        }
        /// <summary>
        /// 产品三级类别
        /// </summary>
        public int? c3SysNo;
        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }
        /// <summary>
        /// 产品二级类别
        /// </summary>
        public int? c2SysNo;
        public int? C2SysNo
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }
        /// <summary>
        /// 产品一级类别
        /// </summary>
        public int? c1SysNo;
        public int? C1SysNo
        {
            get { return c1SysNo; }
            set { base.SetValue("C1SysNo", ref c1SysNo, value); }
        }
        /// <summary>
        /// 送修超过天数
        /// </summary>
        public string sendDays;
        [Validate(ValidateType.Interger)]
        public string SendDays
        {
            get { return sendDays; }
            set { base.SetValue("SendDays", ref sendDays, value); }
        }
        /// <summary>
        /// 催讨信息
        /// </summary>
        public bool? hasResponse;
        public bool? HasResponse
        {
            get { return hasResponse; }
            set { base.SetValue("HasResponse", ref hasResponse, value); }
        }
        /// <summary>
        /// 退款状态
        /// </summary>
        public RMARefundStatus? refundStatus;
        public RMARefundStatus? RefundStatus
        {
            get { return refundStatus; }
            set { base.SetValue("RefundStatus", ref refundStatus, value); }
        }
        /// <summary>
        /// 发还状态
        /// </summary>
        public RMARevertStatus? revertStatus;
        public RMARevertStatus? RevertStatus
        {
            get { return revertStatus; }
            set { base.SetValue("RevertStatus", ref revertStatus, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private string channelID;
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                SetValue("ChannelID", ref channelID, value);
            }
        }
        public List<UIWebChannel> WebChannelList { get; set; }
        public List<KeyValuePair<RMARefundStatus?, string>> RefundStatusList { get; set; }
        public List<KeyValuePair<RMARevertStatus?, string>> RevertStatusList { get; set; }
        public List<KeyValuePair<Boolean?, string>> YNList { get; set; }
    }

    /// <summary>
    /// RMA库存查询条件
    /// </summary>
    public class RMAInventoryQueryVM : ModelBase
    {
        public RMAInventoryQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //修改UIWebChannelType.publicChennel 后放开
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.publicChennel });
            this.SearchTypeList = EnumConverter.GetKeyValuePairs<RMAInventorySearchType>();
            this.RMAOwnByList = EnumConverter.GetKeyValuePairs<RMAOwnBy>(EnumConverter.EnumAppendItemType.All);
        }
        public string rmaSysNo;
        [Validate(ValidateType.Interger)]
        public string RMASysNo
        {
            get { return rmaSysNo; }
            set { base.SetValue("RMASysNo", ref rmaSysNo, value); }
        }

        public RMAOwnBy? rmaOwnBy;
        public RMAOwnBy? RMAOwnBy
        {
            get { return rmaOwnBy; }
            set { base.SetValue("RMAOwnBy", ref rmaOwnBy, value); }
        }

        public int? productSysNo;
        [Validate(ValidateType.Interger)]
        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        public string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

        public string locationWarehouse;
        public string LocationWarehouse
        {
            get { return locationWarehouse; }
            set { base.SetValue("LocationWarehouse", ref locationWarehouse, value); }
        }

        public string ownByWarehouse;
        public string OwnByWarehouse
        {
            get { return ownByWarehouse; }
            set { base.SetValue("OwnByWarehouse", ref ownByWarehouse, value); }
        }

        public RMAInventorySearchType searchType;
        public RMAInventorySearchType SearchType
        {
            get { return searchType; }
            set { base.SetValue("SearchType", ref searchType, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private string channelID;
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                SetValue("ChannelID", ref channelID, value);
            }
        }

        public bool IsEnabled
        {
            get { return this.SearchType == RMAInventorySearchType.RMA ? true : false; }
        }

        public List<UIWebChannel> WebChannelList
        {
            get;
            set;
        }

        public List<KeyValuePair<RMAInventorySearchType?, string>> SearchTypeList { get; set; }
        public List<KeyValuePair<RMAOwnBy?, string>> RMAOwnByList { get; set; }
    }

}
