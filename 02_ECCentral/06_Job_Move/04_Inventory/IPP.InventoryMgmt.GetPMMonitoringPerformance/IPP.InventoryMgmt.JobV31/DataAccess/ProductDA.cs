using System;
using System.Collections.Generic;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using IPP.InventoryMgmt.Taobao.JobV31.Provider;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using IPP.InventoryMgmt.Taobao.JobV31.Common;
using System.Text.RegularExpressions;

namespace IPP.InventoryMgmt.JobV31.DataAccess
{
    public class ProductDA
    {
        /// <summary>
        /// 查询商品信息
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public static List<ProductEntity> Query(QueryConditionEntity<QueryProduct> query)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProduct");

            command.CommandText = ProductDASqlHelper.BuilderSql(command, query);

            List<ProductEntity> list = command.ExecuteEntityList<ProductEntity>();

            return list;
        }

        /// <summary>
        /// 修改分仓库存
        /// </summary>
        /// <param name="list"></param>
        public static void ModifyQty(List<ProductEntity> list)
        {
            CommonConst commonConst = new CommonConst();
            foreach (ProductEntity entity in list)
            {
                if (entity.InventoryAlarmQty.HasValue)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("ModifyStockQty");
                    command.SetParameterValue("@ProductMappingSysNo", entity.ProductMappingSysNo);
                    command.SetParameterValue("@WarehouseNumber", entity.WarehouseNumber);
                    command.SetParameterValue("@InventoryQty", entity.ResultQty);
                    command.SetParameterValue("@CompanyCode", commonConst.CompanyCode);

                    command.ExecuteNonQuery();
                }
                else
                {
                    InsertStockQty(entity);
                }
            }

        }

        public static void InsertStockQty(ProductEntity entity)
        {
            CommonConst commonConst = new CommonConst();
            DataCommand command = DataCommandManager.GetDataCommand("InsertStockQty");
            command.SetParameterValue("@ProductMappingSysNo", entity.ProductMappingSysNo);
            command.SetParameterValue("@WarehouseNumber", entity.WarehouseNumber);
            command.SetParameterValue("@WarehouseName", "");
            command.SetParameterValue("@InventoryQty", entity.ResultQty);
            command.SetParameterValue("@CompanyCode", commonConst.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", commonConst.StoreCompanyCode);
            command.SetParameterValue("@InUser", commonConst.UserLoginName);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改总仓库存
        /// </summary>
        /// <param name="list"></param>
        public static void ModifyQty(List<InventoryQtyEntity> list)
        {
            CommonConst commonConst = new CommonConst();
            foreach (InventoryQtyEntity entity in list)
            {
                if (entity.InventoryAlarmQty.HasValue)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("ModifyQty");
                    command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
                    command.SetParameterValue("@InventoryQty", entity.InventoryQty);
                    command.SetParameterValue("@SKU", entity.SKU);
                    command.SetParameterValue("@Type", entity.PartnerType);
                    command.SetParameterValue("@CompanyCode", commonConst.CompanyCode);
                    command.SetParameterValue("@InventoryAlarmQty", entity.InventoryAlarmQty);

                    command.ExecuteNonQuery();
                }
                else
                {
                    InsertQty(entity);
                }
            }
        }

        public static void InsertQty(InventoryQtyEntity entity)
        {
            CommonConst commonConst = new CommonConst();
            DataCommand command = DataCommandManager.GetDataCommand("InsertQty");
            command.SetParameterValue("@ProductMappingSysNo", entity.ProductMappingSysNo);
            command.SetParameterValue("@ProductDescription", "");
            command.SetParameterValue("@InventoryQty", entity.InventoryQty);
            command.SetParameterValue("@CompanyCode", commonConst.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", commonConst.StoreCompanyCode);
            command.SetParameterValue("@InUser", commonConst.UserLoginName);
            command.SetParameterValue("@InventoryAlarmQty", commonConst.InventoryAlarmQty);

            command.ExecuteNonQuery();
        }
    }
    class ProductDASqlHelper
    {
        internal static string BuilderSql(CustomDataCommand command, QueryConditionEntity<QueryProduct> query)
        {
            CommonConst commonConst = new CommonConst();

            string sql = command.CommandText;
            bool hasWhere = false;
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
    sql, command, query.PagingInfo, "its.StockSysNo"))
            {
                QueryProduct condition = query.Condition;
                if (!string.IsNullOrEmpty(condition.CompanyCode))
                {
                    builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.CompanyCode",
                        DbType.String,
                        "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        condition.CompanyCode
                        );
                    hasWhere = true;
                }
                if (!string.IsNullOrEmpty(condition.PartnerType))
                {
                    builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.PartnerType",
                        DbType.String,
                        "@PartnerType",
                        QueryConditionOperatorType.Equal,
                        condition.PartnerType
                        );
                    hasWhere = true;
                }
                if (condition.ProductID != null && condition.ProductID.Length > 0)
                {
                    string productIds = Util.Contract("','", condition.ProductID);
                    productIds = "'" + productIds + "'";
                    builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.ProductID",
                        DbType.String,
                        "@ProductID",
                        QueryConditionOperatorType.In,
                        productIds
                        );
                    hasWhere = true;
                }
                if (condition.ProductSysNo != null && condition.ProductSysNo.Length > 0)
                {
                    string productSysNos = Util.Contract("','", condition.ProductSysNo);
                    productSysNos = "'" + productSysNos + "'";
                    builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.ProductSysNo",
                        DbType.String,
                        "@ProductSysNo",
                        QueryConditionOperatorType.In,
                        productSysNos
                        );
                    hasWhere = true;
                }
                if (condition.SysProductID != null && condition.SysProductID.Length > 0)
                {
                    string sysProductIds = Util.Contract("','", condition.SysProductID);
                    sysProductIds = "'" + sysProductIds + "'";
                    builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.SysProductID",
                        DbType.String,
                        "@SysProductID",
                        QueryConditionOperatorType.In,
                        sysProductIds
                        );
                    hasWhere = true;
                }
                if (condition.WareHourseNumber != null && condition.WareHourseNumber.Length > 0)
                {
                    string wareHourseNumbers = Util.Contract(",", condition.WareHourseNumber);

                    builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "its.StockSysNo",
                        QueryConditionOperatorType.In,
                        wareHourseNumbers
                        );
                    hasWhere = true;
                }

                //状态筛选
                builder.AddCondition(QueryConditionRelationType.AND,
                        "ppm.Status",
                        DbType.String,
                        "@Status",
                        QueryConditionOperatorType.Equal,
                        'A'
                        );
                hasWhere = true;

                sql = builder.BuildQuerySql();

                //有库存变化的才进行数据捞取
                string where = "(isnull(its.AvailableQty,0)+ isnull(its.ConsignQty,0)+isnull(its.VirtualQty,0)) <> isnull(ppsi.InventoryQty,0)";

                sql = sql.Replace("@Where@", string.Format("{0}{1}", hasWhere ? " AND " : " ", where));
            }

            return sql;
        }
    }
}
