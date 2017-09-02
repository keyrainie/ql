using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class MaliceUserQuery : PageBase
    {
        private int customerSysNo;

        private CustomerQueryFacade facade;

        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new CustomerQueryFacade(this);
            base.OnPageLoad(sender, e); 
            try
            {
                customerSysNo = int.Parse(this.Request.Param);
                MaliceCustomerLog.Bind();
            }
            catch { }
        }

        public MaliceUserQuery()
        {
            InitializeComponent();
        }

        private void MaliceCustomerLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            e.PageSize = 1000;
            e.PageIndex = 0;
            facade.QueryMaliceCustomer(customerSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                MaliceCustomerLog.ItemsSource = args.Result.Rows;
                MaliceCustomerLog.TotalCount = args.Result.TotalCount;
            });
        }
    }

}
