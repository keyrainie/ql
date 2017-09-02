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
    public partial class UnicomFreeBuyDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        private SOSIMCardAndContractPhoneInfoVM m_SOSIMCardAndContractPhoneInfoVM;
        public SOSIMCardAndContractPhoneInfoVM CurrentSOSIMCardAndContractPhoneInfoVM
        {
            get
            {
                return m_SOSIMCardAndContractPhoneInfoVM;
            }
            private set
            {
                m_SOSIMCardAndContractPhoneInfoVM = value;
                gdUnicomFreeBuyDetailInfo.DataContext = value;
                gdUnicomFreeBuyContractPhoneDetailInfo.DataContext = m_SOSIMCardAndContractPhoneInfoVM.ContractPhoneDetailInfoVM;
            }
        }

        public UnicomFreeBuyDetail()
        {
            InitializeComponent();  
        }

        public UnicomFreeBuyDetail(SOSIMCardAndContractPhoneInfoVM soSIMCardAndContractPhoneInfoVM, string companyCode)
            : this()
        {
            // 读取枚举 初始化SIM状态
            cmbCertificateType.ItemsSource = EnumConverter.GetKeyValuePairs<CertificateType>(EnumConverter.EnumAppendItemType.None);
            cmbSIMStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SIMStatus>(EnumConverter.EnumAppendItemType.None);
            CurrentSOSIMCardAndContractPhoneInfoVM = soSIMCardAndContractPhoneInfoVM;
            #region 加载联通0元购机信息
            if (string.IsNullOrEmpty(CurrentSOSIMCardAndContractPhoneInfoVM.CustomerName))
            {
                new SOQueryFacade().QuerySOSIMCardInfo(CurrentSOSIMCardAndContractPhoneInfoVM.SOSysNo.Value, companyCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CurrentSOSIMCardAndContractPhoneInfoVM = args.Result.Convert<SOSIMCardAndContractPhoneInfo, SOSIMCardAndContractPhoneInfoVM>();
                    CurrentSOSIMCardAndContractPhoneInfoVM.ContractPhoneDetailInfoVM = args.Result.ContractPhoneDetailInfo.Convert<SOContractPhoneDetailInfo, SOContractPhoneDetailInfoVM>();
                });    
            }
            ECCentral.Portal.Basic.Utilities.UtilityHelper.ReadOnlyControl(gdUnicomFreeBuyDetailInfo, gdUnicomFreeBuyDetailInfo.Children.Count, true);
            ECCentral.Portal.Basic.Utilities.UtilityHelper.ReadOnlyControl(gdUnicomFreeBuyContractPhoneDetailInfo, gdUnicomFreeBuyContractPhoneDetailInfo.Children.Count, true);
            cmbSIMStatus.IsEnabled = true;
            txtMemo.IsReadOnly = false;
            #endregion
        }
          
        #region Event Handler

        private void Button_UnicomFreeBuy_Save_Click(object sender, RoutedEventArgs e)
        {
            CurrentSOSIMCardAndContractPhoneInfoVM = this.gdUnicomFreeBuyDetailInfo.DataContext as SOSIMCardAndContractPhoneInfoVM;
            bool flag = ValidationManager.Validate(this.gdUnicomFreeBuyDetailInfo);
            if (!CurrentSOSIMCardAndContractPhoneInfoVM.HasValidationErrors && flag)
            {
                //新增的时候由父窗口执行真正的保存操作                  
                CloseDialog(new ResultEventArgs
                {
                    DialogResult = DialogResultType.OK,
                    Data = CurrentSOSIMCardAndContractPhoneInfoVM
                });
            }
        }

        private void Button_UnicomFreeBuy_Print_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dPrint = new Dictionary<string, string>();
            dPrint.Add("SOSysNoList", CurrentSOSIMCardAndContractPhoneInfoVM.SOSysNo.ToString());
            HtmlViewHelper.WebPrintPreview("SO", "UnicomFreeBuy", dPrint);
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
