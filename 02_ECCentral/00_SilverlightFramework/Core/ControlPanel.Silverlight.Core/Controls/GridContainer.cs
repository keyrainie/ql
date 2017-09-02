using System;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class GridContainer:Grid,IContainer
    {
        private Request m_currentRequest;
        private object m_mutex = new object();

        private IModuleManager ModuleManager
        {
            get
            {
                return ComponentFactory.GetComponent<IModuleManager>();
            }
        }

        #region EventHandler

        public event EventHandler<LoadedMoudleEventArgs> LoadModule;

        public event EventHandler<LoadProgressEventArgs> LoadProgress;

        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(GridContainer), null);
        #endregion

        public GridContainer()
            : base()
        {
            this.Name = string.IsNullOrEmpty(this.Name)?System.Guid.NewGuid().ToString("N"):this.Name;

            this.ModuleManager.LoadProgress += new EventHandler<LoadProgressEventArgs>(ModuleManager_LoadProgress);
        }

        void ModuleManager_LoadProgress(object sender, LoadProgressEventArgs e)
        {
            if (this.LoadProgress != null)
            {
                this.LoadProgress(this, e);
            }
        }

        protected virtual void OnLoadModule(object sender, LoadedMoudleEventArgs e)
        {
            if (e.Status == LoadModuleStatus.End)
            {
                if (e.Error == null && e.Request.ModuleInfo != null)
                {
                    this.Children.Clear();
                }
                //注销单实例引用，避免内存泄露
                this.ModuleManager.LoadProgress -= new EventHandler<LoadProgressEventArgs>(ModuleManager_LoadProgress);
            }

            lock (m_mutex)
            {
                m_currentRequest = null;
            }

            if (this.LoadModule != null)
            {
                this.LoadModule(this, e);
            }
        }

        public void Load(Request request)
        {
            lock (m_mutex)
            {
                if (m_currentRequest != null)
                {
                    this.CancelLoadModuleAsync(request);
                }
                m_currentRequest = request;
            }
            ModuleManager.LoadModule(request, OnLoadModule);
        }

        public void Load(string url)
        {
            this.Load(new Request(url));
        }

        public bool CancelLoadModuleAsync(Request request)
        {
            return true;
        }
    }
}
