using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 根据query得到产品线信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns>QueryResult</returns>
        [WebInvoke(UriTemplate = "/ProductLine/GetProductLineByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductLineByQuery(ProductLineQueryFilter query)
        {
             int totalCount;
            var dataTable = ObjectFactory<IProductLineDA>.Instance.GetProductLineByQuery(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 得到产品线分类
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductLine/GetAllProductLineCategory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAllProductLineCategory()
        {
            var dataTable = ObjectFactory<IProductLineDA>.Instance.GetAllProductLineCategory();
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }

        /// <summary>
        /// 创建产品线信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductLine/CreateProductLine", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateProductLine(ProductLineInfo info)
        {
            ObjectFactory<ProductLineAppService>.Instance.CreateProductLine(info);
        }
        /// <summary>
        /// 更新产品线信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductLine/UpdateProductLine", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateProductLine(ProductLineInfo info)
        {
            ObjectFactory<ProductLineAppService>.Instance.UpdateProductLine(info);
        }
        /// <summary>
        /// 删除产品线信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/ProductLine/DeleteProductLine", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteProductLine(int SysNo)
        {
            ObjectFactory<ProductLineAppService>.Instance.DeleteProductLine(SysNo);
        }
        /// <summary>
        /// 根据产品线类别SysNo得到产品线DataTable
        /// </summary>
        /// <param name="CategorySysNo"></param>
        /// <returns>QueryResult DataTable cloums(ProductLineSysNo,ProductLineName)</returns>
        [WebInvoke(UriTemplate = "/ProductLine/GetProductLineByProductLineCategorySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductLineByProductLineCategorySysNo(int CategorySysNo)
        {
            var dataTable = ObjectFactory<IProductLineDA>.Instance.GetProductLineByProductLineCategorySysNo(CategorySysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
    }
}
