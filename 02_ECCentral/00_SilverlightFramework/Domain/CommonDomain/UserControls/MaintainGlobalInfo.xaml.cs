using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.CommonDomain.Views;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class MaintainGlobalInfo
    {
        #region Fields

        private readonly MaintainLogCategory m_mainLogCategory;
        private readonly LogCategoryConfig m_logCategoryConfig;
        private readonly QueryLogConfigClient m_queryLogConfigClient;
        private readonly MaintainLogConfigClient m_maintainLogConfigClient;
        private RadioButton m_selectRadioButton;

        #endregion

        #region Properties.

        public IDialog Dialog { get; set; }

        #endregion

        #region Constr.

        public MaintainGlobalInfo()
        {
            InitializeComponent();
        }

        public MaintainGlobalInfo(LogCategoryConfig logCategoryconfig, MaintainLogCategory mainLogCategory)
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MaintainGlobalInfo_Loaded);

            m_logCategoryConfig = logCategoryconfig;
            m_mainLogCategory = mainLogCategory;

            
            m_maintainLogConfigClient = new MaintainLogConfigClient(m_logCategoryConfig);
            m_maintainLogConfigClient.CreateGlobalRegionCompleted += MaintainLogConfigClientCreateGlobalRegionCompleted;
            m_maintainLogConfigClient.EditGlobalRegionCompleted += MMaintainLogConfigClientEditGlobalRegionCompleted;

            m_queryLogConfigClient = new QueryLogConfigClient(m_logCategoryConfig);
            m_queryLogConfigClient.GetGlobalRegionCompleted += QueryLogConfigClientGetGlobalRegionCompleted;
            
            MaintainArea.DataContext = new MaintainLogConfigService.LogGlobalRegionBody();
            DataGridGlobalInfos.LoadingDataSource += new System.EventHandler<Silverlight.Controls.Data.LoadingDataEventArgs>(DataGridGlobalInfos_LoadingDataSource);
        }

        #endregion

        #region Call Back.

        void MMaintainLogConfigClientEditGlobalRegionCompleted(object sender, EditGlobalRegionCompletedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            ButtonNew.IsEnabled = true;

            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            if (m_selectRadioButton != null)
            {
                var bindingObj = (QueryLogConfigService.LogGlobalRegionBody)m_selectRadioButton.DataContext;

                bindingObj.GlobalName = e.Result.Body.GlobalName;
                bindingObj.Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status);

                DataGridGlobalInfos.SelectedItem = bindingObj;
            }

            m_mainLogCategory.UpdateGlobalRegion();
            m_logCategoryConfig.UpdateGlobalRegion();
            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);
        }

        void MaintainLogConfigClientCreateGlobalRegionCompleted(object sender, CreateGlobalRegionCompletedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            ButtonNew.IsEnabled = true;

            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            var global = (ObservableCollection<QueryLogConfigService.LogGlobalRegionBody>)DataGridGlobalInfos.ItemsSource ??
                         new ObservableCollection<QueryLogConfigService.LogGlobalRegionBody>();

            var insert = new QueryLogConfigService.LogGlobalRegionBody
                             {
                                 GlobalID = e.Result.Body.GlobalID,
                                 GlobalName = e.Result.Body.GlobalName,
                                 Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status),
                                 IsChecked = true
                             };

            global.Insert(0, insert);
            DataGridGlobalInfos.ItemsSource = global;
            DataGridGlobalInfos.SelectedItem = insert;

            m_mainLogCategory.UpdateGlobalRegion();
            m_logCategoryConfig.UpdateGlobalRegion();
            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);
        }

        void QueryLogConfigClientGetGlobalRegionCompleted(object sender, GetGlobalRegionCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            if (e.Result != null)
            {
                var collection = new ObservableCollection<QueryLogConfigService.LogGlobalRegionBody>();
                foreach (var item in e.Result.ResultList)
                {
                    collection.Add(item);
                }
                DataGridGlobalInfos.ItemsSource = collection;
            }

            this.ButtonNew.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        #endregion

        #region Event.

        void MaintainGlobalInfo_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridGlobalInfos.Bind();
        }

        void DataGridGlobalInfos_LoadingDataSource(object sender, Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());
        }

        private void RadioButtonDataGridChecked(object sender, RoutedEventArgs e)
        {
            TextBoxGlobalName.IsEnabled = true;
            ComboBoxStatus.IsEnabled = true;
            ButtonSave.IsEnabled = true;

            var radioButton = sender as RadioButton;

            if (radioButton != null)
                // ReSharper disable PossibleInvalidOperationException
                if (radioButton.IsChecked.Value || radioButton.DataContext != null)
                // ReSharper restore PossibleInvalidOperationException
                {
                    m_selectRadioButton = radioButton;

                    var globalRegion = (QueryLogConfigService.LogGlobalRegionBody)m_selectRadioButton.DataContext;

                    MaintainArea.DataContext = new MaintainLogConfigService.LogGlobalRegionBody
                                                   {
                                                       GlobalID = globalRegion.GlobalID,
                                                       GlobalName = globalRegion.GlobalName,
                                                       Status =
                                                           ContractConvert.ConvertFromQueryToMaintain(
                                                               globalRegion.Status)
                                                   };
                }
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            MaintainArea.DataContext = new MaintainLogConfigService.LogGlobalRegionBody();

            ButtonSave.IsEnabled = true;
            TextBoxGlobalName.IsEnabled = true;
            ComboBoxStatus.IsEnabled = true;

            if (m_selectRadioButton != null)
            {
                m_selectRadioButton.IsChecked = false;
                m_selectRadioButton = null;
            }

            DataGridGlobalInfos.SelectedItem = null;
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(MaintainArea))
            {
                //MaintainLogConfigService.LogGlobalRegionBody global = (MaintainLogConfigService.LogGlobalRegionBody)this.MaintainArea.DataContext;

                if (m_selectRadioButton != null)
                {
                    m_maintainLogConfigClient.EditGlobalRegionAsync(new LogGlobalRegionContract
                        {
                            //Body = new MaintainLogConfigService.LogGlobalRegionBody()
                            //{
                            //    GlobalID = global.GlobalID,
                            //    GlobalName = global.GlobalName,
                            //    Status = global.Status
                            //}
                            Body = (MaintainLogConfigService.LogGlobalRegionBody)MaintainArea.DataContext
                        });
                }
                else
                {
                    m_maintainLogConfigClient.CreateGlobalRegionAsync(new LogGlobalRegionContract
                                                                          {
                                                                              //Body = new MaintainLogConfigService.LogGlobalRegionBody()
                                                                              //{
                                                                              //    GlobalID = global.GlobalID,
                                                                              //    GlobalName = global.GlobalName,
                                                                              //    Status = global.Status
                                                                              //}
                                                                              Body = (MaintainLogConfigService.LogGlobalRegionBody)MaintainArea.DataContext
                                                                          });
                }
                ButtonSave.IsEnabled = false;
                ButtonNew.IsEnabled = false;
            }
        }

        #endregion
    }
}
