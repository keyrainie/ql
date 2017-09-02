using System;
using System.Collections.Generic;

namespace ECCentral.Service.Utility
{   
    public static class EntityConvertorExtensions
    {
        #region 从ModelBase到BizEntity
        /// <summary>
        /// ConvertEntity
        /// </summary>
        /// <typeparam name="S">SourceEntity Type</typeparam>
        /// <typeparam name="T">targetEntity Type</typeparam>
        /// <param name="source">SourceEntity</param>
        /// <returns>targetEntity</returns>
        public static T Convert<S, T>(this S source)
            where S : class
        {
            return EntityConverter<S, T>.Convert(source);
        }
        /// <summary>
        /// ConvertEntity
        /// </summary>
        /// <typeparam name="S">SourceEntity Type</typeparam>
        /// <typeparam name="T">targetEntity Type</typeparam>
        /// <param name="source">SourceEntity</param>
        /// <param name="manualMap">Action<SourceEntity Type,targetEntity Type></param>
        /// <returns>SourceEntity</returns>
        public static T Convert<S, T>(this S source, Action<S, T> manualMap)
            where S : class
            where T : class
        {
            return EntityConverter<S, T>.Convert(source, manualMap);
        }

        /// <summary>
        /// ConvertListEntity
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        public static C Convert<S, T, C>(this IEnumerable<S> sourceList)
            where S : class
            where T : class
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList);
        }

        /// <summary>
        /// ConvertListEntity
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="sourceList"></param>
        /// <param name="manualMap"></param>
        /// <returns></returns>
        public static C Convert<S, T, C>(this IEnumerable<S> sourceList, Action<S, T> manualMap)
            where S : class
            where T : class
            where C : class, ICollection<T>, new()
        {
            return EntityConverter<S, T>.Convert<C>(sourceList, manualMap);
        }

        #endregion

        public static T DeepCopy<T>(this T t)
        {
            return EntityConverter<T, T>.Convert(t);
        }
    }
}
