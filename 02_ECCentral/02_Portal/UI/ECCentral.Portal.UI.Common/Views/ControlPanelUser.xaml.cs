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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true)]
    public partial class ControlPanelUser : PageBase
    {
        public ControlPanelUserQueryFilterVM QueryVM { get; set; }

        public ControlPanelUser()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.QueryVM = new ControlPanelUserQueryFilterVM();
            this.gridSearchCondition.DataContext = this.QueryVM;

            CodeNamePairHelper.GetList(ConstValue.DomainName_Common
               , new string[] { ConstValue.Key_ControlPanelDept }
               , CodeNamePairAppendItemType.All, (o, p) =>
               {
                   comDept.ItemsSource = p.Result[ConstValue.Key_ControlPanelDept];
               });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridSearchCondition))
            {
                controlPanelUserGrid.Bind();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_UserMgmt_Edit))
            {
                Window.Alert(ResControlPanelUser.Msg_HasNoRight);
                return;
            }
            dynamic item = this.controlPanelUserGrid.SelectedItem as dynamic;

            Dialog(item.SysNo);
        }

        private void UserGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.QueryVM.PagingInfo.PageIndex = e.PageIndex;
            this.QueryVM.PagingInfo.PageSize = e.PageSize;
            this.QueryVM.PagingInfo.SortBy = e.SortField;

            new ControlPanelUserFacade(this).QueryUserList(this.QueryVM, (obj, args) =>
            {
                this.controlPanelUserGrid.ItemsSource = args.Result.Rows;
                this.controlPanelUserGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_UserMgmt_Add))
            {
                Window.Alert(ResControlPanelUser.Msg_HasNoRight);
                return;
            }
            Dialog(null);
        }

        private void Dialog(int? sysNo)
        {
            UCAddControlPanelUser uc = new UCAddControlPanelUser(sysNo) { Page = this };
            uc.Dialog = Window.ShowDialog(sysNo.HasValue ? ResControlPanelUser.Title_EditPayType : ResControlPanelUser.Title_NewPayType, uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        controlPanelUserGrid.Bind();
                }
            });
        }

    }
}
