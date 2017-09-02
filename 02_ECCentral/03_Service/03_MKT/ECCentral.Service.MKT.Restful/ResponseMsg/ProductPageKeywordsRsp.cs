using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.ResponseMsg
{
    /// <summary>
    /// 批量设置产品页面关键字
    /// </summary>
    public class ProductPageKeywordsRsp
    {
        public List<string> ProductList { get; set; }

        /// <summary>
        /// 是否为批量添加，否则批量删除
        /// </summary>
        public bool BatchAdd { get; set; }

        public string ReplKeywords1 { get; set; }

        public string ReplKeywords0 { get; set; }

        public string LanguageCode { get; set; }

        public string CompanyCode { get; set; }
    }
}
