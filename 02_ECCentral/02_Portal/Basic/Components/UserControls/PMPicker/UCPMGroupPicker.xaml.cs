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
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Enum.Resources;
using System.Windows.Data;
using ECCentral.QueryFilter.IM.ProductManager;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Components.UserControls.PMPicker
{
    public partial class UCPMGroupPicker : UserControl
    {
        public PMGroupQueryFacade facade;
        public ProductManagerGroupQueryFilter queryFilter;
        List<ProductManagerGroupVM> itemList;

        public event EventHandler<EventArgs> PMGroupLoaded;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public UCPMGroupPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCPMGroupPicker_Loaded);
        }

        void UCPMGroupPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPMGroupPicker_Loaded);
            var exp = this.GetBindingExpression(UCPMGroupPicker.SelectedPMGroupSysNoProperty);
            if (exp != null
                && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbPMGroupList.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
                queryFilter = new ProductManagerGroupQueryFilter()
                {
                    Status = PMGroupStatus.Active,
                    PagingInfo = new QueryFilter.Common.PagingInfo()
                    {
                        SortBy = string.Empty,
                        PageSize = 0,
                        PageIndex = 0
                    }
                };
                facade = new PMGroupQueryFacade(CPApplication.Current.CurrentPage);
                BindComboxData();
            }
        }

        private static readonly DependencyProperty SelectedPMGroupSysNoProperty =
            DependencyProperty.Register("SelectedPMGroupSysNo", typeof(int?), typeof(UCPMGroupPicker), new PropertyMetadata(null, (s, e) =>
                {
                    var uc = s as UCPMGroupPicker;
                    uc.cmbPMGroupList.SelectedValue = (e.NewValue ?? "").ToString().Trim();
                }));
        private static readonly DependencyProperty SelectedPMGroupNameProperty =
            DependencyProperty.Register("SelectedPMGroupName", typeof(LanguageContent), typeof(UCPMGroupPicker), null);

        public int? SelectedPMGroupSysNo
        {
            get
            {
                return this.cmbPMGroupList.SelectedValue == null ? default(int?) : int.Parse(cmbPMGroupList.SelectedValue.ToString());
            }
            set
            {
                base.SetValue(SelectedPMGroupSysNoProperty, value);
            }
        }

        public LanguageContent SelectedPMGroupName
        {
            get
            {
                ProductManagerGroupInfo pmGroup = this.cmbPMGroupList.SelectedItem as ProductManagerGroupInfo;
                return pmGroup.PMGroupName;
            }
            set
            {
                base.SetValue(SelectedPMGroupNameProperty, value);
            }
        }

        private void BindComboxData()
        {
            facade.GetPMGroup((obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    itemList = DynamicConverter<ProductManagerGroupVM>.ConvertToVMList(args.Result.Rows);
                    itemList.Insert(0, (new ProductManagerGroupVM()
                    {
                        SysNo = null,
                        PMGroupName = ResCommonEnum.Enum_All
                    }));
                    this.cmbPMGroupList.ItemsSource = itemList;
                    this.cmbPMGroupList.SelectedIndex = 0;


                });
        }

        private void OnPMGroupLoaded()
        {
            var handler = PMGroupLoaded;
            if (handler != null)
            {
                EventArgs e = new EventArgs();
                handler(this, e);
            }
        }

        private void cmbPMGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                SelectionChanged(sender, e);
            }
            var control = sender as ComboBox;
            if (sender != null)
            {
                if (control.SelectedValue != null)
                {
                    var pmGroup = control.SelectedItem as ProductManagerGroupInfo;
                    if (pmGroup != null)
                    {
                        SelectedPMGroupName = pmGroup.PMGroupName;
                    }
                }
                else
                {
                    SelectedPMGroupName = null;
                }
            }
            else
            {
                SelectedPMGroupName = null;
            }
        }
    }
}
