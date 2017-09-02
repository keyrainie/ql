using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic;
using System.Windows.Media.Imaging;
using System.Text;
using System.IO;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCUnifiedImageUpload : UserControl
    {
        public IDialog Dialog { get; set; }

        private  string _domainName = ConstValue.DomainName_MKT;

        public System.IO.FileInfo CurrentFile { get; set; }

        public string UpLoadFloder { get; set; }

        public string UploadUrl { get; set; }

        
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public string AppName { get; set; }

        public UCUnifiedImageUpload(string domainNmae)
            : this()
        {
            _domainName = domainNmae;
        }

        public UCUnifiedImageUpload()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(UCDeductList_Loaded);
        }


        void UCDeductList_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCDeductList_Loaded;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.Data = null;
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close(true);
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Multiselect = false };

            openFileDialog.Filter = "Image Files (*.jpg,*.gif,*.png)|*.jpg;*.gif;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFile = openFileDialog.File;
                txtSelectFile.Text = CurrentFile.Name;
                var previewImage = new BitmapImage();
                var myStream = CurrentFile.OpenRead();
                previewImage.SetSource(myStream);
                PreviewImage.Source = previewImage;

            }
        }
        private void btnUplaod_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFile!=null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => UpLoad());
            }
            else
            {
                CurrentWindow.Alert("还未选择图片");              
            }
            
        }
        
        /// <summary>
        /// 上传文件
        /// </summary>
        private void UpLoad()
        {

            var client = new FileUploadClient(_domainName, CurrentFile, 1);
            client.AppName = AppName;
            client.UploadProgressChanged += (se, args) =>
            {
                long t = args.TotalUploadedDataLength;  // 已经上传的数据大小
                UploadProgressBar.Value = (float)t / CurrentFile.Length;   // 上传数据的百分比
            };
            client.UploadErrorOccured += (se, args) =>
            {
                CurrentWindow.Alert(args.Exception.Message);                
            };
            client.UploadCompleted += (se, args) =>
            {
                string imageUrl = System.IO.Path.Combine(UploadUrl, args.ServerFilePath);
               
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.ResultArgs.Data = new { ImageUrl = imageUrl, ImageName = string.IsNullOrEmpty(txtUpLoadFile.Text) ? txtSelectFile.Text : txtUpLoadFile.Text };
                Dialog.Close(true); 
            };
            client.Upload();
        }
    }
}
   
   
