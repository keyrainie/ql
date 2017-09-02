using System;
using System.IO;
using System.ComponentModel;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class LoadCompletedEventArgs : AsyncCompletedEventArgs
    {
        private Stream m_result;
        private Request m_request;

        public Stream Result
        {
            get
            {
                return m_result;
            }
        }

        public Request Request
        {
            get
            {
                return m_request;
            }
        }
        
        public LoadCompletedEventArgs(Request request,Stream result,Exception error,bool canceled,object userState):
            base(error,canceled,userState)
        {
            m_request = request;
            m_result = result;
        }
    }

    public class LoadProgressEventArgs : ProgressChangedEventArgs
    {
        private int m_progressPercentage;
        private long m_totalBytes;
        private long m_bytesReceived;
        private Request m_request;

        /// <summary>
        /// 加载完成的百分比
        /// </summary>
        public int ProgressPercentage
        {
            get { return m_progressPercentage; }
        }

        /// <summary>
        /// 总共需要下载的字节数
        /// </summary>
        public long TotalBytes 
        {
            get { return m_totalBytes; }
        }

        /// <summary>
        /// 已经下载的字节数
        /// </summary>
        public long BytesReceived
        {
            get { return m_bytesReceived; }
        }

        /// <summary>
        /// 当前的请求
        /// </summary>
        public Request Request
        {
            get
            {
                return m_request;
            }
        }

        public LoadProgressEventArgs(Request request, int progressPercentage, long totalBytes, long bytesReceived, object userState)
            : base(progressPercentage, userState)
        {
            this.m_progressPercentage = progressPercentage;
            this.m_totalBytes = totalBytes;
            this.m_bytesReceived = bytesReceived;
            this.m_request = request;
        }
    }

    public interface IXapModuleLoader:Newegg.Oversea.Silverlight.Controls.Components.IComponent
    {
        event EventHandler<LoadCompletedEventArgs> LoadCompleted;
        event EventHandler<LoadProgressEventArgs> LoadProgress;
        object UserState { get; set; }
        void Load(Request request);
        void CancelAsync();
    }
}
