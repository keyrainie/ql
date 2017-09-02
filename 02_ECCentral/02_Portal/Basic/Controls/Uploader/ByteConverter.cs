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
using System.Windows.Data;
using System.Globalization;

namespace ECCentral.Portal.Basic.Controls.Uploader
{
    public class ByteConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string size = "0 KB";

            if (value != null)
            {
                var byteCount = float.Parse(value.ToString());

                if (byteCount >= 1073741824)
                    size = String.Format("{0:##.##}", byteCount / 1073741824) + "GB";
                else if (byteCount >= 1048576)
                    size = String.Format("{0:##.##}", byteCount / 1048576) + "MB";
                else if (byteCount >= 1024)
                    size = String.Format("{0:##.##}", byteCount / 1024) + "KB";
                else if (byteCount > 0 && byteCount < 1024)
                    size = "1KB";    //Bytes are unimportant ;)            

            }

            return size;
        }

        //We only use one-way binding, so we don't implement this.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class PercentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0}%", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }   
}
