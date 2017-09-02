using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Invoice;
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
        private InvoiceInputAppService appService = ObjectFactory<InvoiceInputAppService>.Instance;
        

        #region NoBizQuery

        /// <summary>
        /// 发票匹配审核查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InvoiceInput/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryInvoiceInput(InvoiceInputQueryFilter request)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<IInvoiceInputQueryDA>.Instance.QueryInvoiceInput(request, out totalCount);

            dataTable.Columns.Add("POAmtSum", typeof(decimal));
            dataTable.Columns.Add("EIMSAmtSum", typeof(decimal));
            dataTable.Columns.Add("PO_S", typeof(string));
            dataTable.Columns.Add("Invoice_S", typeof(string));
            dataTable.Columns.Add("InUserAdd", typeof(string));
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    int sysNo = (int)dr["SysNo"];

                    List<APInvoicePOItemInfo> POItemList = appService.GetPOItemsByDocNo(sysNo);
                    List<APInvoiceInvoiceItemInfo> InvoiceItemList = appService.GetInvoiceItemsByDocNo(sysNo);
                    dr["POAmtSum"] = POItemList.Sum(p =>
                    {
                        return p.PoAmt ?? 0;
                    });
                    dr["EIMSAmtSum"] = POItemList.Sum(p =>
                    {
                        return p.EIMSAmt ?? 0;
                    });

                    if (POItemList != null && POItemList.Count > 0)
                    {
                        var result = (from i in POItemList
                                      select i.PoNo).Min();
                        dr["PO_S"] = string.Format("{0}({1})", result.ToString(), POItemList.Count);
                    }
                    if (InvoiceItemList != null && InvoiceItemList.Count > 0)
                    {
                        var result = (from i in InvoiceItemList
                                      select i.InvoiceNo).Min();
                        dr["Invoice_S"] = string.Format("{0}({1})", result.ToString(), InvoiceItemList.Count);
                    }
                    if ((int)dr["IsVendorPortal"] == 1)
                    {
                        var splitArray = dr["InUser"].ToString().Split('/');
                        if (splitArray.Count() == 5)
                        {
                            dr["InUserAdd"] = string.Format("{0}-{1}", splitArray[3], splitArray[4].PadLeft(4, '0'));
                        }
                        else
                        {
                            dr["InUserAdd"] = "N/A";
                        }
                    }
                    else
                    {
                        dr["InUserAdd"] = dr["InUser"];
                    }
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery

        #region BatchAction

        /// <summary>
        /// 批量退回
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/InvoiceInput/BatchVPCancel", Method = "PUT")]
        public string BatchVPCancel(APInvoiceBatchUpdateReq request)
        {
            return appService.BatchVPCancel(request.SysNoList, request.VPCancelReason);
        }

        /// <summary>
        /// 批量提交
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/InvoiceInput/BatchSubmit", Method = "PUT")]
        public string BatchSubmitAPInvoice(List<int> sysNoList)
        {
            return appService.BatchSubmitAPInvoice(sysNoList);
        }

        /// <summary>
        /// 批量撤销
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/InvoiceInput/BatchCancel", Method = "PUT")]
        public string BatchCancelAPInvoice(List<int> sysNoList)
        {
            return appService.BatchCancelAPInvoice(sysNoList);
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/InvoiceInput/BatchAudit", Method = "PUT")]
        public string BatchAuditAPInvoice(List<int> sysNoList)
        {
            return appService.BatchAuditAPInvoice(sysNoList);
        }

        /// <summary>
        /// 批量拒绝
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/InvoiceInput/BatchRefuse", Method = "PUT")]
        public string BatchRefuseAPInvoice(List<int> sysNoList)
        {
            return appService.BatchRefuseAPInvoice(sysNoList);
        }

        #endregion BatchAction

        #region Input

        /// <summary>
        /// 录入POItems
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InvoiceInput/InputPoItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public APInvoiceInfoResp InputPoItem(APInvoiceItemInputEntity inputEntity)
        {
            string errorMsg = string.Empty;
            string vendorName = string.Empty;
            int vendorSysNo;
            List<APInvoicePOItemInfo> result = appService.InputPoItem(inputEntity, ref vendorName, ref errorMsg, out vendorSysNo);
            return new APInvoiceInfoResp()
            {
                poItemList = result,
                VendorName = vendorName,
                VendorSysNo = vendorSysNo,
                ErrorMsg = errorMsg
            };
        }

        /// <summary>
        /// 录入InvoiceItems
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InvoiceInput/InputInvoiceItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public APInvoiceInfoResp InputInvoiceItem(APInvoiceItemInputEntity inputEntity)
        {
            string errorMsg = string.Empty;
            List<APInvoiceInvoiceItemInfo> result = appService.InputInvoiceItem(inputEntity, ref errorMsg);
            return new APInvoiceInfoResp()
            {
                invoiceItemList = result,
                ErrorMsg = errorMsg
            };
        }

        /// <summary>
        /// 获取供应商付款结算公司
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/InvoiceInput/GetPaySettleCompany/{sysNo}")]       
        public int GetPaySettleCompany(string sysNo)
        {
            int vendorSysNo  = 0;
            if (!string.IsNullOrEmpty(sysNo))
            {
                vendorSysNo = Convert.ToInt32(sysNo);
            }

            return ObjectFactory<IInvoiceInputQueryDA>.Instance.GetPaySettleCompany(vendorSysNo);
        }

        #endregion Input

        #region Load

        [WebInvoke(UriTemplate = "/InvoiceInput/Load/{sysNo}", Method = "GET")]
        public APInvoiceInfo LoadAPInvoiceWithItemsBySysNo(string sysNo)
        {
            int _SysNo = 0;
            if (int.TryParse(sysNo, out _SysNo))
            {
                return appService.LoadAPInvoiceWithItemsBySysNo(_SysNo);
            }
            throw new ArgumentException("Invalid sysNo");
        }

        /// <summary>
        /// 加载未录入的POItems
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/InvoiceInput/LoadNotInputPOItems", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<APInvoicePOItemInfo> LoadNotInputPOItems(APInvoiceItemInputEntity request)
        {
            return appService.LoadNotInputPOItems(request);
        }

        #endregion Load

        #region Action--Create&Update

        [WebInvoke(UriTemplate = "/InvoiceInput/CreateAPInvoice", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public APInvoiceInfo CreateAPInvoice(APInvoiceInfo data)
        {
            return appService.CreateAPInvoice(data);
        }

        [WebInvoke(UriTemplate = "/InvoiceInput/UpdateAPInvoice", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public APInvoiceInfo UpdateAPInvoice(APInvoiceInfo data)
        {
            return appService.UpdateAPInvoice(data);
        }

        [WebInvoke(UriTemplate = "/InvoiceInput/SubmitWithSaveAPInvoice", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public APInvoiceInfo SubmitWithSaveAPInvoice(APInvoiceInfo data)
        {
            return appService.SubmitWithSaveAPInvoice(data);
        }

        #endregion Action--Create&Update
    }
}
