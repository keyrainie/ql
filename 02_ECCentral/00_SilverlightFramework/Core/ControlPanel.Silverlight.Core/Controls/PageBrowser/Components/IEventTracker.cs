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

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface IEventTracker : IComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">操作名，比如"Click"</param>
        /// <param name="label">标签名，一般制定为控件名,比如"btnSave"</param>
        void TraceEvent(string action, string label);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageURL">页面URL，必须为相对路径,比如："/CommonDomain/QueryEventTraceLog"</param>
        /// <param name="action">操作名，比如"Click"</param>
        /// <param name="label">标签名，一般制定为控件名,比如"btnSave"</param>
        void TraceEvent(string pageURL, string action, string label);
    }
}
