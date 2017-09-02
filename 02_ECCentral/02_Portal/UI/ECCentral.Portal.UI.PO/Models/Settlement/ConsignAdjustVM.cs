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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignAdjustVM : ModelBase
    {
        public ConsignAdjustVM()
        {
            this.ItemList = new List<ConsignAdjustItemVM>();
        }

        private int? sysNO;
        public int? SysNo
        {
            get { return sysNO; }
            set { base.SetValue("SysNo", ref sysNO, value); }
        }

        private string settleSysNo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger,ErrorMessage="请正确填写结算单号")]
        public string SettleSysNo
        {
            get { return settleSysNo; }
            set { base.SetValue("SettleSysNo", ref settleSysNo, value); }
        }

        private ConsignAdjustStatus? status;
        public ConsignAdjustStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string settleRange;
        public string SettleRange
        {
            get { return settleRange; }
            set { base.SetValue("SettleRange", ref settleRange, value); }
        }

        private DateTime? settleRangeDate;
        [Validate(ValidateType.Required,ErrorMessage="请选择年月")]
        public DateTime? SettleRangeDate
        {
            get { return settleRangeDate; }
            set { base.SetValue("SettleRangeDate", ref settleRangeDate, value); }
        }

        private int? venderSysNo;
        [Validate(ValidateType.Required)]
        public int? VenderSysNo
        {
            get { return venderSysNo; }
            set { base.SetValue("VenderSysNo", ref venderSysNo, value); }
        }

        private string venderName;
        public string VenderName
        {
            get { return venderName; }
            set { base.SetValue("VenderName", ref venderName, value); }
        }


        private int? _PMSysNo;
        [Validate(ValidateType.Required,ErrorMessage="请选择产品经理")]
        public int? PMSysNo
        {
            get { return _PMSysNo; }
            set { base.SetValue("PMSysNo", ref _PMSysNo, value); }
        }

        private List<ConsignAdjustItemVM> itemList;
        public List<ConsignAdjustItemVM> ItemList
        {
            get { return itemList; }
            set { base.SetValue("ItemList", ref itemList, value); }
        }

        private decimal? totalAmt;
        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { base.SetValue("TotalAmt", ref totalAmt, value); }
        }      
    }
}
