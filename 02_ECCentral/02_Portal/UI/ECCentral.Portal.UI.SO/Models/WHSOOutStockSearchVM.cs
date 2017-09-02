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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
    public class WHSOOutStockSearchVM : ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        #region 查询条件

        private DateTime? deliveryDateTime = DateTime.Now.Date;
        /// <summary>
        /// 送货日期
        /// </summary>
        public DateTime? DeliveryDateTime
        {
            get { return deliveryDateTime; }
            set { SetValue<DateTime?>("DeliveryDateTime", ref deliveryDateTime, value); }
        }

        private String deliveryTimeRange;
        /// <summary>
        /// 送货日期 时段
        /// </summary>
        public String DeliveryTimeRange
        {
            get { return deliveryTimeRange; }
            set { SetValue<String>("DeliveryTimeRange", ref deliveryTimeRange, value); }
        }

        private int? stockSysNo;
        /// <summary>
        /// 仓库
        /// </summary>
        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue<int?>("StockSysNo", ref stockSysNo, value); }
        }

        private DateTime? auditDateTimeFrom;
        /// <summary>
        /// 审单时间 开始
        /// </summary>
        public DateTime? AuditDateTimeFrom
        {
            get { return auditDateTimeFrom; }
            set { SetValue<DateTime?>("AuditDateTimeFrom", ref auditDateTimeFrom, value); }
        }

        private DateTime? auditDateTimeTo;
        /// <summary>
        /// 审单时间 结束
        /// </summary>
        public DateTime? AuditDateTimeTo
        {
            get { return auditDateTimeTo; }
            set { SetValue<DateTime?>("AuditDateTimeTo", ref auditDateTimeTo, value); }
        }

        private ConditionType? shipTypeCondition;
        /// <summary>
        /// 送货方式 过滤方式
        /// </summary>
        public ConditionType? ShipTypeCondition
        {
            get { return shipTypeCondition; }
            set { SetValue<ConditionType?>("ShipTypeCondition", ref shipTypeCondition, value); }
        }

        private int? shipTypeSysNo;
        /// <summary>
        /// 送货方式
        /// </summary>
        public int? ShipTypeSysNo
        {
            get { return shipTypeSysNo; }
            set { SetValue<int?>("ShipTypeSysNo", ref shipTypeSysNo, value); }
        }

        private int? receiveAreaSysNo;
        /// <summary>
        /// 送货区域
        /// </summary>
        public int? ReceiveAreaSysNo
        {
            get { return receiveAreaSysNo; }
            set { SetValue<int?>("ReceiveAreaSysNo", ref receiveAreaSysNo, value); }
        }

        private bool? isVAT;
        /// <summary>
        /// 是否增票
        /// </summary>
        public bool? ISVAT
        {
            get { return isVAT; }
            set { SetValue<bool?>("ISVAT", ref isVAT, value); }
        }

        private SOIsSpecialOrder? specialSOType;
        /// <summary>
        /// 是否特殊订单
        /// </summary>
        public SOIsSpecialOrder? SpecialSOType
        {
            get { return specialSOType; }
            set { SetValue<SOIsSpecialOrder?>("SpecialSOType", ref specialSOType, value); }
        }

        private EnoughFlag? enoughFlag;
        /// <summary>
        /// 是否有货
        /// </summary>
        public EnoughFlag? EnoughFlag
        {
            get { return enoughFlag; }
            set { SetValue<EnoughFlag?>("EnoughFlag", ref enoughFlag, value); }
        }

        private String companyCode;
        public String CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
        }

        #endregion

        #region 查询条件绑定数据源

        private List<KeyValuePair<bool?, string>> booleanList;
        /// <summary>
        /// Boolean类型列表
        /// </summary>
        public List<KeyValuePair<bool?, string>> BooleanList
        {
            get
            {
                booleanList = booleanList ?? BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
                return booleanList;
            }
        }

        private List<KeyValuePair<ECCentral.BizEntity.Invoice.DeliveryType?, string>> deliveryTypeList;
        /// <summary>
        /// 配送类型列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.DeliveryType?, string>> DeliveryTypeList
        {
            get
            {
                deliveryTypeList = deliveryTypeList ?? EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.DeliveryType>(EnumConverter.EnumAppendItemType.All);

                return deliveryTypeList;
            }
        }

        private List<CodeNamePair> timeRangeList;
        /// <summary>
        /// 时间段列表
        /// </summary>
        public List<CodeNamePair> TimeRangeList
        {
            get { return timeRangeList; }
            set { SetValue<List<CodeNamePair>>("TimeRangeList", ref timeRangeList, value); }
        }

        private List<KeyValuePair<SOIsSpecialOrder?, string>> specialOrderTypeList;
        /// <summary>
        /// 是否特殊订单
        /// </summary>
        public List<KeyValuePair<SOIsSpecialOrder?, string>> SpecialOrderTypeList
        {
            get
            {
                specialOrderTypeList = specialOrderTypeList ?? EnumConverter.GetKeyValuePairs<SOIsSpecialOrder>(EnumConverter.EnumAppendItemType.All);

                return specialOrderTypeList;
            }
        }

        private List<KeyValuePair<EnoughFlag?, string>> enoughFlagTypeList;
        /// <summary>
        /// 是否有货
        /// </summary>
        public List<KeyValuePair<EnoughFlag?, string>> EnoughFlagTypeList
        {
            get
            {
                enoughFlagTypeList = enoughFlagTypeList ?? EnumConverter.GetKeyValuePairs<EnoughFlag>(EnumConverter.EnumAppendItemType.All);

                return enoughFlagTypeList;
            }
        }

        private List<KeyValuePair<ConditionType?, string>> conditionTypeList;
        /// <summary>
        /// 条件过滤方式
        /// </summary>
        public List<KeyValuePair<ConditionType?, string>> ConditionTypeList
        {
            get
            {
                conditionTypeList = conditionTypeList ?? EnumConverter.GetKeyValuePairs<ConditionType>(EnumConverter.EnumAppendItemType.None);

                return conditionTypeList;
            }
        }
        #endregion

    }

    public class WHSOOutStockSearchView : ModelBase
    {
        public WHSOOutStockSearchVM QueryInfo { get; set; }

        public WHSOOutStockSearchView()
        {
            QueryInfo = new WHSOOutStockSearchVM();
        }
    }
}
