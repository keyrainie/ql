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
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class CheckXapVersionCompletedEventArgs : AsyncCompletedEventArgs
    {
        public List<XapVersionInfo> XapVersionList { get; internal set; }


        public CheckXapVersionCompletedEventArgs(List<XapVersionInfo> xapVersionList, Exception error, bool canceled, object userState) :
            base(error, canceled, userState)
        {
            XapVersionList = xapVersionList;
        }
    }

    public interface IXapVersionController : Newegg.Oversea.Silverlight.Controls.Components.IComponent
    {
        bool EnableAutoCheck { get; set; }
        TimeSpan AutoCheckIntervalTime { get; set; }

        void CheckXapVersionAsync(Action callback);
        event EventHandler<CheckXapVersionCompletedEventArgs> XapVersionChangedCompleted;

        string GetXapVersion(string xapName);
    }


    public class XapVersionInfo
    {
        public string XapName { get; set; }

        public string Version { get; set; }

        public string PreVersion { get; set; }

        public string Title { get; set; }

        public string PublishDate { get; set; }

        public string Description { get; set; }

        public string UpdateLevel { get; set; }
    }
}
