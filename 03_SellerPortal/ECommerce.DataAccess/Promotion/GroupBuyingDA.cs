using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using ECommerce.Entity.Promotion;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Common;
using ECommerce.Utility;
using System.Collections;


namespace ECommerce.DataAccess.Promotion
{
    public class GroupBuyingDA
    {
        /// <summary>
        /// 创建团购
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Create(GroupBuyingInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateProductGroupBuying");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@GroupBuyingTitle", entity.GroupBuyingTitle);
            command.SetParameterValue("@GroupBuyingDesc", entity.GroupBuyingDesc);
            command.SetParameterValue("@GroupBuyingRules", entity.GroupBuyingRules);
            command.SetParameterValue("@GroupBuyingDescLong", entity.GroupBuyingDescLong);
            command.SetParameterValue("@GroupBuyingPicUrl", entity.GroupBuyingPicUrl);
            command.SetParameterValue("@GroupBuyingMiddlePicUrl", entity.GroupBuyingMiddlePicUrl);
            command.SetParameterValue("@GroupBuyingSmallPicUrl", entity.GroupBuyingSmallPicUrl);
            command.SetParameterValue("@OriginalPrice", entity.CurrentPrice);
            command.SetParameterValue("@DealPrice", entity.GroupBuyPrice);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@IsByGroup", "N");
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@LimitOrderCount", entity.LimitOrderCount);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@InUser", entity.InUserName);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", entity.CompanyCode);
            command.SetParameterValue("@LanguageCode", entity.LanguageCode);
            command.SetParameterValue("@CurrencySysNo", 1);
            command.SetParameterValue("@VendorSysNo", entity.SellerSysNo);

            command.SetParameterValue("@Reasons", "");
            command.SetParameterValue("@SettlementStatus", "N");
            command.SetParameterValue("@GroupBuyingTypeSysNo", 0);
            command.SetParameterValue("@GroupBuyingAreaSysNo", 0);
            command.SetParameterValue("@CurrentSellCount", 0);
            command.SetParameterValue("@CouponValidDate", null);
            command.SetParameterValue("@LotteryRule", null);
            command.SetParameterValue("@GroupBuyingCategorySysNo", 0);
            command.SetParameterValue("@IsWithoutReservation", 0);
            command.SetParameterValue("@IsVouchers", 0);

            command.ExecuteNonQuery();

