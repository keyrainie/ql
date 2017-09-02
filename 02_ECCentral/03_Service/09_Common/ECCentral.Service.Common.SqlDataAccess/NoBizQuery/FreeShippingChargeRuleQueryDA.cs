using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IFreeShippingChargeRuleQueryDA))]
    public class FreeShippingChargeRuleQueryDA : IFreeShippingChargeRuleQueryDA
    {
        public DataTable Query(FreeShippingChargeRuleQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryFreeShippingChargeRule");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, null, "[SysNo] DESC"))
            {
                if (filter.StartDate.HasValue || filter.EndDate.HasValue)
                {
                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    #region 构造时间条件
                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.OR);
                    // s1 ≤ start && t1 ≥ start
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "StartDate", DbType.Date,
                        "@StartDate1", QueryConditionOperatorType.MoreThanOrEqual, filter.StartDate);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "StartDate", DbType.Date,
                        "@EndDate1", QueryConditionOperatorType.LessThanOrEqual, filter.EndDate);
                    builder.ConditionConstructor.EndGroupCondition();

                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.OR);
                    // s1 ≥ start && t1 ≤ end
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "StartDate", DbType.Date,
               "@StartDate2", QueryConditionOperatorType.LessThanOrEqual, filter.StartDate);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EndDate", DbType.Date,
                        "@EndDate2", QueryConditionOperatorType.MoreThanOrEqual, filter.EndDate);
                    builder.ConditionConstructor.EndGroupCondition();

                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.OR);
                    // s1 ≤ end && t1 ≥ end
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EndDate", DbType.Date,
               "@StartDate3", QueryConditionOperatorType.MoreThanOrEqual, filter.StartDate);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EndDate", DbType.Date,
                        "@EndDate3", QueryConditionOperatorType.LessThanOrEqual, filter.EndDate);
                    builder.ConditionConstructor.EndGroupCondition();
                    #endregion
                    builder.ConditionConstructor.EndGroupCondition();
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AmountSettingValue", DbType.Decimal,
                    "@AmtFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.AmtFrom);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AmountSettingValue", DbType.Decimal,
                    "@AmtTo", QueryConditionOperatorType.LessThanOrEqual, filter.AmtTo);

                if (filter.PayTypeSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PayTypeSettingValue", DbType.AnsiString,
                        "@PayTypeSettingValueLike", QueryConditionOperatorType.Like, string.Format(",{0},", filter.PayTypeSysNo));
                }

                if (filter.ShipAreaID.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ShipAreaSettingValue", DbType.AnsiString,
                        "@ShipAreaSettingValueLike", QueryConditionOperatorType.Like, string.Format(",{0},", filter.ShipAreaID));
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.AnsiStringFixedLength,
                    "@Status", QueryConditionOperatorType.Equal, filter.Status);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AmountSettingType", DbType.Int32,
                    "@AmountSettingType", QueryConditionOperatorType.Equal, filter.AmountSettingType);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("AmountSettingType", typeof(FreeShippingAmountSettingType));
                enumList.Add("Status", typeof(FreeShippingAmountSettingStatus));
                DataTable dt = command.ExecuteDataTable(enumList);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string areaSettingName = String.Empty;
                    string[] areaNameParts;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!(row["AreaSettingName"] is DBNull) && row["AreaSettingName"] != null)
                        {
                            areaSettingName = Convert.ToString(row["AreaSettingName"]);
                            areaNameParts = areaSettingName.Split(new char[] { ',' }, StringSplitOptions.None);
                            if (areaNameParts.Length > 4)
                            {
                                row["AreaSettingName"] = string.Format("{0}等{1}个地区",
                                    string.Join(",", areaNameParts.ToList().GetRange(0, 4)), areaNameParts.Length);
                            }
                        }
                    }
                }

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
