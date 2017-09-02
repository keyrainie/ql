using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 批量创建赠品规则实体
    /// </summary>
    public class BatchCreateGiftRuleInfo:ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 促销活动系统编号
        /// </summary>
        public int PromotionSysNo { get; set; }
        /// <summary>
        /// 赠品组合类型
        /// </summary>
        public SaleGiftGiftItemType? GiftComboType { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int? SumCount { get; set; }
        /// <summary>
        /// 是否为特色赠品
        /// </summary>
        public int IsSpecial { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 产品清单
        /// </summary>
        public List<ProductItemInfo> ProductList { get; set; }

        #region ICompany Members
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser
        {
            get;
            set;
        }
    }
}
