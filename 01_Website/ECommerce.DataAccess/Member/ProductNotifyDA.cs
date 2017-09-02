using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Member;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Member
{
    /// <summary>
    /// 商品到货通知
    /// </summary>
    public class ProductNotifyDA
    {
        public static QueryResult<ProductNotifyInfo> QueryProductNotify(ProduceNotifiyQueryFilter filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_QueryProductNotify");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, filter.ConvertToPaging(), "pnr.[SysNo] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pnr.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pnr.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                command.CommandText = sqlBuilder.BuildQuerySql();
                var newsList = command.ExecuteEntityList<ProductNotifyInfo>();
                var totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                QueryResult<ProductNotifyInfo> result = new QueryResult<ProductNotifyInfo>();
                result.ResultList = newsList;
                result.PageInfo = filter.ConvertToPageInfo(totalCount);
                return result;
            }
        }

        public static ProductNotifyInfo GetProductNotify(string email, int productSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetProductNotifyForEmailAndProductSysNo");
            dataCommand.SetParameterValue("@Email", email);
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteEntity<ProductNotifyInfo>();
        }

        public static int CreateProductNotify(ProductNotifyInfo info)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateProductNotify");
            dataCommand.SetParameterValue("@CustomerSysNo", info.CustomerSysNo);
            dataCommand.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            dataCommand.SetParameterValue("@Email", info.Email);
            dataCommand.SetParameterValue("@Status", info.Status);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteScalar<int>();
        }

        public static void UpdateProductNotify(int sysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateProductNotify");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@Status", NotifyStatus.Unnotify);
            dataCommand.ExecuteNonQuery();
        }

        public static void DeleteProductNotify(int sysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_DeleteProductNotify");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public static void ClearProductNotify(int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_ClearProductNotify");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

    }
}
