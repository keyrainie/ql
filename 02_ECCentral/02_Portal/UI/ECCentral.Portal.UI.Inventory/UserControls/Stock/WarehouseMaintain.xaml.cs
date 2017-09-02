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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class WarehouseMaintain : UserControl
    {
        private WarehouseInfoVM WarehouseVM;
        private WarehouseFacade WarehouseFacade;
        public int? WarehouseSysNo
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseSysNo">为null表示创建仓库，否则表示修改</param>
        public WarehouseMaintain()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(WarehouseMaintain_Loaded);
        }
        void WarehouseMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            WarehouseFacade = new WarehouseFacade(Page);
            //(中蛋定制化 不需要此信息)
            //new WarehouseOwnerQueryFacade().GetWarehouseOwnerByCompanyCode(CPApplication.Current.CompanyCode, (ownerList) =>
            //    {
            //        ownerList.Insert(0, new WarehouseOwnerInfoVM
            //        {
            //            SysNo = null,
            //            OwnerName = ResCommonEnum.Enum_Select
            //        });
            //        WarehouseVM = WarehouseVM ?? new WarehouseInfoVM();
            //        WarehouseVM.OwnerList = ownerList;
            //    });

            WarehouseFacade.GetProductCountryList((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                cmbOrgin.ItemsSource = arg.Result;
                cmbOrgin.SelectedIndex = 0;

                if (WarehouseSysNo.HasValue)
                {
                    WarehouseFacade.GetWarehouseInfo(WarehouseSysNo.Value, (vm) =>
                    {
                        if (vm == null || vm.CompanyCode == null || vm.CompanyCode.Trim() != CPApplication.Current.CompanyCode)
                        {
                            vm = null;
                            Page.Context.Window.Alert("没有找到相应的仓库信息，此仓库信息可能已经被删除。");
                        }
                        //else//(中蛋定制化 不需要此信息)
                        //{
                        //    if (WarehouseVM != null)
                        //    {
                        //        vm.OwnerList = WarehouseVM.OwnerList;
                        //    }
                        //}
                        WarehouseVM = vm ?? new WarehouseInfoVM();
                        IniPageData();
                    });
                }
                else
                {
                    IniPageData();
                }
            });
            Loaded -= new RoutedEventHandler(WarehouseMaintain_Loaded);
        }
        private void IniPageData()
        {
            WarehouseVM = WarehouseVM ?? new WarehouseInfoVM();
            this.DataContext = WarehouseVM;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this);
            if (WarehouseVM.HasValidationErrors)
            {
                return;
            }
            WarehouseVM.CompanyCode = CPApplication.Current.CompanyCode;
            if (WarehouseSysNo.HasValue)
            {
                WarehouseFacade.UpdateWarehouseInfo(WarehouseVM, () =>
                {
                    if (Saved != null)
                    {
                        Saved(sender, e);
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    }
                });
            }
            else
            {
                WarehouseFacade.CreateWarehouseInfo(WarehouseVM, () =>
                {
                    WarehouseSysNo = WarehouseVM.SysNo;
                    if (Saved != null)
                    {
                        Saved(sender, e);
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    }
                });
            }

        }
        public event EventHandler Saved;

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
