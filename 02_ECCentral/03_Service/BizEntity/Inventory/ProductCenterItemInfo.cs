using System;
using System.Net;
using System.Collections.Generic;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 备货中心商品信息
    /// </summary>
    public class ProductCenterItemInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ItemSysNumber { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public int? ProductStatus { get; set; }

        /// <summary>
        /// 总可用库存
        /// </summary>
        public int? _AllAvailableQty;
        public int? AllAvailableQty
        {
            get { return _AllAvailableQty; }
            set { _AllAvailableQty = value ?? 0; }
        }

        /// <summary>
        /// 总代销库存
        /// </summary>
        public int? _ConsignQty;
        public int? ConsignQty
        {
            get { return _ConsignQty; }
            set { _ConsignQty = value ?? 0; }
        }

        /// <summary>
        /// 是否是库存同步商品
        /// </summary>
        public int? IsSynProduct { get; set; }

        /// <summary>
        /// 新单合作方商品编号
        /// </summary>
        public string SynProductID { get; set; }

        /// <summary>
        /// 合作伙伴类型
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// 库存同步数量
        /// </summary>
        public int? _InventoryQty;
        public int? InventoryQty 
        {
            get { return _InventoryQty; }
            set { _InventoryQty = value ?? 0; }
        }

        /// <summary>
        /// 同步采购价格
        /// </summary>
        public decimal? _PurchasePrice;
        public decimal? PurchasePrice 
        {
            get { return _PurchasePrice; }
            set { _PurchasePrice = value ?? 0.00M; }
        }

        /// <summary>
        /// 库存合作商品描述
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// 采购在途数量
        /// </summary>
        public int? _PurchaseQty;
        public int? PurchaseQty 
        {
            get { return _PurchaseQty; }
            set { _PurchaseQty = value ?? 0; }
        }

        /// <summary>
        /// 滞销库存数量
        /// </summary>
        public string _UnmarketableQty;
        /// <summary>
        /// 滞销库存数量
        /// </summary>
        public string UnmarketableQty 
        {
            get { return _UnmarketableQty; }
            set { _UnmarketableQty = value; }
        }

        /// <summary>
        /// 滞销商品的库龄
        /// </summary>
        public int? _InstockDays;
        public int? InstockDays
        {
            get { return _InstockDays; }
            set { _InstockDays = value ?? 0; }
        }

        /// <summary>
        /// 正常采购价格
        /// </summary>
        public decimal? _VirtualPrice;
        /// <summary>
        /// 正常采购价格
        /// </summary>
        public decimal? VirtualPrice 
        {
            get { return _VirtualPrice; }
            set { _VirtualPrice = value ?? 0.00M; }
        }

        /// <summary>
        /// 总被订购库存
        /// </summary>
        public int? _OrderQty;
        public int? OrderQty 
        {
            get { return _OrderQty; }
            set { _OrderQty = value ?? 0; }
        }

        /// <summary>
        /// 总虚库库存
        /// </summary>
        public int? _VirtualQty;
        public int? VirtualQty 
        {
            get { return _VirtualQty; }
            set { _VirtualQty = value ?? 0; }
        }

        /// <summary>
        /// 中转仓库存
        /// </summary>
        public int? _TransferStockQty;
        public int? TransferStockQty 
        {
            get { return _TransferStockQty; }
            set { _TransferStockQty = value ?? 0; }
        }

        /// <summary>
        /// 建议备货总数
        /// </summary>
        public int? _SuggestQtyAll;
        public int? SuggestQtyAll
        {
            get { return _SuggestQtyAll; }
            set { _SuggestQtyAll = value ?? 0; }
        }

        /// <summary>
        ///  是否中专需要重新既算 总建议备货数量
        /// </summary>
        public int? _SuggestQtyAllZhongZhuan;
        public int? SuggestQtyAllZhongZhuan
        {
            get { return _SuggestQtyAllZhongZhuan; }
            set { _SuggestQtyAllZhongZhuan = value ?? 0; }
        }

        /// <summary>
        /// 成本价格
        /// </summary>
        public decimal? _UnitCost;
        public decimal? UnitCost
        {
            get { return _UnitCost; }
            set { _UnitCost = value ?? 0.00M; }
        }

        /// <summary>
        /// D1(隔一天之前的那天)
        /// </summary>
        public int? _D1;
        public int? D1
        {
            get { return _D1; }
            set { _D1 = value ?? 0; }
        }

        /// <summary>
        /// D2
        /// </summary>
        public int? _D2;
        public int? D2
        {
            get { return _D2; }
            set { _D2 = value ?? 0; }
        }

        /// <summary>
        /// D3
        /// </summary>
        public int? _D3;
        public int? D3
        {
            get { return _D3; }
            set { _D3 = value ?? 0; }
        }

        /// <summary>
        /// D4
        /// </summary>
        public int? _D4;
        public int? D4
        {
            get { return _D4; }
            set { _D4 = value ?? 0; }
        }

        /// <summary>
        /// D5
        /// </summary>
        public int? _D5;
        public int? D5
        {
            get { return _D5; }
            set { _D5 = value ?? 0; }
        }

        /// <summary>
        /// D6
        /// </summary>
        public int? _D6;
        public int? D6
        {
            get { return _D6; }
            set { _D6 = value ?? 0; }
        }

        /// <summary>
        /// D7
        /// </summary>
        public int? _D7;
        public int? D7
        {
            get { return _D7; }
            set { _D7 = value ?? 0; }
        }

        /// <summary>
        /// D123
        /// </summary>
        public int? _D123;
        public int? D123
        {
            get { return _D123; }
            set { _D123 = value ?? 0; }
        }

        /// <summary>
        /// W1
        /// </summary>
        public int? _W1;
        public int? W1
        {
            get { return _W1; }
            set { _W1 = value ?? 0; }
        }

        /// <summary>
        /// W2
        /// </summary>
        public int? _W2;
        public int? W2
        {
            get { return _W2; }
            set { _W2 = value ?? 0; }
        }

        /// <summary>
        /// W3
        /// </summary>
        public int? _W3;
        public int? W3
        {
            get { return _W3; }
            set { _W3 = value ?? 0; }
        }

        /// <summary>
        /// W4
        /// </summary>
        public int? _W4;
        public int? W4
        {
            get { return _W4; }
            set { _W4 = value ?? 0; }
        }


        /// <summary>
        /// M1
        /// </summary>
        public int? _M1;
        public int? M1
        {
            get { return _M1; }
            set { _M1 = value ?? 0; }
        }

        /// <summary>
        /// M2
        /// </summary>
        public int? _M2;
        public int? M2
        {
            get { return _M2; }
            set { _M2 = value ?? 0; }
        }

        /// <summary>
        /// M3
        /// </summary>
        public int? _M3;
        public int? M3
        {
            get { return _M3; }
            set { _M3 = value ?? 0; }
        }

        /// <summary>
        /// 采购价格备注
        /// </summary>
        public string PO_Memo { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 京东价格
        /// </summary>
        public decimal? JDPrice { get; set; }

        /// <summary>
        /// 是否为代销商品
        /// </summary>
        public int? IsConsign { get; set; }


        public string VFType { get; set; }

        /// <summary>
        /// 商品整网的日均销售量
        /// </summary>
        public decimal? _AllStockAVGDailySales;
        public decimal? AllStockAVGDailySales
        {
            get { return _AllStockAVGDailySales; }
            set { _AllStockAVGDailySales = value ?? 0.00M; }
        }

        /// <summary>
        /// 商品整网可销售天数
        /// </summary>
        public int? _AllStockAvailableSalesDays;
        public int? AllStockAvailableSalesDays
        {
            get { return _AllStockAvailableSalesDays; }
            set { _AllStockAvailableSalesDays = value ?? 0; }
        }

        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string BrandCh { get; set; }

        /// <summary>
        /// 品牌英文文名称
        /// </summary>
        public string BrandEn { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Brand
        {
            get
            {
                return string.IsNullOrEmpty(BrandCh) ? BrandEn : BrandCh;
            }
        }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 各仓库信息列表
        /// </summary>
        public List<ProductCenterStockInfo> SuggestTransferStocks
        {
            get;
            set;
        }

        /// <summary>
        /// 是否批号管理商品
        /// </summary>
        public string IsBatch { get; set; }

        /// <summary>
        /// 商品当天出库量
        /// </summary>
        public int? _AllOutStockQuantity;
        public int? AllOutStockQuantity
        {
            get { return _AllOutStockQuantity; }
            set { _AllOutStockQuantity = value ?? 0; }
        }

        /// <summary>
        /// 对应的京东商品编号
        /// </summary>
        public String JDItemNumber { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int? Point { get; set; }

        /// <summary>
        /// 商品毛利率
        /// </summary>
        public string GrossProfitRate
        {
            get
            {

                decimal getCurrentPrice = CurrentPrice.HasValue ? CurrentPrice.Value : 0m;
                decimal getUnitCost = UnitCost.HasValue ? UnitCost.Value : 0m;
                int getPoint = Point.HasValue ? Point.Value : 0;



                if ((getCurrentPrice - (decimal?)(getPoint * 0.1)) != 0 && (getCurrentPrice - (decimal?)(getPoint * 0.1) - getUnitCost) != 0)
                {
                    decimal tempGPR = (getCurrentPrice - (decimal)(getPoint * 0.1) - getUnitCost) / (getCurrentPrice - (decimal)(getPoint * 0.1));
                    return Math.Round(tempGPR * 100, 2).ToString() + "%";
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

    }
}
