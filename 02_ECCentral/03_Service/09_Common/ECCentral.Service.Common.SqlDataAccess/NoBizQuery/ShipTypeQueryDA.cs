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
    [VersionExport(typeof(IShipTypeQueryDA))]
    public class ShipTypeQueryDA : IShipTypeQueryDA
    {
        /// <summary>
        /// 查询配送方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryShipTypeList(ShipTypeQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipType_QueryShipType");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "ST.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.ShipTypeID", DbType.String, "@ShipTypeID", QueryConditionOperatorType.Equal, filter.ShipTypeID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.ShipTypeName", DbType.String, "@ShipTypeName", QueryConditionOperatorType.Like, filter.ShipTypeName);

                //if(filter.IsOnlineShow.HasValue&&filter.IsOnlineShow.Value==ECCentral.BizEntity.Common.HYNStatus.Hide)
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.IsOnlineShow", DbType.Int32, "@IsOnlineShow", QueryConditionOperatorType.NotEqual, ECCentral.BizEntity.Common.HYNStatus.Hide);
                //else
                if (filter.IsOnlineShow != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.IsOnlineShow", DbType.Int32, "@IsOnlineShow", QueryConditionOperatorType.Equal, filter.IsOnlineShow.GetHashCode());
                }
                if (filter.IsWithPackFee != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.IsWithPackFee", DbType.Int32, "@IsWithPackFee", QueryConditionOperatorType.Equal, filter.IsWithPackFee.GetHashCode());
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.OnlyForStockSysNo", DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ST.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("IsOnlineShow", typeof(ECCentral.BizEntity.Common.HYNStatus));
                enumList.Add("IsWithPackFee", typeof(ECCentral.BizEntity.MKT.NYNStatus));
                enumList.Add("DeliveryPromise", typeof(ECCentral.BizEntity.Common.DeliveryStatusFor24H));
                enumList.Add("PackStyle",typeof(ECCentral.BizEntity.Common.ShippingPackStyle));
                enumList.Add("ShipTypeEnum", typeof(ECCentral.BizEntity.Common.ShippingTypeEnum));
                enumList.Add("DeliveryType", typeof(ECCentral.BizEntity.Common.ShipDeliveryType));
                enumList.Add("StoreType", typeof(ECCentral.BizEntity.IM.StoreType));
                //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                //pairList.Add("PackStyle", "Common", "ShippingPackStyle");
                //pairList.Add("ShipTypeEnum", "Common", "ShipTypeEnum");
                //pairList.Add("DeliveryType", "Common", "DeliveryType");

                var dt = cmd.ExecuteDataTable(enumList);//<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                
                return dt;//.Tables[0];
            }
        }
        /// <summary>
        /// 查询配送方式-产品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryShipTypeProductList(ShipTypeProductQueryFilter filter,out int totalCount)
        {
            totalCount = 0;
            object _itemRange=null;;
            object _type=null;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeProduct_QueryShipTypeProduct");
            PagingInfoEntity pagingInfo = ToPagingInfo(filter.PagingInfo);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "SPI.SysNo DESC"))
            {
                if (filter.ShipTypeProductType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPI.[Type]", DbType.String, "@Type", QueryConditionOperatorType.Equal, EnumCodeMapper.TryGetCode(filter.ShipTypeProductType, out _type));
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPI.Description", DbType.String, "@Description", QueryConditionOperatorType.Like, filter.Description);
                if (filter.ProductRange.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPI.ItemRange", DbType.String, "@ItemRange", QueryConditionOperatorType.Equal, EnumCodeMapper.TryGetCode(filter.ProductRange, out _itemRange));
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPI.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPM.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShippingType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SPM.StockSysNo", DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, filter.WareHouse);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList EnumList = new EnumColumnList();
                EnumList.Add("ItemRange",typeof(ECCentral.BizEntity.Common.ProductRange));
                EnumList.Add("Type", typeof(ECCentral.BizEntity.Common.ShipTypeProductType));
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
        private static void MapSortField(ShipTypeQueryFilter filter)
        {
            if (filter.PageInfo != null && !string.IsNullOrEmpty(filter.PageInfo.SortBy))
            {
                var index = 0;
                index = filter.PageInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PageInfo.SortBy.Substring(0, filter.PageInfo.SortBy.Length - index);
                var sortFiled = filter.PageInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        filter.PageInfo.SortBy = sortFiled.Replace("SysNo", "ST.SysNo");
                        break;
                }
            }
        }
        #endregion
    }
}
