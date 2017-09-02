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
    public class ShiftRequestPrintData : IPrintDataBuild
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
                    ShiftRequestInfo requestInfo = ObjectFactory<ShiftRequestAppService>.Instance.GetShiftRequestInfoBySysNo(requestSysNo);

                    variables.Add("SelfCompanyName", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Name"));
                    variables.Add("SelfCompanyAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Address"));
                    variables.Add("SelfCompanyPhone", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Phone"));
                    variables.Add("SelfCompanyWebAddress", ResouceManager.GetMessageString(CommonConst.MessagePath_Common, "Res_Company_Url"));
                    variables.Add("CreateUser", requestInfo.CreateUser.UserDisplayName);
                    variables.Add("CreateDate", requestInfo.CreateDate.HasValue ? requestInfo.CreateDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("AuditUser", requestInfo.AuditUser.UserDisplayName);
                    variables.Add("AuditDate", requestInfo.AuditDate.HasValue ? requestInfo.AuditDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("RequestSysNo", requestInfo.RequestID);
                    variables.Add("OutStock", requestInfo.SourceStock == null ? "" : requestInfo.SourceStock.StockName);
                    variables.Add("InStock", requestInfo.TargetStock == null ? "" : requestInfo.TargetStock.StockName);
                    variables.Add("Status", requestInfo.RequestStatus.ToDisplayText());
                    variables.Add("OutStockUser", requestInfo.OutStockUser.UserDisplayName);
                    variables.Add("OutStockDate", requestInfo.OutStockDate.HasValue ? requestInfo.OutStockDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("InStockUser", requestInfo.InStockUser.UserDisplayName);
                    variables.Add("InStockDate", requestInfo.InStockDate.HasValue ? requestInfo.InStockDate.Value.ToString(SOConst.LongTimeFormat) : string.Empty);
                    variables.Add("Note", requestInfo.Note);

                    DataTable dtProduct = new DataTable();
                    dtProduct.Columns.AddRange(new System.Data.DataColumn[] 
                        { 
                            new DataColumn("ProductID"), 
                            new DataColumn("ProductName"), 
                            new DataColumn("Quantity"), 
                        });
                    requestInfo.ShiftItemInfoList.ForEach(p =>
                    {
                        DataRow drProduct = dtProduct.NewRow();
                        drProduct["ProductID"] = p.ShiftProduct.ProductID;
                        drProduct["ProductName"] = p.ShiftProduct.ProductBasicInfo.ProductBriefTitle;
                        ;
                        drProduct["Quantity"] = p.ShiftQuantity;
                        dtProduct.Rows.Add(drProduct);
                    });
                    tableVariables.Add("ProductList", dtProduct);
                }
            }

        }

    } 
}
