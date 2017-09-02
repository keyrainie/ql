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

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShipTypeAreaPriceQueryDA))]
   public  class ShipTypeAreaPriceQueryDA:IShipTypeAreaPriceQueryDA
    {
        /// <summary>
        /// 查询配送方式-产品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryShipTypeAreaPriceList(ShipTypeAreaPriceQueryFilter filter, out int TotalCount)
        {
            MapSortField(filter);
            TotalCount = 0;
            PagingInfoEntity pagingInfo = ToPagingInfo(filter.PagingInfo);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeAreaPrice_QueryShipTypeAreaPrice");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "SAP.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SAP.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShipTypeSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SAP.MerchantSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                if (filter.CompanyCustomer.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SAP.CompanyCustomer", DbType.Int32, "@CompanyCustomer", QueryConditionOperatorType.Equal, (int)filter.CompanyCustomer.Value);
                }

                if (!string.IsNullOrEmpty(filter.AreaName))
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ProvinceName", DbType.String, "@ProvinceName", QueryConditionOperatorType.Equal, filter.AreaName);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CityName", DbType.String, "@CityName", QueryConditionOperatorType.Equal, filter.AreaName);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.DistrictName", DbType.String, "@DistrictName", QueryConditionOperatorType.Equal, filter.AreaName);
                    }
                }
                if (filter.AreaSysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNO", DbType.String, "@SysNO", QueryConditionOperatorType.Equal, filter.AreaSysNo);
                    }
                }
                else if (filter.CitySysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.CitySysNo);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNo);

                    }
                }
                else if (filter.ProvinceSysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNo);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNo);
                    }
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable("CompanyCustomer", typeof(ECCentral.BizEntity.Common.CompanyCustomer));
                TotalCount = int.Parse(cmd.GetParameterValue("@TotalCount").ToString());
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
        private static void MapSortField(ShipTypeAreaPriceQueryFilter filter)
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
                        filter.PagingInfo.SortBy = sortFiled.Replace("SysNo", "SAP.SysNo");
                        break;
                    case "AreaName":
                        filter.PagingInfo.SortBy = sortFiled.Replace("AreaName", "A.ProvinceName");
                        break;
                }
            }
        }
        #endregion
    }
}
