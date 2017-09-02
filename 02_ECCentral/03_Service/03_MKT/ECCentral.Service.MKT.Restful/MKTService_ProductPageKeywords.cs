using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private ProductPageKeywordsAppService productPageKeywordsAppService = ObjectFactory<ProductPageKeywordsAppService>.Instance;

        #region 产品页面关键字（ProductPageKeywords）

        /// <summary>
        /// JOB更新产品页面关键字
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/BatchUpdateKeywordsFromQueue", Method = "PUT")]
        public virtual void BatchUpdateKeywordsFromQueue(string companyCode)
        {
            productPageKeywordsAppService.BatchUpdateKeywordsFromQueue(companyCode);
        }

        /// <summary>
        /// JOB更新产品页面关键字
        /// </summary>
        [WebInvoke(UriTemplate = "/Job/BatchUpdateKeywordsForKeywords0", Method = "PUT")]
        public virtual void BatchUpdateKeywordsForKeywords0(string companyCode)
        {
            productPageKeywordsAppService.BatchUpdateKeywordsForKeywords0(companyCode);
        }

        [WebInvoke(UriTemplate = "/Job/BatchUpdateKeywordsFromProduct", Method = "PUT")]
        public virtual void BatchUpdateKeywordsFromProduct(ECCentral.BizEntity.IM.ProductInfo product)
        {
            productPageKeywordsAppService.BatchUpdateKeywordsFromProduct(product);
        }

        /// <summary>
        /// 加载产品页面关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/KeywordsInfo/LoadProductPageKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        //[DataTableSerializeOperationBehavior]
        //public virtual ProductPageKeywords LoadProductPageKeywords(int sysNo)
        //{
        //    return productPageKeywordsAppService.LoadProductPageKeywords(sysNo);
        //}

        /// <summary>
        /// 删除或添加产品页面关键字
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchUpdateProductPageKeywords", Method = "PUT")]
        public virtual void BatchUpdateProductPageKeywords(ECCentral.Service.MKT.Restful.ResponseMsg.ProductPageKeywordsRsp rsp)
        {
            productPageKeywordsAppService.BatchUpdateProductPageKeywords(rsp.ProductList, rsp.BatchAdd, rsp.ReplKeywords0, rsp.ReplKeywords1, rsp.LanguageCode, rsp.CompanyCode);
        } 
        

        /// <summary>
        /// 获取页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetProductKeywordsEditUserList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual List<UserInfo> GetProductKeywordsEditUserList(string companyCode)
        {
            return productPageKeywordsAppService.GetProductKeywordsEditUserList(companyCode);
        }

        /// <summary>
        /// 更新产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/UpdateProductPageKeywords", Method = "PUT")]
        public virtual void UpdateProductPageKeywords(ProductPageKeywords item)
        {
            productPageKeywordsAppService.UpdateProductPageKeywords(item);
        }

        /// <summary>
        /// 上传批量添加产品页面关键字
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchImportProductKeywords", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public virtual void BatchImportProductKeywords(string uploadFileInfo)
        {
            productPageKeywordsAppService.BatchImportProductKeywords(uploadFileInfo);
        }

        /// <summary>
        /// 查询产品页面关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryProductPageKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductPageKeywords(ProductKeywordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryProductPageKeywords(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
         /// <summary>
        /// 得到某类下所有的属性
        /// </summary>
        /// <param name="GetPropertyByCategory3SysNo"></param>
        /// <returns>QueryResult</returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetPropertyByCategory3SysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetPropertyByCategory3SysNo(int Category3SysNo)
        {
            var dataTable = ObjectFactory<IProductPageKeywordsDA>.Instance.GetPropertyByCategory3SysNo(Category3SysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
        /// <summary>
        /// 根据属性得到所有属性值
        /// </summary>
        /// <param name="PropertySysNo"></param>
        /// <returns>QueryResult</returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetPropertyValueByPropertySysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetPropertyValueByPropertySysNo(int PropertySysNo)
        {
            var dataTable = ObjectFactory<IProductPageKeywordsDA>.Instance.GetPropertyValueByPropertySysNo(PropertySysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
        #endregion
    }
}
