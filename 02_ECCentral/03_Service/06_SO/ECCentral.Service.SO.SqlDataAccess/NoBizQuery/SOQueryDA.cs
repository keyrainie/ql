using System;
using System.Data;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.SO.SqlDataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.SO.IDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISOQueryDA))]
    public class SOQueryDA : ISOQueryDA
    {
        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = SortFieldMapping(pagingInfo.SortBy),
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (tsort[0].ToUpper())
                {
                    case "SOSYSNO":
                        tsort[0] = "sm.SysNo";
                        break;
                    case "CUSTOMERSYSNO":
                        tsort[0] = "sm.CustomerSysNo";
                        break;
                    case "SOTYPE":
                        tsort[0] = "SO_CheckShipping.SOType";
                        break;
                    case "SOSTATUS":
                        tsort[0] = "sm.[Status]";
                        if (tsort.Length > 1)
                        {
                            tsort[1] += ",SO_CheckShipping.IsCombine DESC,SO_CheckShipping.IsMergeComplete ASC, sm.HaveAutoRMA DESC";
                        }
                        break;
                    case "SOAMOUNT":
                        tsort[0] = "CashPay+PayPrice+sm.ShipPrice+PremiumAmt+sm.DiscountAmt";
                        break;
                    case "ORDERTIME":
                        tsort[0] = "sm.OrderDate";
                        break;
                    case "AUDITTIME":
                        tsort[0] = "sm.AuditTime";
                        break;
                    case "OrderDate":
                        tsort[0] = "SO.Orderdate";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        public DataTable Query(SORequestQueryFilter filter, out int dataCount)
        {
            dataCount = 0;
            bool includeHistory = filter.IncludeHistory.HasValue && filter.IncludeHistory.Value;
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig(includeHistory ? "SO_Query_GetSOAll" : "SO_Query_GetSO");
            command.CommandTimeout = 600;

            using (DynamicQuerySqlBuilder queryBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, ToPagingInfo(filter.PageInfo), "sm.OrderDate DESC"))
            {
                //编号格式验证
                if (filter.SOSysNo != null && Regex.IsMatch(filter.SOSysNo, @"^[,\. ]*\d+[\d,\. ]*$"))
                {
                    filter.SOSysNo = String.Join(",", filter.SOSysNo.Split(new char[] { '.', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    filter.SOSysNo = null;
                }
                if (!string.IsNullOrEmpty(filter.SOSysNo))
                {
                    string querySOList = filter.SOSysNo;
                    string subQuerySQLSOID = null;
                    if (includeHistory)
                    {
                        subQuerySQLSOID =
@"SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster IN ({0})
UNION ALL
SELECT sysno FROM OverseaOrderManagement.dbo.V_OM_SO_Master with(nolock) WHERE sysno IN ({0})
UNION ALL
SELECT sosplitmaster FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
UNION ALL
SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster in
 (SELECT sosplitmaster FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0}))";

                    }
                    else
                    {
                        subQuerySQLSOID =
@"SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping  with(nolock) WHERE sosplitmaster IN ({0}) or sosysno IN ({0})
UNION ALL
SELECT sosplitmaster FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
UNION ALL
SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster in
 (SELECT sosplitmaster FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0}))";
                    }

                    subQuerySQLSOID = string.Format(subQuerySQLSOID, querySOList);
                    queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "sm.sysno",
                        QueryConditionOperatorType.In,
                        subQuerySQLSOID);
                }
                else
                {
                    if (includeHistory)
                    {
                        #region 特殊条件构造-使用视图查询包括历史的记录
                        if (!string.IsNullOrEmpty(filter.ProductID))
                        {
                            string subQuerySQLPID = @"select vsi.SoSysNo from IPP3.dbo.v_so_item vsi WITH (NOLOCK)
                        LEFT join OverseaContentManagement.dbo.V_CM_ItemCommonInfo as product WITH (NOLOCK) on product.sysno = vsi.productsysno
                        where product.productid = @ProductID";

                            command.AddInputParameter("@ProductID", DbType.String);
                            command.SetParameterValue("@ProductID", filter.ProductID);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(
                                QueryConditionRelationType.AND,
                                "sm.sysno",
                                QueryConditionOperatorType.In,
                                subQuerySQLPID);
                        }

                        if (filter.Category3SysNo.HasValue)
                        {
                            string subQuerySQLC3 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.v_so_item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category3Sysno = " + filter.Category3SysNo;


                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                             "sm.sysno",
                                                             QueryConditionOperatorType.In,
                                                             subQuerySQLC3
                                                             );
                        }
                        else if (filter.Category2SysNo.HasValue)
                        {
                            string subQuerySQLC2 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.v_so_item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category2Sysno = " + filter.Category2SysNo;


                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                             "sm.sysno",
                                                             QueryConditionOperatorType.In,
                                                             subQuerySQLC2
                                                             );
                        }
                        else if (filter.Category1SysNo.HasValue)
                        {
                            string subQuerySQLC1 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.v_so_item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category1Sysno = " + filter.Category1SysNo;

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "sm.sysno",
                                                           QueryConditionOperatorType.In,
                                                           subQuerySQLC1
                                                           );
                        }

                        if (filter.StockSysNo.HasValue)
                        {
                            string subQuerySQLPID = @"select vsi.SoSysNo from IPP3.dbo.v_so_item vsi WITH (NOLOCK)
                        where vsi.WarehouseNumber = @StockSysNo";

                            command.AddInputParameter("@StockSysNo", DbType.AnsiStringFixedLength);
                            command.SetParameterValue("@StockSysNo", filter.StockSysNo);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(
                                QueryConditionRelationType.AND,
                                "sm.sysno",
                                QueryConditionOperatorType.In,
                                subQuerySQLPID);
                        }

                        if (filter.PMSysNo.HasValue)
                        {
                            string subQuerySQLPMSysNo = @"select vsi.SoSysNo from IPP3.dbo.v_so_item vsi WITH (NOLOCK)
                        LEFT join OverseaContentManagement.dbo.V_CM_ItemCommonInfo as product WITH (NOLOCK) on product.sysno = vsi.productsysno
                        where product.PMUserSysNo = @PMSysNo";

                            command.AddInputParameter("@PMSysNo", DbType.Int32, filter.PMSysNo);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                               "sm.sysno",
                                                               QueryConditionOperatorType.In,
                                                               subQuerySQLPMSysNo
                                                               );
                        }
                        #endregion
                    }
                    else
                    {
                        #region 特殊条件构造-只查询当前数据表
                        if (!string.IsNullOrEmpty(filter.ProductID))
                        {
                            string subQuerySQLPID = @"select vsi.SoSysNo from IPP3.dbo.SO_Item vsi WITH (NOLOCK)
                        LEFT join OverseaContentManagement.dbo.V_CM_ItemCommonInfo as product WITH (NOLOCK) on product.sysno = vsi.productsysno
                        where product.productid = @ProductID";

                            command.AddInputParameter("@ProductID", DbType.String);
                            command.SetParameterValue("@ProductID", filter.ProductID);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(
                                QueryConditionRelationType.AND,
                                "sm.sysno",
                                QueryConditionOperatorType.In,
                                subQuerySQLPID);
                        }
                        if (filter.Category1SysNo.HasValue)
                        {
                            string subQuerySQLC1 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.SO_Item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category1Sysno = " + filter.Category1SysNo;

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "sm.sysno",
                                                           QueryConditionOperatorType.In,
                                                           subQuerySQLC1
                                                           );
                        }
                        if (filter.Category2SysNo.HasValue)
                        {
                            string subQuerySQLC2 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.SO_Item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category2Sysno = " + filter.Category2SysNo;


                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                             "sm.sysno",
                                                             QueryConditionOperatorType.In,
                                                             subQuerySQLC2
                                                             );
                        }
                        if (filter.Category3SysNo.HasValue)
                        {
                            string subQuerySQLC3 = @"SELECT vsi.SOSysNo
                        FROM   IPP3.dbo.SO_Item as vsi (NOLOCK)
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemCommonInfo Pd with(nolock) on Pd.sysno=vsi.productsysno
                        LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category with(nolock) on category.Category3Sysno=Pd.Category3SysNo
                        WHERE category.Category3Sysno = " + filter.Category3SysNo;


                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                             "sm.sysno",
                                                             QueryConditionOperatorType.In,
                                                             subQuerySQLC3
                                                             );
                        }

                        if (filter.StockSysNo.HasValue)
                        {
                            string subQuerySQLPID = @"select vsi.SoSysNo from IPP3.dbo.SO_Item vsi WITH (NOLOCK)
                        where vsi.WarehouseNumber = @StockSysNo";

                            command.AddInputParameter("@StockSysNo", DbType.AnsiStringFixedLength);
                            command.SetParameterValue("@StockSysNo", filter.StockSysNo);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(
                                QueryConditionRelationType.AND,
                                "sm.sysno",
                                QueryConditionOperatorType.In,
                                subQuerySQLPID);
                        }

                        if (filter.PMSysNo.HasValue)
                        {
                            string subQuerySQLPMSysNo = @"select vsi.SoSysNo from IPP3.dbo.SO_Item vsi WITH (NOLOCK)
                        LEFT join OverseaContentManagement.dbo.V_CM_ItemCommonInfo as product WITH (NOLOCK) on product.sysno = vsi.productsysno
                        where product.PMUserSysNo = @PMSysNo";

                            command.AddInputParameter("@PMSysNo", DbType.Int32, filter.PMSysNo);

                            queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                               "sm.sysno",
                                                               QueryConditionOperatorType.In,
                                                               subQuerySQLPMSysNo
                                                               );
                        }
                        #endregion
                    }

                    #region 动态拼装条件

                    if (filter.IncomeStatus.HasValue && (int)filter.IncomeStatus.Value != int.MinValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.[Status]", DbType.Int32, "@SOIncomeStatus",
                            filter.IncomeStatus == ECCentral.BizEntity.Invoice.SOIncomeStatus.Abandon ? QueryConditionOperatorType.IsNull : QueryConditionOperatorType.Equal, filter.IncomeStatus);
                    }

                    if (filter.NetPayStatus.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vinp.[Status]", DbType.Int32, "@NetPayStatus",
                            filter.NetPayStatus == ECCentral.BizEntity.Invoice.NetPayStatus.Abandon ? QueryConditionOperatorType.IsNull : QueryConditionOperatorType.Equal, filter.NetPayStatus);
                    }
                    if (filter.SOStatus.HasValue)
                    {
                        switch (filter.SOStatus.Value)
                        {
                            case SOStatus.Abandon:
                                {
                                    queryBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND,
                                                            "sm.status",
                                                            DbType.Int32,
                                                            "@SOStatus", QueryConditionOperatorType.LessThanOrEqual,
                                                            QueryConditionOperatorType.MoreThanOrEqual, -1, -3);
                                    break;
                                }
                            case SOStatus.SystemCancel:
                                {
                                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                            "sm.status",
                                                            DbType.Int32,
                                                            "@SOStatus", 
                                                            QueryConditionOperatorType.Equal, -4);
                                    break;
                                }
                            case SOStatus.Reject:
                                {
                                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                            "sm.Status",
                                                            DbType.Int32,
                                                            "@SOStatus",
                                                            QueryConditionOperatorType.Equal,
                                                            SOStatus.Reject);
                                    break;
                                }
                            case SOStatus.Shipping:
                                {
                                    queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                            "sm.status", QueryConditionOperatorType.Equal, "4");
                                    queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                            "SO_CheckShipping.IsCombine", QueryConditionOperatorType.Equal, "1");
                                    queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                            "SO_CheckShipping.IsMergeComplete", QueryConditionOperatorType.Equal, "0");

                                    break;
                                }
                            default:
                                {
                                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                            "sm.status",
                                                            DbType.Int32,
                                                            "@SOStatus",
                                                            QueryConditionOperatorType.Equal,
                                                            filter.SOStatus);
                                    break;
                                }
                        }
                    }

                    if (filter.SOStatusArray != null && filter.SOStatusArray.Count > 0)
                    {
                        queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "sm.status",
                                                           QueryConditionOperatorType.In,
                                                           string.Join(",", filter.SOStatusArray.Select(p => (int)p)));
                    }

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                "sm.InvoiceNo",
                                                DbType.String,
                                                "@InvoiceNo",
                                                QueryConditionOperatorType.Equal,
                                                filter.InvoiceNo
                                                );

                    //礼品卡
                    if (filter.SOType == SOType.ElectronicCard)
                    {
                        queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                    "SO_CheckShipping.SOType",
                                                    DbType.Int32,
                                                    "@SOType",
                                                    QueryConditionOperatorType.Equal,
                                                    SOType.ElectronicCard
                                                    );
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                     "SO_CheckShipping.SOType",
                                                     DbType.Int32,
                                                     "@SOType",
                                                     QueryConditionOperatorType.Equal,
                                                     SOType.PhysicalCard
                                                     );
                        queryBuilder.ConditionConstructor.EndGroupCondition();
                    }
                    else if (filter.SOType == SOType.General)
                    {
                        queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                    "SO_CheckShipping.SOType",
                                                    DbType.Int32,
                                                    "@SOType",
                                                    QueryConditionOperatorType.IsNull,
                                                    SOType.General
                                                    );
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                     "SO_CheckShipping.SOType",
                                                     DbType.Int32,
                                                     "@SOType",
                                                     QueryConditionOperatorType.Equal,
                                                     SOType.General
                                                     );
                        queryBuilder.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                    "SO_CheckShipping.SOType",
                                                    DbType.String,
                                                    "@SOType",
                                                    QueryConditionOperatorType.Equal,
                                                    filter.SOType
                                                    );
                    }
                    /*//Siege 以旧换新 
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                             "OCN.Status",
                                             DbType.String,
                                             "@OcnStatus",
                                             QueryConditionOperatorType.Equal,
                                             filter.OcnStatus
                                             );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                              "OCN.IsSubmit",
                                              DbType.String,
                                              "@IsSubmit",
                                              QueryConditionOperatorType.Equal,
                                              filter.IsSubmit
                                              );
                    //*/


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "sm.IsVAT",
                                            DbType.Int32,
                                            "@IsVAT",
                                            QueryConditionOperatorType.Equal,
                                            filter.IsVAT
                                            );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "SO_CheckShipping.IsVATPrinted",
                                            DbType.Int32,
                                            "@IsVATPrinted",
                                            QueryConditionOperatorType.Equal,
                                            filter.VATIsPrinted
                                            );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "sm.receiveaddress",
                                            DbType.String,
                                            "@ReceiveAddress",
                                            QueryConditionOperatorType.LeftLike,
                                            filter.ReceiveAddress
                                            );

                    if (!string.IsNullOrEmpty(filter.ReceiveName))
                    {
                        queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                "sm.receivename",
                                                DbType.String,
                                                "@ReceiveName",
                                                QueryConditionOperatorType.LeftLike,
                                                filter.ReceiveName
                                                );
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                "sm.ReceiveContact",
                                                DbType.String,
                                                "@ReceiveContact",
                                                QueryConditionOperatorType.LeftLike,
                                                filter.ReceiveName
                                                );

                        queryBuilder.ConditionConstructor.EndGroupCondition();
                    }

                    if (!string.IsNullOrEmpty(filter.ReceivePhone))
                    {
                        queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                 "sm.receivephone",
                                 DbType.String,
                                 "@ReceivePhone",
                                 QueryConditionOperatorType.LeftLike,
                                 filter.ReceivePhone
                                 );
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                  "sm.ReceiveCellPhone",
                                  DbType.String,
                                  "@ReceiveCellPhone",
                                  QueryConditionOperatorType.LeftLike,
                                  filter.ReceivePhone
                                  );
                        queryBuilder.ConditionConstructor.EndGroupCondition();
                    }

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "cm.FromLinkSource",
                                            DbType.String,
                                            "@FromLinkSource",
                                            QueryConditionOperatorType.Like,
                                            filter.FromLinkSource
                                            );

                    /*// Siege 
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "SO_CheckShipping.VPOStatus",
                                            DbType.String,
                                            "@VPOStatus",
                                            QueryConditionOperatorType.Equal,
                                            filter.VPOStatus
                                            );
                    ////*/
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "sm.orderdate",
                                            DbType.DateTime,
                                            "@StartDate",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                            filter.FromOrderTime
                                            );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "sm.orderdate",
                                            DbType.DateTime,
                                            "@EndDate",
                                            QueryConditionOperatorType.LessThan,
                                            filter.ToOrderTime
                                            );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "sm.paytypesysno",
                                            DbType.Int32,
                                            "@PayTypeSysNo",
                                            QueryConditionOperatorType.Equal,
                                            filter.PayTypeSysNo
                                            );

                    if (filter.IsVIPCustomer.HasValue)
                    {
                        queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        if (filter.IsVIPCustomer.Value)
                        {
                            queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                   "cm.VIPRank",
                                                   DbType.Int32,
                                                   "@VIPRank1",
                                                   QueryConditionOperatorType.Equal,
                                                   ECCentral.BizEntity.Customer.VIPRank.VIPAuto
                                                   );
                            queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                   "cm.VIPRank",
                                                   DbType.Int32,
                                                   "@VIPRank2",
                                                   QueryConditionOperatorType.Equal,
                                                   ECCentral.BizEntity.Customer.VIPRank.VIPManual
                                                   );
                        }
                        else
                        {
                            queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                   "cm.VIPRank",
                                                   DbType.Int32,
                                                   "@VIPRank3",
                                                   QueryConditionOperatorType.Equal,
                                                   ECCentral.BizEntity.Customer.VIPRank.NormalAuto
                                                   );
                            queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                   "cm.VIPRank",
                                                   DbType.Int32,
                                                   "@VIPRank4",
                                                   QueryConditionOperatorType.Equal,
                                                   ECCentral.BizEntity.Customer.VIPRank.NormalManual
                                                   );
                        }
                        queryBuilder.ConditionConstructor.EndGroupCondition();
                    }


                    if (filter.IsVIP.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "ISNULL(sm.IsVIP,0)",
                            DbType.Int32,
                            "@IsVIP",
                            QueryConditionOperatorType.Equal,
                            filter.IsVIP.Value
                            );
                    }

                    if (filter.IsPhoneOrder.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                "ISNULL(SO_CheckShipping.IsPhoneOrder,0)",
                                                DbType.Int32,
                                                "@IsPhoneOrder",
                                                QueryConditionOperatorType.Equal,
                                                filter.IsPhoneOrder.Value);
                    }
                    if (filter.IsBackOrder.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                "ISNULL(SO_CheckShipping.IsBackOrder,0)",
                                                DbType.Int32,
                                                "@IsBackOrder",
                                                QueryConditionOperatorType.Equal,
                                                filter.IsBackOrder.Value
                                                );

                    }

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "SO_CheckShipping.IsExpiateOrder",
                                            DbType.Int32,
                                            "@IsExpiateOrder",
                                            QueryConditionOperatorType.Equal,
                                            filter.IsExpiateOrder
                                            );


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                "SM.DeliveryDate",
                                                DbType.DateTime,
                                                "@DeliveryDate",
                                                QueryConditionOperatorType.Equal,
                                                filter.DeliveryDate
                                                );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "SM.DeliveryTimeRange",
                                                 DbType.Int32,
                                                 "@DeliveryTimeRange",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.DeliveryTimeRange
                                                 );
                    //if (filter.PromotionCodeSysNo.HasValue && filter.PromotionCodeSysNo.Value > 0)
                    if (filter.PromotionCodeSysNo.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                     "sm.PlatPromotionCodeSysNo",
                                     DbType.Int32,
                                     "@PromotionCodeSysNo",
                                     QueryConditionOperatorType.Equal,
                                     filter.PromotionCodeSysNo);
                    }
                    else if (filter.IsUsePromotion.HasValue)
                    {
                        queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "ISNULL(sm.PlatPromotionCodeSysNo,0)",
                                                 DbType.Int32,
                                                 "@PromotionCodeSysNo",
                                                 filter.IsUsePromotion.Value ? QueryConditionOperatorType.NotEqual : QueryConditionOperatorType.Equal,
                                                 0);
                    }


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "(CashPay+PayPrice+sm.ShipPrice+PremiumAmt+sm.DiscountAmt)",
                                                 DbType.Decimal,
                                                 "@FromTotalAmount",
                                                 QueryConditionOperatorType.MoreThanOrEqual,
                                                 filter.FromTotalAmount);
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "(CashPay+PayPrice+sm.ShipPrice+PremiumAmt+sm.DiscountAmt)",
                                                 DbType.Decimal,
                                                 "@ToTotalAmount",
                                                 QueryConditionOperatorType.LessThanOrEqual,
                                                 filter.ToTotalAmount);

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "sm.auditusersysno",
                                                 DbType.Int32,
                                                 "@AuditUserSysNo",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.AuditUserSysNo);


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "sm.audittime",
                                                 DbType.DateTime,
                                                 "@AuditStartTime",
                                                 QueryConditionOperatorType.MoreThanOrEqual,
                                                 filter.FromAuditTime);

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "sm.audittime",
                                                 DbType.DateTime,
                                                 "@LessThanOrEqual",
                                                 QueryConditionOperatorType.LessThan,
                                                 filter.ToAuditTime);

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "sm.OutUserSysNo",
                                                 DbType.Int32,
                                                 "@OutUserSysNo",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.OutStockUserSysNo
                                                 );
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "sm.ShipTypeSysNo",
                                                 DbType.Int32,
                                                 "@ShipTypeSysNo",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.ShipTypeSysNo
                                                 );


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "cm.SysNo",
                                                 DbType.Int32,
                                                 "@CustomerSysNo",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.CustomerSysNo
                                                 );

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "ISNULL(KFC.FraudType,0)",
                                                 DbType.Int32,
                                                 "@KFCStatus",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.KFCType
                                                 );


                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                 "SO_CheckShipping.CustomerIPAddress",
                                                 DbType.String,
                                                 "@CustomerIPAddress",
                                                 QueryConditionOperatorType.Equal,
                                                 filter.CustomerIPAddress);

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                 "SO_CheckShipping.IsFPSO",
                                 DbType.Int32,
                                 "@IsFPSO",
                                 filter.FPStatus.HasValue ? QueryConditionOperatorType.Equal : QueryConditionOperatorType.IsNull,
                                 filter.FPStatus);

                    if (filter.OutSubStockSysNo.HasValue)
                    {
                        if (filter.OutSubStockSysNo > 50)
                        {
                            queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                                "SO_CheckShipping.StockStatus =1 AND SO_CheckShipping.LocalWHSysNo = @LocalWHSysNo");

                            command.AddInputParameter("@LocalWHSysNo", DbType.AnsiStringFixedLength);
                            command.SetParameterValue("@LocalWHSysNo", filter.OutSubStockSysNo);
                        }
                        else if (filter.OutSubStockSysNo == 0)
                        {
                            queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                                "(SO_CheckShipping.StockStatus =0 or SO_CheckShipping.LocalWHSysNo is null)");
                        }
                        else if (filter.OutSubStockSysNo == 2)
                        {
                            queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                                "(SO_CheckShipping.StockStatus =2)");
                        }
                    }


                    if (filter.IsInputContractNumber == true)
                    {
                        queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                            "sis.ContractNumber <> ''");
                    }
                    else if (filter.IsInputContractNumber == false)
                    {
                        queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                            "(sis.ContractNumber = '' or sis.ContractNumber is null)");
                    }
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SO_CheckShipping.MerchantSysNo",
                        DbType.Int32,
                        "@MerchantSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.MerchantSysNo);

                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SO_CheckShipping.StockType",
                        DbType.AnsiStringFixedLength,
                        "@StockType",
                        QueryConditionOperatorType.Equal,
                        filter.StockType);
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SO_CheckShipping.ShippingType",
                        DbType.AnsiStringFixedLength,
                        "@ShippingType",
                        QueryConditionOperatorType.Equal,
                        filter.DeliveryType);
                    queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SO_CheckShipping.InvoiceType",
                        DbType.AnsiStringFixedLength,
                        "@InvoiceType",
                        QueryConditionOperatorType.Equal,
                        filter.InvoiceType);

                    #region Modify By Kilin 2011-11-1
                    ///查询第三方订单
                    if (!string.IsNullOrEmpty(filter.SynSOSysNo) || !string.IsNullOrEmpty(filter.SynSOType))
                    {
                        string subSql = "SELECT SOSysNo FROM OverseaOrderManagement.dbo.ThridPart_SOMapping WITH(NOLOCK) WHERE 1 = 1 ";
                        //按名称查
                        if (!string.IsNullOrEmpty(filter.SynSOSysNo))
                        {
                            StringBuilder sbSysNos = new StringBuilder();
                            var sysnSoSysNos = (new Regex(@"\d+")).Matches(filter.SynSOSysNo);

                            foreach (Match item in sysnSoSysNos)
                            {
                                sbSysNos.AppendFormat(",'{0}'", item.Value);
                            }
                            if (sbSysNos.Length > 0)
                            {
                                sbSysNos = sbSysNos.Remove(0, 1);
                                subSql = string.Format("{0} AND  OrderID IN ({1})", subSql, sbSysNos.ToString());
                            }
                        }
                        //按频道查询
                        if (!string.IsNullOrEmpty(filter.SynSOType))
                        {
                            subSql = subSql + " AND [TYPE] = @SynSOType ";
                            command.AddInputParameter("@SynSOType", DbType.String);
                            command.SetParameterValue("@SynSOType", filter.SynSOType);
                        }

                        queryBuilder.ConditionConstructor.AddSubQueryCondition(
                            QueryConditionRelationType.AND,
                            "sm.SysNo",
                            QueryConditionOperatorType.In,
                            subSql);
                    }
                    #endregion

                    //CRL19992 By Kilin 查询团购订单
                    if (filter.SOType == SOType.GroupBuy)
                    {
                        //如果不包含团购部分失败的订单，则过滤掉团购部分失败的订单
                        if (filter.IncludeFailedGroupBuyingProduct == true)
                        {
                            queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                                , "SO_CheckShipping.SettlementStatus"
                                , DbType.String
                                , "@GroupBuying_SettlementStatus"
                                , QueryConditionOperatorType.Equal
                                , "P");
                        }
                    }
                    //购物车编号条件
                    if (!string.IsNullOrEmpty(filter.ShoppingCartNo))
                    {
                        queryBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "shc.ShoppingCartSysNo",
                            DbType.Int32,
                            "@ShoppingCartSysNo",
                            QueryConditionOperatorType.Equal,
                            filter.ShoppingCartNo
                        );
                    }

                    //Add 2013-10-29 10:30:04 Chester
                    if (!string.IsNullOrEmpty(filter.MembershipCard))
                    {
                        queryBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "Customer.MembershipCard",
                            DbType.String,
                            "@MembershipCard",
                            QueryConditionOperatorType.Like,
                            filter.MembershipCard
                        );
                    }

                    if (!string.IsNullOrEmpty(filter.ProductName))
                    {
                        string subQuerySQLProductName = @"SELECT SOMaster.SysNo
                                                        FROM ipp3.dbo.product AS Product
                                                        INNER JOIN ipp3.dbo.SO_Item AS SOItem ON Product.SysNo =SOItem.ProductSysNo
                                                        INNER JOIN IPP3.dbo.SO_Master AS SOMaster ON SOItem.SOSysNo = SOMaster.SysNo
                                                        WHERE Product.ProductName like @ProductName
                                                        GROUP BY SOMaster.SysNo";

                        command.AddInputParameter("@ProductName", DbType.String, string.Format("%{0}%", filter.ProductName));

                        queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "sm.sysno",
                                                           QueryConditionOperatorType.In,
                                                           subQuerySQLProductName
                                                           );
                    }

                    if (filter.InputTime.HasValue)
                    {
                        string subQuerySQLInputTime = @"select SoSysNo from ipp3.dbo.Finance_netpay where  DateDiff(d,inputtime,@InputTime)=0";

                        command.AddInputParameter("@InputTime", DbType.DateTime, filter.InputTime.Value);

                        queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "sm.sysno",
                                                           QueryConditionOperatorType.In,
                                                           subQuerySQLInputTime
                                                           );
                    }

                    #endregion
                }

                #region 设置基本参数

                queryBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "sm.CompanyCode"
                    , DbType.AnsiStringFixedLength
                    , "@CompanyCode"
                    , QueryConditionOperatorType.Equal
                    , filter.CompanyCode);

                #endregion
                command.CommandText = queryBuilder.BuildQuerySql();
            }
            //command.SetParameterValue<SORequestQueryFilter>(filter);

            EnumColumnList enumColList = new EnumColumnList();
            enumColList.Add("SOType", typeof(SOType));
            enumColList.Add("Status", typeof(SOStatus));
            enumColList.Add("NetPayStatus", typeof(ECCentral.BizEntity.Invoice.NetPayStatus));
            enumColList.Add("SOIncomeStatus", typeof(ECCentral.BizEntity.Invoice.SOIncomeStatus));
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("SOIncomeStatusText", typeof (string));
                
                foreach (DataRow dr in dt.Rows)
                {
                    int ippStatus = dr["Status"] != null && dr["Status"] != DBNull.Value ? (int)dr["Status"] : int.MinValue;
                    if (ippStatus != int.MinValue)
                    {
                        int isAutoRMA = dr["HaveAutoRMA"] != null && dr["HaveAutoRMA"] != DBNull.Value ? (int)dr["HaveAutoRMA"] : 0;

                        int isCombine = dr["IsCombine"] != null && dr["IsCombine"] != DBNull.Value ? (int)dr["IsCombine"] : 0;
                        int isMergeComplete = dr["IsMergeComplete"] != null && dr["IsMergeComplete"] != DBNull.Value ? (int)dr["IsMergeComplete"] : 0;
                        SOStatus status = ECCentral.Service.SO.SqlDataAccess.SODA.Mapping_SOStatus_IPPToThis(ippStatus, isAutoRMA != 0, isCombine == 1, isMergeComplete == 1);
                        dr["Status"] = (int)status;
                    }

                    // Set column value for SOIncomeStatusText
                    SOIncomeStatus? soIncomeStatus = dr.IsNull("SOIncomeStatus") ? new SOIncomeStatus?() : (SOIncomeStatus) dr["SOIncomeStatus"];
                    NetPayStatus?   netPayStatus   = dr.IsNull("NetPayStatus")   ? new NetPayStatus?()   : (NetPayStatus) dr["NetPayStatus"];
                    string soIncomeStatusText = string.Empty;

                    if (soIncomeStatus == null || soIncomeStatus == BizEntity.Invoice.SOIncomeStatus.Abandon)
                        soIncomeStatusText = netPayStatus.HasValue && netPayStatus == BizEntity.Invoice.NetPayStatus.Origin
                            ? ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied
                            : ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay;
                    else
                        soIncomeStatusText = EnumHelper.GetDescription(soIncomeStatus.Value);
                    dr["SOIncomeStatusText"] = soIncomeStatusText;
                }
                command.ConvertEnumColumn(dt, enumColList);
            }
            object count = command.GetParameterValue("@TotalCount");
            dataCount = count == null || count == DBNull.Value ? 0 : (int)count;
            return dt;
        }

        public DataTable InvoiceChangeLogQuery(SOInvoiceChangeLogQueryFilter filter, out int dataCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetInvoiceChangeLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "CreateTime DESC"))
            {
                #region 赋值参数

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SONumber",
                DbType.Int32,
                "@SONumber",
                QueryConditionOperatorType.Equal,
                filter.SOSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "WarehouseNumber",
                    DbType.AnsiStringFixedLength,
                    "@WarehouseNumber",
                    QueryConditionOperatorType.Equal,
                    filter.StockSysNo);
                if (filter.ChangeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "ChangeType",
                        DbType.String,
                        "@ChangeType",
                        QueryConditionOperatorType.Like,
                        filter.ChangeType.ToDisplayText());
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CreateTime",
                    DbType.DateTime,
                    "@CreateTimeBegin",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.CreateTimeBegin);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CreateTime",
                    DbType.DateTime,
                    "@CreateTimeEnd",
                    QueryConditionOperatorType.LessThan,
                    filter.CreateTimeEnd);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SICL.CompanyCode",
                    DbType.String,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                var dt = cmd.ExecuteDataTable();

                dataCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable PendingListQuery(SOPendingQueryFilter filter, out int dataCount, bool isNeedChange)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_Pending");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(filter.PagingInfo),
                "a.SysNO DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
              QueryConditionRelationType.AND,
              "a.CreateTime",
              DbType.DateTime,
              "@CreateTimeFrom",
              QueryConditionOperatorType.MoreThan,
              filter.CreateTimeFrom
              );

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "a.CreateTime",
                  DbType.DateTime,
                  "@CreateTimeTo",
                  QueryConditionOperatorType.LessThanOrEqual,
                  filter.CreateTimeTo
                  );
                if (filter.SOSysNo.HasValue)
                {
                    string subQuerySQLSOID = @"SELECT sosysno FROM  @tb ";
                    subQuerySQLSOID = string.Format(subQuerySQLSOID, filter.SOSysNo);
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "a.SOSysNo",
                        QueryConditionOperatorType.In,
                        subQuerySQLSOID);
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "a.Status",
                    DbType.Int32,
                    "@Status",
                    QueryConditionOperatorType.Equal,
                    filter.Status
                    );

                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "a.WarehouseNumber",
                 DbType.Int32,
                 "@WarehouseNumber",
                 QueryConditionOperatorType.Equal,
                 filter.WarehouseNumber
                 );

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.UpdateUser",
                DbType.Int32,
                "@UpdateUser",
                QueryConditionOperatorType.Equal,
                filter.UpdateUserSysNo
                );
                #region Set Base Parameter

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.CompanyCode",
                DbType.String,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode
                );

                #endregion

                command.CommandText = sqlBuilder.BuildQuerySql();
                if (filter.SOSysNo.HasValue)
                {
                    string subQuerySQLSOID = @"
                                DECLARE @tb TABLE(id int identity(1,1) primary key ,sosysno int );
                                INSERT INTO @tb
                                SELECT sosysno
                                FROM [IPP3].[dbo].SO_CheckShipping  with(nolock) WHERE sosplitmaster IN ({0})
                                UNION ALL
                                SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster in
                                 (SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0}))";
                    subQuerySQLSOID = string.Format(subQuerySQLSOID, filter.SOSysNo);
                    command.CommandText = command.CommandText.Replace("#tempCondition#", subQuerySQLSOID);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace("#tempCondition#", "");
                }

                var dt = command.ExecuteDataTable();
                if (isNeedChange)
                {
                    //替换枚举列和CodeNamePair列
                    EnumColumnList enumColList = new EnumColumnList();
                    enumColList.Add("Status", typeof(SOPendingStatus));
                    enumColList.Add("SoStatusShow", typeof(SOStatus));

                    CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                    codeNameList.Add("IsPartialShipping", "SO", "OutStockStatus", "IsPartialShipping_Value");

                    command.ConvertColumn(dt, enumColList, codeNameList);
                }
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable PendingListQuery(SOPendingQueryFilter filter, out int dataCount)
        {
            return PendingListQuery(filter, out dataCount, true);
        }

        public DataTable InternalMemoQuery(SOInternalMemoQueryFilter queryEntity, out int dataCount, bool isNeedChange = true)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_InternalMemo");

            DataTable result = null;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(queryEntity.PagingInfo),
                "SOIM.SysNO DESC"))
            {
                if (queryEntity.SOSysNo.HasValue)
                {
                    string subQuerySQLSOID = @"SELECT sosysno
                                FROM [IPP3].[dbo].SO_CheckShipping  with(nolock) WHERE sosplitmaster IN ({0})
                                UNION ALL
                                SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster in
                                 (SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0}))";

                    subQuerySQLSOID = string.Format(subQuerySQLSOID, queryEntity.SOSysNo);

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "SOIM.SOSysNo",
                        QueryConditionOperatorType.In,
                        subQuerySQLSOID);
                }


                sqlBuilder.ConditionConstructor.AddCondition(
                          QueryConditionRelationType.AND,
                          "SOIM.SysNo",
                          DbType.Int32,
                          "@SystemNumber",
                          QueryConditionOperatorType.Equal,
                          queryEntity.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOIM.CreateTime",
                         DbType.DateTime,
                        "@CreateTimeBegin",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryEntity.CreateTimeBegin);

                sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "SOIM.CreateTime",
                        DbType.DateTime,
                       "@CreateTimeEnd",
                       QueryConditionOperatorType.LessThan,
                       queryEntity.CreateTimeEnd);
                if (queryEntity.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "SOIM.Status",
                              DbType.Int32,
                              "@Status",
                              QueryConditionOperatorType.Equal,
                              (int)queryEntity.Status);
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                          QueryConditionRelationType.AND,
                          "SOIM.CallType",
                          DbType.Int32,
                          "@CallType",
                          QueryConditionOperatorType.Equal,
                          queryEntity.CallType);

                sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "SOIM.[CreateUserSysNo]",
                         DbType.Int32,
                         "@CreateUserSysNo",
                         QueryConditionOperatorType.Equal,
                         queryEntity.CreateUserSysNo);

                //以前存在  关闭者系统编号
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SOIM.UpdateUserSysNo",
                    DbType.Int32,
                    "@UpdateUserSysNo",
                    QueryConditionOperatorType.Equal,
                    queryEntity.UpdateUserSysNo);

                //关闭日志时间参数
                sqlBuilder.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "SOIM.UpdateTime",
                      DbType.DateTime,
                     "@UpdateTimeBegin",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     queryEntity.UpdateTimeBegin);

                sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "SOIM.UpdateTime",
                        DbType.DateTime,
                       "@UpdateTimeEnd",
                       QueryConditionOperatorType.LessThan,
                       queryEntity.UpdateTimeEnd);

                //责任部门参数
                sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOIM.DepartmentCode",
                        DbType.String,
                        "@DepartmentCode",
                        QueryConditionOperatorType.Equal,
                        queryEntity.ResponsibleDep);

                //紧急程度参数
                sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOIM.Importance",
                        DbType.Int32,
                        "@Importance",
                        QueryConditionOperatorType.Equal,
                        queryEntity.Importance);


                #region SetBaseParameter
                sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SOIM.[CompanyCode]",
                        DbType.String,
                        "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        queryEntity.CompanyCode);

                #endregion

                command.CommandText = sqlBuilder.BuildQuerySql();
                result = command.ExecuteDataTable();
                //替换枚举列和CodeNamePair列
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(SOInternalMemoStatus));

                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                if (isNeedChange)
                {
                    codeNameList.Add("SourceSysNo", "SO", "SOInernalMemoSource");
                    codeNameList.Add("Importance", "SO", "CallBackDegree");
                }

                command.ConvertColumn(result, enumColList, codeNameList);
            }

            dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return result;
        }
        #region 中蛋定制化查询

        public DataTable ComplainQuery(ComplainQueryFilter filter, out int totalCount)
        {
            return ComplainQuery(filter, out totalCount, true);
        }

        public DataTable ComplainQuery(ComplainQueryFilter filter, out int totalCount, bool isNeedChangeCodeName)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_ComplainList");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "C.SysNO DESC"))
            {
                AddComplainParameter(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(SOComplainStatus));
                enumColList.Add("ReplyType", typeof(SOComplainReplyType));

                CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();
                if (isNeedChangeCodeName)
                {
                    codenameColList.Add("ResponsibleDepartment", "SO", "ResponsibleDept");
                }

                cmd.ConvertColumn(dt, enumColList, codenameColList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddComplainParameter(ComplainQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            if (filter.OnlyCustomer)
            {
                sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.CreateCustomerSysNo",
                DbType.String,
                "@CreateCustomerSysNo",
                QueryConditionOperatorType.NotEqual,
                0
                );
            }
            sb.ConditionConstructor.AddCondition(
            QueryConditionRelationType.AND,
            "C.CompanyCode",
            DbType.AnsiStringFixedLength,//原先的@CompanyCode数据类型（Nvarchar）定义有误，现改为 AnsiStringFixedLength （Char）
            "@CompanyCode",
            QueryConditionOperatorType.Equal,
            filter.CompanyCode
            );

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.ComplainSysNo",
                DbType.String,
                "@ComplainID",
                QueryConditionOperatorType.Equal,
                filter.ComplainID
                );

            sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "C.SysNo",
                 DbType.Int32,
                 "@SystemNumber",
                 QueryConditionOperatorType.Equal,
                 filter.SystemNumber
                 );

            if (filter.SOSysNo.HasValue)
            {
                string subQuerySQLSOID = @"SELECT sosysno
                                FROM [IPP3].[dbo].SO_CheckShipping  with(nolock) WHERE sosplitmaster IN ({0})
                                UNION ALL
                                SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0})
                                UNION ALL
                                SELECT sosysno FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosplitmaster in
                                 (SELECT sosplitmaster 
                                FROM [IPP3].[dbo].SO_CheckShipping with(nolock) WHERE sosysno IN ({0}))";

                subQuerySQLSOID = string.Format(subQuerySQLSOID, filter.SOSysNo);

                sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "C.SOSysNo",
                    QueryConditionOperatorType.In,
                    subQuerySQLSOID);
            }


            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.Subject",
                DbType.String,
                "@Subject",
                QueryConditionOperatorType.Like,
                filter.Subject
                );

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.ComplainType",
                DbType.String,
                "@ComplainType",
                QueryConditionOperatorType.Equal,
                filter.ComplainType
                );


            sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "C.CreateDate",
                 DbType.DateTime,
                 "@CreateDateFrom",
                 QueryConditionOperatorType.MoreThanOrEqual,
                 filter.CreateDateFrom
                 );

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.CreateDate",
                DbType.DateTime,
                "@CreateDateTo",
                QueryConditionOperatorType.LessThan,
                filter.CreateDateTo
                );
            #region OutdatedTime

            DateTime? outDatedTime = null;
            if (filter.OutdatedTimeType.HasValue)
            {
                switch (filter.OutdatedTimeType.Value)
                {
                    case OutdatedType.MoreTwoHours:
                        outDatedTime = CommonUtility.AddWorkMinute(DateTime.Now, -1 * 60 * 2);
                        break;
                    case OutdatedType.MoreOneDay:
                        outDatedTime = CommonUtility.AddWorkMinute(DateTime.Now, -1 * 60 * 9);
                        break;
                    case OutdatedType.MoreThreeDays:
                        outDatedTime = CommonUtility.AddWorkMinute(DateTime.Now, -1 * 60 * 9 * 3);
                        break;
                }
            }

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.CreateDate",
                DbType.DateTime,
                "@OutdatedTime",
                QueryConditionOperatorType.LessThan,
                outDatedTime
                );
            #endregion

            #region Modified by Wind 2009-21-21

            if (filter.Status.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "C.Status",
                    DbType.Int32,
                    "@Status",
                    QueryConditionOperatorType.Equal,
                    (int)filter.Status.Value
                    );
            }
            else
            {
                if (filter.ValidCase)
                {
                    sb.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "C.Status",
                         DbType.Int32,
                         "@Status",
                         QueryConditionOperatorType.NotEqual,
                         -1
                         );
                }
            }

            #endregion

            sb.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "C.ComplainSourceType",
               DbType.String,
               "@ComplainSourceType",
               QueryConditionOperatorType.Equal,
               filter.ComplainSourceType
               );

            sb.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "C.ResponsibleDept",
               DbType.String,
               "@ResponsibleDept",
               QueryConditionOperatorType.Equal,
               filter.ResponsibleDept
               );

            if (!string.IsNullOrEmpty(filter.OrderType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "C.OrderType",
                    DbType.String,
                    "@OrderType",
                    QueryConditionOperatorType.Equal,
                    filter.OrderType);
            }

            //[Jay]:按照CustomerID精确匹配
            if (!string.IsNullOrEmpty(filter.CustomerIDSingle)
                && string.IsNullOrEmpty(filter.CustomerID))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CUIN2.CustomerID",
                    DbType.String,
                    "@CustomerID",
                    QueryConditionOperatorType.Equal,
                    filter.CustomerIDSingle
                    );
            }
            else
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CUIN2.CustomerID",
                    DbType.String,
                    "@CustomerID",
                    QueryConditionOperatorType.LeftLike,//原先的 Like 匹配方式不合适，现改为 LeftLike
                    filter.CustomerID
                );
            }

            //未处理人
            if (filter.OperatorSysNo == 0)
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "C.OperatorSysNo",
                   DbType.Int32,
                   "@OperatorSysNo",
                   QueryConditionOperatorType.IsNull,
                   DBNull.Value
                   );
            }
            else
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "C.OperatorSysNo",
                   DbType.Int32,
                   "@OperatorSysNo",
                   QueryConditionOperatorType.Equal,
                   filter.OperatorSysNo
                   );
            }
            if (filter.StartFollowUpdate.HasValue
                || filter.EndFollowUpdate.HasValue)
            {
                ConditionConstructor subQuery = sb.ConditionConstructor.AddSubQueryCondition(
                               QueryConditionRelationType.AND, "C.SysNo",
                               QueryConditionOperatorType.In, @"SELECT [ComplainSysNo]
                                                                    FROM 
                                                                    (   SELECT [ComplainSysNo],max([CreateDate]) AS 'CreateDate'
                                                                        FROM [IPP3].[dbo].[Complain_History]
                                                                        GROUP BY [ComplainSysNo]
                                                                    ) AS b");

                subQuery.AddCondition(QueryConditionRelationType.AND, "CreateDate", DbType.DateTime, "@Sub1StartFollowUpdate",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.StartFollowUpdate);

                subQuery.AddCondition(QueryConditionRelationType.AND, "CreateDate", DbType.DateTime, "@Sub1EndFollowUpdate",
                    QueryConditionOperatorType.LessThan, filter.EndFollowUpdate);
            }

            //对处理完毕处理人及时间的search
            if (filter.Status == SOComplainStatus.Complete)
            {
                if (filter.StartCompleteDate.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CC.CloseDate",
                        DbType.DateTime,
                        "@StartCompleteDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.StartCompleteDate
                    );
                }
                if (filter.EndCompleteDate.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CC.CloseDate",
                        DbType.DateTime,
                        "@EndCompleteDate",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.EndCompleteDate
                    );
                }
            }

            if (filter.IsReOpen)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "C.ReopenCount",
                    DbType.Int32,
                    "@ReOpenCount",
                    QueryConditionOperatorType.Equal,
                    filter.ReOpenCount.HasValue ? filter.ReOpenCount : 0
                );
            }

            if (filter.SpendHours.HasValue)
            {
                QueryConditionOperatorType operatorType = QueryConditionOperatorType.Equal;
                switch (filter.Operator4SpendHours.ToLower())
                {
                    case "greater":
                        operatorType = QueryConditionOperatorType.MoreThan;
                        break;
                    case "less":
                        operatorType = QueryConditionOperatorType.LessThan;
                        break;
                }

                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ISNULL(C.SpendHours,0)",
                    DbType.Int32,
                    "@SpendHours",
                    operatorType,
                    filter.SpendHours
                );
            }

            cmd.CommandText = sb.BuildQuerySql();
        }

        #region 查询出货单
        public DataTable OutStockQuery(SOOutStockQueryFilter filter, out int totalCount)
        {
            return OutStockQuery(filter, out totalCount, true);
        }

        /// <summary>
        /// 查询出货单
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">查询记录数</param>
        /// <param name="isNeedChange">是否交换CodeName</param>
        /// <returns>记录集</returns>
        int totalCount1 = 0;
        int totalCount2 = 0;
        bool isNeedSearch = true;
        DataTable totalResult = new DataTable();
        SOOutStockQueryFilter currentFilter = new SOOutStockQueryFilter();
        public DataTable OutStockQuery(SOOutStockQueryFilter filter, out int totalCount, bool isNeedChange)
        {
            if (filter.IsUniteOrderCount == null
                || filter.IsUniteOrderCount == false)
            {
                if (filter.IsCurrentData == SOIsCurrentData.Current)
                {
                    return GetSOOutStockCurrent(filter, out totalCount);
                }
                else
                {
                    return GetSOOutStockAll(filter, out totalCount);
                }
            }
            else
            {
                //判断如果查询条件有变化的时候重新查询数据
                if (currentFilter.SOID != filter.SOID || currentFilter.CustomerName != filter.CustomerName || currentFilter.DeliveryPsersonNo != filter.DeliveryPsersonNo
                    || currentFilter.ShippedOutTimeFrom != filter.ShippedOutTimeFrom || currentFilter.ShippedOutTimeTo != filter.ShippedOutTimeTo
                    || currentFilter.IsBig != filter.IsBig || currentFilter.IsCurrentData != filter.IsCurrentData || currentFilter.IsFirstFilter != filter.IsFirstFilter
                    || currentFilter.IsPackaged != filter.IsPackaged || currentFilter.IsUniteOrderCount != filter.IsUniteOrderCount || currentFilter.IsVAT != filter.IsVAT
                    || currentFilter.IsVIPCustomer != filter.IsVIPCustomer || currentFilter.ProductSysNo != filter.ProductSysNo || currentFilter.ReceiveAddress != filter.ReceiveAddress
                    || currentFilter.ReceiveAreaSysNo != filter.ReceiveAreaSysNo || currentFilter.ShipTypeSysNo != filter.ShipTypeSysNo || currentFilter.SpecialSOType != filter.SpecialSOType
                    || currentFilter.StockSysNo != filter.StockSysNo || currentFilter.CompanyCode != filter.CompanyCode)
                {
                    currentFilter = filter;
                    isNeedSearch = true;
                }
                string sortField = string.Empty;
                string sorter = string.Empty;
                if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
                {
                    sortField = filter.PagingInfo.SortBy.ToLower().Split(" ".ToCharArray())[0].Trim();
                    sorter = filter.PagingInfo.SortBy.ToLower().Split(" ".ToCharArray())[1].Trim();
                }

                DataTable result1;
                DataTable result2;

                int startRowIndex = filter.PagingInfo.PageIndex.Value;
                int pageSize = filter.PagingInfo.PageSize.Value;
                if (isNeedSearch)
                {
                    if (filter.IsCurrentData == SOIsCurrentData.Current)
                    {
                        result1 = GetSOOutStockCurrent(filter, out totalCount1);
                        result2 = GetSOOutStockCurrentMerge(filter, out totalCount2);
                        isNeedSearch = false;
                    }
                    else
                    {
                        result1 = GetSOOutStockAll(filter, out totalCount1);
                        result2 = GetSOOutStockAllMerge(filter, out totalCount2);
                        isNeedSearch = false;
                    }
                    #region 去除重复单
                    if (result1 != null
                        && result1.Rows.Count > 0)
                    {
                        if (result2 != null
                            && result2.Rows.Count > 0)
                        {
                            if (result1.Rows.Count >= result2.Rows.Count)
                            {
                                for (int i = result1.Rows.Count - 1; i >= 0; i--)
                                {
                                    for (int k = result2.Rows.Count - 1; k >= 0; k--)
                                    {
                                        if (Convert.ToInt32(result1.Rows[i]["SysNo"]) == Convert.ToInt32(result2.Rows[k]["SysNo"]))
                                        {
                                            result2.Rows.RemoveAt(k);
                                            totalCount2--;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (result1.Rows.Count <= result2.Rows.Count)
                    {
                        for (int i = result2.Rows.Count - 1; i >= 0; i--)
                        {
                            for (int k = result1.Rows.Count - 1; k >= 0; k--)
                            {
                                if (Convert.ToInt32(result2.Rows[i]["SysNo"]) == Convert.ToInt32(result1.Rows[k]["SysNo"]))
                                {
                                    result1.Rows.RemoveAt(k);
                                    totalCount1--;
                                }
                            }
                        }
                    }
                    #endregion

                    totalCount = totalCount1 + totalCount2;
                    totalResult = result1.Clone();
                    foreach (DataRow row in result1.Rows)
                    {
                        totalResult.ImportRow(row);
                    }

                    foreach (DataRow row in result2.Rows)
                    {
                        totalResult.ImportRow(row);
                    }
                }
                else
                    totalCount = totalCount1 + totalCount2;

                DataTable result = new DataTable();

                //分检出指定的记录
                result = totalResult.Clone();
                if ((startRowIndex + 1) * pageSize < totalCount)
                {
                    for (int i = 0; i < pageSize; i++)
                    {
                        result.ImportRow(totalResult.Rows[(startRowIndex + 1) * pageSize - i]);
                    }
                }
                else
                {
                    int lastPageCount = pageSize - ((startRowIndex + 1) * pageSize - totalCount);//计算最后一页的记录数
                    for (int j = 0; j < lastPageCount; j++)
                    {
                        result.ImportRow(totalResult.Rows[lastPageCount - j]);
                    }
                }
                return result;
            }

        }

        #region 获取当前数据
        public DataTable GetSOOutStockCurrent(SOOutStockQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_OutStockSearchCurrent");
            DynamicQuerySqlBuilder sb;
            if (filter.IsUniteOrderCount == false
                || filter.IsUniteOrderCount == null)
                sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SysNo DESC");
            else
                sb = new DynamicQuerySqlBuilder(cmd, "SysNo DESC");
            AddOutStockParameterCurrent(filter, cmd, sb);
            var dt = cmd.ExecuteDataTable();
            //替换枚举列
            EnumColumnList enumColList = new EnumColumnList();
            enumColList.Add("IncomeStatus", typeof(SOIncomeStatus));

            CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();

            cmd.ConvertColumn(dt, enumColList, codenameColList);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        private void AddOutStockParameterCurrent(SOOutStockQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            AddOutStockCommonParameter(filter, cmd, sb);

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.OutTime",
                DbType.DateTime,
                "@ShippedOutTimeFrom",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.ShippedOutTimeFrom
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.OutTime",
                DbType.DateTime,
                "@ShippedOutTimeTo",
                QueryConditionOperatorType.LessThan,
                filter.ShippedOutTimeTo
                );
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion 获取当前数据

        #region 获取当前数据 并单统计
        public DataTable GetSOOutStockCurrentMerge(SOOutStockQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_OutStockSearchCurrent");

            using (var sb = new DynamicQuerySqlBuilder(cmd, "SysNo DESC"))
            {
                AddOutStockParameterCurrentMerge(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                //替换枚举列
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("IncomeStatus", typeof(SOIncomeStatus));

                CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();

                cmd.ConvertColumn(dt, enumColList, codenameColList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddOutStockParameterCurrentMerge(SOOutStockQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            AddOutStockCommonParameter(filter, cmd, sb);

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_CheckShipping.MergeOutTime",
                DbType.DateTime,
                "@MergeOutTime",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.ShippedOutTimeFrom
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_CheckShipping.MergeOutTime",
                DbType.DateTime,
                "@MergeOutTime",
                QueryConditionOperatorType.LessThan,
                filter.ShippedOutTimeTo
                );
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion 获取当前数据 并单统计

        #region 获取所有数据
        public DataTable GetSOOutStockAll(SOOutStockQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_OutStockSearchAll");
            DynamicQuerySqlBuilder sb;
            if (filter.IsUniteOrderCount == false
                || filter.IsUniteOrderCount == null)
                sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SysNo DESC");
            else
                sb = new DynamicQuerySqlBuilder(cmd, "SysNo DESC");
            AddOutStockParameterAll(filter, cmd, sb);
            var dt = cmd.ExecuteDataTable();
            //替换枚举列
            EnumColumnList enumColList = new EnumColumnList();
            enumColList.Add("IncomeStatus", typeof(SOIncomeStatus));

            CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();

            cmd.ConvertColumn(dt, enumColList, codenameColList);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        private void AddOutStockParameterAll(SOOutStockQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            AddOutStockCommonParameter(filter, cmd, sb);

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.OutTime",
                DbType.DateTime,
                "@ShippedOutTimeFrom",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.ShippedOutTimeFrom
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.OutTime",
                DbType.DateTime,
                "@ShippedOutTimeTo",
                QueryConditionOperatorType.LessThan,
                filter.ShippedOutTimeTo
                );
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion 获取所有数据

        #region 获取所有数据 并单统计
        public DataTable GetSOOutStockAllMerge(SOOutStockQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_OutStockSearchAll");
            using (var sb = new DynamicQuerySqlBuilder(cmd, "SysNo DESC"))
            {
                AddOutStockParameterAllMerge(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                //替换枚举列
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("IncomeStatus", typeof(SOIncomeStatus));

                CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();

                cmd.ConvertColumn(dt, enumColList, codenameColList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddOutStockParameterAllMerge(SOOutStockQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            AddOutStockCommonParameter(filter, cmd, sb);
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_CheckShipping.MergeOutTime",
                DbType.DateTime,
                "@ShippedOutTimeFrom",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.ShippedOutTimeFrom
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_CheckShipping.MergeOutTime",
                DbType.DateTime,
                "@ShippedOutTimeTo",
                QueryConditionOperatorType.LessThan,
                filter.ShippedOutTimeTo
                );
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion 获取所有数据 并单统计

        #region 添加公共的参数

        private void AddOutStockCommonParameter(SOOutStockQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.ShipTypeSysNo",
                DbType.Int32,
                "@ShipTypeSysNo",
                QueryConditionOperatorType.Equal,
                filter.ShipTypeSysNo
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.Status",
                DbType.Int32,
                "@Status",
                QueryConditionOperatorType.Equal,
                4
                );
            if (filter.DeliveryPsersonNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SM.FreightUserSysNo",
                    DbType.Int32,
                    "@DeliveryPsersonNo",
                    QueryConditionOperatorType.Equal,
                    filter.DeliveryPsersonNo
                    );
            }

            if (!string.IsNullOrEmpty(filter.ProductSysNo))
            {
                var subQuery = sb.ConditionConstructor.AddSubQueryCondition(
                    QueryConditionRelationType.AND,
                    "SM.SysNO",
                    QueryConditionOperatorType.In,
                    @"SELECT  SI.SOSysNo
                    FROM	IPP3.dbo.SO_item SI (NOLOCK)
		                    LEFT JOIN OverseaContentManagement.dbo.V_CM_ItemBasicInfo PRDI (NOLOCK)
			                    ON(SI.ProductSysNo = PRDI.SysNo AND PRDI.ProductType != 3) 
                    --#StrWhere#
                    ");
                subQuery.AddCondition(QueryConditionRelationType.AND, "PRDI.ProductID", System.Data.DbType.String, "@ProductID", QueryConditionOperatorType.Like, filter.ProductSysNo);
            }

            if (filter.StockSysNo.HasValue)
            {
                var subQuery = sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "SM.SysNO", QueryConditionOperatorType.In,
                    @"SELECT SOSysNo 
                    FROM IPP3.dbo.so_item 
                    --#StrWhere#
                    ");
                subQuery.AddCondition(QueryConditionRelationType.AND, "WarehouseNumber", System.Data.DbType.Int32, "@WarehouseNumber", QueryConditionOperatorType.Equal, filter.StockSysNo);
            }

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.IsVAT",
                DbType.Int32,
                "@IsVAT",
                QueryConditionOperatorType.Equal,
                LegacyEnumMapper.ConvertSYNStatus(filter.IsVAT)
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.IsLarge",
                DbType.Int32,
                "@IsBig",
                QueryConditionOperatorType.Equal,
                LegacyEnumMapper.ConvertSYNStatus(filter.IsBig)
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.IsPrintPackageCover",
                DbType.Int32,
                "@IsPackaged",
                QueryConditionOperatorType.Equal,
                LegacyEnumMapper.ConvertSYNStatus(filter.IsPackaged)
                );

            if (filter.IsVIPCustomer.HasValue)
            {
                if (LegacyEnumMapper.ConvertSYNStatus(filter.IsVIPCustomer.Value) == 1)
                {
                    sb.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND,
                        "(Customer.VIPRank = 2 OR Customer.VIPRank = 4 )"
                        );
                }
                else if (LegacyEnumMapper.ConvertSYNStatus(filter.IsVIPCustomer.Value) == 0)
                {
                    sb.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND,
                        "(Customer.VIPRank = 1 OR Customer.VIPRank = 3 )"
                        );
                }
            }

            if (filter.DeliveryPsersonNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SM.FreightUserSysNo",
                    DbType.Int32,
                    "@DeliveryPsersonNo",
                    QueryConditionOperatorType.Equal,
                    filter.DeliveryPsersonNo
                    );
            }

            if (filter.ReceiveAreaSysNo.HasValue
                && filter.ReceiveAreaSysNo.Value > 0)
            {
                sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SM.ReceiveAreaSysNo",
                    DbType.Int32,
                    "@ReceiveAreaSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.ReceiveAreaSysNo
                    );
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.OR,
                    "AREA.CitySysno",
                    DbType.Int32,
                    "@CitySysNo",
                    QueryConditionOperatorType.Equal,
                    filter.ReceiveAreaSysNo
                    );
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.OR,
                    "AREA.ProvinceSysno",
                    DbType.Int32,
                    "@ProviceSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.ReceiveAreaSysNo
                    );
                sb.ConditionConstructor.EndGroupCondition();
            }

            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.ReceiveAddress",
                DbType.String,
                "@ReceiveAddress",
                QueryConditionOperatorType.Like,
                filter.ReceiveAddress
                );
            sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SM.SysNO",
                    DbType.String,
                    "@SOID",
                    QueryConditionOperatorType.Equal,
                    filter.SOID
                    );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_CheckShipping.SpecialSOType",
                DbType.Int32,
                "@SpecialSOType",
                QueryConditionOperatorType.Equal,
                filter.SpecialSOType
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.ReceiveContact",
                DbType.String,
                "@CustomerName",
                QueryConditionOperatorType.Equal,
                filter.CustomerName
                );
            if (filter.IsFirstFilter == SYNStatus.Yes)
            {
                sb.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND,
                        "Exists (Select Top 1 1  From IPP3.DBO.V_SO_Master With(NOLOCK) Where Status=4 AND ShiptypeSysNo=SM.ShipTypeSysNo AND CustomerSysno=SM.CustomerSysNo Group by CustomerSysNo,ShipTypeSysNo Having (count(*)=1))"
                    );
            }
        }

        #endregion 添加公共的参数

        #endregion 查询出货单

        /// <summary>
        /// 订单系统日志查询
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="totalCount">列表总数</param>
        /// <returns>查询结果</returns>
        public DataTable LogQuery(SOLogQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_Log");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SO_Log.SysNo DESC"))
            {
                #region 赋值参数

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO_Log.SOSysNo",
                DbType.Int32,
                "@SONumber",
                QueryConditionOperatorType.Equal,
                filter.SOSysNo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                #endregion

                var dt = cmd.ExecuteDataTable("OptType", typeof(BizLogType));

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return dt;
            }
        }

        /// <summary>
        /// 分仓销售单据查询 
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">查询的记录数</param>
        /// <returns>记录集</returns>
        public DataTable WHSOOutStockQuery(WHSOOutStockQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_WHSOOutStockList");
            cmd.CommandTimeout = 600;

            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PageInfo), "SM.SysNO DESC"))
            {
                AddParameter4GetWHSOOutStockList(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                EnumColumnList enumColList = new EnumColumnList();

                enumColList.Add("Status", typeof(SOStatus));
                enumColList.Add("IsEnough", typeof(EnoughFlag));
                enumColList.Add("PayStatus", typeof(NetPayStatus));
                enumColList.Add("SpecialSOType", typeof(SpecialSOType));
                enumColList.Add("ISVAT", typeof(BooleanType));

                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                codeNameList.Add("DeliveryTimeRange", "Common", "TimeRange");

                cmd.ConvertColumn(dt, enumColList, codeNameList);

                return dt;
            }
        }

        private static void AddParameter4GetWHSOOutStockList(WHSOOutStockQueryFilter filter, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            sb1.Append("WHERE SM.Status = 1 ");
            sb2.Append("WHERE 1=1 ");


            if (filter.DeliveryDateTime.HasValue
                && filter.DeliveryDateTime.Value.Year > 1)
            {
                if (!String.IsNullOrWhiteSpace(filter.DeliveryTimeRange))
                {
                    sb1.AppendFormat(" AND ( ISNUll(SM.DeliveryDate,OrderDate) < '{0}' OR (ISNUll(SM.DeliveryDate,OrderDate) = '{0}' AND SM.DeliveryTimeRange <= {1}))", filter.DeliveryDateTime.Value.ToShortDateString(), filter.DeliveryTimeRange);
                }
                else
                {
                    sb1.AppendFormat(" AND ISNUll(SM.DeliveryDate,OrderDate) <= '{0}'", filter.DeliveryDateTime.Value.ToShortDateString());
                }
            }

            if (filter.ShipTypeSysNo.HasValue)
            {
                string shipTypeConditionString = string.Empty;
                ConditionType shipTypeConditionEnum = ConditionType.Equal;
                if (filter.ShipTypeCondition.HasValue &&
                   Enum.TryParse<ConditionType>(Convert.ToString(filter.ShipTypeCondition.Value), out shipTypeConditionEnum))
                {
                    switch (shipTypeConditionEnum)
                    {
                        case ConditionType.Equal: shipTypeConditionString = "=";
                            break;
                        case ConditionType.Unequal: shipTypeConditionString = "<>";
                            break;
                        default: throw new NotImplementedException();
                    }
                }

                sb1.Append(" AND SM.ShipTypeSysno").Append(shipTypeConditionString).Append(filter.ShipTypeSysNo.Value.ToString());
            }

            if (filter.StockSysNo.HasValue)
            {
                sb1.AppendFormat(" AND SI.WarehouseNumber = {0}", filter.StockSysNo.Value);
            }

            if (filter.ISVAT.HasValue)
            {
                sb1.AppendFormat(" AND SM.IsVat = {0}", filter.ISVAT.Value);
            }

            if (filter.AuditDateTimeFrom.HasValue)
            {
                sb1.AppendFormat(" AND SM.Audittime >= '{0}'", filter.AuditDateTimeFrom.Value.ToString());
            }

            if (filter.AuditDateTimeTo.HasValue)
            {
                sb1.AppendFormat(" AND SM.Audittime <= '{0}'", filter.AuditDateTimeTo.Value.ToString());
            }

            if (filter.ReceiveAreaSysNo.HasValue && filter.ReceiveAreaSysNo > 0)
            {
                sb2.AppendFormat(" AND ( SM.ReceiveAreaSysNo = {0} OR A.CitySysno = {0} OR A.ProvinceSysno = {0})", filter.ReceiveAreaSysNo.Value.ToString());
            }

            if (filter.SpecialSOType.HasValue)
            {
                if (filter.SpecialSOType.Value.ToString() == "0")
                    sb2.AppendFormat(" AND (S.SpecialSOType = {0} OR S.SpecialSOType IS Null)", filter.SpecialSOType.Value);
                else
                    sb2.AppendFormat(" AND S.SpecialSOType = {0}", filter.SpecialSOType.Value);
            }

            if (filter.EnoughFlag.HasValue)
            {
                sb2.AppendFormat(" AND TSM.IsEnough = {0}", filter.EnoughFlag.Value);
            }

            if (filter.StockSysNo.HasValue)
            {
                sb2.AppendFormat(@" AND SM.SysNo NOT IN (SELECT SONumber FROM OverseaInvoiceReceiptManagement.dbo.Invoice_Master WITH(NOLOCK) WHERE WarehouseNumber = '{0}')", filter.StockSysNo.Value);
            }

            if (!string.IsNullOrEmpty(filter.CompanyCode))
            {
                sb2.AppendFormat(@" AND SM.CompanyCode = {0}", filter.CompanyCode);
            }

            command.CommandText = sqlBuilder.BuildQuerySql();

            command.CommandText = command.CommandText.Replace("#WHERE01#", sb1.ToString());
            command.CommandText = command.CommandText.Replace("#WHERE02#", sb2.ToString());
        }

        #region 查询配送历史

        private const int INT_NULL = 0;
        private const string QueryDeliveryHistorySO_SQL_NAME = "SO_Query_DeliveryHistorySO";
        private const string QueryDeliveryHistoryNoSO_SQL_NAME = "SO_Query_DeliveryHistoryNoSO";
        private const string QueryDeliveryHistory_SQL_NAME = "SO_Query_DeliveryHistory";

        /// <summary>
        /// 查询配送历史  
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">查询的记录数</param>
        /// <returns></returns>
        public DataTable GetDeliveryHistoryList(SODeliveryHistoryQueryFilter query, out int totalCount)
        {
            if (query.OrderType.HasValue)
            {
                if (query.OrderType.Value == (int)ECCentral.BizEntity.SO.DeliveryType.SO)
                {
                    return GetDeliveryHistoryTable(query, QueryDeliveryHistorySO_SQL_NAME, out totalCount);
                }
                else
                {
                    return GetDeliveryHistoryTable(query, QueryDeliveryHistoryNoSO_SQL_NAME, out totalCount);
                }
            }
            else
            {
                return GetDeliveryHistoryViewList(query, QueryDeliveryHistory_SQL_NAME, out totalCount);
            }
        }

        public DataTable GetDeliveryHistoryTable(SODeliveryHistoryQueryFilter query, string configName, out int totalCount)
        {
            if (!string.IsNullOrEmpty(query.PageInfo.SortBy))
            {
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("DeliveryDate", "D.DeliveryDate");
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("DeliveryTimeRange", "D.DeliveryTimeRange");
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("DeliveryMemo", "D.DeliveryMemo");
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("Note", "D.Note");
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("Status", "D.Status");
                //query.PagingInfo.SortField = query.PagingInfo.SortField.Replace("Weight", "D.Weight");
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("DeliveryManUser", "D.DeliveryManUserSysNo");

            }
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig(configName);
            DataTable result = null;

            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(query.PageInfo), "   D.Status DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);


                if (query.OrderType.Value == (int)ECCentral.BizEntity.SO.DeliveryType.SO)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, ECCentral.BizEntity.SO.DeliveryType.SO);
                }
                else
                {

                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.NotEqual, ECCentral.BizEntity.SO.DeliveryType.SO);
                }

                if (query.OrderType.HasValue)
                {
                    if (query.OrderType.Value == (int)ECCentral.BizEntity.SO.DeliveryType.SO)
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType1", QueryConditionOperatorType.Equal, (int)ECCentral.BizEntity.SO.DeliveryType.RMARequest);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.OrderType", DbType.Int32, "@OrderType2", QueryConditionOperatorType.Equal, (int)ECCentral.BizEntity.SO.DeliveryType.VendorMend);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.OrderType", DbType.Int32, "@OrderType3", QueryConditionOperatorType.Equal, (int)ECCentral.BizEntity.SO.DeliveryType.VendorReturn);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.OrderType", DbType.Int32, "@OrderType4", QueryConditionOperatorType.Equal, (int)ECCentral.BizEntity.SO.DeliveryType.RMARevert);

                        sb.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType5", QueryConditionOperatorType.Equal, query.OrderType.Value);
                    }
                }
                if (query.Status.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status.Value);
                if (query.DeliveryManUserSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, query.DeliveryManUserSysNo.Value);
                if (query.OrderSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo.Value);

                if (query.AreaSysNo.HasValue && query.AreaSysNo != INT_NULL)
                {
                    sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.AreaSysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.EndGroupCondition();
                }
                if (query.DeliveryDateFrom.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.DeliveryDateFrom);
                }
                if (query.DeliveryDateTo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateTo", QueryConditionOperatorType.LessThan, query.DeliveryDateTo);
                }
                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                EnumColumnList enumColList = new EnumColumnList();

                enumColList.Add("OrderType", typeof(ECCentral.BizEntity.SO.DeliveryType));
                enumColList.Add("DeliveryTimeRange", typeof(ECCentral.BizEntity.SO.DeliveryTimeRange));
                enumColList.Add("Status", typeof(DeliveryStatus));

                cmd.ConvertEnumColumn(result, enumColList);
            }

            return result;
        }

        public DataTable GetDeliveryHistoryViewList(SODeliveryHistoryQueryFilter query, string configName, out int totalCount)
        {
            if (!string.IsNullOrEmpty(query.PageInfo.SortBy))
            {
                query.PageInfo.SortBy = query.PageInfo.SortBy.Replace("DeliveryManUser", "DeliveryManUserSysNo");
            }
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig(configName);
            DataTable result = null;

            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(query.PageInfo), "  Status DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);


                if (query.OrderType.HasValue)
                {
                    if (query.OrderType.Value == 2)
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 3);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 4);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 5);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 6);

                        sb.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType.Value);
                    }
                }
                if (query.Status.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status.Value);
                if (query.DeliveryManUserSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, query.DeliveryManUserSysNo.Value);
                if (query.OrderSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo.Value);

                if (query.AreaSysNo.HasValue && query.AreaSysNo != INT_NULL)
                {
                    sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AreaSysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "CitySysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "ProvinceSysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, query.AreaSysNo.Value);
                    sb.ConditionConstructor.EndGroupCondition();
                }
                if (query.DeliveryDateFrom.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DeliveryDate", DbType.DateTime, "@DeliveryDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.DeliveryDateFrom);
                }
                if (query.DeliveryDateTo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DeliveryDate", DbType.DateTime, "@DeliveryDateTo", QueryConditionOperatorType.LessThan, query.DeliveryDateTo);
                }
                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                EnumColumnList enumColList = new EnumColumnList();

                enumColList.Add("OrderType", typeof(ECCentral.BizEntity.SO.DeliveryType));
                enumColList.Add("DeliveryTimeRange", typeof(ECCentral.BizEntity.SO.DeliveryTimeRange));
                enumColList.Add("Status", typeof(DeliveryStatus));

                cmd.ConvertEnumColumn(result, enumColList);
            }

            return result;
        }
        #endregion

        #region 查询配送任务

        /// <summary>
        /// 查询配送任务
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        public DataTable GetDeliveryAssignTask(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            int? pageSize = qcentity.PageInfo.PageSize;
            int? startIndex = qcentity.PageInfo.PageIndex;

            DataTable result = null;
            totalCount = 0;

            if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.SO)
            {
                result = GetDeliveryAssignTask4SO(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.RMARequest)
            {
                result = GetDeliveryAssignTask4RMARequest(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.RMARevert)
            {
                result = GetDeliveryAssignTask4RMARevert(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.VendorMend)
            {
                result = GetDeliveryAssignTask4VendorOut(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.VendorReturn)
            {
                result = GetDeliveryAssignTask4VendorIn(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.RMAALL)
            {
                result = GetDeliveryAssignTask4RMAAll(qcentity, out totalCount);
            }
            else if (qcentity.OrderType == (int)ECCentral.BizEntity.SO.DeliveryType.ALL)
            {
                result = GetDeliveryAssignTask4All(qcentity, out totalCount);
            }

            qcentity.PageInfo.PageSize = pageSize;
            qcentity.PageInfo.PageIndex = (startIndex.HasValue) ? startIndex.Value : 0;

            if (result != null)
            {
                if (!result.Columns.Contains("RowID"))
                {
                    result.Columns.Add(new DataColumn("RowID", typeof(int)));
                }
                int pageSizeInt = qcentity.PageInfo.PageSize.HasValue ? qcentity.PageInfo.PageSize.Value : 0;
                int pageIndexInt = qcentity.PageInfo.PageIndex.HasValue ? qcentity.PageInfo.PageIndex.Value : 0;

                int startRowNum = pageSizeInt * (pageIndexInt) + 1;

                foreach (DataRow row in result.Rows)
                {
                    row["RowID"] = startRowNum++;
                }

                CustomDataCommand tempCmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4SO");

                EnumColumnList enumColList = new EnumColumnList();

                enumColList.Add("OrderType", typeof(ECCentral.BizEntity.SO.DeliveryType));

                tempCmd.ConvertEnumColumn(result, enumColList);
            }

            return result;
        }

        /// <summary>
        /// SO送货单查询
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        private DataTable GetDeliveryAssignTask4SO(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            //QuerySorterHelper4AssignTaskSO.SetSorterSQL(qcentity.PagingInfo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4SO");

            DataTable result = new DataTable();
            totalCount = 0;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(qcentity.PageInfo),
                "D.SysNO DESC"))
            {
                AddParameter4DeliveryAssignTask4SO(qcentity, command, sqlBuilder);
                result = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }

            if (!result.Columns.Contains("OrderType"))
            {
                result.Columns.Add("OrderType", typeof(ECCentral.BizEntity.SO.DeliveryType));
            }

            if (result != null
                && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    row["OrderType"] = ECCentral.BizEntity.SO.DeliveryType.SO;
                    row["Amt"] = (row["IsPayWhenRecv"] != DBNull.Value && Convert.ToInt32(row["IsPayWhenRecv"]) == 1) ? Convert.ToDecimal(row["Amt"]) - Convert.ToDecimal(row["PrePayAmt"]) : 0;

                    if (row["Amt"] != null && row["Amt"] != DBNull.Value)
                    {
                        qcentity.TotalAmt += Convert.ToDecimal(row["Amt"]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取RMA_Request中符合条件的数据
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        private DataTable GetDeliveryAssignTask4RMARequest(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            //QuerySorterHelper4TaskRMARequest.SetSorterSQL(qcentity.PagingInfo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4RMARequest");

            DataTable result = new DataTable();
            totalCount = 0;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(qcentity.PageInfo),
                "D.SysNO DESC"))
            {
                AddParameter4DeliveryAssignTask4RMARequest(qcentity, command, sqlBuilder);
                result = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }

            if (!result.Columns.Contains("OrderType"))
            {
                result.Columns.Add("OrderType", typeof(ECCentral.BizEntity.SO.DeliveryType));
            }

            if (result != null
                && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    row["OrderType"] = ECCentral.BizEntity.SO.DeliveryType.RMARequest;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取RMA_Revert发货中符合条件的数据。 
        /// RMA_Revert有多种状态，比如待发还、已返还、作废，这里只需要显示待发还的数据。
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        private DataTable GetDeliveryAssignTask4RMARevert(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            //QuerySorterHelper4AssignTaskRMARevert.SetSorterSQL(qcentity.PagingInfo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4RMARevert");

            DataTable result = new DataTable();
            totalCount = 0;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(qcentity.PageInfo),
                "D.SysNO DESC"))
            {
                AddParameter4DeliveryAssignTask4RMARevert(qcentity, command, sqlBuilder);
                result = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }

            if (result != null
                && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    row["OrderType"] = ECCentral.BizEntity.SO.DeliveryType.RMARevert;
                }
            }

            return result;
        }

        /// <summary>
        /// Vendor送修
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        private DataTable GetDeliveryAssignTask4VendorOut(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            //QuerySorterHelper4AssignTaskVendorOut.SetSorterSQL(qcentity.PagingInfo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4VendorOut");

            DataTable result = new DataTable();
            totalCount = 0;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(qcentity.PageInfo),
                "D.SysNO DESC"))
            {
                AddParameter4DeliveryAssignTask4VendorOut(qcentity, command, sqlBuilder);
                result = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }

            if (result != null
                && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    row["OrderType"] = ECCentral.BizEntity.SO.DeliveryType.VendorMend;
                }
                ProcessRMAControlNOs(result, qcentity.CompanyCode);
            }

            return result;
        }

        private void ProcessRMAControlNOs(DataTable items, string companyCode)
        {
            if (items != null
                && items.Rows.Count > 0)
            {
                List<string> tmpOutBoundSysNos = new List<string>();
                foreach (DataRow row in items.Rows)
                {
                    tmpOutBoundSysNos.Add(Convert.ToString(row["SysNo"]));
                }

                DataTable tmpQueryResut4RMAC = GetDeliveryAssignTaskRMAControl(tmpOutBoundSysNos, companyCode);
                if (tmpQueryResut4RMAC != null
                    && tmpQueryResut4RMAC.Rows.Count > 0)
                {
                    if (!items.Columns.Contains("RMAControlNos"))
                    {
                        items.Columns.Add("RMAControlNos", typeof(string));
                    }

                    foreach (DataRow x in items.Rows)
                    {
                        foreach (DataRow y in tmpQueryResut4RMAC.Rows)
                        {
                            if (string.Equals(x["SysNo"],y["OutBoundSysNO"]))
                            {
                                x["RMAControlNos"] = Convert.ToString(x["RMAControlNos"]) + string.Format("{0},", Convert.ToString(y["RegistersysNo"]));
                            }
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(x["RMAControlNos"])))
                        {
                            x["RMAControlNos"] = Convert.ToString(x["RMAControlNos"]).TrimEnd(",".ToCharArray());
                        }
                    }
                }
            }
        }

        private DataTable GetDeliveryAssignTaskRMAControl(List<string> OutBoundSysNos, string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTaskRMAControl");

            DataTable result = new DataTable();

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                null,
                "OutBoundSysNO DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);


                AddParameter4DeliveryAssignTask4RMAControl(OutBoundSysNos, command, sqlBuilder);
                result = command.ExecuteDataTable();
            }
            return result;
        }

        private static void AddParameter4DeliveryAssignTask4RMAControl(List<string> OutBoundSysNos, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            //sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "OutBoundSysNO", System.Data.DbType.Int32, OutBoundSysNos);

            if (OutBoundSysNos != null
                && OutBoundSysNos.Count > 0)
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("OutBoundSysNO IN ({0})", string.Join(",", OutBoundSysNos.ToArray())));
            }

            command.CommandText = sqlBuilder.BuildQuerySql();
        }

        /// <summary>
        /// Vendor送修返还
        /// </summary>
        /// <param name="qcentity"></param>
        /// <returns></returns>
        private DataTable GetDeliveryAssignTask4VendorIn(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            //QuerySorterHelper4AssignTaskVendorIn.SetSorterSQL(qcentity.PagingInfo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetDeliveryAssignTask4VendorIn");

            DataTable result = new DataTable();
            totalCount = 0;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(qcentity.PageInfo),
                "D.SysNO DESC"))
            {
                AddParameter4DeliveryAssignTask4VendorIn(qcentity, command, sqlBuilder);
                result = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            }

            if (result != null
                && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    row["OrderType"] = ECCentral.BizEntity.SO.DeliveryType.VendorReturn;
                }
                ProcessRMAControlNOs(result, qcentity.CompanyCode);
            }

            return result;
        }

        private DataTable GetDeliveryAssignTask4RMAAll(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            string sortField = string.Empty;
            string sorter = string.Empty;
            if (!string.IsNullOrEmpty(qcentity.PageInfo.SortBy))
            {
                sortField = qcentity.PageInfo.SortBy.ToLower().Split(" ".ToCharArray())[0].Trim();
                sorter = qcentity.PageInfo.SortBy.ToLower().Split(" ".ToCharArray())[1].Trim();
            }

            int startRowIndex = qcentity.PageInfo.PageIndex.Value;
            int pageSize = qcentity.PageInfo.PageSize.Value;
            int sumTotalCount = 0;
            totalCount = 0;

            qcentity.PageInfo.PageSize = int.MaxValue;
            qcentity.PageInfo.PageIndex = 0;

            //分别取出不同单据的记录并整合到一起
            DataTable result = new DataTable();
            DataTable queryAllList = new DataTable();

            DataTable query4RMARequest;
            DataTable query4RMARevert;
            DataTable query4VendorIn;
            DataTable query4VendorOut;

            int tempInt = 0;
            query4RMARequest = GetDeliveryAssignTask4RMARequest(qcentity, out tempInt);
            sumTotalCount += tempInt;

            if (query4RMARequest != null && query4RMARequest.Rows.Count > 0)
            {
                foreach (DataRow row in query4RMARequest.Rows)
                {
                    queryAllList.Rows.Add(row);
                }
            }

            tempInt = 0;
            query4RMARevert = GetDeliveryAssignTask4RMARevert(qcentity, out tempInt);
            sumTotalCount += tempInt;

            if (query4RMARevert != null && query4RMARevert.Rows.Count > 0)
            {
                foreach (DataRow row in query4RMARevert.Rows)
                {
                    queryAllList.Rows.Add(row);
                }
            }

            tempInt = 0;
            query4VendorIn = GetDeliveryAssignTask4VendorIn(qcentity, out tempInt);
            sumTotalCount += tempInt;

            if (query4VendorIn != null && query4VendorIn.Rows.Count > 0)
            {
                foreach (DataRow row in query4VendorIn.Rows)
                {
                    queryAllList.Rows.Add(row);
                }
            }

            tempInt = 0;
            query4VendorOut = GetDeliveryAssignTask4VendorOut(qcentity, out tempInt);
            sumTotalCount += tempInt;

            if (query4VendorOut != null && query4VendorOut.Rows.Count > 0)
            {
                foreach (DataRow row in query4VendorOut.Rows)
                {
                    queryAllList.Rows.Add(row);
                }
            }

            //按指定属性排序
            DataTable queryAllSortList = queryAllList.Copy();
            DataView dv = queryAllList.DefaultView;
            dv.Sort = qcentity.PageInfo.SortBy;
            queryAllSortList = dv.ToTable();

            totalCount = sumTotalCount;

            result = new DataTable();
            //分检出指定页的记录
            if (startRowIndex < queryAllSortList.Rows.Count)
            {
                for (int i = startRowIndex; i < (startRowIndex + pageSize); i++)
                {
                    if (i >= queryAllSortList.Rows.Count)
                        break;

                    result.Rows.Add(queryAllSortList.Rows[i]);
                }
            }

            return result;
        }

        private DataTable GetDeliveryAssignTask4All(SODeliveryAssignTaskQueryFilter qcentity, out int totalCount)
        {
            string sortField = string.Empty;
            string sorter = string.Empty;
            if (!string.IsNullOrEmpty(qcentity.PageInfo.SortBy))
            {
                sortField = qcentity.PageInfo.SortBy.ToLower().Split(" ".ToCharArray())[0].Trim();
                sorter = qcentity.PageInfo.SortBy.ToLower().Split(" ".ToCharArray())[1].Trim();
            }

            int startRowIndex = qcentity.PageInfo.PageIndex.Value;
            int pageSize = qcentity.PageInfo.PageSize.Value;
            int sumTotalCount = 0;
            totalCount = 0;

            qcentity.PageInfo.PageIndex = int.MaxValue;
            qcentity.PageInfo.PageIndex = 0;

            //分别取出不同单据的记录并整合到一起
            DataTable result = new DataTable();
            DataTable queryAllList = new DataTable();

            DataTable query4SO;
            DataTable query4RMARequest;
            DataTable query4RMARevert;
            DataTable query4VendorIn;
            DataTable query4VendorOut;

            int tempInt = 0;
            query4SO = GetDeliveryAssignTask4SO(qcentity, out tempInt);
            sumTotalCount += tempInt;

            queryAllList.Merge(query4SO, true, MissingSchemaAction.Add);

            tempInt = 0;
            query4RMARequest = GetDeliveryAssignTask4RMARequest(qcentity, out tempInt);
            sumTotalCount += tempInt;

            queryAllList.Merge(query4RMARequest, true, MissingSchemaAction.Add);

            tempInt = 0;
            query4RMARevert = GetDeliveryAssignTask4RMARevert(qcentity, out tempInt);
            sumTotalCount += tempInt;

            queryAllList.Merge(query4RMARevert, true, MissingSchemaAction.Add);

            tempInt = 0;
            query4VendorIn = GetDeliveryAssignTask4VendorIn(qcentity, out tempInt);
            sumTotalCount += tempInt;

            queryAllList.Merge(query4VendorIn, true, MissingSchemaAction.Add);

            tempInt = 0;
            query4VendorOut = GetDeliveryAssignTask4VendorOut(qcentity, out tempInt);
            sumTotalCount += tempInt;

            queryAllList.Merge(query4VendorOut, true, MissingSchemaAction.Add);

            //按指定属性排序
            DataTable queryAllSortList = queryAllList.Copy();
            DataView dv = queryAllList.DefaultView;
            dv.Sort = qcentity.PageInfo.SortBy;
            queryAllSortList = dv.ToTable();

            totalCount = sumTotalCount;

            foreach (DataColumn dc in queryAllSortList.Columns)
            {
                result.Columns.Add(new DataColumn(dc.ColumnName, dc.DataType));
            }

            //分检出指定页的记录
            if (startRowIndex < queryAllSortList.Rows.Count)
            {
                object[] objResult = new object[queryAllSortList.Columns.Count];
                for (int i = startRowIndex; i < (startRowIndex + pageSize); i++)
                {
                    if (i >= queryAllSortList.Rows.Count)
                        break;

                    queryAllSortList.Rows[i].ItemArray.CopyTo(objResult, 0);
                    result.Rows.Add(objResult);
                }
            }
            return result;
        }

        private void AddParameter4DeliveryAssignTask4SO(SODeliveryAssignTaskQueryFilter qcentity, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, qcentity.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 1);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, qcentity.OrderSysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, qcentity.DeliveryTime);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, qcentity.DeliveryTimeRange);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, qcentity.DeliveryMan);

            if (qcentity.Area.HasValue)
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.AreaSysNo", System.Data.DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", System.Data.DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", System.Data.DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.OutTime", DbType.DateTime, "@OutTime1", QueryConditionOperatorType.MoreThanOrEqual, qcentity.OutStockTimeFrom);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.OutTime", DbType.DateTime, "@OutTime2", QueryConditionOperatorType.LessThanOrEqual, qcentity.OutStockTimeTo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SM.PayTypeSysNo", DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, qcentity.PayType);

            sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "SM.SysNo", System.Data.DbType.Int32, qcentity.SOSysNos);

            command.CommandText = sqlBuilder.BuildQuerySql();
        }

        private void AddParameter4DeliveryAssignTask4RMARequest(SODeliveryAssignTaskQueryFilter qcentity, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, qcentity.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 3);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RR.Status", DbType.Int32, "@RRStatus1", QueryConditionOperatorType.NotEqual, -1);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RR.Status", DbType.Int32, "@RRStatus2", QueryConditionOperatorType.NotEqual, 2);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, qcentity.OrderSysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, qcentity.DeliveryTime);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, qcentity.DeliveryTimeRange);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, qcentity.DeliveryMan);

            if (qcentity.Area.HasValue)
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.AreaSysNo", System.Data.DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", System.Data.DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", System.Data.DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }

            sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "RR.SOSysNo", System.Data.DbType.Int32, qcentity.SOSysNos);

            command.CommandText = sqlBuilder.BuildQuerySql();
        }

        private void AddParameter4DeliveryAssignTask4RMARevert(SODeliveryAssignTaskQueryFilter qcentity, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, qcentity.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 6);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, qcentity.OrderSysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, qcentity.DeliveryTime);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, qcentity.DeliveryTimeRange);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, qcentity.DeliveryMan);

            if (qcentity.Area.HasValue)
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.AreaSysNo", System.Data.DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", System.Data.DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", System.Data.DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }

            sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "RR.SOSysNo", System.Data.DbType.Int32, qcentity.SOSysNos);

            command.CommandText = sqlBuilder.BuildQuerySql();

        }

        private void AddParameter4DeliveryAssignTask4VendorOut(SODeliveryAssignTaskQueryFilter qcentity, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, qcentity.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 4);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, qcentity.OrderSysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, qcentity.DeliveryTime);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, qcentity.DeliveryTimeRange);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, qcentity.DeliveryMan);

            if (qcentity.Area.HasValue)
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.AreaSysNo", System.Data.DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", System.Data.DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", System.Data.DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }

            command.CommandText = sqlBuilder.BuildQuerySql();
        }

        private void AddParameter4DeliveryAssignTask4VendorIn(SODeliveryAssignTaskQueryFilter qcentity, CustomDataCommand command, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, qcentity.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, 5);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, qcentity.OrderSysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, qcentity.DeliveryTime);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, qcentity.DeliveryTimeRange);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, qcentity.DeliveryMan);

            if (qcentity.Area.HasValue)
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.AreaSysNo", System.Data.DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysNo", System.Data.DbType.Int32, "@CitySysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysNo", System.Data.DbType.Int32, "@ProvinceSysNo", QueryConditionOperatorType.Equal, qcentity.Area);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }

            command.CommandText = sqlBuilder.BuildQuerySql();
        }

        #endregion

        /// <summary>
        /// OPC查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable OPCOfflineMasterQuery(OPCQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_QueryOPCOfflineMaster");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(filter.PagingInfo),
                "master.TransactionNumber DESC"))
            {
                if (filter.ActionType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "master.[ActionType]",
                          DbType.String,
                         "@ActionType",
                         QueryConditionOperatorType.Equal,
                         (char)filter.ActionType.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "cust.[CustomerID]",
                      DbType.String,
                     "@CustomerID",
                     QueryConditionOperatorType.Equal,
                     filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "master.[Indate]",
                      DbType.DateTime,
                     "@CreateDateBegin",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CreateTimeFrom);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "master.[CompanyCode]",
                    DbType.String,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                   filter.CompanyCode);

                if (filter.CreateTimeTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "master.[Indate]",
                          DbType.DateTime,
                         "@CreateDateEnd",
                         QueryConditionOperatorType.LessThanOrEqual,
                         filter.CreateTimeTo.Value.AddDays(1));
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "master.[SONumber]",
                      DbType.Int32,
                     "@SONumber",
                     QueryConditionOperatorType.Equal,
                     filter.SONumber);

                if (filter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "master.[Status]",
                          DbType.String,
                         "@Status",
                         QueryConditionOperatorType.Equal,
                         (char)filter.Status.Value);
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                var dt = command.ExecuteDataTable();

                //替换枚举列和CodeNamePair列
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("ActionType", typeof(WMSAction));
                enumColList.Add("Status", typeof(OPCStatus));
                //enumColList.Add("NeedResponse", typeof(SYNStatus));

                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();

                command.ConvertColumn(dt, enumColList, codeNameList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 特殊分仓订单查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable SpecialSOQuery(SpecialSOSearchQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_SpecialSO");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(filter.PageInfo),
                "sm.[OrderDate] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "OrderDate",
                            DbType.DateTime,
                            "@StartDate",
                            QueryConditionOperatorType.MoreThanOrEqual,
                            filter.StartDate);

                sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "OrderDate",
                            DbType.DateTime,
                            "@EndDate",
                            QueryConditionOperatorType.LessThan,
                            filter.EndDate);
                if (filter.SOStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.AND,
                                "sm.Status",
                                DbType.Int32,
                                "@SOStatus",
                                QueryConditionOperatorType.Equal,
                                (int)filter.SOStatus.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                           QueryConditionRelationType.AND,
                           "sm.CompanyCode",
                           DbType.String,
                           "@CompanyCode",
                           QueryConditionOperatorType.Equal,
                           filter.CompanyCode);

                #region Inner SQL Script
                string innerSQL = @" si.ProductSysNo in  (
	                                Select   Distinct V.itemsysnumber From 
	                                ( Select  itemsysnumber  From   @VTableName  Inv With(NoLock)
	                                Where   (IsNull(AccountQty,0)+IsNull(ConsignQty,0))=0 And  IsNull(VirtualQty,0)>0  @VStockSysNo
	                                ) V,
	                                ( Select  itemsysnumber  From  @NVTableName  Inv With(NoLock)
	                                Where  (IsNull(AccountQty,0)+IsNull(ConsignQty,0))>0 And  IsNull(VirtualQty,0)<=0  @NVStockSysNo
	                                ) NV
	                                Where V.itemsysnumber=NV.itemsysnumber
	                                )
	                                And si.ProductType != 3";
                //string innerSQL = " 1 = 1 "; 
                #endregion

                if (!filter.StockV.HasValue)
                {
                    innerSQL = innerSQL.Replace("@VTableName", "OverseaInventoryManagement.dbo.V_INM_Inventory");
                    innerSQL = innerSQL.Replace("@VStockSysNo", "");
                }
                else
                {
                    innerSQL = innerSQL.Replace("@VTableName", "OverseaInventoryManagement.dbo.V_INM_Inventory_Stock");
                    innerSQL = innerSQL.Replace("@VStockSysNo", " And warehousesysnumber=" + filter.StockV.Value.ToString());
                }

                if (!filter.StockNV.HasValue)
                {
                    innerSQL = innerSQL.Replace("@NVTableName", "OverseaInventoryManagement.dbo.V_INM_Inventory");
                    innerSQL = innerSQL.Replace("@NVStockSysNo", "");
                }
                else
                {
                    innerSQL = innerSQL.Replace("@NVTableName", "OverseaInventoryManagement.dbo.V_INM_Inventory_Stock");
                    innerSQL = innerSQL.Replace("@NVStockSysNo", " And warehousesysnumber=" + filter.StockNV.Value.ToString());
                }

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, innerSQL);

                command.CommandText = sqlBuilder.BuildQuerySql();

                command.CommandText = command.CommandText.Replace("#extendWhere#", " WHERE " + innerSQL);

                var dt = command.ExecuteDataTable("SOStatus", typeof(SOStatus));
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询运单
        /// </summary>
        /// <param name="filter">SOPackageCoverSearchFilter</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable PackageCoverSearchQuery(SOPackageCoverSearchFilter filter, out int totalCount)
        {

            return PackageCoverSearchQuery(filter, out totalCount, true);
        }

        public DataTable PackageCoverSearchQuery(SOPackageCoverSearchFilter filter, out int totalCount, bool isNeedChange)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPackageCover");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SONumber DESC"))
            {
                PackageCoverSearchParameter(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                //替换枚举列
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("SignStatus", typeof(PackageSignStatus));
                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                cmd.ConvertColumn(dt, enumColList, codeNameList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        }

        private void PackageCoverSearchParameter(SOPackageCoverSearchFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {

            /// <summary>
            /// 销售订单编号
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SONumber",
                     DbType.Int32,
                    "@SONumber",
                    QueryConditionOperatorType.Equal,
                    filter.SONumber);

            /// <summary>
            /// 包裹单号
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "TrackingNumber",
                DbType.String,
                "@TrackingNumber",
                QueryConditionOperatorType.Equal,
                filter.TrackingNumber);

            /// <summary>
            /// 配送方式
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "SM.ShipTypeSysNo",
                 DbType.Int32,
                 "@ShipTypeSysNo",
                 QueryConditionOperatorType.Equal,
                 filter.ShipTypeSysNo
                 );


            /// <summary>
            /// 仓库
            /// </summary>
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "WareHouseNumber",
                  DbType.Int32,
                  "@WareHouseNumber",
                  QueryConditionOperatorType.Equal,
                  filter.WareHouseNumber);

            /// <summary>
            /// 出库日期(从)
            /// </summary>
            sb.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "SM.OutTime",
               DbType.DateTime,
               "@ShippedOutTimeFrom",
               QueryConditionOperatorType.MoreThanOrEqual,
               filter.ShippedOutTimeFrom
               );

            /// <summary>
            /// 出库日期(至)
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SM.OutTime",
                DbType.DateTime,
                "@ShippedOutTimeTo",
                QueryConditionOperatorType.LessThan,
                filter.ShippedOutTimeTo
                );

            /// <summary>
            /// 签收状态
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                     QueryConditionRelationType.AND,
                     "SignStatus",
                     DbType.Int32,
                     "@SignStatus",
                     QueryConditionOperatorType.Equal,
                     filter.SignStatus
                     );

            /// <summary>
            /// 送货区域
            /// </summary>
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "ReceiveAreaSysNo",
                DbType.Int32,
                "@ReceiveAreaSysNo",
                QueryConditionOperatorType.Equal,
                filter.ReceiveAreaSysNo);
            cmd.CommandText = sb.BuildQuerySql();
        }

        /// <summary>
        /// 查询手动更改仓库信息订单
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">查询记录数</param>
        /// <returns></returns>
        public DataTable WHUpdateQuery(SOWHUpdateQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_WHUpdate");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SI.SysNo DESC"))
            {
                AddWHUpdateParameter(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 添加手动更改仓库信息的查询参数
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cmd"></param>
        /// <param name="sb"></param>
        private void AddWHUpdateParameter(SOWHUpdateQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SI.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SI.SOSysNo",
                DbType.String,
                "@SOSysNo",
                QueryConditionOperatorType.Equal,
                filter.SOSysNo
                );
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SI.ProductSysNo",
                DbType.Int32,
                "@ProductSysNo",
                QueryConditionOperatorType.Equal,
                filter.ProductSysNo
                );
            cmd.CommandText = sb.BuildQuerySql();
        }

        /// <summary>
        /// 生成出库单查询
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="totalCount">总共的条数</param>
        /// <returns>数据集合</returns>
        public DataTable OutStock4FinanceQuery(SOOutStock4FinanceQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_OutStock");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(filter.PageInfo),
                "SM.SysNo "))
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                sb1.AppendFormat("WHERE SM.Status = 1 And SM.CompanyCode='{0}'", filter.CompanyCode);
                sb2.AppendFormat("WHERE 1=1 AND SM.CompanyCode='{0}'", filter.CompanyCode);

                if (filter.DeliveryDateTime.HasValue
                    && filter.DeliveryDateTime.Value.Year > 1)
                {
                    if (filter.DeliveryTimeRange.HasValue)
                    {
                        sb1.AppendFormat(" AND (SM.DeliveryDate < '{0}' OR (SM.DeliveryDate = '{0}' AND SM.DeliveryTimeRange <= {1}))", filter.DeliveryDateTime, filter.DeliveryTimeRange);
                    }
                    else
                    {
                        sb1.AppendFormat(" AND SM.DeliveryDate <= '{0}'", filter.DeliveryDateTime);
                    }
                }

                if (filter.ShipTypeSysNo.HasValue)
                {
                    sb1.Append(" AND SM.ShipTypeSysno")
                        .Append(filter.ShipTypeCondition == ConditionType.Equal ? "=" : "<>")
                        .Append(filter.ShipTypeSysNo);
                }

                if (filter.StockSysNo.HasValue)
                {
                    sb1.AppendFormat(" AND SI.WarehouseNumber = {0}", filter.StockSysNo);
                }

                if (filter.ISVAT.HasValue)
                {
                    sb1.AppendFormat(" AND SM.IsVat = {0}", (int)filter.ISVAT.Value);
                }


                if (filter.ReceiveAreaSysNo.HasValue && filter.ReceiveAreaSysNo > 0)
                {
                    sb2.AppendFormat(" AND ( SM.ReceiveAreaSysNo = {0} OR A.CitySysno = {0} OR A.ProvinceSysno = {0})", filter.ReceiveAreaSysNo);
                }

                if (filter.SpecialSOType.HasValue)
                {
                    if (filter.SpecialSOType.Value == SOIsSpecialOrder.Normal)
                        sb2.AppendFormat(" AND (S.SpecialSOType = {0} OR S.SpecialSOType IS Null)", (int)SOIsSpecialOrder.Normal);
                    else
                        sb2.AppendFormat(" AND S.SpecialSOType = {0}", filter.SpecialSOType);
                }

                if (filter.EnoughFlag.HasValue)
                {
                    sb2.AppendFormat(" AND TSM.IsEnough = {0}", filter.EnoughFlag);
                }

                command.CommandText = sqlBuilder.BuildQuerySql();

                command.CommandText = command.CommandText.Replace("#WHERE01#", sb1.ToString());
                command.CommandText = command.CommandText.Replace("#WHERE02#", sb2.ToString());

                var dt = command.ExecuteDataTable();
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(SOStatus));
                enumColList.Add("PayStatus", typeof(PayableStatus));
                enumColList.Add("IsEnough", typeof(EnoughFlag));
                enumColList.Add("DeliveryTimeRange", typeof(ECCentral.BizEntity.SO.DeliveryTimeRange));

                CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();
                //codenameColList.Add("DeliveryTimeRange", "Common", "TimeRange");

                command.ConvertColumn(dt, enumColList, codenameColList);
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return dt;
            }
        }

        ///// <summary>
        ///// 第三方订单查询   
        ///// </summary>
        ///// <param name="filter">SOThirdPartSOSearchFilter</param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>       
        public DataTable ThirdPartSOSearchQuery(SOThirdPartSOSearchFilter filter, out int totalCount)
        {
            return ThirdPartSOSearchQuery(filter, out totalCount, true);
        }

        public DataTable ThirdPartSOSearchQuery(SOThirdPartSOSearchFilter filter, out int totalCount, bool isNeedChange)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetThirdSOMappingList");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "OrderID DESC"))
            {
                ThirdPartSOSearchParameter(filter, cmd, sb);
                var dt = cmd.ExecuteDataTable();
                //替换枚举列
                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                codeNameList.Add("CreateResult", "SO", "CreateResultType");
                codeNameList.Add("StatusSyncResult", "SO", "StatusSyncResultType");
                cmd.ConvertColumn(dt, null, codeNameList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void ThirdPartSOSearchParameter(SOThirdPartSOSearchFilter query, CustomDataCommand cmd, DynamicQuerySqlBuilder builder)
        {
            if (query.ShippedOutTimeTo.HasValue)
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "tps.InDate"
                    , DbType.DateTime
                    , "@CreateDateEnd"
                    , QueryConditionOperatorType.LessThan
                    , query.ShippedOutTimeTo.Value.AddDays(1));
            }
            if (query.ShippedOutTimeFrom.HasValue)
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "tps.InDate"
                    , DbType.DateTime
                    , "@CreateDateStart"
                    , QueryConditionOperatorType.MoreThan
                    , query.ShippedOutTimeFrom.Value);
            }
            if (!string.IsNullOrEmpty(query.CreateResult))
            {
                if (query.CreateResult == "N")
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "tps.CreateResult"
                        , DbType.String
                        , "@CreateResult"
                        , QueryConditionOperatorType.IsNull
                        , query.CreateResult);
                }
                else
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "tps.CreateResult"
                        , DbType.String
                        , "@CreateResult"
                        , QueryConditionOperatorType.Equal
                        , query.CreateResult);
                }
            }

            if (!string.IsNullOrEmpty(query.OrderID))
            {
                string[] arr = query.OrderID.Split('.');
                if (arr.Length > 2000)
                {
                    arr = arr.Skip(2000).ToArray();
                }
                string value = string.Format("{0}{1}{0}", "'", Contract<string>("','", arr));

                builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "tps.OrderID"
                    , QueryConditionOperatorType.In
                    , value);
            }
            if (!string.IsNullOrEmpty(query.SOSysNo))
            {
                string[] arr = query.SOSysNo.Split('.');
                if (arr.Length > 2000)
                {
                    arr = arr.Skip(2000).ToArray();
                }
                string value = Contract<int>(",", arr);

                builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "tps.SOSysNo"
                    , QueryConditionOperatorType.In
                    , value);
            }

            if (!string.IsNullOrEmpty(query.StatusSyncResult))
            {
                if (query.StatusSyncResult == "N")
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "tps.StatusSyncResult"
                    , DbType.String
                    , "@StatusSyncResult"
                    , QueryConditionOperatorType.IsNull
                    , query.StatusSyncResult);
                }
                else
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "tps.StatusSyncResult"
                        , DbType.String
                        , "@StatusSyncResult"
                        , QueryConditionOperatorType.Equal
                        , query.StatusSyncResult);
                }
            }

            if (!string.IsNullOrEmpty(query.Type))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "tps.Type"
                    , DbType.String
                    , "@Type"
                    , QueryConditionOperatorType.Equal
                    , query.Type);
            }
            if (!string.IsNullOrEmpty(query.CompanyCode))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "tps.CompanyCode"
                    , DbType.String
                    , "@CompanyCode"
                    , QueryConditionOperatorType.Equal
                    , query.CompanyCode);
            }
            cmd.CommandText = builder.BuildQuerySql();
        }

        private static string Contract<T>(string split, Array arr)
        {
            if (arr == null && arr.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                string tmp = arr.GetValue(i).ToString().Trim();
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (typeof(T) == typeof(int))
                    {
                        int value = 0;
                        if (int.TryParse(tmp, out value))
                        {
                            sb.AppendFormat("{0}{1}", value.ToString(), split);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1}", tmp, split);
                    }
                }
            }
            if (sb.Length > split.Length)
            {
                sb = sb.Remove(sb.Length - split.Length, split.Length);
            }
            return sb.ToString();
        }

        public DataTable OZZOOriginNoteQuery(DefaultQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_GetOZZOOriginNote");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PagingInfo), "SO.Orderdate DESC"))
            {
                #region 赋值参数

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO.Status",
                DbType.Int32,
                "@Status",
                QueryConditionOperatorType.Equal,
                0);

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "SO.Shiptypesysno",
                DbType.Int32,
                "@Shiptypesysno",
                QueryConditionOperatorType.Equal,
                10);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "SO.CompanyCode",
                    DbType.String,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                #endregion
                var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询 订单拦截设置信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        public DataTable SOInterceptQuery(SOInterceptQueryFilter filter, out int dataCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOInterceptList");
            command.CommandTimeout = 600;
            DataTable result = null;
            #region 查询条件赋值
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText,
                command,
                ToPagingInfo(filter.PagingInfo),
                "h.SysNo DESC"))
            {

                if (!string.IsNullOrEmpty(filter.ShipTypeEnum))
                {
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                               "s.ShipTypeEnum",
                                               QueryConditionOperatorType.In,
                                               filter.ShipTypeEnum
                                               );
                }

                if (!string.IsNullOrEmpty(filter.ShippingTypeID))
                {
                    filter.ShipTypeSysNo = Convert.ToInt32(filter.ShippingTypeID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "s.SysNo",
                                               DbType.Int32,
                                               "@ShipTypeSysNo",
                                               QueryConditionOperatorType.Equal,
                                               filter.ShipTypeSysNo
                                               );

                }

                if (!string.IsNullOrEmpty(filter.StockSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.WareHouseNumber",
                                               DbType.AnsiStringFixedLength,
                                               "@WareHouseNumber",
                                               QueryConditionOperatorType.Equal,
                                               filter.StockSysNo
                                               );

                }

                if (!string.IsNullOrEmpty(filter.HasTrackingNumber))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.HasTrackingNumber",
                                               DbType.AnsiStringFixedLength,
                                               "@HasTrackingNumber",
                                               QueryConditionOperatorType.Equal,
                                               filter.HasTrackingNumber
                                               );

                }

                if (!string.IsNullOrEmpty(filter.ShipTimeType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.ShipTimeType",
                                               DbType.AnsiStringFixedLength,
                                               "@ShipTimeType",
                                               QueryConditionOperatorType.Equal,
                                               filter.ShipTimeType
                                               );

                }

                if (!string.IsNullOrEmpty(filter.EmailAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.EmailAddress",
                                               DbType.AnsiStringFixedLength,
                                               "@EmailAddress",
                                               QueryConditionOperatorType.LeftLike,
                                               filter.EmailAddress
                                               );

                }

                if (!string.IsNullOrEmpty(filter.CCEmailAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.CCEmailAddress",
                                               DbType.AnsiStringFixedLength,
                                               "@CCEmailAddress",
                                               QueryConditionOperatorType.LeftLike,
                                               filter.CCEmailAddress
                                               );

                }


                if (!string.IsNullOrEmpty(filter.FinanceEmailAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.FinanceEmailAddress",
                                               DbType.AnsiStringFixedLength,
                                               "@FinanceEmailAddress",
                                               QueryConditionOperatorType.LeftLike,
                                               filter.FinanceEmailAddress
                                               );

                }

                if (!string.IsNullOrEmpty(filter.FinanceCCEmailAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                               "h.FinanceCCEmailAddress",
                                               DbType.AnsiStringFixedLength,
                                               "@FinanceCCEmailAddress",
                                               QueryConditionOperatorType.LeftLike,
                                               filter.FinanceCCEmailAddress
                                               );

                }
                command.CommandText = sqlBuilder.BuildQuerySql();

                CodeNamePairColumnList codenameColList = new CodeNamePairColumnList();
                codenameColList.Add("HasTrackingNumber", "SO", "HasTrackingNumber");
                codenameColList.Add("ShipTimeType", "SO", "ShipTimeType");

                result = command.ExecuteDataTable(codenameColList);
            }
            #endregion
            dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return result;
        }

        /// <summary>
        /// 配送服务评级
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetDeliveryScoreList(SODeliveryScoreQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_DeliveryScore");

            DataTable result = null;
            totalCount = 0;

            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, ToPagingInfo(filter.PageInfo), " SS.SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SS.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                if (filter.DeliveryManUserSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryManUserSysNo", DbType.Int32, "@DeliveryManUserSysNo", QueryConditionOperatorType.Equal, filter.DeliveryManUserSysNo.Value);

                if (filter.ShipTypeSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "S.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, filter.ShipTypeSysNo.Value);
                if (filter.DeliveryDateFrom.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.DeliveryDateFrom);
                }
                if (filter.DeliveryDateTo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.DeliveryDate", DbType.DateTime, "@DeliveryDateTo", QueryConditionOperatorType.LessThan, filter.DeliveryDateTo.Value.AddDays(1));
                }
                if (filter.OrderSysNo.HasValue)
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "S.SysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, filter.OrderSysNo.Value);

                if (filter.VIPRank.HasValue)
                {
                    if (filter.VIPRank.Value == 0)
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.VIPRank", DbType.Int32, "@VIPRank", QueryConditionOperatorType.Equal, 1);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "C.VIPRank", DbType.Int32, "@VIPRank", QueryConditionOperatorType.Equal, 3);
                        sb.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.VIPRank", DbType.Int32, "@VIPRank", QueryConditionOperatorType.Equal, 2);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "C.VIPRank", DbType.Int32, "@VIPRank", QueryConditionOperatorType.Equal, 4);
                        sb.ConditionConstructor.EndGroupCondition();
                    }
                }
                if (filter.AreaSysNo.HasValue && filter.AreaSysNo != INT_NULL)
                {
                    sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.AreaSysNo", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.CitySysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.AreaSysNo.Value);
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.ProvinceSysno", DbType.Int32, "@AreaSysNo", QueryConditionOperatorType.Equal, filter.AreaSysNo.Value);
                    sb.ConditionConstructor.EndGroupCondition();
                }
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);

                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            }

            return result;
        }

        #endregion

        #region 非Filter的查询,暂定为NoBiz

        /// <summary>
        /// 根据SO订单获取BackOrderItem
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>查询结果</returns>
        public DataTable QueryBackOrderItem(int soSysNo)
        {
            if (soSysNo == 0) return null;
            DataCommand cmd = DataCommandManager.GetDataCommand("SO_Query_BackOrderItem");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 根据订单查询采购商品数量
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public DataTable QueryCheckItemAccountQty(int soSysNo)
        {
            if (soSysNo == 0) return null;
            DataCommand cmd = DataCommandManager.GetDataCommand("SO_Query_CheckItemAccountQty");
            cmd.SetParameterValue("@SOSysNO", soSysNo);
            return cmd.ExecuteDataTable();
        }

        /// <summary>
        /// 查询商品的分仓OrderQty中的未支付数量
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="stockSysNo">仓库号</param>
        /// <returns>未支付数量</returns>
        public int QueryCalUnPayOrderQty(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SO_Query_CalUnPayOrderQty");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            return cmd.ExecuteScalar<int>();
        }

        #endregion
    }
}
