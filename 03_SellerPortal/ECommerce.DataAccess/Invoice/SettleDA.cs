using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Invoice;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Invoice
{
    public class SettleDA
    {

        public static DataTable SettleQuery(SettleQueryFilter filter, out int dataCount)
        {
            CustomDataCommand command = DataCommandManager.
CreateCustomDataCommandFromConfig("Invocie_SearchCommissionSettle");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
            command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "SysNo DESC"))
            {


                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SysNo", DbType.String,
                    "@SysNo", QueryConditionOperatorType.LeftLike,
                    filter.SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
             "MerchantSysNo", DbType.String,
             "@MerchantSysNo", QueryConditionOperatorType.LeftLike,
             filter.MerchantSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Type", DbType.AnsiStringFixedLength,
                    "@Type", QueryConditionOperatorType.Equal,
                    filter.Type);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SettledDateTime", DbType.AnsiStringFixedLength,
                    "@SettledBeginDateTime", QueryConditionOperatorType.MoreThanOrEqual,
                    filter.SettledBeginDateTime);


                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SettledDateTime", DbType.AnsiStringFixedLength,
                    "@SettledEndDateTime", QueryConditionOperatorType.LessThanOrEqual,
                    filter.SettledEndDateTime);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
             "IssuingDateTime", DbType.AnsiStringFixedLength,
             "@IssuingBeginDateTime", QueryConditionOperatorType.MoreThanOrEqual,
             filter.IssuingBeginDateTime);


                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "IssuingDateTime", DbType.AnsiStringFixedLength,
                    "@IssuingEndDateTime", QueryConditionOperatorType.LessThanOrEqual,
                    filter.IssuingEndDateTime);

                command.CommandText = builder.BuildQuerySql();

                string typeStr = string.Empty;
                if (filter.Type != null)
                {
                    typeStr = string.Format("Where Type={0}", ((int)filter.Type.Value).ToString());
                }

                command.CommandText = command.CommandText.Replace("#StrWhere2#", typeStr);
                var dt = command.ExecuteDataTable();
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(SettleOrderStatus));
                enumColList.Add("Type", typeof(SettleOrderType));
                command.ConvertEnumColumn(dt, enumColList);
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public static CommissionMasterInfo GetCommissionMasterInfoBySysNo(int sysno, int merchantSysNO)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_GetCommissionMasterInfoBySysNo");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@MerchantSysNo", merchantSysNO);
            var result = command.ExecuteEntity<CommissionMasterInfo>();
            return result;
        }

        public static List<CommissionItemInfo> GetCommissionItem(int sysNo, int merchantSysNO)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_GetCommissionItemByCommissionSysNo");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(command, "CI.SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CI.CommissionMasterSysNo", DbType.Int32, "@CommissionMasterSysNo", QueryConditionOperatorType.Equal, sysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CM.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, merchantSysNO);
                command.CommandText = sb.BuildQuerySql();
                return command.ExecuteEntityList<CommissionItemInfo>();
            }
        }

        public static List<CommissionItemLogDetailInfo> QueryCommissionLogDetail(int merchantsysno, string type, List<int> itemSysnoList)
        {
            //需要传入的参数 ItemSysNoList,MerchantSysNo
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionItemLogDetailByItemSysNoList");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(command, "CIL.InDate DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CM.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, merchantsysno);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CIL.Type", DbType.AnsiStringFixedLength, "@Type", QueryConditionOperatorType.Equal, type);
                if (itemSysnoList == null || itemSysnoList.Count == 0) itemSysnoList.Add(0);
                sb.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "CIL.CommissionItemSysNo", DbType.Int32, itemSysnoList);
                command.CommandText = sb.BuildQuerySql();
                return command.ExecuteEntityList<CommissionItemLogDetailInfo>();
            }
        }
    }
}
