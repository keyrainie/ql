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
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class AdjustRequestInvoice : UserControl
    {
        AdjustRequestMaintainFacade MaintainFacade;
        AdjustRequestVM requestVM;

        public AdjustRequestVM RequestVM
        {
            get
            {
                return requestVM;
            }
            set
            {
                requestVM = value;
                if (requestVM.InvoiceInfo == null)
                {
                    requestVM.InvoiceInfo = new AdjustRequestInvoiceVM();
                }

                this.DataContext = requestVM.InvoiceInfo;
            }
        }

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        
        public IPage Page
        { 
            get; set; 
        }
        public IDialog Dialog 
        { 
            get; set; 
        }

        public AdjustRequestInvoice()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AdjustRequestInvoice_Loaded);            
        }

        void AdjustRequestInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            MaintainFacade = new AdjustRequestMaintainFacade(Page);
            Loaded -= new RoutedEventHandler(AdjustRequestInvoice_Loaded);
        }
        

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SavePreValidate())
            {                
                MaintainFacade.MaintainRequestInvoiceInfo(RequestVM, vm =>
                {
                    if (vm != null)
                    {
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK,
                            Data = vm
                        });
                        this.CurrentWindow.Alert("损益单发票更新成功");
                    }
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

        private bool SavePreValidate()
        {
            if (string.IsNullOrEmpty(RequestVM.InvoiceInfo.ReceiveName))
            {
                this.CurrentWindow.Alert("收货人不能为空");
                return false;
            }

            if (string.IsNullOrEmpty(RequestVM.InvoiceInfo.ContactAddress))
            {
                this.CurrentWindow.Alert("联系地址不能为空");
                return false;
            }

            if (string.IsNullOrEmpty(RequestVM.InvoiceInfo.ContactPhoneNumber))
            {
                this.CurrentWindow.Alert("电话不能为空");
                return false;
            }
            return true;
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
