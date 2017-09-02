using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IOldChangeNewQueryDA))]
    public class OldChangeNewQueryDA : IOldChangeNewQueryDA
    {
        /// <summary>
        /// 以旧换新补贴款查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns></returns>
        public DataTable OldChangeNewQuery(OldChangeNewQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Query_OldChangeNew");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "result.SysNo desc"))
            {
                AddOldChangeNewParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(OldChangeNewStatus));
                cmd.ConvertEnumColumn(dt, enumColList);

                dt.Columns.Add("StatusCode", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["StatusCode"] = row["Status"].ToString();
                }

                #region 添加计算列
                dt.Columns.Add("TotalRebate", typeof(decimal));
                dt.Columns.Add("TotalReviseRebate", typeof(decimal));
                dt.Columns.Add("TotalPassReviseRebate", typeof(decimal));
                dt.Columns.Add("TotalReturnRebate", typeof(decimal));
                dt.Columns.Add("TotalReturnReviseRebate", typeof(decimal));

                if (dt != null
                    && dt.Rows.Count > 0)
                {
                    decimal TotalRebate = Convert.ToDecimal(cmd.GetParameterValue("TotalRebate"));
                    decimal TotalReviseRebate = Convert.ToDecimal(cmd.GetParameterValue("TotalReviseRebate"));
                    decimal TotalPassReviseRebate = Convert.ToDecimal(cmd.GetParameterValue("TotalPassReviseRebate") == DBNull.Value ? 0m : cmd.GetParameterValue("TotalPassReviseRebate"));
                    decimal TotalReturnReviseRebate = Convert.ToDecimal(cmd.GetParameterValue("TotalReturnReviseRebate") == DBNull.Value ? 0m : cmd.GetParameterValue("TotalReturnReviseRebate"));

                    dt.Rows[0]["TotalRebate"] = TotalRebate;
                    dt.Rows[0]["TotalReviseRebate"] = TotalReviseRebate;
                    dt.Rows[0]["TotalPassReviseRebate"] = TotalPassReviseRebate;
                    dt.Rows[0]["TotalReturnReviseRebate"] = TotalReturnReviseRebate;

                }
                #endregion

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddOldChangeNewParameters(OldChangeNewQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "rb.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
            sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");
            if (!string.IsNullOrEmpty(filter.OrderNoList))
            {
                filter.OrderNoList = filter.OrderNoList.Replace(" ", "");
                filter.OrderNoList = filter.OrderNoList.Replace(".", ",");
                sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("rb.SoSysNo in ({0})", filter.OrderNoList));
            }
            else if (filter.OrderNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "rb.SoSysNo",
                 DbType.Int32,
                 "@Sosysno",
                 QueryConditionOperatorType.Equal,
                 filter.OrderNo);
            }
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "rb.TradeInId",
                DbType.String,
                "@TradeInId",
                QueryConditionOperatorType.Equal,
                filter.ApplyID);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "rb.Licence",
                DbType.String,
                "@Licence",
                QueryConditionOperatorType.Equal,
                filter.CertificateNo);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "vm.CustomerSysNo",
                 DbType.Int32,
                 "@CustomerSysNo",
                 QueryConditionOperatorType.Equal,
                 filter.CustomerNo);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "C.CustomerID",
                 DbType.String,
                 "@CustomerID",
                 QueryConditionOperatorType.Equal,
                 filter.CustomerID);
            if (!string.IsNullOrEmpty(filter.ProductType))
            {
                sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(@"EXISTS(
                    SELECT i.SoSysNo FROM ipp3.dbo.V_SO_Item i
                    INNER JOIN IPP3.dbo.Product p
                    ON p.SysNo=i.ProductSysNo 
                    WHERE p.C3SysNo IN({0}) AND i.SOSysNo=rb.SoSysNo)", filter.ProductType));
            }
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "rb.InDate",
               DbType.DateTime,
               "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
               filter.CreateDateFrom);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "rb.InDate",
               DbType.DateTime,
               "@InDateTo",
               QueryConditionOperatorType.LessThanOrEqual,
               filter.CreateDateTo);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "rb.ConfirmDate",
               DbType.DateTime,
               "@ConfirmDateFrom",
               QueryConditionOperatorType.MoreThanOrEqual,
               filter.CompleteDateFrom);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "rb.ConfirmDate",
               DbType.DateTime,
               "@ConfirmDateTo",
               QueryConditionOperatorType.LessThanOrEqual,
               filter.CompleteDateTo);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "rb.Status",
              DbType.Int32,
              "@Status",
              QueryConditionOperatorType.Equal,
              filter.Status);
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "rb.ReferenceID",
              DbType.String,
              "@ReferenceID",
              QueryConditionOperatorType.Equal,
              filter.RefundCertificate);

            cmd.AddOutParameter("@TotalRebate", DbType.String, 50);
            cmd.AddOutParameter("@TotalReviseRebate", DbType.String, 50);
            cmd.AddOutParameter("@TotalPassReviseRebate", DbType.String, 50);
            cmd.AddOutParameter("@TotalReturnReviseRebate", DbType.String, 50);
            cmd.CommandText = sb.BuildQuerySql();
        }

        /// <summary>
        /// 获取以旧换新补贴款信息列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<OldChangeNewInfo> GetOldChangeNewList(OldChangeNewQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Get_OldChangeNewInfo");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd,"rb.SysNo"))
            {
                if (filter.OrderNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "S.SoSysNo",
                     DbType.String,
                     "@Sosysno",
                     QueryConditionOperatorType.Equal,
                     filter.OrderNo);
                }
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "rb.SysNo",
                    DbType.AnsiStringFixedLength, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "S.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                cmd.CommandText = sb.BuildQuerySql();
                if (filter.Status.HasValue && filter.Status.Value == -99)
                {
                    cmd.CommandText = cmd.CommandText.Replace("@isNew@", "AND rb.Status>=0");
                }
                else
                {
                    cmd.CommandText = cmd.CommandText.Replace("@isNew@", string.Empty);
                }
                List<OldChangeNewInfo> list = cmd.ExecuteEntityList<OldChangeNewInfo>();

                list.ForEach(e =>
                    {
                        filter.SysNo = e.SoSysNo;
                        e.SOItems = GetSOItem(filter);
                    });
                return list;
            }
        }

        private List<SOListInfo> GetSOItem(OldChangeNewQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Get_OldChangeNewSOItem");
            cmd.AddInputParameter("@SoSysNo", System.Data.DbType.String, filter.OrderNo);
            cmd.AddInputParameter("@CompanyCode", System.Data.DbType.AnsiStringFixedLength, filter.CompanyCode);
            return cmd.ExecuteEntityList<SOListInfo>();
        }

        /// <summary>
        /// Check以旧换新信息是否有效
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool IsOldChangeNewSO(OldChangeNewQueryFilter filter)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_Get_OldChangeNewSO");
            cmd.AddInputParameter("@SOSysNo", System.Data.DbType.Int32, filter.OrderNo);
            cmd.AddInputParameter("@CompanyCode", System.Data.DbType.Int32, filter.CompanyCode);
            return cmd.ExecuteScalar<int>() > 0;
        }

        #region 排序列映射
        private static void MapSortField(OldChangeNewQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortFiled = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SysNo", "SysNo");
                        break;
                    case "OrderAmt":
                        filter.PagingInfo.SortBy = sortFiled.Replace("OrderAmt", "rb.OrderAmt");
                        break;
                    case "SOSysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SOSysNo", "rb.SOSysNo");
                        break;
                    case "InUser":
                        filter.PagingInfo.SortBy = sortFiled.Replace("InUser", "rb.InUser");
                        break;
                    case "InDate":
                        filter.PagingInfo.SortBy = sortFiled.Replace("InDate", "rb.InDate");
                        break;
                    case "ConfirmUser":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ConfirmUser", "rb.ConfirmUser");
                        break;
                    case "ConfirmDate":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ConfirmDate", "rb.ConfirmDate");
                        break;
                    case "TradeInId":
                        filter.PagingInfo.SortBy = sortFiled.Replace("TradeInId", "rb.TradeInId");
                        break;
                    case "Licence":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Licence", "rb.Licence");
                        break;
                    case "Rebate":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Rebate", "rb.Rebate");
                        break;
                    case "ReviseRebate":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ReviseRebate", "rb.ReviseRebate");
                        break;
                    case "ReferenceID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ReferenceID", "rb.ReferenceID");
                        break;
                    case "CustomerSysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CustomerSysNo", "S.CustomerSysNo");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Status", "rb.Status");
                        break;
                }
            }
        }
        #endregion
    }
}
