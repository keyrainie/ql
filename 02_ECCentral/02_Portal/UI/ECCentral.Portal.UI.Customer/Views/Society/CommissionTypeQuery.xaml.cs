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
using ECCentral.Portal.UI.Customer.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.UserControls.Customer;
using ECCentral.Portal.UI.Customer.UserControls.Gift;
using ECCentral.Portal.UI.Customer.Facades.RequestMsg;

namespace ECCentral.Portal.UI.Customer.Views.Society
{
    [View(IsSingleton = true)]
    public partial class CommissionTypeQuery : PageBase
    {
        public CommissionTypeQueryVM QueryVM { get; set; }
        
        public CommissionTypeQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.QueryVM = new CommissionTypeQueryVM();
            this.gridSearchCondition.DataContext = this.QueryVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridSearchCondition))
            {
                dataCommissionTypeList.Bind();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_PayType_Edit))
            {
                Window.Alert(ResCommissionTypeQuery.Msg_HasNoRight);
                return;
            }
            dynamic item = this.dataCommissionTypeList.SelectedItem as dynamic;

            Dialog(item.SysNo);
        }

        private void dataCommissionTypeList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.QueryVM.PagingInfo.PageIndex = e.PageIndex;
            this.QueryVM.PagingInfo.PageSize = e.PageSize;
            this.QueryVM.PagingInfo.SortBy = e.SortField;

            new CommissionTypeQueryFacade(this).QueryCommissionType(this.QueryVM, (obj, args) =>
            {
                this.dataCommissionTypeList.ItemsSource = args.Result.Rows;
                this.dataCommissionTypeList.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_PayType_Add))
            //{
            //    Window.Alert(ResCommissionTypeQuery.Msg_HasNoRight);
            //    return;
            //}
            Dialog(null);
        }

        private void Dialog(int? sysNo)
        {
            UCAddCommissionType uc = new UCAddCommissionType(sysNo) { Page = this };
            uc.Dialog = Window.ShowDialog(sysNo.HasValue ? ResCommissionTypeQuery.Title_EditPayType : ResCommissionTypeQuery.Title_NewPayType, uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        dataCommissionTypeList.Bind();
                }
            });
        }

        private void Hyperlink_Help_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
