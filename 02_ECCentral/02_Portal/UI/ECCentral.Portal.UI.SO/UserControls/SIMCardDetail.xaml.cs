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
    public partial class SIMCardDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private SOSIMCardAndContractPhoneInfoVM m_SIMCardAndContractPhoneInfoVM;
        public SOSIMCardAndContractPhoneInfoVM CurrentSIMCardAndContractPhoneInfoVM
        {
            get
            {
                return m_SIMCardAndContractPhoneInfoVM;
            }
            private set
            {
                m_SIMCardAndContractPhoneInfoVM = value;
                gdSIMCardInfo.DataContext = value;
            }
        }

        public SIMCardDetail()
        {
            InitializeComponent();  
        }

        public SIMCardDetail(SOSIMCardAndContractPhoneInfoVM soSIMCardAndContractPhoneInfoVM, string companyCode)
            : this()
        {
            // 读取枚举 初始化SIM状态
            cmbCertificateType.ItemsSource = EnumConverter.GetKeyValuePairs<CertificateType>(EnumConverter.EnumAppendItemType.None);
            cmbSIMStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SIMStatus>(EnumConverter.EnumAppendItemType.None);
            CurrentSIMCardAndContractPhoneInfoVM = soSIMCardAndContractPhoneInfoVM;
            #region 加载SIM卡信息     
            if (string.IsNullOrEmpty(CurrentSIMCardAndContractPhoneInfoVM.CustomerName))
            {
                new SOQueryFacade().QuerySOSIMCardInfo(CurrentSIMCardAndContractPhoneInfoVM.SOSysNo.Value, companyCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CurrentSIMCardAndContractPhoneInfoVM = args.Result.Convert<SOSIMCardAndContractPhoneInfo, SOSIMCardAndContractPhoneInfoVM>();
                });    
            }
            ECCentral.Portal.Basic.Utilities.UtilityHelper.ReadOnlyControl(gdSIMCardInfo, gdSIMCardInfo.Children.Count,true);
            cmbSIMStatus.IsEnabled = true;
            txtMemo.IsReadOnly = false;
            #endregion

            Loaded += new RoutedEventHandler(SIMCardDetail_Loaded);
        }

        void SIMCardDetail_Loaded(object sender, RoutedEventArgs e)
        {
            RightControl();
        }

        void RightControl()
        {
            Button_SIMCard_Save.Visibility = AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_SOSIMStatusEidt) ? Visibility.Visible : Visibility.Collapsed;
        }
          
        #region Event Handler

        private void Button_SIMCard_Save_Click(object sender, RoutedEventArgs e)
        {
            CurrentSIMCardAndContractPhoneInfoVM = this.gdSIMCardInfo.DataContext as SOSIMCardAndContractPhoneInfoVM;
            bool flag = ValidationManager.Validate(this.gdSIMCardInfo);
            if (!CurrentSIMCardAndContractPhoneInfoVM.HasValidationErrors && flag)
            {
                //新增的时候由父窗口执行真正的保存操作                  
                CloseDialog(new ResultEventArgs
                {
                    DialogResult = DialogResultType.OK,
                    Data = CurrentSIMCardAndContractPhoneInfoVM
                });
            }
        }

        private void Button_SIMCard_Print_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dPrint = new Dictionary<string, string>();
            dPrint.Add("SOSysNoList", CurrentSIMCardAndContractPhoneInfoVM.SOSysNo.ToString());
            HtmlViewHelper.WebPrintPreview("SO", "SOSIMCard", dPrint);
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
