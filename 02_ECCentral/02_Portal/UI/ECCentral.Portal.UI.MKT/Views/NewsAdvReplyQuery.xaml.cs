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
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class NewsAdvReplyQuery : PageBase
    {
        private NewsAdvReplyQueryVM model;
        private NewsAdvReplyQueryFacade facade;
        private NewsAdvReplyQueryFilter filter;
        private List<NewsAdvReplyVM> gridVM;
        private NewsAdvReplyQueryFilter filterVM;

        public NewsAdvReplyQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new NewsAdvReplyQueryFacade(this);
            model = new NewsAdvReplyQueryVM();
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            filter = new NewsAdvReplyQueryFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            showStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.MKT.NewsAdvReplyStatus>(EnumConverter.EnumAppendItemType.All);
            comIsUploadImage.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.MKT.NYNStatus>(EnumConverter.EnumAppendItemType.All);
            CodeNamePairHelper.GetList("MKT", "CommentsCategory",CodeNamePairAppendItemType.All, (obj, args) =>
            {
	            if (args.FaultsHandle()) return;
                comCommentCategory.ItemsSource = args.Result;
            });
            
            facade.GetNewAdvReplyCreateUsers((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                BizEntity.Common.UserInfo select = new BizEntity.Common.UserInfo();
                select.UserID="0";
                select.UserName= ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All;
                List<BizEntity.Common.UserInfo> source= args.Result;
                source.Add(select);
                lastEditUsers.ItemsSource = source;
            });

            showStatus.SelectedIndex = 0;
            lastEditUsers.SelectedIndex = 0; 
            comCommentCategory.SelectedIndex = 0;
            comIsUploadImage.SelectedIndex = 0;
            base.OnPageLoad(sender, e);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<NewsAdvReplyQueryVM, NewsAdvReplyQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<NewsAdvReplyQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
			
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
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<NewsAdvReplyQueryVM, NewsAdvReplyQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryNewsAdvReply(QueryResultGrid.QueryCriteria as NewsAdvReplyQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                gridVM = DynamicConverter<NewsAdvReplyVM>.ConvertToVMList<List<NewsAdvReplyVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                if (gridVM != null)
                {
                    btnShow.IsEnabled = true;
                    btnUnShow.IsEnabled = true;
                }
                else
                {
                    btnShow.IsEnabled = false;
                    btnUnShow.IsEnabled = false;
                }
            });
        }

        /// <summary>
        /// 批量展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            List<int> itemsSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    itemsSysNo.Add(item.SysNo.Value);
            });
            if (itemsSysNo.Count > 0)
                facade.BatchSetNewsAdvReplyShow(itemsSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 批量屏蔽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnShow_Click(object sender, RoutedEventArgs e)
        {
            List<int> itemsSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    itemsSysNo.Add(item.SysNo.Value);
            });
            if (itemsSysNo.Count > 0)
                facade.BatchSetNewsAdvReplyHide(itemsSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {
            NewsAdvReplyVM vm = this.QueryResultGrid.SelectedItem as NewsAdvReplyVM;
            NewsAdvReply item = vm.ConvertVM<NewsAdvReplyVM, NewsAdvReply>();
            facade.UpdateNewsAdvReplyStatus(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 查看图片  ？？？？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbCheckImage_Click(object sender, RoutedEventArgs e)
        {
            NewsAdvReplyVM item = this.QueryResultGrid.SelectedItem as NewsAdvReplyVM;
            if (item != null)
            {
                UCManageNewsAdvReplyImage usercontrol = new UCManageNewsAdvReplyImage();
                usercontrol.SysNo = item.SysNo.Value;
                usercontrol.ImageList = item.Image;
                usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_ManagerImage, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
            }
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 评论回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlReply_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            UCAddNewsAdvReply usercontrol = new UCAddNewsAdvReply();
            usercontrol.SysNo = item.SysNo;
            usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CommentReply, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridVM == null || checkBoxAll == null)
                return;
            gridVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

    }
}
