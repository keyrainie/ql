using ECCentral.BizEntity.IM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class ComputerConfigItem
    {
        #region 配置单商品信息
        public int SysNo { get; set; }

        public int ComputerConfigSysNo { get; set; }

        public int ComputerPartSysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public decimal Discount { get; set; }

        public int ProductQty { get; set; }

        #endregion

        #region ComputerParts相关
        /// <summary>
        /// 组件名称，比如CPU,内存等
        /// </summary>
        public string ComputerPartName { get; set; }

        /// <summary>
        /// 是否必选组件
        /// </summary>
        public YNStatus IsMust { get; set; }

        /// <summary>
        /// 组件在列表中显示的优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 组件说明
        /// </summary>
        public string Note { get; set; }

        #endregion

        #region 配置单商品详细信息
        /// <summary>
        /// 产品成本
        /// </summary>
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 产品卖价
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 可卖库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        #endregion

        /// <summary>
        /// 组件商品的可选分类
        /// </summary>
        public List<ComputerPartsCategory> PartsCategories { get; set; }

        //以下字段为DIY装机调度Job使用
        public ProductStatus ProductStatus { get; set; }
    }
}
