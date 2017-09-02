using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IConsignAdjustDA))]
    public class ConsignAdjustDA : IConsignAdjustDA
    {
        public DataTable Query(ConsignAdjustQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = -1;
                return null;
            }

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignAdjustList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "C.SysNo DESC"))
            {

                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.VendorSysNo",
                      DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }

                if (queryFilter.PMSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.PMSysNo",
                  DbType.Int32, "@PMSysNo", QueryConditionOperatorType.Equal, queryFilter.PMSysNo.Value);
                }

                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status.Value);
                }
                //if (queryFilter.SettleSysno.HasValue)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SettleSysno",
                //   DbType.Int32, "@SettleSysno", QueryConditionOperatorType.Equal, queryFilter.SettleSysno.Value);
                //}

                if (queryFilter.SettleRange.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.SettleRange",
                   DbType.String, "@SettleRange", QueryConditionOperatorType.Equal, queryFilter.SettleRange.Value.ToString("yyyy-MM"));
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                command.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                command.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                EnumColumnList columnEnums = new EnumColumnList();
                columnEnums.Add("Status", typeof(ConsignAdjustStatus));
                DataTable dt = command.ExecuteDataTable(columnEnums);
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public ConsignAdjustInfo LoadInfo(int SysNo)
        {
            ConsignAdjustInfo result = null;
            //DataCommand command = DataCommandManager.GetDataCommand("LoadConsignAdjust");
            DataCommand command = DataCommandManager.GetDataCommand("LoadConsignAdjustBySysNo");
            command.SetParameterValue("@SysNo", SysNo);
            result = command.ExecuteEntity<ConsignAdjustInfo>();
            if (result != null)
            {
                result.ItemList = LoadInfoItem(result.SysNo.Value);
            }
            return result;
        }
        private List<ConsignAdjustItemInfo> LoadInfoItem(int ConsignAdjustSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadConsignAdjustItem");
            command.SetParameterValue("@ConsignAdjustSysNo", ConsignAdjustSysNo);
            return command.ExecuteEntityList<ConsignAdjustItemInfo>();
        }

        public ConsignAdjustInfo Create(ConsignAdjustInfo request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateConsignAdjust");
            command.SetParameterValue("@VendorSysNo", request.VenderSysNo);
            command.SetParameterValue("@PMSysNo", request.PMSysNo);
            command.SetParameterValue("@SettleSysNo", request.SettleSysNo);
            command.SetParameterValue("@SettleRange", request.SettleRangeDate.Value.ToString("yyyy-MM"));
            command.SetParameterValue("@TotalAmt", request.TotalAmt);
            command.SetParameterValue("@Status", request.Status);
            command.SetParameterValueAsCurrentUserSysNo("@InUser");
            request.SysNo = command.ExecuteScalar<int>();
            if (request.SysNo.HasValue && request.SysNo.Value > 0)
            {

                return request;
            }
            else
            {
                return null;
            }
        }

        public ConsignAdjustInfo Update(ConsignAdjustInfo request)
        {
            //目前只能更新item
            UpdateTotalAmt(request);
            UpdateItem(request);
            return request;
        }
        public bool CreateConsignAdjustItem(ConsignAdjustItemInfo request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateConsignAdjustItem");
            command.SetParameterValue("@ConsignAdjustSysNo", request.ConsignAdjustSysNo);
            command.SetParameterValue("@DeductSysNo", request.DeductSysNo);
            command.SetParameterValue("@DeductAmt", request.DeductAmt);
            return command.ExecuteNonQuery() > 0;
        }
        private bool DeleteConsignAdjustItemByConsignAdjust(int ConsignAdjustSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteConsignAdjustItem");
            command.SetParameterValue("@ConsignAdjustSysNo", ConsignAdjustSysNo);
            return command.ExecuteNonQuery() > 0;
        }
        public ConsignAdjustInfo UpdateItem(ConsignAdjustInfo request)
        {
            //更新只更改item,全删再新加
            DeleteConsignAdjustItemByConsignAdjust(request.SysNo.Value);
            foreach (var item in request.ItemList)
            {
                item.ConsignAdjustSysNo = request.SysNo.Value;
                CreateConsignAdjustItem(item);
            }
            return request;
        }


        public ConsignAdjustInfo UpdateStatus(ConsignAdjustInfo request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConsignAdjustStatus");
            command.SetParameterValue("@Status", (int)request.Status.Value);
            command.SetParameterValue("@SysNo", request.SysNo);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUser");
            if (command.ExecuteNonQuery() > 0)
            {
                return request;
            }
            else
            {
                return null;
            }
        }

        public ConsignAdjustInfo UpdateTotalAmt(ConsignAdjustInfo request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConsignAdjustTotalAmt");
            command.SetParameterValue("@TotalAmt", request.TotalAmt);
            command.SetParameterValue("@SysNo", request.SysNo);
            if (command.ExecuteNonQuery() > 0)
            {
                return request;
            }
            else
            {
                return null;
            }
        }

        public ConsignAdjustInfo Delete(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteConsignAdjust");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<ConsignAdjustInfo>();
        }
    }
}
