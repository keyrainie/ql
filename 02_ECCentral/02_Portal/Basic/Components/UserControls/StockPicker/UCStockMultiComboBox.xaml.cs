using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Service.Utility;


namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public partial class UCStockMultiComboBox : UserControl
    {
        private static readonly DependencyProperty SelectedWebChannelIDProperty =
                DependencyProperty.Register("SelectedWebChannelID", typeof(string), typeof(UCStockMultiComboBox), new PropertyMetadata(null));


        private static readonly DependencyProperty StockBlankItemModeProperty =
                DependencyProperty.Register("StockBlankItemTypeType", typeof(object), typeof(UCStockMultiComboBox), new PropertyMetadata(null));

        private static readonly DependencyProperty SelectedStockSysNoProperty =
        DependencyProperty.Register("SelectedStockSysNo", typeof(ObservableCollection<int>), typeof(UCStockMultiComboBox), new PropertyMetadata(null, (obj, args) =>
        {
            if (args.NewValue != null)
            {
                UCStockMultiComboBox uc = (UCStockMultiComboBox)obj;
                uc.SetCheckedStatus();
            }
        }));

        public ObservableCollection<int> SelectedStockSysNo
        {
            get
            {
                var selectedStockSysNo  = (ObservableCollection<int>)GetValue(SelectedStockSysNoProperty);
                if (selectedStockSysNo == null)
                {
                    selectedStockSysNo = new ObservableCollection<int>();
                    SelectedStockSysNo = selectedStockSysNo;
                }
                return selectedStockSysNo;
            }
            set
            {
                base.SetValue(SelectedStockSysNoProperty, value);
            }
        }

        public string SelectedWebChannelID
        {
            get
            {
                return (string)GetValue(SelectedWebChannelIDProperty);
            }
            set
            {
                base.SetValue(SelectedWebChannelIDProperty, value);
            }
        }

        public ComboBoxBlankItemMode? StockBlankItemMode
        {
            get
            {
                return ((ComboBoxBlankItemMode?)base.GetValue(StockBlankItemModeProperty));
            }
            set
            {
                base.SetValue(StockBlankItemModeProperty, value);
            }
        }

        private MultiStockComboVM m_dataContext;
        private MultiStockComboVM ViewModel
        {
            get
            {
                if (m_dataContext == null)
                {
                    m_dataContext = new MultiStockComboVM();
                }
                return m_dataContext;
            }
            set
            {
                m_dataContext = value;
            }
        }

        private StockQueryFacade m_Facade = null;
        private StockQueryFacade Facade
        {
            get
            {
                if (m_Facade == null)
                {
                    m_Facade = new StockQueryFacade();
                }
                return m_Facade;
            }
        }

        private bool preOperIsOpen = false;

        public UCStockMultiComboBox()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCStockComboBox_Loaded);
        }

        private void UCStockComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCStockComboBox_Loaded);

            var exp1 = this.GetBindingExpression(UCStockMultiComboBox.SelectedWebChannelIDProperty);

            if (exp1 != null && exp1.ParentBinding != null)
            {
                string path = exp1.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbWebChannel.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            this.tbSelectedText.Text = ResStockPicker.ComboBox_PleaseSelect;
            LoadWebChannelList();
        }

        private void LoadWebChannelList()
        {
            ViewModel.WebChannelList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
            ViewModel.WebChannelList.Insert(0, new WebChannelVM { ChannelID = null, SysNo = null, ChannelName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_Select });

            cmbWebChannel.SelectedValue = ViewModel.WebChannelID;
            cmbWebChannel.ItemsSource = ViewModel.WebChannelList;
        }

        private void cmbWebChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != cmbWebChannel.SelectedValue)
            {
                if (!string.IsNullOrEmpty(cmbWebChannel.SelectedValue.ToString()))
                {
                    string webChannelID = cmbWebChannel.SelectedValue.ToString();
                    Facade.QueryStockListByWebChannel(webChannelID, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        ViewModel.StockList = args.Result.Convert<StockInfo, StockVM>();
                        ViewModel.StockList.ForEach(item => item.PropertyChanged += item_PropertyChanged);

                        cmbStockList.ItemsSource = ViewModel.StockList;
                    });
                }
            }

        }

        private void cmbStockList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStockList.SelectedItem = null;
            SetString();
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            cmbStockList.SelectedItem = null;
            StockVM itemVM = (StockVM)sender;
            if (itemVM.SysNo.HasValue)
            {
                if (itemVM.IsChecked && !SelectedStockSysNo.Any(f => f == itemVM.SysNo))
                {
                    SelectedStockSysNo.Add(itemVM.SysNo.Value);
                }
                else if (!itemVM.IsChecked && SelectedStockSysNo.Any(f => f == itemVM.SysNo))
                {
                    SelectedStockSysNo.Remove(itemVM.SysNo.Value);
                }
            }
            SetString();
        }

        private void SetCheckedStatus()
        {
            if (SelectedStockSysNo != null)
            {
                List<StockVM> result = cmbStockList.ItemsSource as List<StockVM>;
                if (result != null)
                {
                    var clone = SelectedStockSysNo.DeepCopy();
                    result.ForEach(f => f.IsChecked = false);
                    foreach (var itemValue in clone)
                    {
                        result.Where(w => w.SysNo == itemValue).ForEach(x => x.IsChecked = true);
                    }
                }
            }
        }

        public void SetString()
        {
            string selectionString = "";
            int selectedItemCount = 0;
            foreach (StockVM item in cmbStockList.ItemsSource)
            {
                if (item.IsChecked)
                {
                    selectedItemCount++;
                    selectionString = selectionString + item.StockName + ",";
                }
            }
            if (selectionString != "" && selectedItemCount > 0)
            {
                selectionString = selectionString.Substring(0, selectionString.Length - 1);
            }
            else
            {
                selectionString = ResStockPicker.ComboBox_PleaseSelect;
            }
            tbSelectedText.Text = selectionString;

            Size tbSelectedTextArea = new Size(tbSelectedText.ActualWidth, tbSelectedText.ActualHeight);
            tbSelectedText.Measure(tbSelectedTextArea);
            if (tbSelectedTextArea.Width > cmbStockList.ActualWidth - 20)
            {
                tbSelectedText.Text = string.Format("已选择{0}项", selectedItemCount);
            }
        }


        private void tbSelectedText_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (preOperIsOpen)
            {
                cmbStockList.IsDropDownOpen = false;
                preOperIsOpen = false;
            }
        }

        private void tbSelectedText_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!preOperIsOpen)
            {
                cmbStockList.IsDropDownOpen = true;
                preOperIsOpen = true;
            }
        }


    }
}
