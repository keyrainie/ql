using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using MerchantCommissionSettle.Entities;
using MerchantCommissionSettle.Components;
using System.Data;

namespace MerchantCommissionSettle.DataAccess
{
    internal sealed class CommissionDA
    {
        public void SendEmail(string mailAddress, string mailSubject, string mailBody, int status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", mailAddress);
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailBody);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);

            command.ExecuteNonQuery();
        }

        public CommissionMaster GetCommissionMasterBySysNo(DateTime startTime, DateTime endTime)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionMasterBySysNo");
            command.SetParameterValue("@SysNo", endTime);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteEntity<CommissionMaster>();
        }

        public int UpdateInvoiceMasterCommissionStatus(int sysNo,string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInvoiceMasterCommissionStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", status);            
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public int UpdateRmaRefundCommissionStatus(int sysNo, string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRmaRefundCommissionStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public List<CommissionRule> GetCommissionRules()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionRules");

            return command.ExecuteEntityList<CommissionRule>();
        }

        public List<CommissionRule> GetCommissionRulesByMerchantSysNo(int merchantSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionRulesByMerchantSysNo");

            command.SetParameterValue("@MerchantSysNo", merchantSysNo);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteEntityList<CommissionRule>();
        }

        public CommissionMaster GetCommissionMasterByMerchantSysNo(int merchantSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionMasterByMerchantSysNo");

            command.SetParameterValue("@MerchantSysNo", merchantSysNo);
            command.SetParameterValue("@CompanyCode",GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);
            command.SetParameterValue("@LanguageCode", GlobalSettings.LanguageCode);
            command.SetParameterValue("@CurrencyCode", GlobalSettings.CurrencyCode);
            command.SetParameterValue("@InUser", GlobalSettings.UserName);

            return command.ExecuteEntity<CommissionMaster>();
        }

        public CommissionItem GetCommissionItemByVMSysNo(int vendorManufacturerSysNo,int masterSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommissionItemByVMSysNo");

            command.SetParameterValue("@VendorManufacturerSysNo", vendorManufacturerSysNo);
            command.SetParameterValue("@CommissionMasterSysNo", masterSysNo);
            command.SetParameterValue("@InUser", GlobalSettings.UserName);
            command.SetParameterValue("@EditUser", GlobalSettings.UserName);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);
            command.SetParameterValue("@CurrencyCode", GlobalSettings.CurrencyCode);
            command.SetParameterValue("@LanguageCode", GlobalSettings.LanguageCode);

            return command.ExecuteEntity<CommissionItem>();
        }

        public int CreateCommissionLog(CommissionLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateCommissionLog");
            command.CommandTimeout = 120;

            command.SetParameterValue("@CommissionItemSysNo", log.CommissionItemSysNo);
            command.SetParameterValue("@ReferenceSysNo", log.ReferenceSysNo);
            command.SetParameterValue("@ReferenceType", log.ReferenceType);



            if (log.PromotionDiscount == 0)
            {
                command.SetParameterValue("@PromotionDiscount", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@PromotionDiscount", log.PromotionDiscount);
            }

            if (log.ProductSysNo == 0)
            {
                command.SetParameterValue("@ProductSysNo", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@ProductSysNo", log.ProductSysNo);
            }

            if (log.Price == 0)
            {
                command.SetParameterValue("@Price", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@Price", log.Price);
            }

            if (log.Qty == 0)
            {
                command.SetParameterValue("@Qty", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@Qty", log.Qty);
            }

            command.SetParameterValue("@Type", log.Type);
            command.SetParameterValue("@InUser", log.InUser);
            command.SetParameterValue("@EditUser", log.EditUser);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", GlobalSettings.StoreCompanyCode);
            command.SetParameterValue("@LanguageCode", GlobalSettings.LanguageCode);
            command.SetParameterValue("@CurrencyCode", GlobalSettings.CurrencyCode);

            return command.ExecuteNonQuery();
        }

        public List<CommissionLog> GetCommissionLog(List<int> payTermsSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommission_Log1");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "MerchantSysNo"))
            {
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "v.PayPeriodType", DbType.Int32, payTermsSysNo);
                command.CommandText = builder.BuildQuerySql();
            }
            
            command.AddInputParameter("@CompanyCode",DbType.AnsiStringFixedLength,GlobalSettings.CompanyCode);

            return command.ExecuteEntityList<CommissionLog>();
        }

        public List<CommissionLog> GetExistingCommissionLog()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCommission_Log2");
            command.CommandTimeout = 120;

            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteEntityList<CommissionLog>();
        }

        public Int32 UpdateCommissionItem(CommissionItem item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCommissionItem");

            command.SetParameterValue("@SysNo", item.SysNo);
            command.SetParameterValue("@CommissionMasterSysNo", item.CommissionMasterSysNo);
            command.SetParameterValue("@VendorManufacturerSysNo", item.VendorManufacturerSysNo);
            command.SetParameterValue("@RuleSysNo", item.RuleSysNo);
            command.SetParameterValue("@RentFee", item.Rent);
            command.SetParameterValue("@DeliveryFee", item.DeliveryFee);
            command.SetParameterValue("@SaleCommissionAmt", item.SalesCommissionFee);
            command.SetParameterValue("@OrderCommissionAmt", item.OrderCommissionFee);
            command.SetParameterValue("@TotalSaleAmt", item.TotalSaleAmt);
            command.SetParameterValue("@EditUser", item.EditUser);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public Int32 UpdateCommissionLog(CommissionLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCommissionLog");

            command.SetParameterValue("@SysNo", log.SysNo);
            command.SetParameterValue("@CommissionItemSysNo", log.CommissionItemSysNo);
            command.SetParameterValue("@Type", log.Type);
            command.SetParameterValue("@ReferenceSysNo", log.ReferenceSysNo);
            command.SetParameterValue("@ReferenceType", log.ReferenceType);

            if (log.PromotionDiscount == 0)
            {
                command.SetParameterValue("@PromotionDiscount", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@PromotionDiscount", log.PromotionDiscount);
            }

            if (log.ProductSysNo == 0)
            {
                command.SetParameterValue("@ProductSysNo",DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@ProductSysNo", log.ProductSysNo);
            }

            if (log.Price == 0)
            {
                command.SetParameterValue("@Price", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@Price", log.Price);
            }

            if (log.Qty == 0)
            {
                command.SetParameterValue("@Qty", DBNull.Value);
            }
            else
            {
                command.SetParameterValue("@Qty", log.Qty);
            }

            command.SetParameterValue("@EditUser", log.EditUser);

            return command.ExecuteNonQuery();
        }

        public Int32 UpdateCommissionMaster(CommissionMaster item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCommissionMaster");

            command.SetParameterValue("@SysNo", item.SysNo);
            command.SetParameterValue("@MerchantSysNo", item.MerchantSysNo);
            command.SetParameterValue("@Status", item.Status);
            command.SetParameterValue("@TotalAmt", item.TotalAmt);
            command.SetParameterValue("@RentFee", item.RentFee);
            command.SetParameterValue("@DeliveryFee", item.DeliveryFee);
            command.SetParameterValue("@SalesCommissionFee", item.SalesCommissionFee);
            command.SetParameterValue("@OrderCommissionFee", item.OrderCommissionFee);
            command.SetParameterValue("@BeginDate", item.BeginDate);
            command.SetParameterValue("@EndDate", item.EndDate);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public Int32 SettleCommission(CommissionMaster item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CloseCommissionMaster");

            command.SetParameterValue("@SysNo", item.SysNo);
            command.SetParameterValue("@Status", item.Status);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public List<Vendor> GetVendorByPayPeriodType(List<int> types)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorByPayPeriodType");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "PayPeriodType", DbType.Int32, types);

                command.CommandText = sqlBuilder.BuildQuerySql();
            }

            return command.ExecuteEntityList<Vendor>();
        }

        public Vendor GetVendorBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorBySysNo");

            command.SetParameterValue("@SysNo", sysNo);            
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteEntity<Vendor>();
        }

        public Int32 DeleteCommissionLog(int itemSysNo, string type)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteCommissionLog");

            command.SetParameterValue("@CommissionItemSysNo", itemSysNo);
            command.SetParameterValue("@Type", type);            
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public CommissionLog GetOrderCommissionLog(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetOrderCommissionLog");

            command.SetParameterValue("@SoSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", GlobalSettings.CompanyCode);

            return command.ExecuteEntity<CommissionLog>();
        }

        #region 增加手动结算供应商的运算
        public List<CommissionLog> GetCommissionLog(List<int> payTermsSysNo, int vendorSysno, DateTime maxEndDate)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommission_Log1");
            command.CommandTimeout = 120;

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "MerchantSysNo"))
            {
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "v.PayPeriodType", DbType.Int32, payTermsSysNo);

                if (vendorSysno > 0)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.SysNO", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, vendorSysno);
                }
                               
                command.CommandText = builder.BuildQuerySql();
            }

            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, GlobalSettings.CompanyCode);
            command.AddInputParameter("@MaxEndDate", DbType.DateTime, maxEndDate);

            return command.ExecuteEntityList<CommissionLog>();
        }

        public List<CommissionLog> GetExistingCommissionLog(int vendorSysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommission_Log2");
            command.CommandTimeout = 120;
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "MerchantSysNo"))
            {               
                command.CommandText = builder.BuildQuerySql();

                if (vendorSysno > 0)
                {                    
                    command.CommandText = command.CommandText.Replace("/*strWhere*/", " AND c.MerchantSysNo=" + vendorSysno);
                }
            }
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, GlobalSettings.CompanyCode);

            return command.ExecuteEntityList<CommissionLog>();
        }
        #endregion
    }
}
