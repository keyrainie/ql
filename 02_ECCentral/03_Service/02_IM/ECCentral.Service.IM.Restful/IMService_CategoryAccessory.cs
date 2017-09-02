//************************************************************************
// 用户名				泰隆优选
// 系统名				分类配件管理
// 子系统名		        分类配件管理Restful实现
// 作成者				Tom
// 改版日				2012.5.21
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
        #region 查询

        /// <summary>
        /// 查询分类属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryAccessory/QueryCategoryAccessory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCategoryAccessories(CategoryAccessoriesQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "CategoryAccessoryCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<ICategoryAccessoriesQueryDA>.Instance.QueryCategoryAccessories(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryAccessory/GetCategoryAccessoryBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryAccessory GetCategoryAccessoryBySysNo(int sysNo)
        {
            var entity = ObjectFactory<CategoryAccessoryAppService>.Instance.GetCategoryAccessoryBySysNo(sysNo);
            return entity;
        }
      
        #endregion

        #region 修改以及添加操作
        /// <summary>
        /// 创建分类属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryAccessory/CreateCategoryAccessory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryAccessory CreatetCategoryAccessory(CategoryAccessory request)
        {
            var entity = ObjectFactory<CategoryAccessoryAppService>.Instance.CreatetCategoryAccessory(request);
            return entity;
        }

        /// <summary>
        /// 修改分类属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryAccessory/UpdateCategoryAccessory", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryAccessory UpdateCategoryAccessory(CategoryAccessory request)
        {
            var entity = ObjectFactory<CategoryAccessoryAppService>.Instance.UpdateCategoryAccessory(request);
            return entity;
        }
        #endregion
    }
}