using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MerchantCommissionSettle.Components
{
    static class Helper
    {
        public static string FileName = string.Empty;
        private const string DateStartFormat = "yyyy-MM-dd 00:00:00";
        private const string DateEndFormat = "yyyy-MM-dd 23:59:59";

        /// <summary>
        /// 给重复文件加编号
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetOKFileName(string fullPath, int index)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (index == 0)
            {
                FileName = fi.Name.Replace(fi.Extension, string.Empty);
            }
            if (File.Exists(fullPath))
            {
                index++;
                fullPath = fi.DirectoryName + Path.DirectorySeparatorChar + FileName + (index == 0 ? "" : index.ToString()) + fi.Extension;
                fullPath = GetOKFileName(fullPath, index);
            }
            return fullPath;
        }

        public static DateTime ToStartDate(this DateTime date)
        {
            return DateTime.Parse(date.ToString(DateStartFormat));
        }

        public static DateTime ToEndDate(this DateTime date)
        {
            return DateTime.Parse(date.ToString(DateEndFormat));
        }

        public static string GetPath(string originalPath)
        {
            string path;

            if (Path.IsPathRooted(originalPath))
            {
                path = originalPath;
            }
            else
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalPath);
            }

            return path;
        }

        public static string ToXmlString(this object source) 
        {
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb))
            {
                XmlSerializer xs = new XmlSerializer(source.GetType());

                xs.Serialize(sw,source);
            }

            return sb.ToString();
        }

        public static T ToObject<T>(this string source)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            using(StringReader sr = new StringReader(source))
            {
                return (T)xs.Deserialize(sr);               
            }
        }
    }
}

