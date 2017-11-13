using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.SO
{
    /// <summary>
    /// 订单撮合交易日志
    /// </summary>
    public class SOLogMatchedTrading
    {
        /// <summary>
        ///临时主键
        /// </summary>
        public int? ID
        {
            get;
            set;
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
      
        
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime? InDate
        {
            get;
            set;
        }
        /// <summary>
        /// 记录人员
        /// </summary>
        public string InUser
        {
            get;
            set;
        }
        /// <summary>
        /// 记录内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }
        /// <summary>
        /// 记录类型
        /// </summary>
        public string Type
        {
            get;
            set;
        }
    }
}
