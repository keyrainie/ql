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

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface INotificationBox : IComponent
    {
        void Show(string title, string content, TimeSpan displayTime);

        void Show(string title, FrameworkElement content, TimeSpan displayTime);
    }
}
