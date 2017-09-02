using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.PO.AppService
{
    /// <summary>
    /// 代销商品结算
    /// </summary>
     [VersionExport(typeof(SettleProductPrintData))]
    public class SettleProductPrintData : IPrintDataBuild
    {
        public void BuildData(System.Collections.Specialized.NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            string param = requestPostData["SettleSysNo"];
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
            tableVariables.Add("SettleProductPrint", table);
        }

        private void InitData(int sysno, DataTable table)
        {
            SettleInfo settleInfo = new SettleInfo();
            settleInfo.SysNo = sysno;
            settleInfo = ObjectFactory<ConsignSettlementProcessor>.Instance.GetSettleAccount(settleInfo);
            //获取商家信息
            VendorInfo VendorInfo = ObjectFactory<VendorProcessor>.Instance.LoadVendorInfo(settleInfo.VendorSysNo.Value);

            DataRow row = null;
            row = table.NewRow();

            string DefaultMony = ((int)0).ToString("C");

            row["SettleSysNo"] = sysno;//结算单号
            row["Agreement"] = string.Empty;//合同号
            row["ConsignRange"] = DateTime.Now.ToString("yyyy/MM");//结算年月
            row["PrintDate"] = DateTime.Now.ToString();//打印日期

            row["PayeeName"] = VendorInfo.VendorFinanceInfo.AccountContact;//全称
            row["PayeeNo"] = VendorInfo.VendorBasicInfo.VendorID;//供货商代码
            row["PayeeBack"] = VendorInfo.VendorFinanceInfo.BankName;//开户行
            row["PayeeBackCardNo"] = VendorInfo.VendorFinanceInfo.AccountNumber;//账号

            row["PayerName"] = "泰隆优选";//全称
            row["PayerDepartment"] = string.Empty;//付款部门
            row["PayerBack"] = string.Empty;//开户行
            row["PayerBackCardNo"] = string.Empty;//账号

            //应付税金
            decimal RateAmount = 0;

            //应付价税合计
            decimal RateTotal = 0;

            //税金(13%)
            decimal RateCost13 = 0;

            //税金(17%)
            decimal RateCost17 = 0;

            // 税金(其他)
            decimal RateAmountOther = 0;

            //价款(17%)
            decimal Cost17 = 0;

            //价款(13%)
            decimal Cost13 = 0;

            //价款(其它)
            decimal CostOther = 0;

            List<SettleItemInfo> itemList = settleInfo.SettleItemInfos;
            if (itemList != null)
            {
                itemList.ForEach(x =>
                {
                    Cost13 += PointTwo(x.Cost13);
                    Cost17 += PointTwo(x.Cost17);
                    CostOther += PointTwo(x.CostOther);

                    RateCost13 += PointTwo(x.Rate13);
                    RateCost17 += PointTwo(x.Rate17);
                    RateAmountOther += PointTwo(x.RateOther);
                });
            }

            row["Cost"] = PointTwo((Cost13 + Cost17 + CostOther)).ToString("C");//价款
            row["Tax"] = PointTwo((RateCost13 + RateCost17 + RateAmountOther)).ToString("C");//税金
            row["TaxAndCost"] = PointTwo((Cost13 + Cost17 + CostOther + RateCost13 + RateCost17 + RateAmountOther)).ToString("C");//价税合计

            row["17Cost"] = Cost17.ToString("C");//17%税率价款
            row["17Tax"] = RateCost17.ToString("C");//税金
            row["17TaxAndCost"] = PointTwo((Cost17 + RateCost17)).ToString("C");//17价税

            row["13Cost"] = Cost13.ToString("C");//13%价款
            row["13Tax"] = RateCost13.ToString("C");//税金
            row["13TaxAndCost"] = PointTwo((Cost13 + RateCost13)).ToString("C");//税金合计

            row["OtCost"] = CostOther.ToString("C");//其他价款
            row["OtTax"] = RateAmountOther.ToString("C");//其他税金
            row["OtTaxAndCost"] = PointTwo((CostOther + RateAmountOther)).ToString("C");//其他价税

            row["DeducMoneySum"] = DefaultMony;//扣款合计
            row["RealPayMent"] = row["TaxAndCost"];//实际付款金额

            if (settleInfo.CreateUserSysNo.HasValue)
            {
                UserInfo userInfo = ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(settleInfo.CreateUserSysNo.Value);
                if (userInfo != null)
                    row["BussinessRechecker"] = userInfo.UserName;//制单人
            }

            if (settleInfo.AuditUserSysNo.HasValue)
            {
                UserInfo userInfo = ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(settleInfo.AuditUserSysNo.Value);
                if (userInfo != null)
                    row["Bussinesssh"] = userInfo.UserName;//审核人
            }

            //发票明细
            DataTable itemTable = CreateItemsPrintData();
            InitItemData(settleInfo.SettleItemInfos, itemTable);
            row["Items"] = itemTable;

            table.Rows.Add(row);
        }

        private decimal PointTwo(decimal item)
        {
            return Math.Round(item, 2, MidpointRounding.AwayFromZero);
        }

        private void InitItemData(List<SettleItemInfo> items, DataTable table)
        {
            int rowCount = items.Count;
            DataRow row = null;
            SettleItemInfo item = null;
            for (int i = 0; i < rowCount; i++)
            {
                item = items[i];
                row = table.NewRow();
                row["SettleType"] = GetTypeStr(item.OrderType);
                row["SettleItemSysno"] = item.OrderSysNo;
                row["CostDetail"] = (item.Cost13 + item.Cost17 + item.CostOther).ToString("C");
                row["RateAmountDetail"] = (item.Rate13 + item.Rate17 + item.RateOther).ToString("C");
                row["RateTotalDetail"] = (item.Cost13 + item.Cost17 + item.CostOther + item.Rate13 + item.Rate17 + item.RateOther).ToString("C");
                table.Rows.Add(row);
            }
        }

        private string GetTypeStr(int? orderType)
        {
            if (orderType.HasValue)
            {
                switch (orderType)
                {
                    case 3: return "进货单";
                    case 5: return "返厂单";
                    case 7: return "进价变价单";
                    default: return "";
                }
            }
            else
            {
                return "";
            }
        }

        private DataTable CreateItemsPrintData()
        {
            DataTable table = new DataTable();
            table.Columns.Add("SettleType");
            table.Columns.Add("SettleItemSysno");
            table.Columns.Add("CostDetail");
            table.Columns.Add("RateAmountDetail");
            table.Columns.Add("RateTotalDetail");
            return table;
        }

        private DataTable CreatePrintData()
        {
            DataTable table = new DataTable();

            table.Columns.Add("SettleSysNo");//结算单号
            table.Columns.Add("Agreement");//合同号
            table.Columns.Add("ConsignRange");//结算年月
            table.Columns.Add("PrintDate");//打印日期

            table.Columns.Add("PayeeName");//全称
            table.Columns.Add("PayeeNo");//供货商代码
            table.Columns.Add("PayeeBack");//开户行
            table.Columns.Add("PayeeBackCardNo");//账号

            table.Columns.Add("PayerName");//全称
            table.Columns.Add("PayerDepartment");//付款部门
            table.Columns.Add("PayerBack");//开户行
            table.Columns.Add("PayerBackCardNo");//账号

            table.Columns.Add("Cost");//价款
            table.Columns.Add("Tax");//税金
            table.Columns.Add("TaxAndCost");//价税合计

            table.Columns.Add("17Cost");//17%税率价款
            table.Columns.Add("17Tax");//税金
            table.Columns.Add("17TaxAndCost");//17价税

            table.Columns.Add("13Cost");//13%价款
            table.Columns.Add("13Tax");//税金
            table.Columns.Add("13TaxAndCost");//税金合计

            table.Columns.Add("OtCost");//其他价款
            table.Columns.Add("OtTax");//其他税金
            table.Columns.Add("OtTaxAndCost");//其他价税

            table.Columns.Add("DeducMoneySum");//扣款合计
            table.Columns.Add("RealPayMent");//实际付款金额


            table.Columns.Add("BussinessRechecker");//制单人
            table.Columns.Add("Bussinesssh");//审核人

            table.Columns.Add("Items", typeof(DataTable));

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
