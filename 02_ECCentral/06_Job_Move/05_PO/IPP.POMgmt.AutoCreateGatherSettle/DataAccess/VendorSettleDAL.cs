using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using POASNMgmt.AutoCreateVendorSettle.Compoents;
using POASNMgmt.AutoCreateVendorSettle.Entities;
using System.Data;
using POASNMgmt.AutoCreateGatherSettle.DataAccess;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateVendorSettle.DataAccess
{
    public class VendorSettleDAL
    {
        public void SendEmail(string mailAddress, string mailSubject, string mailBody, int status,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", mailAddress);
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailBody);
            command.SetParameterValue("@Staues", status);
            command.SetParameterValue("@CompanyCode", companyCode);

            command.ExecuteNonQuery();
        }

        public List<GatherSettleInfo> GetConsginToAccLogList(List<int> payPeriodTypes)
        {
            if (payPeriodTypes == null || payPeriodTypes.Count == 0)
            {
                return new List<GatherSettleInfo>();
            }

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetGatherSettleList");
            command.CommandTimeout = 120;

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
            {
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.String, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                command.CommandText = builder.BuildQuerySql();
                return command.ExecuteEntityList<GatherSettleInfo>();
            }
        }

        public List<GatherSettleInfo> GetConsginToAccLogList(List<int> payPeriodTypes, DateTime maxOrderEndData, int merchantSysNo)
        {
            if (payPeriodTypes == null || payPeriodTypes.Count == 0)
            {
                return new List<GatherSettleInfo>();
            }

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetGatherSettleList");
            command.CommandTimeout = 120;

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
            {
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);                             
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.String, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);

                if (merchantSysNo > 0)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, merchantSysNo);
                }

                command.CommandText = builder.BuildQuerySql();

                command.CommandText = command.CommandText.Replace("/*#RelaceWhere#*/", " Where OutOrRefundDateTime < '" + maxOrderEndData.ToString() + "'");

                return command.ExecuteEntityList<GatherSettleInfo>();
            }
        }

        //public VendorSettleGatherMsg Create(VendorSettleGatherMsg entity, List<int> payPeriodTypes, List<int> soList)
        public GatherSettlementInfo Create(GatherSettlementInfo entity, List<int> payPeriodTypes, List<int> soList)
        
        {
    
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorSettleGather");

            #region
            command.SetParameterValue("@MerchantSysNo", entity.VendorSysNo);    //@@
            command.SetParameterValue("@StockSysNo", entity.StockSysNo);        //@@
            //command.SetParameterValue("@MerchantSysNo", entity.VendorInfo.SysNo);
            command.SetParameterValue("@MerchantSysNo", entity.VendorSysNo);
            //command.SetParameterValue("@StockSysNo", entity.SourceStockInfo.SysNo);
            command.SetParameterValue("@StockSysNo", entity.StockSysNo);
            #endregion

            command.SetParameterValue("@TotalAmt", entity.TotalAmt);
            command.SetParameterValue("@InUser", GlobalSettings.UserName);
            command.SetParameterValue("@InDate", DateTime.Now);
            command.SetParameterValue("@EditUser", GlobalSettings.UserName);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@CurrencySysNo", "CNY");
            command.SetParameterValue("@Status", (int)entity.Status);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);
            command.SetParameterValue("@Memo", entity.Memo);
            ////////////////////////////////////////////////////////////////////
           
            entity.SysNo = System.Convert.ToInt32(command.ExecuteScalar());
           ///////////////////////////////////////////////////////////////////////
            
            //@@if (entity.SettleGatherItems != null && entity.SettleGatherItems.Count > 0)
            if (entity.GatherSettlementItemInfoList != null && entity.GatherSettlementItemInfoList.Count > 0)
            {
                List<string> listString = new List<string>();
                int k = 0;
                
                foreach (int soNumber in soList)
                {
                    if (k % 500 == 0)
                    {
                        listString.Add(soNumber.ToString());
                    }
                    else
                    {
                        listString[k / 500] = listString[k / 500] + "," + soNumber.ToString();
                    }
                    k++;
                }

                foreach (string solist in listString)
                {
                    CreateItems(entity, payPeriodTypes, solist);
                }
                //////////////////////////////////////////////////////////////////////////////////
                UpdateTotal(entity);
            }
            return entity;
        }

        private void UpdateTotal(GatherSettlementInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettleGatherToAmt");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.ExecuteNonQuery();
        }

        private int CreateItems(GatherSettlementInfo entity, List<int> payPeriodTypes, string soNumberList)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InnertGatherSettleItemsList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
            {
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.String, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                command.CommandText = builder.BuildQuerySql();
            }
            command.CommandText = command.CommandText.Replace("#SettlementSysNo#", entity.SysNo.Value.ToString())
                                                     .Replace("#WarehouseNumber#", entity.StockSysNo.Value.ToString())
                                                     //.Replace("#WarehouseNumber#", entity.SourceStockInfo.SysNo.Value.ToString())
                                                     .Replace("#SONumber#", soNumberList)
                                                     .Replace("#VendorSysno#", entity.VendorSysNo.Value.ToString());
                                                     //.Replace("#VendorSysno#", entity.VendorInfo.SysNo.Value.ToString());
           return command.ExecuteNonQuery();
        }

        //private SettleGatherItemMsg CreateSettleItem(SettleGatherItemMsg entity)
        private GatherSettlementItemInfo CreateSettleItem(GatherSettlementItemInfo entity)
        {
            DataCommand command = null;
            command = DataCommandManager.GetDataCommand("CreateVendorSettleGatherItem");
            command.SetParameterValue("@OrderNo", entity.SONumber);
            command.SetParameterValue("@OrderType", entity.SettleType);
            command.SetParameterValue("@SettleSysNo", entity.SettleSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@Qty", entity.Quantity);
            command.SetParameterValue("@SalesAmt", entity.OriginalPrice);
            command.SetParameterValue("@InUser", GlobalSettings.UserName);
            command.SetParameterValue("@InDate", DateTime.Now);
            command.SetParameterValue("@EditUser", GlobalSettings.UserName);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@ItemType", entity.ProductSysNo.Value <= 0 ? "SHP" : "PRD");
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);
            System.Convert.ToInt32(command.ExecuteNonQuery());
            return entity;
        }
    }
}
