using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Invoice;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.BizEntity.IM;
using System.Data;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        #region 应付款汇总查询
        [WebInvoke(UriTemplate = "/Invoice/QueryFinance", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList FinanceQuery(FinanceQueryFilter filter)
        {
            int totalCount = 0;
            double totalPayAmt = 0.0;

            var dt = ObjectFactory<IFinanceDA>.Instance.FinanceQuery(filter, out totalCount, out totalPayAmt);
            
            var amtDT = new DataTable();
            amtDT.Columns.Add("Amt");
            var row = amtDT.NewRow();
            row[0] = totalPayAmt;
            amtDT.Rows.Add(row);

            return new QueryResultList()
            {
                new QueryResult(){ Data = dt, TotalCount = totalCount},
                new QueryResult(){ Data = amtDT}
            };
        }

        [WebInvoke(UriTemplate = "/Invoice/ExportFinance", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportFinance(FinanceQueryFilter filter)
        {
            int totalCount = 0;
            double totalPayAmt = 0.0;

            var dt = ObjectFactory<IFinanceDA>.Instance.FinanceExport(filter, out totalCount, out totalPayAmt);
            if (dt.Columns.Contains("RMACount"))
            {
                dt.Columns.Add("RMACountDescription");
                foreach (DataRow row in dt.Rows)
                { 
                    int rmaCount = 0;
                    if(row["RMACount"] != null)
                    {
                        if (int.TryParse(row["RMACount"].ToString(), out rmaCount) && rmaCount > 0)
                        {
                            row["RMACountDescription"] = ResCommonEnum.Boolean_True;
                            continue;
                        }
                    }
                    row["RMACountDescription"] = ResCommonEnum.Boolean_False;
                }
            }

            return new QueryResult() { Data = dt, TotalCount = totalCount };
        }
        #endregion

        /// <summary>
        /// 获取PM组信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/GetPMGroup", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult GetPMGroup()
        {
            int totalCount = 0;
            return new QueryResult()
            {
                Data = ObjectFactory<IFinanceDA>.Instance.GetPMGroupList(),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 添加备注
        /// </summary>
        [WebInvoke(UriTemplate = "/Invoice/AddMemo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public PayableInfo AddMemo(PayableInfo info)
        {
            return ObjectFactory<FinanceAppService>.Instance.AddMemo(info);
        }

        /// <summary>
        /// 查询应付款信息
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/PaybleQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<PayableInfo> PayableQuery(PayableCriteriaInfo info)
        {
            return ObjectFactory<FinanceAppService>.Instance.PayableQuery(info);
        }

        /// <summary>
        /// 根据SysNo获取Memo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/GetMemoBySysNo/{sysNo}", Method = "GET")]
        public QueryResult GetMemoBySysno(string sysNo)
        {
            return new QueryResult()
            {
                Data = ObjectFactory<IFinanceDA>.Instance.GetMemoBySysNo(Convert.ToInt32(sysNo))
            };
        }
    }
}
