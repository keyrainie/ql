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
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS
{
    public partial class UCBatchInvoiceInput : UserControl
    {
        #region 页面初始化
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
        private List<EIMSInvoiceEntryVM> invoiceEntryViewList;

        public List<EIMSInvoiceEntryVM> InvoiceEntryViewList
        {
            get { return invoiceEntryViewList; }
            set { invoiceEntryViewList = value; }
        }

        private List<EIMSInvoiceInfoEntityVM> invoiceInfoEntityViewList;

        public List<EIMSInvoiceInfoEntityVM> InvoiceInfoEntityViewList
        {
            get { return invoiceInfoEntityViewList; }
            set { invoiceInfoEntityViewList = value; }
        }

        private EIMSInvoiceInfoEntityVM invoiceInfoEntityView;

        public EIMSInvoiceInfoEntityVM InvoiceInfoEntityView
        {
            get { return invoiceInfoEntityView; }
            set { invoiceInfoEntityView = value; }
        }

        private EIMSOrderMgmtFacade facade;

        public UCBatchInvoiceInput()
        {
            InitializeComponent();
        }

        public UCBatchInvoiceInput(List<EIMSInvoiceEntryVM> invoiceEntryViewList)
        {
            InitializeComponent();
            this.InvoiceEntryViewList = invoiceEntryViewList;
            Loaded += new RoutedEventHandler(UCBatchInvoiceInput_Loaded);
        }

        void UCBatchInvoiceInput_Loaded(object sender, RoutedEventArgs e)
        {
            BindTaxRate();
            InitControls();
            facade = new EIMSOrderMgmtFacade();
            this.gridInfo.DataContext = InvoiceInfoEntityView = new EIMSInvoiceInfoEntityVM();
            BindInvoiceInfo(this.InvoiceEntryViewList);
        }

        /// <summary>
        /// 绑定税率信息
        /// </summary>
        private void BindTaxRate()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_TaxRate, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result != null)
                {
                    this.cmbTax.ItemsSource = args.Result;
                    this.cmbTax.SelectedIndex = 0;
                }
            });
        }

        /// <summary>
        /// 初始化控件状态
        /// </summary>
        private void InitControls()
        {
            this.txtBillNo.IsEnabled = this.txtBillAmt.IsEnabled = false;
        }

        private void BindInvoiceInfo(List<EIMSInvoiceEntryVM> list)
        {
            if (list != null)
            {
                decimal InvoiceAmtCount = 0m;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                    {
                        txtBillNo.Text += ";";
                    }
                    txtBillNo.Text += list[i].InvoiceNumber;
                    InvoiceAmtCount += list[i].InvoiceAmount;
                }
                txtBillAmt.Text = InvoiceAmtCount.ToString();
            }
        }

        #endregion

        #region 按钮事件相关
        /// <summary>
        /// 确认按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtInvoiceAmt.Text)
                || string.IsNullOrEmpty(this.txtInvoiceInputSysNo.Text)
                || this.dpInvoiceInputDate.SelectedDate == null)
            {
                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InfoNotNull);
            }
            else
            {
                if (this.txtInvoiceInputSysNo.Text.Length != 8)
                    this.CurrentWindow.Alert(string.Format(ResEIMSInvoiceEntry.Msg_InvoiceInputCheck, this.txtInvoiceInputSysNo.Text));
                else
                {

                    decimal invoiceAmt = 0m;
                    if (Regex.IsMatch(this.txtInvoiceAmt.Text, @"(^\d+$)|(^\d+.\d+$)"))
                    {
                        invoiceAmt = Convert.ToDecimal(this.txtInvoiceAmt.Text);
                    }
                    if (invoiceAmt != Convert.ToDecimal(this.txtBillAmt.Text))
                    {
                        this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InvoiceInputAmountNoEquels);
                    }
                    else
                    {
                        BatchInvoiceInuputInfo();
                        facade.CreateEIMSInvoiceInput(InvoiceInfoEntityViewList, (obj, args) =>
                        {
                            if (args.FaultsHandle()) return;
                            if (args.Result.IsSucceed)
                            {
                                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InputSuccess);
                                InvoiceInfoEntityViewList.Clear();
                                this.Dialog.ResultArgs.isForce = true;
                                this.Dialog.Close();
                            }
                            else
                            {
                                this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InputFailed);
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.isForce = false;
            this.Dialog.Close();
        }

        /// <summary>
        /// 组织批量录入的数据
        /// </summary>
        private void BatchInvoiceInuputInfo()
        {
            InvoiceInfoEntityViewList = new List<EIMSInvoiceInfoEntityVM>();
            InvoiceInfoEntityView.EIMSInvoiceInputExtendList = new List<EIMSInvoiceInputExtendVM>();
            for (int i = 0; i < InvoiceEntryViewList.Count; i++)
            {
                InvoiceInfoEntityView.EIMSInvoiceInputExtendList.Add(new EIMSInvoiceInputExtendVM()
                {
                    InvoiceNumber = InvoiceEntryViewList[i].InvoiceNumber
                });
            }
            InvoiceInfoEntityView.VendorNumber = InvoiceEntryViewList[0].VendorNumber;
            InvoiceInfoEntityViewList.Add(InvoiceInfoEntityView);

        }

        #endregion
    }
}
