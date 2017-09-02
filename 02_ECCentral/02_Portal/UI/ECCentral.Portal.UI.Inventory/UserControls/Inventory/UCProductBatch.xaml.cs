using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Facades.Request;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Models.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Request;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory.Request;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.Inventory.UserControls.Inventory
{
    public partial class UCProductBatch : UserControl
    {
        #region Properties
        public int? StockSysNo
        {
            get;
            set;
        }

        public PageType PType
        {
            get;
            set;
        }

        #endregion

        public IWindow CurrentWidnow
        {
            get { return CPApplication.Current.CurrentPage.Context.Window; }
        }

        #region Field

        public string ProductSysNo { get; set; }
        public bool IsBatch { get; set; }
        public string ProductID { get; set; }
        public bool IsCreateMode { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsNotLend_Return { get; set; }
        public ConvertProductType ConvertType { get; set; }
        public int OperationQuantity { get; set; }
        public decimal? ConverterCost { get; set; }

        public IDialog DialogHandler { get; set; }

        protected List<ProductBatchInfoVM> batches;
        public ProductBatchRequestVM filterVM;
        protected ProductBatchQueryFilter query;
        private ProductBatchQueryFacade facade;
        private List<ProductBatchInfoVM> batchVM;
        private List<ProductBatchInfoVM> selectedLst;

        #endregion

        public UCProductBatch()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCProductBatch_Loaded);
            this.ucProductPicker.ProductSelected += ucProductPicker_ProductSelected;
        }

        void ucProductPicker_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            var isbatch = e.SelectedProduct.IsHasBatch;
            // 转换单算转换成本
            if (this.PType == PageType.Convert)
            {
                this.filterVM.ConvertCost = e.SelectedProduct.UnitCost;
            }

            if (isbatch == 0)
            {
                filterVM.HasBatch = false;
                this.batchExp.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.batchExp.Visibility = System.Windows.Visibility.Visible;
                //this.txtLendNum.Visibility = System.Windows.Visibility.Collapsed;
                //this.lblLendNum.Visibility = System.Windows.Visibility.Collapsed;

                this.IsBatch = true;
                filterVM.HasBatch = true;
                this.batchdg.Bind();
            }
            filterVM.ProductSysNo = e.SelectedProduct.SysNo.ToString();
        }

        public UCProductBatch(string productSysNo, string productID, bool isBatch, List<ProductBatchInfoVM> vm)
            : this()
        {
            batchVM = vm;
            this.ProductSysNo = productSysNo;
            this.ProductID = productID;
            this.IsBatch = isBatch;
        }

        public UCProductBatch(ProductBatchRequestVM requestVM, List<ProductBatchInfoVM> batchVM)
            : this(requestVM.ProductSysNo, requestVM.ProductID, false, batchVM)
        {

        }

        void UCProductBatch_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(UCProductBatch_Loaded);

            batchExp.Visibility = this.IsBatch ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            filterVM = new ProductBatchRequestVM();
            query = new ProductBatchQueryFilter();
            batches = new List<ProductBatchInfoVM>();
            facade = new ProductBatchQueryFacade(CPApplication.Current.CurrentPage);

            filterVM.HasBatch = false;
            filterVM.IsCreateMode = this.IsCreateMode;
            filterVM.IsNotReturn = this.IsNotLend_Return;
            //this.txtReturn.Visibility = System.Windows.Visibility.Collapsed;
            //this.lblReturn.Visibility = System.Windows.Visibility.Collapsed;
            //this.txtLendNum.Visibility = System.Windows.Visibility.Visible;
            //this.lblLendNum.Visibility = System.Windows.Visibility.Visible;


            if (StockSysNo.HasValue)
                filterVM.StockSysNo = StockSysNo.Value;
            if (ReturnDate.HasValue)
                filterVM.ReturnDate = ReturnDate.Value;
            if (this.ConverterCost.HasValue)
                filterVM.ConvertCost = this.ConverterCost;
            filterVM.IsCreateMode = IsCreateMode;
            filterVM.PType = this.PType;

            if (!this.IsNotLend_Return)
            {
                this.batchdg.Columns[10].Visibility = System.Windows.Visibility.Visible;
                this.batchdg.Columns[9].IsReadOnly = true;
                //this.txtReturn.Visibility = System.Windows.Visibility.Visible;
                //this.lblReturn.Visibility = System.Windows.Visibility.Visible;

                //this.txtLendNum.Visibility = System.Windows.Visibility.Collapsed;
                //this.lblLendNum.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (this.PType == PageType.Lend)
            {
                this.batchdg.Columns[9].Header = "借出数量";
            }
            else
            {
                this.batchdg.Columns[9].Header = "数量";
            }

            // 修改Model
            if (!IsCreateMode)
            {
                if (!this.IsBatch)
                {
                    switch (this.PType)
                    {
                        case PageType.Lend:
                            this.filterVM.LendNum = this.OperationQuantity.ToString();
                            break;
                        case PageType.Adjust:
                            this.filterVM.AdjustNum = this.OperationQuantity.ToString();
                            break;
                        case PageType.Convert:
                            this.filterVM.ConvertNum = this.OperationQuantity.ToString();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    this.filterVM.HasBatch = true;
                }

                filterVM.ProductSysNo = this.ProductSysNo;
                filterVM.ProductID = this.ProductID;
                this.batchdg.Bind();
            }

            // Convert
            if (this.PType == PageType.Convert)
            {
                if (this.ConvertType == ConvertProductType.Source)
                {
                    this.filterVM.ConvertType = "源商品";
                }
                else if (this.ConvertType == ConvertProductType.Target)
                {
                    this.filterVM.ConvertType = "目标商品";
                }
            }

            this.basicInfo.DataContext = filterVM;
        }

        protected void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool IsOk = true;

            if (null == this.CurrentWidnow)
                return;

            // UI Check
            if (!ValidationManager.Validate(this.basicInfo))
                return;

            // 已选择的批次
            this.selectedLst = new List<ProductBatchInfoVM>();
            if (this.IsNotLend_Return)
            {
                batches.ToList<ProductBatchInfoVM>().ForEach((p) =>
                {
                    if (p.Quantity != 0)
                    {
                        selectedLst.Add(p);
                    }
                });
            }
            else
            {
                batchVM.ToList<ProductBatchInfoVM>().ForEach((p) =>
                {
                    if (p.ReturnQuantity.HasValue&&p.ReturnQuantity != 0)
                    {
                        selectedLst.Add(p);
                    }
                });
            }

            // 根据商品编号获取商品信息
            ProductQueryFacade facade = new ProductQueryFacade(CPApplication.Current.CurrentPage);
            PagingInfo paging = new PagingInfo
            {
                PageIndex = 0,
                PageSize = 1,
            };
            List<ProductVM> productLst = new List<ProductVM>();
            facade.QueryProduct(new ProductSimpleQueryVM { ProductSysNo = filterVM.ProductSysNo }, paging
                , (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                productLst = DynamicConverter<ProductVM>.ConvertToVMList<List<ProductVM>>(args.Result.Rows);

                // 附加批次信息
                List<BatchInfoVM> batchList = EntityConverter<ProductBatchInfoVM, BatchInfoVM>.Convert(selectedLst);

                ProductVMAndBillInfo paramVM = new ProductVMAndBillInfo();
                paramVM.ProductVM = productLst;

                if (paramVM.ProductVM.FirstOrDefault().IsHasBatch == 1)
                {
                    paramVM.ProductVM.FirstOrDefault().ProductBatchLst = batchList;
                    if (filterVM.ReturnDate.HasValue)
                    {
                        paramVM.ReturnDate = filterVM.ReturnDate;
                    }
                    // check batch info
                    IsOk = PreCheckHasBatch();
                }
                else if (paramVM.ProductVM.FirstOrDefault().IsHasBatch == 0)
                {
                    switch (this.PType)
                    {
                        case PageType.Lend:
                            paramVM.Quantity = Convert.ToInt32(filterVM.LendNum);
                            paramVM.ReturnQuantity = Convert.ToInt32(filterVM.ReturnNum);
                            break;
                        case PageType.Adjust:
                            paramVM.Quantity = Convert.ToInt32(filterVM.AdjustNum);
                            break;
                        case PageType.Convert:
                            paramVM.Quantity = Convert.ToInt32(filterVM.ConvertNum);
                            break;
                    }
                    if (filterVM.ReturnDate.HasValue)
                    {
                        paramVM.ReturnDate = filterVM.ReturnDate;
                    }
                }

                if (!IsOk)
                    return;

                this.DialogHandler.ResultArgs.Data = paramVM;

                this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
                this.DialogHandler.Close();
            });


        }

        protected void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentWidnow == null)
                return;

            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandler.Close();
        }

        private void BindDataGrid(int totalCount, dynamic rows)
        {
            this.batchdg.TotalCount = totalCount;
            batches = DynamicConverter<ProductBatchInfoVM>.ConvertToVMList<List<ProductBatchInfoVM>>(rows);
            // 初始化数据
            batches.ForEach(p => p.IsNotLend_Return = IsNotLend_Return);

            // 修改时
            if (batchVM != null && batchVM.Count > 0 && !IsCreateMode)
            {
                foreach (var item in batchVM)
                {
                    batches.ForEach((vm) =>
                    {
                        if (vm.BatchNumber == item.BatchNumber)
                        {
                            vm.Quantity = item.Quantity;
                            vm.ReturnQuantity = item.ReturnQuantity;
                            item.Status = vm.Status;
                            item.StatusText = vm.StatusText;
                        }
                    });
                }
            }
            if (batches != null && batches.Count > 0)
            {
                if (!this.IsNotLend_Return)
                {
                    this.batchdg.ItemsSource = new ObservableCollection<ProductBatchInfoVM>(batchVM);
                }
                else
                {
                    this.batchdg.ItemsSource = batches;
                }

                this.batchExp.Visibility = System.Windows.Visibility.Visible;


            }
            else
            {
                this.batchExp.Visibility = System.Windows.Visibility.Collapsed;

            }
        }

        protected void batchdg_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (query == null || !this.IsBatch)
            {
                return;
            }
            PagingInfo pageing = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };


            query.PagingInfo = pageing;
            query.StockSysNo = Convert.ToInt32(filterVM.StockSysNo);
            if (!string.IsNullOrEmpty(filterVM.ProductSysNo))
                query.ProductSysNo = Convert.ToInt32(filterVM.ProductSysNo);


            facade.QueryProductBatchInfo(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                BindDataGrid(args.Result.TotalCount, args.Result.Rows);
            });
        }

        private void UCProductPicker_ProductSelected_1(object sender, ProductSelectedEventArgs e)
        {

        }

        private void SetControler(bool isBatch)
        {
            switch (PType)
            {
                case PageType.Lend:
                    if (isBatch)
                    {

                    }
                    break;
            }
        }

        private bool PreCheckHasBatch()
        {
            string tipMsg= "";

            if (this.batches != null && this.batches.Count > 0)
            {
                int sum = 0;
                bool haveItem = false;
                
                this.selectedLst.ForEach(p =>
                {
                    CheckBatch(p, ref tipMsg);

                    if (!haveItem && p.Quantity != 0)
                    {
                        haveItem = true;
                    }

                });

                if (!haveItem)
                {
                    CurrentWidnow.Alert("请至少选择一个批次的数量。");
                    return false;
                }

                if (tipMsg.Length > 5)
                {
                    CurrentWidnow.Alert(tipMsg.ToString());
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();

            if (this.PType != PageType.Adjust)
            {
                if (!Regex.IsMatch(txt.Text, @"^[1-9](\d{0,5})$"))
                {
                    if (Regex.IsMatch(txt.Text, @"^\d+$"))
                    {
                        txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                    }
                    else
                    {
                        txt.Text = "0";
                    }
                }
            }
            else
            {
                if (!Regex.IsMatch(txt.Text, @"^-|(\d{0,5})$"))
                {
                    if (Regex.IsMatch(txt.Text, @"^\d+$"))
                    {
                        txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                    }
                    else
                    {
                        txt.Text = "0";
                    }
                }
            }
        }

        private void txtReturnQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            //ConvertRequestItemVM vm = txt.DataContext as ConvertRequestItemVM;
            if (!Regex.IsMatch(txt.Text, @"^[1-9](\d{0,5})$"))
            {
                if (Regex.IsMatch(txt.Text, @"^\d+$"))
                {
                    txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                }
                else
                {
                    txt.Text = "0";
                }
            }
        }

        private void CheckBatch(ProductBatchInfoVM p, ref string tipMsg)
        {
            switch (this.PType)
            {
                case PageType.Lend:
                    PrintTipMsg(p, "借出", ref tipMsg);
                    break;
                case PageType.Adjust:
                    if (p.Quantity < 0)
                    {
                        PrintTipMsg(p, "损益", ref tipMsg);
                    }
                    break;
                case PageType.Convert:
                    PrintTipMsg(p, "转换", ref tipMsg);
                    break;
            }
        }

        private void PrintTipMsg(ProductBatchInfoVM p, string title, ref string tipMsg)
        {

            if (p.ReturnQuantity > 0 && p.ReturnQuantity > p.Quantity && !IsNotLend_Return)
            {
                tipMsg += string.Format("批次为 {0} 的商品{1}数量不能大于借出数量.\r\n", p.BatchNumber,"归还");
            }

            if (p.Quantity != 0 && IsNotLend_Return)
            {
                if (!(p.Quantity > 0 && this.PType == PageType.Adjust) && Math.Abs(p.Quantity) > p.ActualQty)
                {
                    tipMsg += string.Format("批次为 {0} 的商品{1}数量不能大于库存数量.\r\n", p.BatchNumber, title);
                }
            }

            if (p.Quantity != 0 && p.Status.Equals("R"))
            {
                tipMsg += string.Format("批次号为 {0} 的批号未激活，不能使用.\r\n", p.BatchNumber);
            }
        }
    }
}
