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
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true)]
    public partial class AreaDelivery : PageBase
    {
        AreaDeliveryQueryFilterVM queryFilterVM;
        AreaDeliveryFacade areaDeliveryFacade;
        CommonDataFacade commonFacade;

        public AreaDelivery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            areaDeliveryFacade = new AreaDeliveryFacade(this);
            commonFacade=new CommonDataFacade(this);
            queryFilterVM = new AreaDeliveryQueryFilterVM();
            this.gridSearchCondition.DataContext = queryFilterVM;

            areaDeliveryFacade.QueryWHAreaList((s, args) =>
            {
                if (args.FaultsHandle()) 
                    return;
                args.Result.Insert(0, new Service.Common.Restful.ResponseMsg.AreaDelidayResponse() { WHArea = null, City = ResAreaDelivery.ComboBox_PleaseSelect });
                comCity.ItemsSource = args.Result;
                comCity.SelectedIndex = 0;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridSearchCondition))
            {
                areaDeliveryGrid.Bind();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_AreaDelivery_Edit))
            {
                Window.Alert(ResAreaDelivery.Msg_HasNoRight);
                return;
            }
            dynamic item = this.areaDeliveryGrid.SelectedItem as dynamic;

            Dialog(item.SysNo);
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_AreaDelivery_Delete))
            {
                Window.Alert(ResAreaDelivery.Msg_HasNoRight);
                return;
            }
            Window.Confirm(ResAreaDelivery.ConfirmText_Delete, (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic item = this.areaDeliveryGrid.SelectedItem as dynamic;
                    string tmpSysNo = item.SysNo.ToString();
                    areaDeliveryFacade.DeleteAreaDelivery(tmpSysNo, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("操作已成功！", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                        areaDeliveryGrid.Bind();
                    });
                }
            });
        }

        private void areaDeliveryGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.queryFilterVM.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilterVM.PagingInfo.PageSize = e.PageSize;
            this.queryFilterVM.PagingInfo.SortBy = e.SortField;

           areaDeliveryFacade.QueryAreaDeliveryList(this.queryFilterVM, (obj, args) =>
            {
                this.areaDeliveryGrid.ItemsSource = args.Result.Rows;
                this.areaDeliveryGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_AreaDelivery_Add))
            {
                Window.Alert(ResAreaDelivery.Msg_HasNoRight);
                return;
            }
            Dialog(null);
        }

        private void Dialog(int? sysNo)
        {
            UCAddAreaDelivery uc = new UCAddAreaDelivery(sysNo) { Page = this };
            uc.Dialog = Window.ShowDialog(sysNo.HasValue ? ResAreaDelivery.Title_EditPayType : ResAreaDelivery.Title_NewPayType, uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        areaDeliveryGrid.Bind();
                }
            });
        }

    }
}
