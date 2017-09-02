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
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOPendingListQueryVM : ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        private int? soSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get { return soSysNo; }
            set { SetValue<int?>("SOSysNo", ref soSysNo, value); }
        }

        private int? status;
        public int? Status
        {
            get { return status; }
            set { SetValue<int?>("Status", ref status, value); }
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
        private DateTime? fromInTime;
        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime? FromInTime
        {
            get { return fromInTime; }
            set { SetValue<DateTime?>("FromLogTime", ref fromInTime, value); }
        }
        private DateTime? toInTime;
        /// <summary>
        /// 日志时间
        /// </summary> 
        public DateTime? ToInTime
        {
            get { return toInTime; }
            set { SetValue<DateTime?>("ToLogTime", ref toInTime, value); }
        }

        #region 数据源属性
        private List<KeyValuePair<SOPendingStatus?, string>> statusList;
        /// <summary>
        /// 发票修改日志列表
        /// </summary>
        public List<KeyValuePair<SOPendingStatus?, string>> StatusList
        {
            get
            {
                statusList = statusList ?? EnumConverter.GetKeyValuePairs<SOPendingStatus>(EnumConverter.EnumAppendItemType.All);
                return statusList;
            }
            set { statusList = value; }
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

    public class SOPendingVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_WarehouseNumber;
        public Int32? WarehouseNumber
        {
            get { return this.m_WarehouseNumber; }
            set { this.SetValue("WarehouseNumber", ref m_WarehouseNumber, value); }
        }

        //private Int32? m_IsPartialShipping;
        //public Int32? IsPartialShipping
        //{
        //    get { return this.m_IsPartialShipping; }
        //    set { this.SetValue("IsPartialShipping", ref m_IsPartialShipping, value); }
        //}

        private SOPendingStatus? m_Status;
        public SOPendingStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

    }

    public class SOPendingListView
    {
        public SOPendingListQueryVM QueryInfo { get; set; }
        public List<SOPendingVM> Result { get; set; }
        public SOPendingListView()
        {
            QueryInfo = new SOPendingListQueryVM();
        }
    }
}
