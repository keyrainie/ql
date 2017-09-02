//************************************************************************
// 用户名				泰隆优选
// 系统名				分类延保管理
// 子系统名		        分类延保管理Restful实现
// 作成者				Kevin
// 改版日				2012.6.4
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

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 分类延保

        /// <summary>
        /// 查询分类延保
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/QueryCategoryExtendWarranty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCategoryExtendWarranty(CategoryExtendWarrantyQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<ICategoryExtendWarrantyQueryDA>.Instance.QueryCategoryExtendWarranty(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取分类延保信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/GetCategoryExtendWarrantyBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarranty GetCategoryExtendWarrantyBySysNo(int sysNo)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.GetCategoryExtendWarrantyBySysNo(sysNo);
            return entity;
        }


        /// <summary>
        /// 创建分类延保
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/CreateCategoryExtendWarranty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarranty CreatetCategoryExtendWarranty(CategoryExtendWarranty request)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.CreatetCategoryExtendWarranty(request);
            return entity;
        }

        /// <summary>
        /// 修改分类延保
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/UpdateCategoryExtendWarranty", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarranty UpdateCategoryExtendWarranty(CategoryExtendWarranty request)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.UpdateCategoryExtendWarranty(request);
            return entity;
        }
        #endregion

        #region 分类延保排除品牌

        /// <summary>
        /// 查询分类延保排除品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/QueryCategoryExtendWarrantyDisuseBrand", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrandQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<ICategoryExtendWarrantyQueryDA>.Instance.QueryCategoryExtendWarrantyDisuseBrand(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取分类延保排除品牌信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/GetCategoryExtendWarrantyDisuseBrandBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarrantyDisuseBrand GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.GetCategoryExtendWarrantyDisuseBrandBySysNo(sysNo);
            return entity;
        }


        /// <summary>
        /// 创建分类延保排除品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/CreateCategoryExtendWarrantyDisuseBrand", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarrantyDisuseBrand CreatetCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand request)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.CreatetCategoryExtendWarrantyDisuseBrand(request);
            return entity;
        }

        /// <summary>
        /// 修改分类延保排除品牌
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryExtendWarranty/UpdateCategoryExtendWarrantyDisuseBrand", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryExtendWarrantyDisuseBrand UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand request)
        {
            var entity = ObjectFactory<CategoryExtendWarrantyAppService>.Instance.UpdateCategoryExtendWarrantyDisuseBrand(request);
            return entity;
        }
        #endregion
    }
}