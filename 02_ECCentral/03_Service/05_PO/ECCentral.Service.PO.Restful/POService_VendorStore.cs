using System.Data;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.BizEntity.PO.Vendor;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        [WebInvoke(UriTemplate = "/VendorStore/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public VendorStoreInfo CreateVendorStore(VendorStoreInfo entity)
        {
            return ObjectFactory<VendorStoreAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/VendorStore/Update", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public VendorStoreInfo UpdateVendorStore(VendorStoreInfo entity)
        {
            return ObjectFactory<VendorStoreAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/VendorStore/UpdateCommissionRuleTemplate", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void UpdateCommissionRuleTemplate(CommissionRuleTemplateInfo info)
        {
            ObjectFactory<VendorStoreAppService>.Instance.UpdateCommissionRuleTemplate(info);
        }

        [WebInvoke(UriTemplate = "/VendorStore/GetCommissionRuleTemplateInfo/{sysno}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public CommissionRuleTemplateInfo GetCommissionRuleTemplateInfo(string sysno)
        {
            return ObjectFactory<VendorStoreAppService>.Instance.GetCommissionRuleTemplateInfo(int.Parse(sysno));
        }



        [WebInvoke(UriTemplate = "/VendorStore/QueryCommissionRuleTemplateInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCommissionRuleTemplateInfo(CommissionRuleTemplateQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IVendorStoreQueryDA>.Instance.QueryCommissionRuleTemplateInfo(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/VendorStore/BatchSetCommissionRuleActive", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void BatchSetCommissionRuleActive(string sysnos)
        {
            ObjectFactory<VendorStoreAppService>.Instance.BatchSetCommissionRuleStatus(sysnos, CommissionRuleStatus.Active);
        }

        [WebInvoke(UriTemplate = "/VendorStore/BatchSetCommissionRuleDEActive", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public void BatchSetCommissionRuleDEActive(string sysnos)
        {
            ObjectFactory<VendorStoreAppService>.Instance.BatchSetCommissionRuleStatus(sysnos, CommissionRuleStatus.Deactive);
        }

        [WebInvoke(UriTemplate = "/VendorStore/ExportExcelForVendorBrandFiling/{vendorId}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult ExportExcelForVendorBrandFiling(string vendorId)
        {
            var brands = ObjectFactory<VendorStoreAppService>.Instance.GetVendorBrandFilingList(Convert.ToInt32(vendorId)).Where(data=>data.RequestType != VendorModifyRequestStatus.Apply);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ManufacturerName");
            dataTable.Columns.Add("BrandName");
            dataTable.Columns.Add("BrandInspectionNo");

            foreach (var brand in brands)
            {
                //代理厂商
                var newRow = dataTable.NewRow();
                newRow["ManufacturerName"] = brand.ManufacturerInfo.ManufacturerNameLocal.Content;
                newRow["BrandName"] = brand.BrandInfo.BrandNameLocal.Content;
                newRow["BrandInspectionNo"] = brand.StoreBrandFilingInfo.InspectionNo;
                dataTable.Rows.Add(newRow);
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = dataTable.Rows.Count
            };
        }

        /// <summary>
        /// 查询商家二级域名
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VendorStore/QuerySecondDomain", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySecondDomain(SecondDomainQueryFilter queryFilter)
        {
            int totalCount;
            DataTable table = ObjectFactory<IVendorStoreQueryDA>.Instance.QuerySecondDomain(queryFilter, out totalCount);

            QueryResult result = new QueryResult()
            {
                Data = table,
                TotalCount = totalCount
            };

            return result;
        }

        [WebInvoke(UriTemplate = "/VendorStore/SecondDomainAuditThrough", Method = "PUT")]
        public string SecondDomainAuditThrough(int SysNo)
        {
            ObjectFactory<VendorStoreAppService>.Instance.SecondDomainAuditThrough(SysNo);
            return "操作成功！";
        }

        [WebInvoke(UriTemplate = "/VendorStore/SecondDomainAuditThroughNot", Method = "PUT")]
        public string SecondDomainAuditThroughNot(int SysNo)
        {
            ObjectFactory<VendorStoreAppService>.Instance.SecondDomainAuditThroughNot(SysNo);
            return "操作成功！";
        }

    }
}
