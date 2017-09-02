using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMKTPointEmail.Biz.Entities
{
    public class AccountEntityBase : EntityBase
    {
        /// <summary>
        /// 账号名称
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号当前可用点数
        /// </summary>
        public int PointAvailable { get; set; }

        /// <summary>
        /// 点数报警下限值
        /// </summary>
        public int PointLowerLimit { get; set; }

        /// <summary>
        /// 报警邮件接收人的邮件地址列表
        /// </summary>
        public string RecvMailList { get; set; }

        /// <summary>
        /// 记录的预处理状态，A:有效的;U:无效，不可用
        /// </summary>
        public char Status { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo { get; set; }


        /// <summary>
        /// 邮件模板
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string MailBody { get; set; }
    }
}
