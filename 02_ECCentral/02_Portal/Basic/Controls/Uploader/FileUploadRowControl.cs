using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using ECCentral.Portal.Basic.Utilities.GifUtility;

namespace ECCentral.Portal.Basic.Controls.Uploader
{
    public class FileUploadRowControl : ContentControl
    {
        private const string RemoveButtonName = "RemoveButton";
        private const string PreviewImageName = "PreviewImage";
        private const string UploadProgressBarName = "UploadProgressBar";

        private Button m_removeBtn;
        private Image m_previewImage;
        private ProgressBar m_uploadProgressBar;        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.m_uploadProgressBar = GetTemplateChild(UploadProgressBarName) as ProgressBar;
            this.m_removeBtn = GetTemplateChild(RemoveButtonName) as Button;
            this.m_previewImage = GetTemplateChild(PreviewImageName) as Image;
            if (m_removeBtn != null)
            {
                m_removeBtn.Click += new RoutedEventHandler(m_removeBtn_Click);
            }
        }

        public FileUploadRowControl()
        {
            DefaultStyleKey = typeof(FileUploadRowControl);
            Loaded += new RoutedEventHandler(FileUploadRowControl_Loaded);            
        }

        void FileUploadRowControl_Loaded(object sender, RoutedEventArgs e)
        {
            UploadClient fu = DataContext as UploadClient;
            fu.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(fu_PropertyChanged);

            LoadImage(fu);        
        }

        void fu_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UploadClient fu = sender as UploadClient;
            if (e.PropertyName == "DisplayThumbnail")
            {
                LoadImage(fu);
            }

            else if (e.PropertyName == "Status")
            {
                switch (fu.Status)
                {
                    case UploadStatus.Pending:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case UploadStatus.Uploading:
                        VisualStateManager.GoToState(this, "Uploading", true);
                        break;
                    case UploadStatus.Complete:
                        VisualStateManager.GoToState(this, "Complete", true);
                        break;
                    case UploadStatus.Error:
                        VisualStateManager.GoToState(this, "Error", true);
                        break;
                    case UploadStatus.Canceled:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case UploadStatus.Canceling:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case UploadStatus.Removed:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadImage(UploadClient fu)
        {
            if (fu != null && fu.DisplayThumbnail)
            {
                try
                {
                    if ((fu.Name.ToLower().EndsWith("jpg") || fu.Name.ToLower().EndsWith("jpeg")|| fu.Name.ToLower().EndsWith("png")))
                    {
                        BitmapImage imageSource = new BitmapImage();

                        using (var stream = fu.File.OpenRead())
                        {
                            imageSource.SetSource(stream);
                        }
                        m_previewImage.Source = imageSource;
                        m_previewImage.Visibility = Visibility.Visible;
                    }
                    else if (fu.Name.ToLower().EndsWith("gif"))
                    {
                        GifImage gif = null;
                        using (var stream = fu.File.OpenRead())
                        {
                            gif = GIFDecoder.Decode(stream);
                            if (gif.Frames.Count > 0)
                            {
                                m_previewImage.Source = gif.Frames[0].Image;
                                m_previewImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string message = e.Message;
                }
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    m_previewImage.Visibility = Visibility.Collapsed;
                });
            }
        }

        void m_removeBtn_Click(object sender, RoutedEventArgs e)
        {
            UploadClient fu = DataContext as UploadClient;
            if (fu != null)
            {
                fu.RemoveUpload();
            }
        }
    }
}
