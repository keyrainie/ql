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
    public class LendRequestPrintData : IPrintDataBuild
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
                    LendRequestInfo requestInfo = ObjectFactory<LendRequestAppService>.Instance.GetLendRequestInfoBySysNo(requestSysNo);

                    variables.Add("SelfCompanyName", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Name"));
                    variables.Add("SelfCompanyAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Address"));
                    variables.Add("SelfCompanyPhone", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Phone"));
                    variables.Add("SelfCompanyWebAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Url"));
                    variables.Add("CreateUser", requestInfo.CreateUser.UserDisplayName);
                    variables.Add("CreateDate", requestInfo.CreateDate.HasValue ? requestInfo.CreateDate.Value.ToString() : string.Empty);
                    variables.Add("AuditUser", requestInfo.AuditUser.UserDisplayName);
                    variables.Add("AuditDate", requestInfo.AuditDate.HasValue ? requestInfo.AuditDate.Value.ToString() : string.Empty);
                    variables.Add("RequestSysNo", requestInfo.RequestID);
                    variables.Add("Stock", requestInfo.Stock == null ? "" : requestInfo.Stock.StockName);
                    variables.Add("Status", requestInfo.RequestStatus.ToDisplayText());
                    variables.Add("OutStockUser", requestInfo.OutStockUser.UserDisplayName);
                    variables.Add("OutStockDate", requestInfo.OutStockDate.HasValue ? requestInfo.OutStockDate.Value.ToString() : string.Empty);
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
                    requestInfo.LendItemInfoList.ForEach(p =>
                    {
                        totalAmount += p.LendQuantity * p.LendUnitCost;
                        DataRow drProduct = dtProduct.NewRow();
                        drProduct["ProductID"] = p.LendProduct.ProductID;
                        drProduct["ProductName"] = p.LendProduct.ProductBasicInfo.ProductTitle;
                        drProduct["Price"] = p.LendUnitCost.ToString("N2");
                        drProduct["Quantity"] = p.LendQuantity;
                        drProduct["Amount"] = (p.LendQuantity * p.LendUnitCost).ToString("N2");
                        dtProduct.Rows.Add(drProduct);
                    });
                    variables.Add("TotalAmount", totalAmount.ToString("N2"));
                    tableVariables.Add("ProductList", dtProduct);

                }
            }

        }

    }  
}
