using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using ECCentral.BizEntity.PO.Commission;
namespace IPP.Oversea.CN.POASNMgmt.DataAccess
{
    public class QueryBaseCondition
    {
        private string m_companyCode;

        public string CompanyCode
        {
            get
            {
                if (string.IsNullOrEmpty(m_companyCode))
                {
                    //m_companyCode = CPContext.Current.CompanyCode;
                    m_companyCode = "8601";
                }
                return m_companyCode;
            }

            set
            {
                m_companyCode = value;
            }
        }
    }
    public class CommonHelper
    {
        public static void SetCommonParams(DataCommand command)
        {
            SetCommonParams(command, new EntityBase());
        }

        public static void SetCommonParams(DataCommand command, EntityBase entity)
        {
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
        }

    }
    public class ProviderHelper
    {
        public static string DefaultDateFormatString
        {
            get
            {
                return "yyyy-MM-dd HH:mm:ss";
            }
        }

        public static string CompanyCode
        {
            get
            {
                //return CPContext.Current.CompanyCode;
                return "8601";
            }
        }

        public static string DefaultDateShortFormatString
        {
            get
            {
                return "yyyy-MM-dd HH:mm:ss";
            }
        }

        public static string FormatDateString(DateTime dateTime)
        {
            return dateTime.ToString(DefaultDateFormatString);
        }

        public static string FormatDateString(DateTime? dateTime)
        {
            return dateTime.HasValue ? FormatDateString(dateTime.Value) : String.Empty;
        }

        public static DateTime? ParseDate(string dateTimeString)
        {
            DateTime dateTime;

            if (DateTime.TryParse(dateTimeString, out dateTime))
            {
                return dateTime;
            }

            return null;
        }

