using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public partial class UCProductSearchForSO : UserControl
    {
        //赠品信息是否展示
        bool isShowRowDetails = false;
        #region Properties

        /// <summary>
        /// 返回值暂时保存的值，由于赠品可能出现重复，所以如果有重复情况，那key值应该是主商品SysNo+赠品SysNo
        /// </summary>
        Dictionary<string, ProductVM> m_selectData;

        ProductQueryFacade m_facade;

        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandler { get; set; }

        /// <summary>
        /// 查询条件,与UI建立双向绑定
        /// </summary>
        private ProductQueryFilter m_queryReq;

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        #endregion

        public UCProductSearchForSO()
        {
            InitializeComponent();
            this.Grid.DataContext = m_queryReq = new ProductQueryFilter();
            this.Loaded += UCProductSearchForSO_Loaded;
            m_selectData = new Dictionary<string, ProductVM>();
        }

        void UCProductSearchForSO_Loaded(object sender, RoutedEventArgs e)
        {
            m_facade = new ProductQueryFacade(CPApplication.Current.CurrentPage);
            //商品状态
            this.cmbType.ItemsSource = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            this.cmbType.SelectedIndex = 0;

            //库存比较操作符
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_Compare, (o, p) =>
            {
                this.cmbCompare.ItemsSource = p.Result;
                this.cmbCompare.SelectedIndex = 0;
            });
        }

        #region Event Handlers

        #region 按钮事件

        /// <summary>
        /// 查询按钮事件，直接调用DataGrid的Bind方法，DataGrid内部会触发LoadingDataSource事件
        /// </summary>
        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        void ButtonConfirmSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandler == null)
                return;

            if (m_selectData.Count == 0)
            {
                this.CurrentWindow.Alert(ResProductPicker.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //选择返回的数据
            var result = new List<ProductVM>();

            var list = m_selectData.Select(p => p.Value);

            #region 赠品数量的处理
                
            //处理逻辑为如果多种商品选择同一增品，那赠品数应该合并为一个
            //如果赠品数量大于onlineqty,那赠品数量=onlineqty
            //有可能赠品既是商品
            var groupGiftList = list
                .GroupBy(p => new {SysNo = p.SysNo,SOPT = p.ProductType });
            foreach (var item in groupGiftList)
            {
                var first = item.First();
                first.MasterSysNos = new List<int>();
                if (item.Count() == 1)
                {
                    if (first.SOProductType == SOProductType.Gift)
                    {
                        first.Quantity = CalcGiftQty(first.RuleQty
                                                    , GetData(first.MasterSysNo.Value, null).Quantity.Value
                                                    , first.OnlineQty.Value);
                        first.MasterSysNos.Add(first.MasterSysNo.Value);
                    }
                }
                else
                { 
                    //多个商品对应一个赠品的情况,只有赠品才有这种情况出现
                    
                    //赠品的online库存数，计算的时候为唯一
                    int giftOnlineQty = first.OnlineQty.Value;
                    int giftCountQty = 0;
                    foreach(var giftItem in item)
                    {
                        //商品的规则不一样，这里需要提出来单独计算
                        int masterQty = GetData(giftItem.MasterSysNo.Value, null).Quantity.Value;

                        int giftQty = CalcGiftQty(giftItem.RuleQty, masterQty, giftOnlineQty);

                        giftCountQty += giftQty;

                        //减去onlineqty
                        giftOnlineQty -= giftQty;

                        first.MasterSysNos.Add(giftItem.MasterSysNo.Value);
                    }
                    first.Quantity = giftCountQty;
                }
                //需要每条想的主商品编号初始为null,避免混淆
                first.MasterSysNo = null;
                result.Add(first);
            }

            #endregion

            #region 需要将延保转成商品

            foreach (var item in list)
            {
                //延保只有一个数据
                if (item.SOProductType == SOProductType.Product
                    && item.ExtendedWarrantyList != null
                    && item.ExtendedWarrantyList.Count == 1)
                {
                    var extended = item.ExtendedWarrantyList[0];
                    ProductVM vm = new ProductVM();
                    vm.ProductID = string.Format("{0}E", item.ProductID);
                    vm.Quantity = item.Quantity;
                    vm.ProductName = extended.BriefName;
                    vm.SOProductType = SOProductType.ExtendWarranty;
                    vm.CurrentPrice = extended.ServiceUnitPrice;
                    vm.MasterSysNo = item.SysNo;
                    vm.SysNo = extended.SysNo;
                    vm.TariffRate = item.TariffRate;
                    vm.UnitCost = extended.ServiceCost;
                    vm.AvgCost = extended.ServiceCost;
                    result.Add(vm);
                }
            }

            #endregion

            this.DialogHandler.ResultArgs.Data = result;
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandler.Close();
        }

        /// <summary>
        /// 计算赠品的Qty
        /// </summary>
        /// <param name="inputRuleQty">规则Qty基数</param>
        /// <param name="masterQty">主商品Qty(主商品可能有多个)</param>
        /// <param name="giftOnlineQty">赠品OnlineQty</param>
        /// <returns>赠品Qty</returns>
        private static int CalcGiftQty(int? inputRuleQty, int masterQty, int giftOnlineQty)
        {
            //计算赠品库存
            int ruleQty = 1;
            if (inputRuleQty.HasValue && inputRuleQty.Value > 0)
            {
                ruleQty = inputRuleQty.Value;
            }
            return giftOnlineQty - masterQty * ruleQty >= 0 ? masterQty * ruleQty : giftOnlineQty;
        }

        void ButtonCloseDialog_Click(object sender, RoutedEventArgs e)
        {
            if (this.DialogHandler == null)
                return;

            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.DialogHandler.Close();
        }

        /// <summary>
        /// 统一处理查询区域中TextBox的回车执行查询事件
        /// </summary>
        void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    this.DataGrid.Bind();
                }
            }
        }

        #endregion

        #region 表格内部事件

        #region 主表格

        //主表格全选标示
        bool m_isClickAll;

        //主表格全选是否已提示
        bool m_isAlerted;

        /// <summary>
        /// 请求服务，并将查询结果绑定到DataGrid
        /// </summary>
        void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件，分页信息
            //2.请求服务查询
            m_queryReq.PagingInfo = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            //不读取作废商品
            m_queryReq.IsNotAbandon = true;
            //不加载商家商品
            m_queryReq.AZCustomer = 0;
            m_queryReq.MerchantSysNo = 1;

            m_facade.QueryProduct(m_queryReq, (s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    DataGrid.TotalCount = args.Result.TotalCount;
                    Dictionary<string, object> changeColumns = new Dictionary<string, object>();
                    changeColumns.Add("IsCheck", false);
                    changeColumns.Add("Quantity", 1);
                    DataGrid.ItemsSource = args.Result.Rows.ToList(changeColumns);
                }
            });
        }

        //单项被选中状态
        private void DataGridCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            //需要先找到所在的行
            var row = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");

            var datagrid = UtilityHelper.GetParentObject<DataGrid>(ckb, "DataGrid");

            //判断库存
            DynamicXml selectedModel = datagrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;
            int outQty = 0;
            if (!int.TryParse(selectedModel["Quantity"].ToString(), out outQty))
            {
                selectedModel["Quantity"] = outQty = 1;
            }
            if (outQty < 1)
            {
                selectedModel["Quantity"] = outQty = 1;
            }
            int InventoryType = Convert.ToInt32(selectedModel["InventoryType"]);
            //自营商品库存检查
            if (InventoryType != (int)ProductInventoryType.Company
                && InventoryType != (int)ProductInventoryType.GetShopInventory
                && InventoryType != (int)ProductInventoryType.TwoDoor)
            {
               
                //总库存
                int qty = (int)selectedModel["OnlineQty"] - outQty;
                if (qty < 0)
                {
                    //全选只弹出一次，这里需要做一个弹出标示以弹出，将不再弹出，还需要判断是否是全选触发
                    if (!m_isClickAll || !m_isAlerted)
                    {
                        m_isAlerted = true;
                        CurrentWindow.Alert(ResProductPicker.Msg_OnlineQTYLessOne);
                    }
                    //当前项为不选中状态
                    ckb.IsChecked = false;
                    return;
                }
                //分仓最大库存
                qty = (int)selectedModel["MaxQuantity"] - outQty;
                if (qty < 0)
                {
                    //全选只弹出一次，这里需要做一个弹出标示以弹出，将不再弹出，还需要判断是否是全选触发
                    if (!m_isClickAll || !m_isAlerted)
                    {
                        m_isAlerted = true;
                        CurrentWindow.Alert(string.Format(ResProductPicker.Msg_OnlineQTYLessMaxBatchStock, selectedModel["MaxQuantity"]));
                    }
                    //当前项为不选中状态
                    ckb.IsChecked = false;
                    return;
                }
                if (selectedModel["TariffRate"] == null)
                {
                    if (!m_isClickAll || !m_isAlerted)
                    {
                        m_isAlerted = true;
                        CurrentWindow.Alert(ResProductPicker.Msg_NoTariffRate);
                    }
                    //当前项为不选中状态
                    ckb.IsChecked = false;
                    return;
                    
                }
            }
            //绑定数据：（暂定为界面数据绑定的，如果有其他需求再定）
            var productVM = DynamicConverter<ProductVM>.ConvertToVM(selectedModel);
            //特殊字段单独赋值
            productVM.SOProductType = SOProductType.Product;

            AddData(productVM);

            ReloadSelected();
        }

        //单项取消状态
        private void DataGridCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;

            //取消所有子类的选中绑定
            var row = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");

            //主表格定义数据
            DynamicXml selectedModel = DataGrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;

            //赠品表格
            var giftDataGird = UtilityHelper.GetChildObject<DataGrid>(row, "DataGridGift");
            if (giftDataGird != null)
            {
                var giftData = giftDataGird.ItemsSource as dynamic;
                if (giftData != null)
                {
                    foreach (var item in giftData)
                    {
                        item.IsCheck = false;
                    }
                }
                //下面操作为临时添加（实际情况不知道）
                giftDataGird.ItemsSource = null;
                giftDataGird.ItemsSource = giftData;
            }

            //延保
            var extendWarrantyDataGird = UtilityHelper.GetChildObject<DataGrid>(row, "DataGridExtendWarranty");
            if (extendWarrantyDataGird != null)
            {
                var data = extendWarrantyDataGird.ItemsSource as List<ExtendedWarrantyVM>;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.IsCheck = false;
                    }
                }
            }

            //附件

            //取消绑定数据
            //这里只用考虑主商品，副商品将会在取消选择中触发
            RemoveData((int)selectedModel["SysNo"], null);

            ReloadSelected();
        }

        /// <summary>
        /// DataGrid中当前页全选，全不选事件处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            //全选点击标示
            m_isClickAll = true;
            //初始化已选中状态
            m_isAlerted = false;

            CheckBox ckb = sender as CheckBox;
            //根据当前控件查询父类控件
            var datagrid = UtilityHelper.GetParentObject<DataGrid>(ckb, "DataGrid");
            dynamic viewList = datagrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    if (ckb.IsChecked!=null && ckb.IsChecked.Value)
                    {
                        DynamicXml selectedModel = view as DynamicXml;
                        int outQty = 0;
                        if (!int.TryParse(selectedModel["Quantity"].ToString(), out outQty))
                        {
                            selectedModel["Quantity"] = outQty = 1;
                        }
                        if (outQty < 1)
                        {
                            selectedModel["Quantity"] = outQty = 1;
                        }
                        int InventoryType = Convert.ToInt32(selectedModel["InventoryType"]);
                        //自营商品库存检查
                        if (InventoryType != (int)ProductInventoryType.Company
                            && InventoryType != (int)ProductInventoryType.GetShopInventory
                            && InventoryType != (int)ProductInventoryType.TwoDoor)
                        {

                            //总库存
                            int qty = (int)selectedModel["OnlineQty"] - outQty;
                            if (qty < 0)
                            {
                                //全选只弹出一次，这里需要做一个弹出标示以弹出，将不再弹出，还需要判断是否是全选触发
                                if (!m_isClickAll || !m_isAlerted)
                                {
                                    m_isAlerted = true;
                                    CurrentWindow.Alert(ResProductPicker.Msg_OnlineQTYLessOne);
                                }
                                //当前项为不选中状态
                                continue;
                            }
                            //分仓最大库存
                            qty = (int)selectedModel["MaxQuantity"] - outQty;
                            if (qty < 0)
                            {
                                //全选只弹出一次，这里需要做一个弹出标示以弹出，将不再弹出，还需要判断是否是全选触发
                                if (!m_isClickAll || !m_isAlerted)
                                {
                                    m_isAlerted = true;
                                    CurrentWindow.Alert(string.Format(ResProductPicker.Msg_OnlineQTYLessMaxBatchStock, selectedModel["MaxQuantity"]));
                                }
                                continue;
                            }
                            if (selectedModel["TariffRate"] == null)
                            {
                                if (!m_isClickAll || !m_isAlerted)
                                {
                                    m_isAlerted = true;
                                    CurrentWindow.Alert(ResProductPicker.Msg_NoTariffRate);
                                }
                                continue;
                            }
                        }
                    }
                    //在这里就会触发列的Checked或则UnChecked事件
                    view.IsCheck = ckb.IsChecked.Value;
                }
            }

            m_isClickAll = false;
        }

        //行模板详细状态改变触发事件
        private void DataGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            //如果是展开状态将加载数据
            if (e.Row.DetailsVisibility == Visibility.Visible)
            {
                var data = DataGrid.Columns[0].GetCellContent(e.Row).DataContext as DynamicXml;

                #region Gift

                if ((int)data["GiftItemCount"] > 0)
                {
                    //加载礼品
                    var giftDataGird = e.DetailsElement.FindName("DataGridGift") as DataGrid;
                    //如果加载过将不再读取
                    if (giftDataGird.ItemsSource == null)
                    {
                        m_facade.GetValidVenderGifts(data["SysNo"], (o, args) =>
                        {
                            if (!args.FaultsHandle())
                            {
                                var list = args.Result.Rows;
                                foreach (var item in list)
                                {
                                    item["IsCheck"] = false;
                                    item["MasterSysNo"] = data["SysNo"];
                                }
                                giftDataGird.ItemsSource = list;
                            }
                        });
                    }
                }

                #endregion

                #region ExtendWarranty
                if ((int)data["ExtendWarrantyCount"] > 0)
                {
                    //加载延保产品
                    var extendWarrantyDataGird = e.DetailsElement.FindName("DataGridExtendWarranty") as DataGrid;
                    //如果加载过将不再读取
                    if (extendWarrantyDataGird.ItemsSource == null)
                    {
                        var query = new CategoryExtendWarrantyQueryFilter();
                        query.PagingInfo = new PagingInfo
                        {
                            PageIndex = 0,
                            PageSize = 10,
                            SortBy = "A.SysNo"
                        };
                        query.C3SysNo = (int)data["C3SysNo"];
                        query.MaxUnitPrice = (decimal)data["Price"];
                        query.MinUnitPrice = (decimal)data["Price"];

                        m_facade.GetExtendWarranty(query, (o, args) =>
                        {
                            if (!args.FaultsHandle())
                            {
                                List<ExtendedWarrantyVM> vmlist = new List<ExtendedWarrantyVM>();
                                //将数据转为VMList
                                foreach (var item in args.Result.Rows)
                                {
                                    ExtendedWarrantyVM vm = DynamicConverter<ExtendedWarrantyVM>.ConvertToVM(item);
                                    vm.ServiceYears = (decimal)item["Years"];
                                    vm.ServiceUnitPrice = item["UnitPrice"];
                                    vm.ServiceCost = item["Cost"];
                                    vmlist.Add(vm);
                                }
                                extendWarrantyDataGird.ItemsSource = vmlist;
                            }
                        });
                    }
                }
                #endregion
            }
        }

        //数量输入判断(这里获取到的数据不是最新，除非再次触发，所以直接获取当前选择的值最佳)
        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox inputBox = sender as TextBox;
            int defaultQty = 1;

            DynamicXml selectedModel = DataGrid.SelectedItem as DynamicXml;
            //判断库存
            int outQty = 0;
            if ((bool)selectedModel["IsCheck"])
            {
                if (!int.TryParse(inputBox.Text, out outQty))
                {
                    inputBox.Text = defaultQty.ToString();
                    selectedModel["Quantity"] = outQty = defaultQty;
                }
                if (outQty < 1)
                {
                    inputBox.Text = defaultQty.ToString();
                    selectedModel["Quantity"] = outQty = 1;
                }
                int InventoryType = Convert.ToInt32(selectedModel["InventoryType"]);
                if (InventoryType == (int)ProductInventoryType.Company
                    || InventoryType == (int)ProductInventoryType.GetShopInventory
                    || InventoryType == (int)ProductInventoryType.TwoDoor)
                {
                    //ERP模式不验证库存
                }
                else
                {                  
                   
                    int qty = (int)selectedModel["OnlineQty"] - outQty;
                    if (qty < 0)
                    {
                        CurrentWindow.Alert(ResProductPicker.Msg_OnlineQTYLessOne);
                        inputBox.Text = defaultQty.ToString();
                        selectedModel["Quantity"] = 1;
                        return;
                    }
                }
                //绑定数据：（暂定为界面数据绑定的，如果有其他需求再定）
                var productVM = GetData((int)selectedModel["SysNo"], null);
                productVM.Quantity = outQty;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                int? tmpGiftCount = (e.AddedItems[0] as dynamic).GiftItemCount;
                if (tmpGiftCount.HasValue && tmpGiftCount.Value > 0 && isShowRowDetails)
                    DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                else
                    DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                isShowRowDetails = true;
            }
            else
            {
                DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                isShowRowDetails = false;
            }
        }

        #endregion

        #region 礼品表格

        //赠品全选
        private void DataGridGiftCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;

            //根据当前控件查询父类控件
            var datagrid = UtilityHelper.GetParentObject<DataGrid>(ckb, "DataGridGift");
            dynamic viewList = datagrid.ItemsSource as dynamic;
            //有数据在执行选中操作
            if (viewList != null)
            {
                foreach (var item in viewList)
                {
                    item.IsCheck = ckb.IsChecked.Value;
                }
                //下面操作为临时添加（实际情况不知道）
                datagrid.ItemsSource = null;
                datagrid.ItemsSource = viewList;
            }
        }

        //增品单项被选中状态
        private void DataGridGiftCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            var currentrow = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");
            if (currentrow == null)
            {
                return;
            }
            /*
             * 1.点击的时候会判断库存，如果库存不足判断提示，成功继续，失败返回
             * 2.点击触发主表格的行的复选框是否选中
             * 3.触发绑定数据
             */
            //找父表格
            var dataGrid = UtilityHelper.GetParentObject<DataGrid>(currentrow, "DataGridGift");
            DynamicXml selectedModel = dataGrid.Columns[0].GetCellContent(currentrow).DataContext as DynamicXml;

            if ((int)selectedModel["OnlineQty"] == 0)
            {
                ckb.IsChecked = false;
                return;
            }

            //查询主所在行
            var row = UtilityHelper.GetParentObject<DataGridRow>(dataGrid, "");
            //获取主当前绑定的项的值
            var data = DataGrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;

            //当前项被选中的时候，那父类的DataGrid的checkbox也要被选中
            if (!(bool)data["IsCheck"])
            {
                data["IsCheck"] = true;
            }
            //触发绑定数据
            //绑定数据：（暂定为界面数据绑定的，如果有其他需求再定）
            var productVM = DynamicConverter<ProductVM>.ConvertToVM(selectedModel);
            //特殊字段单独赋值
            productVM.SOProductType = SOProductType.Gift;

            AddData(productVM);
        }

        //增品单项被取消状态
        private void DataGridGiftCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //取消绑定数据
            CheckBox ckb = sender as CheckBox;
            var row = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");
            if (row == null)
            {
                return;
            }
            var dataGrid = UtilityHelper.GetParentObject<DataGrid>(row, "DataGridGift");
            DynamicXml selectedModel = dataGrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;
            RemoveData((int)selectedModel["SysNo"], selectedModel["MasterSysNo"]);
        }

        #endregion

        #region 延保表格

        private void DataGridExtendWarrantyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;
            //找父表格
            var datagrid = UtilityHelper.GetParentObject<DataGrid>(rbtn, "DataGridExtendWarranty");

            ExtendedWarrantyVM selectedModel = datagrid.SelectedItem as ExtendedWarrantyVM;
            //点击当前选项，那其他选项也该被取消选中
            var dataGridDataContext = datagrid.ItemsSource as List<ExtendedWarrantyVM>;
            dataGridDataContext.Where(p => p.SysNo != selectedModel.SysNo).ToList()
                               .ForEach(p => { p.IsCheck = false; });

            //主表格需要被选中状态
            //查询所在行
            var row = UtilityHelper.GetParentObject<DataGridRow>(datagrid, "");
            //获取当前绑定的项的值
            var data = DataGrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;
            if (!(bool)data["IsCheck"])
            {
                data["IsCheck"] = true;
            }

            //触发绑定数据
            var product = GetData((int)data["SysNo"], null);
            //选中的时候重新new,就不会出现多重的情况
            product.ExtendedWarrantyList = new List<ExtendedWarrantyVM>();
            product.ExtendedWarrantyList.Add(selectedModel);
        }

        #endregion

        #region 附件表格



        #endregion

        #region 选中表格

        private void hlbDelete_Click(object sender, RoutedEventArgs e)
        {
            bool isFindInCurrentPage = false;
            var selectedProductSysNo = (int)((HyperlinkButton)sender).Tag;
            var itemSource = DataGrid.ItemsSource;

            foreach (DynamicXml item in itemSource)
            {
                if ((int)item["SysNo"] == selectedProductSysNo)
                {
                    if ((bool)item["IsCheck"] == false)
                    { 
                        //如果已经去掉了，翻页后再回来就会有这情况
                        RemoveData(selectedProductSysNo, null);
                        ReloadSelected();
                    }
                    item["IsCheck"] = false;
                    isFindInCurrentPage = true;
                    break;
                }
            }
            if (!isFindInCurrentPage)
            {
                RemoveData(selectedProductSysNo, null);
                ReloadSelected();
            }
        }

        #endregion

        #endregion

        #endregion

        #region private method

        private void ReloadSelected()
        {
            var selectedList = m_selectData.Select(p => p.Value).Where(p=>p.SOProductType == SOProductType.Product);
            SelectedListBox.ItemsSource = selectedList;
        }

        /// <summary>
        /// 添加选中数据到
        /// </summary>
        /// <param name="productVM"></param>
        private void AddData(ProductVM productVM)
        {
            string key = productVM.SysNo.ToString();
            //赠品判断
            if (productVM.SOProductType == SOProductType.Gift)
            {
                key = string.Format("{0}_{1}", productVM.MasterSysNo, key);
            }
            if (!m_selectData.ContainsKey(key))
            {
                m_selectData.Add(key, productVM);
            }
        }

        private void RemoveData(int productSysNo, object masterSysNo)
        {
            string key = productSysNo.ToString();
            if (masterSysNo != null)
            {
                key = string.Format("{0}_{1}", masterSysNo, key);
            }

            if (m_selectData.ContainsKey(key))
            {
                m_selectData.Remove(key);
            }
        }

        private ProductVM GetData(int productSysNo, object masterSysNo)
        {
            string key = productSysNo.ToString();
            if (masterSysNo != null)
            {
                key = string.Format("{0}_{1}", masterSysNo, key);
            }
            if (m_selectData.ContainsKey(key))
            {
                return m_selectData[key];
            }
            return null;
        }

        #endregion

    }
}
