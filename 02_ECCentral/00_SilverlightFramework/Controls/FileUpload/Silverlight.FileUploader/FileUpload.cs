using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.FileUploader.Resource;
using Newegg.Oversea.Silverlight.GifUtility;
using System.Collections.Generic;
using System.Windows.Browser;
using System.Threading;

namespace Newegg.Oversea.Silverlight.FileUploader
{
    public class FileUpload : INotifyPropertyChanged
    {
        public event ProgressChangedEvent UploadProgressChanged;
        public event EventHandler StatusChanged;
        public event UploadCompletedEvent UploadCompleted;
        public event UploadCanceledEvent UploadCanceled;

        public long ChunkSize = 4 * 1024 * 1024;

        #region Private Fields

        private Dispatcher Dispatcher;
        private string tipsInfo;
        private string statusDesc;
        private bool cancel;
        private bool remove;
        private bool displayThumbnail;
        private FileInfo file;
        private long fileLength;
        private long bytesUploaded;
        private int uploadPercent;
        private FileUploadStatus status;

        #endregion

        #region Public Property

        public Uri UploadUrl { get; set; }

        //服务端返回的Guid,标识此文件在服务端生成的唯一名字-
        private string ServerFileGuid { get; set; }

        private bool m_tmpCanEditFileName;

        public FileInfo File
        {
            get { return file; }
            set
            {
                file = value;
                FileLength = file.Length;
            }
        }

