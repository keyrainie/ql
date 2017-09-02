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
using ECCentral.Portal.UI.SO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOPriceCompensation : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private List<SOItemInfoVM> m_ItemsVM;
        public List<SOItemInfoVM> CurrentItemsVM
        {
            get
            {
                return m_ItemsVM;
            }
            private set
            {
                m_ItemsVM = value;
                gridSOPriceCompensation.DataContext = value;
            }
        }
   
        public SOPriceCompensation()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOPriceCompensation_Loaded);
        }

        private void SOPriceCompensation_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(SOPriceCompensation_Loaded);

            btn_SOPriceCompensation_Update.Visibility = AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_IsWholeSale) ? Visibility.Visible : Visibility.Collapsed;
        }

        public SOPriceCompensation(List<SOItemInfoVM> ItemsVM)
            : this()
        {
            CurrentItemsVM = ItemsVM;
            CurrentItemsVM.ForEach(p => p.TruePrice = (p.OriginalPrice ?? 0) + (p.AdjustPrice ?? 0));
            dgridSOPriceCompensation.ItemsSource = CurrentItemsVM;
        }
    
        #region Event Handler
      
        /// <summary>
        /// 更新购物车商品价格价格补偿）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SOPriceCompensation_Update_Click(object sender, RoutedEventArgs e)
        {
            List<SOItemInfoVM>  model = dgridSOPriceCompensation.DataContext as List<SOItemInfoVM>;            
            Boolean CheckResult = true;
            foreach (var item in model)
            {
                if (item.AdjustPrice != 0 && string.IsNullOrEmpty(item.AdjustPriceReason))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOPriceCompensation_Error, MessageType.Error);
                    CheckResult = false;
                    break;
                }
                else if (item.AdjustPrice > item.TruePrice)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOPriceCompensation_Price_Input_Than_OldPrice_Error, MessageType.Error);
                    CheckResult = false;
                    break;
                }
            }
            if (CheckResult && model!=null)
            {                
                //新增的时候由父窗口执行真正的保存操作                
                CurrentItemsVM = model;
                CloseDialog(new ResultEventArgs
                {
                    DialogResult = DialogResultType.OK,
                    Data = CurrentItemsVM
                });                
            }
        }

        #endregion Event Handler

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
