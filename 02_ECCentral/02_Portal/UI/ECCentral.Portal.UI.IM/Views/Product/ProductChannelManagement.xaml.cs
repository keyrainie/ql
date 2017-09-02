using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductChannelManagement : PageBase
    {
        public ProductChannelManagement()
        {
            InitializeComponent();
        }


        ProductChannelQueryVM model;


        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductChannelQueryVM();

            ProductChannelInfoFacade facade = new ProductChannelInfoFacade();
            facade.GetChannelInfoList((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;

                }
                model.ChannelList = arg.Result;

                model.ChannelList.Insert(0, new ChannelInfo() { SysNo = 0, ChannelName = "所有" });

                this.DataContext = model;
                cbChannelProductStatus.SelectedIndex = 0;

            });
        }


        #region 查询绑定

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.dgProductChannelQueryResult.Bind();
        }

        private void dgProductChannelQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductChannelInfoQueryFacade facade = new ProductChannelInfoQueryFacade(this);
            model = (ProductChannelQueryVM)this.DataContext;
            facade.QueryProductChannelInfo(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }

                this.dgProductChannelQueryResult.ItemsSource = list;
                this.dgProductChannelQueryResult.TotalCount = args.Result.TotalCount;
            });
            cbDemo.IsChecked = false;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = dgProductChannelQueryResult.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }        
        }

        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void hyperlinkEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);
            
            ProductChannelManagementDetail detail = new ProductChannelManagementDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelManagement.Dialog_Edit, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelQueryResult.Bind();
                }
            }, new Size(700, 400));



        }

        private void hyperlinkSet_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);

            ProductChannelPeriodPriceInfo detail = new ProductChannelPeriodPriceInfo();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelManagement.Dialog_PeriodPriceSet, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelQueryResult.Bind();
                }
            }, new Size(700, 600));



        }

        private void hyperlinkProductID_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelQueryResult.SelectedItem as dynamic;
            if (item != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, item.ProductSysno), null, true);
            }
        }

        #endregion

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            model = (ProductChannelQueryVM)this.DataContext;

            if (this.model.ChannelSysNo > 0)
            {

                var ucPicker = new UCProductSearch();
                ucPicker.SelectionMode = SelectionMode.Multiple;
                ucPicker.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", ucPicker, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        if (args.Data != null)
                        {
                            var selectedProductList = args.Data as List<Basic.Components.UserControls.ProductPicker.ProductVM>;

                            List<ProductChannelInfo> productList = new List<ProductChannelInfo>();

                            selectedProductList.ForEach(p => productList.Add(new ProductChannelInfo { ChannelInfo = new ChannelInfo { SysNo = this.model.ChannelSysNo }
                                                                                                    , ProductSysNo = p.SysNo
                                                                                                    , ProductID = p.ProductID 
                                                                                                    , CreateUser= new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo
                                                                                                                                ,UserName = CPApplication.Current.LoginUser.LoginName
                                                                                                                                , UserDisplayName = CPApplication.Current.LoginUser.DisplayName }}));

                            var facade = new ProductChannelInfoFacade(this);

                            facade.CreateProductChannelInfo(productList, (o, a) =>
                            {
                                if (a.FaultsHandle())
                                {
                                    dgProductChannelQueryResult.Bind();
                                    return;
                                }
                            });    

                        }
                    }
                });
            }
            else
            {
                Window.Alert("请先选择渠道！", MessageType.Information);
                return;
            }
        }

        private void btnBatchSet_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductChannelQueryResult.ItemsSource != null)
            {
                var viewList = dgProductChannelQueryResult.ItemsSource as List<dynamic>;
                var selectSource = viewList.Where(p => p.IsChecked).ToList();
                if (selectSource == null || selectSource.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                ProductChannelBatchSetDetail detail = new ProductChannelBatchSetDetail();

                detail.SelectRows = selectSource;

                detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelManagement.Btn_BatchSet, detail, (s, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        dgProductChannelQueryResult.Bind();
                    }
                }, new Size(700, 250));

            }
        }

        //批量设置有效
        private void btnBatchUpdateStatusValid_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductChannelQueryResult.ItemsSource != null)
            {
                var viewList = dgProductChannelQueryResult.ItemsSource as List<dynamic>;
                var sysNolist = viewList.Where(p => p.IsChecked).Select(p => p.SysNo).ToList();

                if (sysNolist == null || sysNolist.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                //初始化数据
                ProductChannelVM vm = new ProductChannelVM();
                vm.Status = ProductChannelInfoStatus.Active;
                vm.SysNoList = new List<int>();

                foreach (var item in sysNolist)
                {
                    vm.SysNoList.Add(item);
                }

                ProductChannelInfoFacade facade = new ProductChannelInfoFacade();
                

                facade.BatchUpdateChannelProductInfoStatus(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        dgProductChannelQueryResult.Bind();
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
        }

        //批量设置无效
        private void btnBatchUpdateStatusInvalid_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductChannelQueryResult.ItemsSource != null)
            {
                var viewList = dgProductChannelQueryResult.ItemsSource as List<dynamic>;
                var sysNolist = viewList.Where(p => p.IsChecked).Select(p => p.SysNo).ToList();

                if (sysNolist == null || sysNolist.Count == 0)
                {
                    Window.Alert("请选择一条记录！", MessageType.Error);
                    return;
                }

                //初始化数据
                ProductChannelVM vm = new ProductChannelVM();
                vm.Status = ProductChannelInfoStatus.DeActive;
                vm.SysNoList = new List<int>();

                foreach (var item in sysNolist)
                {
                    vm.SysNoList.Add(item);
                }

                ProductChannelInfoFacade facade = new ProductChannelInfoFacade();


                facade.BatchUpdateChannelProductInfoStatus(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        dgProductChannelQueryResult.Bind();
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                });
            }
        }


        
        #endregion

        private void cbChannelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbChannelName.SelectedValue.ToString() == "200")
            {
                this.txb_TaobaoSKU.Visibility = Visibility.Visible;
                this.txt_TaobaoSKU.Visibility = Visibility.Visible;
            }
            else
            {
                this.txb_TaobaoSKU.Visibility = Visibility.Collapsed;
                this.txt_TaobaoSKU.Visibility = Visibility.Collapsed;
                this.txt_TaobaoSKU.Text = string.Empty;
            }
        }

    }
}
