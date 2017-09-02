using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {        
        private ECDynamicCategoryAppService appService = ObjectFactory<ECDynamicCategoryAppService>.Instance;       

        [WebInvoke(UriTemplate = "/ECDynamicCategory/Create", Method = "POST")]
        public ECDynamicCategory CreateDynamicCategory(ECDynamicCategory entity)
        {
            return appService.Create(entity);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/Update", Method = "PUT")]
        public void UpdateDynamicCategory(ECDynamicCategory entity)
        {
            appService.Update(entity);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/Delete", Method = "DELETE")]
        public void DeleteDynamicCategory(int sysNo)
        {
            appService.Delete(sysNo);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/InsertCategoryProductMapping", Method = "POST")]
        public void InsertDynamicCategoryProductMapping(ECDynamicCategoryMappingReq req)
        {
            appService.InsertCategoryProductMapping(req.DynamicCategorySysNo.Value, req.ProductSysNoList);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/DeleteCategoryProductMapping", Method = "DELETE")]
        public void DeleteDynamicCategoryProductMapping(ECDynamicCategoryMappingReq req)
        {
            appService.DeleteCategoryProductMapping(req.DynamicCategorySysNo.Value, req.ProductSysNoList);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/GetCategoryTree", Method = "POST")]
        public ECDynamicCategory QueryDynamicCategoryTree(ECDynamicCategoryQueryFilter filter)
        {
            return appService.GetCategoryTree(filter.Status, filter.CategoryType);
        }

        [WebInvoke(UriTemplate = "/ECDynamicCategory/QueryMapping", Method = "POST")]
        public QueryResult QueryDynamicCategoryMapping(ECDynamicCategoryMappingQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IECDynamicCategoryQueryDA>.Instance.QueryECDynamicCategoryMapping(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
    }
}
