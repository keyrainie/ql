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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using System.Text.RegularExpressions;
using Newegg.Oversea.Silverlight.ControlPanel.Core;


namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class NewsAndBulletinNew : PageBase
    {
        private NewsInfoQueryVM viewModel;
        private NewsFacade facade;
        private NewsInfoQueryFilter filter;

        public NewsAndBulletinNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new NewsFacade(this);
            viewModel = new NewsInfoQueryVM();
            QuerySection.DataContext = viewModel;
            filter = new NewsInfoQueryFilter();          
            InitPageControlsData();
        }

        


        private void InitPageControlsData()
        {
            facade.LoadCreateUsers((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                args.Result.Insert(0, new UserInfo() { UserName = ResCommonEnum.Enum_All });
                cbInUsers.ItemsSource = args.Result;
                lstChannel.SelectedIndex = 1;
            });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            filter = viewModel.ConvertVM<NewsInfoQueryVM, NewsInfoQueryFilter>();
            filter.NewsType = ucPosition.PageType;
            filter.ReferenceSysNo = ucPosition.PageID;
            filter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            Regex regex = new Regex("<(.[^>]*)>", RegexOptions.IgnoreCase);

            facade.QueryNews(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows.ToList("IsChecked", false))
                {
                    row["Title"] = regex.Replace(row["Title"], "");                 
                    list.Add(row);
                }

                QueryResultGrid.ItemsSource = list;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {

            UCAddNewsAndBulletinNew addNews = new UCAddNewsAndBulletinNew(Window);
            addNews.dialog = Window.ShowDialog(ResNewsInfo.Title_AddNewsAndBulletinNew, addNews, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    QueryResultGrid.Bind();
                }
            });
        }


        private List<int> GetSelectedSysNoList()
        {
            List<int> sysNoList = new List<int>();
            if (this.QueryResultGrid.ItemsSource != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        sysNoList.Add(view.SysNo);
                    }
                }
            }
            return sysNoList;
        }

        /// <summary>
        /// 批量不显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchHide_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = GetSelectedSysNoList();

            if (list.Count == 0)
            {
                Window.Alert(ResKeywords.Information_MoreThanOneRecord);
                return;
            }
            else
            {
                facade.Deactive(list, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_SettingSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    QueryResultGrid.Bind();
                });
            }
        }
        private void hlNewsLink_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }
        //选择全部
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void hyperlinkEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic news = this.QueryResultGrid.SelectedItem as dynamic;
            if (news != null)
            {
                UCAddNewsAndBulletinNew editNews = new UCAddNewsAndBulletinNew(Window);
                editNews.entity.SysNo = news.SysNo;
                editNews.dialog = Window.ShowDialog(ResNewsInfo.Title_EditNews, editNews, (obj1, args1) =>
                {
                    if (args1.DialogResult == DialogResultType.OK)
                    {
                        QueryResultGrid.Bind();
                    }
                });

            }
        }

    }

}
