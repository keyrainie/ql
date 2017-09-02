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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.UserControls;
using ECCentral.Portal.UI.Common.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton=true)]
    public partial class QueryPayType : PageBase
    {
        public PayTypeQueryVM QueryVM { get; set; }

        public QueryPayType()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.QueryVM = new PayTypeQueryVM();
            this.gridSearchCondition.DataContext = this.QueryVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(ValidationManager.Validate(this.gridSearchCondition))
            {
                dataPayTypeList.Bind();
            }           
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_PayType_Edit))
            {
                Window.Alert(ResQueryPayType.Msg_HasNoRight);
                return;
            }
            dynamic item =this.dataPayTypeList.SelectedItem as dynamic;

            Dialog(item.SysNo);
        }

        private void dataPayTypeList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.QueryVM.PagingInfo.PageIndex = e.PageIndex;
            this.QueryVM.PagingInfo.PageSize = e.PageSize;
            this.QueryVM.PagingInfo.SortBy = e.SortField;

            new PayTypeFacade(this).QueryPayTypeList(this.QueryVM, (obj, args) =>
            {
                this.dataPayTypeList.ItemsSource = args.Result.Rows;
                this.dataPayTypeList.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_PayType_Add))
            {
                Window.Alert(ResQueryPayType.Msg_HasNoRight);
                return;
            }
            Dialog(null);
        }

        private void Dialog(int? sysNo)
        {
            UCAddPayType uc = new UCAddPayType(sysNo) { Page = this };
            uc.Dialog = Window.ShowDialog(sysNo.HasValue ? ResQueryPayType.Title_EditPayType : ResQueryPayType.Title_NewPayType, uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        dataPayTypeList.Bind();
                }
            });
        }

        private void Hyperlink_Help_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
