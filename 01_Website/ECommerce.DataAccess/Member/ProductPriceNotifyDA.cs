using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Member;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Member
{
    /// <summary>
    /// 商品降价通知
    /// </summary>
    public class ProductPriceNotifyDA
    {
        public static QueryResult<ProductPriceNotifyInfo> QueryProductPriceNotify(ProducePriceNotifiyQueryFilter filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_QueryProductPriceNotify");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, filter.ConvertToPaging(), "w.[SysNo] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "w.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "w.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "w.LanguageCode", DbType.String, "@LanguageCode", QueryConditionOperatorType.Equal, ConstValue.LanguageCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "w.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, ConstValue.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "w.StoreCompanyCode", DbType.String, "@StoreCompanyCode", QueryConditionOperatorType.Equal, ConstValue.StoreCompanyCode);
                command.CommandText = sqlBuilder.BuildQuerySql();
                var newsList = command.ExecuteEntityList<ProductPriceNotifyInfo>();
                var totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                QueryResult<ProductPriceNotifyInfo> result = new QueryResult<ProductPriceNotifyInfo>();
                result.ResultList = newsList;
                result.PageInfo = filter.ConvertToPageInfo(totalCount);
                return result;
            }
        }

        public static ProductPriceNotifyInfo GetProductPriceNotify(int customerSysno, int productSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetProductPriceNotify");
            dataCommand.SetParameterValue("@CustomerSysno", customerSysno);
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteEntity<ProductPriceNotifyInfo>();
        }

        public static int CreateProductPriceNotify(ProductPriceNotifyInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateProductPriceNotify");
            dataCommand.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            dataCommand.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            dataCommand.SetParameterValue("@ExpectedPrice", entity.ExpectedPrice);
            dataCommand.SetParameterValue("@InstantPrice", entity.InstantPrice);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteScalar<int>();
        }

        public static void UpdateProductPriceNotify(ProductPriceNotifyInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateProductPriceNotify");
            dataCommand.SetParameterValue("@SysNo", entity.SysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            dataCommand.SetParameterValue("@ExpectedPrice", entity.ExpectedPrice);
            dataCommand.SetParameterValue("@InstantPrice", entity.InstantPrice);
            dataCommand.ExecuteNonQuery();
        }

        public static void CancelProductPriceNotify(int sysNo, int customerSysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CancelProductPriceNotify");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            dataCommand.ExecuteNonQuery();
        }

        public static void DeleteProductPriceNotify(int sysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_DeleteProductPriceNotify");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public static void ClearProductPriceNotify(int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_ClearProductPriceNotify");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }
    }
}
