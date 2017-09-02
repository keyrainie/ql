using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


    static class Helper
    {
        public static string FileName = string.Empty;
        private const string DateStartFormat = "yyyy-MM-01 00:00:00";
        private const string DateEndFormat = "yyyy-MM-dd 23:59:59";
        private const string DateDisplayFormat = "yyyy-MM-dd hh:mm:ss";
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
        public static DateTime ToDisplay(this DateTime date)
        {
            return DateTime.Parse(date.ToString(DateDisplayFormat));
        }
    }

