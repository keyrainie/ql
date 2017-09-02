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
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class CostChangeItemsQuery : UserControl
    {
        /// <summary>
        /// 返回值暂时保存的值
        /// </summary>
        public Dictionary<string, CostItemInfoVM> m_selectData;

        public CostChangeFacade m_facade;

        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandler { get; set; }

        /// <summary>
        /// 查询条件,与UI建立双向绑定
        /// </summary>
        private CostChangeItemsQueryFilter m_queryReq;

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

        public CostChangeItemsQuery(int vendorSysNo,int pmSysNo)
        {
            InitializeComponent();
            this.DataGrid.DataContext = m_queryReq = new CostChangeItemsQueryFilter() { VendorSysNo = vendorSysNo,PMSysNo = pmSysNo};
            //this.ucVendorPicker.SelectedVendorSysNo = vendorSysNo.ToString();
            //this.ucPM.SelectedPMSysNo = pmSysNo;
            this.Loaded += UCCostChangeItemsQuery_Loaded;
            m_selectData = new Dictionary<string, CostItemInfoVM>();
        }

        void UCCostChangeItemsQuery_Loaded(object sender, RoutedEventArgs e)
        {
            m_facade = new CostChangeFacade(CPApplication.Current.CurrentPage);
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
                this.CurrentWindow.Alert(ResCostChangeItemsQuery.Msg_PleaseSelect, MessageType.Warning);
                return;
            }
            //选择返回的数据
            var result = m_selectData.Select(p => p.Value).ToList<CostItemInfoVM>();

            this.DialogHandler.ResultArgs.Data = result;
            this.DialogHandler.ResultArgs.DialogResult = DialogResultType.OK;
            this.DialogHandler.Close();
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
            m_queryReq.PageInfo = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            int productSysNo;
            if (int.TryParse(ucProduct.ProductSysNo, out productSysNo))
            {
                m_queryReq.ProductSysNo = productSysNo;
            }
            else
            {
                m_queryReq.ProductSysNo = 0;
            }

            m_facade.QueryCostItems(m_queryReq, (s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    DataGrid.TotalCount = args.Result.TotalCount;
                    Dictionary<string, object> changeColumns = new Dictionary<string, object>();
                    changeColumns.Add("IsCheck", false);
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

            DynamicXml selectedModel = datagrid.Columns[0].GetCellContent(row).DataContext as DynamicXml;

            //绑定数据：（暂定为界面数据绑定的，如果有其他需求再定）
            var productVM = DynamicConverter<CostItemInfoVM>.ConvertToVM(selectedModel);

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

            //取消绑定数据
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
                    //在这里就会触发列的Checked或则UnChecked事件
                    view.IsCheck = ckb.IsChecked.Value;
                }
            }

            m_isClickAll = false;
        }

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
            var selectedList = m_selectData.Select(p => p.Value);
            SelectedListBox.ItemsSource = selectedList;
        }

        /// <summary>
        /// 添加选中数据到
        /// </summary>
        /// <param name="productVM"></param>
        private void AddData(CostItemInfoVM itemVM)
        {
            string key = itemVM.SysNo.ToString();

            if (!m_selectData.ContainsKey(key))
            {
                m_selectData.Add(key, itemVM);
            }
        }

        private void RemoveData(int itemSysNo, object item)
        {
            string key = itemSysNo.ToString();

            if (m_selectData.ContainsKey(key))
            {
                m_selectData.Remove(key);
            }
        }

        #endregion

    }
}
