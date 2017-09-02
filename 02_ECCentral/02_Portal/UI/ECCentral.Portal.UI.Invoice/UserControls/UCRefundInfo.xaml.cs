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
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCRefundInfo : UserControl
    {
        private Dictionary<RefundPayType, List<Control>> controlsEnableMap;
        private List<Control> shouldBeControledList;
        private Brush enableBrush;
        private Brush readonlyBrush;

        public UCRefundInfo()
        {
            InitializeComponent();
            Init();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            cmbRefundPayType.SelectionChanged += new SelectionChangedEventHandler(cmbRefundPayType_SelectionChanged);
            Loaded += new RoutedEventHandler(UCRefundInfo_Loaded);
        }

        private void UCRefundInfo_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCRefundInfo_Loaded);
            EnableControl(null);
        }

        private void Init()
        {
            controlsEnableMap = new Dictionary<RefundPayType, List<Control>>();
            //邮政退款
            controlsEnableMap.Add(RefundPayType.PostRefund, new List<Control>()
            {
                tbPostAddress,tbPostCode,tbCashReceiver
            });
            //银行转账
            controlsEnableMap.Add(RefundPayType.BankRefund, new List<Control>()
            {
                tbBankName,tbBranchBankName,tbCardNumber,tbCardOwnerName
            });
            //网关直接退款
            controlsEnableMap.Add(RefundPayType.NetWorkRefund, new List<Control>()
            {
                tbBankName
            });

            shouldBeControledList = new List<Control>()
            {
                tbPostAddress, tbPostCode, tbCashReceiver, tbBankName, tbBranchBankName
                , tbCardNumber, tbCardOwnerName
            };
            enableBrush = new SolidColorBrush(Color.FromArgb(255, 245, 245, 220));
            readonlyBrush = new SolidColorBrush(Color.FromArgb(255, 247, 250, 253));
        }

        private void cmbRefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                KeyValuePair<RefundPayType?, string> selectedItem = (KeyValuePair<RefundPayType?, string>)e.AddedItems[0];
                if (selectedItem.Key != null)
                {
                    var dataContext = this.DataContext as RefundInfoVM;
                    dataContext.ValidationErrors.Clear();

                    List<Control> controls = null;
                    if (controlsEnableMap.ContainsKey(selectedItem.Key.Value))
                    {
                        controls = new List<Control>(controlsEnableMap[selectedItem.Key.Value]);
                    }
                    EnableControl(controls);
                }
            }
        }

        private void EnableControl(List<Control> enableControls)
        {
            shouldBeControledList.ForEach(c =>
            {
                c.IsEnabled = false;
                c.Background = readonlyBrush;
            });
            if (enableControls != null)
            {
                enableControls.ForEach(c =>
                {
                    c.IsEnabled = true;
                    c.Background = enableBrush;
                });
            }
        }

        public void HideRefundCashAmtAndToleranceAmt()
        {
            tblRefundCashAmt.Visibility = Visibility.Collapsed;
            tbRefundCashAmt.Visibility = Visibility.Collapsed;

            tblToleranceAmt.Visibility = Visibility.Collapsed;
            tbToleranceAmt.Visibility = Visibility.Collapsed;
        }

        public void SetColumnWidth(int colIndex, double width)
        {
            LayoutRoot.ColumnDefinitions[colIndex].Width = new GridLength(width, GridUnitType.Pixel);
        }

        public void DisableRefundPayType()
        {
            cmbRefundPayType.IsEnabled = false;
        }
    }
}