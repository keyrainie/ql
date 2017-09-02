using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SOOutStockQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        //订单号
        public string SOID { get; set; }
        //出库日期(从)
        public DateTime? ShippedOutTimeFrom { get; set; }
        //出库日期(至)
        public DateTime? ShippedOutTimeTo { get; set; }
        //客户姓名
        public string CustomerName { get; set; }
        //商品编号
        public string ProductSysNo { get; set; }
        //分仓
        public int? StockSysNo { get; set; }
        //配送方式
        public int? ShipTypeSysNo { get; set; }
        //是否增票
        public SYNStatus? IsVAT { get; set; }
        //是否大件
        public SYNStatus? IsBig { get; set; }
        //配送员
        public int? DeliveryPsersonNo { get; set; }
        //是否VIP客户
        public SYNStatus? IsVIPCustomer { get; set; }
        //是否已打包裹单
        public SYNStatus? IsPackaged { get; set; }
        //是否特殊订单
        public SOIsSpecialOrder? SpecialSOType { get; set; }
        //是否当前数据
        public SOIsCurrentData? IsCurrentData { get; set; }
        //启用顾客首次使用该配送方式过滤
        public SYNStatus? IsFirstFilter { get; set; }
        //收获地址
        public string ReceiveAddress { get; set; }
        //收获地址所属地区
        public int? ReceiveAreaSysNo { get; set; }
        //是否按并单统计
        public bool IsUniteOrderCount { get; set; }
        //公司代码
        public string CompanyCode { get; set; }
    }
}
