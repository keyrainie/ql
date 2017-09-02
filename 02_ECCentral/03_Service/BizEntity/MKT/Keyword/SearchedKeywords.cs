using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 自动匹配关键字
    /// </summary>
    public class SearchedKeywords : IIdentity, IWebChannel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 创建用户类型:0=来自MKT员工    1=来自顾客
        /// </summary>
        public KeywordsOperateUserType? CreateUserType { get; set; }

        /// <summary>
        /// 状态      D=屏蔽 A=展示
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 匹配的数量
        /// </summary>
        public int? ItemCount { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 搜索结果数？
        /// </summary>
        public int? JDCount { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 查询的Session值
        /// </summary>
        public int SearchingSessions { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
