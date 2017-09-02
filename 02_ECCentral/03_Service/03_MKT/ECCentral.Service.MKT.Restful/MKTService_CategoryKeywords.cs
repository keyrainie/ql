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
        private CategoryKeywordsAppService categoryKeywordsAppService = ObjectFactory<CategoryKeywordsAppService>.Instance;

        #region 分类关键字（CategoryKeywords）
        /// <summary>
        /// 是否存在该类别下的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/CheckCategoryKeywordsC3SysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual bool CheckCategoryKeywordsC3SysNo(CategoryKeywords item)
        {
            return categoryKeywordsAppService.CheckCategoryKeywordsC3SysNo(item);
        }

        /// <summary>
        /// 加载分类关键字信息，包括通用关键字和属性关键字列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/KeywordsInfo/LoadCategoryKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        //[DataTableSerializeOperationBehavior]
        //public virtual CategoryKeywords LoadCategoryKeywords(int sysNo)
        //{
        //    return categoryKeywordsAppService.LoadCategoryKeywords(sysNo);
        //}

        /// <summary>
        /// 查询分类关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryCategoryKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCategoryKeywords(CategoryKeywordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IKeywordQueryDA>.Instance.QueryCategoryKeywords(filter, out totalCount);

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion

        #region 三级类通用关键字（CommonKeyWords）
        /// <summary>
        ///设置通用关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddCommonKeyWords", Method = "POST")]
        public virtual void AddCommonKeyWords(CategoryKeywords item)
        {
            categoryKeywordsAppService.AddCommonKeyWords(item);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/UpdateCommonKeyWords", Method = "PUT")]
        public virtual void UpdateCommonKeyWords(CategoryKeywords item)
        {
            categoryKeywordsAppService.UpdateCommonKeyWords(item);
        }

        #endregion

        #region 三级类属性关键字（PropertyKeywords）

        /// <summary>
        /// 获取三级类别下的属性列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetPropertyByCategory", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<ECCentral.BizEntity.IM.CategoryProperty> GetPropertyByCategory(int sysNo)
        {
            return categoryKeywordsAppService.GetPropertyByCategory(sysNo);
        }

        /// <summary>
        ///设定三级类属性关键字  设置属性关键字，送商品类别属性中选择
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddPropertyKeywords", Method = "POST")]
        public virtual void AddPropertyKeywords(CategoryKeywords item)
        {
            categoryKeywordsAppService.AddPropertyKeywords(item);
        }

        /// <summary>
        ///更新三级类通用关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/UpdatePropertyKeywords", Method = "PUT")]
        public virtual void UpdatePropertyKeywords(CategoryKeywords item)
        {
            categoryKeywordsAppService.UpdatePropertyKeywords(item);
        }

        #endregion

    }
}
