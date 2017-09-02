using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public class ShippingInfo : ExtensibleObject
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string TransID { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int ShippingTypeID { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal ShippingPrice { get; set; }

        /// <summary>
        /// 打包费
        /// </summary>
        public decimal ShippingPackageFee { get; set; }


        public override ExtensibleObject CloneObject()
        {
            return new ShippingInfo()
            {
                TransID =this.TransID,
                ShippingTypeID = this.ShippingTypeID,
                ShippingPrice = this.ShippingPrice,
                ShippingPackageFee = this.ShippingPackageFee
            };
        }
    }
}
