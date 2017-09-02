using System;
using System.Windows;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class PageContext
    {
        #region Field
        internal IWindow m_window;
        internal Request m_request;
        private PageBase m_page;
        #endregion

        #region Property
        public IWindow Window
        {
            get
            {
               return m_window;
            }
        }
        public Request Request
        {
            get
            {
                return m_request;
            }
        }
        #endregion

        #region EventHandle
        public event EventHandler OnLoad;
        public event EventHandler<PageCloseEventArgs> OnClose;
        public event EventHandler OnError;
        public event SizeChangedEventHandler OnSizeChanged;
        public event SizeChangedEventHandler OnWindowSizeChanged;
        #endregion

        private PageContext() 
        {
        }

        public PageContext(Request request, IWindow window)
        {
            m_request = request;
            m_window = window;
        }

        public PageContext(Request request, IWindow window, PageBase page)
            : this(request, window)
        {
            m_page = page;
        }

        public virtual void OnPageLoad(object sender, EventArgs e)
        {
            if (this.OnLoad != null)
            {
                this.OnLoad(sender, e);
            }

            if (m_page != null)
            {
                m_page.OnPageLoad(sender, e);
            }
        }

        public virtual void OnPageClose(object sender, PageCloseEventArgs e)
        {
            if (this.OnClose != null)
            {
                this.OnClose(m_page, e);
            }

            if (m_page != null)
            {
                m_page.OnPageClose(sender, e);
            }
        }

        public virtual void OnPageError(object sender, EventArgs e)
        {
            if (this.OnError != null)
            {
                this.OnError(sender, e);
            }

            if (m_page != null)
            {
                m_page.OnPageError(sender, e);
            }
        }

        public virtual void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.OnSizeChanged != null)
            {
                this.OnSizeChanged(sender, e);   
            }

            if (m_page != null)
            {
                m_page.OnPageSizeChanged(sender, e);
            }
        }

        public virtual void OnViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.OnWindowSizeChanged != null)
            {
                this.OnWindowSizeChanged(sender, e);
            }

            if (m_page != null)
            {
                m_page.OnViewerSizeChanged(sender, e);
            }
        }
    }
}
