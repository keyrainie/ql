using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.SO.Resources;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class ComplainAddRequestNew : UserControl
    {
        public IDialog Dialog { get; set; }

        private IPage Page
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page != null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        SOComplainFacade m_facade;

        public ComplainAddRequestNew()
        {
            InitializeComponent();

            BindComboBoxData();
        }

        private void BindComboBoxData()
        {
            //投诉类别
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_ComplainType, (o, p) =>
            {
                this.cmbComplainType.ItemsSource = p.Result;
                this.cmbComplainType.SelectedIndex = 0;
            });

            //投诉来源
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_SOComplainSourceType, (o, p) =>
            {
                this.cmbComplainSourceType.ItemsSource = p.Result;
                this.cmbComplainSourceType.SelectedIndex = 0;
            });

            this.DataContext = new SOComplaintCotentInfoVM();

            m_facade = new SOComplainFacade();

        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                var vm = this.DataContext as SOComplaintCotentInfoVM;
                //验证输入
                ValidationManager.Validate(this);
                if (vm.ValidationErrors.Count != 0) return;

                //必须输入订单号，随之限定用户
                if (!vm.SOSysNo.HasValue)
                {
                    Window.Alert(ResComplain.Info_Complain_SOSysNoRequired, MessageType.Information);
                    this.txtSOSysNo.Focus();
                    return;
                }

                var newComplain = vm.ConvertVM<SOComplaintCotentInfoVM, SOComplaintCotentInfo>();

                m_facade.Create(newComplain, (cobj, cargs) =>
                {
                    if (cargs.FaultsHandle())
                    {
                        return;
                    }
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                });
            }
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
