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
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface ILogin : IComponent
    {
        void AutoLogin(Action<bool> callBack);

        void Login(string userName, string password, Action<bool> callBack);

        void Logout(Action callback);
    }
}
