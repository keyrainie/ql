using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    /// <summary>
    /// 批量创建Combo
    /// </summary>
    public class ComboBatchReq
    {
        /// <summary>
        /// 标题
        /// </summary>
        public LanguageContent Name { get; set; }

        /// <summary>
        /// 当前状态:无效 -1,有效 0,待审核 1
        /// </summary>
        public ComboStatus? Status { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public ComboType SaleRuleType { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 前台是否显示标题
        /// </summary>
        public bool? IsShowName { get; set; }

        /// <summary>
        /// 主数量
        /// </summary>
        public int? MQty { get; set; }

        /// <summary>
        /// 主单件折扣
        /// </summary>
        public decimal? MDiscount { get; set; }

        /// <summary>
        /// 次单件折扣率
        /// </summary>
        public decimal? DiscountRate { get; set; }

        /// <summary>
        /// 次单件折扣
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// 次数量
        /// </summary>
        public int? Qty { get; set; }

        /// <summary>
        /// 主绑定商品
        /// </summary>
        public List<int> MasterItems { get; set; }

        /// <summary>
        /// 次绑定商品
        /// </summary>
        public List<int> Items { get; set; }

        public string CompanyCode { get; set; }
    }
}
