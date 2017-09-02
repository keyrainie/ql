using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IInventoryQueryDA))]
    public class InventoryQueryDA : IInventoryQueryDA
    {
        #region IInventoryQueryDA Members

        public virtual DataTable QueryInventoryList(InventoryQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryInventory");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "b.SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");//[Mark][Alan.X.Luo 硬编码]

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.SysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);
                }
                //if (string.IsNullOrEmpty(queryFilter.AuthorizedPMsSysNumber))
                //{
                //    queryFilter.AuthorizedPMsSysNumber = "-999";
                //}
                if (queryFilter.ManufacturerSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.ManufacturerSysNo",
                        DbType.Int32, "@ManufacturerSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ManufacturerSysNo.Value);
                }
                if (queryFilter.BrandSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.BrandSysNo",
                        DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.BrandSysNo.Value);
                }
                if (queryFilter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c3.SysNo",
                        DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C3SysNo.Value);
                }
                if (queryFilter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c2.SysNo",
                        DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C2SysNo.Value);
                }
                if (queryFilter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c1.SysNo",
                        DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C1SysNo.Value);
                }
                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "plm.LastVendorSysNo",
                        DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.VendorSysNo.Value);
                }

                if (queryFilter.IsInventoryWarning.HasValue && queryFilter.IsInventoryWarning.Value == true )
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "((pe.[SafeQty] IS NOT NULL) AND a.[AvailableQty]+a.[ConsignQty]<=pe.[SafeQty])");
                }


                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.PMUserSysNo",
                QueryConditionOperatorType.In, queryFilter.AuthorizedPMsSysNumber);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            }

            if (queryFilter.IsUnPayShow.HasValue && queryFilter.IsUnPayShow.Value)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int productSysNo = int.Parse(row["ProductSysNo"].ToString());
                        row["UnPayOrderQty"] = GetUnPayQtyTotal(productSysNo);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 查询月底库存，用于发送邮件
        /// </summary>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryInventoryListEndOfMouth()
        {
            DataTable dt = new DataTable();
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryInventoryEndOfMouth");
            dt = dataCommand.ExecuteDataTable();
            return dt;
        }

        public virtual DataTable QueryInventoryStockList(InventoryQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryInventoryStock");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "b.SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.SysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);
                }
                if (queryFilter.StockSysNo.HasValue && queryFilter.StockSysNo.Value > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo",
                     DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal,
                     queryFilter.StockSysNo.Value);
                }
                if (!string.IsNullOrEmpty(queryFilter.PositionInWarehouse))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Position1",
                        DbType.String, "@PositionInWarehouse", QueryConditionOperatorType.Like,
                        queryFilter.PositionInWarehouse);
                }
                if (queryFilter.IsAccountQtyLargerThanZero.HasValue && queryFilter.IsAccountQtyLargerThanZero.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.AccountQty",
                        DbType.Int32, "@AccountQuantity", QueryConditionOperatorType.MoreThan,
                        0);
                }

                //if (string.IsNullOrEmpty(queryFilter.AuthorizedPMsSysNumber))
                //{
                //    queryFilter.AuthorizedPMsSysNumber = "-999";
                //}
                //sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.PMUserSysNo",
                //QueryConditionOperatorType.In, queryFilter.AuthorizedPMsSysNumber);

                if (queryFilter.ManufacturerSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.ManufacturerSysNo",
                        DbType.Int32, "@ManufacturerSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ManufacturerSysNo.Value);
                }
                if (queryFilter.BrandSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.BrandSysNo",
                        DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.BrandSysNo.Value);
                }
                if (queryFilter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c3.SysNo",
                        DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C3SysNo.Value);
                }
                if (queryFilter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c2.SysNo",
                        DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C2SysNo.Value);
                }
                if (queryFilter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c1.SysNo",
                        DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C1SysNo.Value);
                }
                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "plm.LastVendorSysNo",
                        DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.VendorSysNo.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            if (queryFilter.IsUnPayShow.HasValue && queryFilter.IsUnPayShow.Value)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int productSysNo = int.Parse(row["ProductSysNo"].ToString());
                        int stockSysNo = int.Parse(row["StockSysNo"].ToString());
                        row["UnPayOrderQty"] = GetUnPayQtyByStock(productSysNo, stockSysNo);
                    }
                }
            }

            return dt;
        }

        public virtual DataTable QueryVendorInfoForBackOrderToday(BackOrderForTodayQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorInfo");
            StringBuilder sbSQL = new StringBuilder();
            if (!string.IsNullOrEmpty(queryFilter.VendorSysNo) && queryFilter.VendorSysNo != "0")
            {
                sbSQL.Append(" AND  SysNo=" + queryFilter.VendorSysNo);
            }

            if (!string.IsNullOrEmpty(queryFilter.VendorName))
            {
                sbSQL.Append(" AND  VendorName LIKE " + "'" + queryFilter.VendorName + "%" + "'");

            }
            dataCommand.CommandText = dataCommand.CommandText.Replace("#StrWhere#", sbSQL.ToString());
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, " SysNo"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return dt;
        }

        public virtual DataTable QueryProductInventoryByStock(InventoryQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductInventoryByStock");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "a.ProductSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");//[Mark][Alan.X.Luo 硬编码]

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);
                }

                if (queryFilter.StockSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo",
                        DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.StockSysNo.Value);
                }

                //if (string.IsNullOrEmpty(queryFilter.AuthorizedPMsSysNumber))
                //{
                //    queryFilter.AuthorizedPMsSysNumber = "-999";
                //}
                //sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.PMUserSysNo",
                //QueryConditionOperatorType.In, queryFilter.AuthorizedPMsSysNumber);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            }

            return dt;
        }

        public virtual DataTable QueryProductInventoryTotal(InventoryQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductInventoryTotal");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "a.ProductSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");//[Mark][Alan.X.Luo 硬编码]

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);
                }

                if (string.IsNullOrEmpty(queryFilter.AuthorizedPMsSysNumber))
                {
                    queryFilter.AuthorizedPMsSysNumber = "-999";
                }
                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.PMUserSysNo",
                QueryConditionOperatorType.In, queryFilter.AuthorizedPMsSysNumber);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            }

            return dt;
        }
        /// <summary>
        /// pm工作指标监控查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryPMMonitoringPerformanceIndicators(PMMonitoringPerformanceIndicatorsQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPMMPIV3");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            List<string> sort = new List<string>();
            List<string> pms = new List<string>();
            List<string> sourceData = new List<string>();
            string sourceDataG = string.Empty;
            List<string> group = new List<string>();
            List<string> where = new List<string>();

            //init
            sort.Add("PStocks.Category1SysNo");
            sort.Add("PStocks.Category2SysNo");
            group.Add("Category1SysNo");
            group.Add("Category2SysNo");


            if (!string.IsNullOrEmpty(queryFilter.SelectedCategory1))
            {
                where.Add("PStocks.Category1SysNo=" + queryFilter.SelectedCategory1);
                pms.Add("pp1.Category1SysNo= " + queryFilter.SelectedCategory1);
            }

            if (!string.IsNullOrEmpty(queryFilter.SelectedCategory2))
            {
                where.Add("PStocks.Category2SysNo=" + queryFilter.SelectedCategory2);
                pms.Add("pp1.Category2SysNo= " + queryFilter.SelectedCategory2);
            }
            else
            {
                pms.Add("pp1.Category2SysNo=PStocks.Category2SysNo");
            }

            if (!string.IsNullOrEmpty(queryFilter.SelectedPMSysNo))
            {
                where.Add("PStocks.PMUserSysNo=" + queryFilter.SelectedPMSysNo);
                pms.Add("pp1.PMUserSysNo= " + queryFilter.SelectedPMSysNo);
                sort.Add("PStocks.PMUserSysNo");
                group.Add("PStocks.PMUserSysNo");
            }

            if (queryFilter.StockSysNo != null && queryFilter.StockSysNo != 0)
            {
                //[Mark][Alan.X.Luo 硬编码]
                if (queryFilter.StockSysNo == 51)
                {
                    //[Mark][Alan.X.Luo 硬编码]
                    pms.Add("(pp1.StockSysNo=51 or pp1.StockSysNo=59)");

                    //[Mark][Alan.X.Luo 硬编码]
                    sourceDataG = ",CASE WHEN pp.StockSysNo=59 THEN 51 ELSE  pp.StockSysNo End";
                    sourceData.Add("CASE  WHEN SUM(isnull(IsOutOfStock,0))=2 THEN 1 ELSE 0 END as IsOutOfStock");
                    //[Mark][Alan.X.Luo 硬编码]
                    sourceData.Add("CASE WHEN pp.StockSysNo=59 THEN 51 ELSE  pp.StockSysNo End AS StockSysNo");
                    //Losing
                    sourceData.Add("CASE  WHEN SUM(isnull(IsOutOfStock,0))=2 THEN sum(isnull(Losing,0)) ELSE 0 END AS Losing");
                }
                else
                {

                    pms.Add("pp1.StockSysNo=" + queryFilter.StockSysNo);
                    sourceDataG = ",pp.StockSysNo";
                    sourceData.Add("SUM(isnull(IsOutOfStock,0)) as IsOutOfStock");
                    sourceData.Add("pp.StockSysNo");

                    sourceData.Add("sum(isnull(Losing,0)) AS Losing");


                }


                where.Add("PStocks.StockSysNo=" + queryFilter.StockSysNo);
                group.Add("StockSysNo");
                sort.Add("PStocks.StockSysNo");
            }
            else
            {
                sourceData.Add("CASE  WHEN SUM(isnull(IsOutOfStock,0))=6 THEN 1 ELSE 0 END AS IsOutOfStock");
                sourceData.Add("CASE  WHEN SUM(isnull(IsOutOfStock,0))=6 THEN sum(isnull(Losing,0)) ELSE 0 END AS Losing");
            }


            if (!string.IsNullOrEmpty(queryFilter.AVGSaledQty))
            {

                string Operators = string.Empty;

                if (queryFilter.AVGSaledQtyCondition == ">=")
                {
                    Operators = ">=";
                }
                else if (queryFilter.AVGSaledQtyCondition == "=")
                {
                    Operators = "=";
                }
                else if (queryFilter.AVGSaledQtyCondition == "<=")
                {
                    Operators = "<=";
                }

                if (Operators == "<=" || queryFilter.AVGSaledQty == "0")
                {

                    where.Add("(PStocks.AVGDS" + Operators + queryFilter.AVGSaledQty + " OR PStocks.AVGDS is null)");
                    //pms.Add("(pa1.AVGDailySales" + Operators + queryEntity.Condition.AVGSaledQty + " OR pa1.AVGDailySales is null)");

                }
                else
                {
                    where.Add("PStocks.AVGDS " + Operators + queryFilter.AVGSaledQty);
                    //pms.Add("pa1.AVGDailySales" + Operators + queryEntity.Condition.AVGSaledQty);
                }
            }

            string sortStr = string.Join(",", sort.ToArray());
            string pmsStr = string.Join(" And ", pms.ToArray());
            string sourceDataStr = string.Join(",", sourceData.ToArray());
            string groupStr = string.Join(",", group.ToArray());
            string whereStr = string.Join(" And ", where.ToArray());

            dataCommand.ReplaceParameterValue("#SortColumnName#", sortStr);
            dataCommand.ReplaceParameterValue("#PMCondition#", "Where " + pmsStr);
            dataCommand.ReplaceParameterValue("#SourceData#", sourceDataStr);
            dataCommand.ReplaceParameterValue("#Group#", "Group by " + groupStr);

            dataCommand.ReplaceParameterValue("#StrWhere#", where.Count == 0 ? "" : "Where " + whereStr);

            dataCommand.ReplaceParameterValue("#SourceDataG#", sourceDataG);

            dataCommand.ReplaceParameterValue("#AvailableSalesDays#", queryFilter.AvailableSaledDays);

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "c1.C1Name"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            dt.Columns.Add("LSDRate", typeof(System.String));
            dt.Columns.Add("ShortageRate", typeof(System.String));
            CalcPMData(dt);

            return dt;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取显示未支付的OrderQty
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        private int GetUnPayQtyByStock(int productSysNo, int stockSysNo)
        {
            return GetUnPayQty(productSysNo, stockSysNo, 0);
        }

        /// <summary>
        /// 获取显示未支付的OrderQty
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        private int GetUnPayQtyTotal(int productSysNo)
        {
            return GetUnPayQty(productSysNo, 0, 1);
        }

        private int GetUnPayQty(int productSysNo, int stockSysNo, int queryFlag)
        {
            var command = DataCommandManager.GetDataCommand("QueryUnPayQty");
            command.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@whNo", stockSysNo.ToString());
            command.SetParameterValue("@Flag", queryFlag);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        #endregion 私有方法

        /// <summary>
        /// 计算PM工作指标数据的 缺货率 和 低于可销售天数占比
        /// </summary>
        /// <param name="dtPM"></param>
        private void CalcPMData(DataTable dtPM)
        {
            foreach (DataRow dr in dtPM.Rows)
            {
                //计算低于可销售天数占比
                decimal rate = Convert.ToDecimal(dr["LSD"]) / Convert.ToDecimal(dr["Total"]);
                dr["LSDRate"] = Math.Round(rate * 100, 2).ToString() + "%";

                //计算缺货率
                rate = Convert.ToDecimal(dr["Shortage"]) / Convert.ToDecimal(dr["Total"]);
                dr["ShortageRate"] = Math.Round(rate * 100, 2).ToString() + "%";
            }
        }

        /// <summary>
        /// 获取商品归属PM
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public int GetProductBelongPMSysNo(int productSysNo)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductBelongPMSysNo");
            dataCommand.SetParameterValue("@SysNo", productSysNo);
            Object result = dataCommand.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取商品归属Vendor
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public int GetProductBelongVendorSysNo(int productSysNo)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductBelongVendorSysNo");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            Object result = dataCommand.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取滞销库存详细信息
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public List<UnmarketabelInventoryInfo> GetUnmarketableInventoryInfo(int productSysNo, string companyCode)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetUnmarketableInventoryInfo");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return dataCommand.ExecuteEntityList<UnmarketabelInventoryInfo>();
        }

        /// <summary>
        /// 商品入库出库报表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductCostInAndCostOutReport(CostInAndCostOutReportQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductCostInAndCostOutReport");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, pagingInfo, "p.[SysNo] DESC"))
            {
                builder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, @"(
EXISTS(SELECT TOP 1 1 FROM @tCostIn c WHERE p.SysNo = c.ProductSysNo)OR 
EXISTS(SELECT TOP 1 1 FROM @tCostOut c WHERE p.SysNo = c.ProductSysNo))");

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c1.SysNo", DbType.Int32,
 "@C1SysNo", QueryConditionOperatorType.Equal, filter.Category1SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c2.SysNo", DbType.Int32,
 "@C2SysNo", QueryConditionOperatorType.Equal, filter.Category2SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c3.SysNo", DbType.Int32,
 "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.ProductID", DbType.String,
