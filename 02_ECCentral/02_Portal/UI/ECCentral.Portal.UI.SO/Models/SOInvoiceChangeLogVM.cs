using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOInvoiceChangeLogQueryVM : ModelBase
    {

        public PagingInfo PagingInfo { get; set; }

        private int? soSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get { return soSysNo; }
            set { SetValue<int?>("SOSysNo", ref soSysNo, value); }
        }

        private string changeType;
        public string ChangeType
        {
            get { return changeType; }
            set { SetValue<string>("ChangeType", ref changeType, value); }
        }
        private int? stockSysNo;
        /// <summary>
        /// 分仓编号
        /// </summary>
        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue<int?>("StockSysNo", ref stockSysNo, value); }
        }
        private DateTime? fromLogTime;
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime? FromLogTime
        {
            get { return fromLogTime; }
            set { SetValue<DateTime?>("FromLogTime", ref fromLogTime, value); }
        }
        private DateTime? toLogTime;
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime? ToLogTime
        {
            get { return toLogTime; }
            set { SetValue<DateTime?>("ToLogTime", ref toLogTime, value); }
        }

        #region 数据源属性


        private List<KeyValuePair<string, string>> changeTypeList;
        /// <summary>
        /// 发票修改日志列表
        /// </summary>
        public List<KeyValuePair<string, string>> ChangeTypeList
        {
            get
            {
                //changeTypeList = changeTypeList ?? EnumConverter.GetKeyValuePairs<string>(EnumConverter.EnumAppendItemType.All);
                return changeTypeList;
            }
        }
        private List<StockInfo> stockList;
        /// <summary>
        /// 仓库列表
        /// </summary>
        public List<StockInfo> StockList
        {
            get { return stockList; }
            set { SetValue<List<StockInfo>>("StockList", ref stockList, value); }
        }
        #endregion
    }

    public class SOInvoiceChangeLogVM : ModelBase
    {

    }

    public class SOInvoiceChangeLogQueryView
    {
        public SOInvoiceChangeLogQueryVM QueryInfo { get; set; }
        public List<SOInvoiceChangeLogVM> Result { get; set; }
        public SOInvoiceChangeLogQueryView()
        {
            QueryInfo = new SOInvoiceChangeLogQueryVM();
        }
    }
}
