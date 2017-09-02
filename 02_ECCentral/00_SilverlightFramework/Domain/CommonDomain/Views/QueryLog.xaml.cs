using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogService;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Windows.Input;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View]
    public partial class QueryLog : PageBase
    {
        private QueryLogConfigClient m_queryLogConfigClient;
        private QueryLogEntryClient m_queryLogEntryClient;

        public QueryLog()
        {
            InitializeComponent();

            this.GridSearchArea.DataContext = new LogQueryModel();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.m_queryLogConfigClient = new QueryLogConfigClient();
            this.Window.DocumentVerticalScrollBar = ScrollBarVisibility.Disabled;
            this.Window.DocumentHorizontalScrollBar = ScrollBarVisibility.Disabled;

            this.m_queryLogConfigClient.GetGlobalRegionCompleted += new EventHandler<GetGlobalRegionCompletedEventArgs>(serviceClient_GetGlobalRegionCompleted);
            this.m_queryLogConfigClient.GetLocalRegionCompleted += new EventHandler<GetLocalRegionCompletedEventArgs>(serviceClient_GetLocalRegionCompleted);
            this.m_queryLogConfigClient.GetCategoryConfigsCompleted += new EventHandler<GetCategoryConfigsCompletedEventArgs>(serviceClient_GetCategoryConfigsCompleted);

            m_queryLogEntryClient = new QueryLogEntryClient(this);
            this.m_queryLogEntryClient.GetLogsCompleted += new EventHandler<GetLogsCompletedEventArgs>(serviceClient_GetLogsCompleted);

            this.ButtonSearch.Click += new RoutedEventHandler(ButtonSearch_Click);

            this.textBoxLogId.KeyUp += new System.Windows.Input.KeyEventHandler(TextBox_KeyUp);
            this.textBoxRefKey.KeyUp += new System.Windows.Input.KeyEventHandler(TextBox_KeyUp);
            this.GetGlobalRegion();
        }

        #region CallBack

        private void serviceClient_GetGlobalRegionCompleted(object sender, GetGlobalRegionCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            if (e.Result.ResultList != null)
            {
                LogGlobalRegionBody firstItem = new LogGlobalRegionBody()
                {
                    GlobalID = null,
                    GlobalName = CommonResource.ComboBox_ExtraAllText
                };
                List<LogGlobalRegionBody> list = e.Result.ResultList.ToList();
                list.Insert(0, firstItem);

                this.ddlGlobal.ItemsSource = list;

                LogQueryModel queryModel = e.UserState as LogQueryModel;
                if (queryModel != null)
                {
                    this.ddlGlobal.SelectedValue = queryModel.GlobalID;
                }
                else
                {
                    this.ddlGlobal.SelectedIndex = 0;
                }
            }

            this.DataGridLogList.QueryCriteria = GridSearchArea.DataContext;
            this.DataGridLogList.Bind();
        }

        private void serviceClient_GetLocalRegionCompleted(object sender, GetLocalRegionCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            if (e.Result.ResultList != null)
            {
                LogLocalRegionBody firstItem = new LogLocalRegionBody()
                {
                    GlobalID = null,
                    LocalID = null,
                    LocalName = CommonResource.ComboBox_ExtraAllText
                };
                List<LogLocalRegionBody> list = e.Result.ResultList.ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Status == Status.Inactive)
                    {
                        list.Remove(list[i]);
                    }
                }
                list.Insert(0, firstItem);

                this.ddlLocal.ItemsSource = list;
                this.ddlLocal.IsEnabled = true;


                this.ddlCategory.IsEnabled = false;

                LogQueryModel queryModel = e.UserState as LogQueryModel;
                if (queryModel != null)
                {
                    this.ddlLocal.SelectedValue = queryModel.LocalID;
                }
                else
                {
                    this.ddlLocal.SelectedIndex = 0;
                }
            }
        }

        private void serviceClient_GetCategoryConfigsCompleted(object sender, GetCategoryConfigsCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            if (e.Result.ResultList != null)
            {
                LogCategoryBody firstItem = new LogCategoryBody()
                {
                    GlobalID = null,
                    LocalID = null,
                    CategoryName = null,
                    CategoryDescription = CommonResource.ComboBox_ExtraAllText
                };
                List<LogCategoryBody> list = e.Result.ResultList.ToList();
                list.ForEach(item => item.CategoryDescription = item.CategoryName);
                list.Insert(0, firstItem);

                this.ddlCategory.ItemsSource = list;
                this.ddlCategory.IsEnabled = true;

                LogQueryModel queryModel = e.UserState as LogQueryModel;
                if (queryModel != null)
                {
                    this.ddlCategory.SelectedValue = queryModel.CategoryName;
                }
                else
                {
                    this.ddlCategory.SelectedIndex = 0;
                }
            }
        }

        private void serviceClient_GetLogsCompleted(object sender, GetLogsCompletedEventArgs e)
        {
            this.ButtonSearch.IsEnabled = true;

            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            this.DataGridLogList.ItemsSource = e.Result.ResultList;
            this.DataGridLogList.TotalCount = e.Result.PagingInfo.TotalCount;
        }

        #endregion

        private void GetGlobalRegion()
        {
            LogGlobalQueryCriteria queryCriteria = new LogGlobalQueryCriteria()
            {
                GlobalRegionStatus = Status.Active
            };

            var dataSource = GridSearchArea.DataContext;

            this.m_queryLogConfigClient.GetGlobalRegionAsync(queryCriteria, dataSource);
        }

        private void ddlGlobal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            LogGlobalRegionBody selectedItem = this.ddlGlobal.SelectedItem as LogGlobalRegionBody;

            if (selectedItem != null && selectedItem.GlobalID != null)
            {
                LogLocalQueryCriteria queryCriteria = new LogLocalQueryCriteria()
                {
                    GlobalID = selectedItem.GlobalID,
                    LocalRegionStatus = Status.Active
                };

                var dataSource = GridSearchArea.DataContext;
                ddlLocal.IsShowLoading = true;
                this.m_queryLogConfigClient.GetLocalRegionAsync(queryCriteria, dataSource);
            }
            else
            {
                this.ddlLocal.ItemsSource = null;
                this.ddlLocal.IsEnabled = false;
                this.ddlCategory.ItemsSource = null;
                this.ddlCategory.IsEnabled = false;
            }
        }

        private void ddlLocal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogLocalRegionBody selectedItem = this.ddlLocal.SelectedItem as LogLocalRegionBody;

            if (selectedItem != null && selectedItem.GlobalID != null && selectedItem.LocalID != null)
            {
                LogCategoryQueryCriteria queryCriteria = new LogCategoryQueryCriteria()
                {
                    GlobalID = selectedItem.GlobalID,
                    LocalID = selectedItem.LocalID,
                    Status = Status.Active
                };

                var dataSource = GridSearchArea.DataContext;
                ddlCategory.IsShowLoading = true;
                this.m_queryLogConfigClient.GetCategoryConfigsAsync(queryCriteria, dataSource);
            }
            else
            {
                this.ddlCategory.ItemsSource = null;
                this.ddlCategory.IsEnabled = false;
            }
        }

        void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {

            Shortcuts.SetEnable(ButtonSearch, false);
            var logQueryModel = GridSearchArea.DataContext as LogQueryModel;

            this.DataGridLogList.QueryCriteria = logQueryModel;
            this.DataGridLogList.Bind();
        }

        private void dgLog_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            LogQueryModel queryModel = e.QueryCriteria as LogQueryModel;
            if (queryModel != null)
            {
                LogQueryCriteria queryCriteria = queryModel.ToQueryCriteria();
                queryCriteria.PagingInfo = new Newegg.Oversea.Silverlight.CommonDomain.QueryLogService.PagingInfo()
                {
                    PageSize = e.PageSize,
                    StartRowIndex = e.PageIndex * e.PageSize,
                    SortField = e.SortField
                };

                this.ButtonSearch.IsEnabled = false;

                this.m_queryLogEntryClient.GetLogsAsync(queryCriteria);
            }
        }

        private void DataGridHyperlinkCategory_Click(object sender, RoutedEventArgs e)
        {
            LogEntryBody logEntry = (LogEntryBody)((FrameworkElement)sender).DataContext;
            App.s_isRefreshForNavigation = true;
            Window.Navigate("/CommonDomain/LogCategoryConfig?GlobalID=" + logEntry.GlobalID + "&LocalID=" + logEntry.LocalID + "&CategoryName=" + logEntry.CategoryName, null, true);
        }

        void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                ButtonSearch_Click(null, null);
            }
        }
    }

}
