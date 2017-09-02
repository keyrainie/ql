using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using System.Threading;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.Basic.Utilities
{
    public class EnumConverter : IValueConverter
    {
        private static Dictionary<string, global::System.Resources.ResourceManager> ResourceManagerDictionary = new Dictionary<string, global::System.Resources.ResourceManager>();
        private static object ResLocker = new object();
        private static global::System.Resources.ResourceManager GetResourceManager(Type enumType)
        {
            object[] resAttrs = enumType.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string resTypeName = resAttrs == null || resAttrs.Length < 1 ? null : (resAttrs[0] as DescriptionAttribute).Description;
            global::System.Resources.ResourceManager resourceMan = null;
            if (resTypeName != null)
            {
                if (ResourceManagerDictionary.ContainsKey(resTypeName))
                {
                    resourceMan = ResourceManagerDictionary[resTypeName];
                }
                else
                {
                    lock (ResLocker)
                    {
                        if (!ResourceManagerDictionary.ContainsKey(resTypeName))
                        {
                            string resName = resTypeName;
                            Type type;
                            if (resTypeName.IndexOf(',') > 0)
                            {
                                resName = resTypeName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                                type = Type.GetType(resTypeName);
                            }
                            else
                            {
                                type = typeof(ResCommonEnum);
                            }
                            resourceMan = new global::System.Resources.ResourceManager(resName, type.Assembly);
                            if (resourceMan != null)
                            {
                                ResourceManagerDictionary.Add(resTypeName, resourceMan);
                            }
                        }
                    }
                }
            }
            else
            {
                resourceMan = ResCommonEnum.ResourceManager;
            }
            return resourceMan;

        }

        private static string GetResourceString(global::System.Resources.ResourceManager manager, string key)
        {
            return manager == null ? null : manager.GetString(key, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// 将枚举值转换成KeyValuePair&lt;Nullable&lt;TEnum>, string&gt;列表返回,
        /// value是通过枚举类型的DescriptionAttribute指定资源类名（如果没有通过DescriptionAttribute指定资源，则默认以"ECCentral.BizEntity.Resources.ResCommonEnum"为资源类）,
        /// 从而取得每个枚举值（在资源中以"[枚举类型]_[枚举值字段名称]"为Key）的描述;
        /// </summary>>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="appendType">是否插入“所有”项，且插入到第一个位置,“所有”项的Key值为null。</param>
        /// <returns></returns>
        public static List<KeyValuePair<Nullable<TEnum>, string>> GetKeyValuePairs<TEnum>(EnumAppendItemType appendType) where TEnum : struct
        {
            List<KeyValuePair<Nullable<TEnum>, string>> keyValuePairList = new List<KeyValuePair<Nullable<TEnum>, string>>();
            Type enumType = typeof(TEnum);
            if (enumType.IsEnum)
            {
                FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                global::System.Resources.ResourceManager resourceManager = GetResourceManager(enumType);
                string description = null;
                foreach (FieldInfo field in fields)
                {
                    description = null;
                    string resKey = string.Format("{0}_{1}", enumType.Name, field.Name);
                    description = GetResourceString(resourceManager, resKey);
                    //if (description == null)
                    //{
                    //    object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    //    if (attrs != null && attrs.Length > 0)
                    //    {
                    //        DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                    //        description = desc.Description;
                    //    }
                    //}
                    description = description ?? field.Name;
                    object enumValue = field.GetValue(null);
                    object[] ObsoleteAttrs = field.GetCustomAttributes(typeof(ObsoleteAttribute),true);
                    if (ObsoleteAttrs.Length <= 0)
                    {
                        keyValuePairList.Add(new KeyValuePair<Nullable<TEnum>, string>((TEnum)enumValue, description));
                    }
                }
                if (appendType != EnumAppendItemType.None)
                {
                    description = null;
                    string key_CustomSelect = String.Format("{0}__Select", enumType.Name);
                    switch (appendType)
                    {
                        case EnumAppendItemType.All:
                            description = ResCommonEnum.Enum_All;
                            break;
                        case EnumAppendItemType.Select:
                            description = ResCommonEnum.Enum_Select;
                            break;
                        case EnumAppendItemType.Custom_All:
                            description = GetResourceString(resourceManager, enumType.Name) ?? ResCommonEnum.Enum_All;
                            break;
                        case EnumAppendItemType.Custom_Select:
                            description = GetResourceString(resourceManager, key_CustomSelect) ?? ResCommonEnum.Enum_Select;
                            break;
                    }
                    keyValuePairList.Insert(0, new KeyValuePair<Nullable<TEnum>, string>(null, description));
                }
            }
            return keyValuePairList;
        }

        /// <summary>
        /// 将枚举值转换成KeyValuePair&lt;Nullable&lt;TEnum>, string&gt;列表返回,
        /// value是通过枚举类型的DescriptionAttribute指定资源类名（如果没有通过DescriptionAttribute指定资源，则默认以"ECCentral.BizEntity.Resources.ResCommonEnum"为资源类）,
        /// 从而取得每个枚举值（在资源中以"[枚举类型]_[枚举值字段名称]"为Key）的描述;
        /// </summary>>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns></returns>
        public static List<KeyValuePair<Nullable<TEnum>, string>> GetKeyValuePairs<TEnum>() where TEnum : struct
        {
            return GetKeyValuePairs<TEnum>(EnumAppendItemType.None);
        }

        public static string GetDescription(object enumValue, Type enumType)
        {
            string description = null;
            if (enumValue != null && enumValue.ToString().Trim() != String.Empty)
            {
                Type enumValueType = enumValue.GetType();
                enumType = enumValueType.IsEnum ? enumValueType : enumType;
                if (enumType != null && enumType.IsEnum)
                {
                    object o = null;
                    if (enumValueType.IsEnum)
                    {
                        o = enumValue;
                    }
                    else
                    {
                        try
                        {
                            o = Enum.Parse(enumType, enumValue.ToString(), true);
                        }
                        catch
                        {
                            o = null;
                        }
                    }
                    FieldInfo field = o == null ? null : enumType.GetField(o.ToString());
                    if (field != null)
                    {
                        string resKey = string.Format("{0}_{1}", enumType.Name, field.Name);
                        description = GetResourceString(GetResourceManager(enumType), resKey);
                        //if (description == null)
                        //{
                        //    object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        //    if (attrs != null && attrs.Length > 0)
                        //    {
                        //        DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                        //        description = desc.Description;
                        //    }
                        //}
                        description = description ?? field.Name;
                    }
                }
            }
            return description;
        }

        public static string GetDescription(Enum value)
        {
            return value == null ? null : GetDescription(value, value.GetType());
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            //Type enumType = null;
            //if (parameter is Type)
            //{
            //    enumType = parameter as Type;
            //}
            //if (parameter is string)
            //{

            //    string name = parameter.ToString().Trim();
            //    if (!string.IsNullOrEmpty(name))
            //    {
            //        enumType = Type.GetType(string.Format("{0}, Version=1.0.0.0, Culture=\"\", PublicTokenKey=null", name));
            //    }
            //}
            if (value != null && value.GetType().IsEnum)
            {
                return GetDescription(value as Enum);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public enum EnumAppendItemType
        {
            None,
            /// <summary>
            /// 默认“所有”项
            /// </summary>
            All,
            /// <summary>
            /// 默认“请选择”项
            /// </summary>
            Select,//
            /// <summary>
            /// 自定义“所有”项描述
            /// </summary>
            Custom_All,//
            /// <summary>
            /// 自定义“请选择”项描述
            /// </summary>
            Custom_Select//
        }
    }
    public static class EnumExtension
    {

        public static string ToDescription(this Enum value)
        {
            return EnumConverter.GetDescription(value);
        }

        /*
        /// <summary>
        /// 将枚举描述转为资源文件描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnumDescriptionToResource(this Enum value)
        {
            Type enumType = value.GetType();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (enumType.IsEnum)
            {
                FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (FieldInfo field in fields)
                {
                    string description = string.Empty;
                    string resKey = string.Format("{0}_{1}", enumType.Name, field.Name);

                    object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                        description = desc.Description;
                    }

                    builder.AppendLine(String.Format("<data name=\"{0}_{1}\" xml:space=\"preserve\"><value>{2}</value></data>", enumType.Name, field.Name, description));

                }
            }
            return builder.ToString();
        }
        //*/
    }
}
