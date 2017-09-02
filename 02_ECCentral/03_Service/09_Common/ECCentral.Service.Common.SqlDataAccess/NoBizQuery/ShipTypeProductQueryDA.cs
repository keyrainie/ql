using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;
using System.Data;
using ECCentral.Service.Common.SqlDataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IShipTypeProductQueryDA))]
    public class ShipTypeProductQueryDA : IShipTypeProductQueryDA
    {
        /// <summary>
        /// 查询配送方式-产品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryShipTypeProductList(ShipTypeProductQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);
            totalCount = 0;
            object _itemRange = null; ;
            object _type = null;
            object _companyCustomer;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeProduct_QueryShipTypeProduct");
            PagingInfoEntity pagingInfo = ToPagingInfo(filter.PagingInfo);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "B.SysNo DESC"))
            {
                if (filter.ShipTypeProductType.HasValue && EnumCodeMapper.TryGetCode(filter.ShipTypeProductType, out _type))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.[Type]", DbType.String, "@Type", QueryConditionOperatorType.Equal, _type);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Description", DbType.String, "@Description", QueryConditionOperatorType.Like, filter.Description);
                if (filter.ProductRange.HasValue && EnumCodeMapper.TryGetCode(filter.ProductRange, out _itemRange))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ItemRange", DbType.String, "@ItemRange", QueryConditionOperatorType.Equal, _itemRange);
                }
                if (filter.CompanyCustomer != null && EnumCodeMapper.TryGetCode(filter.CompanyCustomer,out _companyCustomer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "B.CompanyCustomer",
                        DbType.String,
                        "@CompanyCustomer",
                        QueryConditionOperatorType.Equal,
                        _companyCustomer);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShippingType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.StockSysNo", DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.WareHouse);
                if (!filter.Category2.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "E.Category1Sysno", DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal, filter.Category1);
                }
                if (filter.Category2.HasValue&&!filter.Category3.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "E.Category2Sysno", DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal, filter.Category2);
                }
                if (filter.Category3.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "E.Category3Sysno", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3);
                }
                if (filter.AreaSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.SysNO", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.AreaSysNo);
                }
                else if (filter.CitySysNo != null && filter.DistrictSysNo == null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.CitySysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.CitySysNo", DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNo);
                }
                else if (filter.ProvinceSysNo.HasValue )
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.SysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.ProvinceSysNo", DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNo);

                }
                
               
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList EnumList = new EnumColumnList();
                EnumList.Add("ItemRange", typeof(ECCentral.BizEntity.Common.ProductRange));
                EnumList.Add("Type", typeof(ECCentral.BizEntity.Common.ShipTypeProductType));
                EnumList.Add("CompanyCustomer", typeof(ECCentral.BizEntity.Common.CompanyCustomer));
                DataTable dt = cmd.ExecuteDataTable(EnumList);
                totalCount = int.Parse(cmd.GetParameterValue("TotalCount").ToString());

                return dt;
            }
        }
        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            return new PagingInfoEntity()
            {
                SortField = (pagingInfo.SortBy == null ? "" : pagingInfo.SortBy),
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        #region 排序列映射
        private static void MapSortField(ShipTypeProductQueryFilter filter)
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
                        filter.PagingInfo.SortBy = sortFiled.Replace("SysNo", "B.SysNo");
                        break;
                    case "Type":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Type", "B.Type");
                        break;
                    case "ProductID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ProductID", "A.ProductID");
                        break;
                    case "ShipTypeName":
                        filter.PagingInfo.SortBy = sortFiled.Replace("ShipTypeName", "A.ShipTypeSysNo");
                        break;
                    case "StockSysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("StockSysNo", "A.StockSysNo");
                        break;
                    case "EditDate":
                        filter.PagingInfo.SortBy = sortFiled.Replace("EditDate", "B.EditDate");
                        break;
                }
            }
        }
        #endregion
    }
}
