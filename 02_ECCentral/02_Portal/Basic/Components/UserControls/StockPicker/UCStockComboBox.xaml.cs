using System;
using System.Collections.Generic;
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

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public partial class UCStockComboBox : UserControl
    {
        private static readonly DependencyProperty SelectedWebChannelIDProperty =
                DependencyProperty.Register("SelectedWebChannelID", typeof(string), typeof(UCStockComboBox), new PropertyMetadata(null));

        private static readonly DependencyProperty SelectedStockSysNoProperty =
                DependencyProperty.Register("SelectedStockSysNo", typeof(int?), typeof(UCStockComboBox), new PropertyMetadata(null));

        private static readonly DependencyProperty SelectedStockNameProperty =
                DependencyProperty.Register("SelectedStockName", typeof(string), typeof(UCStockComboBox), new PropertyMetadata(null));

        private static readonly DependencyProperty StockBlankItemModeProperty =
                DependencyProperty.Register("StockBlankItemTypeType", typeof(object), typeof(UCStockComboBox), new PropertyMetadata(null));

        private static readonly DependencyProperty MerchantSysNoProperty =
                DependencyProperty.Register("MerchantSysNoProperty", typeof(int?), typeof(UCStockComboBox), new PropertyMetadata(null));

        public event EventHandler<EventArgs> cmbStockListSelectionChanged;

        public int SelectedIndex
        {
            set {
                this.cmbStockList.SelectedIndex = value;
            }
        }

        public int? SelectedStockSysNo
        {
            get
            {
                return (int?)GetValue(SelectedStockSysNoProperty);
            }
            set
            {
                base.SetValue(SelectedStockSysNoProperty, value);
            }
        }

        public int? MerchantSysNo
        {
            get
            {
                return (int?)GetValue(MerchantSysNoProperty);
            }
            set
            {
                base.SetValue(MerchantSysNoProperty, value);
            }
        }

        public string SelectedStockName
        {
            get
            {
                return (string)GetValue(SelectedStockNameProperty);
            }
            set
            {
                base.SetValue(SelectedStockNameProperty, value);
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

        private StockComboVM m_dataContext;
        private StockComboVM ViewModel
        {
            get
            {
                if (m_dataContext == null)
                {
                    m_dataContext = new StockComboVM();
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

        public UCStockComboBox()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCStockComboBox_Loaded);
        }

        private void UCStockComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCStockComboBox_Loaded);

            var exp1 = this.GetBindingExpression(UCStockComboBox.SelectedWebChannelIDProperty);

            if (exp1 != null && exp1.ParentBinding != null)
            {
                string path = exp1.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbWebChannel.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            var exp2 = this.GetBindingExpression(UCStockComboBox.SelectedStockSysNoProperty);

            if (exp2 != null && exp2.ParentBinding != null)
            {
                string path = exp2.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbStockList.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            BindData();
        }

        public void BindData()
        {
            LoadWebChannelList();

            if (!this.SelectedStockSysNo.HasValue)
            {
                cmbWebChannel.SelectedValue = ViewModel.WebChannelID;
            }
            else
            {
                ViewModel.StockSysNo = this.SelectedStockSysNo;
                Facade.LoadStockBySysNo((int)ViewModel.StockSysNo, OnLoadStockBySysNo);
            }
        }

        private void LoadWebChannelList()
        {
            ViewModel.WebChannelList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
            ViewModel.WebChannelList.Insert(0, new WebChannelVM { ChannelID = null, SysNo = null, ChannelName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_Select });
            cmbWebChannel.ItemsSource = ViewModel.WebChannelList;
        }

        private void OnLoadStockBySysNo(object sender, RestClientEventArgs<dynamic> args)
        {
            dynamic totalCount = args.Result.TotalCount;
            if (totalCount > 0)
            {
                //渠道库存系统编号只存在一个
                StockVM selectedStock = DynamicConverter<StockVM>.ConvertToVM(args.Result.Rows[0]);

                ViewModel.WebChannelID = selectedStock.WebChannelSysNo.ToString();
                ViewModel.StockSysNo = selectedStock.SysNo;
                ViewModel.StockName = selectedStock.StockName;


            }
            else
            {
                //无效的渠道仓库
                ViewModel.WebChannelID = "1";
                ViewModel.StockSysNo = null;
                ViewModel.StockName = null;
            }

            cmbWebChannel.SelectedValue = ViewModel.WebChannelID;
        }

        private void cmbWebChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != cmbWebChannel.SelectedValue)
            {
                if (!string.IsNullOrEmpty(cmbWebChannel.SelectedValue.ToString()))
                {
                    string webChannelID = cmbWebChannel.SelectedValue.ToString();
                    Facade.QueryStockListByChannelAndMerchant(MerchantSysNo, webChannelID, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        ViewModel.StockList = args.Result.Convert<StockInfo, StockVM>();

                        StockVM blankInfo = new StockVM()
                        {
                            SysNo = null,
                            StockName = ResStockPicker.ComboBox_AllItem
                        };

                        if (this.StockBlankItemMode == ComboBoxBlankItemMode.PleaseSelect)
                        {
                            blankInfo.StockName = ResStockPicker.ComboBox_PleaseSelect;
                        }
                        ViewModel.StockList.Insert(0, blankInfo);
                        cmbStockList.ItemsSource = ViewModel.StockList;
                    });
                }
            }

        }

        private void cmbStockList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StockVM selectedStock = cmbStockList.SelectedItem as StockVM;
            if (selectedStock != null && selectedStock.SysNo != null)
            {
                this.SelectedStockName = selectedStock.StockName;
                this.SelectedStockSysNo = selectedStock.SysNo;
            }
            else
            {
                this.SelectedStockName = string.Empty;
                this.SelectedStockSysNo = null;
            }
            var handler = cmbStockListSelectionChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
