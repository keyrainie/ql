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
        /// 查询三级分类指标
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryKPI/QueryCategoryKPIList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCategoryKPIList(CategoryKPIQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryCondtionInvalid"));
            }
            int totalCount;
            var data = ObjectFactory<ICategoryKPIQueryDA>.Instance.QueryCategoryKPIList(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryKPI/GetCategorySettingBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategorySetting GetCategorySettingBySysNo(int Sysno)
        {
            var result = ObjectFactory<CategorySettingAppService>.Instance.GetCategorySettingBySysNo(Sysno);
            return result;
        }
        #endregion

        #region 添加操作
        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategoryBasic", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryBasic UpdateCategoryBasic(CategoryBasic categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingAppService>.Instance.UpdateCategoryBasic(categoryBasicInfo);
            return result;
        }


        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategoryRMA", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryRMA UpdateCategoryRMA(CategoryRMA categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingAppService>.Instance.UpdateCategoryRMA(categoryBasicInfo);
            return result;
        }

        /// <summary>
        /// 保存RMA指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategoryMinMargin", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public CategoryMinMargin UpdateCategoryMinMargin(CategoryMinMargin categoryBasicInfo)
        {
            var result = ObjectFactory<CategorySettingAppService>.Instance.UpdateCategoryMinMargin(categoryBasicInfo);
            return result;
        }

        /// <summary>
        /// 更新最低限额
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategoryProductMinCommission", Method = "PUT")]      
        public void UpdateCategoryProductMinCommission(CategoryBasic categoryBasicInfo)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategoryProductMinCommission(categoryBasicInfo);
        }

        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategory2ProductMinCommission", Method = "PUT")]      
        public void UpdateCategory2ProductMinCommission(List<CategoryBasic> categoryBasicList)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategory2ProductMinCommission(categoryBasicList);
        }

        [WebInvoke(UriTemplate = "/CategoryKPI/GetCategorySettingByCategory2SysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public CategorySetting GetCategorySettingByCategory2SysNo(int SysNo)
        {
            return ObjectFactory<CategorySettingAppService>.Instance.GetCategorySettingByCategory2SysNo(SysNo);
        }
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategory2Basic", Method = "PUT")]       
        public void UpdateCategory2Basic(CategoryBasic categoryBasicInfo)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategory2Basic(categoryBasicInfo);
        }

        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategory2MinMargin", Method = "PUT")]       
        public void UpdateCategory2MinMargin(CategoryMinMargin categoryBasicInfo)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategory2MinMargin(categoryBasicInfo);
        }
        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategory3MinMarginBat", Method = "PUT")]     
        public void UpdateCategory3MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategory3MinMarginBat(categoryMinMarginList);
        }

        [WebInvoke(UriTemplate = "/CategoryKPI/UpdateCategory2MinMarginBat", Method = "PUT")]      
        public void UpdateCategory2MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            ObjectFactory<CategorySettingAppService>.Instance.UpdateCategory2MinMarginBat(categoryMinMarginList);
        }
        #endregion

    }
}
