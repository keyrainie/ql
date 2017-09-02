//************************************************************************
// 用户名				泰隆优选
// 系统名				商品Domain管理
// 子系统名		        商品Domain管理Restful实现
// 作成者				Hax
// 改版日				2012.6.26
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.Restful.RequestMsg;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/ProductDomain/GetUserListName/{companyCode}/{userSysNoList}", Method = "GET")]
        public string GetUserListName(string companyCode, string userSysNoList)
        {
            return ObjectFactory<IProductDomainQueryDA>.Instance.GetUserNameList(userSysNoList, companyCode);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/QueryProductDomain", Method = "POST")]
        public QueryResult QueryProductDomain(ProductDomainFilter request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            int totalCount;
            var data = ObjectFactory<IProductDomainQueryDA>.Instance.QueryProductDomainList(request, out totalCount);

            return new QueryResult { Data = data, TotalCount = totalCount };
        }

        [WebInvoke(UriTemplate = "/ProductDomain/List/{companyCode}", Method = "GET")]
        public List<ProductDomain> LoadForListing(string companyCode)
        {
            return ObjectFactory<ProductDomainAppService>.Instance.LoadForListing(companyCode);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/LoadDomainCategorys/{productDomainSysNo}", Method = "GET")]
        public virtual List<ProductDepartmentCategory> LoadDomainCategorys(string productDomainSysNo)
        {
            return ObjectFactory<ProductDomainAppService>.Instance.LoadProductDepartmentCategorysByDomainSysNo(int.Parse(productDomainSysNo));
        }

        [WebInvoke(UriTemplate = "/ProductDomain/CreateDomain", Method = "POST")]
        public virtual ProductDomain CreateDomain(ProductDomain entity)
        {
            return ObjectFactory<ProductDomainAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/UpdateDomain", Method = "PUT")]
        public virtual ProductDomain UpdateDomain(ProductDomain entity)
        {
            return ObjectFactory<ProductDomainAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/DeleteDomain", Method = "DELETE")]
        public virtual void DeleteDomain(DeleteProductDomainReq request)
        {
            ObjectFactory<ProductDomainAppService>.Instance.Delete(request.ProductDomainSysNo.Value, request.DepartmentCategorySysNo);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/CreateDepartmentCategory", Method = "POST")]
        public virtual ProductDepartmentCategory CreateDepartmentCategory(ProductDepartmentCategory entity)
        {
            return ObjectFactory<ProductDomainAppService>.Instance.CreateDepartmentCategory(entity);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/UpdateDepartmentCategory", Method = "PUT")]
        public virtual void UpdateDepartmentCategory(ProductDepartmentCategory entity)
        {
            ObjectFactory<ProductDomainAppService>.Instance.UpdateDepartmentCategory(entity);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/DeleteDepartmentCategory", Method = "DELETE")]
        public virtual void DeleteProductDepartmentCategory(int sysNo)
        {
            ObjectFactory<ProductDomainAppService>.Instance.DeleteProductDepartmentCategory(sysNo);
        }

        [WebInvoke(UriTemplate = "/ProductDomain/BatchUpdatePM", Method = "PUT")]
        public virtual void BatchUpdatePM(BatchUpdatePMReq request)
        {
            ObjectFactory<ProductDomainAppService>.Instance.BatchUpdatePM(request.ProductDomainSysNo, request.PMSysNo.Value, request.DepartmentCategoryList);
        }
    }
}
