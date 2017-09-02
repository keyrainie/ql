using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity
{
    public class BizLog
    {
        /// <summary>
        /// Log的系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 业务log类型
        /// </summary>
        public EBizLogType BizLogType { get; set; }
        /// <summary>
        /// 业务对象主键
        /// </summary>
        public int BizKey { get; set; }
        /// <summary>
        /// 业务log标题，不长于50个字符
        /// </summary>
        public string LogTitle { get; set; }
        /// <summary>
        /// 描述，不长于400个字符
        /// </summary>
        public string LogDescription { get; set; }
        /// <summary>
        /// 业务对象
        /// </summary>
        public string BizData { get; set; }
        /// <summary>
        /// 操作人员所属的商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 操作人员编号
        /// </summary>
        public int InUserSysNo { get; set; }
        /// <summary>
        /// 操作人员名称
        /// </summary>
        public string InUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime InDate { get; set; }
    }

     

     
}
