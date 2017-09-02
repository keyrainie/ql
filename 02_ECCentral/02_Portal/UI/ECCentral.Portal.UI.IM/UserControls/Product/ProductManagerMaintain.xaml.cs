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
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductManagerMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        private PMFacade facade;
        public int SysNo { private get; set; }
        public bool IsUpdate { private get; set; }
        private PMVM model;
        public ProductManagerMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new PMFacade();

                facade.QueryAllProductManagerInfo((obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {

                        return;
                    }
                    List<ProductManagerInfo> list = arg.Result;
                    if (IsUpdate) //修改操作
                    {
                        //获取PM信息
                        facade.GetPMBySysNo(SysNo, (objs, args) =>
                        {
                            if (args.FaultsHandle())
                            {

                                return;
                            }
                            model = new PMVM()
                            {
                                SysNo=args.Result.SysNo,
                                PMID = args.Result.UserInfo.UserID,
                                PMUserName = args.Result.UserInfo.UserName,
                                MaxAmtPerOrder = args.Result.MaxAmtPerOrder.ToString(),
                                MaxAmtPerDay = args.Result.MaxAmtPerDay.ToString(),
                                PMDMaxAmtPerOrder = args.Result.PMDMaxAmtPerOrder.ToString(),
                                PMDMaxAmtPerDay = args.Result.PMDMaxAmtPerDay.ToString(),
                                ITMaxWeightforPerDay = args.Result.ITMaxWeightforPerDay.ToString(),
                                ITMaxWeightforPerOrder = args.Result.ITMaxWeightforPerOrder.ToString(),
                                SaleRatePerMonth = args.Result.SaleRatePerMonth.ToString(),
                                SaleTargetPerMonth = args.Result.SaleTargetPerMonth.ToString(),
                                WarehouseNumber = args.Result.WarehouseNumber,
                                Status=args.Result.Status
                            };

                            
                            //加载仓库
                            string[] warehouseArr = model.WarehouseNumber.Split(';');
                            List<ProductSalesAreaBatchStockVM> warehouselist = this.MyWarehouse.listStock.ItemsSource as List<ProductSalesAreaBatchStockVM>;
                            if (warehouseArr.Count() > 0 && warehouselist!=null)
                            {
                                foreach (var item in warehouseArr)
                                {
                                    foreach (var ware in warehouselist)
                                    {
                                        if (item == ware.SysNo.ToString())
                                        {
                                            ware.IsChecked = true;
                                        }
                                    }
                                }
                            }
                            this.DataContext = model;
                        });
                    }
                    else //添加操作
                    {
                        model = new PMVM();
                        this.DataContext = model;
                    }
                    
                 });
             };
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            List<ProductSalesAreaBatchStockVM> stockSource = this.MyWarehouse.listStock.ItemsSource as List<ProductSalesAreaBatchStockVM>;
            PMVM vm = this.DataContext as PMVM;

           // //拼接备份PM
           // string tempStr = "";
           // foreach (var item in source)
           // {
           //     if (item.IsSelected == true)
           //     {
           //         tempStr = tempStr + item.UserInfo.SysNo + ";";
           //     }
           // }
           // if (tempStr.Length > 1)
           // {
           //     tempStr = tempStr.Substring(0, tempStr.Length - 1);
           // }
           //vm.BackupUserList=tempStr;

            //拼接仓库
            string tempStr = "";
           foreach (var item in stockSource)
           {
               if (item.IsChecked == true)
               {
                   tempStr = tempStr + item.SysNo + ";";
               }
           }
           if (tempStr.Length > 1)
           {
               tempStr = tempStr.Substring(0, tempStr.Length - 1);
           }
           vm.WarehouseNumber = tempStr;

           if (IsUpdate)
           {
               facade.UpdatePM(vm, (obj, arg) =>
               {
                   if (arg.FaultsHandle())
                   {
                       return;
                   }
                   CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
               });
           }
           else
           {
               facade.CreatePM(vm, (obj, arg) => 
               {
                   if (arg.FaultsHandle())
                   {
                       return;
                   }
                   CPApplication.Current.CurrentPage.Context.Window.Alert("创建成功!");
               });
           }
        }
    }
}
