using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Resources;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class XapModuleLoader:IXapModuleLoader
    {
        public event EventHandler<LoadCompletedEventArgs> LoadCompleted;
        public event EventHandler<LoadProgressEventArgs> LoadProgress;
        private WebClient m_client;
        private Request m_request;

        public object UserState { get; set; }

        public XapModuleLoader()
        {
            m_client = new WebClient();
        }

        #region IXapModuleLoader Members

        public void Load(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            m_request = request;
            m_client.OpenReadCompleted += new OpenReadCompletedEventHandler(m_client_OpenReadCompleted);
            m_client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(m_client_DownloadProgressChanged);
            m_client.OpenReadAsync(new Uri(request.XapUrl, UriKind.Absolute));
        }

        void m_client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnLoadProgress(this, new LoadProgressEventArgs(m_request, e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived, this.UserState));
        }

        public void CancelAsync()
        {
           m_client.CancelAsync();
        }

        protected virtual void OnLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if(LoadCompleted != null)
            {
                LoadCompleted(sender,e);
            }
        }

        protected virtual void OnLoadProgress(object sender, LoadProgressEventArgs e)
        {
            if (LoadProgress != null)
            {
                LoadProgress(sender, e);
            }
        }

        void m_client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnLoadCompleted(this, new LoadCompletedEventArgs(m_request, null, e.Error, false, this.UserState));
            }
            else
            {
                OnLoadCompleted(this, new LoadCompletedEventArgs(m_request, e.Result, e.Error, false, this.UserState));
            }
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "XapModuleLoader"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            
        }

        public object GetInstance(System.Windows.Controls.TabItem tab)
        {
            return new XapModuleLoader();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
