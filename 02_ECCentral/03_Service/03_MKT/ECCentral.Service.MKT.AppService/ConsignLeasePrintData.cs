using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.AppService
{
    /// <summary>
    /// 转租赁商品结算
    /// </summary>
    [VersionExport(typeof(ConsignLeasePrintData))]
    public class ConsignLeasePrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            string param = requestPostData["GroupSysNo"];
            int GroupSysNo = 0;

            DataTable table = CreatePrintData();
            DataTable errorTable = new DataTable();
            try
            {
                if (!int.TryParse(param, out GroupSysNo))
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_IllegalGroupID"), param));
                }

                InitData(GroupSysNo, table);
            }
            catch (BizException ex)
            {
                InitErrorTable(ex, errorTable);
            }

            tableVariables.Add("Error", errorTable);
            tableVariables.Add("ConsignLeasePrint", table);
        }

        private void InitData(int sysno, DataTable table)
        {
            GroupBuyingSettlementInfo origin = ObjectFactory<GroupBuyingProcessor>.Instance.LoadGroupBuyingSettleBySysNo(sysno);
            if (origin == null)
                return;

            decimal currentMonthMarket = 0;
            DataTable dt = ObjectFactory<GroupBuyingProcessor>.Instance.LoadGroupBuyingSettlementItemBySettleSysNo(sysno);
            if (dt != null || dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    int itemSynso = Convert.ToInt32(r["SysNo"]);

                    DataTable dt2 = ObjectFactory<GroupBuyingProcessor>.Instance.LoadTicketByGroupBuyingSysNo(itemSynso);

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        foreach (DataRow row2 in dt2.Rows)
                        {
                            currentMonthMarket += Convert.ToDecimal(row2["TicketAmt"]); ;
                        }
                    }
                }
            }

            VendorInfo vendor = ExternalDomainBroker.GetVendorInfoBySysNo(origin.VendorSysNo.Value);

            DataRow row = null;
            row = table.NewRow();

            row["StatementSysNo"] = sysno;//结算单号
            row["ContractSysNo"] = string.Empty;//合同号
            row["StatementDate"] = DateTime.Now.ToString("yyyy/MM");//结算年月
            row["PrintDate"] = DateTime.Now.ToString();//打印日期

            row["StatementDepartment"] = ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_StatementDepartment");//结算部门
            row["CurrentMonthMarket"] = currentMonthMarket.ToString("f2");//本月销售
            row["PayDate"] = string.Empty;//付款日

            row["CompanyName1"] = vendor.VendorBasicInfo.VendorNameLocal;//公司名称
            row["CheckType"] = ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_CheckType001");//核算方式

            row["CompanyAddress"] = vendor.VendorBasicInfo.Address;//公司地址
            row["TaxNo"] = vendor.VendorFinanceInfo.TaxNumber;//税号

            row["OpenedBank"] = vendor.VendorFinanceInfo.BankName;//开户行
            row["BankAccount"] = vendor.VendorFinanceInfo.AccountNumber;//账号

            row["VendorCode"] = vendor == null ? string.Empty : vendor.VendorBasicInfo.VendorID;//供货商代码
            row["VendorSysno"] = vendor == null ? string.Empty : origin.VendorSysNo.Value.ToString();//供货商系统编号
            row["CreateDate"] = origin.CreateDate.Value.ToString();//制单日期

            row["CutPaymentName"] = string.Empty;//扣款名称
            row["CutPaymentAmount"] = string.Empty;//扣款金额
            row["TotalCutPaymentAmount"] = string.Empty;//合计

            row["Cost"] = origin.SettleAmt.HasValue ? origin.SettleAmt.Value.ToString("f2") : "0.00";//价款
            row["RateAmount"] = "0.00";//税金
            row["RateTotal"] = origin.SettleAmt.HasValue ? origin.SettleAmt.Value.ToString("f2") : "0.00";//税金合计

            row["17RateCost"] = "0.00";//17%税率价款
            row["17RateAmount"] = "0.00";//税金
            row["17RateTotal"] = "0.00";//税金合计

            row["13RateCost"] = "0.00";//13%税率价款
            row["13RateAmount"] = "0.00";//税金
            row["13RateTotal"] = "0.00";//税金合计

            row["OtherRateCost"] = "0.00";//其他税率价款
            row["OtherRateAmount"] = "0.00";//税金
            row["OtherRateTotal"] = "0.00";//税金合计

            row["ActualPayAmount"] = origin.SettleAmt.HasValue ? origin.SettleAmt.Value.ToString("f2") : "0.00";//实际付款金额

            row["CompanyName2"] = ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_CompanyName2"); ;//公司名称
            row["Producer"] = ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_Producer"); ;//制作人

            row["BusinessCheck"] = origin.EditUserName;//业务审核

            //int usersysno = 0;
            //if (!string.IsNullOrEmpty(origin.EditUser) && int.TryParse(origin.EditUser, out usersysno))
            //{
            //    UserInfo userInfo = ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(usersysno);
            //    if (userInfo != null)
            //        row["BusinessCheck"] = userInfo.UserName;//业务审核
            //}

            row["FinanceRecheck"] = string.Empty;//财务复核

            row["DepartmentManager"] = string.Empty;//部门经理
            row["Manufacturer"] = vendor.VendorBasicInfo.VendorNameLocal;//厂商

            row["display"] = "style='display:none'";

            table.Rows.Add(row);
        }

        private DataTable CreatePrintData()
        {
            DataTable table = new DataTable();

            table.Columns.Add("StatementSysNo");//结算单号
            table.Columns.Add("ContractSysNo");//合同号
            table.Columns.Add("StatementDate");//结算年月
            table.Columns.Add("PrintDate");//打印日期

            table.Columns.Add("StatementDepartment");//结算部门
            table.Columns.Add("CurrentMonthMarket");//本月销售
            table.Columns.Add("PayDate");//付款日

            table.Columns.Add("CompanyName1");//公司名称
            table.Columns.Add("CheckType");//核算方式

            table.Columns.Add("CompanyAddress");//公司地址
            table.Columns.Add("TaxNo");//税号

            table.Columns.Add("OpenedBank");//开户行
            table.Columns.Add("BankAccount");//账号

            table.Columns.Add("VendorCode");//供货商代码
            table.Columns.Add("VendorSysno");//供货商系统编号
            table.Columns.Add("CreateDate");//制单日期

            table.Columns.Add("CutPaymentName");//扣款名称
            table.Columns.Add("CutPaymentAmount");//扣款金额
            table.Columns.Add("TotalCutPaymentAmount");//合计

            table.Columns.Add("Cost");//价款
            table.Columns.Add("RateAmount");//税金
            table.Columns.Add("RateTotal");//税金合计

            table.Columns.Add("17RateCost");//17%税率价款
            table.Columns.Add("17RateAmount");//税金
            table.Columns.Add("17RateTotal");//税金合计

            table.Columns.Add("13RateCost");//13%税率价款
            table.Columns.Add("13RateAmount");//税金
            table.Columns.Add("13RateTotal");//税金合计

            table.Columns.Add("OtherRateCost");//其他税率价款
            table.Columns.Add("OtherRateAmount");//税金
            table.Columns.Add("OtherRateTotal");//税金合计

            table.Columns.Add("ActualPayAmount");//实际付款金额

            table.Columns.Add("CompanyName2");//公司名称
            table.Columns.Add("Producer");//制作人

            table.Columns.Add("BusinessCheck");//业务审核
            table.Columns.Add("FinanceRecheck");//财务复核

            table.Columns.Add("DepartmentManager");//部门经理
            table.Columns.Add("Manufacturer");//厂商

            table.Columns.Add("display");

            return table;
        }

        private void InitErrorTable(BizException ex, DataTable errorTable)
        {
            if (!errorTable.Columns.Contains("ErrorMessage"))
            {
                errorTable.Columns.Add("ErrorMessage");
            }
            DataRow row = errorTable.NewRow();
            row["ErrorMessage"] = ex.Message;
            errorTable.Rows.Add(row);
        }
    }
}
