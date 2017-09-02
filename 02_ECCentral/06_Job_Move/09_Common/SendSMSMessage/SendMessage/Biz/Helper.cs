using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SendMessage.Class
{
    public class Helper
    {

        static Regex Rgx = new Regex(CellPhoneNumberPattern, RegexOptions.Compiled);

        /// <summary>
        /// 检查当前时间是否处于可以发送短信的时间段，避免夜间扰民
        /// </summary>
        public static bool IsInDayTime()
        {
            if (DateTime.Now.Hour < UnSendTimeStart
                && DateTime.Now.Hour >= UnSendTimeEnd)//在规定的时间段内不发送短信息
                return true;
            else
                return false;
        }

        /// <summary>
        ///检查手机号码是否合法 
        /// </summary>
        /// <param name="CellNumber">手机号</param>
        /// <returns></returns>
        public static bool IsCellPhoneNumber(string CellNumber)
        {
            return Rgx.IsMatch(CellNumber);
        }

        /// <summary>
        /// 手机号正则匹配模式字串
        /// </summary>
        public static string CellPhoneNumberPattern
        {
            get
            {
                return ConfigurationManager.AppSettings["PhonePattern"];
            }
        }

        /// <summary>
        /// 发送开始时
        /// </summary>
        public static int UnSendTimeStart
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["UnSendTimeStart"]);
            }
        }

        /// <summary>
        /// 发送结束时
        /// </summary>
        public static int UnSendTimeEnd
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["UnSendTimeEnd"]);
            }
        }

        /// <summary>
        /// 通知邮件接收人
        /// </summary>
        public static string MailTo
        {
            get
            {
                return ConfigurationManager.AppSettings["MailTo"];
            }
        }

        /// <summary>
        /// 通知邮件主题
        /// </summary>
        public static string MailSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["MailSubject"];
            }
        }

        /// <summary>
        /// 运行日志目录
        /// </summary>
        public static string LogFile
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFile"];
            }
        }

        /// <summary>
        /// 日志中的时间格式字串
        /// </summary>
        public static string DateTimeFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["DateTimeFormat"];
            }
        }

        /// <summary>
        /// 下载文件重试次数
        /// </summary>
        public static int RetryTimes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["RetryTimes"]);
            }
        }

        /// <summary>
        /// 重试间隔时间
        /// </summary>
        public static TimeSpan RetryInterval
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["RetryInterval"]);
            }
        }

        /// <summary>
        /// 每次最大发送数
        /// </summary>
        public static int TopCount
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TopCount"]);
            }
        }

        /// <summary>
        /// 晚夜发送短信的紧急程度
        /// </summary>
        public static int Priority
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Priority"]);
            }
        }


        /// <summary>
        /// 是否启动短信内容编码转换
        /// </summary>
        public static bool EnableTranslator
        {
            get
            {
                return Convert.ToBoolean( ConfigurationManager.AppSettings["EnableTranslator"]);
            }
        }

        /// <summary>
        /// 短信内容字符串编号源格式
        /// </summary>
        public static string SourceEncoder
        {
            get
            {
                return ConfigurationManager.AppSettings["SourceEncoder"];
            }
        }

        /// <summary>
        /// 短信内容字符串编号目标格式
        /// </summary>
        public static string TargetEncoder
        {
            get
            {
                return ConfigurationManager.AppSettings["TargetEncoder"];
            }
        }
    }
}
