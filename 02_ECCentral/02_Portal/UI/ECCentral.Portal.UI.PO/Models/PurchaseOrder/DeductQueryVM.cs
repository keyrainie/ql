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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models.PurchaseOrder
{
    public class DeductQueryVM : ModelBase
    {
        public DeductQueryVM()
        {
        }

        private int sysNo;

        public int SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set { base.SetValue("Name", ref name, value); }
        }


        /// <summary>
        /// 扣款类型
        /// </summary>
        private DeductType? deductType;

        public DeductType? DeductType
        {
            get { return deductType; }
            set { base.SetValue("DeductType", ref deductType, value); }
        }


        /// <summary>
        /// 记账类型（记成本/费用）
        /// </summary>
        private AccountType? accountType;

        public AccountType? AccountType
        {
            get { return accountType; }
            set { base.SetValue("AccountType", ref accountType, value); }
        }


        /// <summary>
        /// 扣款方式
        /// </summary>
        private DeductMethod? deductMethod;

        public DeductMethod? DeductMethod
        {
            get { return deductMethod; }
            set { base.SetValue("DeductMethod", ref deductMethod, value); }
        }


        /// <summary>
        /// 状态
        /// </summary>
        private Status? status;

        public Status? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime editDate;

        public DateTime EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private int editUser;

        public int EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
        }
    }
}
