using System;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface ILog : IComponent
    {
        string WriteLog(string content, string category);
        string WriteLog(string content, string category, string localName, string globalName);
        string WriteLog(string content, string category, string referenceKey);
        string WriteLog(string content, string category, string referenceKey, string logUserName, Dictionary<string, object> extendedProperties);
        string WriteLog(string content, string category, string localName, string globalName, string referenceKey, string logUserName, Dictionary<string, object> extendedProperties);

        string LogError(Exception ex);
        string LogError(Exception ex, object[] methodArguments);
        string LogError(Exception ex, string message, object[] methodArguments);
        string LogError(Exception ex, string localName, string globalName, string message, object[] methodArguments);
    }
}
