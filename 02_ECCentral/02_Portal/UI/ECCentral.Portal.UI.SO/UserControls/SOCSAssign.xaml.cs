using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOCSAssign : UserControl
    {
        public IDialog Dialog { get; set; }

        public SOCSAssign()
        {
            InitializeComponent();
            BindData();
        }

        private void BindData()
        {
            //处理人
            (new CommonDataFacade(CPApplication.Current.CurrentPage)).GetUserInfoByDepartmentId(101, (o, p) =>
            {
                if (p.FaultsHandle()) return;

                //添加未分配
                p.Result.Insert(0, new UserInfo()
                {
                    SysNo = 0,
                    UserDisplayName = ResSO.Expander_NoAssign
                });
                this.cmbOperator.ItemsSource = p.Result;
                this.cmbOperator.SelectedIndex = 0;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                //可能数据库没有数据
                if (this.cmbOperator.SelectedValue != null)
                {
                    Dialog.ResultArgs.Data = this.cmbOperator.SelectedValue;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                }
                Dialog.Close();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }
    }
}
