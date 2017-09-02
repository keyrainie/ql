using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.BizEntity.Invoice.ReconReport;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {

        /// <summary>
        /// 查询上传SAP数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/HeaderData/QuerySAPDOCHeader", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySAPDOCHeader(HeaderDataQueryFilter filter)
        {
            int totalCount = 0;
            return new QueryResult()
            {
                Data = ObjectFactory<IHeaderDataQueryDA>.Instance.QuerySAPDOCHeader(filter, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询上传SAP数据明细
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/HeaderData/QuerySAPDOCHeaderDetails", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySAPDOCHeaderDetail(HeaderDataQueryFilter filter)
        {
            int totalCount = 0;
            return new QueryResult()
            {
                Data = ObjectFactory<IHeaderDataQueryDA>.Instance.QuerySAPDOCHeaderDetail(filter, out totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取公司代码(SAP)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/HeaderData/GetCompanyCode", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCompanyCode()
        {
            return new QueryResult()
            {
                Data = ObjectFactory<IHeaderDataQueryDA>.Instance.QueryCompany()
            };
        }

        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransNumbers"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/HeaderData/UpdateSAPStatus", Method = "PUT")]
        public int UpdateSAPStatus(List<int> TransNumbers)
        {
            return ObjectFactory<ReconReportAppService>.Instance.UpdateSAPStatus(TransNumbers);
        }

        //Job自动生成报表
        [WebInvoke(UriTemplate = "/ReconReport/CreateReconReportByJob", Method = "POST")]
        public void CreateReconReportByJob(GenerateReportInfo entity)
        {
            ObjectFactory<ReconReportAppService>.Instance.CreateReconReportByJob(entity.ReportFrom, entity.ReportTo);
        }

        //手动生成报表
        [WebInvoke(UriTemplate = "/ReconReport/CreateReconReportByWeb", Method = "POST")]
        public void CreateReconReportByWeb(GenerateReportInfo entity)
        {
            ObjectFactory<ReconReportAppService>.Instance.CreateReconReportByWeb(entity.ReportFrom, entity.ReportTo);
        }
    }
}
