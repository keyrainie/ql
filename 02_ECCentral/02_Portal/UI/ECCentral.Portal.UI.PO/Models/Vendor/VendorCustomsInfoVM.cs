using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorCustomsInfoVM : ModelBase
    {
        private string _cbtMerchantCode;
        private string _cbtMerchantName;
        private string _cbtProductDeclareSecretKey;
        private string _cbtrecNCode;
        private string _cbtsoDeclareSecretKey;
        private string _cbtsrcNCode;
        private string _easiPaySecretKey;
        private string _ftAppId;
        private string _ftAppSecretKey;
        private string _inspectionCode;
        private string _inspectionProductDeclareSecretKey;
        private string _kjtAppId;
        private string _kjtCode;
        private string _kjtAppSecretKey;
        private string _merchantSysNo;
        private string _payCurrencyCode;
        private string _receiveCurrencyCode;
        private int? _sysNo;

        /// <summary>
        /// 主键
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        /// <summary>
        /// 商家编号，即在NESOFT系统中的VendorSysNo,用于关联Vendor主表
        /// </summary>
        public string MerchantSysNo
        {
            get { return _merchantSysNo; }
            set { SetValue("MerchantSysNo", ref _merchantSysNo, value); }
        }

        /// <summary>
        /// 商检编号，
        /// </summary>
        public string InspectionCode
        {
            get { return _inspectionCode; }
            set { SetValue("InspectionCode", ref _inspectionCode, value); }
        }

        /// <summary>
        /// 泰隆优选中的编号，用于5+2系统，同一个商家在KJT系统与5+2通用一个编号。
        /// </summary>
        public string KJTCode
        {
            get { return _kjtCode; }
            set { SetValue("KJTCode", ref _kjtCode, value); }
        }

        /// <summary>
        /// CBT(上海跨境电商商务进口服务平台)中的商户号，也用于东方支付的商户节点号。
        /// </summary>
        public string CBTMerchantCode
        {
            get { return _cbtMerchantCode; }
            set { SetValue("CBTMerchantCode", ref _cbtMerchantCode, value); }
        }

        /// <summary>
        /// CBT中的商户名称
        /// </summary>
        public string CBTMerchantName
        {
            get { return _cbtMerchantName; }
            set { SetValue("CBTMerchantName", ref _cbtMerchantName, value); }
        }

        /// <summary>
        /// 一级商户号(SRC_NCODE, 东方支付需要的)，本项目中指泰隆优选的商户号作为一级商户号，
        /// 也可以直接使用2级商户号
        /// </summary>
        public string CBTSRC_NCode
        {
            get { return _cbtsrcNCode; }
            set { SetValue("CBTSRC_NCode", ref _cbtsrcNCode, value); }
        }

        /// <summary>
        /// 二级商户号(REC_NCODE, 东方支付需要的)
        /// </summary>
        public string CBTREC_NCode
        {
            get { return _cbtrecNCode; }
            set { SetValue("CBTREC_NCode", ref _cbtrecNCode, value); }
        }

        /// <summary>
        /// 东方支付的商户密钥(128位)
        /// </summary>
        public string EasiPaySecretKey
        {
            get { return _easiPaySecretKey; }
            set { SetValue("EasiPaySecretKey", ref _easiPaySecretKey, value); }
        }

        /// <summary>
        /// 商家外汇收款币种
        /// </summary>
        public string ReceiveCurrencyCode
        {
            get { return _receiveCurrencyCode; }
            set { SetValue("ReceiveCurrencyCode", ref _receiveCurrencyCode, value); }
        }

        /// <summary>
        /// 付款币种，默认都是CNY，人民币
        /// </summary>
        public string PayCurrencyCode
        {
            get { return _payCurrencyCode; }
            set { SetValue("PayCurrencyCode", ref _payCurrencyCode, value); }
        }

        /// <summary>
        /// 5+2系统接口提供方（现在是交换中心）分配给接口调用方的身份标识符
        /// </summary>
        public string FTAppId
        {
            get { return _ftAppId; }
            set { SetValue("FTAppId", ref _ftAppId, value); }
        }

        /// <summary>
        /// 5+2系统接口提供方（现在是交换中心）分配给接口调用方的身份验证信息-验签密钥
        /// </summary>
        public string FTAppSecretKey
        {
            get { return _ftAppSecretKey; }
            set { SetValue("FTAppSecretKey", ref _ftAppSecretKey, value); }
        }

        /// <summary>
        /// 订单申报电商接口主体验签密钥
        /// </summary>
        public string CBTSODeclareSecretKey
        {
            get { return _cbtsoDeclareSecretKey; }
            set { SetValue("CBTSODeclareSecretKey", ref _cbtsoDeclareSecretKey, value); }
        }

        /// <summary>
        /// 商品在CBT备案接口验签密钥
        /// </summary>
        public string CBTProductDeclareSecretKey
        {
            get { return _cbtProductDeclareSecretKey; }
            set { SetValue("CBTProductDeclareSecretKey", ref _cbtProductDeclareSecretKey, value); }
        }

        /// <summary>
        /// 商品在商检备案接口验签密钥,一期暂时不用。
        /// </summary>
        public string InspectionProductDeclareSecretKey
        {
            get { return _inspectionProductDeclareSecretKey; }
            set { SetValue("InspectionProductDeclareSecretKey", ref _inspectionProductDeclareSecretKey, value); }
        }

        /// <summary>
        /// 预留：以后泰隆优选开放API给外部商家时，分配给外部商家的验签密钥
        /// </summary>
        public string KJTAppSecretKey
        {
            get { return _kjtAppSecretKey; }
            set { SetValue("KJTAppSecretKey", ref _kjtAppSecretKey, value); }
        }

        /// <summary>
        /// 预留：以后泰隆优选开放API给外部商家时，分配给外部商家的身份标识符
        /// </summary>
        public string KJTAppId
        {
            get { return _kjtAppId; }
            set { SetValue("KJTAppId", ref _kjtAppId, value); }
        }
    }
}