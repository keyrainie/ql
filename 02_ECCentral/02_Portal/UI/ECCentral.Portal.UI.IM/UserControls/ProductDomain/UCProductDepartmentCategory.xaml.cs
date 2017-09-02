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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Facades;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDepartmentCategory : UserControl
    {
        public IDialog Dialog { get; set; }

        public ProductDepartmentCategoryVM VM { get; private set; }        

        public UCProductDepartmentCategory()
        {
            InitializeComponent();
        }

        public UCProductDepartmentCategory(ProductDepartmentCategoryVM vm)
            : this()
        {
            this.VM = vm;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                //if (this.VM.C2SysNo == null)
                //{
                //    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择类别!", MessageType.Warning);
                //    return;
                //}
                //if (this.VM.PMSysNo == null)
                //{
                //    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择PM!", MessageType.Warning);
                //    return;
                //}
                var facade = new ProductDomainFacade(CPApplication.Current.CurrentPage);
                if (this.VM.SysNo > 0)
                {
                    facade.UpdateDomainCategory(this.VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CloseDialog();
                        
                    });               
                }
                else
                {
                    facade.CreateDomainCategory(this.VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        this.VM.SysNo = args.Result.SysNo;
                        CloseDialog();
                    });               
                }
               
            }
        }

        private void CloseDialog()
        {
            this.VM.PMName = this.ucPMPicker.SelectedPMName;
            this.VM.C2Name = this.ucCategoryPicker.Category2Name;

            if (!string.IsNullOrEmpty(this.ucPMPicker.BackupUserList))
            {
                new ProductDomainFacade(CPApplication.Current.CurrentPage).GetUserListName(this.ucPMPicker.BackupUserList, (o, a) =>
                {
                    this.VM.BackupUserList = a.Result;

                    this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
                    this.Dialog.Close();
                });
            }
            else
            {
                this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
                this.Dialog.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void ucPMPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.VM.PMSysNo > 0 && !string.IsNullOrEmpty(this.ucPMPicker.BackupUserList))
            {
                new ProductDomainFacade(CPApplication.Current.CurrentPage).GetUserListName(this.ucPMPicker.BackupUserList, (obj, args) =>
                {
                    this.VM.BackupUserList = args.Result;
                });
            }
            else
            {
                this.VM.BackupUserList = string.Empty;
            }
        }

        private void ucCategoryPicker_LoadCategoryCompleted(object sender, EventArgs e)
        {
            this.DataContext = this.VM;
        }
    }
}
