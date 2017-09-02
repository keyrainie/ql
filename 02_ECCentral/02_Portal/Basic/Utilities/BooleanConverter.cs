using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.Basic.Utilities
{
    public class BooleanConverter : IValueConverter
    {
        /// <summary>
        /// 取得将bool类型的KeyValuePair&lt;Boolean?, string>列表.
        /// </summary>
        /// <param name="appendType">插入附加项的类型(None表示不插入),将附加项插入到第一个位置,Key值为null。</param>
        /// <returns></returns>
        public static List<KeyValuePair<Boolean?, string>> GetKeyValuePairs(EnumConverter.EnumAppendItemType appendType)
        { 
            List<KeyValuePair<Boolean?, string>> pairs = new List<KeyValuePair<Boolean?, string>>();
            switch (appendType)
            { 
                case EnumConverter.EnumAppendItemType.Custom_All:
                case EnumConverter.EnumAppendItemType.All:
                    pairs.Add(new KeyValuePair<bool?, string>(null, ResCommonEnum.Enum_All));
                    break;
                case EnumConverter.EnumAppendItemType.Custom_Select:
                case EnumConverter.EnumAppendItemType.Select:
                    pairs.Add(new KeyValuePair<bool?, string>(null, ResCommonEnum.Enum_Select));
                    break;
            }
            pairs.Add(new KeyValuePair<bool?, string>(true, ResCommonEnum.Boolean_True));
            pairs.Add(new KeyValuePair<bool?, string>(false, ResCommonEnum.Boolean_False));
            return pairs;
        }
        
        public static string GetDescription(bool value)
        {
            return value ? ResCommonEnum.Boolean_True : ResCommonEnum.Boolean_False;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ToDescription(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(object value)
        {
            string result = string.Empty;
            if (value is bool)
            {
                result = BooleanConverter.GetDescription((bool)value);
            }
            else if (value is bool?)
            {
                var convertValue = (bool?)value;
                if (convertValue.HasValue)
                {
                    result = BooleanConverter.GetDescription(convertValue.Value);
                }
            }
            else if (value is int)
            {
                // >0为true, = 0为false
                result = BooleanConverter.GetDescription((int)value > 0);
            }
            else if (value is int?)
            {
                var convertValue = (int?)value;
                if (convertValue.HasValue)
                {
                    result = BooleanConverter.GetDescription(convertValue.Value > 0);
                }
            }
            else if (value is string)
            {
                //Y 为True 否则为False
                if (value!=null && value.ToString().Length == 1)
                {
                    result = BooleanConverter.GetDescription(value.ToString().ToUpper() == "Y");
                }
            }
            return result;
        }
    }
}
