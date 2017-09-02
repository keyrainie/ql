using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 批量创建赠品销售规则实体
    /// </summary>
    public class BatchCreateSaleGiftSaleRuleInfo:ICompany
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
        /// 是否全局
        /// </summary>
        public bool IsGlobal { get; set; }
        /// <summary>
        /// 赠品类型
        /// </summary>
        public SaleGiftType? GiftType { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<ProductItemInfo> ProductList { get; set; }
        /// <summary>
        /// 排除商品列表
        /// </summary>
        public List<ProductItemInfo> ProductListExclude { get; set; }
        /// <summary>
        /// 包含商品列表
        /// </summary>
        public List<ProductItemInfo> ProductListInclude { get; set; }





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
