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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class DeductVM : ModelBase
    {
        /// <summary>
        /// 扣款项编号
        /// </summary>
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
       

        private string name;
        public string Name
        {
            get { return name; }
            set { base.SetValue("Name", ref name, value); }
        }

        private AccountType accountType;
        public AccountType AccountType
        {
            get { return accountType; }
            set { base.SetValue("AccountType", ref accountType, value); }
        }
        private DeductMethod deductMethod;
        public DeductMethod DeductMethod
        {
            get { return deductMethod; }
            set { base.SetValue("DeductMethod", ref deductMethod, value); }
        }
        private bool isCheckedItem;
        public bool IsCheckedItem {

            get { return isCheckedItem; }
            set { base.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }      
    }
}
