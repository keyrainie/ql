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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public partial class UCStockPicker : UserControl
    {
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCStockPicker()
        {
            InitializeComponent();
            //将UC内部依赖属性上的绑定传递到控件。
            var StockIDBindingExp = this.GetBindingExpression(UCStockPicker.StockIDProperty);
            if (StockIDBindingExp != null && StockIDBindingExp.ParentBinding != null)
            {
                txtStockID.SetBinding(TextBox.TextProperty, StockIDBindingExp.ParentBinding);
            }
            var StockSysNoBindingExp = this.GetBindingExpression(UCStockPicker.StockSysNoProperty);
            if (StockSysNoBindingExp != null && StockSysNoBindingExp.ParentBinding != null)
            {
                txtStockSysNo.SetBinding(TextBox.TextProperty, StockSysNoBindingExp.ParentBinding);
            }
        }

        void txtStockID_KeyUp(object sender, KeyEventArgs e)
        {
            //只处理Enter事件
            if (e.Key != Key.Enter)
                return;
            this.StockSysNo = null;
            string StockID = this.txtStockID.Text.Trim();
            if (string.IsNullOrEmpty(StockID))
                return;
            var StockSelectedEvent = StockSelected;
            if (StockSelectedEvent != null)
            {
                var facade = new StockQueryFacade(CPApplication.Current.CurrentPage);
                facade.LoadStockByID(StockID, OnLoadStockByID);
                e.Handled = true;
            }
        }

        private void OnLoadStockByID(object sender, RestClientEventArgs<dynamic> args)
        {
            dynamic totalCount = args.Result.TotalCount;
            if (totalCount == 0)
            {
                //渠道库存ID不存在
                CurrentWindow.Alert(string.Format(ResStockPicker.Tip_StockIDNotExists, this.txtStockID.Text.Trim()), MessageType.Warning);
            }
            else if (totalCount > 1)
            {
                //同一渠道库存ID存在多个
                UCStockSearch ucStockSearch = new UCStockSearch();
                ucStockSearch.SelectionMode = SelectionMode.Single;
                ucStockSearch.BindDataGrid(totalCount, args.Result.Rows);
                ucStockSearch.DialogHandle = CurrentWindow.ShowDialog(ResStockPicker.Dialog_Title, ucStockSearch, OnDialogResult);
            }
            else
            {
                //渠道库存ID只存在一个
                StockVM selectedStock = DynamicConverter<StockVM>.ConvertToVM(args.Result.Rows[0]);
                this.StockSysNo = selectedStock.SysNo;
                OnStockSelected(selectedStock);
            }
        }

        void ImageStockPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UCStockSearch ucStockSearch = new UCStockSearch();
            ucStockSearch.SelectionMode = SelectionMode.Single;
            ucStockSearch.DialogHandle = CurrentWindow.ShowDialog(ResStockPicker.Dialog_Title, ucStockSearch, OnDialogResult);
        }
        private void OnDialogResult(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                var selectedStock = e.Data as StockVM;
                if (selectedStock != null)
                {
                    this.StockID = selectedStock.StockID;
                    this.StockSysNo = selectedStock.SysNo;
                    OnStockSelected(selectedStock);
                }
            }
        }


        /// <summary>
        /// 依赖属性：渠道库存ID,支持Silverlight绑定等特性
        /// </summary>
        public string StockID
        {
            get
            {
                string value = (string)GetValue(StockIDProperty) ?? "";
                if (value.Trim().Length == 0)
                {
                    value = this.txtStockID.Text.Trim();
                }
                return value;
            }
            set
            {
                SetValue(StockIDProperty, value);
            }
        }

        public static readonly DependencyProperty StockIDProperty =
            DependencyProperty.Register("StockID", typeof(string), typeof(UCStockPicker), new PropertyMetadata(null, (s, e) =>
            {
                var uc = s as UCStockPicker;
                uc.txtStockID.Text = (e.NewValue ?? "").ToString().Trim();
            }));

        /// <summary>
        /// 依赖属性：渠道库存系统编号,支持Silverlight绑定等特性
        /// </summary>
        public int? StockSysNo
        {
            get
            {
                string value = (GetValue(StockSysNoProperty) ?? "").ToString();
                if (value.Trim().Length == 0)
                {
                    value = this.txtStockSysNo.Text.Trim();
                }
                int sysNo;
                if (int.TryParse(value, out sysNo))
                {
                    return sysNo;
                }
                return null;
            }
            set { SetValue(StockSysNoProperty, value); }
        }

        public static readonly DependencyProperty StockSysNoProperty =
            DependencyProperty.Register("StockSysNo", typeof(int?), typeof(UCStockPicker), new PropertyMetadata(null, (s, e) =>
            {
                var uc = s as UCStockPicker;
                uc.txtStockSysNo.Text = (e.NewValue ?? "").ToString().Trim();
            }));

        /// <summary>
        /// 渠道库存选中事件
        /// </summary>
        public event EventHandler<StockSelectedEventArgs> StockSelected;

        private void OnStockSelected(StockVM selectedStock)
        {
            var handler = StockSelected;
            if (handler != null)
            {
                var args = new StockSelectedEventArgs(selectedStock);
                handler(this, args);
            }
        }

        private void txtStockSysNo_KeyUp(object sender, KeyEventArgs e)
        {
            //只处理Enter事件
            if (e.Key != Key.Enter)
                return;
            this.StockID = "";
            string StockSysNoString = this.txtStockSysNo.Text.Trim();
            if (string.IsNullOrEmpty(StockSysNoString))
                return;
            //验证渠道库存系统编号
            int StockSysNo = 0;
            if (!int.TryParse(StockSysNoString, out StockSysNo))
            {
                CurrentWindow.Alert(string.Format(ResStockPicker.Tip_StockSysNoInvalid, StockSysNoString), MessageType.Warning);
                e.Handled = true;
                return;
            }
            var StockSelectedEvent = StockSelected;
            if (StockSelectedEvent != null)
            {
                var facade = new StockQueryFacade(CPApplication.Current.CurrentPage);
                facade.LoadStockBySysNo(StockSysNo, OnLoadStockBySysNo);
                e.Handled = true;
            }
        }

        private void OnLoadStockBySysNo(object sender, RestClientEventArgs<dynamic> args)
        {
            dynamic totalCount = args.Result.TotalCount;
            if (totalCount == 0)
            {
                //渠道库存系统编号不存在
                CurrentWindow.Alert(string.Format(ResStockPicker.Tip_StockSysNoNotExists, this.txtStockSysNo.Text.Trim()), MessageType.Warning);
            }
            else
            {
                //渠道库存系统编号只存在一个
                StockVM selectedStock = DynamicConverter<StockVM>.ConvertToVM(args.Result.Rows[0]);
                this.StockID = selectedStock.StockID;
                OnStockSelected(selectedStock);
            }
        }
    }
}
