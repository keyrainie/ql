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

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderETATimeInfoVM : ModelBase
    {
        private int? eTATimeSysNo;

        public int? ETATimeSysNo
        {
            get { return eTATimeSysNo; }
            set { this.SetValue("ETATimeSysNo", ref eTATimeSysNo, value); }
        }

        /// <summary>
        /// PO编号
        /// </summary>
        /// 
        private int? pOSysNo;

        public int? POSysNo
        {
            get { return pOSysNo; }
            set { this.SetValue("POSysNo", ref pOSysNo, value); }
        }

        /// <summary>
        /// 预计到货时间(ETATime)
        /// </summary>
        private DateTime? eTATime;

        public DateTime? ETATime
        {
            get { return eTATime; }
            set { this.SetValue("ETATime", ref eTATime, value); }
        }

        /// <summary>
        /// 预计到货时段 (上午下午)
        /// </summary>
        private PurchaseOrderETAHalfDayType? halfDay;

        public PurchaseOrderETAHalfDayType? HalfDay
        {
            get { return halfDay; }
            set { this.SetValue("HalfDay", ref halfDay, value); }
        }

        /// <summary>
        /// 状态 : (申请，取消，通过)
        /// </summary>
        private int? status;

        public int? Status
        {
            get { return status; }
            set { this.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 理由
        /// </summary>

        private string memo;

        public string Memo
        {
            get { return memo; }
            set { this.SetValue("Memo", ref memo, value); }
        }

        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { this.SetValue("InDate", ref inDate, value); }
        }

        private string inUser;

        public string InUser
        {
            get { return inUser; }
            set { this.SetValue("InUser", ref inUser, value); }
        }

        private DateTime? editDate;

        public DateTime? EditDate
        {
            get { return editDate; }
            set { this.SetValue("EditDate", ref editDate, value); }
        }

        private string editUser;

        public string EditUser
        {
            get { return editUser; }
            set { this.SetValue("EditUser", ref editUser, value); }
        }
    }
}
