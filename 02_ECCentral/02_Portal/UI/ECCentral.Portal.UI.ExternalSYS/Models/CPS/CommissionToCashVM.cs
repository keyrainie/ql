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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class CommissionToCashVM : ModelBase
    {
        public int SysNo { get; set; }
        /// <summary>
        /// 应支付金额
        /// </summary>
        private string oldPayAmt;
        public string OldPayAmt 
        {
            get { return oldPayAmt; }
            set { SetValue("OldPayAmt", ref oldPayAmt, value); }
        }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        private string newPayAmt;
        public string NewPayAmt
        {
            get { return newPayAmt; }
            set { SetValue("NewPayAmt", ref newPayAmt, value); }
        }

        /// <summary>
        /// 奖金
        /// </summary>
        private string bonus;
        public string Bonus
        {
            get { return bonus; }
            set { SetValue("Bonus", ref bonus, value); }
        }

        private string confirmToCashAmt;
        public string ConfirmToCashAmt
        {
            get { return confirmToCashAmt; }
            set { SetValue("ConfirmToCashAmt", ref confirmToCashAmt, value); }
        }

        private string afterTaxAmt;
        public string AfterTaxAmt
        {
            get { return afterTaxAmt; }
            set { SetValue("AfterTaxAmt", ref afterTaxAmt, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string memo;
        public string Memo
        {
            get { return memo; }
            set { SetValue("Memo", ref memo, value); }
        }
    }
}
