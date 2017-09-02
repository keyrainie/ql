using System;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class VisitDetail : UserControl
    {
        public int VisitSysNo
        {
            private get;
            set;
        }
        CustomerVisitFacade facade = null;

        private CustomerVisitDetailView PageView;
        public VisitDetail()
        {
            InitializeComponent();
            facade = new CustomerVisitFacade();
            PageView = new CustomerVisitDetailView()
            {
                Customer = new CustomerMaster()
            };
            Loaded += new RoutedEventHandler(VisitDetail_Loaded);
        }
        void VisitDetail_Loaded(object sender, RoutedEventArgs e)
        {
            ucVisitLog.IsOrderVisit = false;
            ucVisitLogForMaintain.IsOrderVisit = true;
            PageView.Customer.SysNo = VisitSysNo;
            facade.GetVisitDetailByVisitSysNo(VisitSysNo, (view) =>
            {
                PageView.VisitInfo = view.VisitInfo;
                PageView.VisitLogs = view.VisitLogs;
                PageView.MaintenanceLogs = view.MaintenanceLogs;
                if (view.VisitInfo.CustomerSysNo.HasValue)
                {
                    facade.GetCustomerInfo(view.VisitInfo.CustomerSysNo.Value, (customer) =>
                    {
                        PageView.Customer = customer;
                        ucCustomerBaseInfo.CustomerInfo = customer;
                    });
                }
                ucVisitLog.Logs = PageView.VisitLogs;
                ucVisitLogForMaintain.Logs = PageView.MaintenanceLogs;
                gridCustomerVisit.DataContext = PageView.VisitInfo;
                tbkHasMaintain.Text = PageView.MaintenanceLogs != null && PageView.MaintenanceLogs.Count > 0 ? YNStatus.Y.ToDescription() : YNStatus.N.ToDescription();
                tbkActivated.Text = PageView.VisitInfo.IsActive.HasValue && PageView.VisitInfo.IsActive.Value ? YNStatus.Y.ToDescription() : YNStatus.N.ToDescription();
            });

        }
        public IDialog Dialog
        {
            get;
            set;
        } 
    }
}
