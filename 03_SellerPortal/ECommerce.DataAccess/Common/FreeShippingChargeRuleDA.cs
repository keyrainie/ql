using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility.DataAccess.SearchEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ECommerce.DataAccess.Common
{
    public class FreeShippingChargeRuleDA
    {
        public static DataTable Query(FreeShippingChargeRuleQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryFreeShippingChargeRule");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, filter, "[SysNo] DESC"))
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

                if (filter.ProvinceSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ShipAreaSettingValue", DbType.AnsiString,
                        "@ShipAreaSettingValueLike", QueryConditionOperatorType.Like, string.Format(",{0},", filter.ProvinceSysNo));
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.AnsiStringFixedLength,
                    "@Status", QueryConditionOperatorType.Equal, filter.Status);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AmountSettingType", DbType.Int32,
                    "@AmountSettingType", QueryConditionOperatorType.Equal, filter.AmountSettingType);

                command.SetParameterValue("@SellerSysNo", filter.SellerSysNo);

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




        public static FreeShippingChargeRuleInfoResult Load(int sysno, int SellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFreeShippingChargeRuleBySysNo");
            command.SetParameterValue("@SysNo", sysno);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            FreeShippingChargeRuleInfoResult info = command.ExecuteEntity<FreeShippingChargeRuleInfoResult>(SettingValueMapper);
            SetMultipleSettingValue(new List<FreeShippingChargeRuleInfo>() { info });

            return info;
        }

        public static FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                entity = CreateMasterInfo(entity);
                if (!entity.IsGlobal)
                {
                    UpdateProductSettingValue(entity);
                }
                ts.Complete();
            }
            LoadRuleProductSettingValue(entity);

            return entity;
        }

        public static void UpdateInfo(FreeShippingChargeRuleInfo entity)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UpdateMasterInfo(entity);
                if (!entity.IsGlobal)
                {
                    UpdateProductSettingValue(entity);
                }
                ts.Complete();
            }
        }

        public static void UpdateStatus(FreeShippingChargeRuleInfo entity, int UserSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateFreeShippingChargeRuleStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@EditUser", UserSysNo);
            command.ExecuteNonQuery();
        }

        public static void Delete(int sysno)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                DataCommand command = DataCommandManager.GetDataCommand("DeleteFreeShippingChargeRule");
                command.SetParameterValue("@SysNo", sysno);
                command.ExecuteNonQuery();

                DeleteRuleAllProductSettingValue(sysno);

                ts.Complete();
            }
        }

        public static List<FreeShippingChargeRuleInfo> GetAllByStatus(FreeShippingAmountSettingStatus status, int SellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllFreeShippingChargeRuleByStatus");
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@SellerSysNo", SellerSysNo);
            List<FreeShippingChargeRuleInfo> list = command.ExecuteEntityList<FreeShippingChargeRuleInfo>(SettingValueMapper);
            SetMultipleSettingValue(list);

            return list;
        }

        public static void SettingValueMapper(DbDataReader reader, FreeShippingChargeRuleInfo entity)
        {
            if (!(reader["PayTypeSettingValueStr"] is DBNull) && reader["PayTypeSettingValueStr"] != null)
            {
                entity.PayTypeSettingValue = new List<SimpleObject>();
                foreach (string key in Convert.ToString(reader["PayTypeSettingValueStr"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    entity.PayTypeSettingValue.Add(new SimpleObject() { ID = key });
                }
            }
            if (!(reader["ShipAreaSettingValueStr"] is DBNull) && reader["ShipAreaSettingValueStr"] != null)
            {
                entity.ShipAreaSettingValue = new List<SimpleObject>();
                foreach (string key in Convert.ToString(reader["ShipAreaSettingValueStr"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    entity.ShipAreaSettingValue.Add(new SimpleObject() { ID = key });
                }
            }
        }

        public static void SetMultipleSettingValue(List<FreeShippingChargeRuleInfo> list)
        {
            if (HasElements(list))
            {
                AreaInfoQueryFilter queryFilter = new AreaInfoQueryFilter()
                {
                    PageIndex = 0,
                    PageSize = int.MaxValue,
                    OnlyProvince = true,
                };
                List<AreaInfo> allProvinceList = CommonDA.QueryArea(queryFilter).ResultList;

                int totalCount;
                List<PayTypeInfo> allPayTypeList = CommonDA.GetAllPayType();

                foreach (var item in list)
                {
                    if (HasElements(item.ShipAreaSettingValue) && HasElements(allProvinceList))
                    {
                        foreach (var simpleObj in item.ShipAreaSettingValue)
                        {
                            var p = allProvinceList.Find(x => x.SysNo.ToString() == simpleObj.ID);
                            if (p != null)
                            {
                                simpleObj.Name = p.ProvinceName;
                            }
                        }
                    }
                    if (HasElements(item.PayTypeSettingValue) && HasElements(allPayTypeList))
                    {
                        foreach (var simpleObj in item.PayTypeSettingValue)
                        {
                            var p = allPayTypeList.Find(x => x.SysNo.ToString() == simpleObj.ID);
                            if (p != null)
                            {
                                simpleObj.Name = p.PayTypeName;
                            }
                        }
                    }
                    if (!item.IsGlobal)
                    {
                        LoadRuleProductSettingValue(item);
                    }
                }
            }
        }

        public static FreeShippingChargeRuleInfo CreateMasterInfo(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertFreeShippingChargeRule");
            command.SetParameterValue("@StartDate", entity.StartDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AmountSettingType", entity.AmountSettingType);
            command.SetParameterValue("@AmountSettingValue", entity.AmountSettingValue);
            command.SetParameterValue("@IsGlobal", entity.IsGlobal);
            command.SetParameterValue("@Description", entity.Description);
            command.SetParameterValue("@InUser", entity.InUserSysNo);
            command.SetParameterValue("@SellerSysNo", entity.SellerSysNo);

            string payTypeSettingValue = string.Empty;
            if (entity.PayTypeSettingValue != null)
            {
                payTypeSettingValue = ECommerce.DataAccess.Utility.StringUtility.Join(entity.PayTypeSettingValue, ",");
            }
            command.SetParameterValue("@PayTypeSettingValue", payTypeSettingValue);

            string shipAreaSettingValue = string.Empty;
            if (entity.ShipAreaSettingValue != null)
            {
                shipAreaSettingValue = ECommerce.DataAccess.Utility.StringUtility.Join(entity.ShipAreaSettingValue, ",");
            }
            command.SetParameterValue("@ShipAreaSettingValue", shipAreaSettingValue);

            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());

            return entity;
        }

        public static void UpdateMasterInfo(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateFreeShippingChargeRule");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@StartDate", entity.StartDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@AmountSettingType", entity.AmountSettingType);
            command.SetParameterValue("@AmountSettingValue", entity.AmountSettingValue);
            command.SetParameterValue("@IsGlobal", entity.IsGlobal);
            command.SetParameterValue("@Description", entity.Description);
            command.SetParameterValue("@EditUser", entity.EditUserSysNo);
            string payTypeSettingValue = string.Empty;
            if (entity.PayTypeSettingValue != null)
            {
                payTypeSettingValue = ECommerce.DataAccess.Utility.StringUtility.Join(entity.PayTypeSettingValue, ",");
            }
            command.SetParameterValue("@PayTypeSettingValue", payTypeSettingValue);

            string shipAreaSettingValue = string.Empty;
            if (entity.ShipAreaSettingValue != null)
            {
                shipAreaSettingValue = ECommerce.DataAccess.Utility.StringUtility.Join(entity.ShipAreaSettingValue, ",");
            }
            command.SetParameterValue("@ShipAreaSettingValue", shipAreaSettingValue);

            command.ExecuteNonQuery();
        }

        public static void UpdateProductSettingValue(FreeShippingChargeRuleInfo entity)
        {
            DeleteRuleAllProductSettingValue(entity.SysNo.Value);

            if (entity.ProductSettingValue != null && entity.ProductSettingValue.Count > 0)
            {
                foreach (var item in entity.ProductSettingValue)
                {
                    CreateRuleProductSettingItemValue(entity.SysNo.Value, item);
                }
            }
        }

        public static void LoadRuleProductSettingValue(FreeShippingChargeRuleInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", entity.SysNo.Value);

            entity.ProductSettingValue = command.ExecuteEntityList<SimpleObject>();
        }

        public static void CreateRuleProductSettingItemValue(int ruleSysNo, SimpleObject item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", ruleSysNo);
            command.SetParameterValue("@ProductSysNo", item.SysNo.Value);

            command.ExecuteScalar();
        }

        public static void DeleteRuleAllProductSettingValue(int ruleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteFreeShippingChargeRuleProductSettingValue");
            command.SetParameterValue("@RuleSysNo", ruleSysNo);

            command.ExecuteNonQuery();
        }

        public static bool HasElements(IList list)
        {
            return list != null && list.Count > 0;
        }

    }
}
