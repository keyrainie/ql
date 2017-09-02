using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.CommonDomain.ConfigurationService;
using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using System.Windows.Threading;


namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View]
    public partial class SilverlightConfig : PageBase
    {
        private bool m_isNew = true;
        private ConfigurationServiceV40Client m_serviceClient = null;
        private ConfigModel m_selectedItem;

        public ConfigModel SelectedItem
        {
            get
            {
                return m_selectedItem;
            }
            set
            {
                m_selectedItem = value;
                this.MaintainGrid.DataContext = value;
            }
        }

        public SilverlightConfig()
        {
            InitializeComponent();
            this.m_serviceClient = new ConfigurationServiceV40Client(this);
            this.SelectedItem = new ConfigModel();
            this.DataGridConfigList.SelectionChanged += new SelectionChangedEventHandler(DataGridConfigList_SelectionChanged);

        }

        void DataGridConfigList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var datagrid = sender as DataGrid;
            if (datagrid.SelectedItem != null)
            {
                this.m_isNew = false;
                this.btnDelete.IsEnabled = true;
                this.SelectedItem = UtilityHelper.DeepClone(datagrid.SelectedItem as ConfigModel);
            }
            else
            {
                this.SelectedItem = new ConfigModel();
                this.m_isNew = true;
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            Window.DocumentHorizontalScrollBar = ScrollBarVisibility.Disabled;
            Window.DocumentVerticalScrollBar = ScrollBarVisibility.Disabled;

            m_serviceClient.GetApplicationConfigCompleted += new EventHandler<GetApplicationConfigCompletedEventArgs>(serviceClient_GetApplicationConfigCompleted);
            m_serviceClient.CreateConfigCompleted += new EventHandler<CreateConfigCompletedEventArgs>(serviceClient_CreateConfigCompleted);
            m_serviceClient.EditConfigCompleted += new EventHandler<EditConfigCompletedEventArgs>(serviceClient_EditConfigCompleted);
            m_serviceClient.DeleteConfigCompleted += new EventHandler<DeleteConfigCompletedEventArgs>(serviceClient_DeleteConfigCompleted);

            m_serviceClient.GetApplicationConfigAsync();
        }

        void serviceClient_DeleteConfigCompleted(object sender, DeleteConfigCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            var list = this.DataGridConfigList.ItemsSource as ObservableCollection<ConfigModel>;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ConfigID == e.Result.Body.ConfigID)
                {
                    list.Remove(list[i]);
                    DataGridConfigList.SelectedIndex = -1;
                    break;
                }
            }

            this.DataGridConfigList.ItemsSource = list;
            this.m_isNew = true;
            btnNew_Click(null, null);

            Window.MessageBox.Show(CommonResource.Info_DeleteSuccessfully, MessageBoxType.Success);
        }

        void serviceClient_EditConfigCompleted(object sender, EditConfigCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            var model = e.Result.Body.ConvertTo<ConfigModel>();
            var list = this.DataGridConfigList.ItemsSource as ObservableCollection<ConfigModel>;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ConfigID == model.ConfigID)
                {
                    list[i] = model;
                    break;
                }
            }

            this.DataGridConfigList.ItemsSource = list;
            this.DataGridConfigList.SelectedItem = model;
            SelectedItem = model;

            Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
        }

        void serviceClient_CreateConfigCompleted(object sender, CreateConfigCompletedEventArgs e)
        {
            if (this.Window.FaultHandle.Handle(e))
            {
                return;
            }

            var item = e.Result.Body.ConvertTo<ConfigModel>();

            var list = this.DataGridConfigList.ItemsSource as ObservableCollection<ConfigModel>;

            if (list != null)
            {
                list.Insert(0, item);
            }

            this.DataGridConfigList.ItemsSource = list;
            this.DataGridConfigList.SelectedItem = item;

            SelectedItem = item;
            this.btnDelete.IsEnabled = true;
            this.m_isNew = false;

            Window.MessageBox.Show(CommonResource.Info_CreateSuccessfully, MessageBoxType.Success);
        }

        void serviceClient_GetApplicationConfigCompleted(object sender, GetApplicationConfigCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Window.MessageBox.Show(e.Error.Message, MessageBoxType.Error);
                return;
            }
            if (e.Result != null && e.Result.Count > 0)
            {
                ObservableCollection<ConfigModel> list = new ObservableCollection<ConfigModel>();
                foreach (var item in e.Result)
                {
                    list.Add(item.ConvertTo<ConfigModel>());
                }

                DataGridConfigList.ItemsSource = list;
            }
        }

        private void btnNew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataGridConfigList.SelectedIndex = -1;

            this.m_isNew = true;
            this.btnDelete.IsEnabled = false;
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var msg = new ConfigKeyValueV40();

            msg.Header = new MessageHeader { Language = CPApplication.Current.LanguageCode };
            msg.Body = this.SelectedItem;

            if (!ValidationManager.Validate(this.MaintainGrid))
            {
                return;
            }
            if (this.m_isNew)
            {
                msg.Body.InUser = CPApplication.Current.LoginUser.LoginName;

                m_serviceClient.CreateConfigAsync(msg);
            }
            else
            {
                msg.Body.EditUser = CPApplication.Current.LoginUser.LoginName;
                m_serviceClient.EditConfigAsync(msg);
            }
        }

        private void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Window.Confirm(SilverlightConfigResource.Confirm_DeleteItem, (s, ee) =>
            {
                if (ee.DialogResult == DialogResultType.OK)
                {
                    var msg = new ConfigKeyValueV40();
                    msg.Body = this.SelectedItem;
                    m_serviceClient.DeleteConfigAsync(msg);
                }
            });
        }
    }
}
