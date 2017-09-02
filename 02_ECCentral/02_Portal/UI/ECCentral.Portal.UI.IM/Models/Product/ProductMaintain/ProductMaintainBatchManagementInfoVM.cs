using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBatchManagementInfoVM : ModelBase
    {
        public ProductMaintainBatchManagementInfoVM()
        {
            this.CollectDateTypeList = EnumConverter.GetKeyValuePairs<CollectDateType>(EnumConverter.EnumAppendItemType.None);
        }

        public int ProductSysNo { get; set; }

        private bool isBatch;
        public bool IsBatch
        {
            get
            {
                return isBatch;
            }
            set
            {
                SetValue("IsBatch", ref isBatch, value);
            }
        }

        private bool isCollectBatchNo;
        public bool IsCollectBatchNo
        {
            get
            {
                return isCollectBatchNo;
            }
            set
            {
                SetValue("IsCollectBatchNo", ref isCollectBatchNo, value);
            }
        }

        private CollectDateType collectType;
        public CollectDateType CollectType
        {
            get
            {
                return collectType;
            }
            set
            {
                SetValue("CollectType", ref collectType, value);
            }
        }

        private string minReceiptDays;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string MinReceiptDays
        {
            get
            {
                return minReceiptDays;
            }
            set
            {
                SetValue("MinReceiptDays", ref minReceiptDays, value);
            }
        }

        private string maxDeliveryDays;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string MaxDeliveryDays
        {
            get
            {
                return maxDeliveryDays;
            }
            set
            {
                SetValue("MaxDeliveryDays", ref maxDeliveryDays, value);
            }
        }

        private string guaranteePeriodYear;
        [Validate(ValidateType.Interger)]
        public string GuaranteePeriodYear
        {
            get
            {
                return guaranteePeriodYear;
            }
            set
            {
                SetValue("GuaranteePeriodYear", ref guaranteePeriodYear, value);
            }
        }

        private string guaranteePeriodMonth;
        [Validate(ValidateType.Interger)]
        public string GuaranteePeriodMonth
        {
            get
            {
                return guaranteePeriodMonth;
            }
            set
            {
                SetValue("GuaranteePeriodMonth", ref guaranteePeriodMonth, value);
            }
        }

        private string guaranteePeriodDay;
        [Validate(ValidateType.Interger)]
        public string GuaranteePeriodDay
        {
            get
            {
                return guaranteePeriodDay;
            }
            set
            {
                SetValue("GuaranteePeriodDay", ref guaranteePeriodDay, value);
            }
        }

        private string note;
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                SetValue("Note", ref note, value);                
            }
        }

        private string historyNote;
        public string HistoryNote
        {
            get
            {
                if (this.Logs != null)
                {
                    StringBuilder sb = new StringBuilder();
                    Logs.ForEach(p =>
                    {
                        sb.AppendFormat("{0},{1},{2}\r\n", p.InDate, p.InUser, p.Note);
                    });
                    return sb.ToString();
                }
                return string.Empty;
            }
            set
            {
                SetValue("HistoryNote", ref historyNote, value);
            }
        }

        public List<ProductBatchManagementInfoLog> Logs { get; set; }

        public List<KeyValuePair<CollectDateType?, string>> CollectDateTypeList { get; set; }

        public void FirePropertyChanged(string proertyName)
        {
            base.RaisePropertyChanged(proertyName);
        }
    }
}
