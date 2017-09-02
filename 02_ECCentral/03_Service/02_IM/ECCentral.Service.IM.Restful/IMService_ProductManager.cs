//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员管理
// 子系统名		        商品管理员管理Restful实现
// 作成者				Tom
// 改版日				2012.4.24
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
using ECCentral.BizEntity.Common;
using System;

namespace ECCentral.Service.IM.Restful
{

    public partial class IMService
    {
        #region 公共方法
        /// <summary>
        /// 查询PM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManager/QueryProductManagerInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductManagerInfo(ProductManagerQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductManager", "ProductManagerCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductManagerQueryDA>.Instance.QueryProductManagerInfo(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取PM信息
        /// </summary>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManager/Load/{sysNo}", Method = "GET")]
        //[DataTableSerializeOperationBehavior]
        public ProductManagerInfo GetProductManagerInfoBySysNo(string sysNo)
        {
            int productManagerInfoSysNo = int.Parse(sysNo);
            return ObjectFactory<ProductManagerAppService>.Instance.GetProductManagerInfoBySysNo(productManagerInfoSysNo);
        }

        /// <summary>
        /// 创建PM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManager/CreateProductManagerInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductManagerInfo CreateProductManagerInfo(ProductManagerInfo request)
        {
            var entity = ObjectFactory<ProductManagerAppService>.Instance.CreateProductManagerInfo(request);
            return entity;
        }

        /// <summary>
        /// 修改PM
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManager/UpdateProductManagerInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ProductManagerInfo UpdateProductManagerInfo(ProductManagerInfo request)
        {
            var entity = ObjectFactory<ProductManagerAppService>.Instance.UpdateProductManagerInfo(request);
            return entity;
        }
        #endregion

        #region [PM 控件用的查询方法]
        /// <summary>
        /// 获取 PM LIST
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PM/QueryPMList", Method = "POST")]
        public List<ProductManagerInfo> QueryPMList(ProductManagerQueryFilter query)
        {
            List<ProductManagerInfo> returnList = new List<ProductManagerInfo>();
            returnList = ObjectFactory<ProductManagerAppService>.Instance.GetPMList((PMQueryType)Enum.Parse(typeof(PMQueryType), query.PMQueryType), query.UserName, query.CompanyCode);
            return returnList;
        }
        #endregion

        /// <summary>
        /// 获取 PM Leader LIST
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PM/QueryPMLeaderList/{companyCode}", Method = "GET")]
        public List<ProductManagerInfo> QueryPMLeaderList(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                throw new ArgumentNullException("companyCode");
            }
            return ObjectFactory<ProductManagerAppService>.Instance.GetPMLeaderList(companyCode);            
        }
    }
}
