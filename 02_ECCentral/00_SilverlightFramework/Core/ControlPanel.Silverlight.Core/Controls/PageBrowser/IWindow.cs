using System;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class StatusChangedEventArgs:EventArgs
    {
        private bool m_active;

        public bool IsActive 
        {
            get
            {
                return m_active;
            }
        }

        public StatusChangedEventArgs(bool active):base()
        {
            m_active = active;
        }
    }

    public class ClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
    
    public interface IWindow:IDisposable
    {
        /// <summary>
        /// 窗体状态事件
        /// </summary>
        event EventHandler<StatusChangedEventArgs> WindowStatusChanged;

        /// <summary>
        /// 获取或设置页面高度
        /// </summary>
        double DocumentHeight { get; set; }

        /// <summary>
        /// 获取或设置页面宽度
        /// </summary>
        double DocumentWidth { get; set; }

        /// <summary>
        /// 获取页面容器高度
        /// </summary>
        double WindowHeight { get; }

        /// <summary>
        /// 获取页面容器宽度
        /// </summary>
        double WindowWidth { get; }

        /// <summary>
        /// 设置或获取当前PageTab是否显示横向滚动条
        /// </summary>
        ScrollBarVisibility DocumentHorizontalScrollBar { get; set; }

        /// <summary>
        /// 设置或获取当前PageTab是否显示纵向滚动条
        /// </summary>
        ScrollBarVisibility DocumentVerticalScrollBar { get; set; }

        /// <summary>
        /// 设置或获取是否允许当前PageTab自动重新设置滚动条
        /// </summary>
        bool AllowAutoResetScrollBar { get; set; }

        /// <summary>
        /// 获取PageBrowser的组件集合
        /// </summary>
        ComponentCollection ComponentCollection { get; }

        /// <summary>
        /// 获取PageBrowser的操作信息提示组件
        /// </summary>
        IMessageBox MessageBox { get; }

        /// <summary>
        /// 获取PageBrowser的验证组件，用户判断页面级内容的授权情况
        /// </summary>
        IAuth AuthManager { get; }

        /// <summary>
        /// 获取PageBrowser组件的loading图标组件
        /// </summary>
        ILoadingSpin LoadingSpin { get; }

        /// <summary>
        /// 获取PageBrowser组件的Logger组件, 提供客户端日志记录功能
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// 获取Mailer组件, 提供邮件发送
        /// </summary>
        IMail Mailer { get; }

        /// <summary>
        /// 获取PageBrowser组件的IConfiguration组件;
        /// 用于获取在Application Config页面配置的Key,Value值;
        /// (注意：请与Request中的Configuration区分开来)
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// 获取PageBrowser组件的ICache组件，当前的默认实现会将Cache的类容放到客户端的隔离存储区域内；
        /// </summary>
        ICache Cacher { get; }

        /// <summary>
        /// 当前Window是否是活动状态
        /// </summary>
        bool Status { get; }

        /// <summary>
        /// 获取FaultHandle组件，用来对服务端传回的结果进行异常处理（Note:该组件值只针对纯WCF服务端）；
        /// </summary>
        IFaultHandle FaultHandle { get; }

        /// <summary>
        /// 获取PageBrowser组件的IUserProfile组件，提供对当前用户的Profile数据存取访问；
        /// </summary>
        IUserProfile Profile { get; }

        /// <summary>
        /// 获取窗体右下角通知组件
        /// </summary>
        INotificationBox NotificationBox { get; }

        IEventTracker EventTracker { get; }

        //导航到指定url
        void Navigate(string url);
        void Navigate(string url, object args);
        /// <summary>
        /// 导航到指定的URL页面，该Url支持相对路径(/Order/Maintain)，也支持绝对路径(http://localhost/Portal/Default.aspx#/Order/Maintain)
        /// </summary>
        /// <param name="url">页面的相对/绝对路径</param>
        /// <param name="args">页面间传递的复杂类型，该值就是Request对象中的UserState</param>
        /// <param name="isNewTab">是在当前页打开，还是新打开一个Tab页面</param>
        void Navigate(string url, object args, bool isNewTab);
        /// <summary>
        /// 导航到指定的Request实例页面
        /// </summary>
        void Navigate(Request request, bool isNewTab);
        /// <summary>
        /// 前进
        /// </summary>
        void Forward();
        /// <summary>
        /// 后退
        /// </summary>
        void Back();

        /// <summary>
        /// 关闭当前页面
        /// </summary>
        void Close();

        /// <summary>
        /// 关闭当前页面
        /// </summary>
        void Close(bool isForce);

        /// <summary>
        /// 刷新当前页面
        /// </summary>
        void Refresh();


        //提示弹出窗体
        void Alert(string content);
        void Alert(string content, MessageType type);
        void Alert(string title, string content);
        void Alert(string title, string content, MessageType type);
        void Alert(string title, string content, MessageType type,ResultHandler callback);
        /// <summary>
        /// 弹出提示弹出窗体
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="type">提示的类型</param>
        /// <param name="callback">确认后的回调</param>
        /// <param name="container">需要Show出Alert的容器</param>
        void Alert(string title, string content, MessageType type, ResultHandler callback,Panel container);

        //确认弹出窗体
        void Confirm(string content, ResultHandler callback);
        void Confirm(string title, string content, ResultHandler callback);
        void Confirm(string title, string content, ResultHandler callback,ButtonType type);
        void Confirm(string title, string content, ResultHandler callback,ButtonType type, Panel container);

        //模态/非模态的自定义弹出窗体
        IDialog ShowDialog(string title, FrameworkElement content);
        IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback);
        IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size);
        IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, Panel container);
        IDialog ShowDialog(string title, string url, ResultHandler callback);
        IDialog ShowDialog(string title, string url, ResultHandler callback, Size size);
        IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, Panel container);

        void CloseDialog(object UserState);
        void CloseDialog(object UserState,bool isForce);
        event EventHandler<ResultEventArgs> DialogClosed;
        event EventHandler<ClosingEventArgs> DialogClosing;

        /// <summary>
        /// 此方法为框架内部使用，请外部Domain不要调用
        /// </summary>
        /// <param name="args"></param>
        void ClosingDialog(ClosingEventArgs args);


        //获取指定的组件
        IComponent GetComponentByName(string name);
        T GetComponent<T>() where T : IComponent;
        void UpdatePageTitle();
        void ResetPageViewer();
    }
}
