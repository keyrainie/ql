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
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Data;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true)]
    public partial class LogQuery : PageBase
    {
        LogQueryFilterVM queryFilterVM;
        LogFacade facade;
        Newegg.Oversea.Silverlight.Controls.Data.DataGrid gridLogQuery;

        public LogQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new LogFacade(this);
            queryFilterVM = new LogQueryFilterVM();
            this.gridSearchCondition.DataContext = queryFilterVM;

            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                int tNum=0;
                if (int.TryParse(this.Request.Param, out tNum))
                {
                    queryFilterVM.TicketSysNo = tNum.ToString();
                    btnSearch_Click(null, null);
                }
                else
                {
                    Window.Alert("TicketSysNo错误！");
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ClearValidate();
            ValidationEntity vRequired = new ValidationEntity(ValidationEnum.IsNotEmpty, null, "值不能为空");
            List<ValidationEntity> tVList=new List<ValidationEntity>();
            tVList.Add(vRequired);

            if (!queryFilterVM.CancelOutStore && !queryFilterVM.ISSOLog
                &&string.IsNullOrEmpty( queryFilterVM.TicketSysNo))
            {
                ValidationHelper.Validation(txtTicketSysNo, tVList);
                return;
            }
            if (queryFilterVM.CancelOutStore
                && !queryFilterVM.StartDate.HasValue && !queryFilterVM.EndDate.HasValue)
            {
                Window.Alert("请选择一个时间范围!");
                dpkRang.Focus();
                //ValidationEntity vRang = new ValidationEntity(ValidationEnum.IsInteger, null, "请选择一个时间范围");
                //List<ValidationEntity> tlist = new List<ValidationEntity>();
                //tlist.Add(vRang);
                //ValidationHelper.Validation(dpkRang, tlist);
                return;
            }

            if (!queryFilterVM.CancelOutStore && !queryFilterVM.ISSOLog)
            {
                gridLogQuery = gridLogQuery1;
                panelgridLogQuery1.Visibility = Visibility.Visible;
                panelgridLogQuery2.Visibility = Visibility.Collapsed;
                panelgridLogQuery3.Visibility = Visibility.Collapsed;
            }
            else if (queryFilterVM.CancelOutStore)
            {
                gridLogQuery = gridLogQuery2;
                panelgridLogQuery2.Visibility = Visibility.Visible;
                panelgridLogQuery1.Visibility = Visibility.Collapsed;
                panelgridLogQuery3.Visibility = Visibility.Collapsed;
            }
            else if (queryFilterVM.ISSOLog)
            {
                gridLogQuery = gridLogQuery3;
                panelgridLogQuery3.Visibility = Visibility.Visible;
                panelgridLogQuery1.Visibility = Visibility.Collapsed;
                panelgridLogQuery2.Visibility = Visibility.Collapsed;
            }

            gridLogQuery.Bind();
        }

        private void gridLogQuery_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.queryFilterVM.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilterVM.PagingInfo.PageSize = e.PageSize;
            this.queryFilterVM.PagingInfo.SortBy = e.SortField;
            if (!string.IsNullOrWhiteSpace(this.queryFilterVM.TicketSysNo))
            {
                int ticketSysNo = 0;
                if (!int.TryParse(this.queryFilterVM.TicketSysNo, out ticketSysNo))
                {
                    Window.Alert("无效的TicketSysNo!");
                    return;
                }
            }

            facade.QueryLogList(this.queryFilterVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<LogQueryResultVM> logList = DynamicConverter<LogQueryResultVM>.ConvertToVMList(args.Result.Rows);

                gridLogQuery.ItemsSource = logList;
                gridLogQuery.TotalCount = args.Result.TotalCount;

            });
        }

        private void ClearValidate()
        {
            ValidationHelper.ClearValidation(txtTicketSysNo);
            ValidationHelper.ClearValidation(dpkRang);
            ValidationHelper.ClearValidation(cbxCancelOutStore);
            ValidationHelper.ClearValidation(cbxISSOLog);
        }

        private void cbxCancelOutStore_Checked(object sender, RoutedEventArgs e)
        {
            ClearValidate();
            if (cbxCancelOutStore.IsChecked.Value)
                cbxISSOLog.IsChecked = !cbxCancelOutStore.IsChecked;
        }

        private void cbxISSOLog_Checked(object sender, RoutedEventArgs e)
        {
            ClearValidate();
            if (cbxISSOLog.IsChecked.Value)
                cbxCancelOutStore.IsChecked = !cbxISSOLog.IsChecked;
        }
    }
}
