using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Payment
{
    /// <summary>
    /// 手机支付所需要的必须参数
    /// </summary>
    [XmlRoot("MobilePaySections")]
    public class MobilePaySections
    {
        /// <summary>
        /// 获取设置支付的配置信息
        /// </summary>
        [XmlArray("PaySectionList")]
        [XmlArrayItem("PaySectionItem")]
        public List<MobilePaySectionItem> PaymentItems { get; set; }

    }

    /// <summary>
    /// 手机支付所需要的必须参数
    /// </summary>
    public class MobilePaySectionItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute("name")]
        public string Payname { get; set; }
        /// <summary>
        /// 当前设备能否支持这个支付方式
        /// </summary>
        [XmlArray("SupportDevices")]
        [XmlArrayItem("Device")]
        public List<string> DeviceList { get; set; }

        /// <summary>
        /// 对应在PC上的PayTypeList的PayType,一般都是前台不展示
        /// </summary>
        [XmlArray("PayTypeIDs")]
        [XmlArrayItem("PayId")]
        public List<int> PayTypeIdList { get; set; }

        /// <summary>
        /// SellID，你懂得
        /// </summary>
        /// 
        [XmlElement("SellerID")]
        public string SellerID { get; set; }

        /// <summary>
        /// PartnerID，你懂得。
        /// </summary>

        [XmlElement("PartnerID")]
        public string PartnerID { get; set; }

        /// <summary>
        /// AsrPublicKey，你懂的。
        /// </summary>
        /// 
        [XmlElement("AsrPublicKey")]
        public string AsrPublicKey { get; set; }

        /// <summary>
        /// AsrPrivateKey，你懂得
        /// </summary>
        /// 
        [XmlElement("AsrPrivateKey")]
        public string AsrPrivateKey { get; set; }

        /// <summary>
        ///支付宝安全支付回调路径 
        /// </summary>
        [XmlElement("CallBackUrl")]
        public string CallBackUrl { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [XmlIgnore]
        [ScriptIgnore]
        public string Key
        {
            get { return Payname; }
        }

        /// <summary>
        ///公司编码 
        /// </summary>
        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }
    }
}