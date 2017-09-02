using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;

namespace ECCentral.Service.PO.AppService
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

            string param = requestPostData["ConsignSysNo"];
            int ConsignSysNo = 0;

            DataTable table = CreatePrintData();
            DataTable errorTable = new DataTable();
            try
            {
                if (!int.TryParse(param, out ConsignSysNo))
                {
                    throw new BizException(string.Format("非法的结算单号{0}", param));
                }

                InitData(ConsignSysNo, table);
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
            //获取代销单i型纳西】
            ConsignSettlementInfo origin = ObjectFactory<ConsignSettlementProcessor>.Instance.LoadConsignSettlementInfo(sysno);
            //获取商家信息
            VendorInfo vendor = ObjectFactory<VendorProcessor>.Instance.LoadVendorInfo(origin.VendorInfo.SysNo.Value);

            DataRow row = null;
            row = table.NewRow();

            row["StatementSysNo"] = sysno;//结算单号
            row["ContractSysNo"] = string.Empty;//合同号
            row["StatementDate"] = DateTime.Now.ToString("yyyy/MM");//结算年月
            row["PrintDate"] = DateTime.Now.ToString();//打印日期

            row["StatementDepartment"] = "商品部";//结算部门

            //本月销售
            decimal amount = 0;
            foreach (var sub in origin.ConsignSettlementItemInfoList)
            {
                amount += sub.ConsignToAccLogInfo.SalePrice.Value * sub.ConsignToAccLogInfo.ProductQuantity.Value;
            }
            row["CurrentMonthMarket"] = amount.ToString("f2");//本月销售

            row["PayDate"] = string.Empty;//付款日

            row["CompanyName1"] = vendor.VendorBasicInfo.VendorNameLocal;//公司名称
            row["CheckType"] = "租赁-001";//核算方式

            row["CompanyAddress"] = vendor.VendorBasicInfo.Address;//公司地址
            row["TaxNo"] = vendor.VendorFinanceInfo.TaxNumber;//税号

            row["OpenedBank"] = vendor.VendorFinanceInfo.BankName;//开户行
            row["BankAccount"] = vendor.VendorFinanceInfo.AccountNumber;//账号

            row["VendorCode"] = vendor == null ? string.Empty : vendor.VendorBasicInfo.VendorID;//供货商代码
            row["VendorSysno"] = vendor == null ? string.Empty : origin.VendorInfo.SysNo.Value.ToString();//供货商系统编号
            row["CreateDate"] = DateTime.Now.ToString();//制单日期

            row["CutPaymentName"] = origin.DeductMethod.HasValue ? (origin.DeductMethod == DeductMethod.Cash ? "现金" : "货扣") : "";//扣款名称
            row["CutPaymentAmount"] = origin.DeductMethod.HasValue ? "￥" + origin.DeductAmt.ToString("f2") : "￥0.00";//扣款金额
            row["TotalCutPaymentAmount"] = origin.DeductMethod.HasValue ? "￥" + origin.DeductAmt.ToString("f2") : "￥0.00";//合计

            //类型
            var taxRateData = origin.TaxRateData;
            Func<PurchaseOrderTaxRate, decimal, decimal> Shuijin = (a, b) =>
            {
                return ((decimal)(((int)a) / 100.00)) * b / ((decimal)(((int)a) / 100.00) + 1);
            };
            Func<PurchaseOrderTaxRate, decimal, decimal> Jiakuan = (a, b) =>
            {
                return b / ((decimal)(((int)a) / 100.00) + 1);
            };

            string DefaultMony = ((int)0).ToString("f2");

            row["Cost"] = DefaultMony;//价款
            row["RateAmount"] = DefaultMony;//税金
            row["RateTotal"] = DefaultMony;//税金合计

            row["17RateCost"] = DefaultMony;//17%税率价款
            row["17RateAmount"] = DefaultMony;//税金
            row["17RateTotal"] = DefaultMony;//税金合计

            row["13RateCost"] = DefaultMony;//13%税率价款
            row["13RateAmount"] = DefaultMony;//税金
            row["13RateTotal"] = DefaultMony;//税金合计

            row["OtherRateCost"] = DefaultMony;//其他税率价款
            row["OtherRateAmount"] = DefaultMony;//税金
            row["OtherRateTotal"] = DefaultMony;//税金合计

            decimal jiakuan = Jiakuan(taxRateData.Value, origin.TotalAmt.Value);
            string jiakuanStr = jiakuan.ToString("f2");
            string jieSuam = Shuijin(taxRateData.Value, origin.TotalAmt.Value).ToString("f2");
            string zongJine = origin.TotalAmt.Value.ToString("f2");

            row["Cost"] = jiakuanStr;//价款
            row["RateAmount"] = jieSuam;//税金
            row["RateTotal"] = zongJine;//税金合计

            if (taxRateData == PurchaseOrderTaxRate.Percent017)
            {
                row["17RateCost"] = jiakuanStr;//17%税率价款
                row["17RateAmount"] = jieSuam;//税金
                row["17RateTotal"] = zongJine;//税金合计
            }
            else if (taxRateData == PurchaseOrderTaxRate.Percent013)
            {
                row["13RateCost"] = jiakuanStr;//13%税率价款
                row["13RateAmount"] = jieSuam;//税金
                row["13RateTotal"] = zongJine;//税金合计
            }
            else
            {
                row["OtherRateCost"] = jiakuanStr;//其他税率价款
                row["OtherRateAmount"] = jieSuam;//税金
                row["OtherRateTotal"] = zongJine;//税金合计
            }

            row["ActualPayAmount"] = origin.TotalAmt.HasValue ? origin.TotalAmt.Value.ToString("f2") : "0.00";//实际付款金额

            row["CompanyName2"] = "网购中心";//公司名称
            row["Producer"] = string.Empty;//制作人

            row["BusinessCheck"] = string.Empty;//业务审核
            row["FinanceRecheck"] = string.Empty;//财务复核

            row["DepartmentManager"] = string.Empty;//部门经理
            row["Manufacturer"] = string.Empty;//厂商

            row["display"] = "";

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
