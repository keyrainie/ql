using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IConsignSettlementDA))]
    public class ConsignSettlementDA : IConsignSettlementDA
    {
        #region IConsignSettlementDA Members

        public BizEntity.PO.ConsignSettlementInfo LoadConsignSettlementInfo(int consignSettlementSysNo)
        {
            ConsignSettlementInfo returnEntity = new ConsignSettlementInfo();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleBySysNo");
              
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Settle.SysNo DESC"))
            {
                //TODO: PM权限处理:
                //HasPMRight(builder);

                builder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Settle.SysNo",
                    DbType.Int32,
                    "@SettleSysNo",
                    QueryConditionOperatorType.Equal,
                    consignSettlementSysNo);

                command.CommandText = builder.BuildQuerySql();
            }
            returnEntity = command.ExecuteEntity<ConsignSettlementInfo>();
            return returnEntity;
        }

        public List<ConsignSettlementItemInfo> LoadConsignSettlementItemList(int consignSettlementSysNo)
        {
            List<ConsignSettlementItemInfo> returnList = new List<ConsignSettlementItemInfo>();
            DataCommand commandItem = DataCommandManager.GetDataCommand("GetVendorSettleItemsBySysNo");
            
            commandItem.SetParameterValue("@SettleSysNo", consignSettlementSysNo);
            returnList = commandItem.ExecuteEntityList<ConsignSettlementItemInfo>();
            returnList.ForEach(x =>
            {
                x.ConsignToAccLogInfo.RateMargin = x.ConsignToAccLogInfo.SalePrice - x.ConsignToAccLogInfo.Cost;
                x.ConsignToAccLogInfo.CountMany = x.ConsignToAccLogInfo.ProductQuantity * x.ConsignToAccLogInfo.Cost;
                x.ConsignToAccLogInfo.RateMarginTotal = x.ConsignToAccLogInfo.RateMargin * x.ConsignToAccLogInfo.ProductQuantity;
                x.ConsignToAccLogInfo.LogSysNo = x.POConsignToAccLogSysNo;
                x.Cost = x.ConsignToAccLogInfo.Cost;
                if (x.AcquireReturnPointType.HasValue)
                {
                    if (x.AcquireReturnPointType == 0)
                    {
                        x.ExpectGetPoint = x.AcquireReturnPoint * x.ConsignToAccLogInfo.ProductQuantity;
                    }
                    if (x.AcquireReturnPointType == 1)
                    {
                        x.ExpectGetPoint = x.AcquireReturnPoint.Value / 100 * x.ConsignToAccLogInfo.Cost * x.ConsignToAccLogInfo.ProductQuantity;
                    }
                }
            });
            return returnList;
        }

        public ConsignSettlementInfo UpdateConsignSettlementInfo(ConsignSettlementInfo settlementInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettle");

            command.SetParameterValue("@SysNo", settlementInfo.SysNo);
            command.SetParameterValue("@SettleID", settlementInfo.ReferenceSysNo);
            command.SetParameterValue("@VendorSysNo", settlementInfo.VendorInfo.SysNo);
            command.SetParameterValue("@StockSysNo", settlementInfo.SourceStockInfo.SysNo);
            command.SetParameterValue("@TotalAmt", settlementInfo.TotalAmt);
            command.SetParameterValue("@CurrencySysNo", settlementInfo.CurrencyCode);
            command.SetParameterValue("@Memo", settlementInfo.Memo);
            command.SetParameterValue("@Note", settlementInfo.Note);
            command.SetParameterValue("@SettleBalanceSysNo", settlementInfo.SettleBalanceSysNo);
            command.SetParameterValue("@TaxRate", ((int)settlementInfo.TaxRateData.Value / 100.0));

            //////////////////////////////CLR16063//////////////////////////////
            command.SetParameterValue("@UsingReturnPoint", settlementInfo.EIMSInfo.UsingReturnPoint);
            command.SetParameterValue("@PM_ReturnPointSysNo", settlementInfo.PM_ReturnPointSysNo);
            command.SetParameterValue("@PMSysno", settlementInfo.PMInfo.SysNo);
            command.SetParameterValue("@ReturnPointC3SysNo", settlementInfo.ReturnPointC3SysNo);
            ////////////////////////////////////////////////////////////////////
            command.SetParameterValue("@ProductLineSysNo", settlementInfo.ProductLineSysNo); 

            command.ExecuteNonQuery();

            if (settlementInfo.ConsignSettlementItemInfoList != null && settlementInfo.ConsignSettlementItemInfoList.Count > 0)
            {
                foreach (ConsignSettlementItemInfo item in settlementInfo.ConsignSettlementItemInfoList)
                {
                    //更新SettleItem:
                    if (item.ItemSysNo.HasValue && item.AcquireReturnPointType.HasValue && item.AcquireReturnPoint.HasValue)
                    {
                        command = DataCommandManager.GetDataCommand("UpdateVendorSettleItem");

                        command.SetParameterValue("@Cost", item.Cost);
                        command.SetParameterValue("@SysNo", item.ItemSysNo);
                        command.SetParameterValue("@CurrencySysNo", item.CurrenyCode);
                        command.SetParameterValue("@AcquireReturnPoint", item.AcquireReturnPoint.Value);
                        command.SetParameterValue("@AcquireReturnPointType", item.AcquireReturnPointType.Value);
                        command.SetParameterValue("@SettlePercentage", item.SettlePercentage);
                        command.SetParameterValue("@SettleType", item.SettleType.Value.ToString());
                        command.SetParameterValue("@SettleRuleSysNo", item.SettleRuleSysNo);
                        command.ExecuteNonQuery();
                    }
                    else if (item.ItemSysNo.HasValue)
                    {
                        command = DataCommandManager.GetDataCommand("UpdateVendorSettleItemNOAcquirePoint");

                        command.SetParameterValue("@Cost", item.Cost);
                        command.SetParameterValue("@SysNo", item.ItemSysNo);
                        command.SetParameterValue("@CurrencySysNo", item.CurrenyCode);
                        command.SetParameterValue("@SettlePercentage", item.SettlePercentage);
                        command.SetParameterValue("@SettleType", item.SettleType.Value.ToString());
                        command.SetParameterValue("@SettleRuleSysNo", item.SettleRuleSysNo);
                        command.ExecuteNonQuery();
                    }
                    else
                    {//新建SettleItem:
                        item.SettleSysNo = settlementInfo.SysNo;
                        CreateConsignSettlemtnItemInfo(item);
                    }
                }
            }

            return settlementInfo;
        }

        public ConsignSettlementItemInfo DeleteConsignSettlementItemInfo(ConsignSettlementItemInfo settlementItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteVendorSettleItem");

            command.SetParameterValue("@SysNo", settlementItemInfo.ItemSysNo);
            command.ExecuteNonQuery();
            return settlementItemInfo;
        }

        public void UpdateConsignToAccountLogStatus(int sysNo, ConsignToAccountLogStatus newStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConsignToAccountLogStatus");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", (int)newStatus);
            command.ExecuteNonQuery();
        }

        public ConsignSettlementInfo CreateConsignSettlementInfo(ConsignSettlementInfo consignSettlementInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorSettle");

            command.SetParameterValue("@SysNo", consignSettlementInfo.SysNo);
            command.SetParameterValue("@SettleID", consignSettlementInfo.ReferenceSysNo);
            command.SetParameterValue("@VendorSysNo", consignSettlementInfo.VendorInfo.SysNo);
            command.SetParameterValue("@StockSysNo", consignSettlementInfo.SourceStockInfo.SysNo);
            command.SetParameterValue("@TotalAmt", consignSettlementInfo.TotalAmt);
            command.SetParameterValue("@CurrencySysNo", consignSettlementInfo.CurrencyCode);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@Memo", consignSettlementInfo.Memo);
            command.SetParameterValue("@Note", consignSettlementInfo.Note);
            command.SetParameterValue("@Status", (int)consignSettlementInfo.Status);
            command.SetParameterValue("@SettleBalanceSysNo", consignSettlementInfo.SettleBalanceSysNo);
            command.SetParameterValue("@TaxRate", ((decimal)consignSettlementInfo.TaxRateData) / 100m);
            //////////////////////////////CLR16063//////////////////////////////
            command.SetParameterValue("@UsingReturnPoint", (!consignSettlementInfo.EIMSInfo.UsingReturnPoint.HasValue || consignSettlementInfo.EIMSInfo.UsingReturnPoint == 0m) ? (decimal?)null : consignSettlementInfo.EIMSInfo.UsingReturnPoint.Value);
            command.SetParameterValue("@PM_ReturnPointSysNo", consignSettlementInfo.PM_ReturnPointSysNo);
            command.SetParameterValue("@PMSysno", consignSettlementInfo.PMInfo.SysNo);
            command.SetParameterValue("@ReturnPointC3SysNo", consignSettlementInfo.ReturnPointC3SysNo);
            ////////////////////////////////////////////////////////////////////
            command.SetParameterValue("@CompanyCode", consignSettlementInfo.CompanyCode);
            command.SetParameterValue("@ProductLineSysNo", consignSettlementInfo.ProductLineSysNo);
            command.SetParameterValue("@IsToLease", (int)consignSettlementInfo.LeaseType);
            command.SetParameterValue("@DeductAmt", consignSettlementInfo.DeductAmt);
            command.SetParameterValue("@DeductAccountType", consignSettlementInfo.DeductAccountType==null?-1: (int)consignSettlementInfo.DeductAccountType);
            command.SetParameterValue("@DeductMethod",consignSettlementInfo.DeductMethod==null?-1: (int)consignSettlementInfo.DeductMethod);
            command.SetParameterValue("@ConsignRange",consignSettlementInfo.ConsignRange);
            consignSettlementInfo.SysNo = System.Convert.ToInt32(command.ExecuteScalar());
            consignSettlementInfo.SettleID = "V" + consignSettlementInfo.SysNo.ToString().PadLeft(9, '0');

            if (consignSettlementInfo.ConsignSettlementItemInfoList != null && consignSettlementInfo.ConsignSettlementItemInfoList.Count > 0)
            {
                foreach (ConsignSettlementItemInfo item in consignSettlementInfo.ConsignSettlementItemInfoList)
                {
                    item.SettleSysNo = consignSettlementInfo.SysNo.Value;
                    item.CompanyCode = consignSettlementInfo.CompanyCode;
                    CreateConsignSettlemtnItemInfo(item);
                }
            }

            return consignSettlementInfo;
        }

        public ConsignSettlementItemInfo CreateConsignSettlemtnItemInfo(ConsignSettlementItemInfo settlementItemInfo)
        {
            DataCommand command = null;
            if (settlementItemInfo.AcquireReturnPointType.HasValue)
            {
                command = DataCommandManager.GetDataCommand("CreateVendorSettleItem");
                command.SetParameterValue("@AcquireReturnPoint", settlementItemInfo.AcquireReturnPoint.Value);
                command.SetParameterValue("@AcquireReturnPointType", settlementItemInfo.AcquireReturnPointType.Value);
            }
            else
            {
                command = DataCommandManager.GetDataCommand("CreateVendorSettleItemNOAcquireRP");
            }

            command.SetParameterValue("@SettleSysNo", settlementItemInfo.SettleSysNo);
            command.SetParameterValue("@POConsignToAccLogSysNo", settlementItemInfo.POConsignToAccLogSysNo);
            command.SetParameterValue("@Cost", settlementItemInfo.Cost);
            command.SetParameterValue("@CurrencySysNo", settlementItemInfo.CurrenyCode);
            command.SetParameterValue("@SettlePercentage", settlementItemInfo.SettlePercentage);
            command.SetParameterValue("@SettleType", settlementItemInfo.SettleType);
            command.SetParameterValue("@ConsignSettleRuleSysNO", settlementItemInfo.SettleRuleSysNo);
            command.SetParameterValue("@CompanyCode", "8601");

            settlementItemInfo.ItemSysNo = System.Convert.ToInt32(command.ExecuteScalar());
            return settlementItemInfo;
        }

        public ConsignToAcctLogInfo LoadConsignToAccountLogInfo(int? consignToAccountLogSysNo)
        {
            if (consignToAccountLogSysNo.HasValue)
            {
                ConsignToAcctLogInfo entity;
                DataCommand command = DataCommandManager.GetDataCommand("GetConsignToAccountLog");
                command.SetParameterValue("@SysNo", consignToAccountLogSysNo);
                entity = command.ExecuteEntity<ConsignToAcctLogInfo>();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public bool IsAccountLogExistOtherVendorSettle(int accountLogSysNo)
        {
            DataCommand command;

            command = DataCommandManager.GetDataCommand("IsAccountLogExistOtherVendorSettle");

            command.SetParameterValue("@SysNo", accountLogSysNo);

            return command.ExecuteScalar().ToInteger() > 0 ? true : false;
        }

        public ConsignSettlementInfo UpdateAuditStatus(ConsignSettlementInfo settlementInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAuditStatus");

            command.SetParameterValue("@AuditTime", settlementInfo.AuditDate);
            command.SetParameterValue("@AuditUserSysNo", settlementInfo.AuditUser.UserID);
            command.SetParameterValue("@Memo", settlementInfo.Memo);
            command.SetParameterValue("@Note", settlementInfo.Note);
            command.SetParameterValue("@Status", (int)settlementInfo.Status);
            command.SetParameterValue("@SysNo", settlementInfo.SysNo);
            command.ExecuteNonQuery();

            return settlementInfo;
        }

        public ConsignSettlementInfo UpdateSettleStatus(ConsignSettlementInfo settlementInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSettleStatus");

            command.SetParameterValue("@SettleTime", settlementInfo.SettleDate);
            command.SetParameterValue("@SettleUserSysNo", settlementInfo.SettleUser.UserID);
            command.SetParameterValue("@Memo", settlementInfo.Memo);
            command.SetParameterValue("@Note", settlementInfo.Note);
            command.SetParameterValue("@Status", (int)settlementInfo.Status);
            command.SetParameterValue("@SysNo", settlementInfo.SysNo);
            command.ExecuteNonQuery();

            return settlementInfo;
        }

        public void SettleConsignToAccountLog(int accountLogSysNo, decimal? cost,decimal foldCost)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SettleConsignToAccountLog");
             
            command.SetParameterValue("@SysNo", accountLogSysNo);
            command.SetParameterValue("@SettleCost", cost);
            command.SetParameterValue("@FoldCost", foldCost);
            command.SetParameterValue("@Status", (int)ConsignToAccountLogStatus.Settled);
            command.ExecuteNonQuery();
        }

        public void CancelSettleConsignToAccountLog(int accountLogSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CancelSettleConsignToAccountLog");

            command.SetParameterValue("@SysNo", accountLogSysNo);
            command.SetParameterValue("@SettleCost", DBNull.Value);
            command.SetParameterValue("@FoldCost", DBNull.Value);
            command.SetParameterValue("@Status", (int)ConsignToAccountLogStatus.ManualCreated);

            command.ExecuteNonQuery();
        }

        #endregion IConsignSettlementDA Members

        #region IConsignSettlementDA Members

        public ConsignToAcctLogInfo CreateConsignToAccountLog(ConsignToAcctLogInfo logInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateConsignToAccountLog");

            command.SetParameterValue("@SysNo", logInfo.LogSysNo);
            command.SetParameterValue("@ProductSysNo", logInfo.ProductSysNo);
            command.SetParameterValue("@VendorSysNo", logInfo.VendorSysNo);
            command.SetParameterValue("@StockSysNo", logInfo.StockSysNo);
            command.SetParameterValue("@Quantity", logInfo.ProductQuantity);
            command.SetParameterValue("@CreateCost", logInfo.CreateCost);
            command.SetParameterValue("@SettleCost", logInfo.SettleCost);
            command.SetParameterValue("@ConsignToAccType", (int)logInfo.ConsignToAccType);
            command.SetParameterValue("@Note", logInfo.Note);
            object getCode = "";
            EnumCodeMapper.TryGetCode(logInfo.ConsignToAccStatus, out getCode);
            if (null != getCode)
            {
                command.SetParameterValue("@Status", getCode.ToString());
            }
            command.SetParameterValue("@SettleType", logInfo.SettleType.ToString());
            command.SetParameterValue("@SettlePercentage", logInfo.SettlePercentage);
            command.SetParameterValue("@SalePrice", logInfo.SalePrice);
            command.SetParameterValue("@Point", logInfo.Point);
            command.SetParameterValue("@OrderSysNo", logInfo.OrderSysNo);
            command.SetParameterValue("@CompanyCode", logInfo.CompanyCode);
            logInfo.LogSysNo = System.Convert.ToInt32(command.ExecuteScalar());
            return logInfo;
        }

        #endregion IConsignSettlementDA Members

        private string GetConsignToAccountLogChar(ConsignToAccountLogStatus status)
        {
            switch (status)
            {
                case ConsignToAccountLogStatus.Origin:
                    return "A";
                case ConsignToAccountLogStatus.SystemCreated:
                    return "S";
                case ConsignToAccountLogStatus.ManualCreated:
                    return "C";
                case ConsignToAccountLogStatus.Settled:
                    return "F";
                default:
                    return null;
            }
        }

        public List<int> GetBackUpPMList(int userSysNo, string companyCode)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetBackupUserListBySysNo");
            dataCommand.SetParameterValue("@PMUserSysNo", userSysNo);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            string userList = dataCommand.ExecuteScalar() as string;
            List<int> result = new List<int>();
            if (!string.IsNullOrEmpty(userList))
            {
                string[] pms = userList.Split(';');
                int[] pmlist = Array.ConvertAll(pms, str =>
                {
                    int p = 0;
                    int.TryParse(str, out p);
                    return p;
                });
                result.AddRange(pmlist);
            }
            return result;
        }

        public bool ExistsDifferentPMSysNo(List<int> pms, List<int> productSysNo, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("ExistsDifferentPMSysNoNew");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "SysNo desc"))
            {
                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "product.SysNo", DbType.Int32, productSysNo);
                builder.ConditionConstructor.AddNotInCondition(QueryConditionRelationType.AND, "product.PMUserSysNo", DbType.Int32, pms);

                command.CommandText = builder.BuildQuerySql();
                command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength);
                command.SetParameterValue("@CompanyCode", companyCode);

                return command.ExecuteScalar<int>() > 0;
            }
        }

        public int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetConsignSettlementReturnPointSysNo");
            command.SetParameterValue("@SysNo", consignSettleSysNo);
            var result = command.ExecuteScalar();

            if (result == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <returns></returns>
        public List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleSysNoListByVendorSysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
              dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VendorSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, vendorSysNo);
                if (pmSysNoList != null && pmSysNoList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "PMSysNo", DbType.Int32, pmSysNoList);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                return dataCommand.ExecuteFirstColumn<Int32>();
            }
        }

        /// <summary>
        /// 创建代销转财务记录(Inventory)
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public ConsignToAcctLogInfo CreatePOConsignToAccLogForInventory(ConsignToAcctLogInfo logInfo)
        {
            var command = DataCommandManager.GetDataCommand("CreatePOConsignToAccLogForInventory");
            command.SetParameterValue("@ProductSysNo", logInfo.ProductSysNo);
            command.SetParameterValue("@StockSysNo", logInfo.StockSysNo);
            command.SetParameterValue("@Quantity", logInfo.ProductQuantity);
            command.SetParameterValue("@CreateTime", logInfo.OutStockTime);
            command.SetParameterValue("@CreateCost", logInfo.CreateCost);
            command.SetParameterValue("@CompanyCode", logInfo.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", logInfo.StoreCompanyCode);
            command.SetParameterValue("@OrderSysNo", logInfo.OrderSysNo);
            command.SetParameterValue("@VendorSysNoOut", logInfo.VendorSysNo.HasValue ? logInfo.VendorSysNo : 0);
            command.SetParameterValue("@IsConsign", logInfo.IsConsign.HasValue?logInfo.IsConsign:1);
            logInfo.LogSysNo = System.Convert.ToInt32(command.ExecuteScalar());
            return logInfo;
        }


        public List<ConsignSettlementRulesInfo> GetSettleRuleQuantityCount(int settleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleRuleQuantityCount");
            command.SetParameterValue("@SettleSysNo", settleSysNo);
            return command.ExecuteEntityList<ConsignSettlementRulesInfo>();
            //return command.ExecuteEntityList<ConsignSettlementRulesInfo>((reader, item) =>
            //{
            //    object tmpObj = reader.GetValue(reader.GetOrdinal("Status"));
            //    if (tmpObj != null)
            //    {
            //        switch (tmpObj.ToString().Trim().ToUpper())
            //        {
            //            case ConsignSettleRuleStatus.Available.ToString():
            //                break;
            //        }
            //    }
            //});
        }


        public bool UpdateConsignSettleRuleStatusAndQuantity(int settleRuleId, ConsignSettleRuleStatus status, int settledQuantity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConsignSettleRuleStatus");
            command.SetParameterValue("@SysNo", settleRuleId);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@Quantity", settledQuantity);
            return command.ExecuteNonQuery() > 0;
        }


        public void UpdateExistsConsignSettleRuleItemStatus(int settleRuleId)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateExistsConsignSettleRuleItemStatus");
            command.SetParameterValue("@SysNo", settleRuleId);
            command.ExecuteNonQuery();
        }


        #region 经销商品结算

        /// <summary>
        /// 添加经销商品结算单
        /// </summary>
        /// <param name="settleInfo"></param>
        /// <returns></returns>
        public SettleInfo SettleInfoAdd(SettleInfo settleInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SettleInfoAdd");

            command.SetParameterValue("@VendorSysNo", settleInfo.VendorSysNo);
            command.SetParameterValue("@StockSysNo", settleInfo.StockSysNo);
            command.SetParameterValue("@TotalAmt", settleInfo.TotalAmt);
            command.SetParameterValue("@CreateTime", settleInfo.CreateTime);
            command.SetParameterValue("@CreateUserSysNo", settleInfo.CreateUserSysNo);
            command.SetParameterValue("@Status", settleInfo.Status);
            command.SetParameterValue("@CompanyCode", settleInfo.CompanyCode);

            command.ExecuteNonQuery();

            object sysNoObj = command.GetParameterValue("@SysNo");
            settleInfo.SysNo = sysNoObj.ToInteger();

            return settleInfo;
        }

        /// <summary>
        /// 添加经销商品结算单子项
        /// </summary>
        /// <param name="SettleItemInfo"></param>
        /// <returns></returns>
        public SettleItemInfo SettleItemInfoAdd(SettleItemInfo settleItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SettleItemInfoAdd");

            command.SetParameterValue("@SettleSysNo", settleItemInfo.SettleSysNo);
            //command.SetParameterValue("@OrderSysNo", settleItemInfo.OrderSysNo);
            //command.SetParameterValue("@OrderType", settleItemInfo.FinancePayOrderType);
            command.SetParameterValue("@CompanyCode", settleItemInfo.CompanyCode);
            command.SetParameterValue("@FinancePaySysNo", settleItemInfo.FinancePaySysNo);
            command.ExecuteNonQuery();

            object sysNoObj = command.GetParameterValue("@SysNo");
            settleItemInfo.SysNo = sysNoObj.ToInteger();

            return settleItemInfo;
        }

        /// <summary>
        /// 获取经销商品结算单信息
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public SettleInfo GetSettleInfo(SettleInfo settleInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleInfo");
            command.SetParameterValue("@SettleSysNo", settleInfo.SysNo);
            settleInfo = command.ExecuteEntity<SettleInfo>();
            return settleInfo;
        }
        /// <summary>
        /// 获取经销商品结算单子项
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public List<SettleItemInfo> GetSettleItemInfo(SettleInfo settleInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleItemInfo");
            command.SetParameterValue("@SettleSysNo", settleInfo.SysNo);
             var result = command.ExecuteEntityList<SettleItemInfo>();
             return result;
        }

        /// <summary>
        /// 获取经销商品结算单子项（附带着税金价款信息）
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public List<SettleItemInfo> GetSettleItemInfoWithTaxAndCost(SettleInfo settleInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleItemInfoWithTaxAndCost");
            command.SetParameterValue("@SettleSysNo", settleInfo.SysNo);
            var result = command.ExecuteEntityList<SettleItemInfo>();
            return result;
        }

        /// <summary>
        /// 修改经销商品状态(审核，审核通过 或者 作废)
        /// </summary>
        /// <param name="SettleInfo"></param>
        public void AuditSettleAccount(SettleInfo SettleInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AuditSettleAccount");
            command.SetParameterValue("@SysNo", SettleInfo.SysNo);
            command.SetParameterValue("@status", SettleInfo.Status);
            command.SetParameterValue("@AuditTime", SettleInfo.AuditTime);
            command.SetParameterValue("@AuditUserSysNo", SettleInfo.AuditUserSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改Finance_Pay 支付状态（创建经销商结算单是 修改状态为2 ，如果作废做返回状态到 0）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="OrderSysNo"></param>
        public void ChangeFinancePayStatus(PayableStatus status, int financePaySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangeFinancePayStatus");
            command.SetParameterValue("@PayStatus", status);
            //command.SetParameterValue("@OrderSysNo", OrderSysNo);
            //command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@FinancePaySysNo", financePaySysNo);
            command.ExecuteNonQuery();
 
        }

        /// <summary>
        /// 修改支付字表的支付状态为已支付
        /// </summary>
        /// <param name="payItemStatus"></param>
        /// <param name="dtime"></param>
        /// <param name="financePaySysNo"></param>
        public void FinancePayItemPaid(int financePaySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("FinancePayItemPaid");
            command.SetParameterValue("@PayItemStatus", PayItemStatus.Paid);
            command.SetParameterValue("@PaySysNo", financePaySysNo);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改支付字表的支付状态为待支付状态
        /// </summary>
        /// <param name="payItemStatus"></param>
        /// <param name="financePaySysNo"></param>
        public void FinancePayItemOrigin(int financePaySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("FinancePayItemOrigin");
            command.SetParameterValue("@PayItemStatus", PayItemStatus.Origin);
            command.SetParameterValue("@PaySysNo", financePaySysNo);

            command.ExecuteNonQuery();
        }

        #endregion 
   
    }
}
