using ECommerce.Entity;
using ECommerce.Entity.GiftCard;
using ECommerce.Entity.Product;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace ECommerce.DataAccess.GiftCard
{
    public class GiftCardDA
    {
        /// <summary>
        /// 获取礼品卡商品基本信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<GiftCardProductInfo> QueryGiftCardProductInfo()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryGiftCardProductInfo");
            cmd.SetParameterValue("@C3SysNo", ConstValue.GiftCardCategory3);
            return cmd.ExecuteEntityList<GiftCardProductInfo>();
        }

        public static string LookPassword(string Code, int CurrentCustomerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LookGiftCardPassword");
            cmd.SetParameterValue("@CustomerSysNo", CurrentCustomerSysNo);
            cmd.SetParameterValue("@Code", Code);
            GiftCardInfo tmp = cmd.ExecuteEntity<GiftCardInfo>();
            if (tmp != null)
                return CryptoManager.GetCrypto(CryptoAlgorithm.DES).Decrypt(tmp.Password);
            else
                return null;

        }

        public static QueryResult<GiftCardInfo> QueryMyGiftCardInfo(MyGiftCardQueryInfoFilter filter)
        {
            QueryResult<GiftCardInfo> list = new QueryResult<GiftCardInfo>();
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryMyGiftCardInfo");

            PagingInfoEntity page = new PagingInfoEntity();
            page.MaximumRows = filter.PagingInfo.PageSize;
            page.StartRowIndex = (filter.PagingInfo.PageIndex - 1) * filter.PagingInfo.PageSize;

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, page, "TransactionNumber DESC"))
            {
                if (!String.IsNullOrWhiteSpace(filter.Code))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Code", DbType.String, "@Code", QueryConditionOperatorType.Equal, filter.Code);
                }

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, String.Format("( CustomerSysNo = {0} OR BindingCustomerSysNo = {1} )", filter.CustomerID, filter.CustomerID));

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                list.ResultList = cmd.ExecuteEntityList<GiftCardInfo>();
                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                int pageIndex = filter.PagingInfo.PageIndex;
                if ((pageIndex * filter.PagingInfo.PageSize) > totalCount)
                {
                    if (totalCount != 0 && (totalCount % filter.PagingInfo.PageSize) == 0)
                    {
                        pageIndex = (int)(totalCount / filter.PagingInfo.PageSize);
                    }
                    else
                    {
                        pageIndex = (int)(totalCount / filter.PagingInfo.PageSize) + 1;
                    }
                }

                list.PageInfo = new PageInfo();
                list.PageInfo.TotalCount = totalCount;
                list.PageInfo.PageIndex = pageIndex;
                list.PageInfo.PageSize = filter.PagingInfo.PageSize;
                list.PageInfo.SortBy = filter.PagingInfo.SortBy;
                return list;
            }
        }

        /// <summary>
        /// 礼品卡消费记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryResult<GiftCardUseInfo> QueryUsedRecord(UsedRecordQuery filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GiftCard_QueryUsedRecord");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, filter.ConvertToPaging(), "SO.[SysNo] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GC.Code", DbType.String, "@Code", QueryConditionOperatorType.Equal, filter.Code);
                command.CommandText = sqlBuilder.BuildQuerySql();

                command.SetParameterValue("@CustomerSysNo", filter.CustomerSysNo);
                command.SetParameterValue("@ActionType", "SO");
                
                var newsList = command.ExecuteEntityList<GiftCardUseInfo>();
                var totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                QueryResult<GiftCardUseInfo> result = new QueryResult<GiftCardUseInfo>();
                result.ResultList = newsList;
                result.PageInfo = filter.ConvertToPageInfo(totalCount);
                return result;
            }
        }

        /// <summary>
        /// 通过卡号和密码加载礼品卡
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="password">密码</param>
        /// <returns>礼品卡信息</returns>
        public static GiftCardInfo LoadGiftCard(string code, string password)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GiftCard_LoadGiftCard");
            dataCommand.SetParameterValue("@Code", code);
            dataCommand.SetParameterValue("@Password", password);
            return dataCommand.ExecuteEntity<GiftCardInfo>();
        }

        /// <summary>
        /// 礼品卡绑定帐号
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="customerSysNo">帐户</param>
        /// <returns></returns>
        public static bool BindGiftCard(string code, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GiftCard_BindGiftCard");
            dataCommand.SetParameterValue("@Code", code);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            return dataCommand.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改礼品卡密码
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="pwd">新密码</param>
        public static void ModifyGiftCardPwd(string code, string pwd)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GiftCard_ModifyPwd");
            dataCommand.SetParameterValue("@Code", code);
            dataCommand.SetParameterValue("@Password", pwd);
            dataCommand.ExecuteNonQuery();
        }
    }
}
