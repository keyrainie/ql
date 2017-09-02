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
    public partial class ContractPhoneDetail : UserControl
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
                gdContractPhoneDetailInfo.DataContext = value;
            }
        }

        public ContractPhoneDetail()
        {
            InitializeComponent();  
        }

        public ContractPhoneDetail(SOSIMCardAndContractPhoneInfoVM soSIMCardAndContractPhoneInfoVM, string companyCode)
            : this()
        {
            // 读取枚举 初始化SIM状态
            cmbCertificateType.ItemsSource = EnumConverter.GetKeyValuePairs<CertificateType>(EnumConverter.EnumAppendItemType.None);

            //非未激活状态的不能加载未激活
            var statusList = EnumConverter.GetKeyValuePairs<SIMStatus>(EnumConverter.EnumAppendItemType.None);
            if(soSIMCardAndContractPhoneInfoVM.SIMStatus != SIMStatus.Original)
            {
                statusList.RemoveAll(p=>p.Key == SIMStatus.Original);
            }
            cmbSIMStatus.ItemsSource = statusList;

            CurrentSOSIMCardAndContractPhoneInfoVM = soSIMCardAndContractPhoneInfoVM;
            #region 加载合约机信息
            if (string.IsNullOrEmpty(CurrentSOSIMCardAndContractPhoneInfoVM.CustomerName))
            {
                new SOQueryFacade().QuerySOSIMCardInfo(CurrentSOSIMCardAndContractPhoneInfoVM.SOSysNo.Value, companyCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CurrentSOSIMCardAndContractPhoneInfoVM = args.Result.Convert<SOSIMCardAndContractPhoneInfo, SOSIMCardAndContractPhoneInfoVM>();
                });    
            }
            ECCentral.Portal.Basic.Utilities.UtilityHelper.ReadOnlyControl(gdContractPhoneDetailInfo, gdContractPhoneDetailInfo.Children.Count, true);
            cmbSIMStatus.IsEnabled = true;
            txtMemo.IsReadOnly = false;
            #endregion

            Loaded += new RoutedEventHandler(ContractPhoneDetail_Loaded);
        }

        void ContractPhoneDetail_Loaded(object sender, RoutedEventArgs e)
        {
            RightControl();
        }

        void RightControl()
        {
            Button_UnicomDetail_Save.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOSIMStatusEidt);
        }
        
        #region Event Handler

        private void Button_UnicomDetail_Save_Click(object sender, RoutedEventArgs e)
        {
            CurrentSOSIMCardAndContractPhoneInfoVM = this.gdContractPhoneDetailInfo.DataContext as SOSIMCardAndContractPhoneInfoVM;
            bool flag = ValidationManager.Validate(this.gdContractPhoneDetailInfo);
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

        private void Button_UnicomDetail_Print_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dPrint = new Dictionary<string, string>();
            dPrint.Add("SOSysNoList", CurrentSOSIMCardAndContractPhoneInfoVM.SOSysNo.ToString());
            HtmlViewHelper.WebPrintPreview("SO", "SOContractPhone", dPrint);
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
