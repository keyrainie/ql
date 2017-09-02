using System;
using System.Data;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Text.RegularExpressions;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IWarehouseOwnerQueryDA))]
    public class WarehouseOwnerQueryDA : IWarehouseOwnerQueryDA
    {
        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "OWNERSYSNO":
                        tsort[0] = "a.SysNo";
                        break;
                    case "OWNERID":
                        tsort[0] = "a.OwnerID";
                        break;
                    case "OWNERNAME":
                        tsort[0] = "a.OwnerName";
                        break;
                    case "OWNERTYPE":
                        tsort[0] = "a.OwnerType";
                        break;
                    case "OWNERSTATUS":
                        tsort[0] = "a.Status";
                        break;
                    case "OWNERMEMO":
                        tsort[0] = "ISNULL(a.OwnerMemo,'')";
                        break;                    
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        /// <summary>
        /// 查询仓库所有者
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryWarehouseOwner(WarehouseOwnerQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(filter.PagingInfo.SortBy);
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryWarehouseOwner");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.OwnerName", DbType.String, "@OwnerName",
                        QueryConditionOperatorType.Like, filter.OwnerName);
                if (filter.OwnerStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.[Status]", DbType.Int32, "@OwnerStatus",
                            QueryConditionOperatorType.Equal, filter.OwnerStatus);
                }
                if (filter.OwnerType.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.[OwnerType]", DbType.Int32, "@OwnerType",
                            QueryConditionOperatorType.Equal, filter.OwnerType);
                }
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                        QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sb.BuildQuerySql();

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("OwnerStatus", typeof(ValidStatus));
                enumColumnList.Add("OwnerType", typeof(WarehouseOwnerType));
                
                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }

        }        
    }
}
