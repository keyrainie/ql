using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Text;
using System.Globalization;
using System.Threading;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views.Promotion
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BatchCreateSaleGift : PageBase
    {
        private SaleGiftBatchInfoVM _GiftBatchInfoVM = new SaleGiftBatchInfoVM();

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IDialog CurrentDialog
        {
            get;
            set;
        }
        private CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        private SaleGiftQuery _ParentForm;

        /// <summary>
        /// 父窗体，即赠品查询界面。
        /// </summary>
        public SaleGiftQuery ParentForm
        {
            get {
                return _ParentForm;
            }
            set
            {
                _ParentForm = value;
            }
        }

        public BatchCreateSaleGift()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.DataContext = _GiftBatchInfoVM;

            this.CMB_GiftType.SelectionChanged += new SelectionChangedEventHandler(CMB_GiftType_SelectionChanged);
            
            this.CB_IsMarkGift.Checked += new RoutedEventHandler(CB_IsMarkGift_Checked);
            this.CB_IsMarkGift.Unchecked += new RoutedEventHandler(CB_IsMarkGift_Unchecked);


            this.CMB_GiftType_SelectionChanged(null, null);
            CB_IsMarkGiftIsCheckedChanged();
            InitGiftGrid();

            this.Txt_CombineTip.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            this.Grid_GiftItems.LoadingRow += new EventHandler<DataGridRowEventArgs>(Grid_GiftItems_LoadingRow);
        }

        private List<DataGridRow> rows = new List<DataGridRow>();

        void Grid_GiftItems_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            rows.Add(e.Row);
        }

        void CB_IsMarkGift_Unchecked(object sender, RoutedEventArgs e)
        {
            Txt_TotalQty.IsReadOnly = false;
            this.Grid_GiftItems.Columns[8].Visibility = Visibility.Collapsed;
            this.Grid_GiftItems.Columns[8].IsReadOnly = true;
            this._GiftBatchInfoVM.Gifts.ForEach(p => p.HandselQty = "1");
          
        }

        private void CB_IsMarkGiftIsCheckedChanged()
        {
            if (_GiftBatchInfoVM.IsSpecifiedGift)
            {
                Txt_TotalQty.IsReadOnly = true;
                this.Grid_GiftItems.Columns[8].Visibility = Visibility.Visible;
                this.Grid_GiftItems.Columns[8].IsReadOnly = false;               
            }
            else
            {
                Txt_TotalQty.IsReadOnly = false;
                this.Grid_GiftItems.Columns[8].Visibility = Visibility.Collapsed;
                this.Grid_GiftItems.Columns[8].IsReadOnly = true;
                this._GiftBatchInfoVM.Gifts.ForEach(p => p.HandselQty = "1");
            }
        }

        /// <summary>
        /// 是否赠品变化时。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CB_IsMarkGift_Checked(object sender, RoutedEventArgs e)
        {
            Txt_TotalQty.IsReadOnly = true;
            Txt_TotalQty.Text = string.Empty;
            var error = this._GiftBatchInfoVM.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains("TotalQty"));
            if (error != null)
            {
                this._GiftBatchInfoVM.ValidationErrors.Remove(error);
            }

            this.Grid_GiftItems.Columns[8].Visibility = Visibility.Visible;
            this.Grid_GiftItems.Columns[8].IsReadOnly = false;
        }

        /// <summary>
        /// 设置赠品列表列，除了优先级跟赠送数量，其他列都为只读。
        /// </summary>
        private void InitGiftGrid()
        {
            if (this.Grid_GiftItems.Columns != null && this.Grid_GiftItems.Columns.Count > 0)
            {
                foreach (DataGridColumn column in this.Grid_GiftItems.Columns)
                {
                    if (column.DisplayIndex != 7 && column.DisplayIndex != 8)
                    {
                        column.IsReadOnly = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// 赠品类型改变时。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CMB_GiftType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_GiftBatchInfoVM.SaleGiftType == SaleGiftType.Multiple)
            {
                this.Panel_CombineInfo.Visibility = Visibility.Visible;
                this.Grid_ProductItemInfo2.Visibility = Visibility.Visible;
                this.Grid_ProductItems1.Columns[7].Visibility = System.Windows.Visibility.Visible;
                this.Grid_ProductItems2.Columns[7].Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Panel_CombineInfo.Visibility = Visibility.Collapsed;
                this.Grid_ProductItemInfo2.Visibility = Visibility.Collapsed;
                this.Grid_ProductItems1.Columns[7].Visibility = System.Windows.Visibility.Collapsed;
                this.Grid_ProductItems2.Columns[7].Visibility = System.Windows.Visibility.Collapsed;
            }

            CB_IsMarkGift.IsChecked = false;

            if (_GiftBatchInfoVM.SaleGiftType == SaleGiftType.Vendor)
            {
                CB_IsMarkGift.IsEnabled = false;
                CB_IsMarkGift.IsChecked = true;
                Txt_TotalQty.IsEnabled = false;
                Txt_TotalQty.Text = string.Empty;

                this.Grid_GiftItems.Columns[8].Visibility = Visibility.Visible;                
            }
            else
            {
                CB_IsMarkGift.IsEnabled = true;
                Txt_TotalQty.IsEnabled = true;
            }

            if (_GiftBatchInfoVM.SaleGiftType == SaleGiftType.Full || _GiftBatchInfoVM.SaleGiftType == SaleGiftType.Vendor)
            {
                vendorPicker.IsEnabled = true;
            }
            else
            {
                vendorPicker.IsEnabled = false;
            }

            _GiftBatchInfoVM.ProductItems1.Clear();
            _GiftBatchInfoVM.ProductItems2.Clear();
            _GiftBatchInfoVM.Gifts.Clear();

            this.Grid_GiftItems.ItemsSource = _GiftBatchInfoVM.Gifts;
            this.Grid_ProductItems1.ItemsSource = _GiftBatchInfoVM.ProductItems1;
            this.Grid_ProductItems2.ItemsSource = _GiftBatchInfoVM.ProductItems2;
        }

        private enum ItemAddType
        {
            Product,
            Gift
        }

        /// <summary>
        /// 添加商品到控件。
        /// </summary>
        /// <param name="gridControl"></param>
        /// <param name="itemList"></param>
        /// <param name="addType"></param>
        private void AddProductToDataGrid(Newegg.Oversea.Silverlight.Controls.Data.DataGrid gridControl,List<ProductItemVM> itemList, ItemAddType addType)
        {
            UCProductSearch ucPicker = new UCProductSearch();
            ucPicker.SelectionMode = SelectionMode.Multiple;
            StringBuilder sb = new StringBuilder();
            ucPicker.DialogHandler = CurrentWindow.ShowDialog(ECCentral.Portal.UI.MKT.Resources.ResBatchCreateSaleGift.Info_PickProducts, ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data == null) return;

                    List<ProductVM> selectedList = args.Data as List<ProductVM>;
                    foreach (ProductVM product in selectedList)
                    {
                        if (itemList.Where(p => p.ProductSysNo == product.SysNo).ToList().Count > 0)
                        {
                            sb.AppendLine(string.Format(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_GoodsExsistInList",CurrentCulture), product.ProductID));
                            continue;
                        }

                        if (addType == ItemAddType.Product)
                        {        
                            if (product.Status.Value != BizEntity.IM.ProductStatus.Active)
                            {
                                sb.AppendLine(string.Format(ECCentral.Portal.UI.MKT.Resources.ResBatchCreateSaleGift.Info_ProductMustBeActive, product.ProductID));//商品必须为上架商品。
                                continue;
                            }
                        }
                        else if (addType == ItemAddType.Gift)
                        {
                            if (product.Status.Value != BizEntity.IM.ProductStatus.InActive_UnShow)
                            {
                                sb.AppendLine(string.Format(ECCentral.Portal.UI.MKT.Resources.ResBatchCreateSaleGift.Info_ProductMustBeUnDisplay, product.ProductID));//商品必须为不展示商品。
                                continue;
                            }
                        }
                        else
                        { }

                        ProductItemVM item = new ProductItemVM();
                        item.IsChecked = false;
                        item.ProductID = product.ProductID;
                        item.ProductSysNo = product.SysNo;
                        item.ProductName = product.ProductName;
                        item.AvailableQty = product.AvailableQty;
                        item.ConsignQty = product.ConsignQty;
                        item.UnitCost = product.UnitCost;
                        item.CurrentPrice = product.CurrentPrice;

                        itemList.Add(item);

                    }

                    gridControl.ItemsSource = itemList;

                    if (sb.Length > 0)
                    {
                        CurrentWindow.Alert(sb.ToString());
                    }
                }
            });
        }

        private void BatchDeleteItemFromGridControl(Newegg.Oversea.Silverlight.Controls.Data.DataGrid gridControl)
        {
            if (gridControl == null)
                return;
            if (gridControl.ItemsSource == null)
                return;

            List<ProductItemVM> items = gridControl.ItemsSource as List<ProductItemVM>;
            if (items == null)
                return;

            List<ProductItemVM> removeItems = new List<ProductItemVM>();
            foreach (ProductItemVM item in items)
            {
                if (item.IsChecked)
                {
                    removeItems.Add(item);
                }
            }

            foreach (ProductItemVM removeItem in removeItems)
            {
                items.Remove(removeItem);
            }
            gridControl.ItemsSource = items;            
        }

        private void Button_AddGiftProducts_Click(object sender, RoutedEventArgs e)
        {
            AddProductToDataGrid(this.Grid_GiftItems, _GiftBatchInfoVM.Gifts, ItemAddType.Gift);
        }

        private void Button_AddProductItems2_Click(object sender, RoutedEventArgs e)
        {
            AddProductToDataGrid(this.Grid_ProductItems2, _GiftBatchInfoVM.ProductItems2, ItemAddType.Product);
        }

        private void Button_AddProductItems1_Click(object sender, RoutedEventArgs e)
        {
            AddProductToDataGrid(this.Grid_ProductItems1, _GiftBatchInfoVM.ProductItems1, ItemAddType.Product);
        }

        private void Button_BatchDeleteItems2_Click(object sender, RoutedEventArgs e)
        {
            chkHiddenItem2.IsChecked = false;
            BatchDeleteItemFromGridControl(this.Grid_ProductItems2);
        }

        private void Button_BatchDeleteItems1_Click(object sender, RoutedEventArgs e)
        {
            chkHiddenItem1.IsChecked = false;
            BatchDeleteItemFromGridControl(this.Grid_ProductItems1);
        }

        private void Button_BatchDeleteGift_Click(object sender, RoutedEventArgs e)
        {
            chkHiddenGift.IsChecked = false;
            BatchDeleteItemFromGridControl(this.Grid_GiftItems);
        }

        private bool ValidateGiftGrid()
        {
            foreach (DataGridRow row in rows)
            {
                if (!ValidationManager.Validate(row))
                {
                    return false;
                }
            }

            return true;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (_GiftBatchInfoVM.HasValidationErrors) return;

            if (!ValidationManager.Validate(this.Grid_PrimaryInfo))
            {
                return;
            }

            if (DateTime.Parse(_GiftBatchInfoVM.BeginDate) > DateTime.Parse(_GiftBatchInfoVM.EndDate))
            {
                //Window.Alert("开始时间不能大于结束时间！");
                Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_StartDateMoreThanEndDate",CurrentCulture));
                return;
            }

            if (_GiftBatchInfoVM.SaleGiftType != SaleGiftType.Vendor && string.IsNullOrEmpty(_GiftBatchInfoVM.PromotionName))
            {
                //Window.Alert("规则名称不能为空！");
                Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_RuleNameNotNull", CurrentCulture));
                return;
            }

            if (!_GiftBatchInfoVM.IsSpecifiedGift&&string.IsNullOrEmpty(this._GiftBatchInfoVM.TotalQty))
            {
                //Window.Alert("非指定赠品总数量不能为空！");
                Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_QuantityNotNull", CurrentCulture));
                return;
            }

            if (_GiftBatchInfoVM.SaleGiftType.HasValue)
            {
                foreach (ProductItemVM gift in _GiftBatchInfoVM.Gifts)
                {
                    if (string.IsNullOrEmpty(gift.Priority))
                    {
                        //Window.Alert("优先级不能为空！");
                        Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_PriorityNotNull", CurrentCulture));
                        return;
                    }
                    if (_GiftBatchInfoVM.IsSpecifiedGift && string.IsNullOrEmpty(gift.HandselQty))
                    {
                       // Window.Alert("指定赠品赠送数量不能为空！");
                        Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_TheQuantityNotNull", CurrentCulture));
                        return;
                    }
                }

                if (_GiftBatchInfoVM.SaleGiftType == SaleGiftType.Multiple)
                {
                  
                    foreach (ProductItemVM gift in _GiftBatchInfoVM.ProductItems1)
                    {
                        if (gift.HasValidationErrors) return;

                        if (string.IsNullOrEmpty(gift.PurchasingAmount))
                        {
                            //Window.Alert("左侧购买数量不能为空！");
                            Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_LeftBuyQuantityNotNull", CurrentCulture));
                            return;
                        }                   
                    }

                    foreach (ProductItemVM gift in _GiftBatchInfoVM.ProductItems2)
                    {
                        if (gift.HasValidationErrors) return;

                        if (string.IsNullOrEmpty(gift.PurchasingAmount))
                        {
                            //Window.Alert("右侧购买数量不能为空！");
                            Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Warn_RightBuyQuantityNotNull", CurrentCulture));
                            return;
                        }
                     
                    }  
                }

            }

            if (!ValidateGiftGrid())
            {
                return;
            }

            string errorInfo = "";

            if (!this._GiftBatchInfoVM.ValidateQty(out errorInfo))
            {
                CurrentWindow.Alert(errorInfo);
                return;
            }

            SaleGiftFacade facade = new SaleGiftFacade(CPApplication.Current.CurrentPage);
            facade.BatchCreateSaleGift(_GiftBatchInfoVM, (s, args) =>
            {                
                //Window.Alert("保存成功！");
                Window.Alert(ResBatchCreateSaleGift.ResourceManager.GetString("Tips_SaveSuccess", CurrentCulture));
            });
        }

        /// <summary>
        /// 全选CheckBox打钩时。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetGrid"></param>
        private void OnCheckAllCheckBoxChecked(object sender,Newegg.Oversea.Silverlight.Controls.Data.DataGrid targetGrid)
        {
            var checkBoxAll = sender as CheckBox;
            if (targetGrid == null || targetGrid.ItemsSource == null || checkBoxAll == null)
                return;

            List<ProductItemVM> items = targetGrid.ItemsSource as List<ProductItemVM>;

            if (items == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                ProductItemVM item = items[i];

                item.IsChecked = checkBoxAll.IsChecked ?? false;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Close();
        }

        /// <summary>
        /// 左边商品列表全选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductItem1AllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnCheckAllCheckBoxChecked(sender, this.Grid_ProductItems1);
        }

        /// <summary>
        /// 右边商品列表全选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductItem2AllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnCheckAllCheckBoxChecked(sender, this.Grid_ProductItems2);
        }

        /// <summary>
        /// 赠品列表全选。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GiftsAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnCheckAllCheckBoxChecked(sender, this.Grid_GiftItems);
        }       
    }
}
