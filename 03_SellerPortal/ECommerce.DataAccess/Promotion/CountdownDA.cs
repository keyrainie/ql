using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Promotion;
using ECommerce.Entity;
using ECommerce.Entity.Common;

namespace ECommerce.DataAccess.Promotion
{
    public class CountdownDA
    {
        public CountdownQueryResult Load(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadCountdownInfo");
            command.SetParameterValue("@SysNo", sysNo);
            var result = command.ExecuteEntity<CountdownQueryResult>();
            return result;
        }

        public void CreateCountdown(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateCountdown");


            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice );
            command.SetParameterValue("@CountDownCashRebate", 0);
            command.SetParameterValue("@CountDownPoint", 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty );
            command.SetParameterValue("@SnapShotCurrentPrice",  0);
            command.SetParameterValue("@SnapShotCashRebate", 0);
            command.SetParameterValue("@SnapShotPoint",0);
            command.SetParameterValue("@AffectedVirtualQty", 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", null);
            command.SetParameterValue("@Reasons", null);
            command.SetParameterValue("@IsCountDownAreaShow", 0);
            command.SetParameterValue("@PromotionType", string.Empty);
            command.SetParameterValue("@IsLimitedQty", 1);
            command.SetParameterValue("@IsReservedQty", 0);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsPromotionSchedule", false);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", 0);
            command.SetParameterValue("@MaxPerOrder", 1);
            command.SetParameterValue("@IsHomePageShow", 0);

            command.SetParameterValue("@IsC1Show", "N");
            command.SetParameterValue("@IsC2Show", "N");
            command.SetParameterValue("@IsTodaySpecials", "N");
            command.SetParameterValue("@Is24hNotice", "N");
            command.SetParameterValue("@IsShowPriceInNotice", "N");
            command.SetParameterValue("@IsEndIfNoQty", entity.IsEndIfNoQty);
            command.SetParameterValue("@IsGroupOn", "N");

            command.SetParameterValue("@AreaShowPriority", null);
            command.SetParameterValue("@HomePagePriority", null);
            command.SetParameterValue("@CreateUserSysNo",entity.InUserSysNo);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@VendorSysNo", entity.SellerSysNo);
            command.SetParameterValue("@CreateUserName", entity.InUserName);
            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
        }

        public void UpdateCountdown(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdown");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice );
            command.SetParameterValue("@CountDownCashRebate", 0);
            command.SetParameterValue("@CountDownPoint", 0);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty );
            command.SetParameterValue("@SnapShotCurrentPrice", 0);
            command.SetParameterValue("@SnapShotCashRebate", 0);
            command.SetParameterValue("@SnapShotPoint", 0);
            command.SetParameterValue("@AffectedVirtualQty", 0);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", null);
            command.SetParameterValue("@Reasons", null);
            command.SetParameterValue("@IsCountDownAreaShow", 0);
            command.SetParameterValue("@PromotionType", string.Empty);
            command.SetParameterValue("@IsLimitedQty", 1);
            command.SetParameterValue("@IsReservedQty", 0);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsPromotionSchedule", false);
            command.SetParameterValue("@PromotionTitle", entity.PromotionTitle);
            command.SetParameterValue("@BaseLine", 0);
            command.SetParameterValue("@MaxPerOrder", 1);
            command.SetParameterValue("@IsHomePageShow", 0);

            command.SetParameterValue("@IsC1Show", "N");
            command.SetParameterValue("@IsC2Show", "N");
            command.SetParameterValue("@IsTodaySpecials", "N");
            command.SetParameterValue("@Is24hNotice", "N");
            command.SetParameterValue("@IsShowPriceInNotice", "N");
            command.SetParameterValue("@IsEndIfNoQty", entity.IsEndIfNoQty);
            command.SetParameterValue("@IsGroupOn", "N");

            command.SetParameterValue("@AreaShowPriority", null);
            command.SetParameterValue("@HomePagePriority", null);
            command.SetParameterValue("@EditUser",entity.EditUserName);
            command.ExecuteNonQuery();
        }

        public void MaintainCountdownStatus(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdownStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@EditUser",entity.EditUserName);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 排重
        /// </summary>
        public bool CheckConflict(int excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            if (productSysNos == null || productSysNos.Count == 0)
                throw new ArgumentException("productSysNos");
            string inProductSysNos = productSysNos[0].ToString();
            for (int i = 1; i < productSysNos.Count; i++)
            {
                inProductSysNos += "," + productSysNos[i].ToString();
            }
            DataCommand command = DataCommandManager.GetDataCommand("CheckProductInSCByDateTime");
            command.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            command.ReplaceParameterValue("#ProductSysNos#", inProductSysNos);
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);

            return command.ExecuteScalar<int>() > 0;
        }


        public void MaintainCountdownEndTime(CountdownInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainCountdownEndTime");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@EditUser",entity.EditUserName);
            command.ExecuteNonQuery();
        }

        public QueryResult<CountdownQueryResult> Query(CountdownQueryFilter filter)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CountdownQueryList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "sc.[CreateTime] DESC"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.VendorSysNo", DbType.Int32, "@MerchantSysNo",
                    QueryConditionOperatorType.Equal, filter.SellerSysNo);

                

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[StartTime]", DbType.DateTime, "@CountdownFromStartTime",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CountdownFromStartTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[StartTime]", DbType.DateTime, "@CountdownToStartTime",
                     QueryConditionOperatorType.LessThan,
                     filter.CountdownToStartTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[EndTime]", DbType.DateTime, "@CountdownFromEndTime",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CountdownFromEndTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[EndTime]", DbType.DateTime, "@CountdownToEndTime",
                     QueryConditionOperatorType.LessThan,
                     filter.CountdownToEndTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "p.[SysNo]", DbType.Int32, "@ProductSysNo",
                    QueryConditionOperatorType.Equal, filter.ProductSysNo);

              

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[Status]", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "p.[ProductID]", DbType.AnsiString, "@ProductID",
                    QueryConditionOperatorType.Equal, filter.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[PromotionTitle]", DbType.String, "@PromotionTitle",
                    QueryConditionOperatorType.Like, filter.PromotionTitle);

                
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dataList= cmd.ExecuteEntityList<CountdownQueryResult>();
                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                var result = new QueryResult<CountdownQueryResult>();
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
