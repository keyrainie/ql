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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PollList : PageBase
    {
        private PollListQueryVM model;
        private PollQueryFilter filter;
        private PollQueryFilter filterVM;
        private PollFacade facade;

        public PollList()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new PollListQueryVM();
            model.ChannelID = "1";
            filter = new PollQueryFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            facade = new PollFacade(this);
            cbUserDefined.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            comStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            QuerySection.DataContext = model;
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryPollList(QueryResultGrid.QueryCriteria as PollQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.ItemsSource = DynamicConverter<PollListVM>.ConvertToVMList<List<PollListVM>>(args.Result.Rows);
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCPollAdd usercontrol = new UCPollAdd();
            usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CreatePollItem, usercontrol, (obj, args) =>
            {
                if (args.Data != null)
                {
                    Window.Navigate(string.Format(ConstValue.MKT_PollItemGroupMaintainUrlFormat, args.Data.ToString()), null, true);
                    //PollItemGroupMaintain pollItemGroup = new PollItemGroupMaintain();
                    //pollItemGroup.SysNo = int.Parse(args.Data.ToString());
                    //IDialog EditPollOptionDialog = Window.ShowDialog("投票组管理", pollItemGroup, (obj1, args1) =>
                    //{
                    //    QueryResultGrid.Bind();
                    //});
                }
            });
        }

        /// <summary>
        /// 编辑-》到投票组页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            PollListVM item = this.QueryResultGrid.SelectedItem as PollListVM;
            Window.Navigate(string.Format(ConstValue.MKT_PollItemGroupMaintainUrlFormat, item.SysNo), null, true);


            //dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            //UCPollAdd usercontrol = new UCPollAdd();
            //usercontrol.SysNo = item.SysNo;
            //IDialog dialog = Window.ShowDialog("编辑投票", usercontrol, (obj, args) =>
            //{
            //    QueryResultGrid.Bind();
            //});
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
            filter = model.ConvertVM<PollListQueryVM, PollQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 点击标题-》到投票添加页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlNewsLink_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            UCEditPollOption usercontrol = new UCEditPollOption();
            usercontrol.SysNo = item.SysNo;
            IDialog dialog = Window.ShowDialog(ResNewsInfo.Title_CreatePollOption, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                //if (ucPageType.PageType.HasValue && ucPageType.PageType.Value > 0 && ucPageType.PageType.Value <= 3 && !ucPageType.PageID.HasValue)
                //    Window.Alert(ResComment.Information_PageIDIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                //else
                //{
                filter = model.ConvertVM<PollListQueryVM, PollQueryFilter>();
                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PollQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
                //}
            }
        }

        private void hlTitleEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            UCPollAdd usercontrol = new UCPollAdd();
            usercontrol.SysNo = Convert.ToInt32(item.SysNo.ToString());
            usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CreatePollItem, usercontrol, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                   this.QueryResultGrid.Bind();
                }
            });
        }
    }

}
