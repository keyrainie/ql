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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerPointsAddRequestNew : UserControl
    {

        public IDialog Dialog { get; set; }
        public CustomerPointsAddVM viewModel;
        CustomerPointsAddQueryFacade serviceFacade;
        ECCentral.Portal.UI.Customer.Facades.CustomerFacade queryCustomerFacade;
        List<ValidationEntity> validationListForSoSysNo;
        List<ValidationEntity> validationListForProductID;

        public CustomerPointsAddRequestNew(int? customerSysNo)
        {
            InitializeComponent();
            serviceFacade = new CustomerPointsAddQueryFacade();
            queryCustomerFacade = new ECCentral.Portal.UI.Customer.Facades.CustomerFacade();
            viewModel = new CustomerPointsAddVM();
            this.DataContext = viewModel;
            LoadSysAccountComboBoxData();
            validationListForSoSysNo = new List<ValidationEntity>();
            validationListForProductID = new List<ValidationEntity>();
            if (customerSysNo != null)
            {
                TextBox_CustomerID.SetCustomerSysNo(customerSysNo.Value);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // 添加顾客加积分申请操作:
            if (!ValidateInput())
            {
                return;
            }
            CustomerPointsAddRequest newRequestInfo = new CustomerPointsAddRequest();
            newRequestInfo.CustomerID = viewModel.CustomerID;
            newRequestInfo.CustomerSysNo = viewModel.CustomerSysNo.Value;
            newRequestInfo.PointType = Convert.ToInt32((Combo_Memo.SelectedItem as CodeNamePair).Code);
            newRequestInfo.NewEggAccount = (Combo_SysAccount.SelectedItem as CodeNamePair).Name;
            newRequestInfo.Memo = Combo_Memo.SelectedItem != null ? (Combo_Memo.SelectedItem as CodeNamePair).Name : string.Empty;
            newRequestInfo.OwnByDepartment = Combo_OwnByDepartment.SelectedItem != null ? (Combo_OwnByDepartment.SelectedItem as CodeNamePair).Name : string.Empty;
            if (!string.IsNullOrEmpty(viewModel.SOSysNo))
                newRequestInfo.SOSysNo = Convert.ToInt32(viewModel.SOSysNo);
            newRequestInfo.Point = Convert.ToInt32(viewModel.Point);
            newRequestInfo.PointsItemList = EntityConverter<CustomerPointsAddItemVM, CustomerPointsAddRequestItem>.Convert(viewModel.PointsItemList);
            newRequestInfo.Note = viewModel.Note;
            newRequestInfo.ProductID = txtProductID.Text.Length > 0 ? txtProductID.Text : null;

            serviceFacade.CreateCustomerPointsAddRequest(newRequestInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                else
                {
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.Data = args;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                }
            });
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }

        private void LoadSysAccountComboBoxData()
        {
            this.Combo_SysAccount.Items.Clear();
            this.Combo_Memo.Items.Clear();
            this.Combo_OwnByDepartment.Items.Clear();
            this.Combo_SysAccount.SelectionChanged += new SelectionChangedEventHandler(Combo_SysAccount_SelectionChanged);
            CodeNamePairHelper.GetList("Customer", "SystemAccount", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                this.Combo_SysAccount.ItemsSource = args.Result;
                this.Combo_SysAccount.SelectedIndex = 0;
            });
        }

        void Combo_SysAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //绑定"原因"ComboBox
            if (this.Combo_SysAccount.SelectedValue != null && !string.IsNullOrEmpty(this.Combo_SysAccount.SelectedValue.ToString()))
            {
                queryCustomerFacade.GetCustomerBySysNo(Convert.ToInt32(this.Combo_SysAccount.SelectedValue.ToString()), (obj, args) =>
                {
                    string getPoints = (args.Result.ValidScore.HasValue ? args.Result.ValidScore.Value.ToString() : "0");
                    this.txtSysAccountPoints.Text = string.Format("可用积分:{0}", getPoints);
                });
            }
            else
            {
                this.txtSysAccountPoints.Text = string.Empty;
            }

            BindReasonsBySysAccount();
        }

        private void BindReasonsBySysAccount()
        {
            if (null != Combo_SysAccount.SelectedValue && !string.IsNullOrEmpty(Combo_SysAccount.SelectedValue.ToString()))
            {

                string getSysAccountNo = this.Combo_SysAccount.SelectedValue.ToString();
                // 如果是CS - 补偿性积分，则绑定"责任部门"，清空
                if (Convert.ToInt32(getSysAccountNo) == 705571)
                {
                    this.Combo_Memo.ItemsSource = null;
                    this.Combo_OwnByDepartment.IsEnabled = true;
                    viewModel.OwnByDepartmentVisibility = System.Windows.Visibility.Visible;
                    CodeNamePairHelper.GetList("Customer", "OwnByDepartment", CodeNamePairAppendItemType.Select, (obj, args) =>
                     {
                         this.Combo_OwnByDepartment.ItemsSource = args.Result;
                         this.Combo_OwnByDepartment.SelectedIndex = 0;
                     });
                }
                else
                {
                    viewModel.OwnByDepartmentVisibility = System.Windows.Visibility.Collapsed;
                    this.Combo_OwnByDepartment.IsEnabled = false;
                    this.Combo_OwnByDepartment.ItemsSource = null;
                    CodeNamePairHelper.GetList("Customer", string.Format("ReasonForSystemAccountCode_{0}", getSysAccountNo), (obj, args) =>
                    {
                        this.Combo_Memo.ItemsSource = args.Result;
                        this.Combo_Memo.SelectedIndex = 0;
                    });

                }

            }
            else
            {
                this.Combo_Memo.ItemsSource = null;
                this.Combo_OwnByDepartment.ItemsSource = null;
            }
        }

        private bool ValidateInput()
        {
            viewModel.ValidationErrors.Clear();
            var txtPointReadonlyStatus = this.TextBox_AddPoints.IsReadOnly;
            this.TextBox_AddPoints.IsReadOnly = false;//只有非readoney才能验证
            ValidationManager.Validate(this.AddGrid);
            this.TextBox_AddPoints.IsReadOnly = txtPointReadonlyStatus;

            //如果原因是多付退款的话，需要验证 sosysno  
            if (this.TextBox_LoadingSOSysNo.Visibility == System.Windows.Visibility.Visible)
            {
                validationListForSoSysNo.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResCustomerPointsAddRequest.ValidateMsg_InputSoSysNo));
                validationListForSoSysNo.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResCustomerPointsAddRequest.ValidateMsg_SoSysNoNotValidate));
                if (!ValidationHelper.Validation(this.TextBox_LoadingSOSysNo, validationListForSoSysNo))
                    return false;
            }
            //如果是CS - 补偿性积分，且责任部门为PM实，需要验证ProductID
            if (this.Combo_SysAccount.SelectedValue != null
                && this.Combo_OwnByDepartment.SelectedValue != null)
            {
                if (this.Combo_SysAccount.SelectedValue.ToString() == "705571"
                    && this.Combo_OwnByDepartment.SelectedValue.ToString() == "1")
                {
                    validationListForProductID.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResCustomerPointsAddRequest.Validate_ProductIDNotNull));
                    if (!ValidationHelper.Validation(this.txtInputProductID, validationListForProductID))
                        return false;
                }
                else
                {
                    validationListForProductID.Clear();
                    this.txtInputProductID.Text = string.Empty;
                }
            }
            else
            {
                validationListForProductID.Clear();
                this.txtInputProductID.Text = string.Empty;
            }

            return viewModel.ValidationErrors.Count <= 0;

        }

        private void SwitchShowLoadingSO(bool isShow)
        {
            if (isShow)
            {
                this.TextBox_LoadingSOSysNo.Visibility = Visibility.Visible;
                this.ImageSOLoadingPicker.Visibility = Visibility.Visible;
                this.TextBox_SOSysNo.Visibility = Visibility.Collapsed;
                this.TextBox_AddPoints.IsReadOnly = true;
                this.TextBox_AddPoints.Text = string.Empty;
            }
            else
            {
                this.TextBox_SOSysNo.Visibility = Visibility.Visible;
                viewModel.RequestItemVisibility = Visibility.Collapsed;
                this.TextBox_LoadingSOSysNo.Visibility = Visibility.Collapsed;
                this.ImageSOLoadingPicker.Visibility = Visibility.Collapsed;
                this.TextBox_AddPoints.IsReadOnly = false;
                this.TextBox_AddPoints.Text = string.Empty;
                this.SelectRequestItemGrid.ItemsSource = null;
            }
        }

        private void Combo_OwnByDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.Combo_OwnByDepartment.SelectedValue && !string.IsNullOrEmpty(this.Combo_OwnByDepartment.SelectedValue.ToString()))
            {
                CodeNamePairHelper.GetList("Customer", string.Format("ReasonForSystemAccountCode_OwnByDepartmentCode_{0}", this.Combo_OwnByDepartment.SelectedValue.ToString()), (obj, args) =>
               {
                   this.Combo_Memo.ItemsSource = args.Result;
                   this.Combo_Memo.SelectedIndex = 0;
               });
            }
            else
            {
                this.Combo_Memo.ItemsSource = null;
            }
            CheckProductDomainInfo();
        }

        private void Combo_Memo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.Combo_Memo.SelectedValue)
            {
                //PM-价保积分的客户多付款产品调价,和Seller-Depreciation的客户多付款产品调价需要填写订单号
                string memoName = (this.Combo_Memo.SelectedItem as CodeNamePair).Name;
                if (memoName == ResCustomerEnum.AdjustPointReasonForSysAccount_Reason1_37)
                {
                    SwitchShowLoadingSO(true);
                }
                else
                {
                    SwitchShowLoadingSO(false);
                }
            }
            else
            {
                SwitchShowLoadingSO(false);
            }
            CheckProductDomainInfo();
        }

        //如果是CS - 补偿性积分，且责任部门为PM实，需要加载商品所属的Domain
        private void CheckProductDomainInfo()
        {
            if (this.Combo_SysAccount.SelectedValue == null
                || this.Combo_OwnByDepartment.SelectedValue == null
                || this.Combo_SysAccount.SelectedValue.ToString() != "705571"
                || this.Combo_OwnByDepartment.SelectedValue.ToString() != "1")
            {
                this.tbConfirmCategory1.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.tbConfirmCategory1.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void TextBox_CustomerID_CustomerSelected(object sender, CustomerSelectedEventArgs e)
        {
            //绑定顾客姓名，和可用积分:
            if (e.SelectedCustomer != null)
            {
                queryCustomerFacade.GetCustomerBySysNo(TextBox_CustomerID.CustomerSysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CustomerInfo getInfo = args.Result;
                    viewModel.CustomerName = getInfo.BasicInfo.CustomerName;
                    viewModel.ValidScore = getInfo.ValidScore;
                    viewModel.PointExpiringDate = getInfo.PointExpiringDate;
                });
            }
            else
            {
                viewModel.CustomerName = string.Empty;
                viewModel.ValidScore = null;
                viewModel.PointExpiringDate = null;
            }
        }

        private void ImageSOLoadingPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.TextBox_AddPoints.Focus();
            if (string.IsNullOrEmpty(this.TextBox_LoadingSOSysNo.Text))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.Validate_SOSysNoNull_Text);
                this.TextBox_SOSysNo.Focus();
                return;
            }
            int getSOSysNo = 0;
            if (!Int32.TryParse(this.TextBox_LoadingSOSysNo.Text, out getSOSysNo))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.Validate_SOSysNoValid_Text);
                this.TextBox_SOSysNo.Focus();
                return;
            }
            viewModel.SOSysNo = this.TextBox_LoadingSOSysNo.Text;
            CustomerPointsAddRequestItemsView itemsView = new CustomerPointsAddRequestItemsView(this.TextBox_LoadingSOSysNo.Text, ((CodeNamePair)this.Combo_SysAccount.SelectionBoxItem).Name.ToString());
            itemsView.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(
                       ResCustomerPointsAddRequest.Label_SOItems_View
                       , itemsView
                       , (s, args) =>
                       {
                           if (args.DialogResult == DialogResultType.OK && null != args.Data)
                           {
                               List<CustomerPointsAddItemVM> addItems = args.Data as List<CustomerPointsAddItemVM>;
                               if (null != addItems)
                               {
                                   addItems = addItems.Where(item => item.IsCheckedItem == true && !string.IsNullOrEmpty(item.Point)).ToList();
                                   viewModel.PointsItemList = addItems;
                                   int totalAddPoints = 0;
                                   addItems.ForEach(delegate(CustomerPointsAddItemVM vm)
                                   {
                                       vm.Point = (int.Parse(vm.Point) * vm.Quantity).ToString();
                                       totalAddPoints += int.Parse(vm.Point);
                                   });
                                   this.SelectRequestItemGrid.ItemsSource = addItems;
                                   this.SelectRequestItemGrid.Bind();
                                   this.TextBox_AddPoints.Text = (totalAddPoints == 0 ? string.Empty : totalAddPoints.ToString());
                               }
                               viewModel.RequestItemVisibility = Visibility.Visible;
                           }
                       }
                       , new Size(700, 500)
                );

        }

        //商品所属Domain加载事件
        private void txtProductID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //如果是CS - 补偿性积分，且责任部门为PM实，需要加载商品所属的Domain
            if (this.Combo_SysAccount.SelectedValue != null
                && this.Combo_OwnByDepartment.SelectedValue != null
                && this.Combo_SysAccount.SelectedValue.ToString() == "705571"
                && this.Combo_OwnByDepartment.SelectedValue.ToString() == "1")
            {
                LoadDomainProduct();
            }
        }

        //读取商品所属Domain
        private void LoadDomainProduct()
        {
            string productID = txtInputProductID.Text.Trim();
            if (productID.Length > 0)
            {
                (new OtherDomainQueryFacade()).QueryCategoryC1ByProductID(productID, (o, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        tbConfirmCategory1.Text = string.Format("{0}{1}({2})", ResCustomerPointsAddRequest.Label_ConfirmCategory1, args.Result.CategoryName, args.Result.SysNo);
                        txtProductID.Text = productID;
                    }
                    else
                    {
                        tbConfirmCategory1.Text = txtProductID.Text = "";
                    }
                });
            }
        }
    }
}
