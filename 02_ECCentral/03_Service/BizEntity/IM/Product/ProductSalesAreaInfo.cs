using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.BizEntity.IM.Product
{
    /// <summary>
    /// 商品销售区域信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductSalesAreaInfo : ICompany,ILanguage
    {
        /// <summary>
        /// 仓库信息
        /// </summary>
        [DataMember]
        public StockInfo Stock { get; set; }

        /// <summary>
        /// 销售区域信息
        /// </summary>
        [DataMember]
        public AreaInfo Province { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperationUser { get; set; }
    }
}
