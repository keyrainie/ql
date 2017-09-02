using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    /// <summary>
    /// 弹出窗基类
    /// </summary>
    public class PopWindow : UserControl
    {
        /// <summary>
        /// 当前弹出窗引用
        /// </summary>
        public IDialog CurrentDialog
        {
            get;
            private set;
        }
        /// <summary>
        /// 当前窗口引用
        /// </summary>
        protected IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        /// <summary>
        /// 当前父级页面引用
        /// </summary>
        protected IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        /// <summary>
        /// 弹出消息对话框
        /// </summary>
        /// <param name="content"></param>
        protected void AlertInformationDialog(string content)
        {
            CurrentWindow.Alert(content, MessageType.Information);
        }

        /// <summary>
        /// 弹出消息对话框
        /// </summary>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        protected void AlertInformationDialog(string content, Action callback)
        {
            CurrentWindow.Alert(content, callback);
        }

        /// <summary>
        /// 弹出错误对话框
        /// </summary>
        /// <param name="content"></param>
        protected void AlertErrorDialog(string content)
        {
            CurrentWindow.Alert(content, MessageType.Error);
        }

        /// <summary>
        /// 弹出警告对话框
        /// </summary>
        /// <param name="content"></param>
        protected void AlertWarningDialog(string content)
        {
            CurrentWindow.Alert(content, MessageType.Warning);
        }

        /// <summary>
        /// 弹出待确认对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="OKHandler">点击OK按钮的回调</param>
        /// <param name="CancelHandler">点击Cancel按钮的回调</param>
        protected void AlertConfirmDialog(string title, string content, Action<object> OKHandler, Action<object> CancelHandler)
        {
            CurrentWindow.Confirm(title, content, OKHandler, CancelHandler);
        }

        /// <summary>
        /// 弹出待确认对话框
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="OKHandler">点击OK按钮的回调</param>
        /// <param name="CancelHandler">点击Cancel按钮的回调</param>
        protected void AlertConfirmDialog(string content, Action<object> OKHandler, Action<object> CancelHandler)
        {
            CurrentWindow.Confirm(content, OKHandler, CancelHandler);
        }

        /// <summary>
        /// 弹出待确认对话框
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="OKHandler">点击OK按钮的回调</param>>
        protected void AlertConfirmDialog(string content, Action<object> OKHandler)
        {
            CurrentWindow.Confirm(content, OKHandler);
        }

        /// <summary>
        /// 显示弹出框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="callback"></param>
        public void ShowDialog(string title, ResultHandler callback)
        {
            CurrentDialog = CurrentWindow.ShowDialog(title, this, callback);
        }

        /// <summary>
        /// 显示弹出框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="callback"></param>
        /// <param name="size"></param>
        public void ShowDialog(string title, ResultHandler callback, Size size)
        {
            CurrentDialog = CurrentWindow.ShowDialog(title, this, callback, size);
        }

        /// <summary>
        /// 关闭弹出框
        /// </summary>
        protected void CloseDialog()
        {
            CloseDialog(null, DialogResultType.Cancel);
        }

        /// <summary>
        /// 关闭弹出框
        /// </summary>
        /// <param name="resultType"></param>
        protected void CloseDialog(DialogResultType resultType)
        {
            CloseDialog(null, resultType);
        }

        /// <summary>
        /// 关闭弹出框
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dialogResult"></param>
        protected void CloseDialog(object data, DialogResultType dialogResult)
        {
            if (CurrentDialog != null)
            {
                CurrentDialog.ResultArgs = new ResultEventArgs()
                {
                    Data = data,
                    DialogResult = dialogResult,
                };
                CurrentDialog.Close();
            }
        }
    }
}