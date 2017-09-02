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
using ECCentral.Portal.UI.IM.UserControls;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PMQuery : PageBase
    {

        PMQueryVM model;
        private List<PMVM> _vmList;

        public PMQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new PMQueryVM();
            this.DataContext = model;
            cbPMStatus.SelectedIndex = 0;
        }

        private void dgPMQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PMQueryFacade facade = new PMQueryFacade(this);

            facade.QueryPM(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<PMVM>.ConvertToVMList<List<PMVM>>(args.Result.Rows);
                this.dgPMQueryResult.ItemsSource = _vmList;
                this.dgPMQueryResult.TotalCount = args.Result.TotalCount;
            });
        }        

        private void btnPMSearch_Click(object sender, RoutedEventArgs e)
        {
            dgPMQueryResult.Bind();
        }

        private void btnPMNew_Click(object sender, RoutedEventArgs e)
        {
            dynamic pm = this.dgPMQueryResult.SelectedItem as dynamic;
            ProductManagerMaintain detail = new ProductManagerMaintain() {IsUpdate=false };
            detail.Dialog = Window.ShowDialog("添加PM", detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                   
                }
            }, new Size(800, 600));
        }

        private void hyperlinkPMSysNo_Click(object sender, RoutedEventArgs e)
        {

            //dynamic pm = this.dgPMQueryResult.SelectedItem as dynamic;
            //if (pm != null)
            //{
            //    this.Window.Navigate(string.Format(ConstValue.IM_PMMaintainUrlFormat, pm.SysNo), null, true);
            //}
            //else
            //{
            //    Window.Alert(ResPMQuery.Msg_OnSelectPM, MessageType.Error);
            //}
            dynamic pm = this.dgPMQueryResult.SelectedItem as dynamic;
            ProductManagerMaintain detail = new ProductManagerMaintain() {IsUpdate=true,SysNo= pm.SysNo };
            detail.Dialog = Window.ShowDialog("更新PM", detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {

                }
            }, new Size(800, 600));
        }

    }

}
