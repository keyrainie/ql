using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Text;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.Basic.Utilities
{
    public class FileUploadClient : DependencyObject
    {
        private enum UploadStatus
        {
            Stopped,
            Uploading,
            ToCancel,
            ToSuspend,
            Suspended
        }

        private class FileUploadTask
        {
            public long StartPosition { get; set; }
            public long Length { get; set; }
        }

        #region Private Member

        private string m_Url;

        private string m_DomainName;

        private FileInfo m_FileInfo;
        private string m_FileIdentity;

        private int m_ChunkSize = 1024 * 512;

        private UploadStatus m_UploadStatus = UploadStatus.Stopped;
        private UploadErrorOccuredEventEventArgs m_Exception = null;

        private long m_CurrentUploadLength;
        private object m_CurrentUploadLength_SyncObj = new object();

        private int m_MaxThreadCount;
        private Queue<FileUploadTask> m_TaskQueue;
        private object m_TaskQueue_SyncObj = new object();

        private int m_RunningThreadCount = 0;
        private object m_RunningThreadCount_SyncObj = new object();


        public string AppName { get; set; }

        #endregion

        #region Event

        private event UploadProgressChangedEvent m_UploadProgressChanged;
        public event UploadProgressChangedEvent UploadProgressChanged
        {
            add
            {
                m_UploadProgressChanged += value;
            }
            remove
            {
                m_UploadProgressChanged -= value;
            }
        }
        protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs args)
        {
            UploadProgressChangedEvent handler = m_UploadProgressChanged;
            if (handler != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, args);
                });
            }
        }

        private event UploadErrorOccuredEvent m_UploadErrorOccured;
        public event UploadErrorOccuredEvent UploadErrorOccured
        {
            add
            {
                m_UploadErrorOccured += value;
            }
            remove
            {
                m_UploadErrorOccured -= value;
            }
        }
        protected virtual void OnUploadErrorOccured(UploadErrorOccuredEventEventArgs args)
        {
            UploadErrorOccuredEvent handler = m_UploadErrorOccured;
            if (handler != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, args);
                });
            }
        }

        private event UploadCompletedEvent m_UploadCompleted;
        public event UploadCompletedEvent UploadCompleted
        {
            add
            {
                m_UploadCompleted += value;
            }
            remove
            {
                m_UploadCompleted -= value;
            }
        }
        protected virtual void OnUploadCompleted(UploadCompletedEventArgs args)
        {
            UploadCompletedEvent handler = m_UploadCompleted;
            if (handler != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, args);
                });
            }
        }

        private event UploadSuspendedEvent m_UploadSuspended;
        public event UploadSuspendedEvent UploadSuspended
        {
            add
            {
                m_UploadSuspended += value;
            }
            remove
            {
                m_UploadSuspended -= value;
            }
        }
        protected virtual void OnUploadSuspended(UploadSuspendedEventArgs args)
        {
            UploadSuspendedEvent handler = m_UploadSuspended;
            if (handler != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, args);
                });
            }
        }

        private event UploadCanceledEvent m_UploadCanceled;
        public event UploadCanceledEvent UploadCanceled
        {
            add
            {
                m_UploadCanceled += value;
            }
            remove
            {
                m_UploadCanceled -= value;
            }
        }
        protected virtual void OnUploadCanceled(UploadCanceledEventArgs args)
        {
            UploadCanceledEvent handler = m_UploadCanceled;
            if (handler != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, args);
                });
            }
        }

        #endregion

        #region Contructor

        public FileUploadClient(string domainName, string fileFullPath)
            : this(domainName, fileFullPath, 1)
        {

        }

        public FileUploadClient(string domainName, string fileFullPath, int maxThreadCount)
            : this(domainName, new FileInfo(fileFullPath), maxThreadCount)
        {

        }

        public FileUploadClient(string domainName, FileInfo fileToUpload)
            : this(domainName, fileToUpload, 1)
        {

        }

        public FileUploadClient(string domainName, FileInfo fileToUpload, int maxThreadCount)
        {
            m_DomainName = domainName;
            m_FileInfo = fileToUpload;
            m_MaxThreadCount = maxThreadCount <= 0 ? 1 : maxThreadCount;
            m_Url = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(m_DomainName, "ECCUploadHandlerBaseUrl"); 
        }

        #endregion

        #region Private Helper Method

        private static string CreateFileIdentity(string domainName, FileInfo file)
        {
            string prefix = domainName;
            char[] invalidArray = Path.GetInvalidPathChars();
            foreach (char c in invalidArray)
            {
                prefix = prefix.Replace(c, '\0'); // 路径非法字符替换为空白
            }
            string extensionName = Path.GetExtension(file.Name);
            DateTime time = DateTime.Now;
            string subFolder = prefix + "\\" + time.ToString("yyyy-MM") + "\\" + time.ToString("yyyy-MM-dd");
            string t = Path.Combine(subFolder, Guid.NewGuid().ToString() + extensionName);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(t));
        }

        private bool EndThread()
        {
            lock (m_RunningThreadCount_SyncObj)
            {
                m_RunningThreadCount = m_RunningThreadCount - 1;
                return m_RunningThreadCount == 0;
            }
        }

        #endregion

        public int ChunkSize
        {
            get { return m_ChunkSize; }
            set { m_ChunkSize = value; }
        }

        public void Upload()
        {
            if (m_UploadStatus != UploadStatus.Stopped)
            {
                return;
            }
            // 1. 初始化本次上传的相关环境变量
            m_CurrentUploadLength = 0;
            m_FileIdentity = CreateFileIdentity(m_DomainName, m_FileInfo);
            m_TaskQueue = new Queue<FileUploadTask>();
            m_Exception = null;

            // 2. 构建本次上传的任务队列
            long needUploadLength = m_FileInfo.Length;
            long start = 0;
            while (needUploadLength > 0)
            {
                long len = needUploadLength > m_ChunkSize ? m_ChunkSize : needUploadLength;
                m_TaskQueue.Enqueue(new FileUploadTask
                {
                    StartPosition = start,
                    Length = len
                });
                needUploadLength = needUploadLength - len;
                start = start + len;
            }
            if (m_TaskQueue.Count <= 0)
            {
                return;
            }

            // 3. 按照指定线程数，开异步线程执行任务
            int threadCount = m_MaxThreadCount < m_TaskQueue.Count ? m_MaxThreadCount : m_TaskQueue.Count;
            m_RunningThreadCount = threadCount;
            for (int i = 0; i < threadCount; i++)
            {
                ExecuteUploadTask(i);
            }
            m_UploadStatus = UploadStatus.Uploading;
        }

        public void SuspendUpload()
        {
            if (m_UploadStatus == UploadStatus.Uploading)
            {
                m_UploadStatus = UploadStatus.ToSuspend;
            }
        }

        public void ResumeUpload()
        {
            if (m_UploadStatus == UploadStatus.Suspended)
            {
                // 根据剩余未完成任务数量，决定异步线程数量
                int threadCount = m_MaxThreadCount < m_TaskQueue.Count ? m_MaxThreadCount : m_TaskQueue.Count;
                m_RunningThreadCount = threadCount;
                for (int i = 0; i < threadCount; i++)
                {
                    ExecuteUploadTask(i);
                }
                m_UploadStatus = UploadStatus.Uploading;
            }
        }

        public void CancelUpload()
        {
            if (m_UploadStatus == UploadStatus.Uploading)
            {
                m_UploadStatus = UploadStatus.ToCancel;
            }
            else if (m_UploadStatus == UploadStatus.Suspended)
            {
                AysnDeleteFileFromServer(() =>
                {
                    m_UploadStatus = UploadStatus.Stopped;
                    OnUploadCanceled(new UploadCanceledEventArgs
                    {
                        FileIdentity = m_FileIdentity,
                        FileInfo = m_FileInfo,
                        TotalUploadedDataLength = m_CurrentUploadLength
                    });
                });
            }
        }

        #region Execute Upload Task

        private void ExecuteUploadTask(int threadID)
        {
            // 判断ToCancel的优先级最高，不管是否发生了异常，还是最后一个task都已经上传完毕，
            // 一旦调用Cancel，异常、上传完毕都可以被忽略，都认为上传被取消
            if (m_UploadStatus == UploadStatus.ToCancel)
            {
                if (EndThread())
                {
                    DeleteFileFromServer();
                    m_UploadStatus = UploadStatus.Stopped;
                    OnUploadCanceled(new UploadCanceledEventArgs
                    {
                        FileIdentity = m_FileIdentity,
                        FileInfo = m_FileInfo,
                        TotalUploadedDataLength = m_CurrentUploadLength
                    });
                }
                return;
            }

            if (m_Exception != null)
            {
                if (EndThread())
                {
                    m_Exception.TotalUploadedDataLength = m_CurrentUploadLength;
                    var tmp = m_Exception;
                    m_Exception = null;
                    m_UploadStatus = UploadStatus.Suspended;
                    OnUploadErrorOccured(tmp);
                }
                return;
            }

            // 在判断是否需要暂停前，要先判断一下任务是否已经都完成了；如果不首先判断任务是否都完成，可能会出现一种情况：
            // 当最后一个任务在上传时，才调用了Suspend方法将状态置为ToSuspend；然后线程执行到这里判断状态为ToSuspend了后就进入暂停状态；
            // 而当调用了Resume方法的时候，状态会再次变为Uploading，但是此时Queue里已经没有任务了，那么也就不会再开启上传线程，
            // 那么状态永远都是Uploading，永远无法完成
            // 所以为了避免这种情况发生，在判断ToSuspend之前需要先看看任务是否都已经上传完毕，如果是则直接变成已上传完毕了，之前发生的暂停操作就无效了
            if (m_TaskQueue.Count <= 0)
            {
                if (EndThread())
                {
                    m_UploadStatus = UploadStatus.Stopped;
                    CallServiceUploadStopped();                    
                }
                return;
            }

            if (m_UploadStatus == UploadStatus.ToSuspend)
            {
                if (EndThread())
                {
                    m_UploadStatus = UploadStatus.Suspended;
                    OnUploadSuspended(new UploadSuspendedEventArgs
                    {
                        FileIdentity = m_FileIdentity,
                        FileInfo = m_FileInfo,
                        TotalUploadedDataLength = m_CurrentUploadLength
                    });
                }
                return;
            }

            FileUploadTask task = null;
            if (m_TaskQueue.Count > 0)
            {
                lock (m_TaskQueue_SyncObj)
                {
                    if (m_TaskQueue.Count > 0)
                    {
                        task = m_TaskQueue.Dequeue();
                    }
                }
            }
            if (task == null)
            {
                if (EndThread())
                {
                    m_UploadStatus = UploadStatus.Stopped;
                    CallServiceUploadStopped();
                }
                return;
            }
            try
            {
                string url = m_Url + "?FileIdentity=" + HttpUtility.UrlEncode(m_FileIdentity) + "&StartPosition=" + task.StartPosition;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.BeginGetRequestStream(new AsyncCallback(ReadCallback),
                    new Tuple<HttpWebRequest, FileUploadTask, int>(request, task, threadID));
            }
            catch (Exception ex)
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex);
                var tmp = new UploadErrorOccuredEventEventArgs
                {
                    FileIdentity = m_FileIdentity,
                    FileInfo = m_FileInfo,
                    Exception = ex
                };
                m_Exception = tmp;
                lock (m_TaskQueue_SyncObj)
                {
                    m_TaskQueue.Enqueue(task);
                }
                ExecuteUploadTask(threadID); // 继续任务处理
            }
        }

        private void ReadCallback(IAsyncResult a)
        {
            Tuple<HttpWebRequest, FileUploadTask, int> args = (Tuple<HttpWebRequest, FileUploadTask, int>)a.AsyncState;
            HttpWebRequest webrequest = args.Item1;
            FileUploadTask fileTask = args.Item2;
            try
            {
                using (Stream requestStream = webrequest.EndGetRequestStream(a))
                {
                    using (FileStream fs = m_FileInfo.OpenRead())
                    {
                        fs.Seek(fileTask.StartPosition, SeekOrigin.Begin);
                        byte[] buffer = new Byte[4096];
                        long tempTotal = 0;
                        while (tempTotal < fileTask.Length)
                        {
                            long bytesRead = fileTask.Length - tempTotal;
                            if (bytesRead > buffer.Length)
                            {
                                bytesRead = buffer.Length;
                            }
                            fs.Read(buffer, 0, (int)bytesRead);
                            requestStream.Write(buffer, 0, (int)bytesRead);
                            tempTotal += bytesRead;
                        }
                        fs.Close();
                    }
                    requestStream.Close();
                }
                webrequest.BeginGetResponse(new AsyncCallback(WriteCallback), args);
            }
            catch (Exception ex)
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex);
                var tmp = new UploadErrorOccuredEventEventArgs
                {
                    FileIdentity = m_FileIdentity,
                    FileInfo = m_FileInfo,
                    Exception = ex
                };
                m_Exception = tmp;
                lock (m_TaskQueue_SyncObj)
                {
                    m_TaskQueue.Enqueue(fileTask);
                }
                ExecuteUploadTask(args.Item3); // 继续任务处理
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            Exception ex = null;
            Tuple<HttpWebRequest, FileUploadTask, int> args1 = (Tuple<HttpWebRequest, FileUploadTask, int>)ar.AsyncState;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)args1.Item1.EndGetResponse(ar))
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = reader.ReadToEnd();
                    }
                    response.Close();
                }
            }
            catch (Exception e)
            {
                ComponentFactory.GetComponent<ILog>().LogError(e);
                ex = e;
            }
            if (ex == null)
            {
                UploadProgressChangedEventArgs ea = new UploadProgressChangedEventArgs()
                {
                    FileIdentity = m_FileIdentity,
                    FileInfo = m_FileInfo
                };
                lock (m_CurrentUploadLength_SyncObj)
                {
                    m_CurrentUploadLength = m_CurrentUploadLength + args1.Item2.Length;
                    ea.TotalUploadedDataLength = m_CurrentUploadLength;
                }
                OnUploadProgressChanged(ea);
            }
            else
            {
                var tmp = new UploadErrorOccuredEventEventArgs
                {
                    FileIdentity = m_FileIdentity,
                    FileInfo = m_FileInfo,
                    Exception = ex
                };
                m_Exception = tmp;
                lock (m_TaskQueue_SyncObj)
                {
                    m_TaskQueue.Enqueue(args1.Item2);
                }
            }
            ExecuteUploadTask(args1.Item3); // 继续任务处理
        }

        private void DeleteFileFromServer()
        {
            try
            {
                IAsyncResult rst = AysnDeleteFileFromServer(null);
                if (rst != null)
                {
                    rst.AsyncWaitHandle.WaitOne();
                }
            }
            catch(Exception ex) 
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex);
            }
        }

        private IAsyncResult AysnDeleteFileFromServer(Action callback)
        {
            try
            {
                string url = m_Url + "?FileIdentity=" + HttpUtility.UrlEncode(m_FileIdentity) + "&Action=cancel";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                IAsyncResult rst = request.BeginGetResponse(ar =>
                {
                    Tuple<HttpWebRequest, Action> tag = (Tuple<HttpWebRequest, Action>)ar.AsyncState;
                    try
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)tag.Item1;
                        using (HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(ar))
                        {
                            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            {
                                string responseString = reader.ReadToEnd();
                            }
                            response.Close();
                        }
                    }
                    catch(Exception ex1)
                    {
                        ComponentFactory.GetComponent<ILog>().LogError(ex1);
                    }
                    finally
                    {
                        if (tag.Item2 != null)
                        {
                            tag.Item2();
                        }
                    }
                }, new Tuple<HttpWebRequest, Action>(request, callback));
                return rst;
            }
            catch(Exception ex2)
            {
                ComponentFactory.GetComponent<ILog>().LogError(ex2);
                if (callback != null)
                {
                    callback();
                }
            }
            return null;
        }

        private void CallServiceUploadStopped()
        {
            string url = string.Format("{0}?FileIdentity={1}&appName={2}",m_Url, HttpUtility.UrlEncode(m_FileIdentity),AppName ); 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.BeginGetResponse(new AsyncCallback(CallServiceUploadStoppedCallback), request);
        }

        private void CallServiceUploadStoppedCallback(IAsyncResult a)
        {
            HttpWebRequest webrequest = a.AsyncState as HttpWebRequest;
            Exception ex = null;
            UploadResult result = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)webrequest.EndGetResponse(a))
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = reader.ReadToEnd();
                        result = JsonHelper.JsonDeserialize<UploadResult>(responseString);
                    }
                    response.Close();
                }
            }
            catch (Exception e)
            {
                ComponentFactory.GetComponent<ILog>().LogError(e);
                ex = e;
            }
            if (ex == null)
            {
                if (result != null && result.state == "SUCCESS")
                {
                    WebClient cl = new WebClient();
                    cl.OpenReadAsync(new Uri("http://www.baidu.com/"));
                    OnUploadCompleted(new UploadCompletedEventArgs
                    {
                        FileIdentity = m_FileIdentity,
                        FileInfo = m_FileInfo,
                        TotalUploadedDataLength = m_CurrentUploadLength,
                        ServerFilePath = result.url
                    });
                }
                else
                {

                    WebClient cl = new WebClient();
                    cl.OpenReadAsync(new Uri("http://www.baidu.com/12"));
                    UploadErrorOccuredEventEventArgs ep = new UploadErrorOccuredEventEventArgs();

                    ep.TotalUploadedDataLength = m_CurrentUploadLength;
                    ep.Exception = new Exception(result != null ? result.message : "");
                    ep.FileIdentity = m_FileIdentity;
                    ep.FileInfo = m_FileInfo;
                    m_UploadStatus = UploadStatus.Suspended;
                    OnUploadErrorOccured(ep);
                }
            }
            else
            {
                UploadErrorOccuredEventEventArgs ep = new UploadErrorOccuredEventEventArgs();
                ep.TotalUploadedDataLength = m_CurrentUploadLength;
                ep.Exception = ex;
                ep.FileIdentity = m_FileIdentity;
                ep.FileInfo = m_FileInfo;
                m_UploadStatus = UploadStatus.Suspended;
                OnUploadErrorOccured(ep); 
            }
        }

        #endregion
    }

    public class UploadResult
    {
        public string message { get; set; }

        public string state { get; set; }

        public string url { get; set; }
    }
}
