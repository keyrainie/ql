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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.CommonDomain
{
    public class App : IModule
    {
        #region IModule Members

        public static bool s_isRefreshForNavigation = false;

        public void Initialize()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(String.Format("/Newegg.Oversea.Silverlight.CommonDomain;component/Themes/{0}/CommonDomainStyle.xaml", CPApplication.Current.ThemeCode), UriKind.Relative) });
        }

        #endregion
    }
}
