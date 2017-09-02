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
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class StockMaintain : UserControl
    {
        public int? StockSysNo
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
        public StockInfoVM StockVM;
        StockFacade StockFacade;
        public StockMaintain()
        {
            InitializeComponent();
            StockVM = new StockInfoVM();
            Loaded += new RoutedEventHandler(StockMaintain_Loaded);
        }

        void StockMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            StockFacade = new Facades.StockFacade(Page);
            WarehouseQueryFacade whFacade = new WarehouseQueryFacade(Page);
            whFacade.GetWarehouseListByCompanyCode(CPApplication.Current.CompanyCode, (vmList) =>
            {
                //WarehouseInfoVM blankInfo = new WarehouseInfoVM()
                //{
                //    SysNo = null,
                //    WarehouseID = null,
                //    WarehouseName = ResCommonEnum.Enum_Select
                //};
                //vmList.Insert(0, blankInfo);
                StockVM.WarehouseList = vmList;
            });

            if (StockSysNo.HasValue)
            {
                StockFacade.GetStockInfo(StockSysNo.Value, (vm) =>
                {
                    if (vm == null || vm.CompanyCode == null || vm.CompanyCode.Trim() != CPApplication.Current.CompanyCode)
                    {
                        vm = null;
                        Page.Context.Window.Alert("没有找到相应的仓库信息，此仓库信息可以已经被删除。");
                    }
                    else
                    {
                        StockVM = vm;
                        //if (StockVM != null)
                        //{
                        //    vm.WarehouseList = StockVM.WarehouseList;
                        //}
                    }
                    IniPageData();
                });
            }
            else
            {
                IniPageData();
            }

            Loaded -= new RoutedEventHandler(StockMaintain_Loaded);
        }


        private void IniPageData()
        {                    
            this.DataContext = StockVM;
            if (StockVM.WebChannel == null || String.IsNullOrEmpty(StockVM.WebChannel.ChannelID))
            {
                cmbWebChannelList.SelectedIndex = 0;
            }
            if (StockVM.WarehouseInfo == null || !StockVM.WarehouseInfo.SysNo.HasValue)
            {
                cmbWarehouseList.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StockVM.StockName) || string.IsNullOrEmpty(StockVM.StockID))
            {
                Page.Context.Window.Alert("请确定信息完整输入！");
                return;
            }

            StockVM.CompanyCode = CPApplication.Current.CompanyCode;
            if (StockSysNo.HasValue)
            {
                StockFacade.UpdateStock(StockVM, () =>
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
                StockFacade.CreateStock(StockVM, () =>
                {
                    StockSysNo = StockVM.SysNo;
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
        public event EventHandler Saved;

        private void ShowMessage(string message)
        {
            Page.Context.Window.Alert(message, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
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
