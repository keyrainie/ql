//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理Restful实现
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************

using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 查询

        /// <summary>
        /// 查询商家商品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/QuerySellerProductRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySellerProductRequest(SellerProductRequestQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.SellerProductRequest", "SellerProductRequestCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<ISellerProductRequestQueryDA>.Instance.QuerySellerProductRequest(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取商家商品需求信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/GetSellerProductRequestInfoBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo GetSellerProductRequestInfoBySysNo(int sysNo)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.GetSellerProductRequestInfoBySysNo(sysNo);
            return entity;
        }

        /// <summary>
        /// 根据ProductID获取商家商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/GetSellerProductInfoByProductID", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo GetSellerProductInfoByProductID(string productID)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.GetSellerProductInfoByProductID(productID);
            return entity;
        }


        /// <summary>
        /// 审核通过商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/ApproveProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo ApproveProductRequest(SellerProductRequestInfo request)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.ApproveProductRequest(request);
            return entity;
        }

        /// <summary>
        /// 退回商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/DenyProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo DenyProductRequest(SellerProductRequestInfo request)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.DenyProductRequest(request);
            return entity;
        }

        /// <summary>
        /// 更新商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/UpdateProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo UpdateProductRequest(SellerProductRequestInfo request)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.UpdateProductRequest(request);
            return entity;
        }

        /// <summary>
        /// 创建ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/CreateItemIDForNewProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public SellerProductRequestInfo CreateItemIDForNewProductRequest(SellerProductRequestInfo request)
        {
            var entity = ObjectFactory<SellerProductRequestAppService>.Instance.CreateItemIDForNewProductRequest(request);
            return entity;
        }

        /// <summary>
        /// 批量审核通过商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/BatchApproveProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchApproveProductRequest(List<SellerProductRequestInfo> request)
        {
            ObjectFactory<SellerProductRequestAppService>.Instance.BatchApproveProductRequest(request);
        }

        /// <summary>
        /// 批量退回商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/BatchDenyProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchDenyProductRequest(List<SellerProductRequestInfo> request)
        {
            ObjectFactory<SellerProductRequestAppService>.Instance.BatchDenyProductRequest(request);
        }

        /// <summary>
        /// 批量创建ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SellerProductRequest/BatchCreateItemIDForNewProductRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchCreateItemIDForNewProductRequest(List<SellerProductRequestInfo> request)
        {
            ObjectFactory<SellerProductRequestAppService>.Instance.BatchCreateItemIDForNewProductRequest(request);
        }

        #endregion
    }
}