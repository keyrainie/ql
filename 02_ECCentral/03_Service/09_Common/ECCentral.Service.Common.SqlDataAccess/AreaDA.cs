using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IAreaDA))]
    public class AreaDA : IAreaDA
    {
        /// <summary>
        /// Load加载地区详情
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [Caching(new string[] { "sysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public AreaInfo Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetArea");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                AreaInfo Area= DataMapper.GetEntity<AreaInfo>(row);
                return Area;
            }
            return null;
        }

        /// <summary>
        /// 旧版Load加载地区详情（可以获取区信息）
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [Caching(new string[] { "sysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public AreaInfo OldLoad(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("OldGetArea");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                AreaInfo Area = DataMapper.GetEntity<AreaInfo>(row);
                return Area;
            }
            return null;
        }
        /// <summary>
        /// 创建地区
        /// </summary>
        /// <param name="areaInfo"></param>
        /// <returns></returns>
        public AreaInfo Create(AreaInfo areaInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateArea");
            cmd.SetParameterValue("@ProvinceSysNo", areaInfo.ProvinceSysNo);
            cmd.SetParameterValue("@CitySysNo", areaInfo.CitySysNo);
            cmd.SetParameterValue("@ProvinceName", areaInfo.ProvinceName);
            cmd.SetParameterValue("@CityName", areaInfo.CityName);
            cmd.SetParameterValue("@DistrictName", areaInfo.DistrictName);
            cmd.SetParameterValue("@OrderNumber", areaInfo.OrderNumber);
            cmd.SetParameterValue("@IsLocal", areaInfo.IsLocal);
            cmd.SetParameterValue("@Status", areaInfo.Status);
            cmd.SetParameterValue("@Rank", areaInfo.Rank);
            cmd.SetParameterValue("@WeightLimit", areaInfo.WeightLimit);
            cmd.SetParameterValue("@SOAmountLimit", areaInfo.SOAmountLimit);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            areaInfo.SysNo= (int)cmd.GetParameterValue("@SysNo");
            areaInfo.FullName = cmd.GetParameterValue("@FullName").ToString();
            return areaInfo;
        }
       /// <summary>
       /// 更新地区
       /// </summary>
       /// <param name="areaInfo"></param>
       /// <returns></returns>
        public AreaInfo Update(AreaInfo areaInfo)
        {
            var  cmd = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateArea");
            string sql= cmd.CommandText;
            cmd.SetParameterValue("@SysNo", areaInfo.SysNo);
            cmd.SetParameterValue("@ProvinceSysNo", areaInfo.ProvinceSysNo);
            cmd.SetParameterValue("@CitySysNo", areaInfo.CitySysNo);
            cmd.SetParameterValue("@ProvinceName", areaInfo.ProvinceName);
            cmd.SetParameterValue("@CityName", areaInfo.CityName);
            cmd.SetParameterValue("@DistrictName", areaInfo.DistrictName);
            cmd.SetParameterValue("@OrderNumber", areaInfo.OrderNumber);
            cmd.SetParameterValue("@IsLocal", areaInfo.IsLocal);
            cmd.SetParameterValue("@Status", areaInfo.Status);
            cmd.SetParameterValue("@Rank", areaInfo.Rank);
            cmd.SetParameterValue("@WeightLimit", areaInfo.WeightLimit);
            cmd.SetParameterValue("@SOAmountLimit", areaInfo.SOAmountLimit);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            return areaInfo;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAreaList(AreaQueryFilter filter, out int totalCount)
        {
            totalCount = 0;            
            CustomDataCommand customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAreaList");
            PagingInfoEntity pagingInfo = ToPagingInfo(filter.PagingInfo);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, pagingInfo, "SysNo DESC"))
            {
                if (filter.DistrictSysNumber.HasValue)
                {//精确查询区
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.DistrictSysNumber);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                }
                else 
                if (filter.CitySysNumber.HasValue)
                {//查询出市以及其下属的区
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);
                        
                    }
                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                }
                else if (filter.ProvinceSysNumber.HasValue)
                {//查询出省以及其下属的市和区
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                        //paramValues 设置为filter.ProvinceSysNumber是为了通过构造验证没有实际意义
                        //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "CitySysNo", DbType.String, "", QueryConditionOperatorType.IsNull, filter.ProvinceSysNumber);

                    }
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Rank", DbType.String, "@Rank", QueryConditionOperatorType.Equal, filter.Rank);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WeightLimit", DbType.Int32, "@WeightLimit", QueryConditionOperatorType.Equal, filter.WeightLimit);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOAmountLimit", DbType.Decimal, "@SOAmountLimit", QueryConditionOperatorType.Equal, filter.SOAmtLimit);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");

                customCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = customCommand.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(AreaStatus)}                   
                });

                totalCount = int.Parse(customCommand.GetParameterValue("TotalCount").ToString());

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

        [Caching(ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<AreaInfo> QueryProvinceAreaList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryProvinceAreaList");
            return cmd.ExecuteEntityList<AreaInfo>();
        }

        [Caching(new string[] { "provinceSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<AreaInfo> QueryCityAreaListByProvinceSysNo(int provinceSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryCityAreaListByProvinceSysNo");
            cmd.SetParameterValue("@ProvinceSysNo", provinceSysNo);
            return cmd.ExecuteEntityList<AreaInfo>();
        }

        [Caching(new string[] { "citySysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<AreaInfo> QueryDistrictAreaListByCitySysNo(int citySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryDistrictAreaListByCitySysNo");
            cmd.SetParameterValue("@CitySysNo", citySysNo);
            return cmd.ExecuteEntityList<AreaInfo>();
        }

        public void QueryCurrentAreaStructure(int sysNo, out AreaInfo currentAreaInfo, out List<AreaInfo> proviceList, out List<AreaInfo> cityList, out List<AreaInfo> districtList)
        {
            #region asseign default value

            currentAreaInfo = null;
            proviceList = null;
            cityList = null;
            districtList = null;

            #endregion asseign default value

            currentAreaInfo = this.Load(sysNo);
            proviceList = this.QueryProvinceAreaList();

            if (currentAreaInfo != null)
            {
                if (currentAreaInfo.ProvinceSysNo == null && currentAreaInfo.CitySysNo == null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.SysNo.GetValueOrDefault());
                    districtList = new List<AreaInfo>();
                }
                else if (currentAreaInfo.ProvinceSysNo != null && currentAreaInfo.CitySysNo == null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.ProvinceSysNo.GetValueOrDefault());
                    districtList = this.QueryDistrictAreaListByCitySysNo(currentAreaInfo.SysNo.GetValueOrDefault());
                }
                else if (currentAreaInfo.ProvinceSysNo != null && currentAreaInfo.CitySysNo != null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.ProvinceSysNo.GetValueOrDefault());
                    districtList = this.QueryDistrictAreaListByCitySysNo(currentAreaInfo.CitySysNo.GetValueOrDefault());
                }
            }
        }

        public void QueryCurrentAreaStructure_Old(int sysNo, out AreaInfo currentAreaInfo, out List<AreaInfo> proviceList, out List<AreaInfo> cityList, out List<AreaInfo> districtList)
        {
            #region asseign default value

            currentAreaInfo = null;
            proviceList = null;
            cityList = null;
            districtList = null;

            #endregion asseign default value

            currentAreaInfo = this.OldLoad(sysNo);
            proviceList = this.QueryProvinceAreaList();

            if (currentAreaInfo != null)
            {
                if (currentAreaInfo.ProvinceSysNo == null && currentAreaInfo.CitySysNo == null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.SysNo.GetValueOrDefault());
                    districtList = new List<AreaInfo>();
                }
                else if (currentAreaInfo.ProvinceSysNo != null && currentAreaInfo.CitySysNo == null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.ProvinceSysNo.GetValueOrDefault());
                    districtList = this.QueryDistrictAreaListByCitySysNo(currentAreaInfo.SysNo.GetValueOrDefault());
                }
                else if (currentAreaInfo.ProvinceSysNo != null && currentAreaInfo.CitySysNo != null)
                {
                    cityList = this.QueryCityAreaListByProvinceSysNo(currentAreaInfo.ProvinceSysNo.GetValueOrDefault());
                    districtList = this.QueryDistrictAreaListByCitySysNo(currentAreaInfo.CitySysNo.GetValueOrDefault());
                }
            }
        }

        /// <summary>
        /// 查询市级以上的地域
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAreaNoDistrictList(AreaQueryFilter filter, out int totalCount)
        {
            totalCount = 0;
            CustomDataCommand customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAreaList");
            PagingInfoEntity pagingInfo = ToPagingInfo(filter.PagingInfo);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, pagingInfo, "SysNo DESC"))
            {
                if (filter.DistrictSysNumber.HasValue)
                {//精确查询区
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.DistrictSysNumber);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                }
                else
                    if (filter.CitySysNumber.HasValue)
                    {//查询出市以及其下属的区
                        using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, filter.CitySysNumber);

                        }
                        //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                    }
                    else if (filter.ProvinceSysNumber.HasValue)
                    {//查询出省以及其下属的市和区
                        using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, filter.ProvinceSysNumber);
                            //paramValues 设置为filter.ProvinceSysNumber是为了通过构造验证没有实际意义
                            //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "CitySysNo", DbType.String, "", QueryConditionOperatorType.IsNull, filter.ProvinceSysNumber);

                        }
                    }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Rank", DbType.String, "@Rank", QueryConditionOperatorType.Equal, filter.Rank);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WeightLimit", DbType.Int32, "@WeightLimit", QueryConditionOperatorType.Equal, filter.WeightLimit);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOAmountLimit", DbType.Decimal, "@SOAmountLimit", QueryConditionOperatorType.Equal, filter.SOAmtLimit);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");
                sqlBuilder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "[DistrictName]",QueryConditionOperatorType.IsNull);
                sqlBuilder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "[CityName]", QueryConditionOperatorType.IsNotNull);
                customCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = customCommand.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(AreaStatus)}                   
                });

                totalCount = int.Parse(customCommand.GetParameterValue("TotalCount").ToString());

                return dt;
            }
        }
    }
}