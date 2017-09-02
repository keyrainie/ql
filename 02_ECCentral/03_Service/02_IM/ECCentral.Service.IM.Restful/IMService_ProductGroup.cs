using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 根据商品SysNo获取该商品所属组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetProductGroup", Method = "POST")]
        public ProductGroup GetProductGroup(string productSysNo)
        {
            int sysNo;
            if (Int32.TryParse(productSysNo, out sysNo))
            {
                return ObjectFactory<ProductGroupAppService>.Instance.GetProductGroup(sysNo);
            }
            throw new ArgumentException("GetProductGroup Arg Error");
        }

        /// <summary>
        /// 获取商品组信息
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductGroup/GetProductGroupInfoBySysNo", Method = "POST")]
        public ProductGroup GetProductGroupInfoBySysNo(int productGroupSysNo)
        {
            return ObjectFactory<ProductGroupAppService>.Instance.GetProductGroupInfoBySysNo(productGroupSysNo);
        }

        /// <summary>
        /// 商品组查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductGroup/QueryProductGroupInfo", Method = "POST")]
        public QueryResult QueryProductGroupInfo(ProductGroupQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductGroupQueryDA>.Instance.QueryProductGroupInfo(request, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建商品组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductGroup/CreateProductGroupInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductGroup CreateProductGroupInfo(ProductGroup request)
        {
            var entity = ObjectFactory<ProductGroupAppService>.Instance.CreateProductGroupInfo(request);
            return entity;
        }

        /// <summary>
        /// 编辑商品组信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductGroup/UpdateProductGroupInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ProductGroup UpdateProductGroupInfo(ProductGroup request)
        {
            var entity = ObjectFactory<ProductGroupAppService>.Instance.UpdateProductGroupInfo(request);
            return entity;
        }
    }
}
