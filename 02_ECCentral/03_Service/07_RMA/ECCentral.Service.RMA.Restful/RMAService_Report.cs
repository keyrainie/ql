using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.RMA;
using System.Data;
using ECCentral.Service.RMA.Restful.RequestMsg;
using ECCentral.Service.RMA.AppService;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/Report/ProductCards/QueryProductCardsInventory/{productSysNo}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductCardInventoryByProductSysNo(string productSysNo)
        {
            return new QueryResult()
            {
                Data = ObjectFactory<IReportQueryDA>.Instance.QueryProductCardInventoryByProductSysNo(int.Parse(productSysNo))
            };
        }

        [WebInvoke(UriTemplate = "/Report/ProductCards/QueryProductCards/{productSysNo}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryProductCardsByProductSysNo(string productSysNo)
        {
            return new QueryResult()
            {
                Data = ObjectFactory<IReportQueryDA>.Instance.QueryProductCardsByProductSysNo(int.Parse(productSysNo))
            };
        }

        [WebInvoke(UriTemplate = "/Report/OutBoundNotReturn/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryOutBoundNotReturn(OutBoundNotReturnQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IReportQueryDA>.Instance.QueryOutBoundNotReturn(request, out totalCount);
            dataTable.Columns.Add("SendEmailCount", typeof(int));
            dataTable.Columns.Add("IsContact", typeof(Boolean));
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    dr["SendEmailCount"] = dr["IsSendMail"] == DBNull.Value ? 0 : dr["IsSendMail"];
                    DateTime now = DateTime.Now;
                    VendorStatus Vendor_Status;

                    if (!Enum.TryParse(dr["Vendor_Status"].ToString(), out Vendor_Status))
                    {
                        //转换失败赋个条件判断不需要的值...
                        Vendor_Status = VendorStatus.UnAvailable;
                    }
                    DateTime ValidDate, ExpiredDate;
                    ValidDate = dr["ValidDate"] == DBNull.Value ? now.AddDays(10) : Convert.ToDateTime(dr["ValidDate"]);
                    ExpiredDate = dr["ExpiredDate"] == DBNull.Value ? now.AddDays(-10) : Convert.ToDateTime(dr["ExpiredDate"]);

                    decimal? TotalPOMoney, ContractAmt;
                    TotalPOMoney = dr["TotalPOMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalPOMoney"]);
                    ContractAmt = dr["ContractAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ContractAmt"]);
                    bool IsContact = false;

                    if (Vendor_Status == VendorStatus.Available)
                    {

                        if (now >= ValidDate && now <= ExpiredDate)
                        {
                            IsContact = true;
                        }
                        else if (TotalPOMoney > ContractAmt)
                        {
                            IsContact = true;
                        }
                    }
                    dr["IsContact"] = IsContact;
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Report/OutBoundNotReturn/SendDunEmail", Method = "PUT")]
        public virtual void SendDunEmail(SendDunEmailReq request)
        {
            ObjectFactory<OutBoundAppService>.Instance.SendDunEmail(request.OutboundSysNo, request.RegisterSysNo, request.SendMailCount, request.SOSysNo);
        }

        [WebInvoke(UriTemplate = "/Report/RMAInventory/QueryProduct", Method = "POST")]
        public virtual QueryResultList QueryRMAProductInventory(RMAInventoryQueryFilter request)
        {
            QueryResultList resultList = new QueryResultList();
            int totalCount;
            decimal totalMisCost;
            var dataTable = ObjectFactory<IReportQueryDA>.Instance.QueryRMAProductInventory(request, out totalCount, out  totalMisCost);
            QueryResult resultTable = new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
            DataTable dt = new DataTable();
            dt.Columns.Add("TotleMisCost", typeof(decimal));
            dt.Rows.Add(totalMisCost);

            QueryResult MisCost = new QueryResult()
            {
                Data = dt
            };
            resultList.Add(resultTable);
            resultList.Add(MisCost);
            return resultList;
        }

        [WebInvoke(UriTemplate = "/Report/RMAInventory/QueryProductForExport", Method = "POST")]
        public virtual QueryResult QueryRMAProductInventoryForExport(RMAInventoryQueryFilter request)
        {
            int totalCount;
            decimal totalMisCost;
            var dataTable = ObjectFactory<IReportQueryDA>.Instance.QueryRMAProductInventory(request, out totalCount, out  totalMisCost);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Report/RMAInventory/QueryRMAItem", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryRMAItemInventory(RMAInventoryQueryFilter request)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<IReportQueryDA>.Instance.QueryRMAItemInventory(request, out totalCount);
            dataTable.Columns.Add("Inventory", typeof(int));
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    RMAOutBoundStatus OutBoundStatus;
                    if (!Enum.TryParse(dr["OutBoundStatus"].ToString(), out OutBoundStatus))
                    {
                        //转换失败赋个条件判断不需要的值...
                        OutBoundStatus = RMAOutBoundStatus.Abandon;
                    }
                    RMARevertStatus RevertStatus;
                    if (!Enum.TryParse(dr["RevertStatus"].ToString(), out RevertStatus))
                    {
                        RevertStatus = RMARevertStatus.Abandon;
                    }
                    RMANewProductStatus NewProductStatus;
                    if (!Enum.TryParse(dr["NewProductStatus"].ToString(), out NewProductStatus))
                    {
                        NewProductStatus = RMANewProductStatus.OtherProduct;
                    }

                    dr["Inventory"] = (OutBoundStatus == RMAOutBoundStatus.SendAlready ||
                        RevertStatus == RMARevertStatus.Reverted &&
                        NewProductStatus == RMANewProductStatus.Origin) ? 0 : 1;
                }
            }

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

    }
}
