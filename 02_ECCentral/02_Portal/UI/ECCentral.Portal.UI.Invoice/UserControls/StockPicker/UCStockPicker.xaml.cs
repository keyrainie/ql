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

namespace ECCentral.Portal.UI.Invoice.UserControls.StockPicker
{
    public partial class UCStockPicker : UserControl
    {
        /// <summary>
        /// StockSysNo字符串，多个SysNo之间用逗号（,）隔开
        /// </summary>
        public string StockSysNo
        {
            get
            {
                return (string)GetValue(StockSysNoProperty);
            }
            set
            {
                SetValue(StockSysNoProperty, value);
            }
        }

        public static readonly DependencyProperty StockSysNoProperty =
            DependencyProperty.Register("StockSysNoString", typeof(string), typeof(UCStockPicker), new PropertyMetadata("", (s, e) =>
            {
                var uc = s as UCStockPicker;
                uc.StockSysNo = (e.NewValue ?? "").ToString().Trim();
            }));

        public UCStockPicker()
        {
            InitializeComponent();
        }

        private void btnSearchStock_Click(object sender, RoutedEventArgs e)
        {
            new StockSearch(StockSysNo).ShowDialog(ResStockPicker.Message_Stock, (obj, args) =>
            {
                var data = args.Data as List<StockCheckBoxVM>;
                if (data != null)
                {
                    tbStock.Text = string.Join(",", data.Select(s => s.StockName).ToList());
                    StockSysNo = string.Join(",", data.Select(s => s.StockSysNo).ToList());
                }
            }, new Size(450, 250));
        }
    }
}