using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 配送方式-产品信息
    /// </summary>
    public class ShipTypeProductInfo : IIdentity, ICompany
    {
       
        /// <summary>
        /// 配送方式-产品ID
        /// </summary>
        /// </summary> 
        public int? SysNo
        {
            get;
            set;

        }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;

        }
        /// <summary>
        /// 类型
        /// </summary>
        public ShipTypeProductType? ShipTypeProductType
        {
            get;
            set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库
        /// </summary>
        public int? WareHouse
        {
            get;
            set;
        }
        /// <summary>
        /// 区域系统编号
        /// </summary>
        public int? AreaSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 省编号
        /// </summary>
        public int? ProvinceSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 市编号
        /// </summary>
        public int? CitySysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 县（地区）编号
        /// </summary>
        public int? DistrictSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? ShippingType
        {
            get;
            set;
        }
        /// <summary>
        /// 商品范围   P 商品 || C 商品类别
        /// </summary>
        public ProductRange? ProductRange
        {
            get;
            set;
        }
        /// <summary>
        /// 商品信息列表
        /// </summary>
        public List<ECCentral.BizEntity.IM.ProductInfo> ListProductInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品类别列表
        /// </summary>
        public List<ECCentral.BizEntity.IM.CategoryInfo> ListCategoryInfo
        {
            get;
            set;
        }
    }
}
