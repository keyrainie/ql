using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Text;
using ECCentral.BizEntity.IM;
using System.Data;
using System.IO;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.AppService
{
    public class AdjustRequestPrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string sysNo = requestPostData["SysNo"];
            if (sysNo != null && sysNo.Trim() != String.Empty)
            {
                sysNo = System.Web.HttpUtility.UrlDecode(sysNo);
                int requestSysNo = int.TryParse(sysNo, out requestSysNo) ? requestSysNo : int.MinValue;

                if (requestSysNo > int.MinValue)
                {
                    AdjustRequestInfo requestInfo = ObjectFactory<AdjustRequestAppService>.Instance.GetAdjustRequestInfoBySysNo(requestSysNo);

                    variables.Add("SelfCompanyName", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Name"));
                    variables.Add("SelfCompanyAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Address"));
                    variables.Add("SelfCompanyPhone", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Phone"));
                    variables.Add("SelfCompanyWebAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Url"));
                    variables.Add("CreateUser", requestInfo.CreateUser.UserDisplayName);
                    variables.Add("CreateDate", requestInfo.CreateDate.HasValue ? requestInfo.CreateDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("AuditUser", requestInfo.AuditUser.UserDisplayName);
                    variables.Add("AuditDate", requestInfo.AuditDate.HasValue ? requestInfo.AuditDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("RequestSysNo", requestInfo.RequestID);
                    variables.Add("Stock", requestInfo.Stock == null ? "" : requestInfo.Stock.StockName);
                    variables.Add("Status", requestInfo.RequestStatus.ToDisplayText());
                    variables.Add("OutStockUser", requestInfo.OutStockUser.UserDisplayName);
                    variables.Add("OutStockDate", requestInfo.OutStockDate.HasValue ? requestInfo.OutStockDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("Note", requestInfo.Note);

                    DataTable dtProduct = new DataTable();
                    dtProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("Price"), 
                            new DataColumn("Quantity"), 
                            new DataColumn("Amount") 
                        });
                    decimal totalAmount = 0M;
                    requestInfo.AdjustItemInfoList.ForEach(p =>
                    {
                        totalAmount += p.AdjustQuantity * p.AdjustCost;
                        DataRow drProduct = dtProduct.NewRow();
                        drProduct["ProductID"] = p.AdjustProduct.ProductID;
                        drProduct["ProductName"] = p.AdjustProduct.ProductBasicInfo.ProductTitle;
                        drProduct["Price"] = p.AdjustCost.ToString("N2");
                        drProduct["Quantity"] = p.AdjustQuantity;
                        drProduct["Amount"] = (p.AdjustQuantity * p.AdjustCost).ToString("N2");
                        dtProduct.Rows.Add(drProduct);
                    });
                    variables.Add("TotalAmount", totalAmount.ToString("N2"));
                    tableVariables.Add("ProductList", dtProduct);

                }
            }

        }

    }
    public class AdjustRequestInvoicePrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string sysNo = requestPostData["SysNo"];
            if (sysNo != null && sysNo.Trim() != String.Empty)
            {
                sysNo = System.Web.HttpUtility.UrlDecode(sysNo);
                int requestSysNo = int.TryParse(sysNo, out requestSysNo) ? requestSysNo : int.MinValue;

                if (requestSysNo > int.MinValue)
                {
                    AdjustRequestInfo requestInfo = ObjectFactory<AdjustRequestAppService>.Instance.GetAdjustRequestInfoBySysNo(requestSysNo);

                    DataTable dtInvoice = new DataTable();
                    dtInvoice.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("OutStockDate"),
                        new DataColumn("ReceiveName"),
                        new DataColumn("Contact"),
                        new DataColumn("Address"),
                        new DataColumn("Phone"),
                        new DataColumn("Note"),
                        new DataColumn("CustomerID"),
                        new DataColumn("RequestSysNo"),
                        new DataColumn("UpperAmount"),
                        new DataColumn("ProductList",typeof(DataTable))
                    });
                    tableVariables.Add("InvoiceList", dtInvoice);
                    DataRow dr = dtInvoice.NewRow();
                    dr["ReceiveName"] = requestInfo.InvoiceInfo.ReceiveName;
                    dr["Contact"] = requestInfo.InvoiceInfo.ContactAddress;
                    dr["Address"] = requestInfo.InvoiceInfo.ContactShippingAddress;
                    dr["Phone"] = requestInfo.InvoiceInfo.ContactPhoneNumber;
                    dr["CustomerID"] = requestInfo.InvoiceInfo.CustomerID;
                    dr["RequestSysNo"] = requestInfo.SysNo;
                    dr["OutStockDate"] = requestInfo.OutStockDate.HasValue ? requestInfo.OutStockDate.Value.ToString(SOConst.DateFormat) : string.Empty;
                    dr["Note"] = requestInfo.InvoiceInfo.Note;
                    DataTable dtProduct = new DataTable();
                    dtProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("Price"), 
                            new DataColumn("Quantity"), 
                            new DataColumn("Amount") 
                        });

                    decimal totalAmount = 0M;
                    int i = 0;
                    requestInfo.AdjustItemInfoList.ForEach(p =>
                    {
                        i++;
                        totalAmount += p.AdjustQuantity * p.AdjustCost;
                        DataRow drProduct = dtProduct.NewRow();
                        drProduct["ProductID"] = p.AdjustProduct.ProductID;
                        drProduct["ProductName"] = p.AdjustProduct.ProductBasicInfo.ProductBriefTitle;
                        drProduct["Price"] = p.AdjustCost.ToString("N2");
                        drProduct["Quantity"] = p.AdjustQuantity;
                        drProduct["Amount"] = (p.AdjustQuantity * p.AdjustCost).ToString("N2");
                        dtProduct.Rows.Add(drProduct);

                        if (i % 6 == 0 || i == requestInfo.AdjustItemInfoList.Count)
                        {
                            dr["ProductList"] = dtProduct;
                            dr["UpperAmount"] = MoneyUtility.GetChineseMoney(totalAmount);
                            dtInvoice.Rows.Add(dr);
                            if (i != requestInfo.AdjustItemInfoList.Count - 1)
                            {
                                dtProduct = dtProduct.Clone();
                                DataRow tdr = dtInvoice.NewRow();
                                // dtInvoice.Rows.Add(tdr);
                                tdr["ReceiveName"] = dr["ReceiveName"];
                                tdr["Contact"] = dr["Contact"];
                                tdr["Address"] = dr["Address"];
                                tdr["Phone"] = dr["Phone"];
                                tdr["CustomerID"] = dr["CustomerID"];
                                tdr["RequestSysNo"] = dr["RequestSysNo"];
                                tdr["OutStockDate"] = dr["OutStockDate"];
                                tdr["Note"] = dr["Note"];
                                dr = tdr;
                                dtProduct.Rows.Clear();
                            }
                        }
                    });

                }
            }
        }
    }

}
