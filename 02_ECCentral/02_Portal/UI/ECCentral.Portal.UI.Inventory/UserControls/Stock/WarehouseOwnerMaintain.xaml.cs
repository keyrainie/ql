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
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Inventory.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class WarehouseOwnerMaintain : UserControl
    {        
        public int? OwnerSysNo
        {
            get;
            set;
        }
        public IPage Page
        {
            get;
            set;
        }
        public IDialog Dialog
        { get; set; }
        public WarehouseOwnerInfoVM OwnerVM;
        WarehouseOwnerFacade OwnerFacade;

        public WarehouseOwnerMaintain()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(WarehouseOwnerMaintain_Loaded);
        }


        void WarehouseOwnerMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            OwnerFacade = new WarehouseOwnerFacade(Page);

            if (OwnerSysNo.HasValue)
            {
                OwnerFacade.GetWarehouseOwnerInfo(OwnerSysNo.Value, (vm) =>
                {
                    if (vm == null || vm.CompanyCode == null || vm.CompanyCode.Trim() != CPApplication.Current.CompanyCode)
                    {
                        vm = null;
                        Page.Context.Window.Alert("没有找到相应的仓库所有者信息，此仓库所有者信息可能已经被删除。");
                    }                   
                    OwnerVM = vm ?? new WarehouseOwnerInfoVM();
                    IniPageData();
                });
            }
            else
            {
                IniPageData();
            }

            Loaded -= new RoutedEventHandler(WarehouseOwnerMaintain_Loaded);
        }


        private void IniPageData()
        {
            OwnerVM = OwnerVM ?? new WarehouseOwnerInfoVM();
            this.DataContext = OwnerVM;           
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this))
            {
                OwnerVM.CompanyCode = CPApplication.Current.CompanyCode;
                if (OwnerSysNo.HasValue)
                {
                    OwnerFacade.UpdateWarehouseOwnerInfo(OwnerVM, () =>
                    {
                        if (Saved != null)
                        {
                            Saved(sender, e);
                        }                        
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    });

                }
                else
                {
                    OwnerFacade.CreateWarehouseOwnerInfo(OwnerVM, () =>
                    {
                        OwnerSysNo = OwnerVM.SysNo;
                        if (Saved != null)
                        {
                            Saved(sender, e);
                        }                        
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    });
                }
            }
        }

        public event EventHandler Saved;

        private void ShowMessage(string message)
        {
            Page.Context.Window.Alert(message, MessageType.Warning);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods
    }
}
