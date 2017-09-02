using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
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
    public partial class ProductChannelPeriodPriceInfoDetail : UserControl
    {
        #region 属性

        public ProductChannelVM productVM { get; set; }

        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        private ProductChannelInfoFacade _facade;

        private int _sysNo;

        #endregion

        #region 初始化加载

        public ProductChannelPeriodPriceInfoDetail()
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
                _facade.GetProductChannelPeriodPriceBySysNo(SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("无法渠道商品信息.");
                        return;
                    }
                    var vm = args.Result.Convert<ProductChannelPeriodPrice, ProductChannelPeriodPriceVM>();

                    _sysNo = SysNo.Value;

                    vm.ChannelProductInfo = productVM;

                    DataContext = vm;
                });
            }
            else
            {
                _sysNo = 0;
                var item = new ProductChannelPeriodPriceVM();
                item.Status = ProductChannelPeriodPriceStatus.Init;
                item.ChannelProductInfo = productVM;
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

        private void OperateData(ProductChannelPeriodPriceOperate operate)
        {
            var vm = DataContext as ProductChannelPeriodPriceVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }
            _facade = new ProductChannelInfoFacade();
            vm.SysNo = _sysNo;
            vm.Operate = operate;

            if (!(vm.BeginDate.HasValue && vm.EndDate.HasValue))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("改价时间不能为空.");
                return;
            }

            if (vm.Operate == ProductChannelPeriodPriceOperate.Submit && vm.Note.Equals(string.Empty))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("活动说明不能为空.");
                return;
            }

            if (_sysNo == 0)
            {
                _facade.CreateProductChannelPeriodPrice(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {

                _facade.UpdateProductChannelPeriodPrice(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    CloseDialog(DialogResultType.OK);
                });
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            OperateData(ProductChannelPeriodPriceOperate.CreateOrEdit);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            OperateData(ProductChannelPeriodPriceOperate.Submit);
        }

        private void btnCancelSubmit_Click(object sender, RoutedEventArgs e)
        {
            OperateData(ProductChannelPeriodPriceOperate.CancelSubmit);
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            OperateData(ProductChannelPeriodPriceOperate.Approve);
        }

        private void btnUnApprove_Click(object sender, RoutedEventArgs e)
        {
            OperateData(ProductChannelPeriodPriceOperate.UnApprove);
        }

        #endregion
    }
}
