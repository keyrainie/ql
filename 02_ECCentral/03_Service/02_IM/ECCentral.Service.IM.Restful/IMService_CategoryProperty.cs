//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理Restful实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
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
        [WebInvoke(UriTemplate = "/CategoryProperty/QueryCategoryProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCategoryProperty(CategoryPropertyQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategoryPropertyCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<ICategoryPropertyQueryDA>.Instance.QueryCategoryProperty(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryProperty/GetCategoryPropertyBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryProperty GetCategoryPropertyBySysNo(int sysNo)
        {
            var entity = ObjectFactory<CategoryPropertyAppService>.Instance.GetCategoryPropertyBySysNo(sysNo);
            return entity;
        }

        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryProperty/GetCategoryPropertyByCategorySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public IList<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            var entity = ObjectFactory<CategoryPropertyAppService>.Instance.GetCategoryPropertyByCategorySysNo(categorySysNo);
            return entity;
        }
        #endregion

        #region  修改、添加以及删除操作
        /// <summary>
        /// 根据SysNO删除分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryProperty/DelCategoryPropertyBySysNo", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DelCategoryPropertyBySysNo(IList<int> sysNo)
        {
            ObjectFactory<CategoryPropertyAppService>.Instance.DelCategoryPropertyBySysNo(sysNo);
        }

        /// <summary>
        /// 创建分类属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryProperty/CreateCategoryProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategoryProperty CreateCategoryProperty(CategoryProperty request)
        {
            var entity = ObjectFactory<CategoryPropertyAppService>.Instance.CreateCategoryProperty(request);
            return entity;
        }

        /// <summary>
        /// 修改分类属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryProperty/UpdateCategoryProperty", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryProperty UpdateCategoryProperty(CategoryProperty request)
        {
            var entity = ObjectFactory<CategoryPropertyAppService>.Instance.UpdateCategoryProperty(request);
            return entity;
        }

        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="property"></param>
        [WebInvoke(UriTemplate = "/CategoryProperty/CopyCategoryOutputTemplateProperty", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CopyCategoryOutputTemplateProperty(CategoryProperty property)
        {
            ObjectFactory<CategoryPropertyAppService>.Instance.CopyCategoryOutputTemplateProperty(property);
        }
        /// <summary>
        /// 批量修改属性
        /// </summary>
        /// <param name="listCategoryProperty"></param>
        [WebInvoke(UriTemplate = "/CategoryProperty/UpdateCategoryPropertyByList", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateCategoryPropertyByList(List<CategoryProperty> listCategoryProperty)
        {
            ObjectFactory<CategoryPropertyAppService>.Instance.UpdateCategoryPropertyByList(listCategoryProperty);
        }
        #endregion
    }
}