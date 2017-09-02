using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Media;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductChannelPeriodPriceInfo : UserControl
    {

        #region 属性

        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        private ProductChannelInfoFacade _facade;

        private int _sysNo;
        
        #endregion

        #region 初始化加载

        public ProductChannelPeriodPriceInfo()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindPage();
        }

        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (SysNo != null)
            {
                _facade = new ProductChannelInfoFacade();
                _facade.GetProductChannelInfoBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("无法渠道商品信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = args.Result.Convert<ProductChannelInfo, ProductChannelVM>();

                    vm.ChannelInfo = args.Result.ChannelInfo.Convert<ChannelInfo, ChannelVM>
                           ((v, t) =>
                           {
                               t.ChannelName = v.ChannelName;
                           });

                    _sysNo = SysNo.Value;

                    DataContext = vm;

                    dgProductChannelPeriodPriceQueryResult.Bind();
                });
            }
            else
            {
                _sysNo = 0;
                var item = new SellerProductRequestVM();
                DataContext = item;
            }
        }


        #endregion

        #region 按钮事件


        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }   

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void hyperlinkStop_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ECCentral.Portal.UI.IM.Resources.ResProductChannelPeriodPriceInfo.Dialog_Stop, (obj, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        dynamic item = this.dgProductChannelPeriodPriceQueryResult.SelectedItem as dynamic;
                        if (item == null) return;

                        var vm = new ProductChannelPeriodPriceVM();

                        vm.Operate = ProductChannelPeriodPriceOperate.Stop;
                        vm.SysNo = Convert.ToInt32(item.SysNo);

                        _facade.UpdateProductChannelPeriodPrice(vm, (obju, argsu) =>
                        {
                            if (argsu.FaultsHandle())
                            {
                                return;
                            }

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                            dgProductChannelPeriodPriceQueryResult.Bind();

                        });
                    }
                });
        }

        private void hyperlinkEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgProductChannelPeriodPriceQueryResult.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.SysNo);
            var status = item.Status;           
            ProductChannelPeriodPriceInfoDetail detail = new ProductChannelPeriodPriceInfoDetail();
            detail.SysNo = sysNo;
            detail.productVM = DataContext as ProductChannelVM;
            detail.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelPeriodPriceInfo.Dialog_Edit, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelPeriodPriceQueryResult.Bind();
                }
            }, new Size(650, 350));
        }

        private void dgProductChannelPeriodPriceQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ProductChannelInfoQueryFacade facade = new ProductChannelInfoQueryFacade();

            var model = new ProductChannelPeriodPriceQueryVM { ChannelProductSysNo = _sysNo };
           
            facade.QueryProductChannelPeriodPrice(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }

                this.dgProductChannelPeriodPriceQueryResult.ItemsSource = list;
                this.dgProductChannelPeriodPriceQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductChannelPeriodPriceInfoDetail detail = new ProductChannelPeriodPriceInfoDetail();
            detail.productVM = DataContext as ProductChannelVM;
            detail.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResProductChannelPeriodPriceInfo.Dialog_Add, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgProductChannelPeriodPriceQueryResult.Bind();
                }
            }, new Size(650, 350));
        }

        #endregion

    }
}
