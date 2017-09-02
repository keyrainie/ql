using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Browser;
using System.Windows.Threading;

using ECCentral.Portal.Basic.Controls.Uploader.Resource;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Controls.Uploader
{
    public class UploadClient : INotifyPropertyChanged
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
        private UploadStatus status;
        //单次post的字节大小
        private int bytes;

        #endregion

        #region Public Property

        public Uri UploadUrl { get; set; }        

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
        /// <summary>
        /// 客户端文件名
        /// </summary>
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

        /// <summary>
        /// 生成唯一的文件名
        /// </summary>
        private string FileIdentity { get; set; }        
       
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

        public UploadStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                StatusDesc = ResUploader.ResourceManager.GetString(string.Format("Uploader_Status_{0}", this.status));
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

        public FileUploader Owner { get; set; }

        #endregion

        #region Constructor

        public UploadClient(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            Status = UploadStatus.Pending;
        }

        public UploadClient(Dispatcher dispatcher, Uri uploadUrl)
            : this(dispatcher)
        {
            UploadUrl = uploadUrl;
        }

        public UploadClient(Dispatcher dispatcher, Uri uploadUrl, FileInfo fileToUpload, FileUploader owner)
            : this(dispatcher, uploadUrl)
        {
            File = fileToUpload;
            Owner = owner;
        }

        #endregion

        #region Methods

        private static string CreateFileIdentity(string domainName, FileInfo file)
        {
            string prefix = domainName;
            char[] invalidArray = Path.GetInvalidPathChars();
            foreach (char c in invalidArray)
            {
                prefix = prefix.Replace(c, '\0'); // 路径非法字符替换为空白
            }
            int index = file.Name.IndexOf(".");           
            string extensionName = file.Name.Substring(index);
            DateTime time = DateTime.Now;
            string subFolder = prefix + "\\" + time.ToString("yyyy-MM") + "\\" + time.ToString("yyyy-MM-dd");
            string path = Path.Combine(subFolder, Guid.NewGuid().ToString() + extensionName);
             return Convert.ToBase64String(Encoding.UTF8.GetBytes(path));
           }

        public void Upload()
        {
            if (File == null || UploadUrl == null)
            {
                return;
            }

            this.FileIdentity = CreateFileIdentity(this.Owner.DomainName, this.File);

            //如果被Cancel，则需要全部重新上传
            if (Status == UploadStatus.Canceled)
            {
                BytesUploaded = 0;
            }

            Status = UploadStatus.Uploading;

            cancel = false;

            LoopUploadFile();
        }

        /// <summary>
        /// 循环上传文件
        /// </summary>
        private void LoopUploadFile()
        {            
            UriBuilder ub = new UriBuilder(UploadUrl);

            var queryString = string.Format("{0}FileIdentity={1}&StartPosition={2}",
                string.IsNullOrEmpty(ub.Query) ? "" : ub.Query.Remove(0, 1) + "&",
                HttpUtility.UrlEncode(this.FileIdentity),
                BytesUploaded);
            
            ub.Query = queryString;

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
                this.Owner.ShowMessage(ResUploader.Uploader_FileInUsed, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
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

            bytes = tempTotal;

            fileStream.Close();
            requestStream.Close();
            webrequest.BeginGetResponse(new AsyncCallback(WriteCallback), webrequest);
        }

        private void WriteCallback(IAsyncResult asynchronousResult)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(CPApplication.Current.LanguageCode);
            if (this.Status == UploadStatus.Canceling || cancel)
            {
                this.Status = UploadStatus.Canceled;
                return;
            }
            string responseString = "";
            SingleFileUploadStatus uploadResult = SingleFileUploadStatus.Success;
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
                this.TipsInfo = ResUploader.Uploader_SecurityError;
                this.Status = UploadStatus.Error;
                uploadResult = SingleFileUploadStatus.Failed;
                this.cancel = true;
            }
            catch (Exception ex)
            {
                this.Owner.ShowMessage(ex.Message, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                this.TipsInfo = ex.Message;
                this.Status = UploadStatus.Error;
                uploadResult = SingleFileUploadStatus.Failed;
                this.cancel = true;
            }                                            

            BytesUploaded += this.bytes;

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

            if (uploadResult == SingleFileUploadStatus.Failed)
            {
                this.Status = UploadStatus.Error;
                this.TipsInfo = ResUploader.Uploader_UploadFailed;
            }
            else if (cancel)
            {
                if (remove)
                {
                    Status = UploadStatus.Removed;
                }
                else
                {
                    Status = UploadStatus.Canceled;
                }
            }
            else if (BytesUploaded < FileLength)
            {
                LoopUploadFile();
            }
            else
            {
                Status = UploadStatus.Complete;
            }

            if (Status == UploadStatus.Complete || Status == UploadStatus.Error || Status == UploadStatus.Canceled)
            {
                if (UploadCompleted != null)
                {
                    UploadCompleted(this, new UploadCompletedEventArgs(uploadResult, this.FileIdentity));
                }
            }
        }

        public void CancelUpload()
        {
            cancel = true;
            this.Status = UploadStatus.Canceling;
            BytesUploaded = 0;           
        }

        public void RemoveUpload()
        {
            cancel = true;
            remove = true;
            if (Status != UploadStatus.Uploading)
                Status = UploadStatus.Removed;

            if (UploadCanceled != null)
            {
                List<FileInfo> canceledFiles = new List<FileInfo>();
                canceledFiles.Add(this.File);
                UploadCanceled(this, new UploadCanceledEventArgs(canceledFiles));
            }
        }               
        
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion       
    }
}
