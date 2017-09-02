using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.CommonDomain.Views;
using Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using LogLocalRegionBody = Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService.LogLocalRegionBody;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class MaintainLogCategory
    {
        #region Fields

        private readonly LogCategoryConfig m_logCategoryConfig;
        private readonly MaintainLogConfigClient m_maintainLogConfigClient;
        private readonly QueryLogConfigClient m_queryLogConfigClient;
        private string m_selectedLocalId;
        private string m_selectedGlobalId;
        private bool m_isEdit;

        #endregion

        #region Constr.

        public MaintainLogCategory()
        {
            InitializeComponent();
        }

        public MaintainLogCategory(LogCategoryConfig logCategoryConfig)
        {
            InitializeComponent();

            m_logCategoryConfig = logCategoryConfig;
            m_maintainLogConfigClient = new MaintainLogConfigClient(m_logCategoryConfig);
            m_queryLogConfigClient = new QueryLogConfigClient(m_logCategoryConfig);

            m_maintainLogConfigClient.EditLogCategoryCompleted += MaintainLogConfigClientEditLogCategoryCompleted;
            m_maintainLogConfigClient.CreateLogCategoryCompleted += MaintainLogConfigClientCreateLogCategoryCompleted;

            m_queryLogConfigClient.GetGlobalRegionCompleted += QueryLogConfigClientGetGlobalRegionCompleted;
            m_queryLogConfigClient.GetLocalRegionCompleted += QueryLogConfigClientGetLocalRegionCompleted;

            ResetDataContext();
            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());
            m_isEdit = true;
        }

        #endregion

        #region Call Back.

        void QueryLogConfigClientGetLocalRegionCompleted(object sender, GetLocalRegionCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }

            var result = m_isEdit ? e.Result.ResultList : FilterInActiveLocalRegion(e.Result.ResultList);

            ComboBoxLocal.ItemsSource = result;
            if (m_selectedLocalId != null)
            {
                ComboBoxLocal.SelectedValue = m_selectedLocalId;
            }
            else
            {
                ComboBoxLocal.SelectedIndex = 0;
            }
        }

        void QueryLogConfigClientGetGlobalRegionCompleted(object sender, GetGlobalRegionCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }

            //var result = m_isEdit ? e.Result.ResultList : FilterInActiveGlobalRegion(e.Result.ResultList);

            var result = e.Result.ResultList;

            result.Insert(0, new QueryLogConfigService.LogGlobalRegionBody
                                 {
                                     GlobalName = CommonResource.ComboBox_ExtraSelectText,
                                     Status = QueryLogConfigService.Status.Active
                                 });

            ComboBoxGlobal.ItemsSource = result;
            if (m_selectedGlobalId != null)
            {
                ComboBoxGlobal.SelectedValue = m_selectedGlobalId;
            }
            else
            {
                ComboBoxGlobal.SelectedIndex = 0;
            }
        }

        void MaintainLogConfigClientCreateLogCategoryCompleted(object sender, CreateLogCategoryCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            var queryCategory = new QueryLogConfigService.LogCategoryBody
                                                                      {
                                                                          CategoryDescription = e.Result.Body.CategoryDescription,
                                                                          CategoryName = e.Result.Body.CategoryName,
                                                                          EnableRemoveLog = e.Result.Body.EnableRemoveLog,
                                                                          GlobalID = e.Result.Body.GlobalID,
                                                                          GlobalName = e.Result.Body.GlobalName,
                                                                          InDate = e.Result.Body.InDate,
                                                                          InUser = e.Result.Body.InUser,
                                                                          LocalID = e.Result.Body.LocalID,
                                                                          LocalName = e.Result.Body.LocalName,
                                                                          LogType = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.LogType),
                                                                          RemoveOverDays = e.Result.Body.RemoveOverDays,
                                                                          Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status)
                                                                      };

            if (e.Result.Body.EmailNotification != null)
            {
                e.Result.Body.MyInterval = (e.Result.Body.EmailNotification.Interval / 60).ToString();
                queryCategory.EmailNotification = new QueryLogConfigService.EmailNotificationConfig
                                                      {
                                                          Address = e.Result.Body.EmailNotification.Address,
                                                          FilterDuplicate = e.Result.Body.EmailNotification.FilterDuplicate,
                                                          Interval = int.Parse(e.Result.Body.MyInterval) * 60,
                                                          NeedNotify = e.Result.Body.EmailNotification.NeedNotify,
                                                          Subject = e.Result.Body.EmailNotification.Subject
                                                      };
            }
            m_logCategoryConfig.NewCategoryCallBack(queryCategory);
            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);
        }

        void MaintainLogConfigClientEditLogCategoryCompleted(object sender, EditLogCategoryCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            var queryCategory = new QueryLogConfigService.LogCategoryBody
                                                                      {
                                                                          CategoryDescription = e.Result.Body.CategoryDescription,
                                                                          CategoryName = e.Result.Body.CategoryName,
                                                                          EnableRemoveLog = e.Result.Body.EnableRemoveLog,
                                                                          GlobalID = e.Result.Body.GlobalID,
                                                                          GlobalName = e.Result.Body.GlobalName,
                                                                          InDate = e.Result.Body.InDate,
                                                                          InUser = e.Result.Body.InUser,
                                                                          LocalID = e.Result.Body.LocalID,
                                                                          LocalName = e.Result.Body.LocalName,
                                                                          LogType = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.LogType),
                                                                          RemoveOverDays = e.Result.Body.RemoveOverDays,
                                                                          Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status)
                                                                      };

            if (e.Result.Body.EmailNotification != null)
            {
                e.Result.Body.MyInterval = (e.Result.Body.EmailNotification.Interval / 60).ToString();
                queryCategory.EmailNotification = new QueryLogConfigService.EmailNotificationConfig
                                                      {
                                                          Address = e.Result.Body.EmailNotification.Address,
                                                          FilterDuplicate = e.Result.Body.EmailNotification.FilterDuplicate,
                                                          Interval = int.Parse(e.Result.Body.MyInterval) * 60,
                                                          NeedNotify = e.Result.Body.EmailNotification.NeedNotify,
                                                          Subject = e.Result.Body.EmailNotification.Subject
                                                      };
            }
            m_logCategoryConfig.EditCategoryCallBack(queryCategory);
            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);
        }

        #endregion

        #region Private Methods & Events

        private static ObservableCollection<LogLocalRegionBody> FilterInActiveLocalRegion(IEnumerable<LogLocalRegionBody> filterValue)
        {
            IEnumerable<LogLocalRegionBody> linqResult = from l in filterValue
                                                         where l.Status == QueryLogConfigService.Status.Active
                                                         select l;

            var result = new ObservableCollection<LogLocalRegionBody>(linqResult);

            result.Insert(0, new LogLocalRegionBody
                                 {
                                     LocalName = CommonResource.ComboBox_ExtraSelectText,
                                     Status = QueryLogConfigService.Status.Active
                                 });

            return result;
        }

        private void ComboBoxGlobalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxGlobal.SelectedValue != null)
            {
                m_selectedGlobalId = ComboBoxGlobal.SelectedValue.ToString();
                m_queryLogConfigClient.GetLocalRegionAsync(new LogLocalQueryCriteria
                                                               {
                                                                   GlobalID = ComboBoxGlobal.SelectedValue.ToString(),
                                                                   LocalRegionStatus =
                                                                       QueryLogConfigService.Status.Active
                                                               });
            }
            else
            {
                var list = new ObservableCollection<LogLocalRegionBody>();

                list.Insert(0, new LogLocalRegionBody
                                   {
                                       LocalName = CommonResource.ComboBox_ExtraSelectText,
                                       Status = QueryLogConfigService.Status.Active
                                   });

                ComboBoxLocal.ItemsSource = list;
            }

        }

        private void CheckBoxEnableEmailClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxEnableEmail.IsChecked != null)
            {
                if (CheckBoxEnableEmail.IsChecked.Value &&
                    MaintainArea.DataContext != null &&
                    ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).EmailNotification == null)
                {
                    ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).EmailNotification = new MaintainLogConfigService
                        .EmailNotificationConfig
                                                                                                                  {
                                                                                                                      NeedNotify
                                                                                                                          =
                                                                                                                          true,
                                                                                                                  };
                }
                if (CheckBoxEnableEmail.IsChecked.Value)
                {
                    TextBoxEmailAddress.IsEnabled = true;
                    TextBoxInterval.IsEnabled = true;
                    TextBoxSubject.IsEnabled = true;
                    CheckBoxInstant.IsEnabled = true;
                    CheckBoxDuplicate.IsEnabled = true;
                }
                else
                {
                    TextBoxEmailAddress.IsEnabled = false;
                    TextBoxInterval.IsEnabled = false;
                    TextBoxSubject.IsEnabled = false;
                    CheckBoxInstant.IsEnabled = false;
                    CheckBoxDuplicate.IsEnabled = false;
                    ValidationManager.Validate(MaintainArea);
                }
            }
        }

        private void CheckBoxInstantClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxInstant.IsChecked != null && !CheckBoxInstant.IsChecked.Value)
            {
                ShowImmediatelyConfig(Visibility.Visible);
                CheckBoxDuplicate.IsChecked = false;
                TextBoxInterval.Text = "1";
                ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).MyInterval = "1";
            }
            else
            {
                ShowImmediatelyConfig(Visibility.Collapsed);
                ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).IsInstant = true;
                ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).MyInterval = "0";
                if (((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).EmailNotification != null)
                {
                    ((MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext).EmailNotification.Interval = 0;
                }
                CheckBoxDuplicate.IsChecked = false;
            }
        }

        private void CheckBoxEnableRemovelogClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxEnableRemovelog.IsChecked != null)
            {
                if (CheckBoxEnableRemovelog.IsChecked.Value)
                {
                    TextBoxRemoveOverDays.IsEnabled = true;
                }
                else
                {
                    TextBoxRemoveOverDays.IsEnabled = false;
                    ValidationManager.Validate(MaintainArea);
                }
            }
        }

        private void ShowImmediatelyConfig(Visibility show)
        {
            TextBlockInstant.Visibility = show;
            TextBoxInterval.Visibility = show;
            TextBlockTimeTip.Visibility = show;
            CheckBoxDuplicate.Visibility = show;
        }

        private void SetStatusOfMailNotification(bool enable)
        {
            TextBoxEmailAddress.IsEnabled = enable;
            TextBoxInterval.IsEnabled = enable;
            TextBoxSubject.IsEnabled = enable;
            CheckBoxInstant.IsEnabled = enable;
            CheckBoxDuplicate.IsEnabled = enable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 重置DataContext
        /// </summary>
        public void ResetDataContext()
        {
            var globalList = ComboBoxGlobal.ItemsSource as ObservableCollection<QueryLogConfigService.LogGlobalRegionBody>;

            if (globalList != null && globalList.Count != 0 && globalList[0].GlobalID != null)
            {
                globalList.Insert(0, new QueryLogConfigService.LogGlobalRegionBody
                                         {
                                             GlobalName = CommonResource.ComboBox_ExtraSelectText,
                                             Status = QueryLogConfigService.Status.Active
                                         });

                ComboBoxGlobal.ItemsSource = globalList;
            }

            MaintainArea.DataContext = new MaintainLogConfigService.LogCategoryBody
                                           {
                                               EnableRemoveLog = false,
                                               IsInstant = true
                                           };

            IsEnabled = true;
            m_isEdit = false;

            TextBoxRemoveOverDays.IsEnabled = false;
            ComboBoxLocal.IsEnabled = true;
            ComboBoxGlobal.IsEnabled = true;
            TextBoxCategory.IsEnabled = true;
            SetStatusOfMailNotification(false);
            ShowImmediatelyConfig(Visibility.Collapsed);
        }

        /// <summary>
        /// 绑定LogCategory的详细信息并设置相应状态
        /// </summary>
        /// <param name="entity"></param>
        public void BindDataContext(MaintainLogConfigService.LogCategoryBody entity)
        {
            ComboBoxLocal.IsEnabled = false;
            ComboBoxGlobal.IsEnabled = false;
            TextBoxCategory.IsEnabled = false;

            if (ComboBoxGlobal.Items.Count > 0 && ComboBoxLocal.Items.Count > 0)
            {
                ComboBoxGlobal.SelectedIndex = 0;
                ComboBoxLocal.SelectedIndex = 0;
            }

            if (entity.EmailNotification != null)
            {
                //当延时大于0的时候，设置立即发送为false.
                if (entity.EmailNotification.Interval > 0)
                {
                    entity.IsInstant = false;
                    entity.MyInterval = (entity.EmailNotification.Interval / 60).ToString();
                    ShowImmediatelyConfig(Visibility.Visible);
                }
                else
                {
                    entity.IsInstant = true;
                    ShowImmediatelyConfig(Visibility.Collapsed);
                }

                //当需要邮件通知的时候，设置相应的控件状态和初始值
                if (entity.EmailNotification.NeedNotify)
                {
                    SetStatusOfMailNotification(true);
                }
                else
                {
                    SetStatusOfMailNotification(false);
                    //entity.IsInstant = false;
                }
            }

            TextBoxRemoveOverDays.IsEnabled = entity.EnableRemoveLog;


            MaintainArea.DataContext = entity;
            m_selectedLocalId = entity.LocalID;
            m_selectedGlobalId = entity.GlobalID;
            m_isEdit = true;
        }

        /// <summary>
        /// 持久化LogCategory数据
        /// </summary>
        public void PersistLogCategory()
        {
            if (ValidationManager.Validate(MaintainArea))
            {
                var category =
                    (MaintainLogConfigService.LogCategoryBody)MaintainArea.DataContext;
                if (category.EmailNotification != null && category.EmailNotification.NeedNotify)
                {
                    category.EmailNotification.Interval = category.EmailNotification.Interval * 60;
                }
                else if (category.EmailNotification == null)
                {
                    category.EmailNotification = new MaintainLogConfigService.EmailNotificationConfig();
                }


                if (m_isEdit)
                {
                    m_maintainLogConfigClient.EditLogCategoryAsync(new LogCategoryContract
                                                                       {
                                                                           Body = category
                                                                       });
                }
                else
                {
                    category.InUser = CPApplication.Current.LoginUser.LoginName;
                    m_maintainLogConfigClient.CreateLogCategoryAsync(new LogCategoryContract
                                                                         {
                                                                             Body = category
                                                                         });
                }
            }
        }

        /// <summary>
        /// 更新Global Regions
        /// </summary>
        public void UpdateGlobalRegion()
        {
            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());
        }

        /// <summary>
        /// 更新Local Regions
        /// </summary>
        public void UpdateLocalRegion()
        {
            if (ComboBoxGlobal.SelectedValue != null)
            {
                if (ComboBoxLocal.SelectedValue != null)
                {
                    m_selectedLocalId = ComboBoxLocal.SelectedValue.ToString();
                }
                m_queryLogConfigClient.GetLocalRegionAsync(new LogLocalQueryCriteria
                                                               {
                                                                   GlobalID = m_selectedGlobalId,
                                                                   LocalRegionStatus = QueryLogConfigService.Status.Active
                                                               });
            }
            else
            {
                var list = new ObservableCollection<LogLocalRegionBody>();

                list.Insert(0, new LogLocalRegionBody
                                   {
                                       LocalName = CommonResource.ComboBox_ExtraSelectText,
                                       Status = QueryLogConfigService.Status.Active
                                   });

                ComboBoxLocal.ItemsSource = list;
            }
        }

        #endregion
    }
}
