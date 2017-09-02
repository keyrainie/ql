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
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS
{
    public partial class UCViewInvoice : UserControl
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

        //记录最原始的记录数
        private int OrginCount = 0;

        EIMSInvoiceInfoEntityVM invoiceInfoEntityView;

        public EIMSInvoiceInfoEntityVM InvoiceInfoEntityView
        {
            get { return invoiceInfoEntityView; }
            set { invoiceInfoEntityView = value; }
        }

        EIMSInvoiceEntryVM invoiceEntryView;

        public EIMSInvoiceEntryVM InvoiceEntryView
        {
            get { return invoiceEntryView; }
            set { invoiceEntryView = value; }
        }

        List<EIMSInvoiceInfoEntityVM> invoiceInfoEntityViewList;

        public List<EIMSInvoiceInfoEntityVM> InvoiceInfoEntityViewList
        {
            get { return invoiceInfoEntityViewList; }
            set { invoiceInfoEntityViewList = value; }
        }

        EIMSInvoiceInfoVM invoiceInfoView;

        public EIMSInvoiceInfoVM InvoiceInfoView
        {
            get { return invoiceInfoView; }
            set { invoiceInfoView = value; }
        }


        EIMSOrderMgmtFacade facade;

        public UCViewInvoice()
        {
            InitializeComponent();
        }

        string invoiceNumber;

        public string InvoiceNumber
        {
            get { return invoiceNumber; }
            set { invoiceNumber = value; }
        }

        string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public UCViewInvoice(string invoiceNumber, string type)
        {
            InitializeComponent();
            this.Type = type;
            InitControl(Type);
            BindTaxRate();
            this.InvoiceNumber = invoiceNumber;
            facade = new EIMSOrderMgmtFacade();
            Loaded += new RoutedEventHandler(UCViewInvoice_Loaded);
        }

        public UCViewInvoice(List<EIMSInvoiceInfoEntityVM> invoiceInfoEntityViewList, EIMSInvoiceEntryVM invoiceEntryView, string type)
        {
            InitializeComponent();
            this.Type = type;
            InitControl(Type);
            BindTaxRate();
            this.InvoiceInfoEntityViewList = invoiceInfoEntityViewList;
            this.InvoiceEntryView = invoiceEntryView;
            this.InvoiceNumber = invoiceNumber;
            facade = new EIMSOrderMgmtFacade();
            Loaded += new RoutedEventHandler(UCViewInvoicePreview_Loaded);
        }

        private void InitControl(string type)
        {
            if (type == "View" || type == "Preview")
            {
                this.txtInvoiceAmt.IsEnabled = this.txtInvoiceInputDate.IsEnabled = this.txtInvoiceInputSysNo.IsEnabled = this.cmbTax.IsEnabled = false;
                this.txtAssignedCode.IsEnabled = this.txtBalanceType.IsEnabled = this.txtBillAmt.IsEnabled = this.txtVendorName.IsEnabled = false;
                this.dpInvoiceInputDate.Visibility = System.Windows.Visibility.Collapsed;
                this.gridOnlyOneInvoice.Visibility = System.Windows.Visibility.Visible;
                this.gridOneToInvoices.Visibility = System.Windows.Visibility.Collapsed;
                this.btnEdit.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (type == "Edit")
            {
                this.gridOnlyOneInvoice.Visibility = System.Windows.Visibility.Visible;
                this.gridOneToInvoices.Visibility = System.Windows.Visibility.Collapsed;
                this.txtInvoiceInputSysNo.IsEnabled = false;
                this.txtInvoiceInputDate.Visibility = System.Windows.Visibility.Collapsed;
                this.dpInvoiceInputDate.Visibility = System.Windows.Visibility.Visible;
                this.btnEdit.Visibility = System.Windows.Visibility.Visible;
            }
        }
        #endregion

        #region Loaded事件相关
        //查看
        void UCViewInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InvoiceNumber))
            {
                facade.QueryInvoiceList(InvoiceNumber, (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        if (args.Result.EIMSList.Convert<EIMSInvoiceInfoEntity, EIMSInvoiceInfoEntityVM>().Count > 1)
                        {
                            this.gridOnlyOneInvoice.Visibility = System.Windows.Visibility.Collapsed;
                            this.gridOneToInvoices.Visibility = System.Windows.Visibility.Visible;
                            this.InvoiceEntryView = args.Result.InvoiceInfoList.Convert<EIMSInvoiceEntryInfo, EIMSInvoiceEntryVM>()[0];
                            this.spOneToInvoicesResult.DataContext = InvoiceEntryView;
                            InvoiceInfoEntityViewList = args.Result.EIMSList.Convert<EIMSInvoiceInfoEntity, EIMSInvoiceInfoEntityVM>();
                            decimal? totalInvoiceInputAmount = 0;
                            //添加合计行
                            for (int i = 0; i < InvoiceInfoEntityViewList.Count; i++)
                            {
                                totalInvoiceInputAmount += InvoiceInfoEntityViewList[i].InvoiceInputAmount;
                                if (i == InvoiceInfoEntityViewList.Count - 1)
                                {
                                    InvoiceInfoEntityViewList.Add(new EIMSInvoiceInfoEntityVM() { Index = "合计", InvoiceInputAmount = totalInvoiceInputAmount });
                                    break;
                                }
                            }
                            this.dgOneToInvoicesQueryResult.ItemsSource = InvoiceInfoEntityViewList;

                        }
                        else
                        {
                            this.gridOnlyOneInvoice.Visibility = System.Windows.Visibility.Visible;
                            this.gridOneToInvoices.Visibility = System.Windows.Visibility.Collapsed;
                            this.InvoiceInfoView = args.Result.Convert<EIMSInvoiceInfo, EIMSInvoiceInfoVM>();
                            this.InvoiceInfoEntityView = invoiceInfoView.EIMSList[0];
                            this.spOnlyOneInvoiceResult.DataContext = InvoiceInfoEntityView;
                            OrginCount = InvoiceInfoView.InvoiceInfoList.Count;
                            this.dgOnlyOneInvoiceQueryResult.ItemsSource = InvoiceInfoView.InvoiceInfoList;
                        }

                    });
            }
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UCViewInvoicePreview_Loaded(object sender, RoutedEventArgs e)
        {
            this.gridOnlyOneInvoice.Visibility = System.Windows.Visibility.Collapsed;
            this.gridOneToInvoices.Visibility = System.Windows.Visibility.Visible;
            this.spOneToInvoicesResult.DataContext = InvoiceEntryView;
            decimal? totalInvoiceInputAmount = 0;
            //添加合计行
            for (int i = 0; i < InvoiceInfoEntityViewList.Count; i++)
            {
                totalInvoiceInputAmount += InvoiceInfoEntityViewList[i].InvoiceInputAmount;
                if (i == InvoiceInfoEntityViewList.Count - 1)
                {
                    InvoiceInfoEntityViewList.Add(new EIMSInvoiceInfoEntityVM() { Index = "合计", InvoiceInputAmount = totalInvoiceInputAmount });
                    break;
                }
            }
            this.dgOneToInvoicesQueryResult.ItemsSource = InvoiceInfoEntityViewList;
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
        /// 添加序号列数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOneToInvoicesQueryResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int index = e.Row.GetIndex();
            if (index < InvoiceInfoEntityViewList.Count - 1)
                InvoiceInfoEntityViewList[index].Index = (index + 1).ToString();
        }

        /// <summary>
        /// 当时查看状态时，不显示操作列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOnlyOneInvoiceQueryResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (Type == "View" || Type == "Preview")
            {
                this.dgOnlyOneInvoiceQueryResult.Columns[0].Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (Type == "Edit")
            {
                this.dgOnlyOneInvoiceQueryResult.Columns[0].Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        #region 按钮事件相关

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            List<EIMSInvoiceInfoEntityVM> old = InvoiceInfoView.OldEIMSList;
            if (InvoiceInfoEntityViewList == null)
                InvoiceInfoEntityViewList = new List<EIMSInvoiceInfoEntityVM>();
            if (this.InvoiceInfoView.InvoiceInfoList.Count != OrginCount)
            {
                if (InvoiceInfoEntityView.EIMSInvoiceInputExtendList != null && InvoiceInfoEntityView.EIMSInvoiceInputExtendList.Count != 0)
                {
                    for (int i = 0; i < InvoiceInfoEntityView.EIMSInvoiceInputExtendList.Count; i++)
                    {
                        if (this.InvoiceInfoView.InvoiceInfoList.Count != 0)
                        {
                            for (int j = 0; j < this.InvoiceInfoView.InvoiceInfoList.Count; j++)
                            {
                                if (this.InvoiceInfoView.InvoiceInfoList[j].InvoiceNumber != InvoiceInfoEntityView.EIMSInvoiceInputExtendList[i].InvoiceNumber)
                                    InvoiceInfoEntityView.EIMSInvoiceInputExtendList[i].Status = -1;
                            }
                        }
                        else
                            InvoiceInfoEntityView.EIMSInvoiceInputExtendList[i].Status = -1;
                    }
                }
            }
            InvoiceInfoEntityViewList.Add(InvoiceInfoEntityView);
            decimal? totalInvoiceAmt = 0m;
            if (InvoiceInfoView.InvoiceInfoList != null
                && InvoiceInfoView.InvoiceInfoList.Count != 0)
            {
                for (int i = 0; i < InvoiceInfoView.InvoiceInfoList.Count; i++)
                {
                    totalInvoiceAmt += InvoiceInfoView.InvoiceInfoList[i].InvoiceAmount;
                }
                if (InvoiceInfoEntityView.InvoiceInputAmount != totalInvoiceAmt)
                    this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_InvoiceInputAmountNoEquels);
                else
                {
                    UpdateCode();
                }
            }
            else
                UpdateCode();
        }

        private void UpdateCode()
        {
            facade.UpdateEIMSInvoiceInput(InvoiceInfoEntityViewList, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result.IsSucceed)
                {
                    this.CurrentWindow.Alert(ResEIMSInvoiceEntry.Msg_EditSuccess);
                    this.Dialog.ResultArgs.isForce = true;
                    this.Dialog.Close();
                }
                else
                {
                    this.CurrentWindow.Alert(args.Result.Error);
                }
                InvoiceInfoEntityViewList.Clear();
                
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceInfoEntityViewList != null)
                this.InvoiceInfoEntityViewList.Clear();
            this.Dialog.ResultArgs.isForce = false;
            this.Dialog.Close();
        }
        #endregion

        #region 移除操作
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectView = this.dgOnlyOneInvoiceQueryResult.SelectedItem as EIMSInvoiceEntryVM;
            this.InvoiceInfoView.InvoiceInfoList.Remove(selectView);
            this.dgOnlyOneInvoiceQueryResult.ItemsSource = this.InvoiceInfoView.InvoiceInfoList;
        }
        #endregion
    }
}
