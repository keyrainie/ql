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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.UserControls.Comment;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class LeaveWordQuery : PageBase
    {
        private LeaveWordQueryVM model;
        private LeaveWordQueryFacade facade;
        private LeaveWordQueryFilter filter;
        private LeaveWordQueryFilter filterVM;

        public LeaveWordQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new LeaveWordQueryFacade(this);
            filter = new LeaveWordQueryFilter();
            model = new LeaveWordQueryVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";
            model.IsValidCase = false;
            QuerySection.DataContext = model;
            model.CreateTimeFrom = DateTime.Now.AddDays(-3);
            model.CreateTimeTo = DateTime.Now;

            comProcessStatus.ItemsSource = EnumConverter.GetKeyValuePairs<CommentProcessStatus>(EnumConverter.EnumAppendItemType.All);
            comOverTime.ItemsSource = EnumConverter.GetKeyValuePairs<OverTimeStatus>(EnumConverter.EnumAppendItemType.All);
            facade.GetLeaveWordProcessUser((s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                BizEntity.Common.UserInfo all = new BizEntity.Common.UserInfo();
                //all.SysNo = 0;
                all.UserName = ResKeywords.Option_All;
                List<BizEntity.Common.UserInfo> list = args.Result;
                list.Add(all);
                comProcessUser.ItemsSource = list;
            });	
            base.OnPageLoad(sender, e);
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<LeaveWordQueryVM, LeaveWordQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }


        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            LeaveWordQueryVM item = this.QueryResultGrid.SelectedItem as LeaveWordQueryVM;
            if (item != null)
            {
                //Window.Navigate(string.Format("/ECCentral.Portal.UI.MKT/UCAddAdvertisers/{0}", adv.SysNo), null, true);
                UCEditLeaveWord usercontrol = new UCEditLeaveWord();
                usercontrol.SysNo =int.Parse(item.SysNo);
                usercontrol.Dialog = Window.ShowDialog(ResComment.Title_ReplyCustomerLeaveWords, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
            }
            else
            {
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<LeaveWordQueryVM, LeaveWordQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<LeaveWordQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryLeaveWord(QueryResultGrid.QueryCriteria as LeaveWordQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                //gridVM = DynamicConverter<ReviewScoreItemQueryVM>.ConvertToVMList<List<ReviewScoreItemQueryVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = DynamicConverter<LeaveWordQueryVM>.ConvertToVMList<List<LeaveWordQueryVM>>(args.Result.Rows); //gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });	
        }
        private void hyperlinkSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            if (item != null)
            {
                this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, item.SOSysNo), null, true);
            }
        }
        private void hyperlinkCustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            if (item != null)
            {
                this.Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, item.CustomerSysNo), null, true);
            }
        }
    }

}
