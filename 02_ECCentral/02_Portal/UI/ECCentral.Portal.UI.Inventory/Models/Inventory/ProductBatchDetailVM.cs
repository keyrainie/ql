using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
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

namespace ECCentral.Portal.UI.Inventory.Models.Inventory
{
    public class ProductBatchDetailVM : ModelBase
    {
        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                this.SetValue("BatchNumber", ref batchNumber, value);
            }
        }

        /// <summary>
        /// 生产日期
        /// </summary>
        private DateTime? mfgDate;
        public DateTime? MfgDate
        {
            get { return mfgDate; }
            set { this.SetValue("MfgDate", ref mfgDate, value); }
        }

        /// <summary>
        /// 入库时间
        /// </summary>
        private DateTime inDate;
        public DateTime InDate
        {
            get { return inDate; }
            set { this.SetValue("InDate", ref inDate, value); }
        }

        /// <summary>
        /// 过期日期
        /// </summary>
        private DateTime expDate;
        public DateTime ExpDate
        {
            get { return expDate; }
            set { this.SetValue("ExpDate", ref expDate, value); }
        }

        private int _MaxDeliveryDays;
        public int MaxDeliveryDays
        {
            get { return _MaxDeliveryDays; }
            set { this.SetValue("MaxDeliveryDays", ref _MaxDeliveryDays, value); }
        }

        private string lotNo;
        public string LotNo
        {
            get { return lotNo; }
            set
            {
                this.SetValue("LotNo", ref lotNo, value);
            }
        }

        private int _ActualQty;
        public int ActualQty
        {
            get { return _ActualQty; }
            set { this.SetValue("ActualQty", ref _ActualQty, value); }
        }

        private int _AllocatedQty;
        public int AllocatedQty
        {
            get { return _AllocatedQty; }
            set { this.SetValue("AllocatedQty", ref _AllocatedQty, value); }
        }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private string _StatusText;
        public string StatusText
        {
            get
            {
                if ("A".Equals(Status))
                {
                    _StatusText = "正常";
                }
                else if ("R".Equals(Status))
                {
                    _StatusText = "临期";
                }
                else if ("I".Equals(Status))
                {
                    _StatusText = "过期";
                }
                return _StatusText;
            }
            set
            {
                this.SetValue("StatusText", ref _StatusText, value);
            }
        }

        private int _OperateNum;
        public int Quantity
        {
            get { return _OperateNum; }
            set { this.SetValue("Quantity", ref _OperateNum, value); }
        }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public int StockSysNo { get; set; }

        /// <summary>
        /// 状态列表数据源
        /// </summary>
        private List<KeyValuePair<string, string>> batchStatusDataSource;
        public List<KeyValuePair<string, string>> BatchStatusDataSource
        {
            get { return batchStatusDataSource; }
            set { this.SetValue("BatchStatusDataSource", ref batchStatusDataSource, value); }
        }

    }

}
