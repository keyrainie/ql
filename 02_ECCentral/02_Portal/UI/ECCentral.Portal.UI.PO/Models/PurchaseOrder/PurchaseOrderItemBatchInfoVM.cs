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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models.PurchaseOrder
{
    public class PurchaseOrderItemBatchInfoVM : ModelBase
    {
        private string m_BatchInfoNumber;

        public string BatchInfoNumber
        {
            get { return m_BatchInfoNumber; }
            set { this.SetValue("BatchInfoNumber", ref m_BatchInfoNumber, value); }
        }

        private int? m_StockSysNo;

        public int? StockSysNo
        {
            get { return m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private string m_StockSysNoString;

        public string StockSysNoString
        {
            get { return m_StockSysNoString; }
            set { this.SetValue("StockSysNoString", ref m_StockSysNoString, value); }
        }
        private DateTime? m_DtBatchInfoInTimer;

        public DateTime? DtBatchInfoInTimer
        {
            get { return m_DtBatchInfoInTimer; }
            set { this.SetValue("DtBatchInfoInTimer", ref m_DtBatchInfoInTimer, value); }
        }
        private DateTime m_DtBatchInfoCreateTime;

        public DateTime DtBatchInfoCreateTime
        {
            get { return m_DtBatchInfoCreateTime; }
            set { this.SetValue("DtBatchInfoCreateTime", ref m_DtBatchInfoCreateTime, value); }
        }
        private DateTime m_DtBatchInfoPassTime;

        public DateTime DtBatchInfoPassTime
        {
            get { return m_DtBatchInfoPassTime; }
            set { this.SetValue("DtBatchInfoPassTime", ref m_DtBatchInfoPassTime, value); }
        }
        private int? m_BatchInfoMaxDay;

        public int? BatchInfoMaxDay
        {
            get { return m_BatchInfoMaxDay; }
            set { this.SetValue("BatchInfoMaxDay", ref m_BatchInfoMaxDay, value); }
        }
        private string m_BatchInfoFactory;

        public string BatchInfoFactory
        {
            get { return m_BatchInfoFactory; }
            set { this.SetValue("BatchInfoFactory", ref m_BatchInfoFactory, value); }
        }
        private int? m_BatchInfoStockNumber;

        public int? BatchInfoStockNumber
        {
            get { return m_BatchInfoStockNumber; }
            set { this.SetValue("BatchInfoStockNumber", ref m_BatchInfoStockNumber, value); }
        }
        private PurchaseOrderBatchInfoStatus? m_Status;

        public PurchaseOrderBatchInfoStatus? Status
        {
            get { return m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }
        private int? m_ReturnQty;

        public int? ReturnQty
        {
            get { return m_ReturnQty; }
            set { this.SetValue("ReturnQty", ref m_ReturnQty, value); }
        }
    }
}
