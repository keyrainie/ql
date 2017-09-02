using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.UserControls.PayTypePicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCPayInfo : UserControl
    {
        public UCPayInfo()
        {
            InitializeComponent();
            LayoutRoot.KeyDown += new System.Windows.Input.KeyEventHandler(LayoutRoot_KeyDown);
            btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
        }

        private void LayoutRoot_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
                    OnSearchButtonClick(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            }
        }

        public event RoutedEventHandler SearchOrderButtonClick = null;

        protected void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            var handler = SearchOrderButtonClick;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            OnSearchButtonClick(sender, e);
        }

        public void HideRelatedSOSysNo()
        {
            tblRelatedSOSysNo.Visibility = Visibility.Collapsed;
            tbRelatedSOSysNo.Visibility = Visibility.Collapsed;
        }

        public void SetColumnWidth(int colIndex, double width)
        {
            LayoutRoot.ColumnDefinitions[colIndex].Width = new GridLength(width, GridUnitType.Pixel);
        }
    }
}