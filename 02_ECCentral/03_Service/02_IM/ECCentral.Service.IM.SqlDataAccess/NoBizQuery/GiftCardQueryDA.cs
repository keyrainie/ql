using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IGiftCardQueryDA))]
    public class GiftCardQueryDA : IGiftCardQueryDA
    {
        /// <summary>
        /// 查询礼品卡
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryGiftCardInfo(GiftCardFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_QueryGiftCardInfo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "B.TransactionNumber DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ReferenceSOSysNo", DbType.Int32, "@ReferenceSOSysNo", QueryConditionOperatorType.Equal, filter.ReferenceSOSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.CustomerID", DbType.String, "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Code", DbType.String, "@Code", QueryConditionOperatorType.Equal, filter.CardCode);


                //if (filter.Status.HasValue && filter.Status == ECCentral.BizEntity.IM.GiftCardStatus.Hold)
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.IsHold", DbType.String, "@IsHold", QueryConditionOperatorType.Equal, ECCentral.BizEntity.MKT.YNStatus.Yes);
                //else
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                if (filter.ActionSysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "EXISTS(SELECT TOP 1 1 FROM [OverseaECommerceManagement].[dbo].[GiftCardRedeemLog] WITH (NOLOCK) WHERE Code=B.Code AND ActionSysNo = " + filter.ActionSysNo + " AND ActionType='SO')");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ReferenceSOSysNo", DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Status", DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.InternalType", DbType.String, "@CardType", QueryConditionOperatorType.Like, filter.CardType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                
                if(filter.EndDateFrom==null&&filter.EndDateTo!=null)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.EndDate", DbType.DateTime, "@EndDate", QueryConditionOperatorType.LessThan, filter.EndDateTo);
                else if (filter.EndDateTo==null&&filter.EndDateFrom!=null)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.EndDate", DbType.DateTime, "@EndDate", QueryConditionOperatorType.MoreThanOrEqual, filter.EndDateFrom);
                else
                    sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "B.EndDate", DbType.DateTime, "@EndDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.EndDateFrom, filter.EndDateTo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.IM.GiftCardStatus));
                enumList.Add("IsHold", typeof(ECCentral.BizEntity.MKT.YNStatus));//前台展示状态
                enumList.Add("CardType", typeof(ECCentral.BizEntity.IM.GiftCardType));

                //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                //pairList.Add("InternalType", "MKT", "CommentsCategory"); //
                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
       
        /// <summary>
        /// 查询礼品卡制作
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryGiftCardFabricationMaster(GiftCardFabricationFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_QueryGiftCardFabricationMaster");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "GG.GiftCardFabricationSysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ReferenceSOSysNo", DbType.Int32, "@ReferenceSOSysNo", QueryConditionOperatorType.Equal, filter.ReferenceSOSysNo);
sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GM.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GM.Title", DbType.String, "@Title", QueryConditionOperatorType.Like, filter.Title);

                if (filter.Status.HasValue)
                {
                    if(filter.Status.Value == -2)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "PV.Status IN (4,6,7,8)");
                    else
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GM.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GM.POSysNo", DbType.Int32, "@POSysNo", QueryConditionOperatorType.Like, filter.POSysNo);

                if (filter.InDateTo != null) { filter.InDateTo = (filter.InDateTo == filter.InDateFrom ? filter.InDateTo.Value.AddDays(1) : filter.InDateTo); }
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "GM.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.LessThanOrEqual, QueryConditionOperatorType.MoreThanOrEqual, filter.InDateTo, filter.InDateFrom);

                if (filter.Status.HasValue)
                    cmd.CommandText = sqlBuilder.BuildQuerySql().Replace("#StrWhere2#", " where GG.Status=" + filter.Status.Value).Replace("#StrWhere3#", " where GG.Status=" + filter.Status.Value + " ");
                else
                    cmd.CommandText = sqlBuilder.BuildQuerySql().Replace("#StrWhere2#", "").Replace("#StrWhere3#", "");
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("POStatus", typeof(ECCentral.BizEntity.PO.PurchaseOrderStatus));//前台展示状态
                //enumList.Add("CardType", typeof(ECCentral.BizEntity.IM.GiftCardType));

                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("Status", "IM", "GiftCardFabricationStatus"); //
                var dt = cmd.ExecuteDataTable(enumList, pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询礼品商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryGiftCardProductInfo(GiftCardProductFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_GetGiftVoucherProductInfo");

            using(var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText ,cmd,pagingEntity,"GP.SysNo DESC"))
            {
                if(filter.Price.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,"Price"
                        ,DbType.Decimal,"@Price",QueryConditionOperatorType.Equal,filter.Price);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.IM.GiftVoucherProductStatus));

                var dt = cmd.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return dt;
            }
            
        }

        /// <summary>
        /// 查询礼品券关联商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryGiftVoucherProductRelation(GiftCardProductFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_GetVoucherRelationByVoucherPaging");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                if (filter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GiftVoucherSysNo"
                        , DbType.Int32, "@GiftVoucherSysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.IM.GiftVoucherRelateProductStatus));
                enumList.Add("AuditStatus", typeof(ECCentral.BizEntity.IM.GVRReqAuditStatus));

                var dt = cmd.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return dt;
            }
        }

        /// <summary>
        /// 查询礼品券关联请求
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryGiftVoucherProductRelationReq(GiftCardProductFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_GetVoucherRelationRequestByRelationPaging");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "GP.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GP.AuditStatus", DbType.AnsiStringFixedLength,
                    "@AuditStatus", QueryConditionOperatorType.Equal, "W");

                if (filter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GV.GiftVoucherSysNo"
                        , DbType.Int32, "@GiftVoucherSysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("AuditStatus", typeof(ECCentral.BizEntity.IM.GVRReqAuditStatus));

                var dt = cmd.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return dt;
            }
        }
    }
}
