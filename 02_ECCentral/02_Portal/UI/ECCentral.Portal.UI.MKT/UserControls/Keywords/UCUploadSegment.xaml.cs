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
using ECCentral.Portal.Basic.Controls.Uploader;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCUploadSegment : UserControl
    {
        public IDialog Dialog { get; set; }
        private SegmentQueryFacade facade;

        public UCUploadSegment()
        {
            InitializeComponent();
            this.uploader.AllFileUploadCompleted += new Basic.Controls.Uploader.AllUploadCompletedEvent(uploader_AllFileUploadCompleted);
			
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        private void uploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            uploader.Clear();
            if (args.UploadInfo[0].UploadResult == SingleFileUploadStatus.Failed)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UploadFailed, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }

            facade = new SegmentQueryFacade(CPApplication.Current.CurrentPage);

            facade.BatchImportSegment(args.UploadInfo[0].ServerFilePath, (obj, args2) =>
            {
                if (args2.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }
    }
}
