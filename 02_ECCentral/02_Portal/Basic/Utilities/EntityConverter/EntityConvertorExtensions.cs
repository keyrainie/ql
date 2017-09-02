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
using System.Collections.Generic;
using ECCentral.BizEntity;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.Basic.Utilities
{   
    public static class EntityConvertorExtensions
    {        
        #region 从ModelBase到BizEntity

        public static T ConvertVM<S, T>(this S source)
            where S : ModelBase           
        {
            return EntityConverter<S, T>.Convert(source);
        }

        public static T ConvertVM<S, T>(this S source, Action<S, T> manualMap)
            where S : ModelBase
            where T : class
        {
            return EntityConverter<S, T>.Convert(source, manualMap);
        }

        public static C ConvertVM<S, T, C>(this IEnumerable<S> sourceList)
            where S : ModelBase
            where T : class
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList);
        }

        public static C ConvertVM<S, T, C>(this IEnumerable<S> sourceList, Action<S, T> manualMap)
            where S : ModelBase
            where T : class
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList, manualMap);
        }

        #endregion

        #region 从BizEntity到ModelBase

        public static T Convert<S, T>(this S source)            
            where T : ModelBase
        {
            return EntityConverter<S, T>.Convert(source);
        }

        public static T Convert<S, T>(this S source, Action<S, T> manualMap)
            where T : ModelBase
        {
            return EntityConverter<S, T>.Convert(source, manualMap);
        }

        public static C Convert<S, T, C>(this IEnumerable<S> sourceList)
            where T : ModelBase
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList);
        }

        public static C Convert<S, T, C>(this IEnumerable<S> sourceList, Action<S, T> manualMap)
            where T : ModelBase
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList, manualMap);
        }

        public static List<T> Convert<S, T>(this IEnumerable<S> sourceList)
            where T : ModelBase
        {
            return Convert<S, T, List<T>>(sourceList, null);
        }

        public static List<T> Convert<S, T>(this IEnumerable<S> sourceList, Action<S, T> manualMap)
            where T : ModelBase
        {
            return Convert<S, T, List<T>>(sourceList, manualMap);
        }

        #endregion

        public static T DeepCopy<T>(this T t)
        {
            return EntityConverter<T, T>.Convert(t);
        }
    }
}
