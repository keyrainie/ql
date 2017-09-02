using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.RMA;
using System.Data;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Entity;
using ECommerce.Entity.Order;

namespace ECommerce.DataAccess.RMA
{
    public class RequestDA
    {
        public static int CreateSysNo()
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("CreateRequestMasterSysNo");

            insertCommand.ExecuteNonQuery();

            return (int)insertCommand.GetParameterValue("@SysNo");
        }

        public static string SubmitRMARequest(RMARequestInfo request)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SubmitRMARequest");
            dataCommand.SetParameterValue("@SysNo", request.SysNo);
            dataCommand.SetParameterValue("@Address", request.Address);
            dataCommand.SetParameterValue("@Contact", request.Contact);
            dataCommand.SetParameterValue("@Phone", request.Phone);
            dataCommand.SetParameterValue("@AreaSysNo", request.AreaSysNo);
            dataCommand.SetParameterValue("@IsSubmit", request.IsSubmit);
            dataCommand.ExecuteNonQuery();
            return dataCommand.GetParameterValue("@RequestID").ToString();
        }

        public static RMARequestInfo Create(RMARequestInfo entity)
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("InsertRequestMaster");

            //insertCommand.SetParameterValue<RMARequestInfo>(entity);
            insertCommand.SetParameterValue("@SysNo", entity.SysNo);
            insertCommand.SetParameterValue("@RequestID", entity.RequestID);
            insertCommand.SetParameterValue("@SOSysNo", entity.SOSysNo);
            insertCommand.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            insertCommand.SetParameterValue("@Address", entity.Address);
            insertCommand.SetParameterValue("@Contact", entity.Contact);
            insertCommand.SetParameterValue("@Phone", entity.Phone);
            insertCommand.SetParameterValue("@Status", entity.Status);
            insertCommand.SetParameterValue("@AreaSysNo", entity.AreaSysNo);
            insertCommand.SetParameterValue("@ShipViaCode", entity.ShipViaCode);
            insertCommand.SetParameterValue("@IsSubmit", entity.IsSubmit);
            insertCommand.SetParameterValue("@TrackingNumber", entity.TrackingNumber);
            insertCommand.SetParameterValue("@StockType", entity.StockType.ToString());
            insertCommand.SetParameterValue("@ShippingType", entity.ShippingType.ToString());
            insertCommand.SetParameterValue("@InvoiceType", entity.InvoiceType.ToString());
            insertCommand.SetParameterValue("@MerchantSysno", entity.MerchantSysNo);
            insertCommand.ExecuteNonQuery();

            return entity;
        }

        public static QueryResult<RMARequestInfo> QueryRMARequest(RMAQueryInfo queryInfo)
        {
            if (queryInfo == null)
            {
                return null;
            }

            DataCommand dataCommand = DataCommandManager.GetDataCommand("QueryRequests");
            dataCommand.SetParameterValue("@CustomerSysNo", queryInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@SOSysNo", queryInfo.SOSysNo);
            dataCommand.SetParameterValue("@RequestID", queryInfo.RequestID);
            dataCommand.SetParameterValue("@SysNo", queryInfo.SysNo);
            if (!String.IsNullOrEmpty(queryInfo.ProductName))
            {
                queryInfo.ProductName = string.Format("%{0}%", queryInfo.ProductName.Trim());
            }

            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);

            DataSet dsResult = dataCommand.ExecuteDataSet();
            if (dsResult != null && dsResult.Tables.Count > 1)
            {
                DataTable requestTable = dsResult.Tables[0];
                DataTable registerTable = dsResult.Tables[1];
                int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                List<RMARequestInfo> requestList = null;
                List<RMARegisterInfo> registerList = null;

                if (registerTable.Rows != null && registerTable.Rows.Count > 0)
                {
                    registerList = DataMapper.GetEntityList<RMARegisterInfo, List<RMARegisterInfo>>(registerTable.Rows);
                }
                else
                {
                    registerList = new List<RMARegisterInfo>();
                }

                if (requestTable.Rows != null && requestTable.Rows.Count > 0)
                {
                    requestList = DataMapper.GetEntityList<RMARequestInfo, List<RMARequestInfo>>(requestTable.Rows);

                    requestList.ForEach(request =>
                    {
                        request.Registers = registerList.FindAll(register => register.RequestSysNo == request.SysNo);
                        //单件拆分到最细粒度
                        //if (request.Registers != null && request.Registers.Count > 0)
                        //{
                        //    IEnumerable<IGrouping<int?, RMARegisterInfo>> group = request.Registers.GroupBy(item => item.ProductSysNo);
                        //    List<RMARegisterInfo> list = new List<RMARegisterInfo>();
                        //    foreach (var item in group)
                        //    {
                        //        RMARegisterInfo info = item.First();
                        //        if (info != null)
                        //        {
                        //            info.Quantity = item.Count();
                        //            list.Add(info);
                        //        }
                        //    }
                        //    request.Registers = list;
                        //}
                    });
                }
                else
                {
                    requestList = new List<RMARequestInfo>();
                }


                int pageIndex = queryInfo.PagingInfo.PageIndex;

                if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
                {
                    if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                    {
                        pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                    }
                    else
                    {
                        pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                    }
                }

                QueryResult<RMARequestInfo> result = new QueryResult<RMARequestInfo>();
                result.ResultList = requestList;
                result.PageInfo = new PageInfo();
                result.PageInfo.TotalCount = totalCount;
                result.PageInfo.PageIndex = pageIndex;
                result.PageInfo.PageSize = queryInfo.PagingInfo.PageSize;
                result.PageInfo.SortBy = queryInfo.PagingInfo.SortBy;
                return result;
            }
            return new QueryResult<RMARequestInfo>() { ResultList = new List<RMARequestInfo>(), PageInfo = new PageInfo() };
        }

        public static QueryResult<OrderInfo> QueryCanRequestOrders(RMAQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("QueryCanRequestOrders");
            dataCommand.SetParameterValue("@CustomerSysNo", queryInfo.CustomerSysNo);
            if (!String.IsNullOrEmpty(queryInfo.ProductName))
            {
                queryInfo.ProductName = string.Format("%{0}%", queryInfo.ProductName.Trim());
            }
            dataCommand.SetParameterValue("@SOID", queryInfo.SOID);
            dataCommand.SetParameterValue("@SOIDList", queryInfo.SOIDList != null ? string.Join(",", queryInfo.SOIDList) : string.Empty);
            dataCommand.SetParameterValue("@ProductName", queryInfo.ProductName);
            dataCommand.SetParameterValue("@SOStatus", (int)SOStatus.OutStock);
            dataCommand.SetParameterValue("@WaitingAudit", (int)RMARequestStatus.WaitingAudit);
            dataCommand.SetParameterValue("@Origin", (int)RMARequestStatus.Origin);
            dataCommand.SetParameterValue("@Handling", (int)RMARequestStatus.Handling);

            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);

            DataSet dsResult = dataCommand.ExecuteDataSet();
            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                DataTable reviesTable = dsResult.Tables[0];
                List<OrderInfo> orderList = null;
                if (reviesTable.Rows != null && reviesTable.Rows.Count > 0)
                {
                    orderList = DataMapper.GetEntityList<OrderInfo, List<OrderInfo>>(reviesTable.Rows);
                    List<SOItemInfo> itemList = null;
                    if (dsResult.Tables.Count > 1 && dsResult.Tables[1] != null && dsResult.Tables[1].Rows != null && dsResult.Tables[1].Rows.Count > 0)
                    {
                        itemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(dsResult.Tables[1].Rows);
                    }
                    if (orderList != null)
                    {
                        orderList.ForEach(f =>
                        {
                            if (itemList != null)
                            {
                                f.SOItemList = itemList.FindAll(item => item.SOSysNo == f.SoSysNo);
                            }
                        });
                    }
                    int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                    int pageIndex = queryInfo.PagingInfo.PageIndex;

                    if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
                    {
                        if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                        {
                            pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                        }
                        else
                        {
                            pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                        }
                    }
                    QueryResult<OrderInfo> result = new QueryResult<OrderInfo>();
                    result.ResultList = orderList;
                    result.PageInfo = new PageInfo();
                    result.PageInfo.TotalCount = totalCount;
                    result.PageInfo.PageIndex = pageIndex;
                    result.PageInfo.PageSize = queryInfo.PagingInfo.PageSize;
                    result.PageInfo.SortBy = queryInfo.PagingInfo.SortBy;

                    return result;
                }
            }
            return new QueryResult<OrderInfo>();
        }
    }
}
