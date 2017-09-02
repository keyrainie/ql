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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class CommissionItemDetailView : UserControl
    {
        public VendorCommissionItemType type;
        public CommissionItemVM m_vm;

        public CommissionItemDetailView(CommissionItemVM vm, VendorCommissionItemType type)
        {
            InitializeComponent();
            m_vm = vm;
            this.type = type;
            this.Loaded += new RoutedEventHandler(CommissionItemDetailView_Loaded);
        }

        void CommissionItemDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(CommissionItemDetailView_Loaded);
            BindGrid();
        }

        private void BindGrid()
        {
            switch (type)
            {
                case VendorCommissionItemType.SAC:
                    this.lblSummary.Text = string.Format(ResCommissionItemView.Label_SaleSummayFormatString, m_vm.ManufacturerAndCategoryName, m_vm.TotalSaleAmt.Value.ToString("f2"));
                    this.ViewGrid.ItemsSource = m_vm.DetailList;
                    break;
                case VendorCommissionItemType.SOC:
                    HideColumns();
                    if (m_vm.DetailOrderList != null)
                    {
                        this.lblSummary.Text = string.Format(ResCommissionItemView.Label_OrderSummayFormatString
                                                , m_vm.ManufacturerAndCategoryName
                                                , m_vm.DetailOrderList.Count(i => i.ReferenceType == VendorCommissionReferenceType.SO)
                                                , m_vm.DetailOrderList.Count(i => i.ReferenceType == VendorCommissionReferenceType.RMA));
                        this.ViewGrid.ItemsSource = m_vm.DetailOrderList;
                    }
                    break;
                case VendorCommissionItemType.DEF:
                    HideColumns();
                    if (m_vm.DetailDeliveryList != null)
                    {
                        this.lblSummary.Text = string.Format(ResCommissionItemView.Label_ShipSummayFormatString, m_vm.ManufacturerAndCategoryName, m_vm.DetailDeliveryList.Count);
                        this.ViewGrid.ItemsSource = m_vm.DetailDeliveryList;
                    }
                    break;
                default:
                    break;
            }
        }

        private void HideColumns()
        {
            ViewGrid.Columns[4].Visibility = Visibility.Collapsed;
            ViewGrid.Columns[5].Visibility = Visibility.Collapsed;
            ViewGrid.Columns[6].Visibility = Visibility.Collapsed;
            ViewGrid.Columns[7].Visibility = Visibility.Collapsed;
            ViewGrid.Columns[8].Visibility = Visibility.Collapsed;
            ViewGrid.Columns[9].Visibility = Visibility.Collapsed;
        }
    }
}
