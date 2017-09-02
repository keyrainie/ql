using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 配送方式-短信类型模板
    /// </summary>
    public class SMSTemplate:IIdentity,IWebChannel
    {
        /// <summary>
        /// 模板内容
        /// </summary>
        public string Template { get; set; }
        /// <summary>
        /// 模板系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel{get;set;}
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode {get;set;}
    }
}