"@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.ProductName", DbType.String,
 "@ProductName", QueryConditionOperatorType.Like, filter.ProductName);

                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "brand.SysNo", DbType.Int32, filter.BrandSysNoList);

                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "vendor.SysNo", DbType.Int32, filter.VendorSysNoList);

                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "stk.SysNo", DbType.Int32, filter.WarehouseSysNoList);

                command.CommandText = builder.BuildQuerySql();
                command.CommandText = command.CommandText.Replace("#strWhereDate#", string.Format(" (c.InDate BETWEEN '{0}' AND '{1}') "
                    , filter.DateTimeFrom.HasValue ? filter.DateTimeFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse("1900-01-01 00:00:00").ToString("yyyy-MM-dd HH:mm:ss")
                    , filter.DateTimeTo.HasValue ? filter.DateTimeTo.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse("2100-12-31 23:59:59").ToString("yyyy-MM-dd HH:mm:ss")));

                DataTable dt = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 商品库龄报表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductStockAgeReport(StockAgeReportQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
             {
                 SortField = filter.PagingInfo.SortBy,
                 StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                 MaximumRows = filter.PagingInfo.PageSize
             };

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductStockAgeReport");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, pagingInfo, "p.[SysNo] DESC"))
            {

                command.AddInputParameter("@StatisticDate", DbType.DateTime, filter.StatisticDate.AddDays(1).AddSeconds(-1));

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c1.SysNo", DbType.Int32,
"@C1SysNo", QueryConditionOperatorType.Equal, filter.C1SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c2.sysno", DbType.Int32,
"@C2SysNo", QueryConditionOperatorType.Equal, filter.C2SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c3.sysno", DbType.Int32,
"@C3SysNo", QueryConditionOperatorType.Equal, filter.C3SysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo", DbType.Int32,
"@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo", DbType.Int32,
"@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.sysno", DbType.Int32,
"@VendorSysNo", QueryConditionOperatorType.Equal, filter.VendorSysNo);

                #region 根据选择的库龄类型构造时间条件
                StringBuilder strUpdateProductStockAgeInventory = new StringBuilder();
                foreach (string stockAgeTypeID in filter.StockAgeTypeList)
                {
                    Tuple<DateTime, DateTime, string> dateRange = GetStockAgeDateRange(filter.StatisticDate.Date, stockAgeTypeID);

                    strUpdateProductStockAgeInventory.AppendFormat(@"UPDATE t
SET t.Amt{2} = i.Amount,t.Qty{2}  = i.Quantity
FROM @ProductStockAgeInventory t
LEFT JOIN ipp3.dbo.FN_EC_GetProductPOInventory('{0}','{1}') i
ON t.ProductSysNo = i.ProductSysNo AND t.StockSysNo = i.StockSysNo AND t.VendorSysNo = i.VendorSysNo;"
                        , dateRange.Item1.ToString("yyyy-MM-dd HH:mm:ss"), dateRange.Item2.ToString("yyyy-MM-dd HH:mm:ss"), dateRange.Item3);
                    strUpdateProductStockAgeInventory.AppendLine();
                }
               
                #endregion

                command.CommandText = builder.BuildQuerySql();
                command.CommandText = command.CommandText.Replace("#strUpdateProductStockAgeInventory#", strUpdateProductStockAgeInventory.ToString());

                DataTable dt = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private Tuple<DateTime, DateTime, string> GetStockAgeDateRange(DateTime baseDate, string stockAgeTypeID)
        {
            switch (stockAgeTypeID)
            {
                case "1":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-1), baseDate.AddDays(1).AddSeconds(-1),"1m");
                case "2":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-2), baseDate.AddMonths(-1).AddSeconds(-1), "1m2m");
                case "3":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-3), baseDate.AddMonths(-2).AddSeconds(-1), "2m3m");
                case "4":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-6), baseDate.AddMonths(-3).AddSeconds(-1), "3m6m");
                case "5":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-9), baseDate.AddMonths(-6).AddSeconds(-1), "6m9m");
                case "6":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddMonths(-12), baseDate.AddMonths(-9).AddSeconds(-1), "9m12m");
                case "7":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddYears(-2), baseDate.AddYears(-1).AddSeconds(-1), "1y2y");
                case "8":
                    return new Tuple<DateTime, DateTime, string>(baseDate.AddYears(-3), baseDate.AddYears(-2).AddSeconds(-1), "2y3y");
                case "9":
                    return new Tuple<DateTime, DateTime, string>(DateTime.Parse("1900-01-01"), baseDate.AddYears(-3).AddSeconds(-1), "3y");
                default:
                    throw new NotImplementedException("不支持的库龄类型！");
            }
        }
    }
}
