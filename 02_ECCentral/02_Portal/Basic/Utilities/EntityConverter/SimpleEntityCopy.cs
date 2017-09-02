//************************************************************************
// 用户名				泰隆优选
// 系统名				通用方法
// 子系统名		        实体之间赋值
// 作成者				Tom
// 改版日				2011.8.11
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace IPP.CN.IM.Service.Common.Utility
{
    public sealed class SimpleEntityCopy
    {
        /// <summary>
        /// 两个实体的关系
        /// </summary>
        public class PropertyMapper
        {
            public PropertyInfo SourceProperty
            {
                get;
                set;
            }
            public PropertyInfo TargetProperty
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 获取两个实体对应关系
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static IList<PropertyMapper> GetMapperProperties(Type sourceType, Type targetType)
        {
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();
            return (from s in sourceProperties
                    from t in targetProperties
                    where s.Name == t.Name
                    && s.CanRead 
                    && t.CanWrite 
                    && s.PropertyType == t.PropertyType
                    && !s.PropertyType.Name.ToLower().Contains("list")
                    select new PropertyMapper
                    {
                        SourceProperty = s,
                        TargetProperty = t
                    }).ToList();
        }

        /// <summary>
        /// 两个实体赋值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void CopyProperties(object source, object target)
        {
            if (source == null) return;
            var sourceType = source.GetType();
            if(target==null)
            {
                target = Invoker.CreateInstance(sourceType);
            }
            Type targetType = target.GetType();
            var mapperProperties = GetMapperProperties(sourceType, targetType);
            if (mapperProperties == null) return;
            var count = mapperProperties.Count;
            for (var index = 0; index < count; index++)
            {
                var property = mapperProperties[index];
                var sourceValue = property.SourceProperty.GetValue(source, null);
                var result = IsNullableChangeType(property.SourceProperty.PropertyType);
                if (property.SourceProperty.PropertyType.IsValueType || property.SourceProperty.PropertyType == typeof(string) || result)
                {
                    property.TargetProperty.SetValue(target, sourceValue, null);
                }
                else if (property.SourceProperty.PropertyType.IsClass && !targetType.IsGenericType)
                {
                    var targetValue = property.TargetProperty.GetValue(source, null);
                    CopyProperties(sourceValue, targetValue);
                }
                else if(property.SourceProperty.PropertyType.Name.ToLower().Contains("comparisonex"))
                {
                    var method = property.SourceProperty.PropertyType.GetMethod("DeepCopy");
                    if(method!=null)
                    {
                        sourceValue = method.Invoke(null, new object[1] { sourceValue });
                         property.TargetProperty.SetValue(target, sourceValue, null);
                    }
                }
            }
        }

        private static bool IsNullableChangeType(Type targetType)
        {
            if (targetType != null && targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>)
                 && targetType.GetGenericArguments() != null
                 && targetType.GetGenericArguments().Length == 1)
            {
                return true;
            }
            return false;
        }


    }
}
