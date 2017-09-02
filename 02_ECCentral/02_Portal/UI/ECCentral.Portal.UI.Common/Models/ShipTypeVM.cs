using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Common.Models
{
    /// <summary>
    ///查询 配送方式
    /// </summary>
    public class ShipTypeQueryVM : ModelBase
    {
        public PagingInfo PageInfo { get; set; }
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        public string _companyCode;
        public string CompanyCode
        {
            get { return _companyCode; }
            set { SetValue("CompanyCode", ref _companyCode, value); }
        }

         /// <summary>
        /// 配送方式ID
        /// </summary>
        public string  _shipTypeID;
        public string ShipTypeID 
        {
            get { return _shipTypeID; }
            set { SetValue("ShipTypeID", ref _shipTypeID, value); }
        }
        /// <summary>
        ///配送方式名称
        /// </summary>
        public string _shipTypeName;
        public string ShipTypeName
        {
            get { return _shipTypeName; }
            set { SetValue("ShipTypeName", ref _shipTypeName, value); }
        }

        /// <summary>
        /// 本地仓库发货
        /// </summary>
        public int? _stockSysNo;
        public int? StockSysNo
        {
            get { return _stockSysNo; }
            set { SetValue("StockSysNo", ref _stockSysNo, value); }
        }   

        /// <summary>
        /// 前台显示
        /// </summary>
        public HYNStatus? _isOnlineShow;
        public HYNStatus? IsOnlineShow
        {
            get { return _isOnlineShow; }
            set { SetValue("IsOnlineShow", ref _isOnlineShow, value); }
        }  

        /// <summary>
        ///收取包裹费
        /// </summary>
        public SYNStatus? _isWithPackFee;
        public SYNStatus? IsWithPackFee
        {
            get { return _isWithPackFee; }
            set { SetValue("IsWithPackFee", ref _isWithPackFee, value); }
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
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
    }


    /// <summary>
    /// 配送方式
    /// </summary>
    public class ShipTypeVM : ModelBase
    {
        public int? SysNo { get; set; }
        /// <summary>
        /// 配送方式编号
        /// </summary>
        public string ShippingTypeID { get; set; }

        /// <summary>
        /// 配送方式描述
        /// </summary>
        public string ShippingTypeName { get; set; }

        public string ShipTypeDesc { get; set; }
        public string Provider { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? AreaSysNo { get; set; }
        public string Address { get; set; }
        public string DisplayShipName { get; set; }
        /// <summary>
        /// 服务时限
        /// </summary>
        public int? DeliveryType { get; set; }
        /// <summary>
        /// 自提点联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 保价费基数
        /// </summary>
        public decimal? PremiumBase { get; set; }

        /// <summary>
        /// 保价费率
        /// </summary>
        public decimal? PremiumRate { get; set; }

        /// <summary>
        /// 免运费金额
        /// </summary>
        public decimal? FreeShipBase { get; set; }

        /// <summary>
        /// 订单金额上限
        /// </summary>
        public decimal? CompensationLimit { get; set; }

        /// <summary>
        /// 打包材料
        /// </summary>
        public int? PackStyle { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? OrderNumber { get; set; }
        public int? OnlyForStockSysNo { get; set; }

        /// <summary>
        /// 隔几天一送
        /// </summary>
        public int? Availsection { get; set; }
        /// <summary>
        /// 隔的那个几天
        /// </summary>
        public int? IntervalDays { get; set; }
        /// <summary>
        /// 会员免运费级别
        /// </summary>
        public int? CustomerRankFreeShip { get; set; }

        /// <summary>
        /// 送货周期
        /// </summary>
        public string Period { get; set; }

        public string ShortName { get; set; }
        /// <summary>
        /// 是否专用配送方式
        /// </summary>
        public bool? IsSpecified { get; set; }

        /// <summary>
        /// 是否提供24小时配送
        /// </summary>
        public DeliveryStatusFor24H? DeliveryPromise { get; set; }

        /// <summary>
        /// 前台显示
        /// </summary>
        public HYNStatus? IsOnlineShow { get; set; }

        public HYNStatus? IsWithPackFee { get; set; }
        public int? ShipTypeEnum { get; set; }

        /// <summary>
        /// 对应DS(并单)
        /// </summary>
        public int? DSSysNo { get; set; }

        public string CompanyCode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? IsGetBySelf { get; set; }
    }
}
