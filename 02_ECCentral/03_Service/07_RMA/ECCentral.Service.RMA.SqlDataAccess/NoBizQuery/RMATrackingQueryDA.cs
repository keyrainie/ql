using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRMATrackingQueryDA))]
    public class RMATrackingQueryDA : IRMATrackingQueryDA
    {
        #region IRMATrackingQueryDA Members

        public virtual List<UserInfo> GetRMATrackingCreateUsers()
        {
            List<UserInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRMATrackingCreateUsers");
            cmd.SetParameterValue("@UserStatus", ECCentral.BizEntity.Customer.CustomerStatus.Valid);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<UserInfo, List<UserInfo>>(reader);
                return list;
            }
        }

        public virtual List<UserInfo> GetRMATrackingUpdateUsers()
        {
            List<UserInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRMATrackingUpdateUsers");
            cmd.SetParameterValue("@UserStatus", ECCentral.BizEntity.Customer.CustomerStatus.Valid);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<UserInfo, List<UserInfo>>(reader);
                return list;
            }
        }

        public virtual List<UserInfo> GetRMATrackingHandleUsers()
        {
            List<UserInfo> list;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRMATrackingHandleUsers");
            cmd.SetParameterValue("@UserStatus", ECCentral.BizEntity.Customer.CustomerStatus.Valid);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                list = DataMapper.GetEntityList<UserInfo, List<UserInfo>>(reader);
                return list;
            }
        }


        public virtual DataTable QueryRMATracking(RMATrackingQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryTracking");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " A.SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.RegisterSysNo", DbType.Int32, "@RegisterSysNo", QueryConditionOperatorType.Equal, filter.RegisterSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.SOSysNo", DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.CreateTime", DbType.DateTime, "@CreateTime", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.CreateTimeFrom, filter.CreateTimeTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.UpDateTime", DbType.DateTime, "@CloseTime", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.CloseTimeFrom, filter.CloseTimeTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CreateUserSysNo", DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, filter.CreateUserSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.UpdateUserSysNo", DbType.Int32, "@UpdateUserSysNo", QueryConditionOperatorType.Equal, filter.UpdateUserSysNo);

                if (filter.NextHandler != null)
                {
                    ConditionConstructor subQueryBuilder = sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "Select SysNo From dbo.RMA_Register RMA with(nolock)");
                    subQueryBuilder.AddCustomCondition(QueryConditionRelationType.AND, "RMA.SysNo=A.RegisterSysNo");
                    subQueryBuilder.AddCondition(QueryConditionRelationType.AND, "RMA.NextHandler", DbType.Int32, "@NextHandler", QueryConditionOperatorType.Equal, filter.NextHandler);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.[CompanyCode]", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(InternalMemoStatus));
                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                codeNameList.Add("Source", "RMA", "RMAInternalMemoSourceType");
                DataTable dt = cmd.ExecuteDataTable(enumList, codeNameList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
