using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IOrdrQueryDA))]
    public class OrdrQueryDA : IOrdrQueryDA
    {
        public DataTable Query(OrderQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CPSOrderQuery");
            StringBuilder whereStrSO = new StringBuilder("WHERE 1 = 1 ");
            StringBuilder whereStrRMA = new StringBuilder("WHERE 1 = 1 ");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //单据类型
                object _type;
                if (filter.OrderType != null && EnumCodeMapper.TryGetCode(filter.OrderType, out _type))
                {
                    whereStrSO.Append(" AND csItem.Type = @Type ");
                    whereStrRMA.Append(" AND csItem.Type = @Type ");
                    cmd.AddInputParameter("@Type", DbType.String, filter.OrderType);
                }

                if (!string.IsNullOrEmpty(filter.OrderSysNoList))
                {
                    string orderSysNoList = filter.OrderSysNoList.Replace('.', ',');

                    whereStrSO.Append(" AND csItem.OrderSysNo IN (" + orderSysNoList + ") ");
                    whereStrRMA.Append(" AND csItem.OrderSysNo IN (" + orderSysNoList + ") ");
                }

                //主渠道名称
                if (!string.IsNullOrEmpty(filter.MasterChannelID))
                {
                    whereStrSO.Append(" AND cpsUser.CustomerID = @MasterChannelID ");
                    whereStrRMA.Append(" AND cpsUser.CustomerID = @MasterChannelID ");
                    cmd.AddInputParameter("@MasterChannelID", DbType.String, filter.MasterChannelID);
                }

                //渠道来源【Source】
                if (!string.IsNullOrEmpty(filter.SubChannelID))
                {
                    whereStrSO.Append(" AND channel.Source = @SubChannelID ");
                    whereStrRMA.Append(" AND channel.Source = @SubChannelID ");
                    cmd.AddInputParameter("@SubChannelID", DbType.String, filter.SubChannelID);
                }
                //下单时间【SO RMA创建时间】
                if (filter.CreateDateBegin.HasValue)
                {
                    whereStrSO.Append(" AND ippSO.OrderDate >= @CreateDateBegin ");
                    whereStrRMA.Append(" AND cpsRMA.CreateTime >= @CreateDateBegin ");
                    cmd.AddInputParameter("@CreateDateBegin", DbType.DateTime, filter.CreateDateBegin);
                }
                if (filter.CreateDateEnd.HasValue)
                {
                    whereStrSO.Append(" AND ippSO.OrderDate < @CreateDateEnd ");
                    whereStrRMA.Append(" AND cpsRMA.CreateTime < @CreateDateEnd ");
                    cmd.AddInputParameter("@CreateDateEnd", DbType.DateTime, filter.CreateDateEnd.Value.AddDays(1));
                }

                //交易完成日期
                if (filter.FinishDateBegin.HasValue)
                {
                    whereStrSO.Append(" AND ippSO.OutTime >= @FinishDateBegin ");
                    whereStrRMA.Append(" AND cpsRMA.RefundTime >= @FinishDateBegin ");
                    cmd.AddInputParameter("@FinishDateBegin", DbType.DateTime, filter.FinishDateBegin);
                }
                if (filter.FinishDateEnd.HasValue)
                {
                    whereStrSO.Append(" AND ippSO.OutTime < @FinishDateEnd ");
                    whereStrRMA.Append(" AND cpsRMA.RefundTime < @FinishDateEnd ");
                    cmd.AddInputParameter("@FinishDateEnd", DbType.DateTime, filter.FinishDateEnd.Value.AddDays(1));
                }

                //结算日期
                if (filter.SettlementDateBegin.HasValue)
                {
                    whereStrSO.Append(" AND csMaster.SettledTime >= @SettlementDateBegin ");
                    whereStrRMA.Append(" AND csMaster.SettledTime >= @SettlementDateBegin ");
                    cmd.AddInputParameter("@SettlementDateBegin", DbType.DateTime, filter.SettlementDateBegin);
                }
                if (filter.SettlementDateEnd.HasValue)
                {
                    whereStrSO.Append(" AND csMaster.SettledTime < @SettlementDateEnd ");
                    whereStrRMA.Append(" AND csMaster.SettledTime < @SettlementDateEnd ");
                    cmd.AddInputParameter("@SettlementDateEnd", DbType.DateTime, filter.SettlementDateEnd.Value.AddDays(1));
                }

                //结算状态
                object _status;
                if (filter.SettledStatus != null && EnumCodeMapper.TryGetCode(filter.SettledStatus, out _status))
                {
                    whereStrSO.Append(" AND csItem.Status = @SettledStatus ");
                    whereStrRMA.Append(" AND csItem.Status = @SettledStatus ");
                    cmd.AddInputParameter("@SettledStatus", DbType.String, filter.SettledStatus);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.CommandText = cmd.CommandText.Replace("#StrWhereSO#", whereStrSO.ToString());
                cmd.CommandText = cmd.CommandText.Replace("#StrWhereRMA#", whereStrRMA.ToString());

            
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("SettledStatus", typeof(FinanceStatus));
                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                if (totalCount > 0)
                {
                    dt = this.GetOrderStatus(dt, "OrderStatus");
                }

                return dt;
            }
        }

        private DataTable GetOrderStatus(DataTable dt, string colName)
        {
            string colNameTmp = dt.Columns[colName].ColumnName + "_ECCentral_Auto_Removed_820319";
            dt.Columns[colName].ColumnName = colNameTmp;
            dt.Columns.Add(colName, typeof(string));
            foreach (DataRow r in dt.Rows)
            {
                if (r["Type"] != DBNull.Value && r[colNameTmp] != DBNull.Value)
                {
                    if (r["Type"].ToString().Contains("SO"))
                    {
                        switch (r[colNameTmp].ToString())
                        {
                            case "0":
                                r[colName] = "待审核";
                                break;
                            case "1":
                                r[colName] = "待出库";
                                break;
                            case "2":
                                r[colName] = "待支付";
                                break;
                            case "3":
                                r[colName] = "待主管审";
                                break;
                            case "4":
                                r[colName] = "已出库";
                                break;
                            case "-1":
                                r[colName] = "欧亚作废";
                                break;
                            case "-2":
                                r[colName] = "客户作废";
                                break;
                            case "-3":
                                r[colName] = "主管作废";
                                break;
                            case "-4":
                                r[colName] = "系统自动作废";
                                break;
                            case "-5":
                                r[colName] = "BackOrder";
                                break;
                            case "-6":
                                r[colName] = "已拆分";
                                break;
                            default:
                                break;
                        }
                    }
                    else if (r["Type"].ToString().Contains("RMA"))
                    {
                        switch (r[colNameTmp].ToString())
                        {
                            case "0":
                                r[colName] = "待退款";
                                break;
                            case "2":
                                r[colName] = "已退款";
                                break;
                            case "3":
                                r[colName] = "待审核";
                                break;
                            case "-1":
                                r[colName] = "作废";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return dt;
        }
    }
}
