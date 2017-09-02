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

using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Models.VendorUserInfo;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal
{
    public partial class UCProductCheck : UserControl
    {
        #region 初始化
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

        VendorProductQueryFilter fitler;

        public VendorProductQueryFilter Fitler
        {
            get { return fitler; }
            set { fitler = value; }
        }

        VendorFacade m_facade;

        List<VendorProductVM> vendorSettedProductList;

        List<VendorProductVM> vendorUnSetProductList;

        bool isFirst = true;

        public UCProductCheck(VendorProductQueryFilter filter)
        {
            InitializeComponent();
            this.Fitler = filter;
            this.spAutoMgmt.DataContext = this.Fitler;
            this.isFirst = false;
            m_facade = new VendorFacade();
            Loaded += new RoutedEventHandler(UCProductCheck_Loaded);
        }
        #endregion

        #region 页面加载
        void UCProductCheck_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Fitler.IsAuto)
            {
                this.tcProduct.IsHitTestVisible = false;
                this.btnCancelAll.Visibility = this.btnCancelSelected.Visibility = System.Windows.Visibility.Collapsed;
            }
            this.dgSettedProducts.Bind();
            this.dgUnSetProducts.Bind();
        }

        private void dgSettedProducts_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.Fitler.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            this.Fitler.IsMapping = true;
            m_facade.QueryVendorProduct(this.Fitler, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.dgSettedProducts.TotalCount = args.Result.TotalCount;
                    vendorSettedProductList = DynamicConverter<VendorProductVM>.ConvertToVMList(args.Result.Rows.ToList("IsCheck", false));
                    this.dgSettedProducts.ItemsSource = vendorSettedProductList;
                });
        }

        private void dgUnSetProducts_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.Fitler.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            this.Fitler.IsMapping = false;
            m_facade.QueryVendorProduct(this.Fitler, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                this.dgUnSetProducts.TotalCount = args.Result.TotalCount;
                vendorUnSetProductList = DynamicConverter<VendorProductVM>.ConvertToVMList(args.Result.Rows.ToList("IsCheck", false));
                this.dgUnSetProducts.ItemsSource = vendorUnSetProductList;
            });
        }
        #endregion

        #region 全选
        private void UnDataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dgUnSetProducts.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dgSettedProducts.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }
        #endregion

        //取消设置选中项
        private void btnCancelSelected_Click(object sender, RoutedEventArgs e)
        {
            if (vendorSettedProductList == null || vendorSettedProductList.Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }
            int flag = 0;
            foreach (var list in vendorSettedProductList)
            {
                if (list.IsCheck)
                {
                    flag++;
                    break;
                }
            }
            if (flag == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }
            VendorProductListVM productList = new VendorProductListVM();
            productList.IsAuto = this.Fitler.IsAuto ? 1 : 0;
            productList.UserSysNo = this.Fitler.UserSysNo;
            productList.VendorSysNo = this.Fitler.VendorSysNo;
            productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
            productList.CancelSetProductSysNoList = new List<int>();

            foreach (var settedList in vendorSettedProductList)
            {
                if (settedList.IsCheck)
                    productList.CancelSetProductSysNoList.Add(settedList.SysNo);
            }
            m_facade.UpdateVendorProduct(productList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                    this.dgSettedProducts.Bind();
                    this.dgUnSetProducts.Bind();
                }
            });
        }

        //取消设置所有
        private void btnCancelAll_Click(object sender, RoutedEventArgs e)
        {
            VendorProductListVM productList = new VendorProductListVM();
            productList.IsAuto = this.Fitler.IsAuto ? 1 : 0;
            productList.UserSysNo = this.Fitler.UserSysNo;
            productList.VendorSysNo = this.Fitler.VendorSysNo;
            productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
            productList.SetAndCancelAll = false;
            m_facade.UpdateVendorProduct(productList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                    this.dgSettedProducts.Bind();
                    this.dgUnSetProducts.Bind();
                }
            });
            this.tcProduct.SelectedIndex = 1;
        }

        //设置所有
        private void btnSetAllUnSet_Click(object sender, RoutedEventArgs e)
        {
            VendorProductListVM productList = new VendorProductListVM();
            productList.IsAuto = this.Fitler.IsAuto ? 1 : 0;
            productList.UserSysNo = this.Fitler.UserSysNo;
            productList.VendorSysNo = this.Fitler.VendorSysNo;
            productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
            productList.VendorManufacturerSysNo = this.Fitler.VendorManufacturerSysNo;
            productList.C2SysNo = this.Fitler.C2SysNo;
            productList.C3SysNo = this.Fitler.C3SysNo;
            productList.SetAndCancelAll = true;
            m_facade.UpdateVendorProduct(productList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                    this.dgSettedProducts.Bind();
                    this.dgUnSetProducts.Bind();
                }
            });
            this.tcProduct.SelectedIndex = 0;
        }

        //设置选中项
        private void btnSetSelected_Click(object sender, RoutedEventArgs e)
        {
            if (vendorUnSetProductList == null || vendorUnSetProductList.Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }
            int flag = 0;
            foreach (var list in vendorUnSetProductList)
            {
                if (list.IsCheck)
                {
                    flag++;
                    break;
                }
            }
            if (flag == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_PleaseSelect);
                return;
            }
            VendorProductListVM productList = new VendorProductListVM();
            productList.IsAuto = this.Fitler.IsAuto ? 1 : 0;
            productList.UserSysNo = this.Fitler.UserSysNo;
            productList.VendorSysNo = this.Fitler.VendorSysNo;
            productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
            productList.C2SysNo = this.Fitler.C2SysNo;
            productList.C3SysNo = this.Fitler.C3SysNo;
            productList.SetProductSysNoList = new List<int>();
            foreach (var settedList in vendorUnSetProductList)
            {
                if (settedList.IsCheck)
                    productList.SetProductSysNoList.Add(settedList.SysNo);
            }
            m_facade.UpdateVendorProduct(productList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                    this.dgSettedProducts.Bind();
                    this.dgUnSetProducts.Bind();
                }
            });
        }

        //自动添加可管理商品
        private void IsAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (!isFirst)
            {
                VendorProductListVM productList = new VendorProductListVM();
                productList.IsAuto = 1;
                productList.UserSysNo = this.Fitler.UserSysNo;
                productList.VendorSysNo = this.Fitler.VendorSysNo;
                productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
                productList.VendorManufacturerSysNo = this.Fitler.VendorManufacturerSysNo;
                productList.C2SysNo = this.Fitler.C2SysNo;
                productList.C3SysNo = this.Fitler.C3SysNo;
                this.btnCancelAll.Visibility = this.btnCancelSelected.Visibility = System.Windows.Visibility.Collapsed;
                m_facade.UpdateVendorProduct(productList, (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                            this.tcProduct.IsHitTestVisible = false;
                            this.tcProduct.SelectedIndex = 0;
                            this.dgSettedProducts.Bind();
                            this.dgUnSetProducts.Bind();
                        }
                    });
            }
        }

        //不自动设置可管理商品
        private void NotAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (!isFirst)
            {
                this.tcProduct.IsHitTestVisible = true;
                VendorProductListVM productList = new VendorProductListVM();
                productList.IsAuto = 0;
                productList.UserSysNo = this.Fitler.UserSysNo;
                productList.VendorSysNo = this.Fitler.VendorSysNo;
                productList.ManufacturerSysNo = this.Fitler.ManufacturerSysNo;
                productList.VendorManufacturerSysNo = this.Fitler.VendorManufacturerSysNo;
                productList.C2SysNo = this.Fitler.C2SysNo;
                productList.C3SysNo = this.Fitler.C3SysNo;
                this.btnCancelAll.Visibility = this.btnCancelSelected.Visibility = System.Windows.Visibility.Visible;
                m_facade.UpdateVendorProduct(productList, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResVendorInfo.Msg_UpdateSucess);
                        this.dgSettedProducts.Bind();
                        this.dgUnSetProducts.Bind();
                    }
                });
            }
        }
    }
}
