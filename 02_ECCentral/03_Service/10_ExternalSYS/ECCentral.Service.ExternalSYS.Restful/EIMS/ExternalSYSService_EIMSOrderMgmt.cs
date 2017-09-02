using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService.EIMS;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// EIMS结算类型变更单据查询
        /// </summary>
        /// <param name="filter">条件集合</param>
        /// <returns>结果集合</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryEIMSEventMemo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryEIMSEventMemo(EIMSEventMemoQueryFilter filter)
        {           
            return QueryList<EIMSEventMemoQueryFilter>(filter, ObjectFactory<IEIMSEventMemoQueryFilterDA>.Instance.EIMSEventMemoQuery);
        }

        /// <summary>
        /// EIMS发票信息查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryInvoiceInfoList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult InvoiceInfoListQuery(EIMSInvoiceEntryQueryFilter filter)
        {
            return QueryList<EIMSInvoiceEntryQueryFilter>(filter, ObjectFactory<IEIMSEventMemoQueryFilterDA>.Instance.InvoiceInfoListQuery);
        }

        /// <summary>
        /// 根据单据号查询发票信息
        /// </summary>
        /// <param name="invoiceNo">单据号</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryInvoiceList/{invoiceNumber}", Method = "GET")]
        public EIMSInvoiceInfo QueryInvoiceList(string invoiceNumber)
        {
            return ObjectFactory<IEIMSEventMemoQueryFilterDA>.Instance.QueryInvoiceList(invoiceNumber);
        }

        /// <summary>
        /// 录入发票信息
        /// </summary>
        /// <param name="entities"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/CreateEIMSInvoiceInput", Method = "POST")]
        public EIMSInvoiceResult CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            return ObjectFactory<EIMSEventMemoAppService>.Instance.CreateEIMSInvoiceInput(entities);
        }

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="entities"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateEIMSInvoiceInput", Method = "PUT")]
        public EIMSInvoiceResult UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            return ObjectFactory<EIMSEventMemoAppService>.Instance.UpdateEIMSInvoiceInput(entities);
        }

    }
}
