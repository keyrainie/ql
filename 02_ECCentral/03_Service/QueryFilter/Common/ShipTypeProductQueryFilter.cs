using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
  public class ShipTypeProductQueryFilter
    {
         public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 配送方式-产品ID
        /// </summary>
        public int? SysNo { get; set;}
        /// <summary>
        ///类型
        /// </summary>
        public ShipTypeProductType?  ShipTypeProductType{ get; set;}
        /// <summary>
        /// 商户
        /// </summary>
        public CompanyCustomer? CompanyCustomer { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description{ get; set;}       
        /// <summary>
        /// 仓库
        /// </summary>
        public int? WareHouse{ get; set;}
        /// <summary>
        /// 收货区域编号
        /// </summary>
        public int? AreaSysNo { get; set; }
        /// <summary>
        /// 收货省份编号
        /// </summary>
        public int? ProvinceSysNo { get; set; }
        /// <summary>
        /// 收货城市编号
        /// </summary>
        public int? CitySysNo { get; set; }
        /// <summary>
        /// 地区编号
        /// </summary>
        public string DistrictSysNo { get; set; }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? ShippingType{ get; set;}
        /// <summary>
        /// 商品范围
        /// </summary>
        public  ProductRange? ProductRange{ get; set;}
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID{ get; set;}
        /// <summary>
        /// 商品类别C1
        /// </summary>
        public int? Category1 { get; set; }
        /// <summary>
        /// 商品类别C2
        /// </summary>
        public int? Category2 { get; set; }
        /// <summary>
        /// 商品类别C3
        /// </summary>
        public int? Category3{ get; set;}
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string CompanyCode { get; set; }

    }
}
