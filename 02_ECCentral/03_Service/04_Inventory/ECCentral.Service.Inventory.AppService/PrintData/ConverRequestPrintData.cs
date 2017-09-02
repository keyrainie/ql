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
    public class ConverRequestPrintData : IPrintDataBuild
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
                    ConvertRequestInfo requestInfo = ObjectFactory<ConvertRequestAppService>.Instance.GetConvertRequestInfoBySysNo(requestSysNo);

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

                    DataTable dtSourceProduct = new DataTable();
                    dtSourceProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("Price"), 
                            new DataColumn("Quantity"), 
                            new DataColumn("Amount") 
                        });

                    DataTable dtTargetProduct = dtSourceProduct.Clone();
                    decimal sourceTotalAmount = 0M, targetTotalAmount = 0M;
                    requestInfo.ConvertItemInfoList.ForEach(p =>
                    {
                        DataRow drProduct = p.ConvertType == ConvertProductType.Source ? dtSourceProduct.NewRow() : dtTargetProduct.NewRow();
                        drProduct["ProductID"] = p.ConvertProduct.ProductID;
                        drProduct["ProductName"] = p.ConvertProduct.ProductBasicInfo.ProductTitle;
                        drProduct["Price"] = p.ConvertUnitCost.ToString("N2");
                        drProduct["Quantity"] = p.ConvertQuantity;
                        drProduct["Amount"] = (p.ConvertQuantity * p.ConvertUnitCost).ToString("N2");
                        if (p.ConvertType == ConvertProductType.Source)
                        {
                            sourceTotalAmount += p.ConvertQuantity * p.ConvertUnitCost;
                            dtSourceProduct.Rows.Add(drProduct);
                        }
                        else
                        {
                            targetTotalAmount += p.ConvertQuantity * p.ConvertUnitCost;
                            dtTargetProduct.Rows.Add(drProduct);
                        }
                    });
                    variables.Add("SourceTotalAmount", sourceTotalAmount.ToString("N2"));
                    variables.Add("TargetTotalAmount", targetTotalAmount.ToString("N2"));
                    tableVariables.Add("SourceProductList", dtSourceProduct);
                    tableVariables.Add("TargetProductList", dtTargetProduct);
                }
            }

        }

    }

}
