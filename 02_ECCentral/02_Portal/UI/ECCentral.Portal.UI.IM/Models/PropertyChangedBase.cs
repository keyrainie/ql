using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public static class MyPropertyChangedEx
    {
        public static void NotifyPropertyChanged<T, TProperty>(this T iMyPropertyChanged
                                                               , Expression<Func<T, TProperty>> expression)
            where T : PropertyChangedBase
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                iMyPropertyChanged.NotifyPropertyChanged(propertyName);
            }
        }
    }

     [DataContract]
    public class ModelBaseEx:ModelBase
    {
        public delegate string EnumChangeHadle(object args);

        public EnumChangeHadle OnEnumChange { get; set; }

        /// <summary>
        /// 设置查询条件
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceName"></param>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        protected bool SetValue<TPropertyType>(string propertyName, ref TPropertyType target, TPropertyType value, Type resourceType, string resourceName, ref string queryFilter)
        {
            var result = base.SetValue(propertyName, ref target, value);

            string queryDesc = GetHeardValue(resourceType, resourceName);
            if (queryFilter == null) queryFilter = "";
            if (!String.IsNullOrEmpty(queryDesc))
            {
                queryFilter = RemoveValue(queryDesc, queryFilter);
                if (value==null)
                {
                    queryFilter = RemoveValue(queryDesc, queryFilter);
                    return result;
                }
                if (value is string)
                {
                    var tempValue = Convert.ToString(value).Trim();
                    if (string.IsNullOrEmpty(tempValue))
                    {
                        return result;
                    }
                }
                if (typeof(TPropertyType).IsGenericType
                    && typeof(TPropertyType).GetGenericTypeDefinition() == typeof(Nullable<>)
                    && typeof(TPropertyType).GetGenericArguments().Length == 1)
                {
                    var currentType = UtilityHelper.GetGenericType(value, typeof(TPropertyType));
                    if (currentType.IsEnum)
                    {
                        if (OnEnumChange != null)
                        {
                            var valueDesc = OnEnumChange(value);
                            queryDesc = String.Format("【{0}={1}】", queryDesc, valueDesc);
                        }
                    }
                    else
                    {
                        if (value != null)
                        {
                            queryDesc = String.Format("【{0}={1}】", queryDesc, value);
                        }
                        //if ((bool)Invoker.PropertyGet(typeof(TPropertyType), value, "HasValue", false, true))
                        //{
                        //    var source = Invoker.PropertyGet(typeof(TPropertyType), value, "Value", false, true);
                        //    queryDesc = String.Format("【{0}={1}】", queryDesc, source);
                        //}
                    }
                }
                else if(typeof(TPropertyType).IsEnum)
                {
                    var valueDesc = OnEnumChange(value);
                    queryDesc = String.Format("【{0}={1}】", queryDesc, valueDesc);
                }
                else
                {
                    queryDesc = String.Format("【{0}={1}】", queryDesc, value);
                }
               
            }
            queryFilter += queryDesc;
            return result;
        }

        /// <summary>
        /// 设置ComparisonEx查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="value"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceName"></param>
        /// <param name="queryFilter"></param>
        /// <param name="valueDesc"></param>
        protected  void SetValueEx<T,TV>(Comparison<T,TV> value,Type resourceType, string resourceName, ref string queryFilter,string valueDesc="")
         {
            string queryDesc = GetHeardValue(resourceType, resourceName);
            if (!String.IsNullOrEmpty(queryDesc) && value != null)
            {
                if (queryFilter == null) queryFilter = "";
                queryFilter = RemoveValue(queryDesc, queryFilter);
                if (value.ComparisonValue.Equals(null))
                {
                    return;
                }
                dynamic tempentity = (Comparison<T, TV>)value;
                var desc = EnumConverter.GetDescription(tempentity.QueryConditionOperator);
                if (String.IsNullOrEmpty(valueDesc))
                    queryDesc = String.Format(" 【{0}{1}{2}】", queryDesc, desc,
                                                 value.ComparisonValue);
                else
                    queryDesc = String.Format(" 【{0}{1}{2}】", queryDesc, desc,
                                                  valueDesc);
            }
            queryFilter += queryDesc;
         }

        /// <summary>
        /// 去除条件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        private string RemoveValue(string value,  string queryFilter)
        {
            if (String.IsNullOrEmpty(queryFilter)) return queryFilter;
            var regexStr = String.Format(@"【{0}[^】]+】", value);
            var regex = new Regex(regexStr);
            var desc = regex.Replace(queryFilter, "").Trim();
            return desc;
        }

        /// <summary>
        /// 得到条件名称
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private string GetHeardValue(Type resourceType, string resourceName)
        {
            var property = resourceType.GetProperty(resourceName);
            string queryDesc = "";
            if (property != null)
            {
                object obj2 = property.GetValue(null, null);
                queryDesc = Convert.ToString(obj2);
                if(queryDesc.IndexOf(":")!=-1)
                {
                    queryDesc = queryDesc.Replace(":", "");
                }
            }
            return queryDesc;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected new bool SetValue<TPropertyType>(string propertyName, ref TPropertyType target, TPropertyType value)
        {
            var result = base.SetValue(propertyName, ref target, value);
            return result;
        }


    }
}
