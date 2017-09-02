using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 商家自定义扩展信息，用于关务对接相关信息，与Vendor主表为1对1的关系，
    /// Vendor创建时，需初始化本表
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorCustomsInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 商家编号，即在NESOFT系统中的VendorSysNo,用于关联Vendor主表
        /// </summary>
        [DataMember]
        public string MerchantSysNo { get; set; }

        /// <summary>
        /// 商检编号，
        /// </summary>
        [DataMember]
        public string InspectionCode { get; set; }

        /// <summary>
        /// 泰隆优选中的编号，用于5+2系统，同一个商家在KJT系统与5+2通用一个编号。
        /// </summary>
        [DataMember]
        public string KJTCode { get; set; }

        /// <summary>
        /// CBT(上海跨境电商商务进口服务平台)中的商户号，也用于东方支付的商户节点号。
        /// </summary>
        [DataMember]
        public string CBTMerchantCode { get; set; }

        /// <summary>
        /// CBT中的商户名称
        /// </summary>
        [DataMember]
        public string CBTMerchantName { get; set; }
        
        /// <summary>
        /// 一级商户号(SRC_NCODE, 东方支付需要的)，本项目中指泰隆优选的商户号作为一级商户号，
        /// 也可以直接使用2级商户号
        /// </summary>
        [DataMember]
        public string CBTSRC_NCode { get; set; }

        /// <summary>
        /// 二级商户号(REC_NCODE, 东方支付需要的)
        /// </summary>
        [DataMember]
        public string CBTREC_NCode { get; set; }

        /// <summary>
        /// 东方支付的商户密钥(128位)
        /// </summary>
        [DataMember]
        public string EasiPaySecretKey { get; set; }

        /// <summary>
        /// 商家外汇收款币种
        /// </summary>
        [DataMember]
        public string ReceiveCurrencyCode { get; set; }

        /// <summary>
        /// 付款币种，默认都是CNY，人民币
        /// </summary>
        [DataMember]
        public string PayCurrencyCode { get; set; }

        /// <summary>
        /// 5+2系统接口提供方（现在是交换中心）分配给接口调用方的身份标识符
        /// </summary>
        [DataMember]
        public string FTAppId { get; set; }

        /// <summary>
        /// 5+2系统接口提供方（现在是交换中心）分配给接口调用方的身份验证信息-验签密钥
        /// </summary>
        [DataMember]
        public string FTAppSecretKey { get; set; }

        /// <summary>
        /// 订单申报电商接口主体验签密钥
        /// </summary>
        [DataMember]
        public string CBTSODeclareSecretKey { get; set; }

        /// <summary>
        /// 商品在CBT备案接口验签密钥
        /// </summary>
        [DataMember]
        public string CBTProductDeclareSecretKey { get; set; }

        /// <summary>
        /// 商品在商检备案接口验签密钥,一期暂时不用。
        /// </summary>
        [DataMember]
        public string InspectionProductDeclareSecretKey { get; set; }

        /// <summary>
        /// 预留：以后泰隆优选开放API给外部商家时，分配给外部商家的验签密钥
        /// </summary>
        [DataMember]
        public string KJTAppSecretKey { get; set; }

        /// <summary>
        /// 预留：以后泰隆优选开放API给外部商家时，分配给外部商家的身份标识符
        /// </summary>
        [DataMember]
        public string KJTAppId { get; set; }
    }
}
