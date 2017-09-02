using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class BatchImportCustomer : UserControl
    {
        List<ValidationEntity> fromLinkList;
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

        public CustomerFacade CustomerFacade
        {
            get;
            set;
        }

        private BatchImportCustomerVM model;
        private BatchImportCustomerVM BatchVM
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                this.gdBatchImportSetting.DataContext = model;
            }
        }

        public BatchImportCustomer()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(BatchImportCustomer_Loaded);
        }
        private void BatchImportCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(BatchImportCustomer_Loaded);
            CustomerFacade = new CustomerFacade(CPApplication.Current.CurrentPage);
            BatchVM = new BatchImportCustomerVM();
            fromLinkList = new List<ValidationEntity>();
        }

        private void FileUploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            if (args.UploadInfo[0].UploadResult == Basic.Controls.Uploader.SingleFileUploadStatus.Failed)
            {
                CurrentWindow.Alert(ResBatchImportCustomer.Message_UploadFailed, MessageType.Error);
                return;
            }
            //上传成功，解析文件
            BatchVM.ImportFilePath = args.UploadInfo[0].ServerFilePath;
            btnImport.IsEnabled = true;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (this.model.TemplateType.ToString() == "VIP")
            {
                fromLinkList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResBatchImportCustomer.Validate_FromLinkNotNull));
                if (!ValidationHelper.Validation(this.tbCustomerSource, fromLinkList))
                    return;
            }
            else
            {
                fromLinkList.Clear();
            }
            ValidationManager.Validate(this.gdBatchImportSetting);
            if (BatchVM.HasValidationErrors)
                return;
            CustomerBatchImportInfo importInfo = BatchVM.ConvertVM<BatchImportCustomerVM, CustomerBatchImportInfo>();
            new CustomerFacade(CPApplication.Current.CurrentPage).BatchImportCustomers(importInfo, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (!string.IsNullOrEmpty(args.Result))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(args.Result);
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            });
        }

        private void FileUploader_UploadStarted(object sender, EventArgs args)
        {
            btnImport.IsEnabled = false;
        }
    }
}