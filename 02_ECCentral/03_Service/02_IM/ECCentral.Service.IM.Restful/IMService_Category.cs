using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/Category/QueryCategory1", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryCategory1(queryFilter);
        }

        [WebInvoke(UriTemplate = "/Category/QueryAllCategory2", Method = "POST", ResponseFormat = WebMessageFormat.Json)
        ]
        public List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryAllCategory2(queryFilter);
        }

        [WebInvoke(UriTemplate = "/Category/QueryAllCategory3", Method = "POST", ResponseFormat = WebMessageFormat.Json)
        ]
        public List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryAllCategory3(queryFilter);
        }

        [WebInvoke(UriTemplate = "/Category/QueryCategory2", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CategoryInfo> QueryCategory2(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryCategory2(queryFilter);

        }

        [WebInvoke(UriTemplate = "/Category/QueryCategory3", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CategoryInfo> QueryCategory3(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryCategory3(queryFilter);

        }

        [WebInvoke(UriTemplate = "/Category/QueryCategory", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryCategory(CategoryQueryFilter queryFilter)
        {
            if (queryFilter == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerCondtionIsNull"));
            }
            int totalCount;
            var dataTable = ObjectFactory<ICategoryQueryDA>.Instance.QueryCategory(queryFilter, out totalCount);
            return new QueryResult()
                       {
                           Data = dataTable,
                           TotalCount = totalCount
                       };

        }

        [WebInvoke(UriTemplate = "/Category/QueryCategory2Info/{c2SysNo}", Method = "GET")]
        public CategoryInfo QueryCategory2Info(string c2SysNo)
        {
            return ObjectFactory<CategoryAppService>.Instance.GetCategory2BySysNo(Convert.ToInt32(c2SysNo));
        }


        [WebInvoke(UriTemplate = "/Category/QueryCategory3Info/{c3SysNo}", Method = "GET")]
        public CategoryInfo QueryCategory3Info(string c3SysNo)
        {
            return ObjectFactory<CategoryAppService>.Instance.GetCategory3BySysNo(Convert.ToInt32(c3SysNo));
        }

        /// <summary>
        /// 查询类别数据
        /// 本方法不区分1，2，3类别
        /// 目前供IM类别控件使用
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Category/QueryAllPrimaryCategory", Method = "POST",
            ResponseFormat = WebMessageFormat.Json)]
        public List<CategoryInfo> QueryAllPrimaryCategory(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<ICategoryQueryDA>.Instance.QueryAllPrimaryCategory(queryFilter);
        }

        /// <summary>
        /// 创建类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Category/CreateCategory", Method = "POST")]
        public CategoryInfo CreateCategory(CategoryInfo request)
        {
            return ObjectFactory<CategoryAppService>.Instance.AddCategory(request);
        }

        /// <summary>
        /// 修改类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Category/UpdateCategory", Method = "PUT")]
        public void UpdateCategory(CategoryRequestApprovalInfo request)
        {
            ObjectFactory<CategoryAppService>.Instance.UpdateCategory(request);
        }

        /// <summary>
        /// 根据sysno获取分类信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Category/GetCategoryBySysNo", Method = "POST")]
        public CategoryInfo GetCategoryBySysNo(int categorySysNo)
        {
            return ObjectFactory<CategoryAppService>.Instance.GetCategoryBySysNo(categorySysNo);
        }

        /// <summary>
        /// 根据query得到不同类别信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Category/GetCategoryListByType", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetCategoryListByType(CategoryQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICategoryDA>.Instance.GetCategoryListByType(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Category/CreateCategoryRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void CreateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalAppService>.Instance.CreateCategoryRequest(info);
        }

        [WebInvoke(UriTemplate = "/Category/UpdateCategoryRequest", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public virtual void UpdateCategoryRequest(CategoryRequestApprovalInfo info)
        {
            ObjectFactory<CategoryRequestApprovalAppService>.Instance.UpdateCategoryRequest(info);
        }

        [WebInvoke(UriTemplate = "/Category/GetProductC1CategoryDomain/{productID}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public CategoryInfo GetProductC1CategoryDomain(string productID)
        {
            return ObjectFactory<CategoryAppService>.Instance.GetC1CategoryInfoByProductID(productID);
        }
        
    }
}