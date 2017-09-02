using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.RMA;
using ECommerce.DataAccess.RMA;
using ECommerce.Enums;
using ECommerce.DataAccess.Member;
using ECommerce.Entity.Order;
using System.Data;
using ECommerce.Entity;
using ECommerce.Utility;
using System.Transactions;

namespace ECommerce.Facade.RMA
{
    public class RMARequestFacade
    {
        public static string SubmitRMARequest(RMARequestInfo request)
        {
            return RequestDA.SubmitRMARequest(request);
        }

        public static string CreateRMARequest(RMARequestInfo request)
        {
            OrderInfo result = CustomerDA.GetQuerySODetailInfo(request.CustomerSysNo.Value, request.SOSysNo.Value);
            //检查是否重复创建
            Dictionary<int, int> master = new Dictionary<int, int>();
            request.Registers.ForEach(r =>
            {
                if (r.SOItemType.Value == SOProductType.Product)
                {
                    if (master.ContainsKey(r.ProductSysNo.Value))
                    {
                        master[r.ProductSysNo.Value] += r.Quantity.Value;
                    }
                    else
                    {
                        master.Add(r.ProductSysNo.Value, r.Quantity.Value);
                    }
                }
            });
            // verify master so items
            foreach (KeyValuePair<int, int> pair in master)
            {
                var soitem = result.SOItemList.Find(s => s.ProductSysNo == pair.Key
                   && s.ProductType == SOItemType.ForSale);
                int regQty = RegisterDA.GetRegisterQty(
                    pair.Key, (int)SOProductType.Product, request.SOSysNo.Value);
                if (soitem.Quantity < regQty + pair.Value)
                {
                    throw new BusinessException("商品【{0}】的售后申请正在处理中，请不要重复申请", soitem.ProductTitle);
                }
            }

            using (TransactionScope ts = new TransactionScope())
            {
                request.SysNo = RequestDA.CreateSysNo();
                request.RequestID = GenerateId(request.SysNo.Value);
                //创建申请单初始状态为【待审核】
                request.Status = RMARequestStatus.WaitingAudit;
                request.ShippingType = DeliveryType.SELF;//request.ShippingType.Trim();
                request.InvoiceType = InvoiceType.SELF;
                request.StockType = StockType.SELF;
                request.MerchantSysNo = result.SellerSysNo;
                RequestDA.Create(request);

                request.Registers.ForEach(register =>
                {
                    var soitem = result.SOItemList.Find(s => s.ProductSysNo == register.ProductSysNo
                        && (int)s.ProductType == (int)register.SOItemType);
                    register.ProductID = soitem.ProductID;
                    register.ProductName = soitem.ProductName;
                    register.CustomerDesc = register.RMAReasonDesc;
                    register.Status = RMARequestStatus.WaitingAudit;
                    register.OwnBy = RMAOwnBy.Origin;
                    register.Location = RMALocation.Origin;
                    register.RevertStatus = null;
                    register.ShippedWarehouse = result.StockSysNo;

                    register.IsWithin7Days = false;
                    register.IsRecommendRefund = false;
                    register.NewProductStatus = RMANewProductStatus.Origin;
                    register.NextHandler = RMANextHandler.RMA;
                    register.SOItemType = (SOProductType)soitem.ProductType;
                    for (int i = 0; i < register.Quantity; i++)
                    {
                        register.SysNo = RegisterDA.CreateSysNo();

                        RegisterDA.Create(register);
                        RegisterDA.InsertRequestItem(request.SysNo.Value, register.SysNo.Value);
                    }
                });
                ts.Complete();
            }
            return request.RequestID;
        }


        public static QueryResult<RMARequestInfo> QueryRequestInfos(RMAQueryInfo query)
        {
            return RequestDA.QueryRMARequest(query);
        }

        private static string GenerateId(int sysNo)
        {
            return string.Format("R0{0}", sysNo.ToString().PadLeft(8, '0'));
        }


        public static QueryResult<OrderInfo> QueryCanRequestOrders(RMAQueryInfo query)
        {
            return RequestDA.QueryCanRequestOrders(query);
        }

        public static OrderInfo GetCanRequestOrder(int customerSysNo, int soSysNo)
        {
            RMAQueryInfo query = new RMAQueryInfo();
            query.SOID = soSysNo.ToString();
            query.CustomerSysNo = customerSysNo;
            query.PagingInfo = new PageInfo()
            {
                PageIndex = 1,
                PageSize = 1
            };
            QueryResult<OrderInfo> result = RequestDA.QueryCanRequestOrders(query);
            if (result.ResultList == null || result.ResultList.Count <= 0)
            {
                return new OrderInfo()
                {
                    SOItemList = new List<SOItemInfo>(0)
                };
            }
            return result.ResultList[0];
        }
        public static RMARegisterInfo LoadRegisterByRegisterSysNo(int sysNo)
        {
            return RegisterDA.LoadRegisterByRegisterSysNo(sysNo);
        }
    }
}
