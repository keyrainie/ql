using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace ECCentral.Job.Invoice.AutoRunSOFreight.Biz
{
    [Serializable]
    [DataContract]
    public class ShippingInfo
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataMember]
        [DataMapping("TransID", DbType.String)]
        public string TransID { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        [DataMapping("ShippingTypeID", DbType.Int32)]
        public int ShippingTypeID { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        [DataMapping("ShippingPrice", DbType.Decimal)]
        public decimal ShippingPrice { get; set; }

        /// <summary>
        /// 打包费
        /// </summary>
        [DataMember]
        [DataMapping("ShippingPackageFee", DbType.Decimal)]
        public decimal ShippingPackageFee { get; set; }
    }
}