            return (int)command.GetParameterValue("@SysNo");
        }
        /// <summary>
        /// 创建团购阶梯价格
        /// </summary>
        /// <param name="ProductGroupBuyingSysNo"></param>
        /// <param name="SellCount"></param>
        /// <param name="GroupBuyingPrice"></param>
        /// <returns></returns>
        public int CreateProductGroupBuyingPrice(int ProductGroupBuyingSysNo, int? SellCount, decimal? GroupBuyingPrice, int? GroupBuyingPoint, decimal? costAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateProductGroupBuyingPrice");
            command.SetParameterValue("@ProductGroupBuyingSysNo", ProductGroupBuyingSysNo);
            command.SetParameterValue("@SellCount", SellCount.Value);
            command.SetParameterValue("@GroupBuyingPrice", GroupBuyingPrice);
            command.SetParameterValue("@GroupBuyingPoint", GroupBuyingPoint);
            command.SetParameterValue("@CostAmt", costAmt);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新团购
        /// </summary>
        /// <param name="entity"></param>
        public void Update(GroupBuyingInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductGroupBuying");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@GroupBuyingTitle", entity.GroupBuyingTitle);
            command.SetParameterValue("@GroupBuyingDesc", entity.GroupBuyingDesc);
            command.SetParameterValue("@GroupBuyingRules", entity.GroupBuyingRules);
            command.SetParameterValue("@GroupBuyingDescLong", entity.GroupBuyingDescLong);
            command.SetParameterValue("@OriginalPrice", entity.CurrentPrice);
            command.SetParameterValue("@DealPrice", entity.GroupBuyPrice);
            command.SetParameterValue("@GroupBuyingPicUrl", entity.GroupBuyingPicUrl);
            command.SetParameterValue("@GroupBuyingMiddlePicUrl", entity.GroupBuyingMiddlePicUrl);
            command.SetParameterValue("@GroupBuyingSmallPicUrl", entity.GroupBuyingSmallPicUrl);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@IsByGroup", "N");
            command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            command.SetParameterValue("@LimitOrderCount", entity.LimitOrderCount);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@EditUser", entity.EditUserName);
            command.SetParameterValue("@Status", entity.Status);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载团购信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public GroupBuyingQueryResult Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingEntity");
            cmd.SetParameterValue("@SysNo", sysNo);

            var ds = cmd.ExecuteDataSet();
            
            GroupBuyingQueryResult result = null;
            if (ds.Tables[0].Rows.Count > 0)
            {
                result = DataMapper.GetEntity<GroupBuyingQueryResult>(ds.Tables[0].Rows[0]);
            }
            //获取团购价格
            GroupBuyingPriceInfo priceInfo = null;
            if (ds.Tables[1].Rows.Count > 0)
            {
                priceInfo = DataMapper.GetEntity<GroupBuyingPriceInfo>(ds.Tables[1].Rows[0]);
            }
            if (priceInfo != null)
            {
                result.GroupBuyPrice = priceInfo.GroupBuyingPrice;
            }
            return result;
        }

        

        /// <summary>
        /// 更新团购状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="status"></param>
        /// <param name="userName"></param>
        public void UpdataSatus(int sysNo, string status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateGroupBuyStatus");
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);

            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新团购结束时间为当前时间，用于终止团购
        /// </summary>
        /// <param name="sysNo"></param>
        public void UpdateGroupBuyingEndDate(int sysNo, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateGroupBuyingEndDate");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查团购冲突
        /// </summary>
        /// <param name="excludeSysNo"></param>
        /// <param name="productSysNos"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool CheckConflict(int excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            if (productSysNos == null || productSysNos.Count == 0)
                throw new ArgumentException("productSysNos");
            string inProductSysNos = productSysNos[0].ToString();
            for (int i = 1; i < productSysNos.Count; i++)
            {
                inProductSysNos += "," + productSysNos[i].ToString();
            }
            DataCommand command = DataCommandManager.GetDataCommand("CheckProductInGBByDateTime");
            command.ReplaceParameterValue("#ProductSysNos#", inProductSysNos);
            command.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);

            return command.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 删除团购阶梯价格
        /// </summary>
        /// <param name="ProductGroupBuyingSysNo"></param>
        public void DeleteProductGroupBuyingPrice(int ProductGroupBuyingSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteProductGroupBuyingPrice");
            command.SetParameterValue("@ProductGroupBuyingSysNo", ProductGroupBuyingSysNo);
            command.ExecuteNonQuery();
        }

        public QueryResult<GroupBuyingQueryResult> Query(GroupBuyingQueryFilter filter)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductGroupBuyingList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "M.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.VendorSysNo", DbType.Int32, "@VendorSysNo",
                    QueryConditionOperatorType.Equal, filter.SellerSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.ProductID", DbType.String, "@ProductID",
                    QueryConditionOperatorType.Equal, filter.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.SysNo", DbType.Int32, "@ProductSysNo",
                    QueryConditionOperatorType.Equal, filter.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.BeginDate", DbType.DateTime, "@BeginDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.BeginDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.BeginDate", DbType.DateTime, "@BeginDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.BeginDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.EndDate", DbType.DateTime, "@EndDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.EndDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.EndDate", DbType.DateTime, "@EndDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.EndDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "M.Status",
                   DbType.AnsiStringFixedLength,
                   "@Status",
                   QueryConditionOperatorType.Equal,
                 filter.Status);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.[GroupBuyingTitle]", DbType.String, "@GroupBuyingTitle",
                    QueryConditionOperatorType.Like, filter.GroupBuyingTitle);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                var dataList = DataMapper.GetEntityList<GroupBuyingQueryResult, List<GroupBuyingQueryResult>>(ds.Tables[0].Rows, true, true);
                var priceInfoList = DataMapper.GetEntityList<GroupBuyingPriceInfo, List<GroupBuyingPriceInfo>>(ds.Tables[1].Rows, true, true);
                foreach (var data in dataList)
                {
                    var found = priceInfoList.FirstOrDefault(item => item.ProductGroupBuyingSysNo == data.SysNo);
                    if (found != null)
                    {
                        data.GroupBuyPrice = found.GroupBuyingPrice;
                    }
                }

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                QueryResult<GroupBuyingQueryResult> result = new QueryResult<GroupBuyingQueryResult>();
                result.ResultList = dataList;
                result.PageInfo = new PageInfo()
                {
                    TotalCount = totalCount,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize
                };
                return result;
            }
        }

    }
}
