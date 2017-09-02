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

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderBasketQueryVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { base.SetValue("UserName", ref userName, value); }
        }
        private string pMGroupName;

        public string PMGroupName
        {
            get { return pMGroupName; }
            set { base.SetValue("PMGroupName", ref pMGroupName, value); }
        }
        private int? status;

        public int? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
        private string pMUserSysNo;

        public string PMUserSysNo
        {
            get { return pMUserSysNo; }
            set { base.SetValue("PMUserSysNo", ref pMUserSysNo, value); }
        }
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }
        private string createUserSysNo;

        public string CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
    }

}
