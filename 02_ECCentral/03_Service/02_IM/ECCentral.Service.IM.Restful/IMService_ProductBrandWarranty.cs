using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Linq.Expressions;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/ProductBrandWarranty/GetProductBrandWarrantyByQuery", Method = "POST")]
        public virtual QueryResult GetProductBrandWarrantyByQuery(ProductBrandWarrantyQueryFilter query)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<IProductBrandWarrantyDA>
                .Instance.GetProductBrandWarrantyByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/ProductBrandWarranty/BrandWarrantyInfoByAddOrUpdate", Method = "PUT")]
        public void BrandWarrantyInfoByAddOrUpdate(ProductBrandWarrantyInfo ProductBrandWarranty)
        {
            ObjectFactory<ProductBrandWarrantyService>.Instance
                .BrandWarrantyInfoByAddOrUpdate(ProductBrandWarranty);          
        }

        [WebInvoke(UriTemplate = "/ProductBrandWarranty/DelBrandWarrantyInfoBySysNos", Method = "DELETE")]
        public void DelBrandWarrantyInfoBySysNos(List<ProductBrandWarrantyInfo> ProductBrandWarrantys)
        {
            ObjectFactory<ProductBrandWarrantyService>.Instance
               .DelBrandWarrantyInfoBySysNos(ProductBrandWarrantys);          
        }

        [WebInvoke(UriTemplate = "/ProductBrandWarranty/UpdateBrandWarrantyInfoBySysNo", Method = "PUT")]
        public void UpdateBrandWarrantyInfoBySysNo(ProductBrandWarrantyInfo ProductBrandWarranty)
        {
            ObjectFactory<ProductBrandWarrantyService>.Instance
            .UpdateBrandWarrantyInfoBySysNo(ProductBrandWarranty);        
        }
    }
}
