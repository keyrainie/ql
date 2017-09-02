using System;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    [View("Error",IsSingleton=false,NeedAccess=false)]
    public partial class ErrorPage : UserControl,IPage
    {
        private PageContext m_context;
        private PageException m_exception;
        private string m_title;

        public ErrorPage()
        {
            InitializeComponent();
        }

        public ErrorPage(PageContext context):this()
        {
            m_context = context;

            if ((m_exception = this.Context.Request.UserState as PageException) != null)
            {
                m_title = m_exception.Title;
                this.DataContext = m_exception;
#if DEBUG
                Request request = null;

                if(m_exception.Request is Request)
                {
                    request = m_exception.Request as Request;
                }
                else if(m_exception.Request is PageContext)
                {
                    request = (m_exception.Request as PageContext).Request;
                }

                TextBlockContent.Text = string.Format("{0}\r\n ●  Target URL:" + (request != null ? request.URL : string.Empty), m_exception is PageException ? m_exception.Message : m_exception.ToString());
#endif
            }
        }

        #region IPage Members

        public string Title
        {
            get { return m_title; }
        }

        public PageContext Context
        {
            get { return m_context; }
        }

        public DependencyObject Description { get; set; }
        #endregion

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            m_context.m_window.Navigate(CPApplication.Current.DefaultPage, null, false);
        }
    }
}
