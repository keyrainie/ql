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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PMGroupQuery : PageBase
    {

        PMGroupQueryVM model;
        private List<PMGroupVM> _vmList;

        public PMGroupQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new PMGroupQueryVM();
            this.DataContext = model;
            cbPMGroupStatus.SelectedIndex = 0;
        }

        private void btnPMGroupNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_PMGroupMaintainCreateFormat, null, true);
        }

        private void btnPMGroupSearch_Click(object sender, RoutedEventArgs e)
        {
            dgPMGroupQueryResult.Bind();
        }

        private void dgPMGroupQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PMGroupQueryFacade facade = new PMGroupQueryFacade(this);

            facade.QueryPMGroup(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<PMGroupVM>.ConvertToVMList<List<PMGroupVM>>(args.Result.Rows);
                this.dgPMGroupQueryResult.ItemsSource = _vmList;
                this.dgPMGroupQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void hyperlinkPMGroupSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic pm = this.dgPMGroupQueryResult.SelectedItem as dynamic;
            if (pm != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_PMGroupMaintainUrlFormat, pm.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResPMGroupQuery.Msg_OnSelectPMGroup, MessageType.Error);
            }
        }

    }

}
