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
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;

#region Newegg.Oversea.Oversea Libs

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Resources;

#endregion Newegg.Oversea.Oversea Libs

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class ValueAddedTaxInvoiceDetail : UserControl
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

        private ValueAddedTaxInfoVM m_valueAddedTax;
        public ValueAddedTaxInfoVM CurrentValueAddedTaxVM
        {
            get
            {
                return m_valueAddedTax;
            }
            private set
            {
                m_valueAddedTax = value;
                gdDetailInfo.DataContext = value;
            }
        }

        public CustomerFacade CustomerFacade
        {
            get;
            set;
        }

        public ValueAddedTaxInvoiceDetail()
        {
            InitializeComponent();
            NewValueAddedTaxInvoiceDataContext();
            Loaded += new RoutedEventHandler(ValueAddedTaxInvoiceDetail_Loaded);
        }

        private void ValueAddedTaxInvoiceDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(ValueAddedTaxInvoiceDetail_Loaded);

            CustomerFacade = new CustomerFacade(CPApplication.Current.CurrentPage);
            if (!string.IsNullOrEmpty(CurrentValueAddedTaxVM.CertificateFileName))
            {
                hlbView.Visibility = System.Windows.Visibility.Visible;
                hlbView.NavigateUri = new Uri(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl") + "/" + CurrentValueAddedTaxVM.CertificateFileName);
            }
            else
            {
                hlbView.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public ValueAddedTaxInvoiceDetail(ValueAddedTaxInfoVM valueAddedTax)
            : this()
        {
            CurrentValueAddedTaxVM = UtilityHelper.DeepClone(valueAddedTax);
        }

        private void NewValueAddedTaxInvoiceDataContext()
        {
            ValueAddedTaxInfoVM model = new ValueAddedTaxInfoVM()
            {
                IsDefault = false
            };
            CurrentValueAddedTaxVM = model;
        }

        #region Event Handler

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValueAddedTaxInfoVM model = this.gdDetailInfo.DataContext as ValueAddedTaxInfoVM;
            bool flag = ValidationManager.Validate(this.gdDetailInfo);

            if (!model.HasValidationErrors && CustomerFacade != null)
            {
                if (model.CustomerSysNo != null)
                {
                    CustomerFacade.UpdateValueAddedTaxInfo(model, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        var copy = UtilityHelper.DeepClone(model);
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK,
                            Data = copy
                        });
                    });
                }
                else
                {
                    //新增的时候由父窗口执行真正的保存操作
                    var copy = UtilityHelper.DeepClone(model);
                    CurrentValueAddedTaxVM = copy;
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = copy
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
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

        private void FileUploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            if (args.UploadInfo[0].UploadResult == Basic.Controls.Uploader.SingleFileUploadStatus.Failed)
            {
                CurrentWindow.Alert(ResValueAddedTaxInvoiceDetail.msg_UploadFalid);
                return;
            }
            //上传成功，返回文件地址：
            CurrentValueAddedTaxVM.CertificateFileName = args.UploadInfo[0].ServerFilePath;
        }

        public void SetAllReadOnlyOrEnable()
        {
            foreach (var item in gdDetailInfo.Children)
            {
                if (item is CheckBox)
                {
                    (item as CheckBox).IsEnabled = false;
                }
                if (item is TextBox)
                {
                    (item as TextBox).IsReadOnly = true;
                }
                if (item is DatePicker)
                {
                    (item as DatePicker).IsEnabled = false;
                }
            }
            FileUploader.IsEnabled = false;
            btnSave.Visibility = System.Windows.Visibility.Collapsed;
            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}