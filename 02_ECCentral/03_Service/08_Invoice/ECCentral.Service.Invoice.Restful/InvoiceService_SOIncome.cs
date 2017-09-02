using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 批量确认收款单
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/SOIncome/BatchConfirm", Method = "PUT")]
        public string BatchConfirmSOIncome(List<int> sysNoList)
        {
            var result = ObjectFactory<SOIncomeAppService>.Instance.BatchConfirm(sysNoList);
            return result.PromptMessage;
        }
        /// <summary>
        /// 强制确认
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchForcesConfirm", Method = "PUT")]
        public string BatchForcesConfirm(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchForcesConfirm(sysNoList);
        }

        /// <summary>
        /// 批量确认销售收款单（Job调用）
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchConfirmJob", Method = "PUT")]
        public ZeroConfirmSOIncomeJobResp BatchConfirmSOIncomeForJob(List<int> sysNoList)
        {
            var result = ObjectFactory<SOIncomeAppService>.Instance.BatchConfirm(sysNoList);

            ZeroConfirmSOIncomeJobResp resp = new ZeroConfirmSOIncomeJobResp();
            resp.Result = new List<ZeroConfirmSOIncomeJobResultItem>();
            foreach (var r in result.SuccessList)
            {
                resp.Result.Add(new ZeroConfirmSOIncomeJobResultItem()
                {
                    SysNo = r.ID
                });
            }
            foreach (var r in result.FaultList)
            {
                resp.Result.Add(new ZeroConfirmSOIncomeJobResultItem()
                {
                    SysNo = r.FaultItem.ID,
                    ErrorDescription = r.FaultException.Message
                });
            }
            return resp;
        }

        /// <summary>
        /// 自动确认收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncome/AutoConfirm", Method = "PUT")]
        public AutoConfirmSOIncomeResp AutoConfirmSOIncome(AutoConfirmSOIncomeReq request)
        {
            List<int> successSysNoList;
            List<int> failedSysNoList;
            int submitConfirmCount;
            string failedMsg;

            ObjectFactory<SOIncomeAppService>.Instance.AutoConfirm(request.FileIdentity, request.SOOutFromDate, request.SOOutToDate,
                out successSysNoList, out failedSysNoList, out submitConfirmCount, out failedMsg);

            AutoConfirmSOIncomeResp resp = new AutoConfirmSOIncomeResp();
            resp.SuccessSysNoList = successSysNoList;
            resp.FailedSysNoList = failedSysNoList;
            resp.SubmitConfirmCount = submitConfirmCount;
            resp.FailedMessage = failedMsg;

            return resp;
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchSetReferenceID", Method = "PUT")]
        public string BatchSetSOIncomeReferenceID(BatchSetSOIncomeReferenceIDReq request)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchSetReferenceID(request.SysNoList, request.ReferenceID);
        }

        /// <summary>
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/SetIncomeAmt", Method = "PUT")]
        public void SetSOIncomeIncomeAmt(SetIncomeAmtReq request)
        {
            ObjectFactory<SOIncomeAppService>.Instance.SetIncomeAmt(request.SysNo, request.IncomeAmt);
        }

        /// <summary>
        /// 批量取消确认收款单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchCancelConfirm", Method = "PUT")]
        public string BatchCancelConfirmSOIncome(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchCancelConfirm(sysNoList);
        }

        /// <summary>
        /// 批量作废销售收款单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchAbandon", Method = "PUT")]
        public string BatchAbandonSOIncome(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchAbandon(sysNoList);
        }

        /// <summary>
        /// 根据系统编号加载销售收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/SOIncome/Load/{sysNo}", Method = "GET")]
        public SOIncomeInfo GetSOIncomeBySysNo(string sysNo)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.GetBySysNo(int.Parse(sysNo));
        }

        /// <summary>
        /// 根据订单系统编号取得有效的收款记录
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/GetValid/{soSysNo}", Method = "GET")]
        public SOIncomeInfo GetValidSOIncomeBySOSysNo(string soSysNo)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.GetValid(int.Parse(soSysNo), SOIncomeOrderType.SO);
        }

        /// <summary>
        /// 手动网关退款
        /// </summary>
        /// <param name="sysNo">单据编号</param>
        [WebInvoke(UriTemplate = "/SOIncome/ManualBankRefund", Method = "PUT")]
        public void ManualBankRefund(string sysNo)
        {
            ObjectFactory<SOIncomeAppService>.Instance.ManualBankRefund(sysNo);
        }

        /// <summary>
        /// 批量手动网关退款
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/SOIncome/BatchManualRefund", Method = "PUT")]
        public string BatchManualRefund(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchManualRefund(sysNoList);
        }

        #region NoBizQuery

        /// <summary>
        /// 查询销售收款单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResultList QuerySOIncome(SOIncomeQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ISOIncomeQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){ Data = dataSet.Tables["DataResult"], TotalCount = totalCount },
                new QueryResult(){ Data = dataSet.Tables["StatisticResult"] }
            };
        }

        /// <summary>
        /// 自动确认收款单时查询未确认收款的订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/QuerySO", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResultList QuerySO(SOQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ISOIncomeQueryDA>.Instance.QuerySO(request, out totalCount);
            return new QueryResultList()
            {
                new QueryResult(){Data = dataSet.Tables["DataResult"],TotalCount=totalCount},
                new QueryResult(){Data = dataSet.Tables["StatisticResult"]}
            };
        }

        [WebInvoke(UriTemplate = "/SOIncome/ExportQuerySO", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult ExportQuerySO(SOQueryFilter request)
        {

            var dataSet = ObjectFactory<ISOIncomeQueryDA>.Instance.ExportQuerySO(request);
            return new QueryResult() { Data = dataSet.Tables["Table"], TotalCount = dataSet.Tables["Table"].Rows.Count };


        }

        /// <summary>
        /// 导出销售收款单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/Export", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult ExportSOIncomeExcelFile(SOIncomeQueryFilter request)
        {
            int totalCount;
            var dataSet = ObjectFactory<ISOIncomeQueryDA>.Instance.Query(request, out totalCount);
            var dataTable = dataSet.Tables["DataResult"];

            dataTable.Columns.Add("BankInfo");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                row["BankInfo"] = row["BankName"] + "-" + row["BranchBankName"];
                if (row["SapImportedStatus"] == DBNull.Value)
                {
                    row["SapImportedStatus"] = (int)SapImportedStatus.UnHandle;
                }
            }

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 导出RO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/ExportRO", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult ExportROExcelFile(SOIncomeQueryFilter request)
        {
            var dataTable = ObjectFactory<ISOIncomeQueryDA>.Instance.QueryROExport(request).Tables["Table"];

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = dataTable.Rows.Count
            };
        }

        [WebInvoke(UriTemplate = "/SOIncome/CreatSOIncome", Method = "POST")]
        public virtual SOIncomeInfo CreatSOIncome(SOIncomeInfo soIncomeInfo)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.Create(soIncomeInfo);
        }


        /// <summary>
        /// 查询应收单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/QuerySaleReceivables", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySaleReceivables(SaleReceivablesQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ISOIncomeQueryDA>.Instance.QuerySaleReceivables(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 运费报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/QuerySOFreightStatDetai", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySOFreightStatDetai(SOFreightStatDetailQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ISOIncomeQueryDA>.Instance.QuerySOFreightStatDetai(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion NoBizQuery


        /// <summary>
        /// 批量确认运费支出
        /// </summary>
        /// <param name="netpaySysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchRealFreightConfirm", Method = "PUT")]
        public string BatchRealFreightConfirm(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchRealFreightConfirm(sysNoList);
        }

        /// <summary>
        /// 批量确认运费收入
        /// </summary>
        /// <param name="netpaySysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SOIncome/BatchSOFreightConfirm", Method = "PUT")]
        public string BatchSOFreightConfirm(List<int> sysNoList)
        {
            return ObjectFactory<SOIncomeAppService>.Instance.BatchSOFreightConfirm(sysNoList);
        }
    }
}