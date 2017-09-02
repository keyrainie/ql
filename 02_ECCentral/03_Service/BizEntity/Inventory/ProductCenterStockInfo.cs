using System;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 备货中心-商品仓库库存和销量信息
    /// </summary>
    public class ProductCenterStockInfo
    {
        public ProductCenterStockInfo()
        {
            PurchaseQty = 0;
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ItemSysNumber { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int? _AvailableQty;
        public int? AvailableQty
        {
            get { return _AvailableQty; }
            set { _AvailableQty = value ?? 0; }
        }

        /// <summary>
        /// 虚库库存
        /// </summary>
        public int? _VirtualQty;
        public int? VirtualQty
        {
            get { return _VirtualQty; }
            set { _VirtualQty = value ?? 0; }
        }

        /// <summary>
        /// 被订购库存
        /// </summary>
        public int? _OrderQty;
        public int? OrderQty
        {
            get { return _OrderQty; }
            set { _OrderQty = value ?? 0; }
        }

        /// <summary>
        /// 代销库存
        /// </summary>
        public int? _ConsignQty;
        public int? ConsignQty
        {
            get { return _ConsignQty; }
            set { _ConsignQty = value ?? 0; }
        }

        public int? _AvailableQtyStock;
        public int? AvailableQtyStock 
        {
            get { return _AvailableQtyStock; }
            set { _AvailableQtyStock = value ?? 0; }
        }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? _PurchaseQty;
        public int? PurchaseQty
        {
            get { return _PurchaseQty; }
            set { _PurchaseQty = value ?? 0; }
        }

        /// <summary>
        /// 采购单据数量
        /// </summary>
        public int? _PurchaseInQty;
        public int? PurchaseInQty
        {
            get { return _PurchaseInQty; }
            set { _PurchaseInQty = value ?? 0; }
        }

        /// <summary>
        /// 移仓在途数量
        /// </summary>
        public int? _ShiftInQty;
        public int? ShiftInQty
        {
            get { return _ShiftInQty; }
            set { _ShiftInQty = value ?? 0; }
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
        /// 覆盖地区W1销量
        /// </summary>
        public int? _W1RegionSalesQty; 
        public int? W1RegionSalesQty 
        {
            get { return _W1RegionSalesQty; }
            set { _W1RegionSalesQty = value ?? 0; }
        }

        /// <summary>
        /// 覆盖地区W2销量
        /// </summary>
        public int? _W2RegionSalesQty; 
        public int? W2RegionSalesQty 
        {
            get { return _W2RegionSalesQty; }
            set { _W2RegionSalesQty = value ?? 0; }
        }

        public decimal? _W1RegionC3SalesQtyRate;
        public decimal? W1RegionC3SalesQtyRate 
        {
            get { return _W1RegionC3SalesQtyRate; }
            set { _W1RegionC3SalesQtyRate = value ?? 0; }
        }

        public decimal? _W2RegionC3SalesQtyRate;
        public decimal? W2RegionC3SalesQtyRate 
        {
            get { return _W2RegionC3SalesQtyRate; }
            set { _W2RegionC3SalesQtyRate = value ?? 0; }
        }

        /// <summary>
        /// 覆盖地区M1销量
        /// </summary>
        public int? _M1RegionSalesQty;
        public int? M1RegionSalesQty
        {
            get { return _M1RegionSalesQty; }
            set { _M1RegionSalesQty = value ?? 0; }
        }

        /// <summary>
        /// 建议备货数量
        /// </summary>
        public int? _SuggestQty;
        public int? SuggestQty
        {
            get { return _SuggestQty; }
            set { _SuggestQty = value ?? 0; }
        }

        /// <summary>
        /// 最后一次采购价格
        /// </summary>
        public decimal? _LastPrice;
        public decimal? LastPrice
        {
            get { return _LastPrice; }
            set { _LastPrice = value ?? 0.00M; }
        }

        public DateTime? LastintimeForDBMap { get; set; }

        /// <summary>
        /// 最后一次采购时间
        /// </summary>
        public DateTime? Lastintime
        {
            get
            {
                if (LastintimeForDBMap == Convert.ToDateTime("0001-01-01 00:00:00") || LastintimeForDBMap == null)
                {
                    return null;
                }
                else
                {
                    return LastintimeForDBMap.Value;
                }
            }
        }

        /// <summary>
        /// 可移库存
        /// </summary>
        public int? _OutStockShiftQty;
        public int? OutStockShiftQty
        {
            get { return _OutStockShiftQty; }
            set { _OutStockShiftQty = value ?? 0; }
        }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public string _WareHouseNumber;
        public string WareHouseNumber
        {
            get { return _WareHouseNumber.Trim(); }
            set { _WareHouseNumber = value; }
        }

        /// <summary>
        /// 最小包装数量
        /// </summary>
        public int? MinPackNumber { get; set; }

        /// <summary>
        /// 最后一次采购供应商送货周期
        /// </summary>
        public string SendPeriod { get; set; }

        /// <summary>
        /// 是否中转(需要重新计算分仓的建议备货数量)
        /// </summary>
        public int? _SuggestQtyZhongZhuan;
        public int? SuggestQtyZhongZhuan
        {
            get { return _SuggestQtyZhongZhuan; }
            set { _SuggestQtyZhongZhuan = value ?? 0; }
        }

        /// <summary>
        /// 商品对应分仓的日均销售量
        /// </summary>
        public decimal? AVGDailySales { get; set; }

        /// <summary>
        /// 商品对应分仓的可销售天数
        /// </summary>
        public int? _AvailableSalesDays;
        public int? AvailableSalesDays
        {
            get { return _AvailableSalesDays; }
            set { _AvailableSalesDays = value ?? 0; }
        }

        /// <summary>
        /// 采购价格
        /// </summary>
        public string _Price;
        public string Price
        {
            get { return string.IsNullOrEmpty(_Price) ? "0.00" : _Price; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _Price = "0.00";
                }
                else
                {
                    _Price = value;
                }
            }
        }

        /// <summary>
        /// 是否中转
        /// </summary>
        public YNStatus? NeedBufferEnable { get; set; }

        /// <summary>
        /// 是否中转
        /// </summary>
        public bool NeedBufferVisible { get; set; }

    }
}
