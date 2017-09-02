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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class UCBackOrderAlertForToday : UserControl
    {
        public UCBackOrderAlertForToday()
        {
            InitializeComponent();
            queryFilter = new BackOrderForTodayQueryFilter();
            this.Loaded += new RoutedEventHandler(UCBackOrderAlertForToday_Loaded);
        }

        public BackOrderForTodayQueryFilter queryFilter;
        public List<InventoryTransferStockingVendorInfoVM> ResultList;
        public InventoryTransferStockingFacade serviceFacade;
        List<ValidationEntity> validateList;
        public IDialog Dialog { get; set; }

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

        void UCBackOrderAlertForToday_Loaded(object sender, RoutedEventArgs e)
        {

            validateList = new List<ValidationEntity>();
            validateList.Add(new ValidationEntity(ValidationEnum.IsInteger, this.txtVendorSysNo.Text, "供应商编号必须为整数"));
            this.Loaded -= UCBackOrderAlertForToday_Loaded;
            ResultList = new List<InventoryTransferStockingVendorInfoVM>();
            serviceFacade = new InventoryTransferStockingFacade(CurrentPage);
            this.GridSearchResult.Bind();
        }

        private void GridSearchResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.queryFilter.VendorSysNo = this.txtVendorSysNo.Text.Trim();
            this.queryFilter.VendorName = this.txtVendorName.Text.Trim();
            this.queryFilter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize
            };
            serviceFacade.QueryVendorInfoListForBackOrderToday(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var getResultList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                ResultList = DynamicConverter<InventoryTransferStockingVendorInfoVM>.ConvertToVMList(getResultList);
                this.GridSearchResult.TotalCount = totalCount;
                this.GridSearchResult.ItemsSource = ResultList;
            });

        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            // 全选Row:
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.GridSearchResult.ItemsSource)
                {
                    foreach (var item in this.GridSearchResult.ItemsSource)
                    {
                        if (item is InventoryTransferStockingVendorInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((InventoryTransferStockingVendorInfoVM)item).IsChecked)
                                {
                                    ((InventoryTransferStockingVendorInfoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((InventoryTransferStockingVendorInfoVM)item).IsChecked)
                                {
                                    ((InventoryTransferStockingVendorInfoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.txtVendorSysNo, validateList))
            {
                return;
            }

            this.GridSearchResult.Bind();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //确定操作:
            if (ResultList.Count > 0)
            {
                List<InventoryTransferStockingVendorInfoVM> selectedList = ResultList.Where(x => x.IsChecked == true).ToList();
                if (selectedList.Count <= 0)
                {
                    CurrentWindow.Alert("请选择至少一个供应商!");
                    return;
                }
                string getSelectedVendorSysNos = string.Empty;
                selectedList.ForEach(x =>
                {
                    getSelectedVendorSysNos += string.Format("{0}|", x.VendorSysNo);
                });
                getSelectedVendorSysNos = getSelectedVendorSysNos.TrimEnd('|');
                this.Dialog.ResultArgs.Data = getSelectedVendorSysNos;
                this.Dialog.Close(true);
            }
            else
            {
                this.Dialog.ResultArgs.Data = null;
                this.Dialog.Close(true);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作:
            this.Dialog.ResultArgs.Data = null;
            this.Dialog.Close(true);
        }
    }
}
