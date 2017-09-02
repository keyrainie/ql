using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品商检信息(数据交换中心传回的商品的商检信息)
    /// </summary>
    public class ProductInspectionInfo
    {
        public string KjtCode { get; set; }

        public int? SysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 物资序号
        /// </summary>
        public string MaterialID { get; set; }
        /// <summary>
        /// 海关申报编号
        /// </summary>
        public string CodeT { get; set; }

        /// <summary>
        /// 附加编码
        /// </summary>
        public string CodeS { get; set; }
        /// <summary>
        /// 申报计量单位
        /// </summary>
        public string GUnit { get; set; }
        /// <summary>
        /// 法定计量单位
        /// </summary>
        public string Unit1 { get; set; }
        /// <summary>
        /// 第二计量单位
        /// </summary>
        public string Unit2 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }
    }
}
