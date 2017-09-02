using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 单品/套餐限购规则
    /// </summary>
    public class BuyLimitRule : IIdentity, IWebChannel, ICompany 
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 限购类型,0-单品限购，1-套餐限购
        /// </summary>
        public LimitType LimitType { get; set; }

        /// <summary>
        /// 如果是单品限购就是商品系统编号，如果是套餐限购就是套餐活动系统编号
        /// </summary>
        public int ItemSysNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 限购的会员等级(多个项目用逗号分隔)
        /// </summary>
        public string MemberRanks { get; set; }

        /// <summary>
        /// 限购的会员卡类型(多个项目用逗号分隔)
        /// </summary>
        public string MemberCardTypes { get; set; }

        /// <summary>
        /// 限购下限
        /// </summary>
        public int MinQty { get; set; }

        /// <summary>
        /// 限购上限
        /// </summary>
        public int MaxQty { get; set; }

        /// <summary>
        /// 每天最大下单次数
        /// </summary>
        public int OrderTimes { get; set; }

        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public LimitStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public WebChannel WebChannel { get; set; }

        #region UI Properties

        /// <summary>
        /// 商品ID(用于维护界面上显示)
        /// </summary>
        public string UIProductID { get; set; }

        #endregion
    }
}
