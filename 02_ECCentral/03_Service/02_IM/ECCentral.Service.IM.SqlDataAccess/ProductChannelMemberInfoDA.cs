//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理
// 作成者				John
// 改版日				2012.11.7
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductChannelMemberInfoDA))]
    internal class ProductChannelMemberInfoDA : IProductChannelMemberInfoDA
    {
        #region ProductChannelMemberInfo
        // 获取渠道列表
        public List<ProductChannelMemberInfo> GetProductChannelMemberInfoList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductChannelMemberInfo");
            command.SetParameterValue("@Status", "A");
            return command.ExecuteEntityList<ProductChannelMemberInfo>();
        }
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询全部外部渠道会员价格
        public List<ProductChannelMemberPriceInfo> GetProductChannelMemberPriceByAll()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductChannelMemberPriceByAll");
            return cmd.ExecuteEntityList<ProductChannelMemberPriceInfo>();
        }
        // 插入会员渠道信息
        public Int32 InsertProductChannelMemberPrices(ProductChannelMemberPriceInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductChannelMemberPrice");
            cmd.SetParameterValue("@ChannelSysNo", entity.ChannelSysNO);
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@MemberPrice", entity.MemberPrice);
            cmd.SetParameterValue("@MemberPricePercent", entity.MemberPricePercent);
            cmd.SetParameterValue("@InUser", entity.InUser);
            cmd.SetParameterValue("@EditUser", entity.EditUser);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", entity.StoreCompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            return cmd.ExecuteNonQuery();
        }
        //查询指定外部渠道会员价格
        public IList<ProductChannelMemberPriceInfo> GetProductChannelMemberPriceByChannelSysNo(int ChannelSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductChannelMemberPriceByChannelSysNo");
            cmd.SetParameterValue("@ChannelSysNo", ChannelSysNo);
            return cmd.ExecuteEntityList<ProductChannelMemberPriceInfo>();
        }
        // 查询会员渠道信息
        public DataTable GetProductChannelMemberPriceInfoUrl(ProductChannelInfoMemberQueryFilter queryCriteria
            , out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductChannelMemberPriceResult");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "B.SysNo DESC"))
            {
                if (queryCriteria.ChannelSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "M.SysNo",
                        DbType.Int32, "@ChannelSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ChannelSysNo);
                }
                if (queryCriteria.C1SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C1.Sysno",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo.Value);
                }

                if (queryCriteria.C2SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.Sysno",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo.Value);
                }
                if (queryCriteria.C3SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C3.Sysno",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo.Value);
                }
                if (!string.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "p.ProductID",
                        DbType.String, "@ProductID",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductID);
                }
                //动态生成SQL语句
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        //更新优惠价和优惠比例
        public Int32 UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand(
                "UpdateProductChannelMemberPriceOrMemberPricePercentBySysNo");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@MemberPrice", entity.MemberPrice);
            cmd.SetParameterValue("@MemberPricePercent", entity.MemberPricePercent);
            cmd.SetParameterValue("@EditDate", DateTime.Now);
            cmd.SetParameterValue("@EditUser", entity.EditUser);
            return cmd.ExecuteNonQuery();
        }

        //删除外部渠道会员价格
        public Int32 DeleteProductChannelMemberPrice(Int32 sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductChannelMemberPrices");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteNonQuery();
        }
        #endregion

        #region ProductChannelMemberPriceLogInfo
        //添加日志记录
        public Int32 InsertProductChannelMemberPriceLog(ProductChannelMemberPriceLogInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductChannelMemberPriceLog");
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@ChannelName", entity.ChannelName);
            cmd.SetParameterValue("@MemberPrice", entity.MemberPrice);
            cmd.SetParameterValue("@MemberPricePercent", entity.MemberPricePercent);
            cmd.SetParameterValue("@OperationType", entity.OperationType);
            cmd.SetParameterValue("@InUser", entity.InUser);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", entity.StoreCompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            return cmd.ExecuteNonQuery();
        }
        //查询日志信息
        public DataTable GetProductChannelMemberPriceLogs(
            ProductChannelInfoMemberQueryFilter queryCriteria, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductChannelMemberLogResult");
            cmd.SetParameterValue("@ProductID", queryCriteria.ProductID);
            cmd.SetParameterValue("@ChannelName", queryCriteria.ChannelName);
            cmd.SetParameterValue("@StartTime", queryCriteria.StartDay);
            cmd.SetParameterValue("@EndTime", queryCriteria.EndDay);
            cmd.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            cmd.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);
            DataTable dt = cmd.ExecuteDataTable();
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }
        #endregion
    }
}