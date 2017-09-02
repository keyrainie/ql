using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 销售立减
    /// </summary>
    public class SaleDiscountRule : IIdentity, IWebChannel, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 三级类别系统编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 规则类型，0-限定金额，1-限定数量
        /// </summary>
        public SaleDiscountRuleType RuleType { get; set; }

        /// <summary>
        /// 循环标识，指是否可以成倍享受折扣，如果为false,则最多享受一次
        /// </summary>
        public bool IsCycle { get; set; }

        /// <summary>
        /// 单品标记,表示符合条件的商品范围按单品来享受折扣
        /// </summary>
        public bool IsSingle { get; set; }

        /// <summary>
        /// 活动商品的金额下限
        /// </summary>
        public decimal MinAmt { get; set; }

        /// <summary>
        /// 活动商品的金额上限
        /// </summary>
        public decimal MaxAmt { get; set; }

        /// <summary>
        /// 购买数量下限
        /// </summary>
        public int MinQty { get; set; }

        /// <summary>
        /// 购买数量上限
        /// </summary>
        public int MaxQty { get; set; }

        /// <summary>
        /// 销售折扣
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public SaleDiscountRuleStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime EditDate { get; set; }

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

        /// <summary>
        /// 商品组系统编号
        /// </summary>
        public int ProductGroupSysNo { get; set; }

        public int? VendorSysNo { get; set; }


        #region UI Properties

        /// <summary>
        /// 所属商家
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 商品ID(用于维护界面上显示)
        /// </summary>
        public string UIProductID { get; set; }

        /// <summary>
        /// 品牌名称(用于维护界面上显示)
        /// </summary>
        public string UIBrandName { get; set; }

        #endregion

        public bool IsC3SysNoValid()
        {
            return C3SysNo > 0;
        }

        public bool IsBrandSysNoValid()
        {
            return BrandSysNo > 0;
        }

        public bool IsProductSysNoValid()
        {
            return ProductSysNo > 0;
        }
    }
}
