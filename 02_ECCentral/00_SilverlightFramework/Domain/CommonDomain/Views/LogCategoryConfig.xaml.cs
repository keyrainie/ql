using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.CommonDomain.UserControls;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;


namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View]
    public partial class LogCategoryConfig : PageBase
    {
        #region Fields.

        private QueryLogConfigClient m_queryLogConfigClient;
        private RadioButton m_selectedRadioButton;
        private CategoryQueryModel m_criteria;
        private int m_selectedIdx = -1;
        private bool m_isLoaded;
        private bool m_isUpdateLocalOrGlobal;

        #endregion

        #region Constr.

        public LogCategoryConfig()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            AuthorizeUserFuncPoint();



            GridSearchArea.DataContext = new CategoryQueryModel();

            Window.DocumentVerticalScrollBar = ScrollBarVisibility.Disabled;
            Window.DocumentHorizontalScrollBar = ScrollBarVisibility.Disabled;

            m_queryLogConfigClient = new QueryLogConfigClient(this);
            Window.WindowStatusChanged += WindowStatusChanged;

            m_queryLogConfigClient.GetGlobalRegionCompleted += QueryLogConfigClientGetGlobalRegionCompleted;
            m_queryLogConfigClient.GetLocalRegionCompleted += QueryLogConfigClientGetLocalRegionCompleted;
            m_queryLogConfigClient.GetCategoryConfigsCompleted += QueryLogConfigClientGetCategoryConfigsCompleted;

            ButtonSearch.Click += new RoutedEventHandler(ButtonSearch_Click);
            TextBoxCategoryName.KeyUp += new KeyEventHandler(TextBox_KeyUp);

            MaintainArea.Children.Add(new MaintainLogCategory(this));
            ((UserControl)MaintainArea.Children[0]).IsEnabled = false;

            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());
        }

        void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                //ButtonSearch_Click(null, null);
                this.Window.LoadingSpin.Show();
            }
        }

        void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGridLogCategory.QueryCriteria = GridSearchArea.DataContext;
            DataGridLogCategory.Bind();
        }

        #endregion

        #region Service Call Back.

        void QueryLogConfigClientGetCategoryConfigsCompleted(object sender, GetCategoryConfigsCompletedEventArgs e)
        {
            ButtonSearch.IsEnabled = true;
            if (Window.FaultHandle.Handle(e))
            {
                return;
            }
            DataGridLogCategory.ItemsSource = e.Result.ResultList;
            DataGridLogCategory.TotalCount = e.Result.PagingInfo.TotalCount;

            if (!m_isLoaded && Request != null && Request.QueryString != null && Request.QueryString.Count > 0)
            {
                m_isLoaded = true;
                DataGridLogCategory.SelectedIndex = 0;
                m_selectedRadioButton = new RadioButton
                                            {
                                                DataContext = DataGridLogCategory.SelectedItem,
                                                IsChecked = true
                                            };
                RadioButtonChecked(m_selectedRadioButton, null);
                ((QueryLogConfigService.LogCategoryBody)m_selectedRadioButton.DataContext).IsChecked = true;
            }

            if (m_selectedIdx > -1 && m_isUpdateLocalOrGlobal)
            {
                DataGridLogCategory.SelectedIndex = m_selectedIdx;
                if (DataGridLogCategory.SelectedItem != null)
                {
                    ((QueryLogConfigService.LogCategoryBody)DataGridLogCategory.SelectedItem).IsChecked = true;
                }
                m_isUpdateLocalOrGlobal = false;
            }
        }

        void QueryLogConfigClientGetLocalRegionCompleted(object sender, GetLocalRegionCompletedEventArgs e)
        {
            if (Window.FaultHandle.Handle(e))
            {
                return;
            }
            List<QueryLogConfigService.LogLocalRegionBody> list;
            if (e.Result.ResultList != null && e.Result.ResultList.Count() > 0)
            {
                list = new List<QueryLogConfigService.LogLocalRegionBody>(e.Result.ResultList);
            }
            else
            {
                list = new List<QueryLogConfigService.LogLocalRegionBody>();
            }
            list.Insert(0, new QueryLogConfigService.LogLocalRegionBody
                               {
                                   LocalName = CommonResource.ComboBox_ExtraAllText
                               });
            ComboBoxLocal.ItemsSource = list;

            var criteria = GridSearchArea.DataContext as CategoryQueryModel;
            if (criteria != null)
            {
                ComboBoxLocal.SelectedValue = criteria.LocalID;
            }
            else
            {
                ComboBoxLocal.SelectedIndex = 0;
            }

            if (!m_isLoaded)
            {
                if (Request != null && Request.QueryString != null && Request.QueryString.Count > 0)
                {
                    ((CategoryQueryModel)GridSearchArea.DataContext).CategoryName = Request.QueryString["CategoryName"];

                    ((CategoryQueryModel)GridSearchArea.DataContext).LocalID = Request.QueryString["LocalID"];
                    ButtonSearch_Click(null, null);
                }
            }

            if (m_criteria != null)
            {
                ComboBoxLocal.SelectedValue = m_criteria.LocalID;
                if (DataGridLogCategory.ItemsSource != null && m_isUpdateLocalOrGlobal)
                {
                    ButtonSearch_Click(null, null);
                }
            }
        }

        void QueryLogConfigClientGetGlobalRegionCompleted(object sender, GetGlobalRegionCompletedEventArgs e)
        {
            if (Window.FaultHandle.Handle(e))
            {
                return;
            }
            ObservableCollection<QueryLogConfigService.LogGlobalRegionBody> list = e.Result.ResultList;

            if (list != null && list.Count > 0)
            {
                list.Insert(0, new QueryLogConfigService.LogGlobalRegionBody
                                   {
                                       GlobalName = CommonResource.ComboBox_ExtraAllText
                                   });
            }

            ComboBoxGlobal.ItemsSource = list;

            if (list != null && list.Count > 0)
            {
                var dataContext = GridSearchArea.DataContext as CategoryQueryModel;

                if (dataContext != null)
                {
                    if (Request != null && Request.QueryString != null && Request.QueryString.Count > 0)
                    {
                        dataContext.GlobalID = Request.QueryString["GlobalID"];
                    }
                    else
                    {
                        dataContext.GlobalID = list[0].GlobalID;
                    }
                }
            }

            if (m_criteria != null)
            {
                ComboBoxGlobal.SelectedValue = m_criteria.GlobalID;
            }
        }

        #endregion

        #region Event.

        void WindowStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.IsActive && App.s_isRefreshForNavigation)
            {
                App.s_isRefreshForNavigation = false;
                Window.Refresh();
            }
        }

        private void ComboBoxGlobalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxGlobal.ItemsSource != null && ComboBoxGlobal.SelectedValue == null)
            {
                var list = new ObservableCollection<QueryLogConfigService.LogLocalRegionBody>
                               {
                                   new QueryLogConfigService.LogLocalRegionBody
                                       {
                                           LocalName = CommonResource.ComboBox_ExtraAllText
                                       }
                               };

                ComboBoxLocal.ItemsSource = list;
            }
            else
            {
                ComboBoxLocal.IsShowLoading = true;
                m_queryLogConfigClient.GetLocalRegionAsync(new LogLocalQueryCriteria
                                                               {
                                                                   GlobalID =
                                                                       ((QueryLogConfigService.LogGlobalRegionBody)
                                                                        ComboBoxGlobal.SelectedItem).GlobalID,
                                                                   LocalRegionStatus =
                                                                       QueryLogConfigService.Status.Active
                                                               });
            }
        }

        private void DataGridLogCategoryLoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            var queryModel = e.QueryCriteria as CategoryQueryModel;

            if (queryModel != null)
            {
                LogCategoryQueryCriteria queryCriteria = queryModel.ToContract();

                queryCriteria.PagingInfo = new PagingInfo
                                               {
                                                   PageSize = e.PageSize,
                                                   StartRowIndex = e.PageIndex * e.PageSize,
                                                   SortField = e.SortField
                                               };

                m_queryLogConfigClient.GetCategoryConfigsAsync(queryCriteria);
                ButtonSearch.IsEnabled = false;
            }
        }

        private void BorderShowMaintainGlobalMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var userControl = new MaintainGlobalInfo(this, (MaintainLogCategory)MaintainArea.Children[0]);
            var dialog = Window.ShowDialog(LogCategoryConfigResource.LogCategoryConfig_ShowDialogMaintainGlobal_Title, userControl);
            userControl.Dialog = dialog;
        }

        private void BorderShowMaintainLocalMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var userControl = new MaintainLocalInfo(this, (MaintainLogCategory)MaintainArea.Children[0]);
            var dialog = Window.ShowDialog(LogCategoryConfigResource.LogCategoryConfig_ShowDialogMaintainLocal_Title, userControl);
            userControl.Dialog = dialog;
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            // ReSharper disable PossibleInvalidOperationException
            if (((RadioButton)sender).IsChecked.Value && ((RadioButton)sender).DataContext != null)
            // ReSharper restore PossibleInvalidOperationException
            {
                m_selectedIdx = DataGridLogCategory.SelectedIndex;
                if (!ButtonSave.IsEnabled)
                {
                    ButtonSave.IsEnabled = true;
                }

                m_selectedRadioButton = sender as RadioButton;

                ((UserControl)MaintainArea.Children[0]).IsEnabled = true;

                var category = (QueryLogConfigService.LogCategoryBody)m_selectedRadioButton.DataContext;
                var maintainCategory = new MaintainLogConfigService.LogCategoryBody
                                           {
                                               CategoryDescription = category.CategoryDescription,
                                               CategoryName = category.CategoryName,
                                               EnableRemoveLog = category.EnableRemoveLog,
                                               GlobalID = category.GlobalID,
                                               GlobalName = category.GlobalName,
                                               InDate = category.InDate,
                                               InUser = category.InUser,
                                               LocalID = category.LocalID,
                                               LocalName = category.LocalName,
                                               LogType = ContractConvert.ConvertFromQueryToMaintain(category.LogType),
                                               RemoveOverDays = category.RemoveOverDays,
                                               MyRemoveOverDays = category.RemoveOverDays.ToString(),
                                               Status = ContractConvert.ConvertFromQueryToMaintain(category.Status)
                                           };
                if (category.EmailNotification != null)
                {
                    maintainCategory.EmailNotification = new MaintainLogConfigService.EmailNotificationConfig
                                                             {
                                                                 Address = category.EmailNotification.Address,
                                                                 FilterDuplicate =
                                                                     category.EmailNotification.FilterDuplicate,
                                                                 Interval = category.EmailNotification.Interval,
                                                                 NeedNotify = category.EmailNotification.NeedNotify,
                                                                 Subject = category.EmailNotification.Subject
                                                             };
                }

                ((MaintainLogCategory)MaintainArea.Children[0]).BindDataContext(maintainCategory);
            }
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            m_selectedIdx = -1;
            ((MaintainLogCategory)MaintainArea.Children[0]).ResetDataContext();

            if (!ButtonSave.IsEnabled)
            {
                ButtonSave.IsEnabled = true;
            }

            if (m_selectedRadioButton != null)
            {
                ((QueryLogConfigService.LogCategoryBody)m_selectedRadioButton.DataContext).IsChecked = false;
                m_selectedRadioButton.IsChecked = false;
                m_selectedRadioButton = null;
            }
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            ((MaintainLogCategory)MaintainArea.Children[0]).PersistLogCategory();
        }

        #endregion

        #region Private Methods.

        private void AuthorizeUserFuncPoint()
        {
            bool canEditGlobalRegion = Window.AuthManager.HasFunction("CP_AuthKey_LogCategoryConfig@EditGlobalRegion");
            bool canEditLocalRegion = Window.AuthManager.HasFunction("CP_AuthKey_LogCategoryConfig@EditLocalRegion");

            BorderShowMaintainGlobal.Visibility = canEditGlobalRegion ? Visibility.Visible : Visibility.Collapsed;
            BorderShowMaintainLocal.Visibility = canEditLocalRegion ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Public Methods.

        public void EditCategoryCallBack(QueryLogConfigService.LogCategoryBody logCategory)
        {
            var oldCategory = (QueryLogConfigService.LogCategoryBody)m_selectedRadioButton.DataContext;
            if (oldCategory != null && logCategory != null)
            {
                oldCategory = UtilityHelper.DeepClone(logCategory);
                m_selectedRadioButton.DataContext = oldCategory;
                m_selectedRadioButton.IsChecked = true;
                var collection = (ObservableCollection<QueryLogConfigService.LogCategoryBody>)DataGridLogCategory.ItemsSource;

                foreach (var item in collection)
                {
                    if (item.CategoryName == oldCategory.CategoryName)
                    {
                        item.Status = oldCategory.Status;
                    }
                }

                DataGridLogCategory.ItemsSource = collection;
            }
        }

        public void NewCategoryCallBack(QueryLogConfigService.LogCategoryBody logCategory)
        {
            var collection = (ObservableCollection<QueryLogConfigService.LogCategoryBody>)DataGridLogCategory.ItemsSource ??
                             new ObservableCollection<QueryLogConfigService.LogCategoryBody>();

            logCategory.IsChecked = true;
            collection.Insert(0, logCategory);
            DataGridLogCategory.ItemsSource = collection;
        }

        public void UpdateGlobalRegion()
        {
            m_isUpdateLocalOrGlobal = true;
            m_criteria = UtilityHelper.DeepClone(GridSearchArea.DataContext as CategoryQueryModel);
            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());
        }

        public void UpdateLocalRegion()
        {
            m_isUpdateLocalOrGlobal = true;
            m_criteria = UtilityHelper.DeepClone(GridSearchArea.DataContext as CategoryQueryModel);
            m_queryLogConfigClient.GetLocalRegionAsync(new LogLocalQueryCriteria
                                                           {
                                                               GlobalID = m_criteria != null ? m_criteria.GlobalID : null
                                                           });
        }

        #endregion
    }
}
