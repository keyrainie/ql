using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;


namespace Newegg.Oversea.Silverlight.Controls
{
    public delegate DependencyObject DelegateGetChildTemplate(string name);
    public delegate void ApplyTemplateEventHandler(object sender,ApplyTemplateEventArgs args);
    
    public class ApplyTemplateEventArgs : EventArgs
    {
        private DelegateGetChildTemplate m_handler;

        public ApplyTemplateEventArgs(DelegateGetChildTemplate handler)
            : base()
        {
            m_handler = handler;
        }

        public DependencyObject GetChildTemplate(string name)
        {
            if (m_handler != null)
            {
                return m_handler(name);
            }

            return null;
        }
    }

    public enum PageBrowserModel
    { 
        SinglePage = 0,
        MultiPage = 1
    }

    public interface IPageBrowser : IWindow
    {
        /// <summary>
        /// Navigate时，触发的事件
        /// </summary>
        event EventHandler<LoadedMoudleEventArgs> Navigating;
        /// <summary>
        /// PageBrowser装载样式时触发的事件
        /// </summary>
        event ApplyTemplateEventHandler ApplyTemplateHandle;
        /// <summary>
        /// PageTab切换时触发的事件
        /// </summary>
        event SelectionChangedEventHandler SelectionChanged;
        /// <summary>
        /// 导航结束后触发的事件
        /// </summary>
        event EventHandler NavigateCompleted;

        /// <summary>
        /// 关闭Tab时触发的事件
        /// </summary>
        event EventHandler TabClosing;

        /// <summary>
        /// PageTab集合
        /// </summary>
        ItemCollection Items { get; }
        /// <summary>
        /// 获取当前选中的PageTab的Content
        /// </summary>
        object SelectedContent {get;}
        /// <summary>
        /// 获取当前选中的PageTab的索引
        /// </summary>
        int SelectedIndex {get;}
        /// <summary>
        ///获取 当前选中的PageTab
        /// </summary>
        object SelectedItem { get; }

        /// <summary>
        /// 获取当前的页面
        /// </summary>
        IPage SelectedPage { get; }

        /// <summary>
        /// 获取所有打开的页面
        /// </summary>
        IList<IPage> OpenPages { get; }

        /// <summary>
        /// 设置或获取设置默认打开页面
        /// </summary>
        string DefaultPage { get; set; }
        /// <summary>
        /// 设置或获取最大开启的PageTab数量
        /// </summary>
        int MaxPageTabTotal { get; set; }
        /// <summary>
        /// 设置或获取PageBrowser的导航模式
        /// </summary>
        PageBrowserModel Model { get; set; }

    }
}