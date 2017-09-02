using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购篮商品信息
    /// </summary>
    public class BasketItemsInfo : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string BriefName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// 订购价格
        /// </summary>
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? LastVendorSysNo { get; set; }

        /// <summary>
        /// 目标仓库系统编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 目标仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 是否中转
        /// </summary>
        public int? IsTransfer { get; set; }

        /// <summary>
        /// 建议备货数量
        /// </summary>
        public int? ReadyQuantity { get; set; }

        /// <summary>
        /// 检查异常信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMSysNo { get; set; }

        /// <summary>
        /// 验证采购蓝商品代销类型和供应商代销类型是否一致
        /// </summary>
        public int? VendorIsConsign { get; set; }

        /// <summary>
        /// 是否为代销商品
        /// </summary>
        public int? IsConsign { get; set; }

        /// <summary>
        /// 赠品主商品系统编号
        /// </summary>
        public int? MasterProductSysNo { get; set; }

        /// <summary>
        /// 赠品编号
        /// </summary>
        public int? GiftSysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        public bool? IsManagerPM { get; set; }
        //产品线相关信息 只在controller中拆单业务中使用，不存数据库，界面不用
        public int? ProductLine_SysNo { get; set; }
        public int? ProductLine_PMSysNo { get; set; }
      
    }
}
