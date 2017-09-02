using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic.Converters
{
    /// <summary>
    /// 类型为Nullable的值类型的转换类，如int?,float?
    /// </summary>
    public class NullableValueTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //传入的object都是string,对其的转换不成功的情况一律返回null
            if (value != null)
            {
                if (value.ToString().Trim().Length > 0)
                {
                    try
                    {
                        if (targetType == typeof(char?))
                        {
                            return (object)System.Convert.ToChar(value);
                        }
                        else if (targetType == typeof(int?))
                        {
                            return (object)System.Convert.ToInt32(value);
                        }
                        else if (targetType == typeof(sbyte?))
                        {
                            return (object)System.Convert.ToSByte(value);
                        }
                        else if (targetType == typeof(byte?))
                        {
                            return (object)System.Convert.ToByte(value);
                        }
                        else if (targetType == typeof(short?))
                        {
                            return (object)System.Convert.ToInt16(value);
                        }
                        else if (targetType == typeof(ushort?))
                        {
                            return (object)System.Convert.ToUInt16(value);
                        }
                        else if (targetType == typeof(int?))
                        {
                            return (object)System.Convert.ToInt32(value);
                        }
                        else if (targetType == typeof(uint?))
                        {
                            return (object)System.Convert.ToUInt32(value);
                        }
                        else if (targetType == typeof(long?))
                        {
                            return (object)System.Convert.ToInt64(value);
                        }
                        else if (targetType == typeof(ulong?))
                        {
                            return (object)System.Convert.ToUInt64(value);
                        }
                        else if (targetType == typeof(DateTime?))
                        {
                            return (object)System.Convert.ToDateTime(value);
                        }
                        else if (targetType == typeof(decimal?))
                        {
                            return (object)System.Convert.ToDecimal(value);
                        }
                        else if (targetType == typeof(float?))
                        {
                            return (object)System.Convert.ToSingle(value);
                        }
                        else if (targetType == typeof(double?))
                        {
                            return (object)System.Convert.ToDouble(value);
                        }
                        else if (targetType == typeof(bool?))
                        {
                            return (object)System.Convert.ToBoolean(value);
                        }
                        else
                        {
                            return value.ToString();
                        }
                    }
                    catch
                    {
                        //转换失败
                    }
                }
            }
            return null;
        }

        #endregion IValueConverter Members
    }
}