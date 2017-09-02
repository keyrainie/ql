using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单商品信息
    /// </summary>
    public class PurchaseOrderItemInfo : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 商品批次信息
        /// </summary>
        public string BatchInfo { get; set; }

        /// <summary>
        /// 采购单编号
        /// </summary>
        public int? POSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

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
        /// 货币编码
        /// </summary>
        public int? CurrencyCode { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// 摊销被取消
        /// </summary>
        public decimal? ApportionAddOn { get; set; }

        /// <summary>
        /// 采购成本
        /// </summary>
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// 当前成本
        /// </summary>
        public decimal? CurrentUnitCost { get; set; }

        /// <summary>
        /// 退货成本
        /// </summary>
        public decimal? ReturnCost { get; set; }

        /// <summary>
        /// 抵扣后总价
        /// </summary>
        public decimal? LineReturnedPointCost { get; set; }
        /// <summary>
        /// 同步采购价格
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? PurchaseQty { get; set; }

        /// <summary>
        /// 移仓在途库存
        /// </summary>
        public int? ShiftQty { get; set; }

        /// <summary>
        /// 采购单商品检查状态
        /// </summary>
        public PurchaseOrdeItemCheckStatus? CheckStatus { get; set; }

        /// <summary>
        /// 采购单商品检查原因
        /// </summary>
        public string CheckReasonMemo { get; set; }

        /// <summary>
        /// 上次采购价格
        /// </summary>
        public decimal? LastOrderPrice { get; set; }

        /// <summary>
        /// ExecptStatus
        /// </summary>
        public int? ExecptStatus { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductMode { get; set; }

        /// <summary>
        /// BM Code
        /// </summary>
        public string BMCode { get; set; }

        /// <summary>
        /// 去税成本
        /// </summary>
        public decimal? UnitCostWithoutTax { get; set; }

        /// <summary>
        /// 京东价
        /// </summary>
        public decimal? JingDongPrice { get; set; }

        /// <summary>
        /// 京东毛利率
        /// </summary>
        public decimal? JingDongTax { get; set; }

        /// <summary>
        /// 有效库存
        /// </summary>
        public int? AvailableQty { get; set; }

        /// <summary>
        /// 上月销售总量
        /// </summary>
        public int? M1 { get; set; }

        /// <summary>
        /// 上周销售总量
        /// </summary>
        public int? Week1SalesCount { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? Tax { get; set; }

        /// <summary>
        /// 上一次调价价格
        /// </summary>
        public DateTime? LastAdjustPriceDate { get; set; }

        /// <summary>
        /// 上一次采购时间
        /// </summary>
        public DateTime? LastInTime { get; set; }

        public int? AcquireReturnPointType { get; set; }

        public decimal? AcquireReturnPoint { get; set; }

        /// <summary>
        /// 未激活或者已失效的库存
        /// </summary>
        public int? UnActivatyCount { get; set; }

        /// <summary>
        /// 正常采购价格
        /// </summary>
        public decimal? VirtualPrice { get; set; }

        /// <summary>
        /// 建议备货数量
        /// </summary>
        public int? ReadyQuantity { get; set; }

        /// <summary>
        /// 是否为同步采购商品
        /// </summary>
        public YNStatus? IsVFItem { get; set; }


        /// <summary>
        /// 是否是虚库商品
        /// </summary>
        public bool? IsVirtualStockProduct { get; set; }


        #region [各分仓库存信息]
        /// <summary>
        /// 上海仓库存
        /// </summary>
        public int? SHInventoryStock { get; set; }
        /// <summary>
        /// 北京仓库存
        /// </summary>
        public int? BJInventoryStock { get; set; }
        /// <summary>
        /// 广州仓库存
        /// </summary>
        public int? GZInventoryStock { get; set; }
        /// <summary>
        /// 成都仓库存
        /// </summary>
        public int? CDInventoryStock { get; set; }
        /// <summary>
        /// 武汉仓库存
        /// </summary>
        public int? WHInventoryStock { get; set; }


        /// <summary>
        /// 广州已采购数量
        /// </summary>
        public int? GZHaveStockNumber { get; set; }
        /// <summary>
        /// 广州移仓在途数量
        /// </summary>
        public int? GZSheftOnRoadNumber { get; set; }
        /// <summary>
        /// 广州待入库数量
        /// </summary>
        public int? GZWaitInStockNumber { get; set; }
        /// <summary>
        /// 广州待审核数量
        /// </summary>
        public int? GZWaitCheckNumber { get; set; }
        /// <summary>
        /// 广州在途数量
        /// </summary>
        public int? GZOnRoadNumber { get; set; }
        /// <summary>
        /// 广州仓W1
        /// </summary>
        public int? GZW1 { get; set; }

        /// <summary>
        /// 北京已采购数量
        /// </summary>
        public int? BJHaveStockNumber { get; set; }
        /// <summary>
        /// 北京移仓在途数量
        /// </summary>
        public int? BJSheftOnRoadNumber { get; set; }
        /// <summary>
        /// 北京待入库数量
        /// </summary>
        public int? BJWaitInStockNumber { get; set; }
        /// <summary>
        /// 北京待审核数量
        /// </summary>
        public int? BJWaitCheckNumber { get; set; }
        /// <summary>
        /// 北京在途数量
        /// </summary>
        public int? BJOnRoadNumber { get; set; }
        /// <summary>
        /// 北京仓W1
        /// </summary>
        public int? BJW1 { get; set; }


        /// <summary>
        /// 上海已采购数量
        /// </summary>
        public int? SHHaveStockNumber { get; set; }
        /// <summary>
        /// 上海移仓在途数量
        /// </summary>
        public int? SHSheftOnRoadNumber { get; set; }
        /// <summary>
        /// 上海待入库数量
        /// </summary>
        public int? SHWaitInStockNumber { get; set; }
        /// <summary>
        /// 上海待审核数量
        /// </summary>
        public int? SHWaitCheckNumber { get; set; }
        /// <summary>
        /// 上海在途数量
        /// </summary>
        public int? SHOnRoadNumber { get; set; }
        /// <summary>
        /// 上海仓W1
        /// </summary>
        public int? SHW1 { get; set; }


        /// <summary>
        /// 武汉已采购数量
        /// </summary>
        public int? WHHaveStockNumber { get; set; }
        /// <summary>
        /// 武汉移仓在途数量
        /// </summary>
        public int? WHSheftOnRoadNumber { get; set; }
        /// <summary>
        /// 武汉待入库数量
        /// </summary>
        public int? WHWaitInStockNumber { get; set; }
        /// <summary>
        /// 武汉待审核数量
        /// </summary>
        public int? WHWaitCheckNumber { get; set; }
        /// <summary>
        /// 武汉在途数量
        /// </summary>
        public int? WHOnRoadNumber { get; set; }
        /// <summary>
        /// 武汉仓W1
        /// </summary>
        public int? WHW1 { get; set; }

        /// <summary>
        /// 成都已采购数量
        /// </summary>
        public int? CDHaveStockNumber { get; set; }
        /// <summary>
        /// 成都移仓在途数量
        /// </summary>
        public int? CDSheftOnRoadNumber { get; set; }
        /// <summary>
        /// 成都待入库数量
        /// </summary>
        public int? CDWaitInStockNumber { get; set; }
        /// <summary>
        /// 成都待审核数量
        /// </summary>
        public int? CDWaitCheckNumber { get; set; }
        /// <summary>
        /// 成都在途数量
        /// </summary>
        public int? CDOnRoadNumber { get; set; }
        /// <summary>
        /// 成都仓W1
        /// </summary>
        public int? CDW1 { get; set; }

        #endregion

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
    }

    public class PurchaseOrderItemProductInfo
    {
        public int SysNo { get; set; }
        public string Id { get; set; }
        public string GiftId { get; set; }
        public string ProductName { get; set; }
        public int StockSysNo { get; set; }
        public string StockName { get; set; }
        public int PurchaseQty { get; set; }
        public int OnlineQty { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? CurrencySysNo { get; set; }
        public decimal TaxRate { get; set; }
        public PurchaseOrderType? POType { get; set; }
        public int PMUserSysNo { get; set; }
        public int POItemSysNo { get; set; }
        public decimal JDPrice { get; set; }
        public int POSysNo { get; set; }

        public int? AcquireReturnPointType { get; set; }
        public decimal? AcquireReturnPoint { get; set; }

        public int? ReadyQuantity { get; set; }
        //把附加的所有的Item都传过来
        public List<PurchaseOrderItemInfo> POItems { get; set; }
        /// <summary>
        /// 获取或设置是否为库存同步合作商品
        /// </summary>
        public string Vfitem { get; set; }

        public string VFType { get; set; }
        /// <summary>
        /// 获取或设置同步采购价格
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 产品正常采购价格
        /// </summary>
        public decimal? VirtualPrice { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }

        public string CompanyCode { get; set; }

    }
}
