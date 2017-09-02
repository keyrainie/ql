using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using IPP.Oversea.CN.ContentMgmt.Baidu.ServiceAdapter;
using System.Text.RegularExpressions;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Utility
{
    public static class JobHelper
    {
        public static void SendExceptionMail(string body)
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
            mail.Subject = "置顶商品管理Job出现异常";
            EmailServiceAdapter.SendEmail(mail);
        }

        public static void SendMail(MailEntity mail)
        {
            if (ConfigurationManager.AppSettings["IsSendEmail"].ToString().ToUpper() == "FALSE")
            {
                return;
            }
            string bannedEmail = ConfigurationManager.AppSettings["BannedEmail"];
            if (mail.To == null)
                return;
            if (bannedEmail.Contains(mail.To))
            {
                return;
            }
            if (String.IsNullOrEmpty(mail.To))
            {
                return;
            }
            EmailServiceAdapter.SendEmail(mail);
        }

        public static String bSubstring(string s, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            if (bytes.GetLength(0) < length)
            {
                return s;
            }
            int n = 0;  //  表示当前的字节数
            int i = 0;  //  要截取的字节数
            for (; i < bytes.GetLength(0) && n < length; i++)
            {
                //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    n++;      //  在UCS2第一个字节时n加1
                }
                else
                {
                    //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节

                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }
            }
            //  如果i为奇数时，处理成偶数
            if (i % 2 == 1)
            {

                //  该UCS2字符是汉字时，去掉这个截一半的汉字
                if (bytes[i] > 0)

                    i = i - 1;

                 //  该UCS2字符是字母或数字，则保留该字符

                else
                    i = i + 1;
            }
            return System.Text.Encoding.Unicode.GetString(bytes, 0, i);
        }

        public static int GetBytesCount(string s)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            int result = 0;
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                if (i % 2 == 0)
                {
                    result++;
                }
                else
                {
                    if (bytes[i] > 0)
                    {
                        result++;
                    }
                }
            }
            return result;
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
    }
}
