namespace Newegg.Oversea.Silverlight.Controls
{
    using System;
    using System.Windows;

    public class LinkClickEventArgs : RoutedEventArgs
    {
        public LinkClickEventArgs()
            : base()
        {
        }

        public Uri NavigateUri
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }
    }
}