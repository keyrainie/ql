using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
 public class ProductSalesAreaBatchQueryFilter
    {
     /// <summary>
     /// 也面包
     /// </summary>
     public PagingInfo PageInfo {get;set; }
     /// <summary>
     /// 类别1
     /// </summary>
     public int? Category1SysNo { get; set; }
     /// <summary>
     /// 类别2
     /// </summary>
     public int? Category2SysNo { get; set; }
     /// <summary>
     /// 类别3
     /// </summary>
     public int? Category3SysNo { get; set; }
     /// <summary>
     /// 产品
     /// </summary>
     public string ProductName { get; set; }

     /// <summary>
     /// 生产商
     /// </summary>
     public string ManufacturerName { get; set; }
     
     /// <summary>
     /// 商品状态
     /// </summary>
     public ProductStatus? ProductStatus { get; set; }

     /// <summary>
     /// 供应商
     /// </summary>
     public string VendorName { get; set; }

     /// <summary>
     /// 省份的SysNo
     /// </summary>
     public string ProvinceSysNos { get; set; }

     /// <summary>
     /// 仓库SysNo
     /// </summary>
     public string StockSysNos { get; set; }


     /// <summary>
     /// 界面查询有两种选择1.商品，2记录 IsSearchProduct=true 表示查商品
     /// </summary>
     public bool IsSearchProduct { get; set; }
    }
}
