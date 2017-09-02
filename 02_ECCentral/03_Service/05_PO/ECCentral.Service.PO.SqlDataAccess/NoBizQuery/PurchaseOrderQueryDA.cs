using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.PO;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPurchaseOrderQueryDA))]
    public class PurchaseOrderQueryDA : IPurchaseOrderQueryDA
    {
        #region IPurchaseOrderQueryDA Members

        public System.Data.DataTable QueryPurchaseOrderList(QueryFilter.PO.PurchaseOrderQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            #region

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPOList");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            BuildSearchPOCondition(queryFilter, dataCommand, pagingInfo);

            if (!string.IsNullOrEmpty(queryFilter.StatusList))
            {
                dataCommand.CommandText = dataCommand.CommandText.Replace("@Status", " @Status OR PO.Status  IN (1,2,3,-2,5,6)");              
            }
            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("status",typeof(PurchaseOrderStatus));
            enumList.Add("PaySettleCompany", typeof(PaySettleCompany));

            dt = dataCommand.ExecuteDataTable(enumList);
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            return dt;

            #endregion
        }

        public DataTable QueryPurchaseOrderHistory(PurchaseOrderQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            CustomDataCommand command = command = DataCommandManager.CreateCustomDataCommandFromConfig("POhistory");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "intime desc"))
            {
                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po_item.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                }

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "po_master.Status", QueryConditionOperatorType.In,
                            "4,6,7,8");

                if (!string.IsNullOrEmpty(queryFilter.StockSysNo) && queryFilter.StockSysNo != "-999999")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po_master.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po_master.CompanyCode",
                 DbType.Int32, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                command.CommandText = sqlBuilder.BuildQuerySql();
                dt = command.ExecuteDataTable();
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }
            return dt;
        }


        #endregion

        private void BuildSearchPOCondition(PurchaseOrderQueryFilter queryFilter, CustomDataCommand dataCommand, PagingInfoEntity pagingInfo)
        {
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "po.sysno desc"))
            {
                #region build search condition
                string replaceSQL1 = "";
                string replaceSQL2 = "where 1=1";
                string replaceSQL3 = "where 1=1";

                if (queryFilter.ETATimeFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ETATime",
                    DbType.DateTime, "@ETATimeFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.ETATimeFrom);
                }

                if (queryFilter.ETATimeTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ETATime",
                    DbType.DateTime, "@ETATimeTo", QueryConditionOperatorType.LessThan, queryFilter.ETATimeTo);
                }

                if (queryFilter.FromTotalAmount.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.TotalAmt",
                    DbType.Decimal, "@FromTotalAmount", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.FromTotalAmount.Value);
                }

                if (queryFilter.ToTotalAmount.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.TotalAmt",
                    DbType.Decimal, "@ToTotalAmount", QueryConditionOperatorType.LessThanOrEqual, queryFilter.ToTotalAmount.Value);
                }

                if (!string.IsNullOrEmpty(queryFilter.LogisticsNumber))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.LogisticsNumber",
                    DbType.String, "@LogisticsNumber", QueryConditionOperatorType.Equal, queryFilter.LogisticsNumber);
                }


                if (!string.IsNullOrEmpty(queryFilter.ExpressName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ExpressName",
                    DbType.String, "@ExpressName", QueryConditionOperatorType.Like, queryFilter.ExpressName);
                }


                if (queryFilter.CreateTimeBegin.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateTime",
                    DbType.DateTime, "@CreateTime", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeBegin);
                }

                if (queryFilter.CreateTimeTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateTime",
                    DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeTo);
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

                #region 添加供应商（创建PO单的对象）
                int vendorCreateUserSysNo = int.Parse(AppSettingManager.GetSetting("PO", "VendorCreateUserSysNo"));
                queryFilter.PMAuthorizedList.Add(vendorCreateUserSysNo);
                #endregion 添加供应商（创建PO单的对象）

                //by Jack.W.Wang  2012-11-8 CRL21776--------------------------BEGIN
                if (!(queryFilter.IsManagerPM ?? false))
                {
                    string sqlStr = @"Select 
									ProductLineSysNo
                            FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p " +
                "WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + " OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "po.ProductLineSysNo", QueryConditionOperatorType.In, sqlStr);
                }
                var createUserSysNo = queryFilter.PMSysNo;

                if (!string.IsNullOrEmpty(createUserSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                         "po.PMSysNo in(" + createUserSysNo + ")");
                }
                //{
                //    var createIsValid = queryFilter.PMAuthorizedList.Exists(
                //        x => x == Convert.ToInt32(createUserSysNo));
                //    if (createIsValid)
                //    {
                //        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                //        "po.PMSysNo in(" + createUserSysNo + ")");
                //    }
                //    else
                //    {
                //        sqlBuilder.ConditionConstructor.AddInCondition(
                //            QueryConditionRelationType.AND,
                //            "po.PMSysNo",
                //            DbType.Int32,
                //            new List<int> { -999 });
                //    }
                //}
                //else
                //{
                //    sqlBuilder.ConditionConstructor.AddInCondition(
                //        QueryConditionRelationType.AND,
                //        "po.PMSysNo",
                //        DbType.Int32,
                //         queryFilter.PMAuthorizedList);
                //}

                //by Jack.W.Wang  2012-11-8 CRL21776--------------------------END
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

                if (queryFilter.VerifyStatus.HasValue)
                {
                    if (queryFilter.VerifyStatus == PurchaseOrderVerifyStatus.WaitingTLAudit)
                    {
                        replaceSQL1 += " and po.TPStatus ='1'  ";
                        replaceSQL3 += " and po.TPStatus ='1'  ";
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                        DbType.Int32, "@WaitingAudit", QueryConditionOperatorType.Equal, (int)PurchaseOrderStatus.WaitingAudit);
                    }
                    if (queryFilter.VerifyStatus == PurchaseOrderVerifyStatus.WaitingPMDAudit )
                    {
                        replaceSQL1 += " and po.TPStatus = '2' ";
                        replaceSQL3 += " and po.TPStatus = '2' ";
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                        DbType.Int32, "@WaitingAudit", QueryConditionOperatorType.Equal, (int)PurchaseOrderStatus.WaitingAudit);
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

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("replaceSQL1", replaceSQL1).Replace("replaceSQL2", replaceSQL2).Replace("replaceSQL3", replaceSQL3); 
                if (pagingInfo != null)
                {
                    dataCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                    dataCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                }
            }
        }

        public DataTable QueryRMAList(PurchaseOrderQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetRMA");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "VendorName,OutTime,PMUserSysNo"))
            {
                string sql = "WHERE 1=1 ";
                if (queryFilter.PMList != null && queryFilter.PMList.Count > 0)
                {
                    sql += "AND Product.PMUserSysNo IN (";
                    foreach (var pm in queryFilter.PMList)
                    {
                        sql += pm.ToString() + " , ";
                    }
                    sql = sql.Substring(0, sql.LastIndexOf(",") - 1) + ")";
                }

                sql += " AND RMA_Register.ResponseDesc IS NOT NULL AND DateAdd(Day,15,RMA_OutBound.OutTime)< '" + DateTime.Now + "'";
                sql += " AND RMA_Register.Status=1 AND RMA_Register.OutBoundStatus = 1 ";
                sql += " AND RMA_OutBound.CompanyCode= '" + queryFilter.CompanyCode + "'";

                dataCommand.CommandText = builder.BuildQuerySql().Replace("StrWhere", sql);

                dt = dataCommand.ExecuteDataTable();
                totalCount = (int)dataCommand.GetParameterValue("@TotalCount");

            }
            return dt;
        }

        public DataTable CountPurchaseOrder(PurchaseOrderQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOCount");
            BuildCountPOCondition(queryFilter, dataCommand, new PagingInfoEntity());
            var data = dataCommand.ExecuteDataTable();
            #region 前台的显示控制
            for (int i = -2; i <= 10; i++)
            {
                var rows = data.Select("status=" + i);
                //待分摊 Status=2 已去掉
                if (rows.Length == 0 && i != 2)
                {
                    data.Rows.Add(new string[4] { i.ToString(), "0", "0", "0" });
                }
            }
            #endregion
            return data;
        }

        private void BuildCountPOCondition(PurchaseOrderQueryFilter queryFilter, CustomDataCommand dataCommand, PagingInfoEntity pagingInfoEntity)
        {
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfoEntity, "po.sysno desc"))
            {
                #region build search condition
                string sql = "";

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
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
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
                #region 添加商家（创建PO单的对象）
                int vendorCreateUserSysNo = int.Parse(AppSettingManager.GetSetting("PO", "VendorCreateUserSysNo"));
                queryFilter.PMAuthorizedList.Add(vendorCreateUserSysNo);
                #endregion 添加商家（创建PO单的对象）
                var createUserSysNo = queryFilter.PMSysNo;
                if (!string.IsNullOrEmpty(createUserSysNo))
                {
                    var createIsValid = queryFilter.PMAuthorizedList.Exists(
                        x => x == Convert.ToInt32(createUserSysNo));
                    if (createIsValid)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "po.PMSysNo in(" + createUserSysNo + ")");
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddInCondition(
                            QueryConditionRelationType.AND,
                            "po.PMSysNo",
                            DbType.Int32,
                            new List<int> { -999 });
                    }
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(
                        QueryConditionRelationType.AND,
                        "po.PMSysNo",
                        DbType.Int32,
                         queryFilter.PMAuthorizedList);
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

                if (queryFilter.IsApportion != "")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsApportion",
                    DbType.Int32, "@IsApportion", QueryConditionOperatorType.Equal, queryFilter.IsApportion);
                }

                if (queryFilter.CreatePOSysNo != "")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CreateUserSysNo",
                    DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, queryFilter.CreatePOSysNo);
                }

                if (queryFilter.CompanySysNo != "")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.SettlementCompany",
                    DbType.Int32, "@SettlementCompany", QueryConditionOperatorType.Equal, queryFilter.CompanySysNo);
                }

                if (queryFilter.CurrencySysNo != "")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CurrencySysNo",
                    DbType.Int32, "@CurrencySysNo", QueryConditionOperatorType.Equal, queryFilter.CurrencySysNo);
                }

                if (queryFilter.IsStockStatus != "")
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

                if (queryFilter.PrintTime.HasValue)
                {
                    sql += string.Format(" and convert(varchar(10),a.MaxTastDate,120) = convert(varchar(10),'{0}',120)", (queryFilter.PrintTime.Value).ToString("yyyy-MM-dd"));
                }

                if (queryFilter.POType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.POType",
                    DbType.Int32, "@POType", QueryConditionOperatorType.Equal, (int)queryFilter.POType);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CompanyCode",
                    DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                if (queryFilter.VerifyStatus.HasValue)
                {
                    if (queryFilter.VerifyStatus == PurchaseOrderVerifyStatus.WaitingTLAudit)
                        sql += " and po.TPStatus ='1'  ";
                    if (queryFilter.VerifyStatus == PurchaseOrderVerifyStatus.WaitingPMDAudit)
                        sql += " and po.TPStatus = '2' ";
                }

                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo) &&
                    Regex.IsMatch(queryFilter.ProductSysNo, "^[0-9]+$"))
                {
                    sql += string.Format(" and  exists ( select top 1 sysno from ipp3.dbo.po_item po_item where po.sysno=po_item.posysno and productsysno = {0} )", queryFilter.ProductSysNo);
                }

                if (queryFilter.StockSysNo != "" || queryFilter.QueryStock != "")
                {
                    //添加在途商品查询
                    if (queryFilter.IsPurchaseQtySearch == true)
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

                if (queryFilter.IsPurchaseQtySearch == true && queryFilter.QueryStatus != string.Empty)
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

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("ReplaceSql", sql);

                if (pagingInfoEntity != null)
                {
                    dataCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                    dataCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                }
            }
        }

        public DataTable QueryPurchaseOrderLastPrice(int itemSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryProductLastPOPrice");

            command.SetParameterValue("@ProductSysNo", itemSysNo);
            DataTable dt = command.ExecuteDataTable();
            return dt;
        }

        public DataTable QueryPurchaseOrderBatchNumberList(PurchaseOrderBatchNumberQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("SearchPOBatchNumberList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "batch.ExpDate desc"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "batch.ProductSysNo",
                    DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "batch.StockSysNo",
                    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                if (pagingInfo != null)
                {
                    dataCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                    dataCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                }
            } 
            dt = dataCommand.ExecuteDataTable("Status", typeof(PurchaseOrderBatchInfoStatus));
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            return dt;
        }

        public DataTable QuertPurchaseOrderEIMSInvoiceInfo(int vendorSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSInvoiceInfoByPMAndVendor");
            dataCommand.AddInputParameter("@VendorNumber", DbType.Int32, vendorSysNo);
            //dataCommand.AddInputParameter("@PM", DbType.String, pmSysNo);

            DataTable dt = new DataTable();
            dt = dataCommand.ExecuteDataTable();
            return dt;
        }

        public DataTable GetPurchaseOrderAccessories(int poSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetProductAccessoriesByPOSysno");
            dataCommand.SetParameterValue("@POSysNo", poSysNo);
            return dataCommand.ExecuteDataTable();
        }

        public DataTable GetNeedSendMailPOForAutoCloseJob(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNeedSendEmailPO");
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            return cmd.ExecuteDataTable();
        }
    }
}