        public static DateTime ParseDateTime(string dateTimeString)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeString, out dateTime))
            {
                return dateTime;
            }
            else
            {
                return DateTime.MinValue;
            }
        }


        public static void SetCommonParams(DynamicQuerySqlBuilder builder)
        {
            SetCommonParams(builder, new QueryBaseCondition());
        }

        public static void SetCommonIntParams(DynamicQuerySqlBuilder builder)
        {
            SetCommonIntParams(builder, new QueryBaseCondition());
        }

        public static void SetCommonParams(DynamicQuerySqlBuilder builder, string tableName)
        {
            SetCommonParams(builder, new QueryBaseCondition(), tableName);
        }

        public static void SetCommonIntParams(DynamicQuerySqlBuilder builder, string tableName)
        {
            SetCommonIntParams(builder, new QueryBaseCondition(), tableName);
        }

        public static void SetCommonParams(DynamicQuerySqlBuilder builder, QueryBaseCondition condition)
        {
            SetCommonParams(builder, condition, string.Empty);
        }

        public static void SetCommonIntParams(DynamicQuerySqlBuilder builder, QueryBaseCondition condition)
        {
            SetCommonIntParams(builder, condition, string.Empty);
        }

        public static void SetCommonParams(DynamicQuerySqlBuilder builder, QueryBaseCondition condition, string tableName)
        {
            string fieldName = string.Format("{0}CompanyCode", (string.IsNullOrEmpty(tableName) ? "" : tableName + "."));

            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, fieldName, System.Data.DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, "8601");
        }

        public static void SetCommonIntParams(DynamicQuerySqlBuilder builder, QueryBaseCondition condition, string tableName)
        {
            string fieldName = string.Format("{0}CompanyCode", (string.IsNullOrEmpty(tableName) ? "" : tableName + "."));

            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, fieldName, System.Data.DbType.Int32,
                    "@CompanyCode", QueryConditionOperatorType.Equal, "8601");
        }

        public static void SetCommonParams(DataCommand command)
        {
            SetCommonParams(command, new QueryBaseCondition() { CompanyCode = "8601" });
        }

        public static void SetCommonParams(DataCommand command, QueryBaseCondition condition)
        {
            command.SetParameterValue("@CompanyCode", "8601");
        }
    }


    [VersionExport(typeof(ICollectionPaymentDA))]
    public class CollectionPaymentDA : ICollectionPaymentDA
    {
        public CollectionPaymentInfo Create(CollectionPaymentInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateCollVendorSettle");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@SettleID", entity.SettleID);
            command.SetParameterValue("@VendorSysNo", entity.VendorInfo.SysNo);
            command.SetParameterValue("@StockSysNo", entity.SourceStockInfo.SysNo);
            command.SetParameterValue("@TotalAmt", entity.TotalAmt);
            command.SetParameterValue("@CurrencySysNo", entity.CurrencyCode);
            command.SetParameterValue("@CreateTime", entity.CreateTime);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUserSysNo);
            command.SetParameterValue("@Memo", entity.Memo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Status", entity.Status);
            //command.SetParameterValue("@SettleBalanceSysNo", entity.SettleBalanceSysNo);

            decimal taxRate = (decimal)((int)entity.TaxRateData.Value / 100.0);
            command.SetParameterValue("@TaxRate", taxRate);
            //////////////////////////////CLR16063//////////////////////////////
            //command.SetParameterValue("@UsingReturnPoint", entity.UsingReturnPoint);
            //command.SetParameterValue("@PM_ReturnPointSysNo", entity.PM_ReturnPointSysNo);
            command.SetParameterValue("@PMSysno", entity.PMInfo.SysNo);
            //command.SetParameterValue("@ReturnPointC3SysNo", entity.ReturnPointC3SysNo);
            ////////////////////////////////////////////////////////////////////
            CommonHelper.SetCommonParams(command);

            entity.SysNo = System.Convert.ToInt32(command.ExecuteScalar());
            entity.SettleID = "V" + entity.SysNo.ToString().PadLeft(9, '0');

            if (entity.SettleItems != null && entity.SettleItems.Count > 0)
            {
                foreach (CollectionPaymentItem item in entity.SettleItems)
                {
                    if (item.SettleSysNo != -1)
                    {
                        item.SettleSysNo = entity.SysNo.Value;
                        CreateSettleItem(item);
                    }
                }
            }

            return entity;
        }

        public CollectionPaymentItem CreateSettleItem(CollectionPaymentItem entity)
        {
            DataCommand command = null;
            if (entity.AcquireReturnPointType.HasValue)
            {
                command = DataCommandManager.GetDataCommand("CreateCollVendorSettleItem");
                command.SetParameterValue("@AcquireReturnPoint", entity.AcquireReturnPoint.Value);
                command.SetParameterValue("@AcquireReturnPointType", entity.AcquireReturnPointType.Value);
            }
            else
            {
                command = DataCommandManager.GetDataCommand("CreateCollVendorSettleItemNOAcquireRP");
            }

            command.SetParameterValue("@SettleSysNo", entity.SettleSysNo);
            command.SetParameterValue("@POConsignToAccLogSysNo", entity.POConsignToAccLogSysNo);
            command.SetParameterValue("@Cost", entity.Cost);
            //command.SetParameterValue("@CurrencySysNo", entity.);
            command.SetParameterValue("@SettlePercentage", entity.SettlePercentage);
            command.SetParameterValue("@SettleType", entity.SettleType);
            command.SetParameterValue("@ConsignSettleRuleSysNO", entity.SettleRuleSysNo);

            CommonHelper.SetCommonParams(command);
            entity.ItemSysNo = System.Convert.ToInt32(command.ExecuteScalar());

            return entity;
        }

        public CollectionPaymentInfo Update(CollectionPaymentInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCollVendorSettle");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@SettleID", entity.SettleID);
            command.SetParameterValue("@VendorSysNo", entity.VendorInfo.SysNo);
            command.SetParameterValue("@StockSysNo", entity.SourceStockInfo.SysNo);
            command.SetParameterValue("@TotalAmt", entity.TotalAmt);
            command.SetParameterValue("@CurrencySysNo", entity.CurrencyCode);
            command.SetParameterValue("@Memo", entity.Memo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@SettleBalanceSysNo", entity.SettleBalanceSysNo);
            decimal taxRate = (decimal)((int)entity.TaxRateData.Value / 100.0);
            command.SetParameterValue("@TaxRate", taxRate);


            //////////////////////////////CLR16063//////////////////////////////
            command.SetParameterValue("@UsingReturnPoint", entity.UsingReturnPoint);
            command.SetParameterValue("@PM_ReturnPointSysNo", entity.PM_ReturnPointSysNo);
            command.SetParameterValue("@PMSysno", entity.PMInfo.SysNo);
            command.SetParameterValue("@ReturnPointC3SysNo", entity.ReturnPointC3SysNo);
            ////////////////////////////////////////////////////////////////////      

            CommonHelper.SetCommonParams(command);

            command.ExecuteNonQuery();

            if (entity.SettleItems != null && entity.SettleItems.Count > 0)
            {
                foreach (CollectionPaymentItem item in entity.SettleItems)
                {
                    if (item.ItemSysNo.HasValue)
                    {
                        command = DataCommandManager.GetDataCommand("UpdateCollVendorSettleItem");
                        CommonHelper.SetCommonParams(command);
                        command.SetParameterValue("@Cost", item.Cost);
                        command.SetParameterValue("@SysNo", item.ItemSysNo.Value);
                        // command.SetParameterValue("@CurrencySysNo", item.CurrencySysNo);
                        //command.SetParameterValue("@AcquireReturnPoint", item.AcquireReturnPoint.Value);
                        //command.SetParameterValue("@AcquireReturnPointType", item.AcquireReturnPointType.Value);
                        command.SetParameterValue("@SettlePercentage", item.SettlePercentage);
                        command.SetParameterValue("@SettleType", item.SettleType);
                        command.SetParameterValue("@ConsignSettleRuleSysNO", item.SettleRuleSysNo);
                        command.ExecuteNonQuery();
                    }
                    //else if (item.ItemSysNo.HasValue)
                    //{
                    //    command = DataCommandManager.GetDataCommand("UpdateCollVendorSettleItemNOAcquirePoint");
                    //    CommonHelper.SetCommonParams(command);
                    //    command.SetParameterValue("@Cost", item.Cost);
                    //    command.SetParameterValue("@SysNo", item.ItemSysNo.Value);
                    //    //command.SetParameterValue("@CurrencySysNo", item.CurrencySysNo);
                    //    command.SetParameterValue("@SettlePercentage", item.SettlePercentage);
                    //    command.SetParameterValue("@SettleType", item.SettleType);
                    //    command.SetParameterValue("@ConsignSettleRuleSysNO", item.ConsignSettleRuleSysNO);
                    //    command.ExecuteNonQuery();
                    //}
                    else
                    {
                        item.SettleSysNo = entity.SysNo;
                        CreateSettleItem(item);
                    }
                }
            }

            return entity;
        }

        public CollectionPaymentInfo UpdateVendorSettleStatus(CollectionPaymentInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCollVendorSettleStatus");

            command.SetParameterValue("@CreateTime", entity.CreateTime);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUserSysNo);
            command.SetParameterValue("@AuditTime", entity.AuditTime);
            command.SetParameterValue("@AuditUserSysNo", entity.AuditUserSysNo);
            command.SetParameterValue("@SettleUserSysNo", entity.SettleUserSysNo);
            command.SetParameterValue("@SettleTime", entity.SettleTime);
            command.SetParameterValue("@Memo", entity.Memo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);
            CommonHelper.SetCommonParams(command);

            command.ExecuteNonQuery();

            return entity;
        }

        public bool DeleteSettleItem(CollectionPaymentItem entity)
        {
            if (entity.ItemSysNo.HasValue)
            {
                DataCommand command = DataCommandManager.GetDataCommand("DeleteCollVendorSettleItem");

                command.SetParameterValue("@SysNo", entity.ItemSysNo.Value);
                CommonHelper.SetCommonParams(command);

                command.ExecuteNonQuery();
            }

            return true;
        }



        public CollectionPaymentInfo GetVendorSettleBySysNo(CollectionPaymentInfo entity)
        {
            DataCommand command;

            if (entity.SysNo.HasValue)
            {
                command = DataCommandManager.GetDataCommand("GetCollVendorSettleBySysNo");

                command.SetParameterValue("@SysNo", entity.SysNo.Value);
                CommonHelper.SetCommonParams(command);

                entity = command.ExecuteEntity<CollectionPaymentInfo>();

                command = DataCommandManager.GetDataCommand("GetCollVendorSettleItemBySettleSysNo");

                command.SetParameterValue("@SettleSysNo", entity.SysNo.Value);
                CommonHelper.SetCommonParams(command);

                entity.SettleItems = command.ExecuteEntityList<CollectionPaymentItem>();
            }

            return entity;
        }

        public bool IsAbandonPayItem(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCollectionPaymentStatusByOrderNo");
            int? status;

            command.SetParameterValue("@SysNo", sysNo);
            CommonHelper.SetCommonParams(command);

            status = command.ExecuteScalar<Nullable<int>>();

            if (status.HasValue)
            {
                if (status.Value == 0)
                {
                    return true;
                }
            }

            return false;
        }



        /// <summary>
        /// 获取统计后的规则对应的商品数量列表
        /// </summary>
        /// <param name="settleSysNo">结算单编号</param>
        /// <returns>获取统计后的规则对应的商品数量列表</returns>
        public List<ConsignSettlementRulesInfo> GetSettleRuleQuantityCount(int settleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCollSettleRuleQuantityCount");
            command.SetParameterValue("@SettleSysNo", settleSysNo);
            return command.ExecuteEntityList<ConsignSettlementRulesInfo>();
        }

        public CollectionPaymentInfo Load(int vendorSettleSysNo)
        {
            CollectionPaymentInfo settle = new CollectionPaymentInfo();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCollVendorSettleBySysNo");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Settle.SysNo DESC"))
            {
                HasPMRight(builder);

                builder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Settle.SysNo",
                    DbType.Int32,
                    "@SettleSysNo",
                    QueryConditionOperatorType.Equal,
                    vendorSettleSysNo);

                ProviderHelper.SetCommonParams(builder, "Settle");

                command.CommandText = builder.BuildQuerySql();
            }

            settle = command.ExecuteEntity<CollectionPaymentInfo>();

            if (settle != null)
            {
                DataCommand commandItem = DataCommandManager.GetDataCommand("GetCollVendorSettleItemsBySysNo");

                commandItem.SetParameterValue("@SettleSysNo", vendorSettleSysNo);
                ProviderHelper.SetCommonParams(commandItem);

                settle.SettleItems = commandItem.ExecuteEntityList<CollectionPaymentItem>();


                settle.SettleItems.ForEach(x =>
                {
                    x.ConsignToAccLogInfo.RateMargin = x.ConsignToAccLogInfo.SalePrice - x.ConsignToAccLogInfo.Cost;
                    x.ConsignToAccLogInfo.CountMany = x.ConsignToAccLogInfo.ProductQuantity * x.ConsignToAccLogInfo.Cost;
                    x.ConsignToAccLogInfo.RateMarginTotal = x.ConsignToAccLogInfo.RateMargin * x.ConsignToAccLogInfo.ProductQuantity;
                    x.ConsignToAccLogInfo.LogSysNo = x.POConsignToAccLogSysNo;
                    x.Cost = x.ConsignToAccLogInfo.Cost.Value;

                });
            }

            return settle;
        }

        #region PM权限处理
        private void HasPMRight(DynamicQuerySqlBuilder builder)
        {
            /*
            var pmHasRight = AuthorizedPMs();
            var createUserSysNo = 0;

            using (IQueryCommon provider = QueryProviderFactory.GetQueryProvider<IQueryCommon>())
            {
                PMCondition conditionUser = new PMCondition()
                {
                    CompanyCode = CPContext.Current.LoginUser.CompanyCode,
                    LoginName = CPContext.Current.LoginUser.LoginName,
                    SourceDirectoryKey = CPContext.Current.LoginUser.SourceDirectoryKey
                };
                createUserSysNo = provider.GetUserSysNo(conditionUser);
            }

            var createIsValid = pmHasRight.Exists(
                x => x == Convert.ToInt32(createUserSysNo));

            if (createIsValid)
            {
                builder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "Settle.PMSysno",
                    DbType.Int32,
                    pmHasRight);
            }
            else
            {
                builder.ConditionConstructor.AddInCondition(
                    QueryConditionRelationType.AND,
                    "Settle.PMSysno",
                    DbType.Int32,
                    new List<int> { -999 });
            }
            */
        }
        #endregion

        public int UpdatePOInstockAmtAndStatus(int poSysNo, int poStatus)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePoInstockAmtAndStatus");
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            cmd.SetParameterValue("@PoStatus", poStatus);
            return cmd.ExecuteNonQuery();
        }

        public POEntity GetPOMaster(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInstockPOMaster");
            command.SetParameterValue("@SysNo", poSysNo);
            CommonHelper.SetCommonParams(command);
            return command.ExecuteEntity<POEntity>();
        }
    }
}
