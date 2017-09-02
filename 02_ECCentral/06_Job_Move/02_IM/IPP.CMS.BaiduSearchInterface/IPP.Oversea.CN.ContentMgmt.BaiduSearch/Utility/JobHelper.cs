using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using IPP.Oversea.CN.ContentMgmt.Baidu.ServiceAdapter;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Utility
{
    public static class JobHelper
    {
        public static void SendMail(string body)
        {
            if (ConfigurationManager.AppSettings["IsSendEmail"].ToString().ToUpper() == "FALSE")
            {
                return;
            }
            MailEntity mail = new MailEntity();
            mail.Body = body;
            mail.From = ConfigurationManager.AppSettings["EmailFrom"];
            mail.To = ConfigurationManager.AppSettings["EmailTo"];
            mail.CC = ConfigurationManager.AppSettings["EmailCC"];

            if (String.IsNullOrEmpty(mail.To))
            {
                return;
            }
            mail.Subject = "百度搜索Job出现异常";
            EmailServiceAdapter.SendEmail(mail);
        }

        public static String bSubstring(string s, int length)
        {
            if (s == null)
            {
                return null;
            }
            if (s.Length <= length / 4)
            {
                return s;
            }
            if (s.Length > length)
            {
                s = s.Substring(0, length);
            }
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(s);
            if (buffer.Length <= length)
            {
                return s;
            }
            int index = length;
            while (index > 0)
            {
                if ((buffer[index] & (byte)0xc0) == (byte)0x80)
                {
                    --index;
                }
                else
                {
                    break;
                }
            }
            return UTF8Encoding.UTF8.GetString(buffer, 0, index);
        }

        public static string CleanInvalidXmlChars(string text)
        {
            StringBuilder result = new StringBuilder();
            char current;
            for (int i = 0; i < text.Length; i++)
            {
                current = text[i];
                if (IsLegalXmlChar(current))
                {
                    result.Append(current);
                }
            }
            return result.ToString();
        }

        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public static string AppendGzExtension(string path)
        {
            const string gzExtension = ".gz";
            string extension = Path.GetExtension(path);
            if (string.Compare(extension, gzExtension, true) != 0)
            {
                path += gzExtension;
            }
            return path;
        }
    }
}
