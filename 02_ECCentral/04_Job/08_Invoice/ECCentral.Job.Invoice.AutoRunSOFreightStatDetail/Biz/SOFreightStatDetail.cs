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
    public class SOFreightStatDetail
    {
        /// <summary>
        /// 订单系统编号
        /// </summary>
        [DataMember]
        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        [DataMember]
        [DataMapping("TrackingNumber", DbType.String)]
        public String TrackingNumber { get; set; }
        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int ShipTypeSysNo { get; set; }
        /// <summary>
        /// 订单重量(g)
        /// </summary>
        [DataMember]
        [DataMapping("SOWeight", DbType.Int32)]
        public int SOWeight { get; set; }
        /// <summary>
        /// 订单收入的运费
        /// </summary>
        [DataMember]
        [DataMapping("SOFreight", DbType.Decimal)]
        public decimal SOFreight { get; set; }
        /// <summary>
        /// 实际重量(KG)
        /// </summary>
        [DataMember]
        [DataMapping("RealWeight", DbType.Decimal)]
        public decimal RealWeight { get; set; }
        /// <summary>
        /// 实际应付运费
        /// </summary>
        [DataMember]
        [DataMapping("RealPayFreight", DbType.Decimal)]
        public decimal RealPayFreight { get; set; }
        /// <summary>
        /// 订单时间
        /// </summary>
        [DataMember]
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        [DataMember]
        [DataMapping("OutDate", DbType.DateTime)]
        public DateTime OutDate { get; set; }
        /// <summary>
        /// 货币编号
        /// </summary>
        [DataMember]
        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }
        /// <summary>
        /// 最大单品重量 
        /// </summary>
        [DataMember]
        [DataMapping("SOSingleMaxWeight", DbType.Int32)]
        public int SOSingleMaxWeight { get; set; }
        /// <summary>
        /// 地区编号
        /// </summary>
        [DataMember]
        [DataMapping("ReceiveAreaSysNo", DbType.Int32)]
        public int ReceiveAreaSysNo { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        [DataMember]
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        [DataMapping("SoAmount", DbType.Decimal)]
        public decimal SoAmount { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        [DataMapping("LocalWHSysNo", DbType.String)]
        public string LocalWHSysNo { get; set; }
    }
}
