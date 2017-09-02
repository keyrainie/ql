using System;
using System.Windows;
using System.Windows.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public enum MessageType
    {
        Information,
        Error,
        Warning,
        Confirm
    }

    public delegate void ResultHandler(object sender, ResultEventArgs e);

    public class ResultEventArgs : EventArgs
    {
        public DialogResultType DialogResult { get; set; }
        public object Data { get; set; }
        public bool isForce { get; set; }
    }

    public interface IDialog : IComponent
    {
        event EventHandler Closed;
        event EventHandler<ClosingEventArgs> Closing;
        ResultEventArgs ResultArgs { get; set; }
        IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, Panel container, IPageBrowser pageBrowser);
        IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, Panel container);
        void Close();
        void Close(bool isForce);
    }
}
