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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCCustomerRange : UserControl
    {
        enum LimitType
        {
            NoLimit,
            CustomerRank,
            CustomerID
        }
                

        private LimitType PreLimitType = LimitType.NoLimit;

        private bool PreIsNoLimitArea = true;
        
        bool isLoaded = false;

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCCustomerRange()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCCustomerRange_Loaded);
        }

        void UCCustomerRange_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.IsCustomerNoLimit.Value)
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;
                PreLimitType = LimitType.NoLimit;
            }
            else if (vm.IsCustomerRank.Value)
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Visible;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;
                PreLimitType = LimitType.CustomerRank;
            }
            else if (vm.IsCustomerID.Value)
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Visible;
                PreLimitType = LimitType.CustomerID;
            }

            if (vm.IsAreaNoLimit.Value)
            {
                gridArea.Visibility = System.Windows.Visibility.Collapsed;
                PreIsNoLimitArea = true;
            }
            else
            {                
                gridArea.Visibility = System.Windows.Visibility.Visible;
                PreIsNoLimitArea = false;
            }

            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "不限";
            defaultItem.Tag = -1;
            defaultItem.IsSelected = false;
            cmbCustomerRank.Items.Add(defaultItem);

            foreach (var value in Enum.GetValues(typeof(CustomerRank)))
            {
                if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null)
                {
                    SimpleObjectViewModel sim = vm.CustomerCondition.RelCustomerRanks.CustomerRankList.FirstOrDefault(f => f.SysNo.Value == (int)value);
                    if (sim != null)
                    {
                        CustomerRank cre = (CustomerRank)Enum.Parse(typeof(CustomerRank), value.ToString(), true);
                        sim.Name = EnumConverter.GetDescription(cre);
                        continue;
                    }
                }
                ComboBoxItem item = new ComboBoxItem();
                CustomerRank k = (CustomerRank)Enum.Parse(typeof(CustomerRank), value.ToString(), true);
                item.Content = EnumConverter.GetDescription(k);
                item.Tag = value;
                item.IsSelected = false;
                cmbCustomerRank.Items.Add(item);
            }
            if (cmbCustomerRank.Items.Count > 0) cmbCustomerRank.SelectedIndex = 0;

            AreaQueryFacade areaFacade = new AreaQueryFacade();
            areaFacade.QueryProvinceAreaList((obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<AreaInfo> areaList = args.Result;
                    foreach (AreaInfo area in areaList)
                    {
                        if (vm.CustomerCondition.RelAreas.AreaList != null)
                        {
                            SimpleObjectViewModel sim =vm.CustomerCondition.RelAreas.AreaList.FirstOrDefault(f => f.SysNo == area.SysNo);
                            if (sim != null)
                            {
                                sim.Name = area.ProvinceName;
                                continue;
                            }
                        }

                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = area.ProvinceName;
                        item.Tag = area.SysNo;
                        item.IsSelected = false;
                        cmbArea.Items.Add(item);
                    }
                    if (cmbArea.Items.Count > 0) cmbArea.SelectedIndex = 0;
                });
             
            if (vm.IsOnlyViewMode)
            {
                OperationControlStatusHelper.SetControlsStatus(gridLayout, true);
            }

            isLoaded = true;
        }

        private void DataGridCheckBoxAllCustomerRank_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            CheckBox chk=(CheckBox)sender;
            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null)
            {
                vm.CustomerCondition.RelCustomerRanks.CustomerRankList.ForEach(f => f.IsChecked = chk.IsChecked);
            }
        }

        private void DataGridCheckBoxAllCustomer_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            if (vm.CustomerCondition.RelCustomers.CustomerIDList != null)
            {
                vm.CustomerCondition.RelCustomers.CustomerIDList.ForEach(f => f.IsChecked = chk.IsChecked);
            }
        }

        private void DataGridCheckBoxAllArea_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            if (vm.CustomerCondition.RelAreas.AreaList != null)
            {
                vm.CustomerCondition.RelAreas.AreaList.ForEach(f => f.IsChecked = chk.IsChecked);
            }
        }

        private void rdoCustomerNoLimit_Click(object sender, RoutedEventArgs e)
        {
            if (PreLimitType == LimitType.NoLimit)
            {
                return;
            }
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;

            if ((vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null && vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 0)
                || (vm.CustomerCondition.RelCustomers.CustomerIDList != null && vm.CustomerCondition.RelCustomers.CustomerIDList.Count > 0))
            {
                CurrentWindow.Confirm("如果设置为不限用户，那么已指定的用户组或自选用户都将全部移除，请确定是否设置为不限用户?", (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null)
                            {
                                vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Clear();
                                dgCustomerRank.ItemsSource = vm.CustomerCondition.RelCustomerRanks.CustomerRankList;
                                dgCustomerRank.Bind();
                            }
                            if (vm.CustomerCondition.RelCustomers.CustomerIDList != null)
                            {
                                vm.CustomerCondition.RelCustomers.CustomerIDList.Clear();
                                dgCustomerID.ItemsSource = vm.CustomerCondition.RelCustomers.CustomerIDList;
                                dgCustomerID.Bind();
                            }
                            
                            gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                            gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;

                            PreLimitType = LimitType.NoLimit;
                        }
                        else
                        {
                            switch (PreLimitType)
                            {
                                case LimitType.CustomerID:
                                    vm.IsCustomerID = true;
                                    vm.IsCustomerRank = false;
                                    vm.IsCustomerNoLimit = false;
                                    break;
                                case LimitType.CustomerRank:
                                    vm.IsCustomerID = false;
                                    vm.IsCustomerRank = true;
                                    vm.IsCustomerNoLimit = false;
                                    break;
                                case LimitType.NoLimit:
                                    vm.IsCustomerID = false;
                                    vm.IsCustomerRank = false;
                                    vm.IsCustomerNoLimit = true;
                                    break;
                            }
                            return;
                        }
                    });
            }
            else
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;
                PreLimitType = LimitType.NoLimit;
            }
        }

        private void rdoCustomerRank_Click(object sender, RoutedEventArgs e)
        {
            if (PreLimitType == LimitType.CustomerRank)
            {
                return;
            }

            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.IsExistThrowInTypeCouponCode.HasValue && vm.IsExistThrowInTypeCouponCode.Value)
            {
                CurrentWindow.Alert("已经存在投放型优惠券，因此客户条件只能是不限！如果需要修改客户条件，请先删除所有的投放型优惠券！");
                vm.IsCustomerID = false;
                vm.IsCustomerRank = false;
                vm.IsCustomerNoLimit = true;
                PreLimitType = LimitType.NoLimit;
                return;
            }

            if (vm.CustomerCondition.RelCustomers.CustomerIDList != null 
                && vm.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
            {
                CurrentWindow.Confirm("如果设置为指定用户组，那么已指定的自选用户都将全部移除，请确定是否设置为指定用户组?", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.CustomerCondition.RelCustomers.CustomerIDList.Clear();
                        dgCustomerID.ItemsSource = vm.CustomerCondition.RelCustomers.CustomerIDList;
                        dgCustomerID.Bind();

                        gridUserRankSetting.Visibility = System.Windows.Visibility.Visible;
                        gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;
                        PreLimitType = LimitType.CustomerRank;
                    }
                    else
                    {

                        switch (PreLimitType)
                        {
                            case LimitType.CustomerID:
                                vm.IsCustomerID = true;
                                vm.IsCustomerRank = false;
                                vm.IsCustomerNoLimit = false;
                                break;
                            case LimitType.CustomerRank:
                                vm.IsCustomerID = false;
                                vm.IsCustomerRank = true;
                                vm.IsCustomerNoLimit = false;
                                break;
                            case LimitType.NoLimit:
                                vm.IsCustomerID = false;
                                vm.IsCustomerRank = false;
                                vm.IsCustomerNoLimit = true;
                                break;
                        }
                        return;
                    }
                });
            }
            else
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Visible;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Collapsed;
                PreLimitType = LimitType.CustomerRank;
            }
        }

        private void rdoCustomerID_Click(object sender, RoutedEventArgs e)
        {
            if (PreLimitType == LimitType.CustomerID)
            {
                return;
            }

            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.IsExistThrowInTypeCouponCode.HasValue && vm.IsExistThrowInTypeCouponCode.Value)
            {
                CurrentWindow.Alert("已经存在投放型优惠券，因此客户条件只能是不限！如果需要修改客户条件，请先删除所有的投放型优惠券！");
                vm.IsCustomerID = false;
                vm.IsCustomerRank = false;
                vm.IsCustomerNoLimit = true;
                PreLimitType = LimitType.NoLimit;
                return;
            }
                        
            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null 
                && vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 0)
            {
                CurrentWindow.Confirm("如果设置为自选用户，那么已指定的用户组都将全部移除，请确定是否设置为自选用户?", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Clear();
                        dgCustomerRank.ItemsSource = vm.CustomerCondition.RelCustomerRanks.CustomerRankList;
                        dgCustomerRank.Bind();

                        gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                        gridUserIDSetting.Visibility = System.Windows.Visibility.Visible;
                        PreLimitType = LimitType.CustomerID;


                        cmbCustomerRank.Items.Clear();
                        foreach (var value in Enum.GetValues(typeof(CustomerRank)))
                        {
                            ComboBoxItem item = new ComboBoxItem();
                            CustomerRank k = (CustomerRank)Enum.Parse(typeof(CustomerRank), value.ToString(), true);
                            item.Content = EnumConverter.GetDescription(k);
                            item.Tag = value;
                            item.IsSelected = false;
                            cmbCustomerRank.Items.Add(item);
                        }
                        if (cmbCustomerRank.Items.Count > 0) cmbCustomerRank.SelectedIndex = 0;
                    }
                    else
                    {
                        switch (PreLimitType)
                        {
                            case LimitType.CustomerID:
                                vm.IsCustomerID = true;
                                vm.IsCustomerRank = false;
                                vm.IsCustomerNoLimit = false;
                                break;
                            case LimitType.CustomerRank:
                                vm.IsCustomerID = false;
                                vm.IsCustomerRank = true;
                                vm.IsCustomerNoLimit = false;
                                break;
                            case LimitType.NoLimit:
                                vm.IsCustomerID = false;
                                vm.IsCustomerRank = false;
                                vm.IsCustomerNoLimit = true;
                                break;
                        }
                        return;
                    }
                });
            }
            else
            {
                gridUserRankSetting.Visibility = System.Windows.Visibility.Collapsed;
                gridUserIDSetting.Visibility = System.Windows.Visibility.Visible;
                PreLimitType = LimitType.CustomerID;
            }
        }

        private void btnAddCustomerRank_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = cmbCustomerRank.SelectedItem as ComboBoxItem;
            if (item == null) return;
            int value = (int)item.Tag;
            string name = item.Content.ToString();
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList == null) vm.CustomerCondition.RelCustomerRanks.CustomerRankList = new List<SimpleObjectViewModel>();
            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList.FirstOrDefault(f => f.SysNo.Value == value) == null)
            { 
                vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Add(new SimpleObjectViewModel()
                {
                    IsChecked = false,
                    SysNo = value,
                    Name = name
                });
                dgCustomerRank.ItemsSource = vm.CustomerCondition.RelCustomerRanks.CustomerRankList;
                dgCustomerRank.Bind();

                cmbCustomerRank.Items.Remove(item);
                if (cmbCustomerRank.Items.Count > 0) cmbCustomerRank.SelectedIndex = 0;
            }
        }

        private void btnRemoveCustomerRank_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel; 
            if (vm.CustomerCondition.RelCustomerRanks.CustomerRankList != null)
            {
                List<SimpleObjectViewModel> removedList = new List<SimpleObjectViewModel>();
                foreach (SimpleObjectViewModel sim in vm.CustomerCondition.RelCustomerRanks.CustomerRankList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.CustomerCondition.RelCustomerRanks.CustomerRankList.Remove(f));
                    dgCustomerRank.ItemsSource = vm.CustomerCondition.RelCustomerRanks.CustomerRankList;
                    dgCustomerRank.Bind();

                    foreach (SimpleObjectViewModel area in removedList)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = area.Name;
                        item.Tag = area.SysNo;
                        item.IsSelected = false;
                        cmbCustomerRank.Items.Insert(0, item);
                    }

                    if (cmbCustomerRank.Items.Count > 0) cmbCustomerRank.SelectedIndex = 0;
                }
            }
            
        }

        private void btnAddCustomerID_Click(object sender, RoutedEventArgs e)
        {
            UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
            ucCustomerSearch.SelectionMode = SelectionMode.Multiple;
            ucCustomerSearch.DialogHandle = CurrentWindow.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
                        if (vm.CustomerCondition.RelCustomers.CustomerIDList == null) vm.CustomerCondition.RelCustomers.CustomerIDList = new List<CustomerAndSendViewModel>();
            
                        List<CustomerVM> selectedList = args.Data as List<CustomerVM>;
                        foreach (CustomerVM cv in selectedList)
                        {
                            vm.CustomerCondition.RelCustomers.CustomerIDList.Add(new CustomerAndSendViewModel()
                            {
                                CustomerSysNo = cv.SysNo,
                                CustomerID = cv.CustomerID,
                                CustomerName = cv.CustomerName,
                                IsChecked = false
                            });
                        }
                        dgCustomerID.ItemsSource = vm.CustomerCondition.RelCustomers.CustomerIDList;
                        dgCustomerID.Bind();
                    }
                });
        }

        private void btnRemoveCustomerID_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.CustomerCondition.RelCustomers.CustomerIDList != null)
            {
                List<CustomerAndSendViewModel> removedList = new List<CustomerAndSendViewModel>();
                foreach (CustomerAndSendViewModel sim in vm.CustomerCondition.RelCustomers.CustomerIDList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.CustomerCondition.RelCustomers.CustomerIDList.Remove(f));
                    dgCustomerID.ItemsSource = vm.CustomerCondition.RelCustomers.CustomerIDList;
                    dgCustomerID.Bind();
                }
            }
        }


        private void btnAddArea_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = cmbArea.SelectedItem as ComboBoxItem;
            if (item == null) return;
            int value = (int)item.Tag;
            string name = item.Content.ToString();
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.CustomerCondition.RelAreas.AreaList == null) vm.CustomerCondition.RelAreas.AreaList = new List<SimpleObjectViewModel>();
            if (vm.CustomerCondition.RelAreas.AreaList.FirstOrDefault(f => f.SysNo.Value == value) == null)
            {
                vm.CustomerCondition.RelAreas.AreaList.Add(new SimpleObjectViewModel()
                {
                    IsChecked = false,
                    SysNo = value,
                    Name = name
                });
                dgArea.ItemsSource = vm.CustomerCondition.RelAreas.AreaList;
                dgArea.Bind();

                cmbArea.Items.Remove(item);
                if (cmbArea.Items.Count > 0) cmbArea.SelectedIndex = 0;
            }
        }

        private void btnRemoveArea_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            if (vm.CustomerCondition.RelAreas.AreaList != null)
            {
                List<SimpleObjectViewModel> removedList = new List<SimpleObjectViewModel>();
                foreach (SimpleObjectViewModel sim in vm.CustomerCondition.RelAreas.AreaList)
                {
                    if (sim.IsChecked.Value)
                    {
                        removedList.Add(sim);
                    }
                }
                if (removedList.Count == 0)
                {
                    this.CurrentWindow.Alert("请先至少选中一条记录!");
                }
                else
                {
                    removedList.ForEach(f => vm.CustomerCondition.RelAreas.AreaList.Remove(f));
                    dgArea.ItemsSource = vm.CustomerCondition.RelAreas.AreaList;
                    dgArea.Bind();

                    foreach (SimpleObjectViewModel area in removedList)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = area.Name;
                        item.Tag = area.SysNo;
                        item.IsSelected = false;
                        cmbArea.Items.Insert(0,item);
                    }
                    if (cmbArea.Items.Count > 0) cmbArea.SelectedIndex = 0;
                }
            }
        }

        private void rdoAreaNoLimit_Click(object sender, RoutedEventArgs e)
        {
            if (PreIsNoLimitArea)
            {
                PreIsNoLimitArea = true;
                return;
            }

            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            
            if (vm.CustomerCondition.RelAreas.AreaList != null && vm.CustomerCondition.RelAreas.AreaList.Count > 0)
            {
                CurrentWindow.Confirm("如果设置为不限地区，那么已添加的地区将全部移除，请确定是否设置为不限地区?", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        vm.CustomerCondition.RelAreas.AreaList.Clear();
                        dgArea.ItemsSource = vm.CustomerCondition.RelAreas.AreaList;
                        dgArea.Bind();

                        gridArea.Visibility = System.Windows.Visibility.Collapsed;
                        PreIsNoLimitArea = true;
                    }
                    else
                    {
                        if (PreIsNoLimitArea)
                        {
                            gridArea.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            gridArea.Visibility = System.Windows.Visibility.Visible;
                        }

                        return;
                    }
                });
            }
            else
            {
                gridArea.Visibility = System.Windows.Visibility.Collapsed;
                PreIsNoLimitArea = true;
            }
        }

        private void rdoAreaLimit_Click(object sender, RoutedEventArgs e)
        {
            gridArea.Visibility = System.Windows.Visibility.Visible;
            PreIsNoLimitArea = false;
        }

       

       
    }
}