        private string m_Name;
        public string Name
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(m_Name))
                {
                    m_Name = this.File.Name.Trim();
                }
                return m_Name;
            }
            set
            {
                m_Name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        //private string m_EditedFileName;
        //public string EditedFileName
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(m_EditedFileName))
        //        {
        //            m_EditedFileName = this.File.Name.Trim();
        //        }
        //        return m_EditedFileName;
        //    }
        //    set
        //    {
        //        m_EditedFileName = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("EditedFileName"));
        //    }
        //}

        public long FileLength
        {
            get { return fileLength; }
            set
            {
                fileLength = value;

                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("FileLength"));
                });
            }
        }

        public bool ResizeImage { get; set; }

        public int ImageSize { get; set; }

        public long BytesUploaded
        {
            get { return bytesUploaded; }
            set
            {
                bytesUploaded = value;

                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("BytesUploaded"));
                });
            }
        }

        public int UploadPercent
        {
            get { return uploadPercent; }
            set
            {
                uploadPercent = value;

                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("UploadPercent"));
                });
            }
        }

        private bool m_CanEditFileName = false;

        public bool CanEditFileName
        {
            get
            {
                return m_CanEditFileName;
            }
            set
            {
                m_CanEditFileName = value;
                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("CanEditFileName"));
                });
            }
        }

        public string StatusDesc
        {
            get
            {
                return statusDesc;
            }
            set
            {
                statusDesc = value;
                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("StatusDesc"));
                });
            }
        }

        public string TipsInfo
        {
            get
            {
                return tipsInfo;
            }
            set
            {
                tipsInfo = value;
                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("TipsInfo"));
                });
            }
        }

        public FileUploadStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                StatusDesc = MessageResource.ResourceManager.GetString(string.Format("Uploader_Status_{0}", this.status));
                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                    if (StatusChanged != null)
                        StatusChanged(this, null);
                });
            }
        }

        public bool DisplayThumbnail
        {
            get { return displayThumbnail; }
            set
            {
                displayThumbnail = value;

                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("DisplayThumbnail"));
                });
            }
        }

        public FileUploadControl Owner { get; set; }

        #endregion

        #region Constructor

        public FileUpload(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            Status = FileUploadStatus.Pending;
        }

        public FileUpload(Dispatcher dispatcher, Uri uploadUrl)
            : this(dispatcher)
        {
            UploadUrl = uploadUrl;
        }

        public FileUpload(Dispatcher dispatcher, Uri uploadUrl, FileInfo fileToUpload, FileUploadControl owner)
            : this(dispatcher, uploadUrl)
        {
            File = fileToUpload;
            Owner = owner;
        }

        #endregion

        #region Methods

        public void Upload()
        {
            if (File == null || UploadUrl == null)
                return;

            //如果被Cancel，则需要全部重新上传
            if (Status == FileUploadStatus.Canceled)
            {
                BytesUploaded = 0;
            }
            Status = FileUploadStatus.Uploading;
            cancel = false;

            if (CanEditFileName)
            {
                m_tmpCanEditFileName = true;
                CanEditFileName = false;
            }
            
            UploadFileEx();
        }

        public void CancelUpload()
        {
            cancel = true;
            this.Status = FileUploadStatus.Canceling;
            BytesUploaded = 0;

            if (m_tmpCanEditFileName)
            {
                CanEditFileName = true;
            }
        }

        public void RemoveUpload()
        {
            cancel = true;
            remove = true;
            if (Status != FileUploadStatus.Uploading)
                Status = FileUploadStatus.Removed;

            if (UploadCanceled != null)
            {
                List<FileInfo> canceledFiles = new List<FileInfo>();
                canceledFiles.Add(this.File);
                UploadCanceled(this, new UploadCanceledEventArgs(canceledFiles));
            }
        }

        public void UploadFileEx()
        {
            long temp = FileLength - BytesUploaded;

            UriBuilder ub = new UriBuilder(UploadUrl);
            bool complete = temp <= ChunkSize;
            // TODO:        
            var queryString = string.Format("{3}Filename={0}&Guid={4}&StartByte={1}&Complete={2}&Param={5}",
                System.Windows.Browser.HttpUtility.UrlEncode(this.Name), //如果文件名中包含 & ，在Handler端会被认为是QueryString的分隔符，导致取出来的文件名不正确
                BytesUploaded,
                complete,
                string.IsNullOrEmpty(ub.Query) ? "" : ub.Query.Remove(0, 1) + "&",
                this.ServerFileGuid,
                this.strUploadParam);

            ub.Query = System.Windows.Browser.HttpUtility.UrlEncode(queryString);

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(ub.Uri);
            webrequest.Method = "POST";
            webrequest.BeginGetRequestStream(new AsyncCallback(ReadCallback), webrequest);
        }

        private void ReadCallback(IAsyncResult asynchronousResult)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CPApplication.Current.LanguageCode);
            HttpWebRequest webrequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the operation.
            Stream requestStream = webrequest.EndGetRequestStream(asynchronousResult);

            byte[] buffer = new Byte[4096];
            int bytesRead = 0;
            int tempTotal = 0;

            Stream fileStream = null;
            try
            {
                fileStream = File.OpenRead();
            }
            catch (IOException)
            {
                this.Owner.ShowMessage(MessageResource.Uploader_FileInUsed, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }

            fileStream.Position = BytesUploaded;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < ChunkSize && !cancel)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                //Comment by Hax 20100409 这里只是把文件读到内存中，其实还没有真正上传
                tempTotal += bytesRead;
            }


            // only close the stream if it came from the file, don't close resizestream so we don't have to resize it over again.

            fileStream.Close();
            requestStream.Close();
            webrequest.BeginGetResponse(new AsyncCallback(WriteCallback), webrequest);
        }

        private void WriteCallback(IAsyncResult asynchronousResult)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CPApplication.Current.LanguageCode);
            if (this.Status == FileUploadStatus.Canceling || cancel)
            {
                this.Status = FileUploadStatus.Canceled;
                return;
            }
            string responseString = "";
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)webrequest.EndGetResponse(asynchronousResult);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            catch (System.Security.SecurityException ex)
            {
                this.Owner.ShowMessage(ex.Message, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                this.TipsInfo = MessageResource.Uploader_SecurityError;
                this.Status = FileUploadStatus.Error;
                this.cancel = true;
            }
            catch (Exception ex)
            {
                this.Owner.ShowMessage(ex.Message, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                this.TipsInfo = ex.Message;
                this.Status = FileUploadStatus.Error;
                this.cancel = true;
            }

            //如果不包含这个字符串，则可能是未登录或登录过期，在Service端的Response中会包含此字符串
            //格式为NeweggFileUploaderResponse_{Guid}_{StreamLength}
            if (!responseString.Contains("NeweggFileUploaderResponse"))
            {
                return;
            }

            var bytesUploaded = 0;
            var returnMsg = "";
            UploadResult uploadResult = UploadResult.Success;
            var array = Regex.Split(responseString, @"\[\^v\^\]", RegexOptions.IgnoreCase);
            if (array.Length == 5)
            {
                this.ServerFileGuid = array[1];
                bytesUploaded = int.Parse(array[2]);
                returnMsg = array[3];
                uploadResult = array[4].ToUpper().Trim() == "SUCCESS" ? UploadResult.Success : UploadResult.Failed;
            }

            BytesUploaded += bytesUploaded;

            if (UploadProgressChanged != null)
            {
                int percent = (int)(((double)BytesUploaded / (double)FileLength) * 100);

                UploadPercent = percent;
                UploadProgressChangedEventArgs args = new UploadProgressChangedEventArgs(percent, bytesUploaded, BytesUploaded, FileLength, file.Name);
                this.Dispatcher.BeginInvoke(delegate()
                {
                    UploadProgressChanged(this, args);
                });
            }

            if (uploadResult == UploadResult.Failed)
            {
                this.Status = FileUploadStatus.Error;
                this.TipsInfo = returnMsg;
            }
            else if (cancel)
            {
                if (remove)
                {
                    Status = FileUploadStatus.Removed;
                }
                else
                {
                    Status = FileUploadStatus.Canceled;
                }
            }
            else if (BytesUploaded < FileLength)
            {
                UploadFileEx();
            }
            else
            {
                Status = FileUploadStatus.Complete;
            }

            if (Status == FileUploadStatus.Complete || Status == FileUploadStatus.Error || Status == FileUploadStatus.Canceled)
            {
                if (UploadCompleted != null)
                {
                    UploadCompleted(this, new UploadCompletedEventArgs(uploadResult, File.Name, returnMsg));
                }
            }
        }


        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private string strUploadParam
        {
            get
            {
                if (UploadParams == null || UploadParams.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    string result = string.Empty;
                    foreach (var p in UploadParams)
                    {
                        if (p.Key.Contains("?") || p.Key.Contains("|")
                            || p.Value.Contains("?") || p.Value.Contains("|"))
                        {
                            throw new ArgumentException("UploadParams can not contains \"?\" or \"|\". ");
                        }
                        result += string.Format("?{0}|{1}", p.Key, p.Value);
                    }
                    return HttpUtility.UrlEncode(result);
                }
            }
        }

        public Dictionary<string, string> UploadParams { get; set; }

    }
}
