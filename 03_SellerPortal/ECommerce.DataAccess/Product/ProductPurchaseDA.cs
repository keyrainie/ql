using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Product
{
    public static class ProductPurchaseDA
    {
        public static List<ProductPurchaseQueryBasicInfo> QueryProductPurchase(ProductPurchaseQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            #region

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductPurchase");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.SortFields,
                StartRowIndex = queryFilter.PageIndex * queryFilter.PageSize,
                MaximumRows = queryFilter.PageSize
            };
            BuildSearchPOCondition(queryFilter, dataCommand, pagingInfo);

            if (!string.IsNullOrEmpty(queryFilter.StatusList))
            {
                dataCommand.CommandText = dataCommand.CommandText.Replace("@Status", " @Status OR PO.Status  IN (1,2,3,-2,5,6)");
            }
            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("status", typeof(PurchaseOrderStatus));
            enumList.Add("PaySettleCompany", typeof(PaySettleCompany));

            List<ProductPurchaseQueryBasicInfo> list = dataCommand.ExecuteEntityList<ProductPurchaseQueryBasicInfo>();
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            return list;

            #endregion
        }

        private static void BuildSearchPOCondition(ProductPurchaseQueryFilter queryFilter, CustomDataCommand dataCommand, PagingInfoEntity pagingInfo)
        {
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "po.sysno desc"))
            {
                #region build search condition
                string replaceSQL1 = "";
                string replaceSQL2 = "where 1=1";
                string replaceSQL3 = "where 1=1";

                if (queryFilter.CreateTimeBegin.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateTime",
                    DbType.DateTime, "@CreateTime", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeBegin.Value);
                }

                if (queryFilter.CreateTimeTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateTime",
                    DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeTo.Value.AddDays(1));
                }
                if (!string.IsNullOrEmpty(queryFilter.AuditUser))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(con_apport.DisplayName like '" + queryFilter.AuditUser.Trim() + "%' OR con_audit.DisplayName like '" + queryFilter.AuditUser.Trim() + "%') ");
                }
                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status);
                }

                if (!string.IsNullOrEmpty(queryFilter.StatusList))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 999);
                }

                if (queryFilter.IsConsign.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                    DbType.Int32, "@IsConsign", QueryConditionOperatorType.Equal, (int)queryFilter.IsConsign);
                }

                if (!string.IsNullOrEmpty(queryFilter.VendorSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.VendorSysNo",
                    DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                }

                #region 权限筛选

                //#region 添加供应商（创建PO单的对象）
                //int vendorCreateUserSysNo = int.Parse(AppSettingManager.GetSetting("PO", "VendorCreateUserSysNo"));
                //queryFilter.PMAuthorizedList.Add(vendorCreateUserSysNo);
                //#endregion 添加供应商（创建PO单的对象）

                //by Jack.W.Wang  2012-11-8 CRL21776--------------------------BEGIN
                //                if (!(queryFilter.IsManagerPM ?? false))
                //                {
                //                    string sqlStr = @"Select 
                //									ProductLineSysNo
                //                            FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p " +
                //                "WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + " OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";
                //                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "po.ProductLineSysNo", QueryConditionOperatorType.In, sqlStr);
                //                }

                var createUserSysNo = queryFilter.PMSysNo;

                if (!string.IsNullOrEmpty(createUserSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                         "po.PMSysNo in(" + createUserSysNo + ")");
                }

                #endregion

                if (string.IsNullOrEmpty(queryFilter.POSysNoExtention))
                {
                    if (queryFilter.POSysNo != "" && queryFilter.POSysNo != null && Regex.IsMatch(queryFilter.POSysNo, "^[0-9]+$"))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.poid",
                       DbType.String, "@poid", QueryConditionOperatorType.Like, queryFilter.POSysNo.Trim());
                    }
                }
                else
                {
                    string[] poSysNoStr = queryFilter.POSysNoExtention.Split('.');
                    List<string> poSysNoList = new List<string>();
                    foreach (string s in poSysNoStr)
                    {
                        poSysNoList.Add(s);
                    }

                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "po.poid",
                        DbType.String, poSysNoList);

                }

                if (!string.IsNullOrEmpty(queryFilter.IsApportion))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsApportion",
                    DbType.Int32, "@IsApportion", QueryConditionOperatorType.Equal, queryFilter.IsApportion);
                }

                if (!string.IsNullOrEmpty(queryFilter.CreatePOSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateUserSysNo",
                    DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, queryFilter.CreatePOSysNo);
                }

                if (queryFilter.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.PaySettleCompany",
                    DbType.Int32, "@PaySettleCompany", QueryConditionOperatorType.Equal, (int)queryFilter.PaySettleCompany);
                }

                if (!string.IsNullOrEmpty(queryFilter.CurrencySysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CurrencySysNo",
                    DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, queryFilter.CurrencySysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.IsStockStatus))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.PartlyReceiveStatus",
                    DbType.Int32, "@PartlyReceiveStatus", QueryConditionOperatorType.Equal, queryFilter.IsStockStatus);
                }

                if (queryFilter.InStockFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.InTime",
                    DbType.DateTime, "@InTime", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.InStockFrom.Value);
                }

                if (queryFilter.InStockTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.InTime",
                    DbType.DateTime, "@InStockTo", QueryConditionOperatorType.LessThan, queryFilter.InStockTo.Value.AddDays(1));
                }

                if (queryFilter.POType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.POType",
                    DbType.Int32, "@POType", QueryConditionOperatorType.Equal, (int)queryFilter.POType);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                if (queryFilter.LeaseFlag.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.LeaseFlag",
                        DbType.Int32, "@LeaseFlag", QueryConditionOperatorType.Equal, queryFilter.LeaseFlag.Value == true ? 1 : 0);
                }

                if (!string.IsNullOrEmpty(queryFilter.VerifyStatus))
                {
                    if (queryFilter.VerifyStatus == "1")
                    {
                        replaceSQL1 += " and po.TPStatus ='1'  ";
                        replaceSQL3 += " and po.TPStatus ='1'  ";
                    }
                    if (queryFilter.VerifyStatus == "2")
                    {
                        replaceSQL1 += " and po.TPStatus = '2' ";
                        replaceSQL3 += " and po.TPStatus = '2' ";
                    }
                }

                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo) &&
                        Regex.IsMatch(queryFilter.ProductSysNo, "^[0-9]+$"))
                {
                    replaceSQL1 += string.Format(" and  exists ( select top 1 sysno from ipp3.dbo.po_item po_item where po.sysno=po_item.posysno and productsysno = {0} )", queryFilter.ProductSysNo);
                    replaceSQL3 += string.Format(" and  exists ( select top 1 sysno from ipp3.dbo.po_item po_item where po.sysno=po_item.posysno and productsysno = {0} )", queryFilter.ProductSysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.StockSysNo))
                {
                    //添加在途商品查询
                    if (queryFilter.IsPurchaseQtySearch.HasValue && queryFilter.IsPurchaseQtySearch.Value)
                    {
                        //获取仓库数据
                        string[] stockArray = queryFilter.QueryStock.Split(',');
                        List<int> stockList = new List<int>(stockArray.Length);
                        for (int i = 0; i < stockArray.Length; i++)
                        {
                            stockList.Add(int.Parse(stockArray[i]));
                        }

                        sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                        sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "po.ITStockSysNo",
                            DbType.Int32, stockList);

                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.StockSysNo",
                            DbType.Int32, "@TransferStock", QueryConditionOperatorType.Equal, 50);
                        sqlBuilder.ConditionConstructor.EndGroupCondition();

                        sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.OR, "po.StockSysNo",
                        DbType.Int32, stockList);
                        sqlBuilder.ConditionConstructor.EndGroupCondition();

                    }
                    else
                    {
                        if (queryFilter.StockSysNo == "50")
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.StockSysNo",
                                DbType.Int32, "@TransferStock", QueryConditionOperatorType.Equal, 50);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ITStockSysNo",
                                DbType.Int32, "@TransferMiddleStock", QueryConditionOperatorType.Equal, queryFilter.TranferStock);
                        }
                        else
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.StockSysNo",
                            DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                        }
                    }
                }

                if (queryFilter.IsPurchaseQtySearch.HasValue && queryFilter.IsPurchaseQtySearch.Value && !string.IsNullOrEmpty(queryFilter.QueryStatus))
                {
                    string[] statusArray = queryFilter.QueryStatus.Split(',');
                    List<int> statusList = new List<int>(statusArray.Length);

                    for (int i = 0; i < statusArray.Length; i++)
                    {
                        statusList.Add(int.Parse(statusArray[i]));
                    }

                    sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "po.Status",
                        DbType.Int32, statusList);
                }
                #endregion

                if (queryFilter.PrintTime.HasValue)
                {
                    replaceSQL2 += string.Format(" and exists (select top 1 1 from scm.dbo.poLog with(nolock) GROUP BY Purno having scm.dbo.poLog.purno= po.sysno and convert(varchar(10),MAX(recdate),120) = convert(varchar(10),'{0}',120))", queryFilter.PrintTime.Value.ToString("yyyy-MM-dd"));
                    replaceSQL3 += string.Format(" and convert(varchar(10),a.MaxTastDate,120) = convert(varchar(10),'{0}',120)", queryFilter.PrintTime.Value.ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(queryFilter.AuditUser))
                {
                    replaceSQL2 += " and con_apport.DisplayName like '" + queryFilter.AuditUser.Trim() + "%' OR con_audit.DisplayName like '" + queryFilter.AuditUser.Trim() + "%'";
                    replaceSQL3 += " and con_apport.DisplayName like '" + queryFilter.AuditUser.Trim() + "%' OR con_audit.DisplayName like '" + queryFilter.AuditUser.Trim() + "%'";
                }

                if (queryFilter.BrandSysNo.HasValue)
                {
                    replaceSQL1 += string.Format(@" AND EXISTS(SELECT TOP 1 1 
                                                                 FROM IPP3.dbo.PO_Item POI
                                                                      INNER JOIN IPP3.dbo.Product PRO
  	                                                                  ON POI.ProductSysNo = PRO.SysNo
  	                                                                  INNER JOIN [OverseaContentManagement].dbo.Brand BRA
  	                                                                  ON BRA.SysNo = PRO.BrandSysNo
                                                                WHERE POI.POSysNo = PO.SysNO
                                                                  AND BRA.SysNo = {0}) ", queryFilter.BrandSysNo.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("replaceSQL1", replaceSQL1).Replace("replaceSQL2", replaceSQL2).Replace("replaceSQL3", replaceSQL3);
                if (pagingInfo != null)
                {
                    dataCommand.SetParameterValue("@StartNumber", queryFilter.PageSize * queryFilter.PageIndex);
                    dataCommand.SetParameterValue("@EndNumber", queryFilter.PageSize * queryFilter.PageIndex + queryFilter.PageSize);
                }
            }
        }


        public static PurchaseOrderItemInfo AddPurchaseOrderItemByProductSysNo(int productSysNo, int sellerSysNo)
        {
            PurchaseOrderItemInfo item = null;
            DataCommand command = DataCommandManager.GetDataCommand("AddPOItemByProductSysNo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@SellerSysNo", sellerSysNo);
            item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            return item;
        }
        public static PurchaseOrderItemInfo GetExtendPurchaseOrderItemInfo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExtendPurchaseOrderItemInfo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            PurchaseOrderItemInfo item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            if (!item.M1.HasValue)
            {
                item.M1 = 0;
            }
            return item;
        }

        public static bool IsVirtualStockPurchaseOrderProduct(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsVirtualStockPurchaseOrderProduct");
            command.SetParameterValue("@ItemSysNumber", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["VirtualQty"] != null && (int)dt.Rows[0]["VirtualQty"] != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static ProductInventoryInfo GetProductInventoryByProductSysNO(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductInventoryByProductSysNO");
            command.SetParameterValue("@ItemSysNumber", productSysNo);
            ProductInventoryInfo inventoryInfo = command.ExecuteEntity<ProductInventoryInfo>();
            return inventoryInfo;
        }

        public static decimal? GetLastPriceBySysNo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductLastPOInfo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["LastPrice"] != null)
            {
                return Convert.ToDecimal(dt.Rows[0]["LastPrice"]);
            }
            return null;
        }


        public static PurchaseOrderInfo LoadPOMaster(int poSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOMasterBySysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            return command.ExecuteEntity<PurchaseOrderInfo>();
        }

        public static PurchaseOrderInfo LoadPOMaster(int poSysNo, int sellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOMaster");
            command.SetParameterValue("@SysNo", poSysNo);
            command.SetParameterValue("@VendorSysNo", sellerSysNo);
            return command.ExecuteEntity<PurchaseOrderInfo>();
        }

        public static PurchaseOrderETATimeInfo LoadPOETATimeInfo(int poSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOETA");
            dataCommand.SetParameterValue("@POSysNo", poSysNo);
            PurchaseOrderETATimeInfo poETA = dataCommand.ExecuteEntity<PurchaseOrderETATimeInfo>();
            return poETA;
        }

        public static List<PurchaseOrderItemInfo> LoadPOItems(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOItemsByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);
            List<PurchaseOrderItemInfo> returnList = command.ExecuteEntityList<PurchaseOrderItemInfo>();
            returnList.ForEach(x =>
            {
                if (IsVirtualStockPurchaseOrderProduct(x.ProductSysNo.Value))
                {
                    x.IsVirtualStockProduct = true;
                }
            });
            return returnList;
        }

        public static PurchaseOrderLogInfo LoadPOLogInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOLog");
            command.SetParameterValue("@POSysNo", poSysNo);
            return command.ExecuteEntity<PurchaseOrderLogInfo>();
        }

        public static PurchaseOrderItemInfo LoadExtendPOItem(int itemSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetJDPriceAndM1AndAndAvailableQty");
            command.SetParameterValue("@ProductSysNo", itemSysNo);
            PurchaseOrderItemInfo item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            if (!item.M1.HasValue)
            {
                item.M1 = 0;
            }
            return item;
        }

        public static List<PurchaseOrderReceivedInfo> LoadPurchaseOrderReceivedInfo(int poSysNo)
        {
            List<PurchaseOrderReceivedInfo> result = new List<PurchaseOrderReceivedInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("GetPOReceivedInfoByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);
            result = command.ExecuteEntityList<PurchaseOrderReceivedInfo>();
            return result;
        }

        public static int? CreatePOSequenceSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOSequence");
            object result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public static PurchaseOrderInfo CreatePO(PurchaseOrderInfo poInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOMaster");
            command.SetParameterValue("@SysNo", poInfo.SysNo.Value);
            command.SetParameterValue("@POID", poInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
            command.SetParameterValue("@VendorSysNo", int.Parse(poInfo.VendorInfo.VendorID));
            command.SetParameterValue("@ITStockSysNo", DBNull.Value);
            command.SetParameterValue("@StockSysNo", poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo);
            command.SetParameterValue("@ShipTypeSysNo", 0);
            command.SetParameterValue("@PayTypeSysNo", 0);
            command.SetParameterValue("@CurrencySysNo", poInfo.PurchaseOrderBasicInfo.CurrencyCode);
            command.SetParameterValue("@ExchangeRate", poInfo.PurchaseOrderBasicInfo.ExchangeRate);
            command.SetParameterValue("@TotalAmt", poInfo.PurchaseOrderBasicInfo.TotalAmt);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@CreateUserSysNo", poInfo.InUserSysNo);
            command.SetParameterValue("@AuditTime", DBNull.Value);
            command.SetParameterValue("@AuditUserSysNo", DBNull.Value);
            command.SetParameterValue("@InTime", DBNull.Value);
            command.SetParameterValue("@InUserSysNo", DBNull.Value);
            command.SetParameterValue("@IsApportion", poInfo.PurchaseOrderBasicInfo.IsApportion);
            command.SetParameterValue("@ApportionTime", DBNull.Value);
            command.SetParameterValue("@ApportionUserSysNo", DBNull.Value);
            command.SetParameterValue("@Memo", poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
            command.SetParameterValue("@InStockMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);
            command.SetParameterValue("@Status", (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@ETP", poInfo.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@IsConsign", (int)poInfo.PurchaseOrderBasicInfo.ConsignFlag);
            command.SetParameterValue("@POType", (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderType);
            command.SetParameterValue("@TaxRate", poInfo.PurchaseOrderBasicInfo.TaxRate);
            command.SetParameterValue("@PurchaseStockSysno", DBNull.Value);
            command.SetParameterValue("@PMSysNo", 1);
            command.SetParameterValue("@ETATime", poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", poInfo.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@AutoSendMail", poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress == "-999" ? "" : poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress);
            command.SetParameterValue("@CompanyCode", poInfo.CompanyCode);
            command.ExecuteScalar();
            return poInfo;
        }

        public static PurchaseOrderItemInfo CreatePOItem(PurchaseOrderItemInfo itemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOItems");
            command.SetParameterValue("@POSysNo", itemInfo.POSysNo);
            command.SetParameterValue("@SysNo", itemInfo.ItemSysNo);
            command.SetParameterValue("@ProductSysNo", itemInfo.ProductSysNo);
            command.SetParameterValue("@BriefName", itemInfo.BriefName);
            command.SetParameterValue("@Quantity", itemInfo.Quantity);
            command.SetParameterValue("@PrePurchaseQty", itemInfo.PrePurchaseQty);
            command.SetParameterValue("@Weight", itemInfo.Weight);
            command.SetParameterValue("@OrderPrice", itemInfo.OrderPrice);
            command.SetParameterValue("@ApportionAddOn", itemInfo.ApportionAddOn);
            command.SetParameterValue("@UnitCost", itemInfo.UnitCost);
            command.SetParameterValue("@ReturnCost", 0);
            command.SetParameterValue("@lastOrderPrice", itemInfo.LastOrderPrice);
            command.SetParameterValue("@ExecptStatus", itemInfo.ExecptStatus);
            command.SetParameterValue("@ProductID", itemInfo.ProductID);
            command.SetParameterValue("@UnitCostWithoutTax", itemInfo.UnitCostWithoutTax);
            command.SetParameterValue("@AvailableQty", itemInfo.AvailableQty);
            command.SetParameterValue("@m1", itemInfo.M1);
            command.SetParameterValue("@PurchaseQty", itemInfo.PurchaseQty);
            command.SetParameterValue("@CurrencySysNo", itemInfo.CurrencyCode);
            command.SetParameterValue("@CurrentPrice", itemInfo.CurrentPrice);
            command.SetParameterValue("@CurrentUnitCost", itemInfo.CurrentUnitCost);
            command.SetParameterValue("@BatchInfo", itemInfo.BatchInfo);
            command.SetParameterValue("@CompanyCode", itemInfo.CompanyCode);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                itemInfo.ItemSysNo = Convert.ToInt32(o);
            }
            return itemInfo;
        }

        public static void UpdatePOStatus(PurchaseOrderInfo localEntity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOStatus");
            command.SetParameterValue("@SysNo", localEntity.SysNo.Value);
            command.SetParameterValue("@Status", localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@TPStatus", localEntity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);
            command.SetParameterValue("@PMRequestMemo", localEntity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@RefuseMemo", localEntity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo);
            command.SetParameterValue("@AuditDate", localEntity.PurchaseOrderBasicInfo.AuditDate);
            command.SetParameterValue("@AuditUserSysNo", localEntity.PurchaseOrderBasicInfo.AuditUserSysNo);
            command.SetParameterValue("@AuditUserName", localEntity.PurchaseOrderBasicInfo.AuditUserName);
            command.ExecuteNonQuery();
        }

        public static void CreatePOETAInfo(PurchaseOrderETATimeInfo poetaEntity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOETAInfo");
            command.SetParameterValue("@POSysNo", poetaEntity.POSysNo);
            command.SetParameterValue("@ETATime", poetaEntity.ETATime);
            command.SetParameterValue("@HalfDay", poetaEntity.HalfDay.ToString());
            command.SetParameterValue("@Status", poetaEntity.Status);
            command.SetParameterValue("@InUser", poetaEntity.InUser);
            command.ExecuteNonQuery();
        }

        public static void UpdatePOETAInfo(PurchaseOrderETATimeInfo poetaEntity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOETAInfo");
            command.SetParameterValue("@POSysNo", poetaEntity.POSysNo);
            command.SetParameterValue("@Status", poetaEntity.Status);
            command.SetParameterValue("@EditUser", poetaEntity.EditUser);
            command.ExecuteNonQuery();
        }

        public static PurchaseOrderItemInfo UpdatePOItem(PurchaseOrderItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItem");

            command.SetParameterValue("@SysNo", entity.ItemSysNo.Value);
            command.SetParameterValue("@POSysNo", entity.POSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@BriefName", entity.BriefName);

            command.SetParameterValue("@Quantity", entity.Quantity);
            command.SetParameterValue("@Weight", entity.Weight);
            command.SetParameterValue("@OrderPrice", entity.OrderPrice);

            command.SetParameterValue("@ApportionAddOn", entity.ApportionAddOn);
            command.SetParameterValue("@UnitCost", entity.UnitCost);
            command.SetParameterValue("@ReturnCost", entity.ReturnCost);

            command.SetParameterValue("@PurchaseQty", entity.PurchaseQty.Value);
            command.SetParameterValue("@CurrencySysNo", entity.CurrencyCode);
            command.SetParameterValue("@ExecptStatus", entity.ExecptStatus);
            command.SetParameterValue("@UnitCostWithoutTax", entity.UnitCostWithoutTax);

            command.SetParameterValue("@AvailableQty", entity.AvailableQty);
            command.SetParameterValue("@m1", entity.M1);

            command.SetParameterValue("@CurrentUnitCost", entity.UnitCost);
            command.SetParameterValue("@CurrentPrice", entity.OrderPrice);
            command.SetParameterValue("@LastAdjustPriceDate", entity.LastAdjustPriceDate);

            command.SetParameterValue("@LastInTime", entity.LastInTime);
            command.SetParameterValue("@lastOrderPrice", entity.LastOrderPrice);

            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return entity;
        }

        public static void UpdatePOItemPurchaseQty(int poItemSysNo, int purchaseQty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItemPurchaseQty");
            command.SetParameterValue("@SysNo", poItemSysNo);
            command.SetParameterValue("@PurchaseQty", purchaseQty);
            command.ExecuteNonQuery();
        }

        public static int WaitingInStockPO(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItemStatusVerifyInStock");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@AuditTime", DateTime.Now);
            command.SetParameterValue("@AuditUserSysNo", 0);
            command.SetParameterValue("@PMRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@ETP", entity.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@ETATime", entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", entity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@TPStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);
            command.SetParameterValue("@RefuseMemo", entity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo);

            command.SetParameterValue("@ApportionUserSysNo", entity.PurchaseOrderBasicInfo.ApportionUserSysNo);
            command.SetParameterValue("@ApportionTime", entity.PurchaseOrderBasicInfo.ApportionTime);
            return command.ExecuteNonQuery();
        }

        public static List<PurchaseOrderSSBLogInfo> LoadPOSSBLog(int poSysNo, PurchaseOrderSSBMsgType msgType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOSSBLog");
            command.SetParameterValue("@POSysNo", poSysNo);
            command.SetParameterValue("@ActionType", msgType.ToString());

            return command.ExecuteEntityList<PurchaseOrderSSBLogInfo>();
        }

        public static string GetItemAccessoriesStringByPurchaseOrder(List<int?> productSysNoList, string companyCode)
        {
            if (productSysNoList.Count == 0)
            {
                return string.Empty;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductAccessoriesByProductSysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, new PagingInfoEntity()
            {
                SortField = null,
                StartRowIndex = 0,
                MaximumRows = int.MaxValue
            }, " Value"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product_Accessories.Status",
                 DbType.String, "@Status", QueryConditionOperatorType.Equal, 0);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product_Accessories.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);

                string condition = " AND (";
                foreach (var sysNo in productSysNoList)
                {
                    condition += "Product_Accessories.ProductSysno = " + sysNo.ToString() + " OR ";
                }
                condition = condition.Substring(0, condition.LastIndexOf("OR"));
                condition += ")";
                var sysNoList = productSysNoList.ConvertAll<string>(p => p.ToString()).ToArray();
                string CustomCondition = "atc.ProductSysNo IN (" + string.Join(",", sysNoList) + ")";
                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("OtherCondition", condition).Replace("#CustomCondition#", CustomCondition);
            }

            DataTable dt = dataCommand.ExecuteDataSet().Tables[0];
            string des = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                des += "【" + dr["ProductID"].ToString() + "】 " + dr["Description"].ToString();
            }

            return des;
        }


        public static bool IsBatchProduct(PurchaseOrderItemInfo poItem)
        {
            PurchaseOrderItemInfo poItemEntity = new PurchaseOrderItemInfo();
            DataCommand cmd = DataCommandManager.GetDataCommand("IsBatchProductInfo");
            cmd.SetParameterValue("@PreviousProductSysNo", poItem.ProductSysNo);
            poItemEntity = cmd.ExecuteEntity<PurchaseOrderItemInfo>();
            if (poItemEntity != null && poItemEntity.ProductSysNo.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新库存调整单库存（可用库存）
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <param name="adjustQty"></param>
        /// <returns></returns>
        public static int UpdateStockInfoForAdjust(int stockSysNo, int productSysNo, int adjustQty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateStockInfoForAdjust");
            command.SetParameterValue("@Qty", adjustQty);
            command.SetParameterValue("@StockSysNo", stockSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            return command.ExecuteNonQuery();
        }
    }
}
