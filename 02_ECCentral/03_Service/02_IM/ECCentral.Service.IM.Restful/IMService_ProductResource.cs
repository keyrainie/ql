//************************************************************************
// 用户名				泰隆优选
// 系统名				商品资源图片管理
// 子系统名		        商品资源图片Restful实现
// 作成者				Tom
// 改版日				2012.6.02
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.Resource;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductResource/DeleteProductResource", Method = "DELETE")]
        public void DeleteProductResource(ProductResourceRequestMsg request)
        {
            ObjectFactory<ProductResourceAppService>.Instance.DeleteProductResource(request.ProductResources);
        }

        /// <summary>
        /// 修改商品图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductResource/ModifyProductResources", Method = "PUT")]
        public void ModifyProductResources(ProductResourceRequestMsg request)
        {
            ObjectFactory<ProductResourceAppService>.Instance.ModifyProductResources(request.ProductResources);
        }

        /// <summary>
        /// 创建商品图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductResource/CreateProductResource", Method = "POST")]
        public IList<ProductResourceForNewegg> CreateProductResource(ProductResourceRequestMsg request)
        {
            return ObjectFactory<ProductResourceAppService>.Instance.CreateProductResource(request.ProductResources);
        }

        /// <summary>
        /// 查询资源文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductResource/QueryResourceList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryResourceList(ResourceQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "ProductGroupSysNoIsNull"));
            }
            int totalCount;
            var dataTable = ObjectFactory<IResourceQueryDA>.Instance.QueryResourceList(request, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = dataTable != null && dataTable.Rows != null ? dataTable.Rows.Count : 0
            };
        }

        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductResource/IsExistFileName", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<string> IsExistFileName(List<string> fileName)
        {
            return ObjectFactory<ProductResourceAppService>.Instance.IsExistFileName(fileName);
        }
    }
}
