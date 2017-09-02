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
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.IM.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RmaPolicyLogManagement : PageBase
    {
         private RmaPolicyLogQueryVM QueryVM { get; set; }
        private RmaPolicyLogFacade facade;
        public RmaPolicyLogManagement()
        {
            InitializeComponent();
            this.dgRmaPolicyLogQueryResult.LoadingDataSource += dgRmaPolicyLogQueryResult_LoadingDataSource;
        }

        void dgRmaPolicyLogQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.QueryRmaPolicyLog(QueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.dgRmaPolicyLogQueryResult.ItemsSource = arg.Result.Rows;
                this.dgRmaPolicyLogQueryResult.TotalCount = arg.Result.TotalCount;

            });
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            
           
            base.OnPageLoad(sender, e);
            facade = new RmaPolicyLogFacade(this);
            QueryVM = new RmaPolicyLogQueryVM();
            if (!string.IsNullOrEmpty(Request.Param))
            {
                QueryVM.RmaPolicySysNO = Request.Param;
                this.dgRmaPolicyLogQueryResult.Bind();
            }
            this.DataContext = QueryVM;
          
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            QueryVM.RmaPolicy = rmaPolicyComboxList.VM.RmaPolicy.SysNo.ToString();
            this.dgRmaPolicyLogQueryResult.Bind();
        }


    }
}
