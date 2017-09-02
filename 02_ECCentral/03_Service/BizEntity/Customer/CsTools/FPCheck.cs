using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 欺诈审核配置信息
    /// </summary>
    public class FPCheck : IWebChannel, IIdentity
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string FPCheckCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string FPCheckName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public FPCheckStatus? FPCheckStatus { get; set; }
        /// <summary>
        /// 欺诈审核项
        /// </summary>
        public List<FPCheckItem> FPCheckItemList { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }


    }
}
