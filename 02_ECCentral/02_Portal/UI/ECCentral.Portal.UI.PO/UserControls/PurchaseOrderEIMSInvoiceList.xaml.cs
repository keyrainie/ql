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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderEIMSInvoiceList : UserControl
    {

        public PurchaseOrderFacade serviceFacade;
        public PurchaseOrderQueryFilter queryFilter;
        public int? VendorSysNo { get; set; }
        public int? PMSysNo { get; set; }
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

        public PurchaseOrderEIMSInvoiceList(int vendorSysNo)
        {
            InitializeComponent();
            this.VendorSysNo = vendorSysNo;
            //this.PMSysNo = pmSysNo;
            this.Loaded += new RoutedEventHandler(PurchaseOrderEIMSInvoiceList_Loaded);
        }

        void PurchaseOrderEIMSInvoiceList_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderEIMSInvoiceList_Loaded;
            queryFilter = new PurchaseOrderQueryFilter();
            serviceFacade = new PurchaseOrderFacade(CurrentPage);
            LoadAvailableEIMSList();
        }

        private void LoadAvailableEIMSList()
        {
            this.gridEIMSInvoiceList.Bind();
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.gridEIMSInvoiceList.ItemsSource)
                {
                    foreach (var item in this.gridEIMSInvoiceList.ItemsSource)
                    {
                        if (item is EIMSInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((EIMSInfoVM)item).IsChecked)
                                {
                                    ((EIMSInfoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((EIMSInfoVM)item).IsChecked)
                                {
                                    ((EIMSInfoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void gridEIMSInvoiceList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.VendorSysNo = VendorSysNo.Value.ToString();
            //queryFilter.PMSysNo = PMSysNo.Value.ToString();
            serviceFacade.QueryPurchaseOrderEIMSInvoiceInfo(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<EIMSInfoVM> EIMSList = new List<EIMSInfoVM>();
                EIMSList = DynamicConverter<EIMSInfoVM>.ConvertToVMList(args.Result.Rows);
                this.gridEIMSInvoiceList.ItemsSource = EIMSList;
            });
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //确定操作:
            List<EIMSInfoVM> list = this.gridEIMSInvoiceList.ItemsSource as List<EIMSInfoVM>;
            if (null != list && 0 < list.Count)
            {
                list = list.Where(x => x.IsChecked).ToList();
                Dialog.ResultArgs.Data = list;
            }
            else
            {
                Dialog.ResultArgs.Data = null;
            }
            Dialog.Close(true);

        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作 ：
            Dialog.ResultArgs.Data = null;
            Dialog.Close(true);
        }
    }
}
