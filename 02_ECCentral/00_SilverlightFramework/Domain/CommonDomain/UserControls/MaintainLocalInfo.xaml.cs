using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.CommonDomain.Views;
using Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;


namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class MaintainLocalInfo : INotifyPropertyChanged
    {
        #region Fields.

        private readonly LogCategoryConfig m_logCategoryConfig;
        private readonly MaintainLogCategory m_mainLogCategory;
        private readonly QueryLogConfigClient m_queryLogConfigClient;
        private readonly MaintainLogConfigClient m_maintainLogConfigClient;
        private string m_globalId;
        private RadioButton m_selectedRadioButton;

        #endregion

        #region Properties.

        public IDialog Dialog { get; set; }

        public string GlobalId
        {
            get
            {
                return m_globalId;
            }
            set
            {
                m_globalId = value;
                RaisePropertyChanged("GlobalID");
            }
        }

        #endregion

        #region Constr.

        public MaintainLocalInfo()
        {
            InitializeComponent();
        }

        public MaintainLocalInfo(LogCategoryConfig logCategoryConfig, MaintainLogCategory mainLogCategory)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MaintainLocalInfo_Loaded);

            m_logCategoryConfig = logCategoryConfig;
            m_mainLogCategory = mainLogCategory;

            m_queryLogConfigClient = new QueryLogConfigClient(m_logCategoryConfig);
            m_queryLogConfigClient.GetGlobalRegionCompleted += QueryLogConfigClientGetGlobalRegionCompleted;
            m_queryLogConfigClient.GetLocalRegionCompleted += QueryLogConfigClientGetLocalRegionCompleted;

            m_maintainLogConfigClient = new MaintainLogConfigClient(m_logCategoryConfig);
            m_maintainLogConfigClient.EditLocalRegionCompleted += MaintainLogConfigClientEditLocalRegionCompleted;
            m_maintainLogConfigClient.CreateLocalRegionCompleted += MaintainLogConfigClientCreateLocalRegionCompleted;

            DataGridLocalInfos.LoadingDataSource += new System.EventHandler<Silverlight.Controls.Data.LoadingDataEventArgs>(DataGridLocalInfos_LoadingDataSource);

            MaintainArea.DataContext = new MaintainLogConfigService.LogLocalRegionBody();

        }

        void MaintainLocalInfo_Loaded(object sender, RoutedEventArgs e)
        {
            m_queryLogConfigClient.GetGlobalRegionAsync(new LogGlobalQueryCriteria());

        }

        void DataGridLocalInfos_LoadingDataSource(object sender, Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryLogConfigClient.GetLocalRegionAsync(new LogLocalQueryCriteria
            {
                GlobalID = GlobalId
            });
        }

        #endregion

        #region Call Back.

        void MaintainLogConfigClientCreateLocalRegionCompleted(object sender, CreateLocalRegionCompletedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            ButtonNew.IsEnabled = true;
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            var globals = (ObservableCollection<QueryLogConfigService.LogLocalRegionBody>)DataGridLocalInfos.ItemsSource ??
                          new ObservableCollection<QueryLogConfigService.LogLocalRegionBody>();

            var insertObj = new QueryLogConfigService.LogLocalRegionBody
                                {
                                    GlobalID = e.Result.Body.GlobalID,
                                    GlobalName = e.Result.Body.GlobalName,
                                    LocalID = e.Result.Body.LocalID,
                                    LocalName = e.Result.Body.LocalName,
                                    Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status),
                                    IsChecked = true
                                };
            globals.Insert(0, insertObj);
            DataGridLocalInfos.ItemsSource = globals;
            DataGridLocalInfos.SelectedItem = insertObj;
            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);

            m_logCategoryConfig.UpdateLocalRegion();
            m_mainLogCategory.UpdateLocalRegion();
        }

        void MaintainLogConfigClientEditLocalRegionCompleted(object sender, EditLocalRegionCompletedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            ButtonNew.IsEnabled = true;
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            if (m_selectedRadioButton != null)
            {
                var bindingObj = (QueryLogConfigService.LogLocalRegionBody)m_selectedRadioButton.DataContext;
                if (bindingObj != null)
                {
                    bindingObj.GlobalID = e.Result.Body.GlobalID;
                    bindingObj.GlobalName = e.Result.Body.GlobalName;
                    bindingObj.LocalName = e.Result.Body.LocalName;
                    bindingObj.Status = ContractConvert.ConvertFromMaintainToQuery(e.Result.Body.Status);
                }
                DataGridLocalInfos.SelectedItem = bindingObj;
            }

            m_logCategoryConfig.Window.MessageBox.Show(LogCategoryConfigResource.LogCategoryConfig_SaveSuccessfully, MessageBoxType.Success);

            m_logCategoryConfig.UpdateLocalRegion();
            m_mainLogCategory.UpdateLocalRegion();
        }

        void QueryLogConfigClientGetLocalRegionCompleted(object sender, GetLocalRegionCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            var observableCollection = new ObservableCollection<QueryLogConfigService.LogLocalRegionBody>();
            foreach (var item in e.Result.ResultList)
            {
                observableCollection.Add(item);
            }
            DataGridLocalInfos.ItemsSource = observableCollection;

            this.ButtonNew.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        void QueryLogConfigClientGetGlobalRegionCompleted(object sender, GetGlobalRegionCompletedEventArgs e)
        {
            if (m_logCategoryConfig.Window.FaultHandle.Handle(e))
            {
                return;
            }
            ComboBoxGlobal.ItemsSource = e.Result.ResultList;
            if (e.Result.ResultList != null && e.Result.ResultList.Count() > 0)
            {
                GlobalId = e.Result.ResultList[0].GlobalID;

                ComboBoxGlobal.SelectedValue = GlobalId;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Event.

        private void ComboBoxGlobalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalId = ComboBoxGlobal.SelectedValue.ToString();
            MaintainArea.DataContext = new MaintainLogConfigService.LogLocalRegionBody();
            TextBoxLocalName.IsEnabled = false;
            ComboBoxLocalStatus.IsEnabled = false;
            ButtonSave.IsEnabled = false;

            DataGridLocalInfos.Bind();
        }

        private void RadioButtonDataGridChecked(object sender, RoutedEventArgs e)
        {
            TextBoxLocalName.IsEnabled = true;
            ComboBoxLocalStatus.IsEnabled = true;
            ButtonSave.IsEnabled = true;
            var radioButton = (RadioButton)sender;

            if (radioButton.IsChecked != null)
                if (radioButton.IsChecked.Value || radioButton.DataContext != null)
                {
                    m_selectedRadioButton = radioButton;
                    var localRegion = (QueryLogConfigService.LogLocalRegionBody)m_selectedRadioButton.DataContext;
                    MaintainArea.DataContext = new MaintainLogConfigService.LogLocalRegionBody
                                                   {
                                                       GlobalID = localRegion.GlobalID,
                                                       GlobalName = localRegion.GlobalName,
                                                       LocalID = localRegion.LocalID,
                                                       LocalName = localRegion.LocalName,
                                                       Status = ContractConvert.ConvertFromQueryToMaintain(localRegion.Status)
                                                   };
                }
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            MaintainArea.DataContext = new MaintainLogConfigService.LogLocalRegionBody();

            TextBoxLocalName.IsEnabled = true;
            ComboBoxLocalStatus.IsEnabled = true;
            ButtonSave.IsEnabled = true;

            if (m_selectedRadioButton != null)
            {
                m_selectedRadioButton.IsChecked = false;
                m_selectedRadioButton = null;
            }

            DataGridLocalInfos.SelectedItem = null;
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(MaintainArea))
            {
                var local = (MaintainLogConfigService.LogLocalRegionBody)MaintainArea.DataContext;

                if (m_selectedRadioButton != null)
                {
                    m_maintainLogConfigClient.EditLocalRegionAsync(new LogLocalRegionContract
                                                                       {
                                                                           Body = new MaintainLogConfigService.LogLocalRegionBody
                                                                                      {
                                                                                          GlobalID = GlobalId,
                                                                                          GlobalName = local.GlobalName,
                                                                                          LocalID = local.LocalID,
                                                                                          LocalName = local.LocalName,
                                                                                          Status = local.Status
                                                                                      }
                                                                       });
                }
                else
                {
                    m_maintainLogConfigClient.CreateLocalRegionAsync(new LogLocalRegionContract
                                                                         {
                                                                             Body = new MaintainLogConfigService.LogLocalRegionBody
                                                                                        {
                                                                                            GlobalID = GlobalId,
                                                                                            GlobalName = local.GlobalName,
                                                                                            LocalID = local.LocalID,
                                                                                            LocalName = local.LocalName,
                                                                                            Status = local.Status
                                                                                        }
                                                                         });
                }
                ButtonSave.IsEnabled = false;
                ButtonNew.IsEnabled = false;
            }
        }

        #endregion
    }
}
