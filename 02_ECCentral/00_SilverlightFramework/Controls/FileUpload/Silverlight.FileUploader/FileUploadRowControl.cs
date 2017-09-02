using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Newegg.Oversea.Silverlight.GifUtility;
using System.Collections.Generic;
using System.IO;

namespace Newegg.Oversea.Silverlight.FileUploader
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
            FileUpload fu = DataContext as FileUpload;
            fu.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(fu_PropertyChanged);

            LoadImage(fu);        
        }

        void fu_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FileUpload fu = sender as FileUpload;
            if (e.PropertyName == "DisplayThumbnail")
            {
                LoadImage(fu);
            }

            else if (e.PropertyName == "Status")
            {
                switch (fu.Status)
                {
                    case FileUploadStatus.Pending:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case FileUploadStatus.Uploading:
                        VisualStateManager.GoToState(this, "Uploading", true);
                        break;
                    case FileUploadStatus.Complete:
                        VisualStateManager.GoToState(this, "Complete", true);
                        break;
                    case FileUploadStatus.Error:
                        VisualStateManager.GoToState(this, "Error", true);
                        break;
                    case FileUploadStatus.Canceled:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case FileUploadStatus.Canceling:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    case FileUploadStatus.Removed:
                        VisualStateManager.GoToState(this, "Pending", true);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadImage(FileUpload fu)
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
            FileUpload fu = DataContext as FileUpload;
            if (fu != null)
            {
                fu.RemoveUpload();
            }
        }
    }
}
