using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Product
{
    public class ProductMatchedTradingDA
    {
        /// <summary>
        /// 产品咨询查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<ProductMatchedTradingQueryBasicInfo> QueryProductMatchedTradingBasicInfoList(ProductMatchedTradingQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("ProductMatchedTrading_GetProductMatchedTradingDetailList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, string.IsNullOrEmpty(pagingEntity.SortField) ? pagingEntity.SortField : "A.SysNo DESC"))
            {
                if (filter.SellerSysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@SellerSysNo", DbType.String, filter.SellerSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.MerchantSysNo",
                        DbType.String, "@SellerSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.SellerSysNo);
                }

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                //中蛋故有
                if (filter.VendorType == 1)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);

                if (filter.ProductGroupNo != 0)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE ProductID = '" + filter.ProductID + "')");
                }

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);
                //商品类别
                if (filter.Category1SysNo != null && filter.Category2SysNo != null && filter.Category3SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE C3SysNo = " + filter.Category3SysNo + " AND CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null && filter.Category2SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT p.SysNo FROM ipp3.dbo.Product p WITH (NOLOCK),ipp3.dbo.Category3 c WITH (NOLOCK) WHERE p.C3SysNo = c.SysNo AND c.C2SysNo =" + filter.Category2SysNo + " AND P.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT p.SysNo FROM ipp3.dbo.Product p WITH (NOLOCK),ipp3.dbo.Category3 c3 WITH (NOLOCK),ipp3.dbo.Category2 c2 WITH (NOLOCK) WHERE p.C3SysNo = c3.SysNo  AND c3.C2SysNo = c2.SysNo AND c2.C1SysNo = " + filter.Category1SysNo + " AND P.companycode=" + filter.CompanyCode + " AND c2.companycode=" + filter.CompanyCode + " AND c3.companycode=" + filter.CompanyCode + ")");

                if (filter.PMUserSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE PMUserSysNo = " + filter.PMUserSysNo + ")");
                if (filter.ProductStatus != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE Status = " + filter.ProductStatus + ")");
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Content", DbType.String, "@Content", QueryConditionOperatorType.Like, filter.Content);
                if (filter.CustomerSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.CustomerSysNo=" + filter.CustomerSysNo + "");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);

                if (filter.CustomerCategory != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.CustomerSysNo IN ( SELECT SysNo FROM ipp3.dbo.Customer WITH (NOLOCK)  WHERE CompanyCustomer = " + filter.CustomerCategory + ")");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();


                //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                //pairList.Add("Status", "MKT", "ReplyStatus");//咨询状态
                ////pairList.Add("ReferenceType", "MKT", "CommentsCategory"); 
                //var dt = cmd.ExecuteDataTable(pairList);
                //totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));


                List<ProductMatchedTradingQueryBasicInfo> list = dataCommand.ExecuteEntityList<ProductMatchedTradingQueryBasicInfo>();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return list;
            }
        }

        /// <summary>
        /// 批量更改状态
        /// </summary>
        /// <param name="items">The items.</param> 
        /// <param name="status">The status.</param>
        public static void BatchSetProductMatchedTradingStatus(List<int> items, string status, string currentUser)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }

            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductMatchedTrading_BatchUpdateProductMatchedTradingStatus");

            dataCommand.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dataCommand.SetParameterValue("@Status", status);
            dataCommand.SetParameterValue("@EditUser", currentUser);
            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据咨询编号，加载相应的咨询
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static ProductMatchedTradingInfo LoadProductMatchedTrading(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductMatchedTrading_GetProductMatchedTradingInfo");

            dataCommand.SetParameterValue("@SysNo", sysNo);

            return dataCommand.ExecuteEntity<ProductMatchedTradingInfo>();
        }

        /// <summary>
        /// 添加产品咨询回复:添加产品咨询回复有3种方式：
        /// 1.	网友回复，需通过审核才展示。
        /// 2.	厂商回复（通过Seller Portal），需通过审核才展示。
        /// 3.	IPP系统中回复，默认直接展示。
        /// </summary>
        /// <param name="item">The item.</param>
        public static void AddProductMatchedTradingReply(ProductMatchedTradingReplyInfo item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductMatchedTrading_InsertProductMatchedTradingReply");

            dataCommand.SetParameterValue<ProductMatchedTradingReplyInfo>(item);

            dataCommand.ExecuteNonQuery();
        }
        /// <summary>
        /// 根据咨询编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static List<ProductMatchedTradingReplyInfo> GetProductMatchedTradingFactoryReply(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductMatchedTrading_GetProductMatchedTradingFactoryReply");

            dataCommand.SetParameterValue("@MatchedTradingSysNo", sysNo);

            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            pairList.Add("Type", "MKT", "MatchedTradingCategory");//回复类型
            pairList.Add("Status", "MKT", "FactoryReplyStatus");
            DataTable dataTable = dataCommand.ExecuteDataTable(pairList);

            List<ProductMatchedTradingReplyInfo> list = new List<ProductMatchedTradingReplyInfo>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductMatchedTradingReplyInfo>(row));
            }

            return list;
        }
    }
}
