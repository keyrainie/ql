using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

using Newegg.Oversea.Silverlight.Controls;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Base
{
    /*
     * Notice: the PageBase must be normal class, 
     * if change to abstract,then can't display in a ui design interface of vs2010 
     */
    public class PageBase : UserControl, IPage
    {
        private PageContext m_context;
        private string m_title;
        internal bool IsLoaded
        {
            get;
            set;
        }

        #region Properties

        /// <summary>
        /// 获取嵌入在Silverlight内部类似于Browser的窗体实例，里面包含了框架级别的功能；
        /// 比如Navigate,ICache,ILog,IUserProfile等功能，都能在该对象中找到对应的API属性；
        /// 详细请参见IWindow的成员说明;
        /// </summary>
        public IWindow Window { get; private set; }

        /// <summary>
        /// 获取当前Page请求的上下文，可以在这里获取到Url, QueryString等；
        /// 很类似于ASP.NET中的Page.Request对象；
        /// 详细请参见Request的成员说明部分；
        /// </summary>
        public Request Request { get; private set; }
        [Obsolete("该属性已过期，直接使用Newegg.Oversea.Silverlight.ControlPanel.Core.Base.PageBase提供的其他属性进行访问.")]
        public PageContext Context
        {
            get { return m_context; }
            internal set
            {
                m_context = new PageContext(value.Request, value.Window, this);
                Window = value.Window;
                Request = value.Request;
            }
        }

        /// <summary>
        /// 获取或设置Page的标题
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
                if (Window != null)
                {
                    Window.UpdatePageTitle();
                }
            }
        }

        /// <summary>
        /// 获取或设置面包屑右边区域的内容，这里可以是一个容器，可以自定义的HyperlinkButton,TextBlock等控件；
        /// </summary>
        public DependencyObject Description { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 当前页面加载并呈现完毕的时候发生；
        /// </summary>
        public event EventHandler OnLoad;

        /// <summary>
        /// 当页面正在被执行关闭的时候发生；
        /// </summary>
        public event EventHandler<PageCloseEventArgs> OnClose;

        /// <summary>
        /// 这个在OVS提供的默认实现包中没有实现该Event，现在的异常会被框架统一处理;
        /// </summary>
        public event EventHandler OnError;

        /// <summary>
        /// 当页面发生变化的时候会响应(Note:如果对SL的布局系统不是很了解，请慎用该事件，使用不当回导致dead cycle)；
        /// </summary>
        public event SizeChangedEventHandler OnSizeChanged;

        /// <summary>
        /// 这个在OVS提供的默认实现包中没有实现该Event；
        /// </summary>
        public event SizeChangedEventHandler OnWindowSizeChanged;

        #endregion

        #region Methods

        /// <summary>
        /// 当前页面加载并呈现完毕的时候发生；
        /// </summary>
        public virtual void OnPageLoad(object sender, EventArgs e)
        {
            Window.DocumentHorizontalScrollBar = ScrollBarVisibility.Disabled;
            Window.DocumentVerticalScrollBar = ScrollBarVisibility.Disabled;
            if (this.OnLoad != null)
            {
                this.OnLoad(sender, e);
            }
            IsLoaded = true;
        }

        /// <summary>
        /// 当页面正在被执行关闭的时候发生；
        /// </summary>
        public virtual void OnPageClose(object sender, PageCloseEventArgs e)
        {
            if (this.OnClose != null)
            {
                this.OnClose(this, e);
            }
        }

        /// <summary>
        /// 这个在OVS提供的默认实现包中没有实现该Method，现在的异常会被框架统一处理;
        /// </summary>
        public virtual void OnPageError(object sender, EventArgs e)
        {
            if (this.OnError != null)
            {
                this.OnError(sender, e);
            }
        }

        /// <summary>
        /// 当页面发生变化的时候会响应(Note:如果对SL的布局系统不是很了解，请慎用该事件，使用不当回导致dead cycle)；
        /// </summary>
        public virtual void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.OnSizeChanged != null)
            {
                this.OnSizeChanged(sender, e);
            }
        }

        /// <summary>
        /// 这个在OVS提供的默认实现包中没有实现该Method；
        /// </summary>
        public virtual void OnViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.OnWindowSizeChanged != null)
            {
                this.OnWindowSizeChanged(sender, e);
            }
        }
        #endregion
    }


    public class PageCloseEventArgs : EventArgs
    {
        /// <summary>
        /// 设置或获取是否取消页面关闭
        /// </summary>
        public bool Cancel { get; set; }
    }
}
