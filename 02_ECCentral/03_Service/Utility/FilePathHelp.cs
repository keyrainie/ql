//************************************************************************
// 用户名				泰隆优选
// 系统名				通用方法
// 子系统名		        获取文件随机路径
// 作成者				Tom
// 改版日				2011.8.11
// 改版内容				新建
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public class FilePathHelp
    {
        private static readonly string _filePathTitle = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePathTitle");

        public static string GetSubFolderName(string fileName)
        {
            string SavePath = "{0}/{1}/{2}/";
            fileName = fileName.ToLower();
            return string.Format(SavePath, GetSingleCharacter(fileName), GetCoupleInteger(fileName), GetCoupleHex(fileName));
        }

        private static string GetSingleCharacter(string key)
        {
            //Format to A-Z
            return ((char)('A' + GetCharToIntSum(key, false) % 26)).ToString();
        }

        private static string GetCoupleInteger(string key)
        {
            //Format to 00-99
            return (GetCharToIntSum(key, true) % 100).ToString("00");
        }

        private static string GetCoupleHex(string key)
        {
            //Format to 00-FF
            return (GetCharToIntSum(key, true) % 256).ToString("X").PadLeft(2, '0');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="isOdd">是否只取奇数</param>
        /// <returns></returns>
        private static int GetCharToIntSum(string chars, bool isOdd)
        {
            int sumAsciiValue = 0;
            if (!string.IsNullOrEmpty(chars))
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    char fileNameChar = chars[i];
                    if (isOdd)
                    {
                        if (i % 2 != 0) sumAsciiValue += fileNameChar * i;
                    }
                    else
                    {
                        sumAsciiValue += fileNameChar;
                    }
                }
            }

            return sumAsciiValue;

        }

        /// <summary>
        /// 读取文件保存路径
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string GetFileSavePath(string fileType)
        {
            string savePath = "";
            switch (fileType)
            {
                case ".jpg":
                    savePath = _filePathTitle + "Original/";
                    break;
                case ".swf":
                case ".flv":
                    savePath = _filePathTitle + "360/";
                    break;
                default:
                    break;
            }

            return savePath;

        }
    }
}
