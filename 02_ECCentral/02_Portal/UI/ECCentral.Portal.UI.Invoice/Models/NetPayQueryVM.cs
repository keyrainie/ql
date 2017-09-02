using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class NetPayQueryVM : ModelBase
    {
        private string m_SysNo;
        /// <summary>
        /// 单据编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string SysNo
        {
            get
            {
                return this.m_SysNo;
            }
            set
            {
                this.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private string m_SOSysNo;
        /// <summary>
        /// 订单号,多个订单号之间用点(.)隔开
        /// </summary>
        [Validate(ValidateType.Regex, @"^([1-9][0-9]{0,9})?(\.[1-9][0-9]{0,9})*$")]
        public string SOSysNo
        {
            get
            {
                return this.m_SOSysNo;
            }
            set
            {
                this.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private SOStatus? m_SOStatus;
        public SOStatus? SOStatus
        {
            get
            {
                return this.m_SOStatus;
            }
            set
            {
                this.SetValue("SOStatus", ref m_SOStatus, value);
            }
        }

        private string m_ShipTypeCode;
        public string ShipTypeCode
        {
            get
            {
                return this.m_ShipTypeCode;
            }
            set
            {
                this.SetValue("ShipTypeCode", ref m_ShipTypeCode, value);
            }
        }

        private string m_PayTypeCode;
        public string PayTypeCode
        {
            get
            {
                return this.m_PayTypeCode;
            }
            set
            {
                this.SetValue("PayTypeCode", ref m_PayTypeCode, value);
            }
        }

        private NetPayStatus? m_Status;
        public NetPayStatus? Status
        {
            get
            {
                return this.m_Status;
            }
            set
            {
                this.SetValue("Status", ref m_Status, value);
            }
        }

        private string m_CreateDateFrom;
        public string CreateDateFrom
        {
            get
            {
                return this.m_CreateDateFrom;
            }
            set
            {
                this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private string m_CreateDateTo;
        public string CreateDateTo
        {
            get
            {
                return this.m_CreateDateTo;
            }
            set
            {
                this.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private NetPaySource? m_Source;
        public NetPaySource? Source
        {
            get
            {
                return this.m_Source;
            }
            set
            {
                this.SetValue("Source", ref m_Source, value);
            }
        }

        private string m_StockID;
        public string StockID
        {
            get
            {
                return this.m_StockID;
            }
            set
            {
                this.SetValue("StockID", ref m_StockID, value);
            }
        }

        private string m_DeliveryDate;
        public string DeliveryDate
        {
            get
            {
                return this.m_DeliveryDate;
            }
            set
            {
                this.SetValue("DeliveryDate", ref m_DeliveryDate, value);
            }
        }

        private string m_DeliveryTimeRange;
        public string DeliveryTimeRange
        {
            get
            {
                return this.m_DeliveryTimeRange;
            }
            set
            {
                this.SetValue("DeliveryTimeRange", ref m_DeliveryTimeRange, value);
            }
        }

        private string m_AmtFrom;
        /// <summary>
        /// 金额范围（起）
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string AmtFrom
        {
            get
            {
                return this.m_AmtFrom;
            }
            set
            {
                this.SetValue("AmtFrom", ref m_AmtFrom, value);
            }
        }

        private string m_AmtTo;
        /// <summary>
        /// 金额范围（止）
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string AmtTo
        {
            get
            {
                return this.m_AmtTo;
            }
            set
            {
                this.SetValue("AmtTo", ref m_AmtTo, value);
            }
        }

        private SOType? m_SOType;
        public SOType? SOType
        {
            get
            {
                return this.m_SOType;
            }
            set
            {
                this.SetValue("SOType", ref m_SOType, value);
            }
        }

        private string m_ChannelID;
        public string ChannelID
        {
            get
            {
                return m_ChannelID;
            }
            set
            {
                base.SetValue("ChannelID", ref m_ChannelID, value);
            }
        }

        /// <summary>
        /// 团购状态
        /// </summary>
        private GroupBuyingSettlementStatus? m_SettlementStatus;
        public GroupBuyingSettlementStatus? SettlementStatus
        {
            get
            {
                return this.m_SettlementStatus;
            }
            set
            {
                base.SetValue("SettlementStatus", ref m_SettlementStatus, value);
            }
        }

        /// <summary>
        /// 订单状态列表
        /// </summary>
        public List<KeyValuePair<SOStatus?, string>> SOStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 订单类型列表
        /// </summary>
        public List<KeyValuePair<SOType?, string>> SOTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// NetPay状态列表
        /// </summary>
        public List<KeyValuePair<NetPayStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<NetPayStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// NetPay来源列表
        /// </summary>
        public List<KeyValuePair<NetPaySource?, string>> SourceList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<NetPaySource>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<WebChannelVM> WebChannelList
        {
            get
            {
                var webchannleList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
                webchannleList.Insert(0, new WebChannelVM()
                {
                    ChannelName = ResCommonEnum.Enum_All
                });
                return webchannleList;
            }
        }

        /// <summary>
        /// 团购状态
        /// </summary>
        public List<KeyValuePair<GroupBuyingSettlementStatus?, string>> SettlementStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<GroupBuyingSettlementStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        #region 延迟加载的属性

        private List<CodeNamePair> m_DeliveryTimeRangeList;
        /// <summary>
        /// 配送日期范围：上午；下午
        /// </summary>
        public List<CodeNamePair> DeliveryTimeRangeList
        {
            get
            {
                return m_DeliveryTimeRangeList;
            }
            set
            {
                base.SetValue("DeliveryTimeRangeList", ref m_DeliveryTimeRangeList, value);
            }
        }

        private List<StockInfo> m_StockList;
        /// <summary>
        /// 分仓列表
        /// </summary>
        public List<StockInfo> StockList
        {
            get
            {
                return m_StockList;
            }
            set
            {
                base.SetValue("StockList", ref m_StockList, value);
            }
        }

        #endregion 延迟加载的属性

        #region 附加属性

        public string CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        //用于金额范围的有效值判断
        public decimal? AmountFrom
        {
            get
            {
                decimal amtFrom;
                if (!decimal.TryParse(m_AmtFrom, out amtFrom))
                {
                    return null;
                }
                return amtFrom;
            }
        }
        public decimal? AmountTo
        {
            get
            {
                decimal amtTo;
                if (!decimal.TryParse(m_AmtTo, out amtTo))
                {
                    return null;
                }
                return amtTo;
            }
        }

        #endregion 附加属性
    }
}