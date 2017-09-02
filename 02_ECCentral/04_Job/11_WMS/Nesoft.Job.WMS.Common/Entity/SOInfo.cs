using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.Common.Entity
{
    public class SOInfo : BaseEntity
    {
        public SOInfo()
        {
            this.OrderItems = new List<SOItemInfo>();
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        [JsonProperty("merchantOrderId")]
        public string SOID { get; set; }
        /// <summary>
        /// 商家在海关的备案号
        /// </summary>
        [JsonProperty("merchantCode")]
        public string MerchantCode { get; set; }
        /// <summary>
        /// 电商备案时提供的完整名称
        /// </summary>
        [JsonProperty("merchantName")]
        public string MerchantName { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        [JsonIgnore]
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 请求时间字符串
        /// </summary>
        [JsonProperty("orderCommitTime")]
        public string CommitTime
        {
            get
            {
                return this.OrderDate.ToString("yyyyMMddhhmmss");
            }
        }
        /// <summary>
        /// 海关关区代码
        /// 字符串： 4 位字符 
        /// 直邮进口模式：关区代码为 2244； 
        /// 浦东机场自贸模式：关区代码为 2216； 
        /// 洋山自贸模式：关区代码为 2249； 
        /// 外高桥自贸模式：关区代码为 2218
        /// </summary>
        [JsonProperty("custCode")]
        public int CustomsCode { get; set; }
        /// <summary>
        /// （为空）
        /// </summary>
        [JsonProperty("cargoDescript")]
        public string CargoDescript { get; set; }
        /// <summary>
        /// （为空）
        /// 字符串，S01：一般进口 S02：保税区进口 为空默认 S01
        /// </summary>
        [JsonProperty("serverType")]
        public string ServerType
        {
            get
            {
                //自贸
                if (this.StockType == 2)
                {
                    return "S02";
                }
                return "S01";
            }
        }
        /// <summary>
        /// 仓库类型，用于判断ServerType
        /// </summary>
        [JsonIgnore]
        public int StockType { get; set; }

        #region 订单物流信息
        /// <summary>
        /// 物流分运单号: 物流提供的唯一分运单号
        /// </summary>
        [JsonProperty("assBillNo")]
        public string AssBillNo { get; set; }
        /// <summary>
        /// 物流公司代码: 字符串 UPS
        /// </summary>
        [JsonProperty("logisticsCorp")]
        public string LogisticsCorp { get; set; }
        /// <summary>
        /// 发件人姓名
        /// </summary>
        [JsonProperty("senderName")]
        public string SenderName { get; set; }
        /// <summary>
        /// 发件人电话
        /// </summary>
        [JsonProperty("senderTel")]
        public string SenderTel { get; set; }
        /// <summary>
        /// 发件方公司名称
        /// </summary>
        [JsonProperty("senderCompanyName")]
        public string SenderCompanyName { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        [JsonProperty("senderAddr")]
        public string SenderAddr { get; set; }
        /// <summary>
        /// 件地邮编: 数字串，例如：200135
        /// </summary>
        [JsonProperty("senderZip")]
        public string SenderZip { get; set; }
        /// <summary>
        /// 发件地城市
        /// </summary>
        [JsonProperty("senderCity")]
        public string SenderCity { get; set; }
        /// <summary>
        /// 发件地省/州名
        /// </summary>
        [JsonProperty("senderProvince")]
        public string SenderProvince { get; set; }
        /// <summary>
        /// 发件地国家: 字符串 参见附录中国家或地区代码中英文代码 例如：USA（代表美国）CHN（代表中国）
        /// </summary>
        [JsonProperty("senderCountry")]
        public string SenderCountry { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [JsonProperty("recName")]
        public string RecName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>
        [JsonProperty("recTel")]
        public string RecTel { get; set; }
        /// <summary>
        /// 收货地国家: 字符串 货品接收所在国家 例如：中国
        /// </summary>
        [JsonProperty("recCountry")]
        public string RecCountry { get; set; }
        /// <summary>
        /// 收货地省/州:  字符串 货品接收所在省/州 例如：上海
        /// </summary>
        [JsonProperty("recProvince")]
        public string RecProvince { get; set; }
        /// <summary>
        /// 收货地城市:  字符串 货品接收所在省/州 例如：上海
        /// </summary>
        [JsonProperty("recCity")]
        public string RecCity { get; set; }
        /// <summary>
        /// 收货地地址: 例如：浦东新区博霞路 160 号
        /// </summary>
        [JsonProperty("recAddr")]
        public string RecAddr { get; set; }
        /// <summary>
        /// 收货地邮编 : 例如：200135
        /// </summary>
        [JsonProperty("recZip")]
        public string RecZip { get; set; }
        #endregion

        #region 订单支付信息
        /// <summary>
        /// 付款币种: 字符串，参见附录币种代码 CNY 人民币 USD 美元
        /// </summary>
        [JsonProperty("payCUR")]
        public string PayCurrencyCode
        {
            get
            {
                return "CNY";
            }
        }
        /// <summary>
        /// 全部购买商品合计总价: 数字串中保留 2 位小数，如120.50 或 100.00，无费用时为 0
        /// </summary>
        [JsonIgnore]
        public decimal AllTotalPrice { get; set; }
        [JsonProperty("allCargoTotalPrice")]
        public string AllTotalPriceP
        {
            get
            {
                return this.AllTotalPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 全部购买商品行邮税合计总价: 数字串中保留 2 位小数，如120.50 或 100.00，无费用时为 0
        /// </summary>
        [JsonIgnore]
        public decimal AllTotalTax { get; set; }
        [JsonProperty("allCargoTotalTax")]
        public string AllTotalTaxP
        {
            get
            {
                return this.AllTotalTax.ToString("#0.00");
            }
        }
        /// <summary>
        /// 物流运费: 数字串中保留 2 位小数，如120.50 或 100.00，无费用时为 0
        /// </summary>
        [JsonIgnore]
        public decimal ExpressPrice { get; set; }
        [JsonProperty("expressPrice")]
        public string ExpressPriceP
        {
            get
            {
                return this.ExpressPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 其它费用: 数字串中保留 2 位小数，如120.50 或 100.00，无费用时为 0
        /// </summary>
        [JsonIgnore]
        public decimal OtherPrice { get; set; }
        [JsonProperty("otherPrice")]
        public string OtherPriceP
        {
            get
            {
                return this.OtherPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 支付总金额: 数字串中保留 2 位小数，如120.50 或 100.00，无费用时为 0
        /// </summary>
        [JsonIgnore]
        public decimal PayAmount { get; set; }
        [JsonProperty("payAmount")]
        public string PayAmountP
        {
            get
            {
                return this.PayAmount.ToString("#0.00");
            }
        }
        #endregion
        [JsonProperty("orderDetail")]
        public List<SOItemInfo> OrderItems { get; set; }
    }
    public class SOItemInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonProperty("cargoName")]
        public string ItemName { get; set; }
        /// <summary>
        /// 商品备案号
        /// </summary>
        [JsonProperty("cargoCode")]
        public string EntryCode { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        [JsonProperty("cargoNum")]
        public int ItemQty { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [JsonIgnore]
        public decimal UnitPrice { get; set; }
        [JsonProperty("cargoUnitPrice")]
        public string UnitPriceP
        {
            get
            {
                return this.UnitPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 商品总价
        /// </summary>
        [JsonIgnore]
        public decimal TotalPrice { get; set; }
        [JsonProperty("cargoTotalPrice")]
        public string TotalPriceP
        {
            get
            {
                return this.TotalPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 单项购买商品行邮税总价
        /// </summary>
        [JsonIgnore]
        public decimal TotalTax { get; set; }
        [JsonProperty("cargoTotalTax")]
        public string TotalTaxP
        {
            get
            {
                return this.TotalTax.ToString("#0.00");
            }
        }
        [JsonIgnore]
        public decimal TariffAmt { get; set; }
        [JsonIgnore]
        public decimal DiscountAmt { get; set; }
        [JsonIgnore]
        public decimal OriginalPrice { get; set; }
    }
}
