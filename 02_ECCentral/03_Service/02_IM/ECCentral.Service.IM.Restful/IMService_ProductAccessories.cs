using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region "配件查询操作"
        
      
        /// <summary>
        /// 根据query得到配件查询信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/GetProductAccessoriesByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductAccessoriesByQuery(ProductAccessoriesQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetProductAccessoriesByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建查询功能信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/CreateAccessoriesQueryMaster", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.CreateAccessoriesQueryMaster(info);
        }

        /// <summary>
        /// 更新查询功能信息
        /// </summary>
        /// <param name="info"></param>Entity
        [WebInvoke(UriTemplate = "/ProductAccessories/UpdateAccessoriesQueryMaster", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.UpdateAccessoriesQueryMaster(info);
        }
        #endregion
        #region "查询条件操作"
        
        #endregion
        /// <summary>
        /// 根据配件查询的SysNo获取查询条件信息 
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/GetAccessoriesQueryConditionBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAccessoriesQueryConditionBySysNo(int SysNo)
        {
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetAccessoriesQueryConditionBySysNo(SysNo);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = 0
            };
        }

        /// <summary>
        /// 创建查询条件
        /// </summary>
        /// <param name="Info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/CreateAccessoriesQueryCondition", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.CreateAccessoriesQueryCondition(Info);
        }
         /// <summary>
        /// 修改查询条件
        /// </summary>
        /// <param name="Info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/UpdateAccessoriesQueryCondition", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.UpdateAccessoriesQueryCondition(Info);
        }
        /// <summary>
        /// 删除查询条件
        /// </summary>
        /// <param name="SysNo"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/DeleteAccessoriesQueryCondition", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteAccessoriesQueryCondition(int SysNo)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.DeleteAccessoriesQueryCondition(SysNo);
        }
        #region "条件选项值操作"

        /// <summary>
        /// 根据query获取选项值
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/GetProductAccessoriesConditionValueByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductAccessoriesConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetProductAccessoriesConditionValueByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 得到某个条件的父节点的所有选项值
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/GetProductAccessoriesConditionValueByCondition", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductAccessoriesConditionValueByCondition(ProductAccessoriesConditionValueQueryFilter query)
        {
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetProductAccessoriesConditionValueByConditionSysNo(query);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="Info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/CreateProductAccessoriesQueryConditionValue", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.CreateProductAccessoriesQueryConditionValue(Info);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/UpdateProductAccessoriesQueryConditionValue", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.UpdateProductAccessoriesQueryConditionValue(Info);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/DeleteProductAccessoriesQueryConditionValue", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteProductAccessoriesQueryConditionValue(int SysNo)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.DeleteProductAccessoriesQueryConditionValue(SysNo);
        }


        #endregion


        #region "查询效果操作"
        /// <summary>
        /// 得到某条件的所有选项值
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/GetConditionValueByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetConditionValueByQuery(ProductAccessoriesConditionValueQueryFilter query)
        {
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetConditionValueByQuery(query);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
         /// <summary>
        /// 得到商品和条件选项值bing的信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCoutn"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAccessories/QueryAccessoriesQueryConditionBind", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.QueryAccessoriesQueryConditionBind(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 删除bing
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductAccessories/DeleteAccessoriesQueryConditionBind", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteAccessoriesQueryConditionBind(List<ProductAccessoriesQueryConditionPreViewInfo> info)
        {
            ObjectFactory<ProductAccessoriesAppService>.Instance.DeleteAccessoriesQueryConditionBind(info);
        }
        #endregion
        #region "导出"

        [WebInvoke(UriTemplate = "/ProductAccessories/GetAccessoriesQueryExcelOutput", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAccessoriesQueryExcelOutput(ProductAccessoriesConditionValueQueryFilter query)
        {
            var dataTable = ObjectFactory<IProductAccessoriesDA>.Instance.GetAccessoriesQueryExcelOutput(query);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = 1
            };
        }
        #endregion
    }
}
