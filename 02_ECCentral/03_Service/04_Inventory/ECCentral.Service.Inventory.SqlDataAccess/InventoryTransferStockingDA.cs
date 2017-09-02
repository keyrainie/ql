using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IInventoryTransferStockingDA))]
    public class InventoryTransferStockingDA : IInventoryTransferStockingDA
    {
        #region IInventoryTransferStockingDA Members

        /// <summary>
        /// 备货中心查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ProductCenterItemInfo> QueryInventoryTransferStockingList(QueryFilter.Inventory.InventoryTransferStockingQueryFilter queryCriteria, out int totalCount)
        {
            if (queryCriteria == null)
            {
                totalCount = 0;
                return null;
            }

            DataTable dt = new DataTable();

            List<ProductCenterItemInfo> resultList = new List<ProductCenterItemInfo>();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryCriteria.PageInfo.SortBy,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize,
                MaximumRows = queryCriteria.PageInfo.PageSize
            };

            if (queryCriteria.IsSortByAsc)
            {
                pagingInfo.SortField = queryCriteria.SortByField + " ASC";
            }
            else
            {
                pagingInfo.SortField = queryCriteria.SortByField + " DESC";
            }         
 
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySuggestTransferAll");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "P.[SysNo] DESC"))
            {

                if (!string.IsNullOrEmpty(queryCriteria.SysNO))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[SysNo]",
                                    DbType.Int32, "@ItemSysNo",
                                    QueryConditionOperatorType.Equal,
                                    Convert.ToInt32((queryCriteria.SysNO)));
                }

                if (!string.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductID]",
                                         DbType.String, "@ItemNumber",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.ProductID);
                }
                if (!string.IsNullOrEmpty(queryCriteria.ProductName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductTitle]",
                                         DbType.String, "@ItemName",
                                         QueryConditionOperatorType.Like,
                                         queryCriteria.ProductName);
                }
                if (queryCriteria.ProductStatus.HasValue)
                {
                    if (queryCriteria.ProductStatusCompareCode == "<>")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]",
                                             DbType.Int32, "@Status",
                                             QueryConditionOperatorType.NotEqual,
                                             (int)queryCriteria.ProductStatus);
                    }
                    else if (queryCriteria.ProductStatusCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]",
                                               DbType.Int32, "@Status",
                                               QueryConditionOperatorType.Equal,
                                               (int)queryCriteria.ProductStatus);
                    }
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category1SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C2.[C1SysNo]",
                                         DbType.Int32, "@C1",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.Category1SysNo);
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category2SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C3.[C2SysNo]",
                                         DbType.Int32, "@C2",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.Category2SysNo);
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category3SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[C3SysNo]",
                                         DbType.Int32, "@C3",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.Category3SysNo);
                }

                if (queryCriteria.ProductType.HasValue)
                {
                    int getProductType = 0;
                    switch (queryCriteria.ProductType.Value)
                    {
                        case ProductType.Normal:
                            getProductType = 0;
                            break;
                        case ProductType.OpenBox:
                            getProductType = 1;
                            break;
                        case ProductType.Bad:
                            getProductType = 2;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductType]",
                                          DbType.Int32, "@ProductType",
                                          QueryConditionOperatorType.Equal,
                                          getProductType);
                }
                if (!string.IsNullOrEmpty(queryCriteria.ManufacturerName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.[ManufacturerName]+M.[BriefName]",
                                          DbType.String, "@ManufacturerName",
                                          QueryConditionOperatorType.Like,
                                          queryCriteria.ManufacturerName);
                }
                if (queryCriteria.ProductConsignFlag != "-999")
                {

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[IsConsign]",
                                          DbType.Int32, "@IsConsign",
                                          QueryConditionOperatorType.Equal,
                                          Convert.ToInt32((queryCriteria.ProductConsignFlag)));
                }

                if (!string.IsNullOrEmpty(queryCriteria.BrandName))
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "(b.BrandName_Ch)",
                                      DbType.String, "@BrandName",
                                      QueryConditionOperatorType.LeftLike,
                                      queryCriteria.BrandName);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "(b.BrandName_En)",
                                      DbType.String, "@BrandName",
                                      QueryConditionOperatorType.LeftLike,
                                      queryCriteria.BrandName);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }

                if (!string.IsNullOrEmpty(queryCriteria.VendorName))
                {
                    string subSQLString_VenderName = @"SELECT PLP.ProductSysNo 
			                            FROM IPP3.dbo.Product_LastPOInfo AS PLP
	                                    LEFT JOIN IPP3.dbo.Vendor AS V WITH(NOLOCK)
	                                        ON PLP.LastVendorSysNo = V.SysNo
			                            where V.[VendorName] like @VendorName";

                    dataCommand.AddInputParameter("@VendorName", DbType.String);
                    dataCommand.SetParameterValue("@VendorName", "%" + queryCriteria.VendorName + "%");

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                             QueryConditionOperatorType.In,
                             subSQLString_VenderName);
                }
                #region

                if (queryCriteria.VendorSysNoList != null && queryCriteria.VendorSysNoList.Count > 0)
                {
                    string subSQLString_VendorSysNoList = @"SELECT PLP.ProductSysNo 
                			                            FROM IPP3.dbo.Product_LastPOInfo AS PLP
                	                                    LEFT JOIN IPP3.dbo.Vendor AS V WITH(NOLOCK)
                	                                        ON PLP.LastVendorSysNo = V.SysNo
                			                            where V.[SysNo] In (" + string.Join(",", queryCriteria.VendorSysNoList) + ")";

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                             QueryConditionOperatorType.In,
                             subSQLString_VendorSysNoList);
                }

                if (queryCriteria.VendorSysNoList != null && queryCriteria.VendorSysNoList.Count > 0)
                {
                    string subSQLString_NotVendorSysNoList = @"SELECT SysNo FROM IPP3.dbo.Product AS PT  WITH(NOLOCK) INNER JOIN (SELECT VM.ManufacturerSysNo,VM.C2SysNo,VM.C3SysNo FROM  IPP3.dbo.Vendor_Manufacturer AS VM WITH(NOLOCK) WHERE VM.VendorSysNo In  (" + string.Join(",", queryCriteria.VendorSysNoList) + ") AND CHARINDEX(';'+CAST(DATEPART(weekday ,GetDate())-1 AS VARCHAR)+';',';'+ISNULL(VM.BuyWeekDay,'')+';')<=0 ) AS TM ON PT.Manufacturersysno = TM.Manufacturersysno AND (TM.c3sysno =PT.c3sysno  OR (SELECT SysNo FROM IPP3.dbo.Category3 AS C3 WITH(NOLOCK) WHERE C3.c2sysno =TM.c2sysno AND SysNo=PT.c3sysno)>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                              "P.[SysNo]",
                               QueryConditionOperatorType.NotIn,
                               subSQLString_NotVendorSysNoList);
                }

                if (!string.IsNullOrEmpty(queryCriteria.PMSysNo))
                {

                    string subSQLString_ProductSysNoList = @"SELECT ProductsysNo FROm OverseaContentManagement.dbo.V_CM_ProductLine_Items 
                                                                  WHERE  ProductLineSysNo IN( SELECT ProductLineSysNo
						                                                                      FROM   OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p
						                                                                      WHERE  PMUserSysNo=" + queryCriteria.PMSysNo + "  OR CHARINDEX(';'+CAST(" + queryCriteria.PMSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                             QueryConditionOperatorType.In,
                             subSQLString_ProductSysNoList);
                }
              
                ////如果没有高级权限 则只能访问自己所能访问到的产品线的商品
                if (queryCriteria.AuthorizedPMsSysNumber != "Senior")
                {
                    string subSQLString_ProductSysNoList = @"SELECT ProductsysNo FROm OverseaContentManagement.dbo.V_CM_ProductLine_Items 
                                                                  WHERE  ProductLineSysNo IN( SELECT ProductLineSysNo
						                                                                      FROM   OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p
						                                                                      WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + "  OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                                QueryConditionOperatorType.In,
                                subSQLString_ProductSysNoList);
                }
               
                #endregion

                if (!string.IsNullOrEmpty(queryCriteria.AverageUnitCost))
                {
                    if (queryCriteria.AverageUnitCostCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                              DbType.Decimal, "@UnitCost",
                                              QueryConditionOperatorType.LessThanOrEqual,
                                              Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                    else if (queryCriteria.AverageUnitCostCompareCode == "=")//==
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                            DbType.Decimal, "@UnitCost",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                    else if (queryCriteria.AverageUnitCostCompareCode == ">=") //>=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                    DbType.Decimal, "@UnitCost",
                                    QueryConditionOperatorType.MoreThanOrEqual,
                                   Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.SalePrice))
                {
                    if (queryCriteria.SalePriceCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                            DbType.Decimal, "@CurrentPrice",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                    else if (queryCriteria.SalePriceCompareCode == "=")//==
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                             DbType.Decimal, "@CurrentPrice",
                                             QueryConditionOperatorType.Equal,
                                             Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                    else if (queryCriteria.SalePriceCompareCode == ">=")//>=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                          DbType.Decimal, "@CurrentPrice",
                                          QueryConditionOperatorType.MoreThanOrEqual,
                                          Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.Point))
                {
                    if (queryCriteria.PointCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                            DbType.Int32, "@Point",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.Point));
                    }
                    else if (queryCriteria.PointCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                            DbType.Int32, "@Point",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.Point));
                    }
                    else if (queryCriteria.PointCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                             DbType.Int32, "@Point",
                                             QueryConditionOperatorType.MoreThanOrEqual,
                                               Convert.ToInt32(queryCriteria.Point));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.FinanceQty))
                {
                    if (queryCriteria.FinanceQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                            DbType.Int32, "@AccountQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.FinanceQty));
                    }
                    else if (queryCriteria.FinanceQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                             DbType.Int32, "@AccountQty",
                                             QueryConditionOperatorType.Equal,
                                          Convert.ToInt32(queryCriteria.FinanceQty));
                    }
                    else if (queryCriteria.FinanceQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                             DbType.Int32, "@AccountQty",
                                             QueryConditionOperatorType.MoreThanOrEqual,
                                        Convert.ToInt32(queryCriteria.FinanceQty));

                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.AvailableQty))
                {
                    if (queryCriteria.AvailableQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                            DbType.Int32, "@AvailableQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                    else if (queryCriteria.AvailableQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                            DbType.Int32, "@AvailableQty",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                    else if (queryCriteria.AvailableQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                           DbType.Int32, "@AvailableQty",
                                           QueryConditionOperatorType.MoreThanOrEqual,
                                           Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.OccupiedQty))
                {
                    if (queryCriteria.OccupiedQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                    else if (queryCriteria.OccupiedQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                    else if (queryCriteria.OccupiedQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                          Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.OrderedQty))
                {
                    if (queryCriteria.OrderedQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                                            DbType.Int32, "@OrderQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                    else if (queryCriteria.OrderedQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                                            DbType.Int32, "@OrderQty",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                    else if (queryCriteria.OrderedQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                          DbType.Int32, "@OrderQty",
                          QueryConditionOperatorType.MoreThanOrEqual,
                            Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.VirtualQty))
                {

                    if (queryCriteria.VirtualQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                            DbType.Int32, "@VirtualQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.VirtualQty));
                    }
                    else if (queryCriteria.VirtualQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                           DbType.Int32, "@VirtualQty",
                                           QueryConditionOperatorType.Equal,
                                         Convert.ToInt32(queryCriteria.VirtualQty));
                    }
                    else if (queryCriteria.VirtualQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                           DbType.Int32, "@VirtualQty",
                                           QueryConditionOperatorType.MoreThanOrEqual,
                                          Convert.ToInt32(queryCriteria.VirtualQty));
                    }

                }

                if (!string.IsNullOrEmpty(queryCriteria.ConsignQty))
                {
                    if (queryCriteria.ConsignQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                              Convert.ToInt32(queryCriteria.ConsignQty));
                    }
                    else if (queryCriteria.ConsignQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.Equal,
                                              Convert.ToInt32(queryCriteria.ConsignQty));
                    }
                    else if (queryCriteria.ConsignQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                           Convert.ToInt32(queryCriteria.ConsignQty));
                    }

                }

                if (!string.IsNullOrEmpty(queryCriteria.OnlineQty))
                {
                    if (queryCriteria.OnlineQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                           Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                    else if (queryCriteria.OnlineQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.Equal,
                                          Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                    else if (queryCriteria.OnlineQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                           Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                }

                if (queryCriteria.IsLarge.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                        "P.IsLarge",
                                        DbType.Int32, "@IsLarge",
                                        QueryConditionOperatorType.Equal,
                                       (int)queryCriteria.IsLarge);
                }

                if (!string.IsNullOrEmpty(queryCriteria.StockSysNo) && queryCriteria.StockSysNo != "-999")
                {
                    string subSQLString = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            where stocksysno = @stocksysno_sub and accountqty {0} @accountqty_sub";

                    string subSQLStringNoQty = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            where stocksysno = @stocksysno_sub";

                    dataCommand.AddInputParameter("@stocksysno_sub", DbType.Int32);
                    dataCommand.SetParameterValue("@stocksysno_sub", Int32.Parse(queryCriteria.StockSysNo));
                    if (!string.IsNullOrEmpty(queryCriteria.SubStockQty))
                    {
                        subSQLString = string.Format(subSQLString, queryCriteria.SubStockQtyCompareCode);
                        dataCommand.AddInputParameter("@accountqty_sub", DbType.Int32);
                        dataCommand.SetParameterValue("@accountqty_sub", queryCriteria.SubStockQty);
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "P.[SysNo]",
                                 QueryConditionOperatorType.In,
                                 subSQLString);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "P.[SysNo]",
                                 QueryConditionOperatorType.In,
                                 subSQLStringNoQty);
                    }
                }

                //Add By Ray.L.Xing 添加根据 是否同步库存商品进行查询
                if (queryCriteria.IsAsyncStock.HasValue)
                {
                    //获取库存同步商品List
                    string subSQLSynProcudtList = @"SELECT DISTINCT ProductSysno                       
                                                    FROM   OverseaContentManagement.dbo.V_CM_Product3Party_Mapping   WITH(NOLOCK) 
                                                    WHERE  [Status]='A' ";

                    if ((int)queryCriteria.IsAsyncStock.Value == 1)//是库存同步商品
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "P.[SysNo]",
                                 QueryConditionOperatorType.In,
                                 subSQLSynProcudtList);
                    }
                    else if ((int)queryCriteria.IsAsyncStock.Value == 0)//不是库存同步商品
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "P.[SysNo]",
                                 QueryConditionOperatorType.NotIn,
                                 subSQLSynProcudtList);
                    }
                }

                //Add By Ray.L.Xing 2010年10月21日 增加 根据采购在途库存查询
                if (!string.IsNullOrEmpty(queryCriteria.PurchaseQty))
                {
                    //当前仓 有具体仓库名
                    string subProductsysnoList = string.Empty;
                    if (!string.IsNullOrEmpty(queryCriteria.StockSysNo) && queryCriteria.StockSysNo != "-999")
                    {
                        //[Mark][Alan.X.Luo 硬编码]
                        string tempStr = "SELECT productsysno  FROM (SELECT productsysno ,SUM(PurchaseQty) AS PurchaseQty,StockSysNo From( SELECT productsysno ,PurchaseQty-Quantity AS PurchaseQty,CASE WHEN ipp3.dbo.PO_Master.itstocksysno IS NULL THEN(CASE  WHEN ipp3.dbo.Po_Master.StockSysNo=59 THEN 51  ELSE ipp3.dbo.Po_Master.StockSysNo END)  WHEN ipp3.dbo.PO_Master.itstocksysno=59 THEN 51  ELSE ipp3.dbo.PO_Master.itstocksysno  END AS StockSysNo  FROM ipp3.dbo.Po_Master WITH(NOLOCK)  INNER JOIN ipp3.dbo.PO_Item WITH(NOLOCK)   ON ipp3.dbo.Po_Master.SysNo=ipp3.dbo.PO_Item.POSysNo  AND ipp3.dbo.Po_Master.status IN (1,2,3,-2,5,6) ) a group by productsysno,StockSysNo) As tempInfo";
                        if (queryCriteria.PurchaseQtyCompareCode == "<=")
                        {
                            //subProductsysnoList = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            //                                        where stocksysno = @stocksysnoFroPurchase_sub and PurchaseQty<=@PurchaseQty_sub";
                            tempStr += "  where tempInfo.StockSysNo=@stocksysnoFroPurchase_sub  AND  ISNULL(tempInfo.PurchaseQty,0) <=@PurchaseQty_sub";
                            subProductsysnoList = @tempStr;

                        }
                        else if (queryCriteria.PurchaseQtyCompareCode == "=")
                        {
                            //subProductsysnoList = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            //                                        where stocksysno = @stocksysnoFroPurchase_sub and PurchaseQty=@PurchaseQty_sub";
                            tempStr += "  where tempInfo.StockSysNo=@stocksysnoFroPurchase_sub  AND ISNULL(tempInfo.PurchaseQty,0)=@PurchaseQty_sub";
                            subProductsysnoList = @tempStr;

                        }
                        else if (queryCriteria.PurchaseQtyCompareCode == ">=")
                        {
                            //subProductsysnoList = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            //                                        where stocksysno = @stocksysnoFroPurchase_sub and PurchaseQty>=@PurchaseQty_sub";
                            tempStr += "  where tempInfo.StockSysNo=@stocksysnoFroPurchase_sub  AND ISNULL(tempInfo.PurchaseQty,0)>=@PurchaseQty_sub";
                            subProductsysnoList = @tempStr;
                        }
                        dataCommand.AddInputParameter("@PurchaseQty_sub", DbType.Int32);
                        dataCommand.AddInputParameter("@stocksysnoFroPurchase_sub", DbType.Int32);
                        dataCommand.SetParameterValue("@stocksysnoFroPurchase_sub", Int32.Parse(queryCriteria.StockSysNo));
                        dataCommand.SetParameterValue("@PurchaseQty_sub", Convert.ToInt32(queryCriteria.PurchaseQty));


                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "P.[SysNo]",
                                QueryConditionOperatorType.In,
                                 subProductsysnoList);
                    }
                    else
                    {

                        //以下逻辑为 所有仓 表示 各个仓库 采购在途数量总和  时 的处理逻辑
                        if (queryCriteria.PurchaseQtyCompareCode == "<=")
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ISNULL(INV.PurchaseQty,0)",
                                                DbType.Int32, "@AllPurchaseQty",
                                                QueryConditionOperatorType.LessThanOrEqual,
                                               Convert.ToInt32(queryCriteria.PurchaseQty));
                        }
                        else if (queryCriteria.PurchaseQtyCompareCode == "=")
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ISNULL(INV.PurchaseQty,0)",
                                                DbType.Int32, "@AllPurchaseQty",
                                                QueryConditionOperatorType.Equal,
                                                  Convert.ToInt32(queryCriteria.PurchaseQty));
                        }
                        else if (queryCriteria.PurchaseQtyCompareCode == ">=")
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ISNULL(INV.PurchaseQty,0)",
                                                DbType.Int32, "@AllPurchaseQty",
                                                QueryConditionOperatorType.MoreThanOrEqual,
                                                Convert.ToInt32(queryCriteria.PurchaseQty));
                        }
                    }
                }

                #region 根据 日均销量天数大于零 查询
                if (!string.IsNullOrEmpty(queryCriteria.DaySalesCount))
                {
                    //当前仓 有具体仓库名
                    string subProductsysnoList = string.Empty;
                    if (queryCriteria.StockSysNo != "-999")
                    {
                        string tempStr = "SELECT productsysno FROM OverseaInventoryManagement.dbo.Inventory_Product_AVGDailySales AS tempPAInfo WITH(NOLOCK) ";
                        if (queryCriteria.DaySalesCountCompareCode == "<=")
                        {
                            tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew>@AVGSaledQty_sub";
                            subProductsysnoList = @tempStr;
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)
                            {
                                tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew<>@AVGSaledQty_sub";
                            }
                            else
                            {
                                tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew=@AVGSaledQty_sub";
                            }
                            subProductsysnoList = @tempStr;

                        }
                        else if (queryCriteria.DaySalesCountCompareCode == ">=")
                        {
                            tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew>=@AVGSaledQty_sub";
                            subProductsysnoList = @tempStr;
                        }

                        dataCommand.AddInputParameter("@AVGSaledQty_sub", DbType.Decimal);
                        dataCommand.AddInputParameter("@stocksysnoFroAVGSaledQty_sub", DbType.Int32);

                        dataCommand.SetParameterValue("@stocksysnoFroAVGSaledQty_sub", Int32.Parse(queryCriteria.StockSysNo));
                        dataCommand.SetParameterValue("@AVGSaledQty_sub", Convert.ToInt32(queryCriteria.DaySalesCount));

                        if (queryCriteria.DaySalesCountCompareCode == "<=")//不再那些 大于的里面
                        {
                            sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "P.[SysNo]",
                                                        QueryConditionOperatorType.NotIn,
                                                         subProductsysnoList);
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")//不再那些 大于的里面
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "P.[SysNo]",
                                                       QueryConditionOperatorType.NotIn,
                                                        subProductsysnoList);
                            }
                            else
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "P.[SysNo]",
                                                       QueryConditionOperatorType.In,
                                                        subProductsysnoList);
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) > 0)
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "P.[SysNo]",
                                                       QueryConditionOperatorType.In,
                                                        subProductsysnoList);
                            }
                        }
                    }
                    else
                    {
                        string tempStr = "SELECT productsysno FROM OverseaInventoryManagement.dbo.Inventory_Product_AllStock_AVGDailySales AS tempPAAInfo WITH(NOLOCK) ";
                        if (queryCriteria.DaySalesCountCompareCode == "<=")
                        {
                            tempStr += "  WHERE tempPAAInfo.AllStockAVGDailySales>@AllStockAVGDailySales_sub";
                            subProductsysnoList = @tempStr;
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)
                            {
                                tempStr += "  WHERE tempPAAInfo.AllStockAVGDailySales<>@AllStockAVGDailySales_sub";
                            }
                            else
                            {
                                tempStr += "  WHERE tempPAAInfo.AllStockAVGDailySales=@AllStockAVGDailySales_sub";
                            }
                            subProductsysnoList = @tempStr;

                        }
                        else if (queryCriteria.DaySalesCountCompareCode == ">=")
                        {
                            tempStr += "  WHERE tempPAAInfo.AllStockAVGDailySales>=@AllStockAVGDailySales_sub";
                            subProductsysnoList = @tempStr;
                        }
                        dataCommand.AddInputParameter("@AllStockAVGDailySales_sub", DbType.Decimal);
                        dataCommand.SetParameterValue("@AllStockAVGDailySales_sub", Convert.ToInt32(queryCriteria.DaySalesCount));
                        if (queryCriteria.DaySalesCountCompareCode == "<=")//不在那些 大于的里面
                        {
                            sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "P.[SysNo]",
                                                        QueryConditionOperatorType.NotIn,
                                                         subProductsysnoList);
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")
                        {

                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)//不在那些 不等于零的里面
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "P.[SysNo]",
                                                        QueryConditionOperatorType.NotIn,
                                                         subProductsysnoList);
                            }
                            else
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "P.[SysNo]",
                                                       QueryConditionOperatorType.In,
                                                        subProductsysnoList);
                            }
                        }
                        else
                        {

                            if (Convert.ToInt32(queryCriteria.DaySalesCount) > 0)
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "P.[SysNo]",
                                                        QueryConditionOperatorType.In,
                                                         subProductsysnoList);
                            }

                        }
                    }
                }
                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                // dt = dataCommand.ExecuteDataTable();
                resultList = dataCommand.ExecuteEntityList<ProductCenterItemInfo>();
                //totalCount = resultList.Count;
                //resultList = resultList.Skip(queryCriteria.PageInfo.PageIndex.Value * queryCriteria.PageInfo.PageSize.Value).Take(queryCriteria.PageInfo.PageSize.Value).ToList();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            }
            resultList = SuggestTransferCacl(resultList, queryCriteria);

            #region 根据 建议备货数量查询条件 来过滤对应的 数据

            if (!string.IsNullOrEmpty(queryCriteria.RecommendBackQty) && Convert.ToInt32(queryCriteria.RecommendBackQty) >= 0 && resultList != null && resultList.Count > 0)
            {
                //当前仓 有具体仓库名              
                if (queryCriteria.StockSysNo != "-999")
                {
                    List<ProductCenterItemInfo> FilterResult = new List<ProductCenterItemInfo>();
                    ProductCenterItemInfo[] SuggestTransferAllEntityArray = new ProductCenterItemInfo[resultList.Count];
                    resultList.CopyTo(SuggestTransferAllEntityArray);
                    FilterResult = SuggestTransferAllEntityArray.ToList<ProductCenterItemInfo>();
                    if (queryCriteria.RecommendBackQtyCompareCode == "<=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    if (InnerItem.SuggestQty > Convert.ToInt32(queryCriteria.RecommendBackQty))
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else if (queryCriteria.RecommendBackQtyCompareCode == "=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    if (Convert.ToInt32(queryCriteria.RecommendBackQty) != InnerItem.SuggestQty)
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else if (queryCriteria.RecommendBackQtyCompareCode == ">=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    if (InnerItem.SuggestQty < Convert.ToInt32(queryCriteria.RecommendBackQty))
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    resultList = FilterResult;
                }
                else
                {
                    if (queryCriteria.RecommendBackQtyCompareCode == "<=")
                    {
                        resultList = resultList.FindAll(x => { return x.SuggestQtyAll <= Convert.ToInt32(queryCriteria.RecommendBackQty); });
                    }
                    else if (queryCriteria.RecommendBackQtyCompareCode == "=")
                    {
                        resultList = resultList.FindAll(x => { return x.SuggestQtyAll == Convert.ToInt32(queryCriteria.RecommendBackQty); });
                    }
                    else if (queryCriteria.RecommendBackQtyCompareCode == ">=")
                    {
                        resultList = resultList.FindAll(x => { return x.SuggestQtyAll >= Convert.ToInt32(queryCriteria.RecommendBackQty); });
                    }
                }
            }

            #endregion


            #region 根据 可销售天数大于零 来过滤 商品
            if (!string.IsNullOrEmpty(queryCriteria.AvailableSaleDays) && Convert.ToInt32(queryCriteria.AvailableSaleDays) >= 0 && resultList.Count > 0)
            {
                //当前仓 有具体仓库名              
                if (queryCriteria.StockSysNo != "-999")
                {
                    List<ProductCenterItemInfo> FilterResult = new List<ProductCenterItemInfo>();
                    ProductCenterItemInfo[] SuggestTransferAllEntityArray = new ProductCenterItemInfo[resultList.Count];
                    resultList.CopyTo(SuggestTransferAllEntityArray);
                    FilterResult = SuggestTransferAllEntityArray.ToList<ProductCenterItemInfo>();
                    if (queryCriteria.AvailableSaleDaysCompareCode == "<=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    //[Mark][Alan.X.Luo 硬编码]
                                    if (InnerItem.WareHouseNumber.Equals("51"))
                                    {
                                        ProductCenterStockInfo tempItem59 = item.SuggestTransferStocks.Find(x => { return x.WareHouseNumber == "59"; });

                                        InnerItem.AvailableQty = InnerItem.AvailableQty + tempItem59.AvailableQty;
                                        InnerItem.ConsignQty = InnerItem.ConsignQty + tempItem59.ConsignQty;
                                        InnerItem.VirtualQty = InnerItem.VirtualQty + tempItem59.VirtualQty;
                                    }
                                    if (InnerItem.AVGDailySales == 0 && (InnerItem.AvailableQty + InnerItem.ConsignQty) > 0)
                                    {
                                        InnerItem.AvailableSalesDays = 999;
                                    }
                                    if (InnerItem.VirtualQty > 0)
                                    {
                                        InnerItem.SuggestQty = 0;
                                        InnerItem.SuggestQtyZhongZhuan = 0;
                                        InnerItem.AvailableSalesDays = 0;
                                    }
                                    if (InnerItem.AvailableSalesDays > Convert.ToInt32(queryCriteria.AvailableSaleDays))
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (queryCriteria.AvailableSaleDaysCompareCode == "=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    //[Mark][Alan.X.Luo 硬编码]
                                    if (InnerItem.WareHouseNumber == "51")
                                    {
                                        var tempItem59 = item.SuggestTransferStocks.Find(x => { return x.WareHouseNumber == "59"; });
                                        InnerItem.AvailableQty = InnerItem.AvailableQty + tempItem59.AvailableQty;
                                        InnerItem.ConsignQty = InnerItem.ConsignQty + tempItem59.ConsignQty;
                                        InnerItem.VirtualQty = InnerItem.VirtualQty + tempItem59.VirtualQty;
                                    }
                                    if (InnerItem.AVGDailySales == 0 && (InnerItem.AvailableQty + InnerItem.ConsignQty) > 0)
                                    {
                                        InnerItem.AvailableSalesDays = 999;
                                    }
                                    if (InnerItem.VirtualQty > 0)
                                    {
                                        InnerItem.SuggestQty = 0;
                                        InnerItem.SuggestQtyZhongZhuan = 0;
                                        InnerItem.AvailableSalesDays = 0;
                                    }
                                    if (Convert.ToInt32(queryCriteria.AvailableSaleDays) != InnerItem.AvailableSalesDays)
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (queryCriteria.AvailableSaleDaysCompareCode == ">=")
                    {
                        foreach (var item in resultList)
                        {
                            foreach (var InnerItem in item.SuggestTransferStocks)
                            {
                                if (InnerItem.WareHouseNumber == queryCriteria.StockSysNo)
                                {
                                    //[Mark][Alan.X.Luo 硬编码]
                                    if (InnerItem.WareHouseNumber == "51")
                                    {
                                        var tempItem59 = item.SuggestTransferStocks.Find(x => { return x.WareHouseNumber == "59"; });
                                        InnerItem.AvailableQty = InnerItem.AvailableQty + tempItem59.AvailableQty;
                                        InnerItem.ConsignQty = InnerItem.ConsignQty + tempItem59.ConsignQty;
                                        InnerItem.VirtualQty = InnerItem.VirtualQty + tempItem59.VirtualQty;
                                    }
                                    if (InnerItem.AVGDailySales == 0 && (InnerItem.AvailableQty + InnerItem.ConsignQty) > 0)
                                    {
                                        InnerItem.AvailableSalesDays = 999;
                                    }
                                    if (InnerItem.VirtualQty > 0)
                                    {
                                        InnerItem.SuggestQty = 0;
                                        InnerItem.SuggestQtyZhongZhuan = 0;
                                        InnerItem.AvailableSalesDays = 0;
                                    }
                                    if (InnerItem.AvailableSalesDays < Convert.ToInt32(queryCriteria.AvailableSaleDays))
                                    {
                                        FilterResult.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    resultList = FilterResult;
                }
                else//整网
                {
                    if (queryCriteria.AvailableSaleDaysCompareCode == "<=")
                    {
                        resultList = resultList.FindAll(x =>
                        {
                            if (x.AllStockAVGDailySales == 0 && (x.AllAvailableQty + x.ConsignQty) > 0)
                            {
                                x.AllStockAvailableSalesDays = 999;
                            }
                            if (x.VirtualQty > 0)
                            {
                                x.SuggestQtyAll = 0;
                                x.SuggestQtyAllZhongZhuan = 0;
                                x.AllStockAvailableSalesDays = 0;
                            }
                            return x.AllStockAvailableSalesDays <= Convert.ToInt32(queryCriteria.AvailableSaleDays);
                        }
                                                                       );
                    }
                    else if (queryCriteria.AvailableSaleDaysCompareCode == "=")
                    {
                        resultList = resultList.FindAll(x =>
                        {
                            if (x.AllStockAVGDailySales == 0 && (x.AllAvailableQty + x.ConsignQty) > 0)
                            {
                                x.AllStockAvailableSalesDays = 999;
                            }
                            if (x.VirtualQty > 0)
                            {
                                x.SuggestQtyAll = 0;
                                x.SuggestQtyAllZhongZhuan = 0;
                                x.AllStockAvailableSalesDays = 0;
                            }
                            return x.AllStockAvailableSalesDays == Convert.ToInt32(queryCriteria.AvailableSaleDays);
                        }
                                                                     );
                    }
                    else if (queryCriteria.AvailableSaleDaysCompareCode == ">=")
                    {
                        resultList = resultList.FindAll(x =>
                        {
                            if (x.AllStockAVGDailySales == 0 && (x.AllAvailableQty + x.ConsignQty) > 0)
                            {
                                x.AllStockAvailableSalesDays = 999;
                            }
                            if (x.VirtualQty > 0)//虚库销售的商品，建议备货数量0，可卖天数0，日均销量照常计算。
                            {
                                x.SuggestQtyAll = 0;
                                x.SuggestQtyAllZhongZhuan = 0;
                                x.AllStockAvailableSalesDays = 0;
                            }
                            return x.AllStockAvailableSalesDays >= Convert.ToInt32(queryCriteria.AvailableSaleDays);
                        }
                                                                     );
                    }
                }
            }
            #endregion

            //result.PagingInfo.TotalCount = result.ResultList.Count;
            return resultList;
        }

        private List<ProductCenterItemInfo> SuggestTransferCacl(List<ProductCenterItemInfo> list
    , InventoryTransferStockingQueryFilter queryCriteria)
        {
            List<ProductCenterStockInfo> itemStocks = QuerySuggestTransferStock(queryCriteria);

            foreach (ProductCenterItemInfo x in list)
            {
                if (null == x.SuggestTransferStocks)
                {
                    x.SuggestTransferStocks = new List<ProductCenterStockInfo>();
                }
                //获取分仓信息列表
                List<ProductCenterStockInfo> items = itemStocks.FindAll(item => item.ItemSysNumber == x.ItemSysNumber);
                //[Mark][Alan.X.Luo 硬编码]
                for (int i = 51; i <= 61; i++)
                {
                    ProductCenterStockInfo stockEntity = items.Find(item => item.WareHouseNumber.Trim() == i.ToString());
                    if (stockEntity != null)
                    {
                        int Q = CaclforeseeStockQty(x, stockEntity, queryCriteria);
                        int QZhongzhuan = CaclforeseeStockQtyZhongZhuan(x, stockEntity, queryCriteria);
                        //计算**仓建议备货数量
                        if (stockEntity.WareHouseNumber.Trim() == "59")//宝山仓 已经并到了与上海仓(嘉定仓)计算
                        {
                            stockEntity.SuggestQty = 0;
                            stockEntity.SuggestQtyZhongZhuan = 0;
                            stockEntity.AvailableSalesDays = 0;
                            stockEntity.AVGDailySales = 0;
                        }
                        else
                        {
                            #region CRL20519 需求完成后 2012年3月29日 用户又觉得 使用不便  又回复到了 老逻辑(以下逻辑和上边的老逻辑代码相同 保留是为了 看到算法调整的每次详情)

                            //SKU有最小包装数量  则   SKU分仓备货数量=向上取整RoundUP[（分仓按销量预测备货量-分仓可用库存-分仓代销库存-分仓采购在途-分仓移仓在途）/SKU最小包装数量,0] * SKU最小包装数量
                            if (stockEntity.MinPackNumber.HasValue && stockEntity.MinPackNumber.Value > 0)
                            {
                                stockEntity.SuggestQty = (int)Math.Ceiling((Double)(Q - stockEntity.AvailableQty - stockEntity.ConsignQty - (stockEntity.PurchaseInQty.HasValue ? stockEntity.PurchaseInQty.Value : 0) - (stockEntity.ShiftInQty.HasValue ? stockEntity.ShiftInQty.Value : 0)) / (Double)stockEntity.MinPackNumber.Value) * stockEntity.MinPackNumber.Value;

                                stockEntity.SuggestQtyZhongZhuan = (int)Math.Ceiling((Double)(QZhongzhuan - stockEntity.AvailableQty - stockEntity.ConsignQty - (stockEntity.PurchaseInQty.HasValue ? stockEntity.PurchaseInQty.Value : 0) - (stockEntity.ShiftInQty.HasValue ? stockEntity.ShiftInQty.Value : 0)) / (Double)stockEntity.MinPackNumber.Value) * stockEntity.MinPackNumber.Value;
                            }
                            else//SKU没有最小包装数量  则   SKU分仓备货数量=分仓按销量预测备货量-分仓可用库存-分仓代销库存-分仓采购在途-分仓移仓在途
                            {
                                stockEntity.SuggestQty = Q - stockEntity.AvailableQty - stockEntity.ConsignQty - (stockEntity.PurchaseInQty.HasValue ? stockEntity.PurchaseInQty.Value : 0) - (stockEntity.ShiftInQty.HasValue ? stockEntity.ShiftInQty.Value : 0);
                                stockEntity.SuggestQtyZhongZhuan = QZhongzhuan - stockEntity.AvailableQty - stockEntity.ConsignQty - (stockEntity.PurchaseInQty.HasValue ? stockEntity.PurchaseInQty.Value : 0) - (stockEntity.ShiftInQty.HasValue ? stockEntity.ShiftInQty.Value : 0);
                            }

                            #endregion

                            if (stockEntity.VirtualQty > 0)//虚库销售的商品（判定虚库库存>0，则认为是虚库销售），建议备货数量为0；
                            {
                                stockEntity.SuggestQty = 0;
                                stockEntity.SuggestQtyZhongZhuan = 0;
                            }
                        }
                        if (stockEntity.SuggestQty < 0)
                        {
                            stockEntity.SuggestQty = 0;
                        }
                        if (stockEntity.SuggestQtyZhongZhuan < 0)
                        {
                            stockEntity.SuggestQtyZhongZhuan = 0;
                        }
                        //计算出库仓可移库存
                        stockEntity.OutStockShiftQty = stockEntity.AvailableQtyStock + stockEntity.ConsignQty - Q;
                        if (stockEntity.OutStockShiftQty < 0)
                        {
                            stockEntity.OutStockShiftQty = 0;
                        }
                        x.SuggestTransferStocks.Add(stockEntity);
                    }
                    else
                    {
                        x.SuggestTransferStocks.Add(new ProductCenterStockInfo()
                        {
                            WareHouseNumber = i.ToString()
                        });
                    }
                }

                if (x.VirtualQty > 0)
                {
                    //计算建议备货总数
                    x.SuggestQtyAll = 0;//虚库销售的商品（判定虚库库存>0，则认为是虚库销售），建议备货数量为0；

                    //计算建议备货总数( 此方法 是专用于   点击中转需要重新计算 建议备货数量)
                    x.SuggestQtyAllZhongZhuan = 0;//虚库销售的商品（判定虚库库存>0，则认为是虚库销售），建议备货数量为0；

                    x.AllStockAvailableSalesDays = 0;//虚库销售的商品（判定虚库库存>0，则认为是虚库销售），可销售天数为0；
                }
                else
                {
                    //计算建议备货总数
                    x.SuggestQtyAll = CaclSuggestQty(x, queryCriteria);

                    //计算建议备货总数( 此方法 是专用于   点击中转需要重新计算 建议备货数量)
                    x.SuggestQtyAllZhongZhuan = CaclSuggestQtyZhongZhuan(x, queryCriteria);
                }

            }
            return list;
        }

        //获取所有分仓备货信息
        public List<ProductCenterStockInfo> QuerySuggestTransferStock(InventoryTransferStockingQueryFilter queryCriteria)
        {
            List<ProductCenterStockInfo> resultList = new List<ProductCenterStockInfo>();

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryCriteria.PageInfo.SortBy,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize,
                MaximumRows = queryCriteria.PageInfo.PageSize
            };
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySuggestTransferStock");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "P.[SysNo] DESC"))
            {
                if (!string.IsNullOrEmpty(queryCriteria.SysNO))
                {

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ISK.[ProductSysNo]",
                                     DbType.Int32, "@ItemSysNo",
                                     QueryConditionOperatorType.Equal,
                                     Convert.ToInt32(queryCriteria.SysNO));
                }
                if (!string.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductID]",
                                         DbType.String, "@ItemNumber",
                                         QueryConditionOperatorType.Equal,
                                         queryCriteria.ProductID);
                }
                if (!string.IsNullOrEmpty(queryCriteria.ProductName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductTitle]",
                                         DbType.String, "@ItemName",
                                         QueryConditionOperatorType.Like,
                                         queryCriteria.ProductName);
                }
                if (queryCriteria.ProductStatus.HasValue)
                {
                    if (queryCriteria.ProductStatusCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]",
                                             DbType.Int32, "@Status",
                                             QueryConditionOperatorType.LessThanOrEqual,
                                             (int)queryCriteria.ProductStatus);
                    }
                    else if (queryCriteria.ProductStatusCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]",
                                               DbType.Int32, "@Status",
                                               QueryConditionOperatorType.Equal,
                                              (int)queryCriteria.ProductStatus);
                    }
                    else if (queryCriteria.ProductStatusCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[Status]",
                                           DbType.Int32, "@Status",
                                           QueryConditionOperatorType.MoreThanOrEqual,
                                           (int)queryCriteria.ProductStatus);
                    }
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category1SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C2.[C1SysNo]",
                                         DbType.Int32, "@C1",
                                         QueryConditionOperatorType.Equal,
                                         Convert.ToInt32(queryCriteria.Category1SysNo));
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category2SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C3.[C2SysNo]",
                                         DbType.Int32, "@C2",
                                         QueryConditionOperatorType.Equal,
                                        Convert.ToInt32(queryCriteria.Category2SysNo));
                }
                if (!string.IsNullOrEmpty(queryCriteria.Category3SysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[C3SysNo]",
                                         DbType.Int32, "@C3",
                                         QueryConditionOperatorType.Equal,
                                        Convert.ToInt32(queryCriteria.Category3SysNo));
                }
                if (queryCriteria.ProductType.HasValue)
                {
                    int getProductType = 0;
                    switch (queryCriteria.ProductType.Value)
                    {
                        case ProductType.Normal:
                            getProductType = 0;
                            break;
                        case ProductType.OpenBox:
                            getProductType = 1;
                            break;
                        case ProductType.Bad:
                            getProductType = 2;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[ProductType]",
                                          DbType.Int32, "@ProductType",
                                          QueryConditionOperatorType.Equal,
                                          getProductType);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ManufacturerName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "M.[ManufacturerName]+M.[BriefName]",
                                          DbType.String, "@ManufacturerName",
                                          QueryConditionOperatorType.Like,
                                          queryCriteria.ManufacturerName);
                }
                if (queryCriteria.ProductConsignFlag != "-999")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[IsConsign]",
                                          DbType.Int32, "@IsConsign",
                                          QueryConditionOperatorType.Equal,
                                          Convert.ToInt32(queryCriteria.ProductConsignFlag));
                }
                if (!string.IsNullOrEmpty(queryCriteria.VendorName))
                {
                    string subSQLString_VenderName = @"SELECT PLP.ProductSysNo 
			                            FROM IPP3.dbo.Product_LastPOInfo AS PLP
	                                    LEFT JOIN IPP3.dbo.Vendor AS V WITH(NOLOCK)
	                                        ON PLP.LastVendorSysNo = V.SysNo
			                            where V.[VendorName] like @VendorName";

                    dataCommand.AddInputParameter("@VendorName", DbType.String);
                    dataCommand.SetParameterValue("@VendorName", "%" + queryCriteria.VendorName + "%");

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "ISK.ProductSysNo",
                             QueryConditionOperatorType.In,
                             subSQLString_VenderName);
                }

                if (queryCriteria.VendorSysNoList != null && queryCriteria.VendorSysNoList.Count > 0)
                {
                    string subSQLString_VendorSysNoList = @"SELECT PLP.ProductSysNo 
                			                            FROM IPP3.dbo.Product_LastPOInfo AS PLP
                	                                    LEFT JOIN IPP3.dbo.Vendor AS V WITH(NOLOCK)
                	                                        ON PLP.LastVendorSysNo = V.SysNo
                			                            where V.[SysNo] In (" + string.Join(",", queryCriteria.VendorSysNoList) + ")";

                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "ISK.ProductSysNo",
                             QueryConditionOperatorType.In,
                             subSQLString_VendorSysNoList);
                }

                if (queryCriteria.VendorSysNoList != null && queryCriteria.VendorSysNoList.Count > 0)
                {
                    string subSQLString_NotVendorSysNoList = @"SELECT SysNo FROM IPP3.dbo.Product AS PT  WITH(NOLOCK)
                                                                              INNER JOIN (SELECT VM.ManufacturerSysNo,VM.C2SysNo,VM.C3SysNo FROM  IPP3.dbo.Vendor_Manufacturer AS VM WITH(NOLOCK) WHERE VM.VendorSysNo In  (" + string.Join(",", queryCriteria.VendorSysNoList) + ") AND CHARINDEX(';'+CAST(DATEPART(weekday ,GetDate())-1 AS VARCHAR)+';',';'+ISNULL(VM.BuyWeekDay,'')+';')<=0 ) AS TM ON PT.Manufacturersysno = TM.Manufacturersysno AND (TM.c3sysno =PT.c3sysno   OR (SELECT SysNo FROM IPP3.dbo.Category3 AS C3 WITH(NOLOCK) WHERE C3.c2sysno =TM.c2sysno AND SysNo=PT.c3sysno)>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                              "P.[SysNo]",
                               QueryConditionOperatorType.NotIn,
                               subSQLString_NotVendorSysNoList);
                }


                #region [Ray.L.Xing]

                //新需求不控制能访问的PM 改为控制能访问到的产品线  所以注销了以下代码

                ////TODO:如果PM列表为空，说明没有权限。
                //if (string.IsNullOrEmpty(queryCriteria.PMSysNo))
                //{
                //    //sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                //    //    "P.[PMUserSysNo]",
                //    //    QueryConditionOperatorType.In,
                //    //    "-999");
                //}
                //else
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //            "P.[PMUserSysNo]"
                //            , DbType.Int32
                //            , "@PMUserSysNo",
                //            QueryConditionOperatorType.Equal,
                //            Convert.ToInt32(queryCriteria.PMSysNo));
                //}
                ////else
                ////{
                ////    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                ////                        "P.[PMUserSysNo]",
                ////                        QueryConditionOperatorType.In,
                ////                        queryCriteria.Condition.AllPMValue);
                ////}


                //新需求 要求只能访问自己能访问到的产品线的商品
                if (!string.IsNullOrEmpty(queryCriteria.PMSysNo))
                {

                    string subSQLString_ProductSysNoList = @"SELECT ProductsysNo FROm OverseaContentManagement.dbo.V_CM_ProductLine_Items 
                                                                  WHERE  ProductLineSysNo IN( SELECT ProductLineSysNo
						                                                                      FROM   OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p
						                                                                      WHERE  PMUserSysNo=" + queryCriteria.PMSysNo + "  OR CHARINDEX(';'+CAST(" + queryCriteria.PMSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                             QueryConditionOperatorType.In,
                             subSQLString_ProductSysNoList);
                }
                ////如果没有高级权限 则只能访问自己所能访问到的产品线的商品
                if (queryCriteria.AuthorizedPMsSysNumber != "Senior")
                {
                    string subSQLString_ProductSysNoList = @"SELECT ProductsysNo FROm OverseaContentManagement.dbo.V_CM_ProductLine_Items 
                                                                  WHERE  ProductLineSysNo IN( SELECT ProductLineSysNo
						                                                                      FROM   OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p
						                                                                      WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + "  OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0)";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "P.[SysNo]",
                                QueryConditionOperatorType.In,
                                subSQLString_ProductSysNoList);
                }

                #endregion
                
                if (!string.IsNullOrEmpty(queryCriteria.AverageUnitCost))
                {
                    if (queryCriteria.AverageUnitCostCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                              DbType.Decimal, "@UnitCost",
                                              QueryConditionOperatorType.LessThanOrEqual,
                                              Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                    else if (queryCriteria.AverageUnitCostCompareCode == "=")//==
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                            DbType.Decimal, "@UnitCost",
                                            QueryConditionOperatorType.Equal,
                                            Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                    else if (queryCriteria.AverageUnitCostCompareCode == ">=") //>=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[UnitCost]",
                                    DbType.Decimal, "@UnitCost",
                                    QueryConditionOperatorType.MoreThanOrEqual,
                                   Convert.ToDecimal(queryCriteria.AverageUnitCost));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.SalePrice))
                {
                    if (queryCriteria.SalePriceCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                            DbType.Decimal, "@CurrentPrice",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                          Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                    else if (queryCriteria.SalePriceCompareCode == "=")//==
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                             DbType.Decimal, "@CurrentPrice",
                                             QueryConditionOperatorType.Equal,
                                          Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                    else if (queryCriteria.SalePriceCompareCode == ">=")//>=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[CurrentPrice]",
                                          DbType.Decimal, "@CurrentPrice",
                                          QueryConditionOperatorType.MoreThanOrEqual,
                                        Convert.ToDecimal(queryCriteria.SalePrice));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.Point))
                {
                    if (queryCriteria.PointCompareCode == "<=")//<=
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                            DbType.Int32, "@Point",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.Point));
                    }
                    else if (queryCriteria.PointCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                            DbType.Int32, "@Point",
                                            QueryConditionOperatorType.Equal,
                                            Convert.ToInt32(queryCriteria.Point));
                    }
                    else if (queryCriteria.PointCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PP.[Point]",
                                             DbType.Int32, "@Point",
                                             QueryConditionOperatorType.MoreThanOrEqual,
                                             Convert.ToInt32(queryCriteria.Point));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.FinanceQty))
                {
                    if (queryCriteria.FinanceQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                            DbType.Int32, "@AccountQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.FinanceQty));
                    }
                    else if (queryCriteria.FinanceQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                             DbType.Int32, "@AccountQty",
                                             QueryConditionOperatorType.Equal,
                                          Convert.ToInt32(queryCriteria.FinanceQty));
                    }
                    else if (queryCriteria.FinanceQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AccountQty]",
                                             DbType.Int32, "@AccountQty",
                                             QueryConditionOperatorType.MoreThanOrEqual,
                                             Convert.ToInt32(queryCriteria.FinanceQty));

                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.AvailableQty))
                {
                    if (queryCriteria.AvailableQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                            DbType.Int32, "@AvailableQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                           Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                    else if (queryCriteria.AvailableQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                            DbType.Int32, "@AvailableQty",
                                            QueryConditionOperatorType.Equal,
                                              Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                    else if (queryCriteria.AvailableQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllAvailableQty]",
                                           DbType.Int32, "@AvailableQty",
                                           QueryConditionOperatorType.MoreThanOrEqual,
                                            Convert.ToInt32(queryCriteria.AvailableQty));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.OccupiedQty))
                {
                    if (queryCriteria.OccupiedQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                    else if (queryCriteria.OccupiedQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                    else if (queryCriteria.OccupiedQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[AllocatedQty]",
                                            DbType.Int32, "@AllocatedQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                           Convert.ToInt32(queryCriteria.OccupiedQty));
                    }
                }

                if (!string.IsNullOrEmpty(queryCriteria.OrderedQty))
                {
                    if (queryCriteria.OrderedQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                                            DbType.Int32, "@OrderQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                    else if (queryCriteria.OrderedQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                                            DbType.Int32, "@OrderQty",
                                            QueryConditionOperatorType.Equal,
                                            Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                    else if (queryCriteria.OrderedQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[OrderQty]",
                          DbType.Int32, "@OrderQty",
                          QueryConditionOperatorType.MoreThanOrEqual,
                           Convert.ToInt32(queryCriteria.OrderedQty));
                    }
                }


                if (!string.IsNullOrEmpty(queryCriteria.VirtualQty))
                {

                    if (queryCriteria.VirtualQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                            DbType.Int32, "@VirtualQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.VirtualQty));
                    }
                    else if (queryCriteria.VirtualQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                           DbType.Int32, "@VirtualQty",
                                           QueryConditionOperatorType.Equal,
                                          Convert.ToInt32(queryCriteria.VirtualQty));
                    }
                    else if (queryCriteria.VirtualQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[VirtualQty]",
                                           DbType.Int32, "@VirtualQty",
                                           QueryConditionOperatorType.MoreThanOrEqual,
                                        Convert.ToInt32(queryCriteria.VirtualQty));
                    }

                }

                if (!string.IsNullOrEmpty(queryCriteria.ConsignQty))
                {
                    if (queryCriteria.ConsignQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.ConsignQty));
                    }
                    else if (queryCriteria.ConsignQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.Equal,
                                             Convert.ToInt32(queryCriteria.ConsignQty));
                    }
                    else if (queryCriteria.ConsignQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VUI.[ConsignQty]",
                                            DbType.Int32, "@ConsignQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                           Convert.ToInt32(queryCriteria.ConsignQty));
                    }

                }

                if (!string.IsNullOrEmpty(queryCriteria.OnlineQty))
                {
                    if (queryCriteria.OnlineQtyCompareCode == "<=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.LessThanOrEqual,
                                            Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                    else if (queryCriteria.OnlineQtyCompareCode == "=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.Equal,
                                              Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                    else if (queryCriteria.OnlineQtyCompareCode == ">=")
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                            "VUI.[OnlineQty]",
                                            DbType.Int32, "@OnlineQty",
                                            QueryConditionOperatorType.MoreThanOrEqual,
                                             Convert.ToInt32(queryCriteria.OnlineQty));
                    }
                }
                if (queryCriteria.IsLarge.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                        "P.IsLarge",
                                        DbType.Int32, "@IsLarge",
                                        QueryConditionOperatorType.Equal,
                                        (int)queryCriteria.IsLarge);
                }

                if (!string.IsNullOrEmpty(queryCriteria.StockSysNo) && queryCriteria.StockSysNo != "-999")
                {
                    string subSQLString = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            where stocksysno = @stocksysno_sub and accountqty {0} @accountqty_sub";

                    string subSQLStringNoQty = @"select productsysno from ipp3.dbo.inventory_stock with(nolock)
                            where stocksysno = @stocksysno_sub";

                    dataCommand.AddInputParameter("@stocksysno_sub", DbType.Int32);
                    dataCommand.SetParameterValue("@stocksysno_sub", Int32.Parse(queryCriteria.StockSysNo));
                    if (!string.IsNullOrEmpty(queryCriteria.SubStockQty))
                    {
                        subSQLString = string.Format(subSQLString, queryCriteria.SubStockQtyCompareCode);
                        dataCommand.AddInputParameter("@accountqty_sub", DbType.Int32);
                        dataCommand.SetParameterValue("@accountqty_sub", Convert.ToInt32(queryCriteria.SubStockQty));
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "ISK.ProductSysNo",
                                 QueryConditionOperatorType.In,
                                 subSQLString);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "ISK.ProductSysNo",
                                 QueryConditionOperatorType.In,
                                 subSQLStringNoQty);
                    }
                }


                //Add By Ray.L.Xing 添加根据 是否同步库存商品进行查询
                if (queryCriteria.IsAsyncStock.HasValue)
                {

                    //获取库存同步商品List
                    string subSQLSynProcudtList = @"SELECT DISTINCT ProductSysno                       
                                                    FROM   OverseaContentManagement.dbo.V_CM_Product3Party_Mapping   WITH(NOLOCK) 
                                                    WHERE  [Status]='A' ";
                    if ((int)queryCriteria.IsAsyncStock.Value == 1)//是库存同步商品
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "ISK.ProductSysNo",
                                 QueryConditionOperatorType.In,
                                 subSQLSynProcudtList);
                    }
                    else if ((int)queryCriteria.IsAsyncStock.Value == 0)//不是库存同步商品
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                "ISK.ProductSysNo",
                                 QueryConditionOperatorType.NotIn,
                                 subSQLSynProcudtList);
                    }
                }

                #region 根据 日均销量大于等于零 查询
                if (!string.IsNullOrEmpty(queryCriteria.DaySalesCount) && Convert.ToInt32(queryCriteria.DaySalesCount) >= 0)
                {
                    //当前仓 有具体仓库名
                    string subProductsysnoList = string.Empty;
                    if (queryCriteria.StockSysNo != "-999")
                    {
                        string tempStr = "SELECT productsysno FROM OverseaInventoryManagement.dbo.Inventory_Product_AVGDailySales AS tempPAInfo WITH(NOLOCK) ";
                        if (queryCriteria.DaySalesCountCompareCode == "<=")
                        {
                            tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew>@AVGSaledQty_sub";
                            subProductsysnoList = @tempStr;
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")
                        {

                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)
                            {
                                tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew<>@AVGSaledQty_sub";
                            }
                            else
                            {
                                tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew=@AVGSaledQty_sub";
                            }
                            subProductsysnoList = @tempStr;

                        }
                        else if (queryCriteria.DaySalesCountCompareCode == ">=")
                        {
                            tempStr += "  WHERE tempPAInfo.StockSysNo=@stocksysnoFroAVGSaledQty_sub   AND tempPAInfo.AVGDailySalesNew>=@AVGSaledQty_sub";
                            subProductsysnoList = @tempStr;
                        }

                        dataCommand.AddInputParameter("@AVGSaledQty_sub", DbType.Decimal);
                        dataCommand.AddInputParameter("@stocksysnoFroAVGSaledQty_sub", DbType.Int32);

                        dataCommand.SetParameterValue("@stocksysnoFroAVGSaledQty_sub", Int32.Parse(queryCriteria.StockSysNo));
                        dataCommand.SetParameterValue("@AVGSaledQty_sub", Convert.ToInt32(queryCriteria.DaySalesCount));

                        if (queryCriteria.DaySalesCountCompareCode == "<=")//不在那些 （大于)的里面
                        {
                            sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "ISK.ProductSysNo",
                                                        QueryConditionOperatorType.NotIn,
                                                         subProductsysnoList);
                        }
                        else if (queryCriteria.DaySalesCountCompareCode == "=")
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) == 0)
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "ISK.ProductSysNo",
                                                        QueryConditionOperatorType.NotIn,
                                                         subProductsysnoList);
                            }
                            else
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                        "ISK.ProductSysNo",
                                                        QueryConditionOperatorType.In,
                                                         subProductsysnoList);
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(queryCriteria.DaySalesCount) > 0)
                            {
                                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                           "ISK.ProductSysNo",
                                                           QueryConditionOperatorType.In,
                                                            subProductsysnoList);
                            }
                        }
                    }
                }
                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                resultList = dataCommand.ExecuteEntityList<ProductCenterStockInfo>();
            }

            return resultList;
        }

        //计算预测备货量
        public int CaclforeseeStockQty(ProductCenterItemInfo x, ProductCenterStockInfo y
            , InventoryTransferStockingQueryFilter queryCriteria)
        {
            int result = 0;

            #region 最新需求（CRL20519：  result = 分仓日均销量 * （ 分仓送货天数 +  分仓备货天数：PM设定 ））

            //供应商分仓送货周期 (表中IPP3.dbo.Vendor_Manufacturer存的数据为 仓库号：周期：    “51：2；52：4；53：43”)
            int VendorSendPeriod = 0;
            if (!string.IsNullOrEmpty(y.SendPeriod))
            {
                string[] SendPeriodArray = y.SendPeriod.Split(';');//先以分号分隔 产生 [51：2],[52：4] 类型的数组
                foreach (string item in SendPeriodArray)
                {
                    string[] StockSendPeriod = item.Split(':');//再以冒号分隔 产生 [51],[2] 类型的数组 0索引位置 为 仓库号 1索引位置为：送货周期
                    if (StockSendPeriod != null && StockSendPeriod.Length > 1)
                    {
                        if (Convert.ToInt32(y.WareHouseNumber) == Convert.ToInt32(StockSendPeriod[0]))//获取对应仓的送货周期
                        {
                            VendorSendPeriod = Convert.ToInt32(StockSendPeriod[1]);
                        }
                    }
                }
            }
            result = (int)(Math.Floor((y.AVGDailySales.HasValue ? y.AVGDailySales.Value : 0) * (VendorSendPeriod + (string.IsNullOrEmpty(queryCriteria.BackDay) ? 0 : Convert.ToInt32(queryCriteria.BackDay)))));


            if (result < 0)
            {
                result = 0;
            }
            return result;

            #endregion
        }

        //计算建议备货总数
        public int CaclSuggestQty(ProductCenterItemInfo entity, InventoryTransferStockingQueryFilter queryCriteria)
        {
            if (string.IsNullOrEmpty(queryCriteria.StockSysNo) || queryCriteria.StockSysNo == "-999")
            {
                return entity.SuggestTransferStocks.Sum<ProductCenterStockInfo>(item => item.SuggestQty.HasValue ? item.SuggestQty.Value : 0);
            }
            else //选择分仓时，总数为分仓建议数量
            {
                int? result = entity.SuggestTransferStocks.Find(item => item.WareHouseNumber.Trim() == queryCriteria.StockSysNo).SuggestQty;
                return result.HasValue ? result.Value : 0;
            }
        }


        //计算预测备货量( 此方法 是专用于   点击中转需要重新计算 建议备货数量)
        public int CaclforeseeStockQtyZhongZhuan(ProductCenterItemInfo x, ProductCenterStockInfo y
            , InventoryTransferStockingQueryFilter queryCriteria)
        {
            int result = 0;


            #region       最新需求（CRL20519：  result = 分仓日均销量 * （ 分仓送货天数 +  分仓备货天数：PM设定 ））

            // 分仓送货周期=中转仓送货周期 + 中转仓库到目的仓库送货周期
            int VendorSendPeriod = 0;                 //分仓送货周期
            int VendorSendPeriod50 = 0;               //中转仓送货周期
            int ZhongZhuanToTargetStockSendPeriod = 0;//中转仓送货周期
            //中转仓送货周期
            if (!string.IsNullOrEmpty(y.SendPeriod))
            {
                //供应商分仓送货周期 (表中IPP3.dbo.Vendor_Manufacturer存的数据为 仓库号：周期：    “51：2；52：4；53：43”)
                string[] SendPeriodArray = y.SendPeriod.Split(';');//先以分号分隔 产生 [51：2],[52：4] 类型的数组
                foreach (string item in SendPeriodArray)
                {
                    string[] StockSendPeriod = item.Split(':');//再以冒号分隔 产生 [51],[2] 类型的数组 0索引位置 为 仓库号 1索引位置为：送货周期
                    if (StockSendPeriod != null && StockSendPeriod.Length > 1)
                    {
                        if (50 == Convert.ToInt32(StockSendPeriod[0]))//获取中转仓的送货周期
                        {
                            VendorSendPeriod50 = Convert.ToInt32(StockSendPeriod[1]);
                        }
                    }
                }
            }
            //系统需要以配置文件形式 读取 中转仓库到各目的仓库的送货周期
            if (Convert.ToInt32(y.WareHouseNumber.Trim()) == 52)
            {
                string ZhongZhuanToBeiJing = AppSettingManager.GetSetting("Inventory", "ZhongZhuanToBeiJing");
                ZhongZhuanToTargetStockSendPeriod = string.IsNullOrEmpty(ZhongZhuanToBeiJing) ? 3 : Convert.ToInt32(ZhongZhuanToBeiJing);
            }
            else if (Convert.ToInt32(y.WareHouseNumber.Trim()) == 53)
            {
                string ZhongZhuanToGuangZhou = AppSettingManager.GetSetting("Inventory", "ZhongZhuanToGuangZhou");
                ZhongZhuanToTargetStockSendPeriod = string.IsNullOrEmpty(ZhongZhuanToGuangZhou) ? 3 : Convert.ToInt32(ZhongZhuanToGuangZhou);
            }
            else if (Convert.ToInt32(y.WareHouseNumber.Trim()) == 55)
            {
                string ZhongZhuanToWuHan = AppSettingManager.GetSetting("Inventory", "ZhongZhuanToWuHan");
                ZhongZhuanToTargetStockSendPeriod = string.IsNullOrEmpty(ZhongZhuanToWuHan) ? 3 : Convert.ToInt32(ZhongZhuanToWuHan);
            }
            // 分仓送货周期=中转仓送货周期 + 中转仓库到目的仓库送货周期
            VendorSendPeriod = VendorSendPeriod50 + ZhongZhuanToTargetStockSendPeriod;
            result = (int)(Math.Floor((y.AVGDailySales.HasValue ? y.AVGDailySales.Value : 0) * (VendorSendPeriod + (!string.IsNullOrEmpty(queryCriteria.BackDay) ? Convert.ToInt32(queryCriteria.BackDay) : 0))));

            if (result < 0)
            {
                result = 0;
            }
            return result;

            #endregion
        }


        //计算建议备货总数( 此方法 是专用于   点击中转需要重新计算 建议备货数量)
        public int CaclSuggestQtyZhongZhuan(ProductCenterItemInfo entity, InventoryTransferStockingQueryFilter queryCriteria)
        {
            if (string.IsNullOrEmpty(queryCriteria.StockSysNo) || queryCriteria.StockSysNo == "-999")
            {
                return entity.SuggestTransferStocks.Sum<ProductCenterStockInfo>(item => item.SuggestQtyZhongZhuan.HasValue ? item.SuggestQtyZhongZhuan.Value : 0);
            }
            else //选择分仓时，总数为分仓建议数量
            {
                int? result = entity.SuggestTransferStocks.Find(item => item.WareHouseNumber.Trim() == queryCriteria.StockSysNo).SuggestQtyZhongZhuan;

                return result.HasValue ? result.Value : 0;
            }
        }


        #endregion
    }
}
