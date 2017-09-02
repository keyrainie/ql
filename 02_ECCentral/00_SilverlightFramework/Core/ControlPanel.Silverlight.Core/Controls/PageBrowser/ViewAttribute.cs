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

namespace Newegg.Oversea.Silverlight.Controls
{
    public enum SingletonTypes
    {
        /// <summary>
        /// 以Page区分是否同一个实例
        /// </summary>
        Page = 0,
        /// <summary>
        /// 以Url区分是否同一个实例
        /// </summary>
        Url
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class View : Attribute
    {
        public bool IsSingleton { get; set; }
        public string ViewName { get; set; }
        public bool NeedAccess { get; set; }
        public SingletonTypes SingletonType { get; set; }


        public View()
            : base()
        {
            IsSingleton = true;
            NeedAccess = true;
            SingletonType = SingletonTypes.Page;
        }

        public View(string viewName)
            : this()
        {
            this.ViewName = viewName;
        }

        public View(string viewName, bool isSingleton)
            : this(viewName)
        {
            IsSingleton = isSingleton;
        }

        public View(string viewName, bool isSingleton, bool needAccess)
            : this(viewName, isSingleton)
        {
            NeedAccess = needAccess;
        }

        public View(string viewName, bool isSingleton, bool needAccess, SingletonTypes singletonType)
        {
            this.SingletonType = singletonType;
        }
    }
}
