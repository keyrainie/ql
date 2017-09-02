using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.RMA.AppService
{
    public class RegisterPrintData : IPrintDataBuild
    {
        private const string Selected   = "■";
        private const string NoSelected = "□";
        private const string Blank ="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            string sysNo = requestPostData["SysNo"];
            if (!string.IsNullOrEmpty(sysNo))
            {
                ProcessType processType;
                InvoiceType? invoiceType;
                CustomerInfo customer;
                RMARequestInfo request;
                List<ProductInventoryInfo> inventoryList;
                int registerSysNo;
                string businessModel;
                int? refundSysNo;
                ProductInventoryType inventoryType;
                if (int.TryParse(sysNo, out registerSysNo))
                {
                    var register = ObjectFactory<RegisterAppService>.Instance.LoadForEditBySysNo(
                        registerSysNo,
                        out businessModel, 
                        out processType, 
                        out invoiceType, 
                        out customer, 
                        out request, 
                        out inventoryList,
                        out refundSysNo,
                        out inventoryType);

                    request.ReceiveUserName = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(request.CreateUserSysNo.ToString(), true);

                    variables.Add("ProductID", register.BasicInfo.ProductID);
                    variables.Add("SysNo", register.SysNo);
                    variables.Add("SOSysNo", request.SOSysNo);
                    variables.Add("ReceiveTime", FormatDateTime(request.ReceiveTime));
                    variables.Add("ProductName", register.BasicInfo.ProductName);
                    variables.Add("Note", request.Note);
                    variables.Add("CustomerDesc", register.BasicInfo.CustomerDesc);
                    variables.Add("ReceiveUserName", request.ReceiveUserName);
                    variables.Add("CheckDesc", register.CheckInfo.CheckDesc);
                    variables.Add("CheckUserName", string.IsNullOrEmpty(register.CheckInfo.CheckUserName) ? Blank : register.CheckInfo.CheckUserName);
                    variables.Add("Blank", Blank);
                    variables.Add("CheckTime", FormatDateTime(register.CheckInfo.CheckTime));
                    variables.Add("ResponseUserName", string.IsNullOrEmpty(register.ResponseInfo.ResponseUserName) ? Blank : register.ResponseInfo.ResponseUserName);
                    variables.Add("ResponseTime", FormatDateTime(register.ResponseInfo.ResponseTime));
                    bool flag = register.RevertInfo.NewProductStatus.HasValue && register.RevertInfo.NewProductStatus == RMANewProductStatus.Origin;
                    variables.Add("IsRefunded", GetYesNoDes(flag));
                    flag = register.RevertInfo.NewProductStatus.HasValue && register.RevertInfo.NewProductStatus == RMANewProductStatus.Origin;
                    variables.Add("IsNewProduct", GetYesNoDes(flag, true));
                    flag = register.BasicInfo.ReturnStatus.HasValue && register.BasicInfo.ReturnStatus.Value == RMAReturnStatus.Returned;
                    variables.Add("IsReturned", GetYesNoDes(flag));
                    variables.Add("ReturnCreateUserName", string.IsNullOrEmpty(register.ReturnInfo.CreateUserName) ? Blank : register.ReturnInfo.CreateUserName);
                    variables.Add("ReturnUserName", string.IsNullOrEmpty(register.ReturnInfo.ReturnUserName) ? Blank : register.ReturnInfo.ReturnUserName);
                    variables.Add("ReturnTime", FormatDateTime(register.ReturnInfo.ReturnTime));
                    variables.Add("RevertShipTypeName", register.RevertInfo.RevertShipTypeName);
                    variables.Add("RevertPackageID", register.RevertInfo.RevertPackageID);
                    variables.Add("RevertCreateUserName", string.IsNullOrEmpty(register.RevertInfo.CreateUserName) ? Blank : register.RevertInfo.CreateUserName);
                    variables.Add("RevertUserName", string.IsNullOrEmpty(register.RevertInfo.RevertUserName) ? Blank : register.RevertInfo.RevertUserName);
                    variables.Add("OutTime", FormatDateTime(register.RevertInfo.OutTime));
                    variables.Add("CloseUserName", string.IsNullOrEmpty(register.BasicInfo.CloseUserName) ? Blank : register.BasicInfo.CloseUserName);
                    variables.Add("CloseTime", FormatDateTime(register.BasicInfo.CloseTime));                    
                }
            }
        }

        private string GetYesNoDes(bool flag, params object[] para)
        {
            if (para.Length == 0)
            {
                string yes = string.Format("{0}是&nbsp;", flag ? Selected : NoSelected);
                string no = string.Format("{0}否", flag ? NoSelected : Selected);
                return yes + no;
            }
            else
            {
                string yes = string.Format("{0}是&nbsp;", flag ? NoSelected : Selected);
                string no = string.Format("{0}否", flag ? Selected : NoSelected );
                return yes + no;
            }
        }
      
        public string FormatDateTime(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "";
        }

        #endregion
    }
}
