using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {

            EnumCodeMapper.AddMap<ECCentral.BizEntity.PO.VendorInvoiceType>(new Dictionary<ECCentral.BizEntity.PO.VendorInvoiceType, string> {
                { ECCentral.BizEntity.PO.VendorInvoiceType.NEG, "NEG"},
                { ECCentral.BizEntity.PO.VendorInvoiceType.MET, "MET"},
                { ECCentral.BizEntity.PO.VendorInvoiceType.GUD, "GUD"}
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.PO.VendorShippingType>(new Dictionary<ECCentral.BizEntity.PO.VendorShippingType, string> {
                { ECCentral.BizEntity.PO.VendorShippingType.NEG, "NEG"},
                { ECCentral.BizEntity.PO.VendorShippingType.MET, "MET"}
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.PO.VendorStockType>(new Dictionary<ECCentral.BizEntity.PO.VendorStockType, string> {
                { ECCentral.BizEntity.PO.VendorStockType.NEG, "NEG"},
                { ECCentral.BizEntity.PO.VendorStockType.MET, "MET"},
                { ECCentral.BizEntity.PO.VendorStockType.NAM, "NAM"}
            });

            EnumCodeMapper.AddMap<GatherSettleStatus>(new Dictionary<GatherSettleStatus, string>{
                { GatherSettleStatus.ABD, "ABD" },
                { GatherSettleStatus.ORG, "ORG" },
                { GatherSettleStatus.AUD, "AUD" },
                { GatherSettleStatus.SET, "SET" }
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.PO.ValidStatus>(new Dictionary<ECCentral.BizEntity.PO.ValidStatus, int> {
                { ECCentral.BizEntity.PO.ValidStatus.D, -1},
                { ECCentral.BizEntity.PO.ValidStatus.A, 0 }
            });

            EnumCodeMapper.AddMap<ConsignToAccountLogStatus>(new Dictionary<ConsignToAccountLogStatus, string>
            {
                    { ConsignToAccountLogStatus.Origin,"A"},
                    {ConsignToAccountLogStatus.SystemCreated, "S"},
                    { ConsignToAccountLogStatus.ManualCreated, "C"},
                    { ConsignToAccountLogStatus.Settled, "F"}

            });
            EnumCodeMapper.AddMap<SettleType>(new Dictionary<SettleType, string>
            {
                    { SettleType.O,"O"},
                    {SettleType.P, "P"}
            });

            EnumCodeMapper.AddMap<ConsignToAccountType>(new Dictionary<ConsignToAccountType, string>
            {
                    { ConsignToAccountType.Adjust,"AD"},
                    { ConsignToAccountType.Manual,"RMA"},
                    { ConsignToAccountType.RMA,"OR"},
                    { ConsignToAccountType.SO,"SO"}
            });
            EnumCodeMapper.AddExtraCodeMap(ConsignToAccountType.Manual, new object[] { "IT" });

            EnumCodeMapper.AddMap<PurchaseOrderETAHalfDayType>(new Dictionary<PurchaseOrderETAHalfDayType, string> {
                { PurchaseOrderETAHalfDayType.AM, "AM"},
                { PurchaseOrderETAHalfDayType.PM, "PM" }
            });

            EnumCodeMapper.AddMap<PurchaseOrderTaxRate>(new Dictionary<PurchaseOrderTaxRate, int> {
                { PurchaseOrderTaxRate.Percent000, 0},
                { PurchaseOrderTaxRate.Percent004, 4 },
                { PurchaseOrderTaxRate.Percent006, 6 },
                { PurchaseOrderTaxRate.Percent013, 13 },
                { PurchaseOrderTaxRate.Percent017, 17 }
            });
            EnumCodeMapper.AddMap<YNStatus>(new Dictionary<YNStatus, string>
            {
                {YNStatus.Yes,"Y"},
                {YNStatus.NO,"N"}
            });
            EnumCodeMapper.AddMap<VendorCommissionItemType>(new Dictionary<VendorCommissionItemType, string>
            {
                      { VendorCommissionItemType.DEF,"DEF"},
                      { VendorCommissionItemType.SOC,"SOC"},
                      { VendorCommissionItemType.SAC,"SAC"}
            });
            EnumCodeMapper.AddMap<VendorCommissionReferenceType>(new Dictionary<VendorCommissionReferenceType, string>
            {
                      { VendorCommissionReferenceType.SO,"SO"},
                      { VendorCommissionReferenceType.RMA,"RMA"}
            });

            EnumCodeMapper.AddMap<VendorRank>(new Dictionary<VendorRank, string>
            {
                      { VendorRank.A,"A"},
                      { VendorRank.B,"B"},
                      { VendorRank.C,"C"}
            });

            EnumCodeMapper.AddMap<ConsignSettleRuleStatus>(new Dictionary<ConsignSettleRuleStatus, string>
            {
                      { ConsignSettleRuleStatus.Wait_Audit,"O"},
                      { ConsignSettleRuleStatus.Stop,"S"},
                      { ConsignSettleRuleStatus.Forbid,"F"},
                      { ConsignSettleRuleStatus.Enable,"E"},
                      { ConsignSettleRuleStatus.Disable,"D"},
                      { ConsignSettleRuleStatus.Available,"A"}
            });
            EnumCodeMapper.AddMap<PurchaseOrderBatchInfoStatus>(new Dictionary<PurchaseOrderBatchInfoStatus, string>
            {
                      { PurchaseOrderBatchInfoStatus.A,"A"},
                      { PurchaseOrderBatchInfoStatus.I,"I"},
                      { PurchaseOrderBatchInfoStatus.R,"R"}
            });

        }
    }
}
