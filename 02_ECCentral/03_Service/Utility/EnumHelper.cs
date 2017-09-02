using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace ECCentral.Service.Utility
{
    public static class EnumHelper
    {
        private static Dictionary<string, global::System.Resources.ResourceManager> s_ResourceManagerDictionary =
            new Dictionary<string, global::System.Resources.ResourceManager>();
        private static object s_SyncResLocker = new object();
        private static global::System.Resources.ResourceManager s_DefaultResourceManager = Type.GetType("ECCentral.BizEntity.Enum.Resources.ResCommonEnum, ECCentral.BizEntity.Enum").GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as global::System.Resources.ResourceManager;

        private static global::System.Resources.ResourceManager GetResourceManager(Type enumType)
        {
            object[] resAttrs = enumType.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string resTypeName = resAttrs == null || resAttrs.Length < 1 ? null : (resAttrs[0] as DescriptionAttribute).Description;
            global::System.Resources.ResourceManager resourceMan = null;
            if (resTypeName != null)
            {
                if (!s_ResourceManagerDictionary.ContainsKey(resTypeName))
                {
                    lock (s_SyncResLocker)
                    {
                        if (!s_ResourceManagerDictionary.ContainsKey(resTypeName))
                        {
                            string resName = resTypeName;
                            string typeName = "ECCentral.BizEntity.Enum.Resources.ResCommonEnum, ECCentral.BizEntity.Enum";
                            if (resTypeName.IndexOf(',') > 0)
                            {
                                resName = resTypeName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                                typeName = resTypeName;
                            }
                            resourceMan = new global::System.Resources.ResourceManager(resName, Type.GetType(typeName).Assembly);
                            if (resourceMan != null)
                            {
                                s_ResourceManagerDictionary.Add(resTypeName, resourceMan);
                            }
                        }
                    }
                }
                if (resourceMan == null && s_ResourceManagerDictionary.ContainsKey(resTypeName))
                {
                    resourceMan = s_ResourceManagerDictionary[resTypeName];
                }
            }
            else
            {
                resourceMan = s_DefaultResourceManager;
            }
            return resourceMan;

        }

        private static string GetResourceString(global::System.Resources.ResourceManager manager, string key)
        {
            return manager == null ? null : manager.GetString(key, Thread.CurrentThread.CurrentCulture);
        }

        public static string GetDisplayText(Enum enumValue)
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException("enumValue");
            }
            Type enumType = enumValue.GetType();
            while (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && enumType.GetGenericArguments() != null
                    && enumType.GetGenericArguments().Length == 1 && enumType.GetGenericArguments()[0].IsEnum)
            {
                enumType = enumType.GetGenericArguments()[0];
            }

            if (!enumType.IsEnum)
            {
                return string.Empty;
            }

            string description = enumValue.ToString();
            FieldInfo field = enumType.GetField(description);
            if (field != null)
            {
                string resKey = string.Format("{0}_{1}", enumType.Name, field.Name);
                description = GetResourceString(GetResourceManager(enumType), resKey);

                if (description == null)
                {
                    object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                        description = desc.Description;
                    }
                }
                description = description ?? field.Name;
            }
            return description;
        }

        public static string ToDisplayText(this Enum value)
        {
            return EnumHelper.GetDisplayText(value);
        }

        public static string GetEnumDesc(Enum e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("enumValue");
            }
            Type enumType = e.GetType();
            while (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && enumType.GetGenericArguments() != null
                    && enumType.GetGenericArguments().Length == 1 && enumType.GetGenericArguments()[0].IsEnum)
            {
                enumType = enumType.GetGenericArguments()[0];
            }
            if (!enumType.IsEnum)
            {
                return string.Empty;
            }
            var enumInfo = e.GetType().GetField(e.ToString());
            var enumAttributes = (DescriptionAttribute[])enumInfo.
                GetCustomAttributes(typeof(DescriptionAttribute), false);
            return enumAttributes.Length > 0 ? enumAttributes[0].Description : e.ToString();
        }

        public static string ToEnumDesc(this Enum value)
        {
            return EnumHelper.GetEnumDesc(value);
        }

        public static string GetDescription(Enum value)
        {
            return value == null ? null : GetDescription(value, value.GetType());
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
    }
    public class BelongToChannelAttribute : Attribute
    {

    }
}
