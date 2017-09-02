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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Service.SO.Restful.RequestMsg;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SendSOInterceptEmail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private SOVM m_SOVM;
        public SOVM CurrentSOVM
        {
            get
            {
                return m_SOVM;
            }
            private set
            {
                m_SOVM = value;
            }
        }

        public SendSOInterceptEmail(SOVM soVM)
        {
            InitializeComponent();
            CurrentSOVM = soVM;
            gdOrderEmailInfo.DataContext = CurrentSOVM.SOInterceptInfoVMList[0];
            if (!soVM.InvoiceInfoVM.IsVAT.HasValue || soVM.InvoiceInfoVM.IsVAT == false)
            {               
                CurrentSOVM.SOInterceptInfoVMList.ForEach(item => { item.FinanceEmailAddress = ""; item.FinanceCCEmailAddress = ""; });              
                gdFinanceEmailInfo.Visibility = Visibility.Collapsed;              
            }
            gdFinanceEmailInfo.DataContext = CurrentSOVM.SOInterceptInfoVMList[0];      
        }

        /// <summary>
        /// 发送订单拦截邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendSOInterceptEmail_Click(object sender, RoutedEventArgs e)
        {
            SOInterceptInfoVM orderEmailInfoVM =new SOInterceptInfoVM();     
            SOInterceptInfoVM financeEmailInfoVM = new SOInterceptInfoVM();
            orderEmailInfoVM.EmailAddress = this.txtEmailAddresse.Text;
            orderEmailInfoVM.CCEmailAddress = this.txtCCEmailAddress.Text;
            financeEmailInfoVM.FinanceEmailAddress = this.txtFinanceEmailAddress.Text;
            financeEmailInfoVM.FinanceCCEmailAddress = this.txtFinanceCCEmailAddress.Text;
            if (CurrentSOVM.InvoiceInfoVM.IsVAT == true)
            {
                if (string.IsNullOrEmpty(orderEmailInfoVM.EmailAddress)
                    || string.IsNullOrEmpty(orderEmailInfoVM.CCEmailAddress)
                    || string.IsNullOrEmpty(financeEmailInfoVM.FinanceEmailAddress)
                    || string.IsNullOrEmpty(financeEmailInfoVM.FinanceCCEmailAddress)
                    )
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SaveSOIntercept_Input_Error, MessageType.Error);
                }
                else
                {
                    ValidationManager.Validate(this.gdOrderEmailInfo);
                    if (orderEmailInfoVM.HasValidationErrors && orderEmailInfoVM.ValidationErrors.Count > 0)
                    {
                        return;
                    }
                    ValidationManager.Validate(this.gdFinanceEmailInfo);
                    if (financeEmailInfoVM.HasValidationErrors && financeEmailInfoVM.ValidationErrors.Count > 0)
                    {
                        return;
                    }
                    SOInterceptInfoVM soInterceptInfoVM = new SOInterceptInfoVM();
                    soInterceptInfoVM.EmailAddress = orderEmailInfoVM.EmailAddress;
                    soInterceptInfoVM.CCEmailAddress = orderEmailInfoVM.CCEmailAddress;
                    soInterceptInfoVM.FinanceEmailAddress = financeEmailInfoVM.FinanceEmailAddress;
                    soInterceptInfoVM.FinanceCCEmailAddress = financeEmailInfoVM.FinanceCCEmailAddress;

                    CurrentSOVM.SOInterceptInfoVMList.Add(soInterceptInfoVM);
                   
                    #region 发送订单拦截邮件
                    SendEmailReq reqSOOrderIntercep = new SendEmailReq();
                    reqSOOrderIntercep.soInfo = SOFacade.ConvertTOSOInfoFromSOVM(CurrentSOVM);
                    reqSOOrderIntercep.Language = CPApplication.Current.LanguageCode;
                    new SOInterceptFacade().SendSOOrderInterceptEmail(reqSOOrderIntercep, (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {                            
                            CloseDialog(new ResultEventArgs
                            {
                                DialogResult = DialogResultType.OK,
                            });
                        }

                    }); 
                    #endregion
                    #region 发送增票拦截邮件
                    SendEmailReq reqSOFinanceIntercep = new SendEmailReq();
                    reqSOFinanceIntercep.soInfo = SOFacade.ConvertTOSOInfoFromSOVM(CurrentSOVM);
                    reqSOFinanceIntercep.Language = CPApplication.Current.LanguageCode;
                    new SOInterceptFacade().SendSOFinanceInterceptEmail(reqSOFinanceIntercep, (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SendEmail_Sucessful, MessageType.Information);
                            CloseDialog(new ResultEventArgs
                            {
                                DialogResult = DialogResultType.OK,
                            });
                        }

                    });                     
                    #endregion
                }
            }
            else
            {
                if (string.IsNullOrEmpty(orderEmailInfoVM.EmailAddress)|| string.IsNullOrEmpty(orderEmailInfoVM.CCEmailAddress))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SaveSOIntercept_Input_Error, MessageType.Error);
                }
                else
                {
                    ValidationManager.Validate(this.gdOrderEmailInfo);
                    if (orderEmailInfoVM.HasValidationErrors && orderEmailInfoVM.ValidationErrors.Count > 0)
                    {
                        return;
                    }
                    SOInterceptInfoVM soInterceptInfoVM = new SOInterceptInfoVM();
                    soInterceptInfoVM.EmailAddress = orderEmailInfoVM.EmailAddress;
                    soInterceptInfoVM.CCEmailAddress = orderEmailInfoVM.CCEmailAddress;
                    CurrentSOVM.SOInterceptInfoVMList.Add(soInterceptInfoVM);
                    #region 发送订单拦截邮件
                    SendEmailReq reqSOOrderIntercep = new SendEmailReq();
                    reqSOOrderIntercep.soInfo = SOFacade.ConvertTOSOInfoFromSOVM(CurrentSOVM);
                    reqSOOrderIntercep.Language = CPApplication.Current.LanguageCode;
                    new SOInterceptFacade().SendSOOrderInterceptEmail(reqSOOrderIntercep,(obj,args)=>
                    {
                        if(!args.FaultsHandle())
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SendEmail_Sucessful, MessageType.Information);
                            CloseDialog(new ResultEventArgs
                            {
                                DialogResult = DialogResultType.OK,
                            });
                        }

                    });                   
                    #endregion
                }
            }                               
        }
        
        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
    }
}
