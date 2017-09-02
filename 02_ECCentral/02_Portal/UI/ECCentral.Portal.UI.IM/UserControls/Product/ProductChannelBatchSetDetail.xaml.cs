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

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductChannelBatchSetDetail : UserControl
    {

        #region 属性

        public IDialog Dialog { get; set; }


        public List<dynamic> SelectRows { get; set; }

        private ProductChannelInfoFacade _facade;
        
        #endregion

        #region 初始化加载

        public ProductChannelBatchSetDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindPage();
        }

        private void BindPage()
        {
            var item = new ProductChannelVM();
            item.IsUsePromotionPriceDisplay = false;
            DataContext = item;
        }

        #endregion

        #region 按钮事件


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ProductChannelVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }          

            var editList = (from c in SelectRows
                             select
                                 new ProductChannelVM
                                 {
                                     SysNo = c.SysNo,
                                     ProductID = c.ProductID,
                                     InventoryPercent=vm.InventoryPercent,
                                     SafeInventoryQty = vm.SafeInventoryQty,
                                     ChannelPricePercent=vm.ChannelPricePercent,
                                     IsUsePromotionPrice=vm.IsUsePromotionPrice                                    
                                 }).ToList();

            _facade = new ProductChannelInfoFacade();

            _facade.BatchUpdateProductChannelInfo(editList, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    CloseDialog(DialogResultType.OK);                
                }
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

        #endregion


    }
}
