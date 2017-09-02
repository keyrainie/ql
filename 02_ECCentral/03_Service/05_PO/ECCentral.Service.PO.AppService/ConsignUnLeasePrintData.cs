using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.AppService
{
    /// <summary>
    /// 代销商品结算
    /// </summary>
     [VersionExport(typeof(ConsignUnLeasePrintData))]
    public class ConsignUnLeasePrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            //throw new NotImplementedException();

            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            string settleSysNoStr = requestPostData["ConsignSysNo"];

            int settleSysNo = int.Parse(settleSysNoStr);

            //获取代销单i型纳西】
            ConsignSettlementInfo consignSettlementInfo = ObjectFactory<ConsignSettlementProcessor>.Instance.LoadConsignSettlementInfo(settleSysNo);
            //获取商家信息
            VendorInfo VendorInfo = ObjectFactory<VendorProcessor>.Instance.LoadVendorInfo(consignSettlementInfo.VendorInfo.SysNo.Value);
            //获取代销单商品信息
            

            //结算单号
            variables.Add("SettleSysNo", consignSettlementInfo.ReferenceSysNo);
            //合同号   :
            variables.Add("Agreement", "");//空
            //结算年月
            variables.Add("ConsignRange", consignSettlementInfo.ConsignRange);
            //打印日期
            variables.Add("PrintDate", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            //收款单位——全称
            variables.Add("PayeeName", VendorInfo.VendorBasicInfo.VendorBriefName);
            //收款单位-供货商代码
            variables.Add("PayeeNo", VendorInfo.VendorBasicInfo.VendorID);
            //开户行
            variables.Add("PayeeBack", VendorInfo.VendorFinanceInfo.BankName);
            //账号
            variables.Add("PayeeBackCardNo", VendorInfo.VendorFinanceInfo.AccountNumber);

            //付款单位-全称 
            variables.Add("PayerName", "泰隆优选");
            //开户行
            variables.Add("PayerBack", "");//空
            //账号
            variables.Add("PayerBackCardNo", "");//空
            //付款部门
            variables.Add("PayerDepartment", "");//空


            string DefaultMony = ((int)0).ToString("C");

            //供应商提供税票
            //价款
            variables.Add("Cost", DefaultMony);
            //税金
            variables.Add("Tax", DefaultMony);
            //价款税金合计
            variables.Add("TaxAndCost", DefaultMony);

            //17
            //价款
            variables.Add("17Cost", DefaultMony);
            //税金
            variables.Add("17Tax", DefaultMony);
            //价款税金合计
            variables.Add("17TaxAndCost", DefaultMony);

            //13
            //价款
            variables.Add("13Cost", DefaultMony);
            //税金
            variables.Add("13Tax", DefaultMony);
            //价款税金合计
            variables.Add("13TaxAndCost", DefaultMony);

            //other
            //价款
            variables.Add("OtCost", DefaultMony);
            //税金
            variables.Add("OtTax", DefaultMony);
            //价款税金合计
            variables.Add("OtTaxAndCost", DefaultMony);

            //类型
            var taxRateData = consignSettlementInfo.TaxRateData;

            
            Func<PurchaseOrderTaxRate, decimal, decimal> Shuijin = (a,b) => {
                return ((decimal)(((int)a) / 100.00)) * b / ((decimal)(((int)a) / 100.00) + 1);
            };

            Func<PurchaseOrderTaxRate, decimal, decimal> Jiakuan = (a, b) =>
            {
                return b / ((decimal)(((int)a) / 100.00) + 1);
            };

            decimal jiakuan = Jiakuan(taxRateData.Value, consignSettlementInfo.TotalAmt.Value);
            string jiakuanStr = jiakuan.ToString("C");
            string jieSuam = Shuijin(taxRateData.Value, consignSettlementInfo.TotalAmt.Value).ToString("C");
            string zongJine = consignSettlementInfo.TotalAmt.Value.ToString("C");

            variables["Cost"] = jiakuanStr;
            variables["Tax"]=jieSuam;
            variables["TaxAndCost"] = zongJine;

            if (taxRateData == PurchaseOrderTaxRate.Percent017)
            {
                variables["17Cost"] = jiakuanStr;
                variables["17Tax"] =jieSuam;
                variables["17TaxAndCost"] = zongJine;
            }
            else if (taxRateData == PurchaseOrderTaxRate.Percent013)
            {
                variables["13Cost"] = jiakuanStr;
                variables["13Tax"] = jieSuam;
                variables["13TaxAndCost"] = zongJine;
            }
            else 
            {
                variables["OtCost"] = jiakuanStr;
                variables["OtTax"] = jieSuam;
                variables["OtTaxAndCost"] = zongJine;
            }

           var deduct = ObjectFactory<DeductProcessor>.Instance.GetSingleDeductBySysNo(VendorInfo.VendorDeductInfo.DeductSysNo.ToString());
           if (deduct == null)
           {
               deduct = new BizEntity.PO.PurchaseOrder.Deduct();
               deduct.DeductType = DeductType.Temp;
           }
           string deductTypeStr = EnumHelper.GetDescription(deduct.DeductType, typeof(DeductType));
            //扣款项目
           variables.Add("DeducType", deductTypeStr);  //****

            //扣款金额
           variables.Add("DeducMoney", consignSettlementInfo.DeductAmt.ToString("C"));
            //备注
            variables.Add("Memo", "");//空

            //扣款合计：
            variables.Add("DeducMoneySum", consignSettlementInfo.DeductAmt.ToString("C"));

            //本月销售
            decimal amount = 0;
            foreach (var sub in consignSettlementInfo.ConsignSettlementItemInfoList)
            {
                amount += sub.ConsignToAccLogInfo.SalePrice.Value * sub.ConsignToAccLogInfo.ProductQuantity.Value;
            }
            variables.Add("CurrentSale", amount.ToString("C"));
            //实际付款金额
            variables.Add("RealPayMent", consignSettlementInfo.TotalAmt.Value.ToString("C"));

            //业务复核
            variables.Add("BussinessRechecker", consignSettlementInfo.AuditUser.UserName);
        }
    }
}
