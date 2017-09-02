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
    public interface IJavascriptInterop : IComponent
    {
        /// <summary>
        /// register managed type to javascript
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        void RegisterType(string key, Type type);

        /// <summary>
        /// register managed class to javascript 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="intance"></param>
        void RegisterScriptObject(string key, object intance);

        /// <summary>
        /// use window.eval to append script in client
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        object ExecuteScript(string script);

        /// <summary>
        /// use window.eval to call javascript's function
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object ExecuteScript(string methodName, params object[] args);

        /// <summary>
        /// use window.eval to call javascript's function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script"></param>
        /// <returns></returns>
        T ExecuteScript<T>(string script);

        /// <summary>
        /// use window.eval to call javascript's function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T ExecuteScript<T>(string methodName, params object[] args);
    }
}
