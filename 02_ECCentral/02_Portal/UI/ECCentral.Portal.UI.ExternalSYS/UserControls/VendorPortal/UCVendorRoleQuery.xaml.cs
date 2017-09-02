using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UCVendorRoleQuery : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorRoleQueryFilter m_query;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCVendorRoleQuery(int? roleSysNo)
        {
            InitializeComponent();

            BindComboBoxData();

            SeachBuilder.DataContext = m_query = new VendorRoleQueryFilter();

            m_query.RoleSysNo = roleSysNo;
            if (m_query.RoleSysNo != null && m_query.RoleSysNo != 0)
            {
                this.txtRoleSysNo.Text = m_query.RoleSysNo.Value.ToString();
                this.QueryResultGrid.Bind();
            }
        }

        private void BindComboBoxData()
        {
            this.cmbRoleStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbRoleStatus.SelectedIndex = 0;
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            m_query.PagingInfo.SortBy = e.SortField;

            (new VendorFacade()).QueryVendorRole(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                QueryResultGrid.ItemsSource = args.Result.Rows;
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.SelectedIndex < 0 || this.QueryResultGrid.ItemsSource == null)
            {
                CurrentWindow.Alert(ResVendorInfo.Msg_SelectRole, MessageType.Error);
                return;
            }
            DynamicXml selectedManufacturer = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != selectedManufacturer)
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.ResultArgs.Data = selectedManufacturer;
                this.Dialog.Close(true);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }
    }
}
