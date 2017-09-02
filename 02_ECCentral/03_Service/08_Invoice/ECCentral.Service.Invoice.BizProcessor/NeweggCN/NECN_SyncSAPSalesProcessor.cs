using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity.NeweggCN.Invoice.SAP;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(NECN_SyncSAPSalesProcessor))]
    public class NECN_SyncSAPSalesProcessor
    {
        public void SyncSAPSales(DataTable dt, SOIncomeOrderType soIncomeOrderType)
        {
            int orderTypeSysNo = -1;
            string orderType = string.Empty;
            switch (soIncomeOrderType)
            {
                case SOIncomeOrderType.SO:
                    orderTypeSysNo = 2;
                    orderType = EnumHelper.ToEnumDesc(SOIncomeOrderType.SO);
                    break;
                case SOIncomeOrderType.RO:
                    orderTypeSysNo = 4;
                    orderType = EnumHelper.ToEnumDesc(SOIncomeOrderType.RO);
                    break;
                case SOIncomeOrderType.AO:
                    orderTypeSysNo = 7;
                    orderType = EnumHelper.ToEnumDesc(SOIncomeOrderType.AO);
                    break;
                case SOIncomeOrderType.RO_Balance:
                    orderTypeSysNo = 6;
                    orderType = EnumHelper.ToEnumDesc(SOIncomeOrderType.RO_Balance);
                    break;
                case SOIncomeOrderType.OverPayment:
                    orderTypeSysNo = 8;
                    orderType = EnumHelper.ToEnumDesc(SOIncomeOrderType.OverPayment);
                    break;
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = new System.TimeSpan(0, 10, 0);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ObjectFactory<INECN_SyncSAPSalesDA>.Instance.DeleteSAPSales(orderTypeSysNo);
                foreach (DataRow dr in dt.Rows)
                {
                    SAPSalesInfo entity = DataMapper.GetEntity<SAPSalesInfo>(dr);

                    entity.SalesSysNo = Convert.ToString(dr["SysNo"]);
                    entity.CreateTime = Convert.ToDateTime(dr["IncomeTime"]);
                    entity.CreateUserName = !dr.IsNull("IncomeUser") ? Convert.ToString(dr["IncomeUser"]) : "";
                    entity.ConfirmUserName = !dr.IsNull("ConfirmUser") ? Convert.ToString(dr["ConfirmUser"]) : "";
                    entity.PayType = dr.IsNull("PayTypeSysNo") ? new Nullable<int>() : Convert.ToInt32(dr["PayTypeSysNo"]);
                    entity.OrderStatus = Convert.ToString(dr["IncomeStatus"]);
                    entity.ReturnCash = !dr.IsNull("SAPReturnCashAmt") ? Convert.ToDecimal(dr["SAPReturnCashAmt"]) : new Nullable<decimal>();
                    entity.ReturnPoint = !dr.IsNull("SAPReturnPointAmt") ? Convert.ToInt32(dr["SAPReturnPointAmt"]) : new Nullable<int>();
                    entity.ShipPrice = Convert.ToDecimal(dr["ShippingFee"]);
                    entity.PayPrice = Convert.ToDecimal(dr["PackageFee"]);
                    entity.Premium = Convert.ToDecimal(dr["RegisteredFee"]);

                    entity.OrderType = orderType;
                    entity.OrderTypeSysNo = orderTypeSysNo;

                    entity.PrepayAmt = (orderTypeSysNo == 2 && entity.PrepayAmt == 0) ? null : entity.PrepayAmt;
                    entity.SOSysNo = orderTypeSysNo != 7 ? entity.SOSysNo : null;
                    entity.ShipCost = (entity.ShipCost == 0 && orderTypeSysNo != 6) ? (decimal?)null : entity.ShipCost;

                    if (entity.RefundPayTypeSysNo != null)
                    {
                        entity.RefundPayType = EnumHelper.ToEnumDesc((RefundPayType)dr["RefundPayTypeSysNo"]);
                    }

                    ObjectFactory<INECN_SyncSAPSalesDA>.Instance.SyncSAPSales(entity);
                }
                scope.Complete();
            }
        }
    }
}
