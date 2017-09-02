using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.SO.IDataAccess;
using System.Data;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISOLogisticDA))]
    public class SOLogisticDA : ISOLogisticDA
    {

        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = pagingInfo.SortBy,
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        public DataTable QueryDiffSODelivery(SODeliveryDiffFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryDeliveryDiff");
            if (!string.IsNullOrEmpty(filter.PageInfo.SortBy))
            {
                filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("SOSysNo", "D.OrderSysNo");

                if (filter.PageInfo.SortBy.Contains("OrderStatus"))
                {
                    filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("OrderStatus", "SM.Status");
                }
                else
                {
                    filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("Status", "D.Status");
                }
                filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("DeliveryDate", "D.DeliveryDate");
                filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("DeliveryTimeRange", "D.DeliveryTimeRange");
                filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("DeliveryMemo", "D.DeliveryMemo");
                filter.PageInfo.SortBy = filter.PageInfo.SortBy.Replace("Note", "D.Note");

            }
            DataTable result = null;

            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PageInfo), "D.OrderSysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.Status", DbType.Int32, "@OrderStatus", QueryConditionOperatorType.Equal, filter.SOStatus);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, filter.FreightMen);

                if (filter.SOSysNo.HasValue && filter.SOSysNo !=0)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo.Value);
                }

                if (filter.DeliveryAreaNo.HasValue && filter.DeliveryAreaNo != 0)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.AreaSysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.DeliveryAreaNo.Value);
                }

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.DeliveryDateTimeFrom);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateTo", QueryConditionOperatorType.LessThan, filter.DeliveryDateTimeFrom);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 1);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);


                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return result;
            }

            
        }

        public int MarkDeliveryExp(int orderSysNo,int orderType,string companyCode,int opUser)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MarkDeliveryExp");
            command.SetParameterValue("@OrderSysNo", orderSysNo);
            command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@OpUser", opUser);
            
            return command.ExecuteNonQuery();
           
        }
    }
}
