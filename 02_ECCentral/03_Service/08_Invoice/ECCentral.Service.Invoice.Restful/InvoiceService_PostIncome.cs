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
        /// 创建电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public PostIncomeInfo CreatePostIncome(PostIncomeInfo input)
        {
            return ObjectFactory<PostIncomeAppService>.Instance.Create(input);
        }

        /// <summary>
        /// 确认电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/Confirm", Method = "PUT")]
        public void ConfirmPostIncome(int sysNo)
        {
            ObjectFactory<PostIncomeAppService>.Instance.Confirm(sysNo);
        }

        /// <summary>
        /// 批量确认电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/BatchConfirm", Method = "PUT")]
        public string BatchConfirmPostIncome(List<int> request)
        {
            return ObjectFactory<PostIncomeAppService>.Instance.BatchConfirm(request);
        }

        /// <summary>
        /// 取消确认电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/CancelConfirm", Method = "PUT")]
        public void CancelConfirmPostIncome(int sysNo)
        {
            ObjectFactory<PostIncomeAppService>.Instance.CancelConfirm(sysNo);
        }

        /// <summary>
        /// 更新电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/PostIncome/Update", Method = "PUT")]
        public void UpdatePostIncome(UpdatePostIncomeReq req)
        {
            ObjectFactory<PostIncomeAppService>.Instance.Update(req.PostIncome, req.ConfirmedSOSysNo);
        }

        /// <summary>
        /// 作废电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/Abandon", Method = "PUT")]
        public void AbandonPostIncome(int sysNo)
        {
            ObjectFactory<PostIncomeAppService>.Instance.Abandon(sysNo);
        }

        /// <summary>
        /// 取消作废电汇邮局收款单
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/CancelAbandon", Method = "PUT")]
        public void CancelAbandonPostIncome(int sysNo)
        {
            ObjectFactory<PostIncomeAppService>.Instance.CancelAbandon(sysNo);
        }

        /// <summary>
        /// 导入
        /// </summary>
        [WebInvoke(UriTemplate = "/PostIncome/Import", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public ImportPostIncomeResp ImportPostIncome(ImportPostIncomeReq req)
        {
            List<PostIncomeInfo> successList = new List<PostIncomeInfo>();
            List<ImportPostIncome> faultList = new List<ImportPostIncome>();
            string message = string.Empty;
            ObjectFactory<PostIncomeAppService>.Instance.ImportPostIncome(
                req.FileIdentity, req.CompanyCode,
            ref  successList, ref  faultList, ref  message);
            return new ImportPostIncomeResp()
            {
                SuccessList = successList,
                FaultList = faultList,
                Message = message
            };
        }

        #region NoBizQuery

        [WebInvoke(UriTemplate = "/PostIncome/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryPostIncome(PostIncomeQueryFilter request)
        {
            int totalCount = 0;
            var dataTable = ObjectFactory<IPostIncomeQueryDA>.Instance.Query(request, out totalCount);
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                if (!row.IsNull("CreateDate"))
                {
                    row["CreateDate"] = Convert.ToDateTime(row["CreateDate"]).ToShortDateString();
                    //((DateTime)row["CreateDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("ModifyDate"))
                {
                    row["ModifyDate"] = Convert.ToDateTime(row["ModifyDate"]).ToShortDateString();
                    //((DateTime)row["ModifyDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("IncomeDate"))
                {
                    row["IncomeDate"] = Convert.ToDateTime(row["IncomeDate"]).ToShortDateString();
                    //((DateTime)row["IncomeDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("ConfirmDate"))
                {
                    if (row["ConfirmStatus"].ToString() == "1")
                    {
                        row["ConfirmDate"] = Convert.ToDateTime(row["ConfirmDate"]).ToShortDateString();
                        //((DateTime)row["ConfirmDate"]).ToString().Substring(0, 10);
                    }
                    else
                    {
                        row["ConfirmDate"] = DBNull.Value;
                    }
                }
            }

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/PostIncome/Export", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult ExportPostIncome(PostIncomeQueryFilter request)
        {
            int totalCount = 0;
            var dataTable = ObjectFactory<IPostIncomeQueryDA>.Instance.Query(request, out totalCount);

            dataTable.Columns.Add("HandleStatusDesc");
            foreach (DataRow row in dataTable.Rows.AsParallel())
            {
                if (!row.IsNull("IncomeAmt"))
                {
                    row["IncomeAmt"] = ((decimal)row["IncomeAmt"]).ToString(InvoiceConst.StringFormat.DecimalFormat);
                }
                if (!row.IsNull("OrderTime"))
                {
                    row["OrderTime"] = ((DateTime)row["OrderTime"]).ToString(InvoiceConst.StringFormat.LongDateFormat);
                }
                if (!row.IsNull("OutTime"))
                {
                    row["OutTime"] = ((DateTime)row["OutTime"]).ToString(InvoiceConst.StringFormat.LongDateFormat);
                }
                if (!row.IsNull("CreateDate"))
                {
                    row["CreateDate"] = ((DateTime)row["CreateDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("ModifyDate"))
                {
                    row["ModifyDate"] = ((DateTime)row["ModifyDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("IncomeDate"))
                {
                    row["IncomeDate"] = ((DateTime)row["IncomeDate"]).ToString().Substring(0, 10);
                }
                if (!row.IsNull("ConfirmDate"))
                {
                    if (row["ConfirmStatus"].ToString() == "1")
                    {
                        row["ConfirmDate"] = ((DateTime)row["ConfirmDate"]).ToString().Substring(0, 10);
                    }
                    else
                    {
                        row["ConfirmDate"] = DBNull.Value;
                    }
                }
                if (!row.IsNull("ConfirmedSOSysNoList"))
                {
                    row["ConfirmedSOSysNoList"] = row["ConfirmedSOSysNoList"].ToString().TrimEnd('.');
                }

                if (row["HandleStatus"].ToString() == "1")
                {
                    row["HandleStatusDesc"] = PostIncomeHandleStatus.Handled.ToDisplayText();
                }
                else
                {
                    if (row["ConfirmStatus"].ToString() == "1")
                    {
                        row["HandleStatusDesc"] = PostIncomeHandleStatusUI.UnHandled.ToDisplayText();
                    }
                    else
                    {
                        row["HandleStatusDesc"] = PostIncomeHandleStatusUI.UnConfirmed.ToDisplayText();
                    }
                }
            }
            return new QueryResult()
            {
                Data = dataTable
                ,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery
    }
}