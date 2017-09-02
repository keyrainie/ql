using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.SO
{
    /// <summary>
    /// 订单日志
    /// </summary>
    public class SOLogInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 操作IP
        /// </summary>
        public string OptIP
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public BizLogType? OptType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName
        {
            get;
            set;
        }

        /// <summary>
        /// 操作描述
        /// </summary>
        public string Note
        {
            get;
            set;
        }
        /// <summary>
        /// 记录日志的用户
        /// </summary>
        public int? UserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 记录日志时间
        /// </summary>
        public DateTime? OptTime
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
    }
}
