using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IVirtualPurchaseOrderQueryDA))]
    public class VirtualPurchaseOrderQueryDA : IVirtualPurchaseOrderQueryDA
    {
        #region IVirtualPurchaseOrderQueryDA Members

        public System.Data.DataTable QueryVirtualPurchaseOrderList(QueryFilter.PO.VirtualPurchaseOrderQueryFilter queryFilter, out int totalCount)
        {
            totalCount = 0;
            DataTable dt = new DataTable();
            DataTable returnDt = new DataTable();
            CustomDataCommand dataCommand;
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            if (queryFilter.DelayDays.HasValue || queryFilter.EstimateDelayDays.HasValue)
            {
                if (queryFilter.IsHasHistory == true)
                {
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryAllVSPO");
                }
                else
                {
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryAllVSPONoHistory");
                }
            }
            else
            {
                if (queryFilter.IsHasHistory == true)
                {
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVSPO");
                }
                else
                {
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVSPONohistory");
                }
            }

            #region
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "VSO.sysno desc"))
            {
                if (queryFilter.CreateDateFrom.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.CreateTime",
                    DbType.DateTime, "@CreateTime", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateDateFrom.Value);
                }
                if (queryFilter.CreateDateTo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.CreateTime",
                    DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, queryFilter.CreateDateTo.Value.AddDays(1));
                }
                if (queryFilter.PayTypeSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.PayTypeSysNo",
                    DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, queryFilter.PayTypeSysNo);
                }
                if (queryFilter.PMLeaderSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.PMUserSysNo",
                    DbType.Int32, "@PMLeaderSysNo", QueryConditionOperatorType.Equal, queryFilter.PMLeaderSysNo);
                }
                if (queryFilter.PMExecSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.PMHandleUserSysNo",
                    DbType.Int32, "@PMExecSysNo", QueryConditionOperatorType.Equal, queryFilter.PMExecSysNo);
                }
                if (queryFilter.ProductSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.ProductSysNo",
                    DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                }
                if (queryFilter.SOStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.Status",
                    DbType.Int32, "@SOStatus", QueryConditionOperatorType.Equal, queryFilter.SOStatus);
                }
                if (queryFilter.Status.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status);
                }
                if (queryFilter.StockSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOItem.WarehouseNumber",
                   DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                }
                if (queryFilter.InStockOrderType.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.InStockOrderType",
                    DbType.Int32, "@InStockOrderType", QueryConditionOperatorType.Equal, queryFilter.InStockOrderType);
                }
                string sysNo = "";
                if (!string.IsNullOrEmpty(queryFilter.VSPOSysNo) && !string.IsNullOrWhiteSpace(queryFilter.VSPOSysNo))
                {
                    sysNo = " AND VSO.SysNo IN ( " + ArraryConvertToString(queryFilter.VSPOSysNo, '.') + " ) ";
                }
                string soSysNo = "";
                if (!string.IsNullOrWhiteSpace(queryFilter.SOSysNo) && !string.IsNullOrEmpty(queryFilter.SOSysNo))
                {
                    soSysNo = " AND VSO.SOSysNo IN ( " + ArraryConvertToString(queryFilter.SOSysNo, '.') + " ) ";
                }
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                      "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                if (queryFilter.InStockStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VSO.InStockStatus",
          DbType.Int32, "@InStockStatus", QueryConditionOperatorType.Equal, queryFilter.InStockStatus);
                }
                if (queryFilter.POStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pomaster.Status",
                    DbType.Int32, "@POStatus", QueryConditionOperatorType.Equal, queryFilter.POStatus);
                }
                if (queryFilter.ShiftStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st_shift.Status",
                    DbType.Int32, "@st_shift", QueryConditionOperatorType.Equal, queryFilter.ShiftStatus);
                }
                if (queryFilter.TransferStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "St_Transfer.Status",
                    DbType.Int32, "@St_Transfer", QueryConditionOperatorType.Equal, queryFilter.TransferStatus);
                }
                if (queryFilter.C3SysNo.HasValue && queryFilter.C3SysNo.Value > 0)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "category.Category3Sysno",
                    DbType.Int32, "@c3sysno", QueryConditionOperatorType.Equal, queryFilter.C3SysNo);
                }
                else
                {
                    if (queryFilter.C2SysNo.HasValue && queryFilter.C2SysNo.Value > 0)
                    {
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "category.Category2Sysno",
                        DbType.Int32, "@c2sysno", QueryConditionOperatorType.Equal, queryFilter.C2SysNo);
                    }
                    else
                    {
                        if (queryFilter.C1SysNo.HasValue && queryFilter.C1SysNo.Value > 0)
                        {
                            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "category.Category1Sysno",
                            DbType.Int32, "@c1sysno", QueryConditionOperatorType.Equal, queryFilter.C1SysNo);
                        }
                    }
                }

                dataCommand.CommandText = sb.BuildQuerySql().Replace("AppendSql", sysNo + soSysNo);

                if (queryFilter.POStatus.HasValue || queryFilter.ShiftStatus.HasValue || queryFilter.TransferStatus.HasValue)
                {
                    string sqlStatusQuery = @"
			                   LEFT JOIN ipp3.dbo.PO_Item  poitem (NOLOCK) 
			                    ON (POItem.POSysNo = VSO.InStockOrderSysNo 
				                    and VSO.ProductSysNo = POItem.ProductSysNo)
			                   LEFT JOIN ipp3.dbo.St_Shift_Item st_shift_item WITH (NOLOCK) 
			                    ON (st_shift_item.shiftsysno = VSO.InStockOrderSysNo 
				                    and VSO.ProductSysNo = st_shift_item.ProductSysNo)
	    	                   LEFT JOIN ipp3.dbo.st_Transfer_Item St_Transfer_Item WITH (NOLOCK) 
			                    ON (St_Transfer_Item.TransferSysNo=VSO.InStockOrderSysNo
			                        and VSO.ProductSysNo=St_Transfer_Item.ProductSysNo and TransferType=2) ";

                    dataCommand.CommandText = dataCommand.CommandText.Replace("#StatusQuery#", sqlStatusQuery);
                }
                else
                {
                    dataCommand.CommandText = dataCommand.CommandText.Replace("#StatusQuery#", string.Empty);
                }
                if (queryFilter.C3SysNo.HasValue || queryFilter.C2SysNo.HasValue || queryFilter.C1SysNo.HasValue)
                {
                    string sqlCategoryQuery = @"
			                          LEFT JOIN OverseaContentManagement.dbo.V_CM_CategoryInfo category WITH (NOLOCK) 
			                          ON product.C3SysNo = category.Category3Sysno";
                    dataCommand.CommandText = dataCommand.CommandText.Replace("#CategoryQuery#", sqlCategoryQuery);
                }
                else
                {
                    dataCommand.CommandText = dataCommand.CommandText.Replace("#CategoryQuery#", string.Empty);
                }

                EnumColumnList columnEnums = new EnumColumnList();
                columnEnums.Add("Status", typeof(VirtualPurchaseOrderStatus));
                columnEnums.Add("SOStatus", typeof(SOStatus));
                columnEnums.Add("POStatus", typeof(PurchaseOrderStatus));
                columnEnums.Add("ShiftStatus", typeof(ShiftRequestStatus));
                columnEnums.Add("TransferStatus", typeof(ConvertRequestStatus));
                columnEnums.Add("InStockStatus", typeof(InStockStatus));
                columnEnums.Add("InStockOrderType", typeof(VirtualPurchaseInStockOrderType));
                dt = dataCommand.ExecuteDataTable(columnEnums);
                returnDt = dt.Copy();

                if (!queryFilter.DelayDays.HasValue && !queryFilter.EstimateDelayDays.HasValue)
                {
                    totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                }

                int DelayDays = 0;
                if (queryFilter.DelayDays.HasValue)
                {
                    DelayDays = Convert.ToInt32(queryFilter.DelayDays.ToString().Trim());
                }
                foreach (DataRow item in dt.Rows)
                {
                    VirtualPurchaseInStockOrderType InStockOrderType = (VirtualPurchaseInStockOrderType)Enum.Parse(typeof(VirtualPurchaseInStockOrderType), item["InStockOrderType"].ToString());
                    DateTime NowDate = DateTime.Now;
                    switch (InStockOrderType)
                    {
                        case VirtualPurchaseInStockOrderType.PO:
                            NowDate = item["RealInstockTime"] != null && item["RealInstockTime"] != DBNull.Value ? Convert.ToDateTime(item["RealInstockTime"].ToString()) : DateTime.Now;
                            break;
                        case VirtualPurchaseInStockOrderType.Convert:
                            NowDate = item["RealInStockTimeForTransfer"] != null && item["RealInStockTimeForTransfer"] != DBNull.Value ? Convert.ToDateTime(item["RealInStockTimeForTransfer"].ToString()) : DateTime.Now;
                            break;
                        case VirtualPurchaseInStockOrderType.Shift:
                            NowDate = item["RealInStockTimeForShift"] != null && item["RealInStockTimeForShift"] != DBNull.Value ? Convert.ToDateTime(item["RealInStockTimeForShift"].ToString()) : DateTime.Now;
                            break;
                    }

                    CheckDelayDays(item, NowDate, Convert.ToDateTime(item["CreateTime"].ToString()), DelayDays);
                }

                if (queryFilter.EstimateDelayDays.HasValue)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        DateTime NowDate = DateTime.Now;
                        if (item["EstimateArriveTime"] != null && item["EstimateArriveTime"] != DBNull.Value)
                        {
                            NowDate = Convert.ToDateTime(item["EstimateArriveTime"]);
                        }
                        CheckYJDelayDays(item, NowDate, Convert.ToDateTime(item["CreateTime"].ToString()), Convert.ToInt32(queryFilter.EstimateDelayDays.Value.ToString()));
                    }
                }

                SetResult(dt);
                if (queryFilter.DelayDays.HasValue || queryFilter.EstimateDelayDays.HasValue)
                {

                    int startNumber = queryFilter.PageInfo.PageSize.Value * queryFilter.PageInfo.PageIndex.Value;
                    int pageSize = queryFilter.PageInfo.PageSize.Value;
                    //if (ToExecel)
                    //{
                    //    startNumber = 0;
                    //    pageSize = 1000000;
                    //}
                    //IEnumerable<DataRow> list = dt.Clone().AsEnumerable();
                    List<DataRow> list = new List<DataRow>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr);
                    }
                    if (queryFilter.DelayDays.HasValue)
                    {
                        list = list.Where(a => (int)a["IsDelay"] == 1).ToList();
                    }

                    if (queryFilter.EstimateDelayDays.HasValue)
                    {
                        list = list.Where(a => (int)a["DelayDays"] == 1).ToList();
                    }

                    totalCount = list.Count();
                    list = list.OrderBy(p => p["RowNumber"]).Skip(startNumber).Take(pageSize).ToList();
                    returnDt.Rows.Clear();
                    foreach (DataRow dr in list)
                    {
                        returnDt.ImportRow(dr);
                    }
                }

                return returnDt;
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// 数组转换为String
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        private string ArraryConvertToString(string str, char splitStr)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string[] arr = str.Split(splitStr);
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Trim() != "")
                {
                    sb.Append(arr[i].Trim() + " , ");
                }
            }
            if (sb.ToString() != "")
            {
                return sb.ToString().Substring(0, sb.ToString().LastIndexOf(","));
            }
            return "";
        }

        private void CheckDelayDays(DataRow item, DateTime beginTime, DateTime createTime, int IsDelay)
        {
            TimeSpan ts = (TimeSpan)(beginTime - createTime);
            int DayCount = ts.Days;
            int tmpWeekDayNum = int.Parse((DayCount / 7).ToString());
            int tmpMode = int.Parse((DayCount % 7).ToString());
            if (Convert.ToInt32(createTime.AddDays(tmpMode).DayOfWeek) < Convert.ToInt32(createTime.DayOfWeek))
            {
                tmpWeekDayNum = tmpWeekDayNum + 1;
            }
            if ((DayCount - (tmpWeekDayNum) * 2) >= IsDelay)
            {
                item["IsDelay"] = 1;
            }
            if ((DayCount - (tmpWeekDayNum) * 2) >= 3)
            {
                item["SysNo"] = item["SysNo"];
                //item["SysNoStyle"] = 1;
            }
        }

        private void CheckYJDelayDays(DataRow item, DateTime beginTime, DateTime createTime, int yjDelayDays)
        {
            TimeSpan ts = (TimeSpan)(beginTime - createTime);
            int DayCount = ts.Days;
            int tmpWeekDayNum = int.Parse((DayCount / 7).ToString());
            int tmpMode = int.Parse((DayCount % 7).ToString());
            if (Convert.ToInt32(createTime.AddDays(tmpMode).DayOfWeek) < Convert.ToInt32(createTime.DayOfWeek))
            {
                tmpWeekDayNum = tmpWeekDayNum + 1;
            }
            if (DayCount - (tmpWeekDayNum) * 2 >= yjDelayDays)
            {
                item["DelayDays"] = 1;
            }
        }

        private void SetResult(DataTable dt)
        {
            if (dt != null)
            {
                //进行数据的统计
                foreach (DataRow list in dt.Rows)
                {
                    //list["SOSysNoShow"] = list["SOSysNo"].ToString();
                    list["PPrice"] = (list["PPrice"] == null || list["PPrice"] == DBNull.Value) ? 0 : Convert.ToDecimal(list["PPrice"].ToString());
                    //list["StatusName"] = list.Status.HasValue ? ((SVStatus)list.Status).GetDesc() : "";
                    //list.EstimateArriveTimeShow = "";
                    if (list["EstimateArriveTime"] != null && list["EstimateArriveTime"] != DBNull.Value && DateTime.Parse(list["EstimateArriveTime"].ToString()) != DateTime.Parse("1900-1-1"))
                    {
                        list["EstimateArriveTime"] = list["EstimateArriveTime"];
                    }
                    list["RealInstockTime"] = list["RealInstockTime"] == null && list["RealInstockTime"] == DBNull.Value ? "" : list["RealInstockTime"];

                    if (list["InStockOrderType"] != null && list["InStockOrderType"] != DBNull.Value)
                    {
                        //单据号设置
                        VirtualPurchaseInStockOrderType InStockOrderType = (VirtualPurchaseInStockOrderType)Enum.Parse(typeof(VirtualPurchaseInStockOrderType), list["InStockOrderType"].ToString());

                        string stockSysNoString = (list["InStockOrderSysNo"] != null && list["InStockOrderSysNo"] != DBNull.Value ? list["InStockOrderSysNo"].ToString() : string.Empty);
                        switch (InStockOrderType)
                        {
                            case VirtualPurchaseInStockOrderType.PO:
                                if (!string.IsNullOrEmpty(stockSysNoString))
                                {
                                    list["InStockOrderSysNo"] = stockSysNoString;
                                }
                                break;
                            case VirtualPurchaseInStockOrderType.Convert:
                                if (!string.IsNullOrEmpty(stockSysNoString))
                                {
                                    list["InStockOrderSysNo"] = "58" + stockSysNoString.ToString().PadLeft(8, '0');
                                }
                                break;
                            case VirtualPurchaseInStockOrderType.Shift:
                                if (!string.IsNullOrEmpty(stockSysNoString))
                                {
                                    list["InStockOrderSysNo"] = "57" + stockSysNoString.ToString().PadLeft(8, '0');
                                }
                                break;
                        }
                    }

                    ////设置单据状态
                    //list.InStockOrderStatusName = "";
                    //switch (InStockOrderType)
                    //{
                    //    case VSPOInstockOrderType.PO:
                    //        if (list.POStatus.HasValue)
                    //        {
                    //            if (((POStatus)list.POStatus) == POStatus.InStock && list.PartlyReceiveStatus == (int)YNStatus.Yes)
                    //            {
                    //                list.InStockOrderStatusName = ItemInstockStatus.PartlyInstock.GetDesc();
                    //            }
                    //            else
                    //            {
                    //                list.InStockOrderStatusName = ((POStatus)list.POStatus).GetDesc();
                    //            }
                    //        }
                    //        break;
                    //    case VSPOInstockOrderType.TransferList:
                    //        if (list.TransferStatus.HasValue)
                    //        {
                    //            list.InStockOrderStatusName = ((TransferStatus)list.TransferStatus).GetDesc();
                    //        }
                    //        break;
                    //    case VSPOInstockOrderType.ShiftList:
                    //        if (list.ShiftStatus.HasValue)
                    //        {
                    //            list.InStockOrderStatusName = ((ShiftStatus)list.ShiftStatus).GetDesc();
                    //        }
                    //        break;
                    //}

                    ////设置商品入库状态
                    //switch (InStockOrderType)
                    //{
                    //    case VSPOInstockOrderType.PO:
                    //        int InStockQty_Po = list.POStatus == (int)POStatus.InStock ? list.InStockQtyPo : 0;
                    //        int ItemInstockStatusShow_Po = ReturnItemInstockStatus(list.QtyPo, InStockQty_Po);
                    //        if (list.InStockOrderSysNo.HasValue)
                    //        {
                    //            list.InStockOrderSysNoShowHref = list.InStockOrderSysNo.Value;

                    //        }
                    //        if (ItemInstockStatusShow_Po == (int)ItemInstockStatus.ZeroInstock)
                    //        {
                    //            list.InStockOrderSysNoShowStyle = 1;
                    //        }

                    //        if (list.InstockStatus == null)
                    //        {
                    //            list.InstockStatus = ItemInstockStatusShow_Po.ToString();
                    //        }
                    //        if (list.RealInstockTime.HasValue && list.RealInstockTime.Value != AppConst.DATETIME_NULL)
                    //        {
                    //            list.RealInstockTimeForOrder = list.RealInstockTime.Value.ToString();
                    //        }
                    //        break;
                    //    case VSPOInstockOrderType.TransferList:
                    //        int ItemInstockStatusShow_TransferList = ReturnItemInstockStatus(list.QtyTransfer, list.InStockQtyTransfer);
                    //        if (ItemInstockStatusShow_TransferList == (int)ItemInstockStatus.ZeroInstock)
                    //        {
                    //            list.InStockOrderSysNoShowStyle = 1;
                    //        }
                    //        list.InstockStatus = ItemInstockStatusShow_TransferList.ToString();
                    //        if (list.RealInStockTimeForTransfer.HasValue && list.RealInStockTimeForTransfer.Value != AppConst.DATETIME_NULL)
                    //        {
                    //            list.RealInstockTimeForOrder = list.RealInStockTimeForTransfer.Value.ToString();
                    //        }
                    //        break;
                    //    case VSPOInstockOrderType.ShiftList:
                    //        int ItemInstockStatusShow_ShiftList = ReturnItemInstockStatus(list.QtyShift, list.InStockQtyShift);
                    //        if (ItemInstockStatusShow_ShiftList == (int)ItemInstockStatus.ZeroInstock)
                    //        {
                    //            list.InStockOrderSysNoShowStyle = 1;
                    //        }
                    //        list.InstockStatus = ItemInstockStatusShow_ShiftList.ToString();
                    //        if (list.RealInStockTimeForShift.HasValue && list.RealInStockTimeForShift.Value != AppConst.DATETIME_NULL)
                    //        {
                    //            list.RealInstockTimeForOrder = list.RealInStockTimeForShift.Value.ToString();
                    //        }
                    //        break;
                    //    default:
                    //        list.ItemInstockStatusShow = "UnKnow";
                    //        break;
                    //}

                    ////设置订单状态
                    //list.SOStatusName = SOStatusExtension.GetSOStatusDesc(list.SOStatusExt);

                    //if (list.InstockStatus != null)
                    //{
                    //    list.ItemInstockStatusShow = ((ItemInstockStatus)Convert.ToInt32(list.InstockStatus)).GetDesc();
                    //}
                }
            }
        }
    }
}
