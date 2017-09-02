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
using System.Collections.ObjectModel;

namespace ECCentral.Portal.Basic.Components.UserControls.CustomerPicker
{
    public class CustomerSimpleQueryVM : ModelBase
    {
        public CustomerSimpleQueryVM()
        {
            SelectedCustomers = new ObservableCollection<CustomerVM>();
        }
        private int? _customerSysNo;
        public int? CustomerSysNo
        {
            get { return _customerSysNo; }
            set
            {
                base.SetValue("CustomerSysNo", ref _customerSysNo, value);
            }
        }

        private string _customerID;
        public string CustomerID
        {
            get { return _customerID; }
            set
            {
                base.SetValue("CustomerID", ref _customerID, value);
            }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                base.SetValue("CustomerName", ref _customerName, value);
            }
        }

        private string _customerEmail;
        public string CustomerEmail
        {
            get { return _customerEmail; }
            set
            {
                base.SetValue("CustomerEmail", ref _customerEmail, value);
            }
        }

        private string _customerPhone;
        public string CustomerPhone
        {
            get { return _customerPhone; }
            set
            {
                base.SetValue("CustomerPhone", ref _customerPhone, value);
            }
        }

        private string _companyCode;
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private ObservableCollection<CustomerVM> _SelectedCustomers;

        public ObservableCollection<CustomerVM> SelectedCustomers
        {
            get { return _SelectedCustomers; }
            set { base.SetValue("SelectedCustomers", ref _SelectedCustomers, value); }
        }

    }
}
