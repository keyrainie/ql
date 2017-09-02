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

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class StockShiftConfig : UserControl
    {

        public IPage Page
        {
            get;
            set;
        }
        public IDialog Dialog
        {
            get;
            set;
        }
        StockShiftConfigVM configVM;
        public StockShiftConfigVM ConfigVM
        {
            get { return configVM; }
            set
            {
                configVM = value ?? new StockShiftConfigVM();
                this.DataContext = configVM;
            }
        }
        public StockShiftConfig()
        {
            InitializeComponent();
            ConfigVM = new StockShiftConfigVM();
            Loaded += new RoutedEventHandler(StockShiftConfig_Loaded);
        }

        void StockShiftConfig_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigVM.ShiftShippingTypeList == null)
            {
                CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, ConstValue.Key_StockShiftConfigShippingType, CodeNamePairAppendItemType.Custom_Select,
                    (obj, args) =>
                    {
                        if (!args.FaultsHandle() && args.Result != null)
                        {
                            ConfigVM.ShiftShippingTypeList = args.Result;
                        }
                    });
            }
            Loaded -= new RoutedEventHandler(StockShiftConfig_Loaded);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this);
            if (ConfigVM.HasValidationErrors)
            {
                return;
            }

            if (ConfigVM.SysNo.HasValue)
            {
                new StockShiftConfigFacade(Page).Update(ConfigVM, () =>
                {
                    Page.Context.Window.Alert("移仓配置信息修改成功!");
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = ConfigVM
                    });     
                });
            }
            else
            {
                new StockShiftConfigFacade(Page).Create(ConfigVM, (vm) =>
                {
                    vm.ShiftShippingTypeList = ConfigVM.ShiftShippingTypeList;
                    ConfigVM = vm;
                    Page.Context.Window.Alert("移仓配置信息添加成功!");
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = ConfigVM 
                    });                   
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods
    }
}
