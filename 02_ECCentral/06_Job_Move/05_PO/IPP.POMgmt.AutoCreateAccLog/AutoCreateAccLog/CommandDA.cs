using System;
using System.Collections.Generic;
using System.Configuration;
using Newegg.Oversea.Framework.DataAccess;

namespace AutoCreateAccLog
{
    public class CommandDA
    {
        public static int SendMail(string project, string to, string from, string cc, string body)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendMail");
            command.SetParameterValue("@Subject", project);
            command.SetParameterValue("@To", to);
            command.SetParameterValue("@From", from);
            command.SetParameterValue("@CC", cc);
            command.SetParameterValue("@Body", body);

            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@StoreCompanyCode", ConfigurationManager.AppSettings["StoreCompanyCode"]);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);
            return command.ExecuteNonQuery();
        }

        public static List<ConsignToAccLogEntity> Query(int pageSize)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Query");
            command.SetParameterValue("@PageSize", pageSize);
            return command.ExecuteEntityList<ConsignToAccLogEntity>();
        }

        public static int InsertConsignToAccLog(ConsignToAccLogEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertConsignToAccLog");
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@ConsignToAccType", entity.ConsignToAccType);
            command.SetParameterValue("@CreateCost", entity.CreateCost);
            command.SetParameterValue("@CreateTime", entity.CreateTime);
            command.SetParameterValue("@CurrencySysNo", entity.CurrencySysNo);
            command.SetParameterValue("@FoldCost", entity.FoldCost);
            command.SetParameterValue("@IsConsign", entity.IsConsign);
            command.SetParameterValue("@LanguageCode", entity.LanguageCode);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@OrderSysNo", entity.OrderSysNo);
            command.SetParameterValue("@Point", entity.Point);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@Quantity", entity.Quantity);
            command.SetParameterValue("@RetailPrice", entity.RetailPrice);
            command.SetParameterValue("@SettleCost", entity.SettleCost);
            command.SetParameterValue("@SettlePercentage", entity.SettlePercentage);
            command.SetParameterValue("@SettleType", entity.SettleType);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@StockSysNo", int.Parse(entity.StockSysNo));
            command.SetParameterValue("@StoreCompanyCode", entity.StoreCompanyCode);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);

            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
