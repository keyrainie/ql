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

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private ProductKeywordsInfoAppService productKeywordsInfoAppService = ObjectFactory<ProductKeywordsInfoAppService>.Instance;


        #region 关键字对应商品（ProductKeywordsInfo）
        /// <summary>
        /// 添加自关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddProductKeywords", Method = "POST")]
        public virtual void AddProductKeywords(ProductKeywordsInfo item)
        {
            productKeywordsInfoAppService.AddProductKeywords(item);
        }

        /// <summary>
        /// 编辑关键字对应商品
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/EditProductKeywords", Method = "PUT")]
        public virtual void EditProductKeywords(ProductKeywordsInfo item)
        {
            productKeywordsInfoAppService.EditProductKeywords(item);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="item"></param>
         [WebInvoke(UriTemplate = "/KeywordsInfo/ChangeProductKeywordsStatus", Method = "PUT")]
        public virtual void ChangeProductKeywordsStatus(List<ProductKeywordsInfo> list)
        {
            productKeywordsInfoAppService.ChangeProductKeywordsStatus(list);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
         [WebInvoke(UriTemplate = "/KeywordsInfo/DeleteProductKeywords", Method = "DELETE")]
        public virtual void DeleteProductKeywords(List<int> list)
        {
            productKeywordsInfoAppService.DeleteProductKeywords(list);
        }


        /// <summary>
        /// 查询关键字对应商品
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryKeyWordsForProduct", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryKeyWordsForProduct(KeyWordsForProductQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryKeyWordsForProduct(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}
