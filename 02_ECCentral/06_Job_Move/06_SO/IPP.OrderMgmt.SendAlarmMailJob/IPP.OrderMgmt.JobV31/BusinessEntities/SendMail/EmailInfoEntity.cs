using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.SendMail
{
    [Serializable]
    public class EmailInfoEntity:EntityBase
    {
        
        /// <summary>
        /// 发送地址
        /// </summary>
        [DataMapping("MailFrom", DbType.String)]
        public string MailFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 发送地址
        /// </summary>
        [DataMapping("MailAddress", DbType.String)]
        public string MailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 抄送地址
        /// </summary>
        [DataMapping("CCMailAddress", DbType.String)]
        public string CCMailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 邮件标题
        /// </summary>
        [DataMapping("MailSubject", DbType.String)]
        public string MailSubject
        {
            get;
            set;
        }

        /// <summary>
        /// 邮件主体
        /// </summary>
        [DataMapping("MailBody", DbType.String)]
        public string MailBody
        {
            get;
            set;
        }
    }
}
