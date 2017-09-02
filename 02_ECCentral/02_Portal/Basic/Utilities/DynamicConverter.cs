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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Reflection;

namespace ECCentral.Portal.Basic.Utilities
{
    public class DynamicConverter<T> where T : ModelBase, new()
    {
        public static T ConvertToVM(dynamic source, params string[] ignoreTargetProperties)
        {
            return ConvertToVM(source, null, ignoreTargetProperties);
        }

        public static T ConvertToVM(dynamic source, Action<dynamic, T> manualMap, params string[] ignoreTargetProperties)
        {
            if (!source.IsObject)
            {
                throw new ArgumentException("The dynamic object is not complex object.", "source");
            }
            List<string> list = new List<string>(ignoreTargetProperties);
            T t = new T();
            var propArray = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in propArray)
            {
                if(list.Contains(item.Name))
                {
                    continue;
                }
                if (item.CanWrite && source.IsDefined(item.Name))
                {
                    //item.SetValue(t, source[item.Name], null);
                    Invoker.PropertySet(t, item.Name, source[item.Name]);
                }
            }
            if (manualMap != null)
            {
                manualMap(source, t);
            }
            return t;
        }

        public static C ConvertToVMList<C>(dynamic source, params string[] ignoreTargetProperties)
            where C : ICollection<T>, new()
        {
            return ConvertToVMList<C>(source, null, ignoreTargetProperties);
        }

        public static C ConvertToVMList<C>(dynamic source, Action<dynamic, T> manualMap, params string[] ignoreTargetProperties)
            where C : ICollection<T>, new()
        {
            if (source == null)
            {
                return default(C);
            }
            if (!source.IsArray)
            {
                throw new ArgumentException("The dynamic object is not enumerable.", "source");
            }
            C c = new C();
            foreach (var item in source)
            {
                c.Add(ConvertToVM(item, manualMap, ignoreTargetProperties));
            }
            return c;
        }

        public static List<T> ConvertToVMList(dynamic source, params string[] ignoreTargetProperties)
        {
            return ConvertToVMList(source, null, ignoreTargetProperties);
        }

        public static List<T> ConvertToVMList(dynamic source, Action<dynamic, T> manualMap, params string[] ignoreTargetProperties)
        {
            return ConvertToVMList<List<T>>(source, manualMap, ignoreTargetProperties);
        }

        public static C ConvertToVMList<C>(IEnumerable<dynamic> sourceList, params string[] ignoreTargetProperties)
            where C : ICollection<T>, new()
        {
            return ConvertToVMList<C>(sourceList, null, ignoreTargetProperties);
        }

        public static C ConvertToVMList<C>(IEnumerable<dynamic> sourceList, Action<dynamic, T> manualMap, params string[] ignoreTargetProperties)
            where C : ICollection<T>, new()
        {
            if (sourceList == null)
            {
                return default(C);
            }

            C c = new C();
            foreach (var item in sourceList)
            {
                c.Add(ConvertToVM(item, manualMap, ignoreTargetProperties));
            }
            return c;
        }

        public static List<T> ConvertToVMList(IEnumerable<dynamic> sourceList, params string[] ignoreTargetProperties)
        {
            return ConvertToVMList(sourceList, null, ignoreTargetProperties);
        }

        public static List<T> ConvertToVMList(IEnumerable<dynamic> sourceList, Action<dynamic, T> manualMap, params string[] ignoreTargetProperties)
        {
            return ConvertToVMList<List<T>>(sourceList, manualMap, ignoreTargetProperties);
        }
    }
}
