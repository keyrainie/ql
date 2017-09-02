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
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Data;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.Basic.Components.UserControls.CustomerPicker
{
    public partial class UCCustomerPicker : UserControl
    {
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCCustomerPicker()
        {
            InitializeComponent();

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //将UC内部依赖属性上的绑定传递到控件。
            var customerIDBindingExp = this.GetBindingExpression(UCCustomerPicker.CustomerIDProperty);
            if (customerIDBindingExp != null && customerIDBindingExp.ParentBinding != null)
            {
                txtCustomerID.SetBinding(TextBox.TextProperty, customerIDBindingExp.ParentBinding);
            }
            var customerSysNoBindingExp = this.GetBindingExpression(UCCustomerPicker.CustomerSysNoProperty);
            if (customerSysNoBindingExp != null && customerSysNoBindingExp.ParentBinding != null)
            {
                txtCustomerSysNo.SetBinding(TextBox.TextProperty, customerSysNoBindingExp.ParentBinding);
            }
        }


        /// <summary>
        /// 依赖属性：顾客ID,支持Silverlight绑定等特性
        /// </summary>
        public string CustomerID
        {
            get
            {
                string value = (string)GetValue(CustomerIDProperty) ?? string.Empty;
                if (value.Trim().Length == 0)
                {
                    value = this.txtCustomerID.Text.Trim();
                }
                return value;
            }
            set
            {
                SetValue(CustomerIDProperty, value);

            }
        }

        public static readonly DependencyProperty CustomerIDProperty =
            DependencyProperty.Register("CustomerID", typeof(string), typeof(UCCustomerPicker), new PropertyMetadata(null, (s, e) =>
                {
                    var uc = s as UCCustomerPicker;
                    uc.txtCustomerID.Text = (e.NewValue ?? string.Empty).ToString().Trim();
                }));

        /// <summary>
        /// 依赖属性：顾客系统编号,支持Silverlight绑定等特性
        /// </summary>
        public int? CustomerSysNo
        {
            get
            {
                string value = (GetValue(CustomerSysNoProperty) ?? "").ToString();
                if (value.Trim().Length == 0)
                {
                    value = this.txtCustomerSysNo.Text.Trim();
                }
                int sysNo;
                if (int.TryParse(value, out sysNo))
                {
                    return sysNo;
                }
                return null;
            }
            set { SetValue(CustomerSysNoProperty, value); }
        }

        public static readonly DependencyProperty CustomerSysNoProperty =
            DependencyProperty.Register("CustomerSysNo", typeof(int?), typeof(UCCustomerPicker), new PropertyMetadata(null, (s, e) =>
                {
                    var uc = s as UCCustomerPicker;
                    uc.txtCustomerSysNo.Text = (e.NewValue ?? "").ToString().Trim();
                }));

        /// <summary>
        /// 顾客选中事件
        /// </summary>
        public event EventHandler<CustomerSelectedEventArgs> CustomerSelected;

        private void OnCustomerSelected(CustomerVM selectedCustomer)
        {
            var handler = CustomerSelected;
            if (handler != null)
            {
                var args = new CustomerSelectedEventArgs(selectedCustomer);
                handler(this, args);
            }
        }

        #region  CustomerSysNo event

        private string oraginCustomerSysNo;

        private void txtCustomerSysNo_GotFocus(object sender, RoutedEventArgs e)
        {
            oraginCustomerSysNo = this.txtCustomerSysNo.Text.Trim();
        }

        private void txtCustomerSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            bool? result = CustomerSysNoPreCheck();
            if (result != null && result.Value)
            {
                ReSetCustomerSysNo();
            }
        }
        private void txtCustomerSysNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            bool? result = CustomerSysNoPreCheck();
            if (result == null)
            {
                e.Handled = true;
            }
            else if (result != null && result.Value)
            {
                ReSetCustomerSysNo();
                oraginCustomerID = this.txtCustomerID.Text.Trim();
                e.Handled = true;
            }

        }

        private bool? CustomerSysNoPreCheck()
        {
            if (string.IsNullOrEmpty(this.txtCustomerSysNo.Text.Trim()))//值为空不查询
            {
                this.txtCustomerSysNo.ClearValidationError();
                this.txtCustomerID.Text = string.Empty;
                return false;
            }
            int sysno;
            if (!int.TryParse(this.txtCustomerSysNo.Text.Trim(), out sysno))//不为int型也需要清空
            {
                this.txtCustomerSysNo.Validation("请输入一个有效的正整数");
                this.txtCustomerSysNo.Text = string.Empty;
                this.txtCustomerID.Text = string.Empty;
                return null;
            }
            if (this.txtCustomerSysNo.Text.Trim() == oraginCustomerSysNo)//值不变不查询
                return false;
            return true;
        }

        private void ReSetCustomerSysNo()
        {
            this.txtCustomerSysNo.ClearValidationError();
            var facade = new CustomerFacade(CPApplication.Current.CurrentPage);
            facade.LoadCustomerBySysNo(int.Parse(this.txtCustomerSysNo.Text.Trim()), OnLoadCustomerBySysNo);
        }

        private void OnLoadCustomerBySysNo(object sender, RestClientEventArgs<dynamic> args)
        {
            if (args.Result == null || args.Result.TotalCount == 0)
            {
                //顾客系统编号不存在
                this.txtCustomerID.Text = string.Empty;
                this.txtCustomerSysNo.Text = string.Empty;
                OnCustomerSelected(null);
            }
            else
            {
                //顾客系统编号只存在一个
                CustomerVM selectedCustomer = DynamicConverter<CustomerVM>.ConvertToVM(args.Result.Rows[0]);
                this.txtCustomerID.Text = selectedCustomer.CustomerID;
                this.txtCustomerSysNo.Text = selectedCustomer.SysNo.ToString();
                OnCustomerSelected(selectedCustomer);
                this.txtCustomerSysNo.Focus();
            }
        }
        #endregion

        #region customerID event

        private string oraginCustomerID = string.Empty;

        private void txtCustomerID_GotFocus(object sender, RoutedEventArgs e)
        {
            oraginCustomerID = this.txtCustomerID.Text.Trim();
        }

        private void txtCustomerID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CustomerIDPreCheck())
            {
                ReSetCustomerID();
            }
        }
        private void txtCustomerID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            if (CustomerIDPreCheck())
            {
                ReSetCustomerID();
                oraginCustomerID = this.txtCustomerID.Text.Trim();
                e.Handled = true;
            }
        }

        private bool CustomerIDPreCheck()
        {
            if (string.IsNullOrEmpty(this.txtCustomerID.Text.Trim()))//值为空不查询
            {
                this.txtCustomerSysNo.Text = string.Empty;
                return false;
            }
            if (this.txtCustomerID.Text.Trim() == oraginCustomerID)//值不变不查询
                return false;
            return true;
        }
        private void ReSetCustomerID()
        {
            this.txtCustomerSysNo.ClearValidationError();
            var facade = new CustomerFacade(CPApplication.Current.CurrentPage);
            facade.LoadCustomerByID(this.txtCustomerID.Text.Trim(), OnLoadCustomerByID);
        }
        private void OnLoadCustomerByID(object sender, RestClientEventArgs<dynamic> args)
        {
            if (args.Result == null || args.Result.TotalCount == 0)
            {
                //顾客帐号不存在
                this.txtCustomerID.Text = string.Empty;
                this.txtCustomerSysNo.Text = string.Empty;
                OnCustomerSelected(null);
            }
            else if (args.Result.TotalCount > 1)
            {
                //同一顾客帐号存在多个
                UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
                ucCustomerSearch.SelectionMode = SelectionMode.Single;
                ucCustomerSearch.DialogHandle = CurrentWindow.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, OnDialogResult);
                ucCustomerSearch._viewModel.CustomerID = this.txtCustomerID.Text;
                ucCustomerSearch.BindDataGrid(1, null);
            }
            else
            {
                //顾客帐号只存在一个
                CustomerVM selectedCustomer = DynamicConverter<CustomerVM>.ConvertToVM(args.Result.Rows[0]);
                this.txtCustomerSysNo.Text = selectedCustomer.SysNo.ToString();
                this.txtCustomerID.Focus();
                OnCustomerSelected(selectedCustomer);
            }
        }

        private void OnDialogResult(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                var selectedCustomer = e.Data as CustomerVM;
                if (selectedCustomer != null)
                {
                    this.txtCustomerID.Text = selectedCustomer.CustomerID;
                    this.txtCustomerSysNo.Text = selectedCustomer.SysNo.ToString();
                    OnCustomerSelected(selectedCustomer);
                    this.txtCustomerID.Focus();
                }
            }
            else if (e.DialogResult == DialogResultType.Cancel)
            {
                this.txtCustomerID.Text = string.Empty;
            }
        }
        #endregion

        #region  searchbutton event
        void ImageCustomerPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
            ucCustomerSearch.SelectionMode = SelectionMode.Single;
            ucCustomerSearch.DialogHandle = CurrentWindow.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, OnDialogResult);
        }
        #endregion

        /// <summary>
        /// 为控件赋初值
        /// </summary>
        /// <param name="customerSysNo"></param>
        public void SetCustomerSysNo(int customerSysNo)
        {
            this.txtCustomerSysNo.Text = customerSysNo.ToString();
            bool? result = CustomerSysNoPreCheck();
            if (result != null && result.Value)
            {
                ReSetCustomerSysNo();
            }

        }
    }
}
