using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品卡制作单主信息
    /// </summary>
    public class GiftCardFabricationMaster : IIdentity, ICompany, IWebChannel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public int? POSysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        
        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public WebChannel WebChannel { get; set; }

        /// <summary>
        /// 礼品卡列表
        /// </summary>
        public List<GiftCardFabrication> GiftCardFabricationList { get; set; }

        /// <summary>
        /// 否高级PM 用于PM产品线相关验证
        /// </summary>
        public bool IsManagerPM { get { return true; } }
    }

    /// <summary>
    /// 礼品卡制作单
    /// </summary>
    public class GiftCardFabrication : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 主信息编号
        /// </summary>
        public int? MasterSysNo { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public ProductInfo Product { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 采购单所属PM
        /// </summary>
        public int? PMUserSysNo { get; set; }
    }
}
