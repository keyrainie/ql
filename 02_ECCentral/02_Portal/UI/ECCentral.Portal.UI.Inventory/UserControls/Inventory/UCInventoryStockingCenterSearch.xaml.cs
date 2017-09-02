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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class UCInventoryStockingCenterSearch : UserControl
    {
        /// <summary>
        /// 用于加载完数据，设置默认值
        /// </summary>
        /// <param name="sender">control</param>
        public delegate void SetDefaultValue(params object [] senders);
        public SetDefaultValue SetStockDefaultValueHandler;
        public SetDefaultValue SetCompareDefaultValueHandler;

        public UCInventoryStockingCenterSearch()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCInventoryStockingCenterSearch_Loaded);
        }

        void UCInventoryStockingCenterSearch_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCInventoryStockingCenterSearch_Loaded;
            BindComboBoxData();
        }

        /// <summary>
        /// 绑定DropDownList数据:
        /// </summary>
        private void BindComboBoxData()
        {
            //排序:
            CodeNamePairHelper.GetList("Inventory", "ProductCenterSortByField", (objj, argss) =>
                  {
                      if (argss.FaultsHandle())
                      {
                          return;
                      }
                      this.cmbOrderBy.ItemsSource = argss.Result;

                      Dictionary<string, string> defaultCompareOperatorType1 = new Dictionary<string, string>();
                      defaultCompareOperatorType1.Add("<=", "<=");
                      defaultCompareOperatorType1.Add("=", "=");
                      defaultCompareOperatorType1.Add(">=", ">=");
                      Dictionary<string, string> defaultCompareOperatorType2 = new Dictionary<string, string>();
                      defaultCompareOperatorType2.Add("=", "=");
                      defaultCompareOperatorType2.Add("<>", "<>");

                      #region [绑定比较操作符:]

                      this.cmbStatusCompare.ItemsSource = defaultCompareOperatorType2;
                      this.cmbStatusCompare.SelectedIndex = 0;

                      this.cmbSalesDayCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbSalesDayCompare.SelectedIndex = 0;
                      this.cmbAvailableSaleDaysCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbAvailableSaleDaysCompare.SelectedIndex = 0;
                      this.cmbRecommendStcokQty.ItemsSource = defaultCompareOperatorType1;
                      this.cmbRecommendStcokQty.SelectedIndex = 0;

                      this.cmbFinanceQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbFinanceQtyCompare.SelectedIndex = 0;

                      this.cmbAvailbleQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbAvailbleQtyCompare.SelectedIndex = 0;

                      this.cmbOrderedQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbOrderedQtyCompare.SelectedIndex = 0;

                      this.cmbSubStockQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbSubStockQtyCompare.SelectedIndex = 0;

                      this.cmbSettleQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbSettleQtyCompare.SelectedIndex = 0;

                      this.cmbCurrentQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbCurrentQtyCompare.SelectedIndex = 0;

                      this.cmbOnlineQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbOnlineQtyCompare.SelectedIndex = 0;

                      this.cmbVirtualQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbVirtualQtyCompare.SelectedIndex = 0;

                      this.cmbPurchaseQtyCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbPurchaseQtyCompare.SelectedIndex = 0;

                      this.cmbAverageUnitCostCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbAverageUnitCostCompare.SelectedIndex = 0;

                      this.cmbPriceCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbPriceCompare.SelectedIndex = 0;

                      this.cmbPointCompare.ItemsSource = defaultCompareOperatorType1;
                      this.cmbPointCompare.SelectedIndex = 0;

                      if (SetCompareDefaultValueHandler != null)
                          SetCompareDefaultValueHandler(cmbAvailableSaleDaysCompare, cmbSalesDayCompare);

                      #endregion

                      //***商品基本信息:
                      //商品类型:
                      this.cmbProductType.ItemsSource = EnumConverter.GetKeyValuePairs<ProductType>();
                      this.cmbProductType.SelectedIndex = 0;

                      //分仓:
                      CodeNamePairHelper.GetList("Inventory", "StockInfo", (obj, args) =>
                      {
                          if (args.FaultsHandle())
                          {
                              return;
                          }
                          this.cmbStock.ItemsSource = args.Result.Where(x => x.Code != "59").ToList();
                          this.cmbStock.SelectedIndex = 0;

                          //用于加载完数据，设置默认值
                          if (SetStockDefaultValueHandler != null)
                              SetStockDefaultValueHandler(cmbStock);
                      });
                      //备货天数:
                      CodeNamePairHelper.GetList("Inventory", "StockDay", (obj, args) =>
                      {
                          if (args.FaultsHandle())
                          {
                              return;
                          }
                          this.cmbStockDay.ItemsSource = args.Result;
                          this.cmbStockDay.SelectedIndex = 0;
                      });
                      //代销属性:
                      CodeNamePairHelper.GetList("Inventory", "ProductConsignFlag", (obj, args) =>
                      {
                          if (args.FaultsHandle())
                          {
                              return;
                          }
                          this.cmbConsign.ItemsSource = args.Result;
                          this.cmbConsign.SelectedIndex = 0;
                      });
                      //状态：
                      this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ProductStatus>(ECCentral.Portal.Basic.Utilities.EnumConverter.EnumAppendItemType.All);

                      //****库存:
                      this.cmbIsSync.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(ECCentral.Portal.Basic.Utilities.EnumConverter.EnumAppendItemType.All);
                      this.cmbIsSync.SelectedIndex = 0;
                      this.cmbIsLarge.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(ECCentral.Portal.Basic.Utilities.EnumConverter.EnumAppendItemType.All);
                      this.cmbIsLarge.SelectedIndex = 0;
                  });
        }

        private void cmbStockDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string getSelectedVal = this.cmbStockDay.SelectedValue.ToString();
            if (getSelectedVal == "-999")
            {
                this.txtStockDay.Text = string.Empty;
                this.txtStockDay.IsReadOnly = false;
            }
            else
            {
                this.txtStockDay.IsReadOnly = true;
                this.txtStockDay.Text = getSelectedVal;
            }
        }
    }
}
