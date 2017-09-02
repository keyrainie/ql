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
    [VersionExport(typeof(IWarehouseQueryDA))]
    public class WarehouseQueryDA : IWarehouseQueryDA
    {

        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "SYSNO":
                        tsort[0] = "a.SysNo";
                        break;
                    case "WAREHOUSEID":
                        tsort[0] = "a.WarehouseID";
                        break;
                    case "WAREHOUSENAME":
                        tsort[0] = "a.WarehouseName";
                        break;
                    case "ADDRESS":
                        tsort[0] = "a.Address";
                        break;
                    case "CONTACT":
                        tsort[0] = "a.Contact";
                        break;
                    case "PHONENUMBER":
                        tsort[0] = "a.Phone";
                        break;
                    case "WAREHOUSESTATUS":
                        tsort[0] = "a.[Status]";
                        break;
                    case "OWNERNAME":
                        tsort[0] = "ISNULL(b.OwnerName, '')";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryWarehouse(WarehouseQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(filter.PageInfo.SortBy);
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryWarehouseListByCondition");//说明：EC_Central产品化时 调用的 SQL为：Inventory_QueryWarehouse 现在是中蛋定制化 不存在渠道仓库所以保持同IPP一致
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo ASC"))
            {
                bool isSysNo = false;
                //编号格式验证
                if (filter.SysNo != null && Regex.IsMatch(filter.SysNo, @"^[,\. ]*\d+[\d,\. ]*$"))
                {
                    filter.SysNo = String.Join(",", filter.SysNo.Split(new char[] { '.', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    isSysNo = true;
                }
                else
                {
                    filter.SysNo = null;
                }

                if (filter.WarehouseID != null && Regex.IsMatch(filter.WarehouseID, @"^[, ]*\w+[\w-#, ]*$"))
                {
                    filter.WarehouseID = "'" + String.Join("','", filter.WarehouseID.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) + "'";
                    isSysNo = true;
                }
                else
                {
                    filter.WarehouseID = null;
                }

                if (isSysNo)
                {
                    sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "a.SysNo", QueryConditionOperatorType.In, filter.SysNo);

                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "a.StockID", QueryConditionOperatorType.In, filter.WarehouseID);
                    sb.ConditionConstructor.EndGroupCondition();
                }
                
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.StockName", DbType.String, "@WarehouseName",
                        QueryConditionOperatorType.LeftLike, filter.WarehouseName);


                //sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //        "a.WarehouseOwnerSysNo", DbType.Int32, "@WarehouseOwnerSysNo",
                //        QueryConditionOperatorType.Equal, filter.OwnerSysNo);


                //if (!filter.OwnerSysNo.HasValue && filter.OwnerSysNo!=0)
                //{
                //    sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND
                //        , " ((a.WarehouseOwnerSysNo IN ( SELECT a.SysNo FROM   OverseaInventoryManagement.dbo.WarehouseOwner a WITH(NOLOCK)WHERE  a.OwnerType=1)) OR a.WarehouseOwnerSysNo IS NULL)");
                //}

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.[Status]", DbType.Int32, "@WarehouseStatus",
                        QueryConditionOperatorType.Equal, filter.WarehouseStatus);
                //sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //        "a.[WarehouseType]", DbType.Int32, "@WarehouseType",
                //        QueryConditionOperatorType.Equal, filter.WarehouseType);
                //sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //        "a.[WarehouseArea]", DbType.Int32, "@WarehouseArea",
                //        QueryConditionOperatorType.Equal, filter.WarehouseArea);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                        QueryConditionOperatorType.Equal, filter.CompanyCode);
                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("WarehouseStatus", typeof(ValidStatus));

                cmd.CommandText = sb.BuildQuerySql();
                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }

        }
    }
}
