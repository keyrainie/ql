using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductQueryCondition : UserControl
    {
        #region 构造函数以及字段
        public const string TemplateType = "query";
        private ProductProfileTemplateFacade _facade;
        public event StockListSelectionChangedEvent OnStockListSelectionChanged;

        public ProductQueryExVM Model
        {
            get
            {
               return this.DataContext as ProductQueryExVM;
            }
        }
        public ProductQueryCondition()
        {
            InitializeComponent();
        }

        private void RaiseStockListSelectionChanged(object data)
        {
            var stockListSelectionChanged = OnStockListSelectionChanged;
            if (stockListSelectionChanged != null)
            {
                var prarm = new StockListSelectionChangedEventArgs { Data = data };
                stockListSelectionChanged(this, prarm);
            }
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _facade = new ProductProfileTemplateFacade();
            cbWarehouseNumberList.cmbStockListSelectionChanged += cbWarehouseNumberList_cmbStockListSelectionChanged;
            BindcbProfileTemplate(0);

        }

        private void cbWarehouseNumberList_cmbStockListSelectionChanged(object sender, EventArgs eventArgs)
        {
            var contorl = (Combox)sender;
            if (contorl != null)
            {
                if (contorl.SelectedValue == null)
                {
                    cbStockAccountQtyCondition.IsEnabled = false;
                    txtStockAccountQty.IsEnabled = false;
                    txtStockAccountQty.Text = "";
                }
                else
                {
                    cbStockAccountQtyCondition.IsEnabled = true;
                    txtStockAccountQty.IsEnabled = true;
                }
            }
        }

        private void cbProfileTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = cbProfileTemplate.SelectedItem as ProductProfileTemplateInfo;
            
            if (selectItem == null || selectItem.SysNo <= 0)
            {
                RaiseStockListSelectionChanged(DataContext);
                return;
            }
            if (!string.IsNullOrWhiteSpace(selectItem.TemplateValue))
            {
                string templateValue = ConvertToString(selectItem.TemplateValue);
                var jsonValue = JsonValue.Parse(templateValue);
                var entity = (ProductQueryExVM)_facade.ConvertToProductQueryExVM(jsonValue, typeof(ProductQueryExVM), null);
                entity.StockQuery.StockAccountQtyValue = entity.StockQuery.StockAccountQtyValue;
                RaiseStockListSelectionChanged(entity);
            }

        }

        #endregion
        #endregion

        #region 方法
        /// <summary>
        ///
        /// </summary>
        /// <param name="type">0初始化 1新增 2删除</param>
        public void BindcbProfileTemplate(int type)
        {
            var userID = CPApplication.Current.LoginUser.ID;

            if ((type == 0 && cbProfileTemplate.ItemsSource == null) || type == 1 || type == 2)
            {
                _facade.QueryProductProfileTemplateList(userID, TemplateType, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    var dataSource = args.Result ??
                                        new List<ProductProfileTemplateInfo>();
                    if (!dataSource.Any(p => p.SysNo == 0))
                    {
                        var item = new ProductProfileTemplateInfo { SysNo = 0, TemplateName = "新模板" };
                        dataSource.Insert(0, item);
                    }
                    cbProfileTemplate.ItemsSource = args.Result;
                    cbProfileTemplate.SelectedValue = 0;
                });
            }
        }


        #endregion

        #region 转换方法
        /// <summary>
        /// 把值转换成json字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertToString(string value)
        {
            var tempStrBytes = Convert.FromBase64String(value);
            var tempStr = Encoding.UTF8.GetString(tempStrBytes, 0, tempStrBytes.Length);
            return tempStr;
        }

        /// <summary>
        /// json字符串加密成Base64String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertToBase64String(string value)
        {
            byte[] bb = Encoding.UTF8.GetBytes(value);
            var tempStr = Convert.ToBase64String(bb);
            return tempStr;
        }
        #endregion

        private void cbEntryStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Model.EntryStatus == ProductEntryStatus.Entry)
            {
                cbEntryStatusEx.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                cbEntryStatusEx.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }

    #region 事件
    public class StockListSelectionChangedEventArgs
    {
        public object Data;
    }

    public delegate void StockListSelectionChangedEvent(object sender, StockListSelectionChangedEventArgs args);
    #endregion
}
