using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SendMKTPointEmail.Biz.Common
{
    public class JobConfig
    {
        /// <summary>
        /// MKT账号列表
        /// </summary>
        public static string MKTAccount
        {
            get { return ConfigurationManager.AppSettings["MKTAccount"]; }
        }

        /// <summary>
        /// MKT积分不足时报警邮件接收人地址列表
        /// </summary>
        public static string MKTAccountMailRecv
        {
            get { return ConfigurationManager.AppSettings["MKTAccountMailRecv"]; }
        }

        /// <summary>
        /// MKT账号报警邮件主题
        /// </summary>
        public static string MKTAccountRevcMailSubjectTemplet
        {
            get { return ConfigurationManager.AppSettings["MKTAccountRevcMailSubjectTemplet"]; }
        }

        /// <summary>
        /// MKT账号报警邮件内容模板
        /// </summary>
        public static string MKTAccountRevcMailBodyTemplet
        {
            get { return ConfigurationManager.AppSettings["MKTAccountRevcMailBodyTemplet"]; }
        }


        /// <summary>
        /// PM账号报警邮件主题
        /// </summary>
        public static string PMAccountRevcMailSubjectTemplet
        {
            get { return ConfigurationManager.AppSettings["AccountRevcMailSubjectTemplet"]; }
        }

        /// <summary>
        /// PM账号报警邮件内容模板
        /// </summary>
        public static string PMAccountRevcMailBodyTemplet
        {
            get { return ConfigurationManager.AppSettings["AccountRevcMailBodyTemplet"]; }
        }


        /// <summary>
        /// 使用DB/WCF发邮件的开关
        /// </summary>
        public static string SendMailMethodSwitch
        {
            get { return ConfigurationManager.AppSettings["SendMailMethodSwitch"]; }
        }

        /// <summary>
        /// 发邮件的地址
        /// </summary>
        public static string SendMailAddress
        {
            get { return ConfigurationManager.AppSettings["SendMailAddress"]; }
        }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public static string CompanyCode
        {
            get { return ConfigurationManager.AppSettings["SendMailCompanyCode"]; }
        }

        /// <summary>
        /// MKT账号统计积分使用的过去天数
        /// </summary>
        public static int MKTAccountPassDays
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["MKTAccountPassDays"]); }
        }


        public static AccountPointNoticeMailEntityCollection AccountPoitInfoList
        {
            get
            {
                var queryEntity = (AccountPointNoticeMailEntitySection)ConfigurationManager.GetSection("pointAccountList");

                return queryEntity.Items;
            }
        }
    }
}
