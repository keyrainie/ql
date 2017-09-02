using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.AppService
{
    /*
     * 变价单打印
     * 
     */
    [VersionExport(typeof(CostChangePrintData))]
    public class CostChangePrintData:IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            CostChangeAppService CostChangeAppServiceInstance= ObjectFactory<CostChangeAppService>.Instance;

            //获取参数
            string costChangeSysNoStr = requestPostData["CostChangeSysNo"];
            int costChangeSysNo = int.Parse(costChangeSysNoStr);

            CostChangeInfo costChangeInfo = CostChangeAppServiceInstance.LoadCostChangeInfo(costChangeSysNo);
            //变价单编号
            variables.AddKeyValue("CostChangeinfoSysNo", costChangeInfo.SysNo.Value.ToString());
            //合同号
            variables.AddKeyValue("ContractNo", "");

            //供应商
            VendorAppService VendorAppServiceInstance = ObjectFactory<VendorAppService>.Instance;

            var vendorNo = costChangeInfo.CostChangeBasicInfo.VendorSysNo;
            VendorInfo vendorInfo = VendorAppServiceInstance.LoadVendorInfoBySysNo(vendorNo);
            var vendorName = vendorInfo.VendorBasicInfo.VendorNameLocal;
            variables.AddKeyValue("VendorName", vendorName);

            //部门
            variables.AddKeyValue("DepartmentName", "商品部");

            //制表时间
            var curentDataStr = DateTime.Now.Date.ToString("yyyy-MM-dd");
            variables.AddKeyValue("CurentDate", curentDataStr);

            DataTable dt = new DataTable();
            //商品ID
            dt.Columns.Add("ProductID");
            //商品名称
            dt.Columns.Add("ProductName");
            //税率
            dt.Columns.Add("TaxRange");
            //变价数量
            dt.Columns.Add("ChangeCount");
            //原进价
            dt.Columns.Add("OldPrice");
            //新进价
            dt.Columns.Add("NewPrice");
            //变价金额
            dt.Columns.Add("ChaneAmount");
            //变价税金
            dt.Columns.Add("ChaneTax");
            //采购单编号
            dt.Columns.Add("OrderNo");
            //备注
            dt.Columns.Add("Memo");

            tableVariables.Add("tableCostChangeDetial", dt);

            Func<PurchaseOrderTaxRate, decimal, decimal> Shuijin = (a, b) =>
            {
                return ((decimal)(((int)a) / 100.00)) * b / ((decimal)(((int)a) / 100.00) + 1);
            };

            Func<PurchaseOrderTaxRate, decimal, decimal> Jiakuan = (a, b) =>
            {
                return b / ((decimal)(((int)a) / 100.00) + 1);
            };


            //采购单实体
            PurchaseOrderAppService purchaseOrderAppServiceInstance = ObjectFactory<PurchaseOrderAppService>.Instance;

            var totalAmount = 0m;
            foreach (var sub in costChangeInfo.CostChangeItems)
            {
                DataRow row = null;
                row = dt.NewRow();

                var taxAndAmount = (sub.NewPrice - sub.OldPrice) * sub.ChangeCount;
                var po = purchaseOrderAppServiceInstance.LoadPurchaseOrderInfo(sub.POSysNo);
                var taxRateType = po.PurchaseOrderBasicInfo.TaxRateType.Value;//增值税类型

                //商品ID
                row["ProductID"] = sub.ProductID;
                //商品名称
                row["ProductName"] = sub.ProductName;
                //税率
                row["TaxRange"] = ((int)taxRateType).ToString();//通过采购单 来获取 税率
                //变价数量
                row["ChangeCount"] = sub.ChangeCount;
                //原进价
                row["OldPrice"] = sub.OldPrice.ToString("C");
                //新进价
                row["NewPrice"] = sub.NewPrice.ToString("C");
                //不含税金额(价款)
                row["ChaneAmount"] = Jiakuan(taxRateType, taxAndAmount).ToString("C");
                //变价税金
                row["ChaneTax"] = Shuijin(taxRateType, taxAndAmount).ToString("C");
                //采购单编号
                row["OrderNo"] = sub.POSysNo;
                //备注
                row["Memo"] = "";

                dt.Rows.Add(row);
                totalAmount += taxAndAmount;
            }
            
            //制单人
            variables.AddKeyValue("CreateUser", costChangeInfo.CostChangeBasicInfo.InUserStr);
            //审核人
            variables.AddKeyValue("AuditUser", costChangeInfo.CostChangeBasicInfo.AuditUserStr);
            //合计
            variables.AddKeyValue("TotalAmount", totalAmount.ToString("C"));

        }
    }
}
