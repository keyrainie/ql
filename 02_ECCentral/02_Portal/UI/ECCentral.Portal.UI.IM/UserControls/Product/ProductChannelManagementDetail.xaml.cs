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
    public partial class ProductChannelManagementDetail : UserControl
    {

        #region Private
        private const int ChannelSysNo = 200;
        #endregion
        
        #region 属性

        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        private ProductChannelInfoFacade _facade;

        private int _sysNo;
        
        #endregion

        #region 初始化加载

        public ProductChannelManagementDetail()
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

                    if (cbIsAppointInventory.IsChecked.HasValue)
                    {
                        this.tb_ChannelSellCount.IsEnabled = cbIsAppointInventory.IsChecked.Value;
                    }
                    this.tb_SynProductID.IsEnabled = !vm.ChannelInfo.SysNo.Equals(ChannelSysNo);
                    this.tb_TaoBaoSku.IsEnabled = !vm.ChannelInfo.SysNo.Equals(ChannelSysNo);
                    if (!vm.ChannelInfo.SysNo.Equals(ChannelSysNo))
                    {
                        this.lbl_TaoBao.Visibility = Visibility.Collapsed;
                        this.tb_TaoBaoSku.Visibility = Visibility.Collapsed;
                        this.lbl_TaoBaoLab.Visibility = Visibility.Collapsed;
                        this.tb_SynProductID.IsEnabled = this.tb_SynProductID.Text.Equals(string.Empty);
                    }

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProductChannelVM;
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

            _facade.UpdateProductChannelInfo(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                CloseDialog(DialogResultType.OK);
            });

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }


        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }


        private void cbIsAppointInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (cbIsAppointInventory.IsChecked.HasValue)
            {
                this.tb_ChannelSellCount.IsEnabled = cbIsAppointInventory.IsChecked.Value;
                this.tb_ChannelSellCount.Text = "0";
            }
        }

        private void dplistStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var status = (ProductChannelInfoStatus)this.dplistStatus.SelectedValue;


            if (status == ProductChannelInfoStatus.DeActive)
            {
                this.cbIsAppointInventory.IsChecked = false;
                this.tb_ChannelSellCount.Text = "0";
            }

            this.cbIsAppointInventory.IsEnabled = status == ProductChannelInfoStatus.Active;

            this.tb_ChannelSellCount.IsEnabled = status == ProductChannelInfoStatus.Active && this.cbIsAppointInventory.IsChecked.Value;
        }


        #endregion



    }
}
