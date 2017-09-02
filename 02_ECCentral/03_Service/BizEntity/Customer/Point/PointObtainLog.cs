using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 积分获得日志
    /// </summary>
    public class PointObtainLog : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysno { get; set; }

        /// <summary>
        /// 获得积分数量
        /// </summary>
        public int? Point { get; set; }

        /// <summary>
        /// 可用积分数
        /// </summary>
        public int? AvailablePoint { get; set; }

        /// <summary>
        /// 积分获得类型 
        /// </summary>
        public int? ObtainType { get; set; }

        /// <summary>
        /// 积分是否通过系统账户发放
        /// </summary>
        public int? IsFromSysAccount { get; set; }

        /// <summary>
        /// 系统账户编号
        /// </summary>
        public int? SystemAccountNo { get; set; }

        /// <summary>
        /// 系统账户名称
        /// </summary>
        public string SystemAccount { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public PointStatus? Status { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public string CurrencyCode { get; set; }

    }
}
