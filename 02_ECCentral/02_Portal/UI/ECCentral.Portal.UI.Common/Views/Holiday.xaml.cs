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
    public partial class Holiday : PageBase
    {
        HolidayQueryFilterVM queryFilterVM;
        HolidayFacade facade;

        public Holiday()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new HolidayFacade(this);
            queryFilterVM = new HolidayQueryFilterVM();
            this.gridSearchCondition.DataContext = queryFilterVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridSearchCondition))
            {
                holidayGrid.Bind();
            }
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_Holiday_Delete))
            {
                Window.Alert(ResHoliday.Msg_HasNoRight);
                return;
            }
            List<int?> sysnoList = new List<int?>();
            dynamic rows = holidayGrid.ItemsSource;
            if (rows == null)
            {
                Window.Alert(ResHoliday.Info_DeleteSelect);
                return;
            }

            foreach (var row in rows)
            {
                if (row.IsChecked)
                {
                    sysnoList.Add(row.SysNo);
                }
            }
            if (sysnoList.Count <= 0)
            {
                Window.Alert(ResHoliday.Info_DeleteSelect);
                return;
            }
            Window.Confirm(ResHoliday.ConfirmText_Delete, (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    facade.DeleteHolidayBatch(sysnoList, (s, args) =>
                    {
                        Window.Alert("提示信息", ResHoliday.Info_DeleteSuccessfully, MessageType.Information, (g, ar) =>
                        {
                            holidayGrid.Bind();
                        });
                    });
                }
            });
        }

        private void holidayGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.queryFilterVM.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilterVM.PagingInfo.PageSize = e.PageSize;
            this.queryFilterVM.PagingInfo.SortBy = e.SortField;

            facade.QueryHolidayList(this.queryFilterVM, (obj, args) =>
            {
                this.holidayGrid.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.holidayGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_Holiday_Add))
            {
                Window.Alert(ResHoliday.Msg_HasNoRight);
                return;
            }
            Dialog(null);
        }

        private void Dialog(int? sysNo)
        {
            UCAddHoliday uc = new UCAddHoliday(sysNo) { Page = this };
            uc.Dialog = Window.ShowDialog(ResHoliday.Title_NewHoliday, uc, (obj, args) =>
            {
                if (args != null)
                {
                    if (args.DialogResult == DialogResultType.OK)
                        holidayGrid.Bind();
                }
            });
        }

        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = holidayGrid.ItemsSource;
            foreach (dynamic row in rows)
            {
                row.IsChecked = chk.IsChecked.Value;
            }
        }

    }
}
