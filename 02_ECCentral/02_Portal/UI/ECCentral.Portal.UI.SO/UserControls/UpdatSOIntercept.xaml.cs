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

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class UpdatSOIntercept : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        public string m_sysNoList;
        public string SysNoList
        {
            get
            {
                return m_sysNoList;
            }
            private set
            {
                m_sysNoList = value;
            }
        }

        private SOInterceptInfoVM m_SOInterceptInfoVM;
        public SOInterceptInfoVM CurrentSOInterceptInfoVM
        {
            get
            {
                return m_SOInterceptInfoVM;
            }
            private set
            {
                m_SOInterceptInfoVM = value;
                gdUpdateSOIntercept.DataContext = m_SOInterceptInfoVM;
            }
        }

        public UpdatSOIntercept(string sysNoList)
        {
            InitializeComponent();
            CurrentSOInterceptInfoVM = new SOInterceptInfoVM();            
            SysNoList = sysNoList;
        }

        /// <summary>
        /// 批量修改订单拦截设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            m_SOInterceptInfoVM = gdUpdateSOIntercept.DataContext as SOInterceptInfoVM;
            CurrentSOInterceptInfoVM.Sysnolist = SysNoList;
            if (   string.IsNullOrEmpty(m_SOInterceptInfoVM.EmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.CCEmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.FinanceEmailAddress)
                || string.IsNullOrEmpty(m_SOInterceptInfoVM.FinanceCCEmailAddress)
                )
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SaveSOIntercept_Input_Error, MessageType.Error);
            }
            else 
            {
                ValidationManager.Validate(this.gdUpdateSOIntercept);
                if (m_SOInterceptInfoVM.HasValidationErrors && m_SOInterceptInfoVM.ValidationErrors.Count > 0)
                {
                    return;
                }
                if (string.IsNullOrEmpty(CurrentSOInterceptInfoVM.Sysnolist))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_SOIntercept_UpdateError_NoData, MessageType.Error);
                }
                SOInterceptInfo req = m_SOInterceptInfoVM.ConvertVM<SOInterceptInfoVM,SOInterceptInfo>();
                new SOInterceptFacade().BatchUpdateSOInterceptInfo(req, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOIntercept.Info_BatchUpdateSOIntercept_Success, MessageType.Information);
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK
                        });
                    }
                });
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
