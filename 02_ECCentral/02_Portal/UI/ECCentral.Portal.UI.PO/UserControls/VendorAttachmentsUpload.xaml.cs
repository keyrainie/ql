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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorAttachmentsUpload : UserControl
    {
        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public VendorAttachmentsUpload()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VendorAttachmentsUpload_Loaded);
        }

        void VendorAttachmentsUpload_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= VendorAttachmentsUpload_Loaded;
        }

        private void ucFileUploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            if (args.UploadInfo[0].UploadResult == Basic.Controls.Uploader.SingleFileUploadStatus.Failed)
            {
                CurrentWindow.Alert(ResVendorMaintain.InfoMsg_UploadFailed);
                return;
            }
            //上传成功，返回文件地址：
            Dialog.ResultArgs.Data = args.UploadInfo[0].ServerFilePath;
            Dialog.Close(true);
        }
    }
}
