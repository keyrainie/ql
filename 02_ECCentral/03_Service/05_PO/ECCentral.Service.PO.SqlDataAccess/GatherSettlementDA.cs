using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IGatherSettlementDA))]
    public class GatherSettlementDA : IGatherSettlementDA
    {
        #region IGatherSettlementDA Members

        public List<BizEntity.PO.GatherSettlementItemInfo> LoadConsignSettlementAllShippingCharge(BizEntity.PO.GatherSettlementInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleGatherALLShippingCharge");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (info.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, info.VendorInfo.SysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, info.CompanyCode);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                }
                command.CommandText = builder.BuildQuerySql();
                string strSQl1 = string.Empty, strSQl2 = string.Empty, strSQl3 = string.Empty;
                // ##OrderDate1##   ##OrderDate2##
                if (!string.IsNullOrEmpty(info.ReferenceSysNo))
                {
                    strSQl1 += " AND main.SONumber =" + info.ReferenceSysNo.ToString() + "  ";
                    strSQl2 += " AND main.SysNo = " + info.ReferenceSysNo.ToString() + "  ";
                    strSQl3 += " AND main.SysNo = " + info.ReferenceSysNo.ToString() + "  ";
                }
                if (info.CreateDateFrom.HasValue)
                {
                    strSQl1 += " AND main.OrderDate>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                    strSQl3 += " AND main.CreateTime>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                }
                if (info.CreateDateTo.HasValue)
                {
                    strSQl1 += " AND main.OrderDate<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                    strSQl3 += " AND main.CreateTime<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }

                if (info.OutStockRefundDateFrom.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate>'" + info.OutStockRefundDateFrom.ToString() + "'";
                    strSQl2 += " AND main.RefundTime>'" + info.OutStockRefundDateFrom.ToString() + "'";
                    strSQl3 += " AND main.RefundTime>'" + info.OutStockRefundDateFrom.ToString() + "'";
                }

                if (info.OutStockRefundDateTo.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate<'" + info.OutStockRefundDateTo.ToString() + "'";
                    strSQl2 += " AND main.RefundTime<'" + info.OutStockRefundDateTo.ToString() + "'";
                    strSQl3 += " AND main.RefundTime<'" + info.OutStockRefundDateTo.ToString() + "'";
                }

                if (info.SourceStockInfo != null && info.SourceStockInfo.SysNo.HasValue)
                {
                    strSQl1 += " AND main.WarehouseNumber=" + info.SourceStockInfo.SysNo.Value.ToString() + " ";
                    strSQl2 += " AND register.LocationWarehouse=" + info.SourceStockInfo.SysNo.Value.ToString() + " ";
                }
                else
                {
                    throw new BizException("未选择仓库，不能进行该操作！");
                }
                command.CommandText = command.CommandText.Replace("##OrderDate1##", strSQl1).Replace("##OrderDate2##", strSQl2).Replace("##OrderDate3##", strSQl3);
            }
            return command.ExecuteEntityList<GatherSettlementItemInfo>();
        }

        public List<GatherSettlementItemInfo> QueryConsignSettlementProductList(GatherSettleItemsQueryFilter queryFilter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleGatherItems");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "b.SONumber DESC"))
            {
                if (queryFilter.VendorSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                }
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                command.CommandText = builder.BuildQuerySql();
                string strSQl1 = string.Empty, strSQl2 = string.Empty, strSQl3 = string.Empty;
                // ##OrderDate1##   ##OrderDate2##
                int referenceId = 0;
                if (int.TryParse(queryFilter.ReferenceSysNo, out referenceId))
                {
                    strSQl1 += " AND main.SONumber =" + queryFilter.ReferenceSysNo + "  ";
                    strSQl2 += " AND main.SysNo = " + queryFilter.ReferenceSysNo + "  ";
                    strSQl3 += " AND main.SysNo = " + queryFilter.ReferenceSysNo + "  ";
                }
                if (queryFilter.CreateDateFrom.HasValue)
                {
                    strSQl1 += " AND main.OrderDate>'" + queryFilter.CreateDateFrom.ToString() + "'";
                    strSQl2 += " AND main.CreateTime>'" + queryFilter.CreateDateFrom.ToString() + "'";
                    strSQl3 += " AND main.CreateTime>'" + queryFilter.CreateDateFrom.ToString() + "'";
                }

                if (queryFilter.CreateDateTo.HasValue)
                {
                    strSQl1 += " AND main.OrderDate<'" + queryFilter.CreateDateTo.ToString() + "'";
                    strSQl2 += " AND main.CreateTime<'" + queryFilter.CreateDateTo.ToString() + "'";
                    strSQl3 += " AND main.CreateTime<'" + queryFilter.CreateDateTo.ToString() + "'";
                }

                if (queryFilter.OutStockRefundDateFrom.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate>'" + queryFilter.OutStockRefundDateFrom.ToString() + "'";
                    strSQl2 += " AND main.RefundTime>'" + queryFilter.OutStockRefundDateFrom.ToString() + "'";
                    strSQl3 += " AND main.RefundTime>'" + queryFilter.OutStockRefundDateFrom.ToString() + "'";
                }

                if (queryFilter.OutStockRefundDateTo.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate<'" + queryFilter.OutStockRefundDateTo.ToString() + "'";
                    strSQl2 += " AND main.RefundTime<'" + queryFilter.OutStockRefundDateTo.ToString() + "'";
                    strSQl3 += " AND main.RefundTime<'" + queryFilter.OutStockRefundDateTo.ToString() + "'";
                }

                if (queryFilter.StockSysNo.HasValue)
                {
                    strSQl1 += " AND main.WarehouseNumber=" + queryFilter.StockSysNo.Value.ToString() + " ";
                    strSQl2 += " AND register.LocationWarehouse=" + queryFilter.StockSysNo.Value.ToString() + " ";
                }
                else
                {
                    throw new BizException("未选择仓库，不能进行该操作！");
                }
                command.CommandText = command.CommandText.Replace("##OrderDate1##", strSQl1).Replace("##OrderDate2##", strSQl2).Replace("##OrderDate3##", strSQl3);
                List<GatherSettlementItemInfo> list = command.ExecuteEntityList<GatherSettlementItemInfo>();
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return list;
            }
        }

        public GatherSettlementInfo CreateGatherSettlement(GatherSettlementInfo info)
        {
            List<int> soList = new List<int>();
            if (info.GatherSettlementItemInfoList != null && info.GatherSettlementItemInfoList.Count > 0)
            {
                var list = from item in info.GatherSettlementItemInfoList
                           select item.OrderSysNo.Value;
                soList = list.ToList().Distinct().ToList();

                int gatherCount = 1000;
                string getItemCountAppSetting = AppSettingManager.GetSetting("PO", "GatherItemsCount");
                if (!string.IsNullOrEmpty(getItemCountAppSetting))
                {
                    gatherCount = int.Parse(getItemCountAppSetting);
                }
                if (soList.Count > gatherCount)
                {
                    throw new BizException(string.Format("该代收结算单单号数量超过{0}，请缩小查询条件", gatherCount.ToString()));
                }
            }

            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorSettleGather");

            command.SetParameterValue("@MerchantSysNo", info.VendorInfo.SysNo.Value);
            command.SetParameterValue("@StockSysNo", info.SourceStockInfo.SysNo.Value);
            command.SetParameterValue("@TotalAmt", info.TotalAmt);
            command.SetParameterValue("@InUser", info.CreateUserName);
            command.SetParameterValue("@InDate", DateTime.Now);
            command.SetParameterValue("@EditUser", info.CreateUserName);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@CurrencySysNo", "CNY");
            command.SetParameterValue("@Status", info.SettleStatus.ToString());
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", info.CompanyCode);
            command.SetParameterValue("@Memo", string.IsNullOrEmpty(info.Memo) ? "" : info.Memo);
            ////////////////////////////////////////////////////////////////////

            info.SysNo = System.Convert.ToInt32(command.ExecuteScalar());

            //创建Items:
            if (info.GatherSettlementItemInfoList != null && info.GatherSettlementItemInfoList.Count > 0)
            {
                List<string> listString = new List<string>();
                int k = 0;

                foreach (int soNumber in soList)
                {
                    if (k % 500 == 0)
                    {
                        listString.Add(soNumber.ToString());
                    }
                    else
                    {
                        listString[k / 500] = listString[k / 500] + "," + soNumber.ToString();
                    }
                    k++;
                }

                foreach (string solist in listString)
                {
                    CreateGatherItem(info, solist);
                    CreateShippingItem(info, solist);
                    CreateROAdjustItem(info, solist);
                }
            }
            info.GatherSettlementItemInfoList = null;
            return info;
        }

        public List<GatherSettlementItemInfo> GetSettleGatherItems(GatherSettlementInfo info)
        {
            StringBuilder strBuid = new StringBuilder();
            strBuid.Append(" AND (  1=2 ");
            if (info.GatherSettlementItemInfoList != null)
            {
                foreach (var item in info.GatherSettlementItemInfoList)
                {
                    strBuid.Append(string.Format(" OR (ReferenceSysNo={0} AND ReferenceType = '{1}') ", item.OrderSysNo, item.SettleType.Value.ToString()));
                }
            }
            strBuid.Append("  ) ");
            DataCommand command = DataCommandManager.GetDataCommand("GetCheckSettleGatherItemsBySettleSysNo");
            command.SetParameterValue("@SettleSysNo", info.SysNo);
            command.ReplaceParameterValue("@WHERESTRING", strBuid.ToString());
            return command.ExecuteEntityList<GatherSettlementItemInfo>();
        }

        public List<GatherSettlementItemInfo> GetSettleGatherItems(int settleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleGatherItemsBySettleSysNo");
            command.SetParameterValue("@SettleSysNo", settleSysNo);

            return command.ExecuteEntityList<GatherSettlementItemInfo>();
        }

        public GatherSettlementInfo GetVendorSettleGatherInfo(GatherSettlementInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleGatherEntityBySettleSysNo");
            command.SetParameterValue("@SysNo", info.SysNo);
            return command.ExecuteEntity<GatherSettlementInfo>();
        }

        public GatherSettlementItemInfo DeleteSettleItem(GatherSettlementItemInfo info, int settleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteVendorSettleGatherItem");
            command.SetParameterValue("@ReferenceSysNo", info.OrderSysNo.Value);
            command.SetParameterValue("@ReferenceType", info.SettleType.ToString());
            command.SetParameterValue("@SettlementSysNo", settleSysNo);
            command.ExecuteNonQuery();
            return info;
        }

        public GatherSettlementInfo UpdateGatherSettlement(GatherSettlementInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettleGather");
            command.SetParameterValue("@MerchantSysNo", info.VendorInfo.SysNo.Value);
            command.SetParameterValue("@StockSysNo", info.SourceStockInfo.SysNo.Value);
            command.SetParameterValue("@TotalAmt", info.TotalAmt);
            command.SetParameterValue("@EditUser", info.CreateUserName);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@SysNo", info.SysNo);
            command.ExecuteNonQuery();
            return info;
        }

        public GatherSettlementInfo UpdateGatherSettlementStatus(GatherSettlementInfo info, bool needUpdateTime)
        {
            if (needUpdateTime)
            {
                //更新审核时间(用于审核操作 ） ：
                DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettleGatherStatus");
                command.SetParameterValue("@Status", info.SettleStatus.ToString());
                command.SetParameterValue("@SysNo", info.SysNo);
                command.SetParameterValue("@AuditUser", info.AuditUser.UserDisplayName);
                command.SetParameterValue("@AuditDate", DateTime.Now);
                command.SetParameterValue("@SettleUser", info.SettleUser);
                command.SetParameterValue("@SettleDate", info.SettleDate);
                command.ExecuteNonQuery();
            }
            else
            {
                //不需要更新审核时间(用于取消审核,作废操作 ） ：
                DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettleGatherStatusNoUpdateTime");
                command.SetParameterValue("@Status", info.SettleStatus.ToString());
                command.SetParameterValue("@SysNo", info.SysNo);
                command.ExecuteNonQuery();
            }
            return info;
        }

        public GatherSettlementInfo UpdateGatherSettlementSettleStatus(GatherSettlementInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorSettleGatherSettleStatus");
            command.SetParameterValue("@Status", info.SettleStatus.ToString());
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@SettleUser", info.CreateUserName);
            command.SetParameterValue("@SettleDate", DateTime.Now);
            command.ExecuteNonQuery();
            return info;
        }

        public GatherSettlementInfo LoadGatherSettlement(GatherSettlementInfo info)
        {
            GatherSettlementInfo settle = new GatherSettlementInfo();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleGatherBySysNo");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Settle.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Settle.SysNo",
                    DbType.Int32,
                    "@SettleSysNo",
                    QueryConditionOperatorType.Equal,
                    info.SysNo);
                command.CommandText = builder.BuildQuerySql();
            }

            settle = command.ExecuteEntity<GatherSettlementInfo>();
            return settle;
        }

        public List<GatherSettlementItemInfo> GetSettleGatherItemsInfoPage(GatherSettlementInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleGatherItemsPageBySysNo");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SysNo DESC"))
            {
                command.CommandText = builder.BuildQuerySql();
                command.CommandText = command.CommandText.Replace("@SettleSysNo", info.SysNo.Value.ToString());
                return command.ExecuteEntityList<GatherSettlementItemInfo>();
            }
        }

        #endregion IGatherSettlementDA Members

        #region [Private Methods]

        private void CreateGatherItem(GatherSettlementInfo entity, string solist)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertConsignSettleGatherItems");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (entity.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, entity.VendorInfo.SysNo.Value);
                }
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, entity.CompanyCode);

                command.CommandText = builder.BuildQuerySql();
                string strSQl1 = string.Empty, strSQl2 = string.Empty;
                // ##OrderDate1##   ##OrderDate2##
                if (!string.IsNullOrEmpty(entity.ReferenceSysNo))
                {
                    strSQl1 += " AND main.SONumber =" + entity.ReferenceSysNo.ToString() + "  ";
                    strSQl2 += " AND main.SysNo = " + entity.ReferenceSysNo.ToString() + "  ";
                }
                if (entity.CreateDateFrom.HasValue)
                {
                    strSQl1 += " AND main.OrderDate>'" + entity.CreateDateFrom.Value.ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime>'" + entity.CreateDateFrom.Value.ToShortDateString() + "'";
                }

                if (entity.CreateDateTo.HasValue)
                {
                    strSQl1 += " AND main.OrderDate<'" + entity.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime<'" + entity.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }

                if (entity.SourceStockInfo.SysNo.HasValue)
                {
                    strSQl1 += " AND main.WarehouseNumber=" + entity.SourceStockInfo.SysNo.Value.ToString() + " ";
                    strSQl2 += " AND register.LocationWarehouse=" + entity.SourceStockInfo.SysNo.Value.ToString() + " ";
                }
                else
                {
                    throw new BizException("未选择仓库，不能进行该操作！");
                }
                command.CommandText = command.CommandText.Replace("##OrderDate1##", strSQl1).Replace("##OrderDate2##", strSQl2);
            }
            command.CommandText = command.CommandText.Replace("#SONumber#", solist)
                                                    .Replace("#StoreCompanyCode#", "'" + entity.CompanyCode + "'")
                                                    .Replace("#CompanyCode#", "'" + entity.CompanyCode + "'")
                                                    .Replace("#InUser#", "'" + entity.CreateUserName + "'")
                                                    .Replace("#SettlementSysNo#", entity.SysNo.Value.ToString());
            command.ExecuteNonQuery();
        }

        private void CreateShippingItem(GatherSettlementInfo entity, string solist)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertConsignSettleGatherALLShippingCharge");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (entity.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, entity.VendorInfo.SysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, entity.CompanyCode);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                }
                command.CommandText = builder.BuildQuerySql();
                string strSQl1 = string.Empty, strSQl2 = string.Empty;
                // ##OrderDate1##   ##OrderDate2##
                if (entity.CreateDateFrom.HasValue)
                {
                    strSQl1 += " AND main.OrderDate>'" + entity.CreateDateFrom.Value.ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime>'" + entity.CreateDateFrom.Value.ToShortDateString() + "'";
                }
                if (entity.CreateDateTo.HasValue)
                {
                    strSQl1 += " AND main.OrderDate<'" + entity.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime<'" + entity.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }
                if (entity.SourceStockInfo.SysNo.HasValue)
                {
                    strSQl1 += " AND main.WarehouseNumber=" + entity.SourceStockInfo.SysNo.Value.ToString() + " ";
                    strSQl2 += " AND register.LocationWarehouse=" + entity.SourceStockInfo.SysNo.Value.ToString() + " ";
                }
                else
                {
                    throw new BizException("未选择仓库，不能进行该操作！");
                }
                command.CommandText = command.CommandText.Replace("##OrderDate1##", strSQl1).Replace("##OrderDate2##", strSQl2);
            }
            command.CommandText = command.CommandText.Replace("#SONumber#", solist)
                                                     .Replace("#StoreCompanyCode#", "'" + entity.CompanyCode + "'")
                                                     .Replace("#CompanyCode#", "'" + entity.CompanyCode + "'")
                                                     .Replace("#InUser#", "'" + entity.CreateUserName + "'")
                                                     .Replace("#SettlementSysNo#", entity.SysNo.Value.ToString());
            command.ExecuteNonQuery();
        }

        private void CreateROAdjustItem(GatherSettlementInfo entity, string solist)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertConsignSettleGatherRO_Adjust");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (entity.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, entity.VendorInfo.SysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, entity.CompanyCode);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                }
                command.CommandText = builder.BuildQuerySql();
                string strSQL3 = string.Empty;
                // ##OrderDate3##
                if (entity.CreateDateFrom.HasValue)
                {
                    strSQL3 += " AND main.CreateTime>'" + entity.CreateDateFrom.Value.ToShortDateString() + "'";
                }
                if (entity.CreateDateTo.HasValue)
                {
                    strSQL3 += " AND main.CreateTime<'" + entity.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }
                command.CommandText = command.CommandText.Replace("##OrderDate3##", strSQL3);
            }
            command.CommandText = command.CommandText.Replace("#SONumber#", solist)
                                                     .Replace("#StoreCompanyCode#", "'" + entity.CompanyCode + "'")
                                                     .Replace("#CompanyCode#", "'" + entity.CompanyCode + "'")
                                                     .Replace("#InUser#", "'" + entity.CreateUserName + "'")
                                                     .Replace("#SettlementSysNo#", entity.SysNo.Value.ToString());
            command.ExecuteNonQuery();
        }

        #endregion [Private Methods]

        public List<GatherSettlementItemInfo> QueryConsignSettlementProductList(GatherSettlementInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleGatherItemsNoPage");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (info.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, info.VendorInfo.SysNo);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, info.CompanyCode);

                command.CommandText = builder.BuildQuerySql();
                string strSQl1 = string.Empty, strSQl2 = string.Empty;
                // ##OrderDate1##   ##OrderDate2##
                if (!string.IsNullOrEmpty(info.ReferenceSysNo))
                {
                    strSQl1 += " AND main.SONumber =" + info.ReferenceSysNo.ToString() + "  ";
                    strSQl2 += " AND main.SysNo = " + info.ReferenceSysNo.ToString() + "  ";
                }
                if (info.CreateDateFrom.HasValue)
                {
                    strSQl1 += " AND main.OrderDate>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                }

                if (info.CreateDateTo.HasValue)
                {
                    strSQl1 += " AND main.OrderDate<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                    strSQl2 += " AND main.CreateTime<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }

                if (info.OutStockRefundDateFrom.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate>'" + info.OutStockRefundDateFrom.ToString() + "'";
                    strSQl2 += " AND main.RefundTime>'" + info.OutStockRefundDateFrom.ToString() + "'";
                }

                if (info.OutStockRefundDateTo.HasValue)
                {
                    strSQl1 += " AND main.InvoiceDate<'" + info.OutStockRefundDateTo.ToString() + "'";
                    strSQl2 += " AND main.RefundTime<'" + info.OutStockRefundDateTo.ToString() + "'";
                }

                if (info.SourceStockInfo.SysNo.HasValue)
                {
                    strSQl1 += " AND main.WarehouseNumber=" + info.SourceStockInfo.SysNo.Value.ToString() + " ";
                    strSQl2 += " AND register.LocationWarehouse=" + info.SourceStockInfo.SysNo.Value.ToString() + " ";
                }
                else
                {
                    throw new BizException("未选择仓库，不能进行该操作！");
                }
                command.CommandText = command.CommandText.Replace("##OrderDate1##", strSQl1).Replace("##OrderDate2##", strSQl2);
                return command.ExecuteEntityList<GatherSettlementItemInfo>();
            }
        }

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <returns></returns>
        public List<int> GetGatherSettlementSysNoListByVendorSysNo(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGatherSettlementSysNoListByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            return command.ExecuteFirstColumn<Int32>();
        }



        public List<GatherSettlementItemInfo> QueryConsignSettleGatherROAdjust(GatherSettlementInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleGatherRO_Adjust");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "b.SONumber DESC"))
            {
                if (info.VendorInfo.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ship.MerchantSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal, info.VendorInfo.SysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, info.CompanyCode);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                }
                command.CommandText = builder.BuildQuerySql();
                string strSQL3 = string.Empty;
                //##OrderDate3##
                if (!string.IsNullOrEmpty(info.ReferenceSysNo))
                {
                    strSQL3 += " AND main.SysNo = " + info.ReferenceSysNo + "  ";

                }
                if (info.CreateDateFrom.HasValue)
                {
                    strSQL3 += " AND main.CreateTime>'" + info.CreateDateFrom.Value.ToShortDateString() + "'";
                }
                if (info.CreateDateTo.HasValue)
                {
                    strSQL3 += " AND main.CreateTime<'" + info.CreateDateTo.Value.AddDays(1).ToShortDateString() + "'";
                }

                if (info.OutStockRefundDateFrom.HasValue)
                {

                    strSQL3 += " AND main.RefundTime>'" + info.OutStockRefundDateFrom.ToString() + "'";
                }

                if (info.OutStockRefundDateTo.HasValue)
                {

                    strSQL3 += " AND main.RefundTime<'" + info.OutStockRefundDateTo.ToString() + "'";
                }

                //补偿退款单与仓库无关，无需根据仓库筛选
                //if (entity.StockSysNo.HasValue)
                //{
                //    strSQL3 += " AND so.StockSysNo=" + entity.StockSysNo.Value.ToString() + " ";
                //}
                //else
                //{
                //    throw new BusinessException("未选择仓库，不能进行该操作！");
                //}
                command.CommandText = command.CommandText.Replace("##OrderDate3##", strSQL3);
            }
            return command.ExecuteEntityList<GatherSettlementItemInfo>();
        }

        public List<SettleItemInfo> GetSettleItemList(int settleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleItemList");
            command.SetParameterValue("@SettleSysNo", settleSysNo);
            return command.ExecuteEntityList<SettleItemInfo>();
        }
    }
}
