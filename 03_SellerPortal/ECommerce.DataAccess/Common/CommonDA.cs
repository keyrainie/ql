using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Utility.DataAccess;
using System.Data;
using ECommerce.Enums;
using ECommerce.Entity.Store.Vendor;

namespace ECommerce.DataAccess
{
    public class CommonDA
    {
        public static bool SendEmail(AsyncEmail item)
        {
            item.Priority = item.Priority ?? 0;
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_SendEmail");
            cmd.SetParameterValue<AsyncEmail>(item);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }


        public static bool SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_SendSMS");
            cmd.SetParameterValue("@CellNumber", phoneNumber);
            cmd.SetParameterValue("@SMSContent", content);
            cmd.SetParameterValue("@Priority", priority);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<AreaInfo> QueryArea(AreaInfoQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryArea");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "a.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.DistrictSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, queryFilter.DistrictSysNo.Value);
                }
                if (queryFilter.CitySysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CitySysNo", DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, queryFilter.CitySysNo.Value);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "a.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.CitySysNo.Value);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }
                if (!string.IsNullOrEmpty(queryFilter.ProvinceSysNo))
                {
                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProvinceSysNo", DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, queryFilter.ProvinceSysNo.Value);
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.ProvinceSysNo", QueryConditionOperatorType.In, queryFilter.ProvinceSysNo);
                }
                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Status", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, queryFilter.Status.Value);
                }
                if (queryFilter.OnlyProvince)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "a.ProvinceSysNo IS NULL");
                }
                if (queryFilter.OnlyCity)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "a.CitySysNo IS NULL");
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "a.ProvinceSysNo IS NOT NULL");
                }
                //if (queryFilter.OnlyDistrict)
                //{
                //    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "a.CitySysNo IS NOT NULL");
                //}
                command.CommandText = sqlBuilder.BuildQuerySql();
                List<AreaInfo> resultList = command.ExecuteEntityList<AreaInfo>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<AreaInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        /// <summary>
        /// 查询顾客
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult QueryCustomers(CustomerQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCustomers");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "c.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo.Value);
                }
                if (!string.IsNullOrWhiteSpace(queryFilter.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.CustomerID", DbType.String, "@CustomerID", QueryConditionOperatorType.Equal, queryFilter.CustomerID);
                }
                if (!string.IsNullOrWhiteSpace(queryFilter.CustomerName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.CustomerName", DbType.String, "@CustomerName", QueryConditionOperatorType.Equal, queryFilter.CustomerName);
                }
                if (!string.IsNullOrWhiteSpace(queryFilter.Email))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.Email", DbType.String, "@Email", QueryConditionOperatorType.Equal, queryFilter.Email);
                }
                if (!string.IsNullOrWhiteSpace(queryFilter.CellPhone))
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "c.Phone", DbType.String, "@Phone", QueryConditionOperatorType.Equal, queryFilter.CellPhone);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "c.CellPhone", DbType.String, "@CellPhone", QueryConditionOperatorType.Equal, queryFilter.CellPhone);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                DataTable resultList = command.ExecuteDataTable(new EnumColumnList{
                    {"Status",typeof(CustomerStatus)}
                    ,{"CustomersType",typeof(CustomerType)}
                    ,{"Rank",typeof(CustomerRank)}
                });
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult(resultList, queryFilter, totalCount);
            }
        }
        /// <summary>
        /// 根据key获取IPP3.dbo.Sys_Configuragtion表中配置的 Value 值.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetSysConfigurationValue(string key, string companyCode)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryConfigurationByKey");
            dataCommand.ReplaceParameterValue("@Key", key);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToString(dataCommand.ExecuteScalar());
        }

        public static QueryResult<BrandInfo> QueryBrandList(BrandQueryFilter queryFilter)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetBrandInfo");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, queryFilter, "c.SysNo DESC"))
            {
                if (queryFilter.Status.HasValue)
                {
                    if (queryFilter.Status.Value.ToString().Equals("Active"))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.Status",
                        DbType.StringFixedLength, "@Status",
                        QueryConditionOperatorType.Equal,
                        "A");
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.Status",
                        DbType.StringFixedLength, "@Status",
                        QueryConditionOperatorType.Equal,
                        "D");
                    }

                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "c.BrandName_Ch",
                       DbType.String, "@BrandName_Ch",
                       QueryConditionOperatorType.Like,
                       queryFilter.BrandName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "m.ManufacturerName",
                       DbType.String, "@ManufacturerName",
                       QueryConditionOperatorType.Like,
                       queryFilter.ManufacturerName);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                List<BrandInfo> resultList = dataCommand.ExecuteEntityList<BrandInfo>();
                int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return new QueryResult<BrandInfo>()
                {
                    PageInfo = new PageInfo()
                    {
                        PageIndex = queryFilter.PageIndex,
                        PageSize = queryFilter.PageSize,
                        TotalCount = totalCount,
                        SortBy = queryFilter.SortFields
                    },
                    ResultList = resultList
                };

            }
        }


        public static QueryResult<ManufacturerInfo> QueryManufacturerList(ManufacturerQueryFilter queryFilter)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryManufacturer");

            if (string.IsNullOrEmpty(queryFilter.ManufacturerNameLocal))
            {
                dc.SetParameterValue("@ManufacturerName", "");
            }
            else
            {
                dc.SetParameterValue("@ManufacturerName", "%" + queryFilter.ManufacturerNameLocal.Trim() + "%");
            }

            dc.SetParameterValue("@Status", queryFilter.Status == null ? -999 : (int)queryFilter.Status);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.SetParameterValue("@SortField", queryFilter.SortFields);
            dc.SetParameterValue("@PageSize", queryFilter.PageSize);
            dc.SetParameterValue("@PageCurrent", queryFilter.PageIndex);

            List<ManufacturerInfo> resultList = dc.ExecuteEntityList<ManufacturerInfo>(); ;
            int totalCount = (int)dc.GetParameterValue("@TotalCount");

            return new QueryResult<ManufacturerInfo>()
            {
                PageInfo = new PageInfo()
                {
                    PageIndex = queryFilter.PageIndex,
                    PageSize = queryFilter.PageSize,
                    TotalCount = totalCount,
                    SortBy = queryFilter.SortFields
                },
                ResultList = resultList
            };
        }

        public static List<CurrencyInfo> GetCurrencyList()
        {
            var dataCommand = DataCommandManager.GetDataCommand("Common_GetCurrencyList");
            return dataCommand.ExecuteEntityList<CurrencyInfo>((s, t) =>
            {
                t.IsLocal = (int)s["IsLocal"] != 0;
            });
        }

        public static List<PayTypeInfo> GetAllPayType()
        {
            var dataCommand = DataCommandManager.GetDataCommand("Common_GetAllPayType");
            return dataCommand.ExecuteEntityList<PayTypeInfo>((s, t) =>
            {
                t.IsNet = (int)s["IsNet"] != 0;
            });
        }

        public static ShipTypeSMS GetShipTypeSMSInfo(int shipTypeSysNo, SMSType smsType, string WebChannelID)
        {
            ShipTypeSMS entity = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryShipTypeSMS");
            cmd.SetParameterValue("@SMSType", smsType);
            cmd.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            entity = cmd.ExecuteEntity<ShipTypeSMS>();
            return entity;
        }

        public static ShippingType GetShippingTypeBySysNo(int shipTypeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShippingTypeBySysNo");
            cmd.SetParameterValue("@SysNo", shipTypeSysNo);
            return cmd.ExecuteEntity<ShippingType>();
        }

        /// <summary>
        /// 根据用户ID获取vendor user cellphone
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static string GetVendorCellPhone(string customerID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetVendorCellPhone");
            cmd.SetParameterValue("@UserID", customerID);
            return cmd.ExecuteScalar<string>();
        }
    }
}
