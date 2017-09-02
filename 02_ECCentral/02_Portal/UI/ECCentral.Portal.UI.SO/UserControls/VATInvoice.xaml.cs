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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class VATInvoice : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private SOVATInvoiceInfoVM m_valueAddedTaxVM;
        public SOVATInvoiceInfoVM CurrentValueAddedTaxVM
        {
            get
            {
                return m_valueAddedTaxVM;
            }
            private set
            {
                m_valueAddedTaxVM = value;
                gdDetailInfo.DataContext = value;
            }
        }

        private List<SOVATInvoiceInfoVM> m_valueAddedTaxVMList;
        public List<SOVATInvoiceInfoVM> ValueAddedTaxVMList
        {
            get
            {
                return m_valueAddedTaxVMList;
            }
            private set
            {
               m_valueAddedTaxVMList = value;
               gridVATListInfo.ItemsSource = value;
            }
        }

        public VATInvoice()
        {
            InitializeComponent();  
        }  
        public VATInvoice(SOVATInvoiceInfoVM valueAddedTax)
            : this()
        {
            CurrentValueAddedTaxVM = valueAddedTax;
            #region 加载增值税列表                        
            new SOQueryFacade().QuerySOVATInvoiceInfo(CurrentValueAddedTaxVM.CustomerSysNo.Value, (obj, args) =>
            {
                  if (args.FaultsHandle())
                  {
                      return;
                  }
                  ValueAddedTaxVMList = args.Result.Convert<SOVATInvoiceInfo, SOVATInvoiceInfoVM>();
                  if (ValueAddedTaxVMList != null && ValueAddedTaxVMList.Count > 0 && CurrentValueAddedTaxVM.SysNo.HasValue)
                  {
                         if (CurrentValueAddedTaxVM.SOSysNo.HasValue && CurrentValueAddedTaxVM.SOSysNo > 0)
                          {                             
                              foreach (var item in ValueAddedTaxVMList)
                              {
                                  if(item.SOSysNo==CurrentValueAddedTaxVM.SOSysNo)
                                  {                                      
                                     CurrentValueAddedTaxVM = item;   
                                      break;
                                  }                       
                              }
                          }
                         else
                         {
                              foreach (var item in ValueAddedTaxVMList)
                              {                       
                                 if (item.SysNo == CurrentValueAddedTaxVM.SysNo)
                                  {
                                      CurrentValueAddedTaxVM = item;
                                  }
                              }
                         }
                  }
             });              
            #endregion
        }
          
        #region Event Handler

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (SOVATInvoiceInfoVM item in ValueAddedTaxVMList)
            {
                if (((HyperlinkButton)sender).CommandParameter.ToString() == item.SysNo.Value.ToString())
                {
                    CurrentValueAddedTaxVM = item;
                }
            }   
        }

        private void Button_SOVATCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }

        private void Button_SOVATConfirm_Click(object sender, RoutedEventArgs e)
        {
            SOVATInvoiceInfoVM model = this.gdDetailInfo.DataContext as SOVATInvoiceInfoVM;
            bool flag = ValidationManager.Validate(this.gdDetailInfo);

            if (string.IsNullOrEmpty(model.CompanyName)
                || string.IsNullOrEmpty(model.TaxNumber)
                || string.IsNullOrEmpty(model.BankAccount))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError_InputError);
                return;
            }

            if (!model.HasValidationErrors&& flag)
            {
                //新增的时候由父窗口执行真正的保存操作                  
                CloseDialog(new ResultEventArgs
                {
                    DialogResult = DialogResultType.OK,
                    Data = model
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
