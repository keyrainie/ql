using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ECommerce.Entity;
using ECommerce.Entity.Payment;
using ECommerce.Entity.Product;
using ECommerce.Entity.Shipping;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    public class GiftCardOrderAmountCalculator : ICalculate
    {
        private Dictionary<string, List<OrderItem>> m_dicSubOrderItem = null;
        private AutoResetEvent m_ResetEvent = null;

        public GiftCardOrderAmountCalculator()
        {
            m_ResetEvent = new AutoResetEvent(true);
        }

        public void Calculate(ref OrderInfo order)
        {
            m_ResetEvent.WaitOne();
            //初始化计算
            Init(order);

            //设置运费为0，但设置配送方式
            CalculateShippingAmount(order);

            //计算手续费
            CalculateCommissionAmount(order);

            m_ResetEvent.Set();
        }

        /// <summary>
        /// 初始化计算
        /// </summary>
        /// <param name="order"></param>
        private void Init(OrderInfo order)
        {
            this.m_dicSubOrderItem = new Dictionary<string, List<OrderItem>>();

            if (order.SubOrderList == null)
            {
                order.SubOrderList = new Dictionary<string, OrderInfo>();
            }

            OrderInfo subOrderInfo = null;
            List<OrderItem> subOrderItemList = null;

            foreach (var kvs in order.SubOrderList)
            {
                subOrderInfo = kvs.Value;
                subOrderInfo.PointPay = 0;
                subOrderInfo.PointPayAmount = 0m;
                subOrderInfo.BalancePayAmount = 0m;

                subOrderItemList = new List<OrderItem>();
                if (subOrderInfo.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in subOrderInfo.OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            itemGroup.ProductItemList.ForEach(product =>
                            {
                                product["UnitDiscountAmt"] = 0m;
                                product["UnitCouponAmt"] = 0m;
                                subOrderItemList.Add(product);
                            });
                        }
                    }
                }
                if (subOrderInfo.GiftItemList != null)
                {
                    subOrderInfo.GiftItemList.ForEach(gift =>
                    {
                        gift["UnitDiscountAmt"] = 0m;
                        gift["UnitCouponAmt"] = 0m;
                        subOrderItemList.Add(gift);
                    });
                }
                if (subOrderInfo.AttachmentItemList != null)
                {
                    subOrderInfo.AttachmentItemList.ForEach(attachment =>
                    {
                        attachment["UnitDiscountAmt"] = 0m;
                        attachment["UnitCouponAmt"] = 0m;
                        subOrderItemList.Add(attachment);
                    });
                }

                this.m_dicSubOrderItem.Add(kvs.Key, subOrderItemList);
            }
        }


        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="order"></param>
        private void CalculateShippingAmount(OrderInfo order)
        {

            #region 【 step1 设置好各个子单的配送方式 】

            List<ShipTypeInfo> allShipTypeList = PipelineDA.GetSupportedShipTypeList(order.Contact.AddressAreaID,(PaymentCategory)order.PaymentCategory);

            ShipTypeInfo curShipTypeInfo = null;
            List<ShipTypeInfo> tempShipTypeList = null;
            List<ShipTypeInfo> subOrderShipTypeList = null;

            List<ShippingFeeQueryInfo> qryList = new List<ShippingFeeQueryInfo>();
            ShippingFeeQueryInfo qry = null;

            foreach (var kvs in order.SubOrderList)
            {
                OrderInfo subOrderInfo = kvs.Value;

                if (subOrderInfo["WarehouseNumber"] != null)
                {
                    int subOrderWarehouseNumber = Convert.ToInt32(subOrderInfo["WarehouseNumber"]);
                    tempShipTypeList = allShipTypeList.FindAll(shipType => shipType.OnlyForStockSysNo == subOrderWarehouseNumber &&
                                                                                shipType.StoreType == (int)subOrderInfo["StoreType"]);
                }
                else
                {
                    tempShipTypeList = allShipTypeList;
                }

                subOrderShipTypeList = null;
                curShipTypeInfo = null;

                if (tempShipTypeList != null)
                {
                    subOrderShipTypeList = new List<ShipTypeInfo>();
                    subOrderShipTypeList.AddRange(tempShipTypeList);
                }

                if (subOrderShipTypeList != null && subOrderShipTypeList.Count > 0)
                {
                    subOrderShipTypeList.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                    curShipTypeInfo = subOrderShipTypeList[0];
                }

                if (curShipTypeInfo != null)
                {
                    subOrderInfo.ShipTypeID = curShipTypeInfo.ShipTypeID;
                    subOrderInfo["ShipTypeName"] = curShipTypeInfo.ShipTypeName;
                    subOrderInfo["ShipTypeDesc"] = string.Format("{0}：{1}出库直邮", curShipTypeInfo.ShipTypeName
                                                                                  , subOrderInfo["WarehouseName"]);

                    qry = new ShippingFeeQueryInfo();
                    qry.TransID = kvs.Key;
                    qry.SoAmount = subOrderInfo.TotalProductAmount;
                    qry.SoTotalWeight = subOrderInfo.TotalWeight;
                    qry.SOSingleMaxWeight = subOrderInfo.MaxWeight;
                    qry.AreaId = subOrderInfo.Contact.AddressAreaID;
                    qry.CustomerSysNo = subOrderInfo.Customer.SysNo;
                    qry.IsUseDiscount = 0;
                    qry.SubShipTypeList = curShipTypeInfo.ShipTypeID.ToString();
                    qry.SellType = Convert.ToInt32(subOrderInfo["SellerType"]);
                    qry.MerchantSysNo = Convert.ToInt32(kvs.Key.Split('|')[0]);
                    qry.ShipTypeId = 0;
                    qryList.Add(qry);
                }
            }

            #endregion

            #region 【 开始计算运费 】

            List<ShippingInfo> shipFeeList = PipelineDA.GetAllShippingFee(qryList);
            ShippingInfo curShippingInfo = null;

            foreach (var kvs in order.SubOrderList)
            {
                curShippingInfo = shipFeeList.Find(x => x.TransID == kvs.Key && x.ShippingTypeID.ToString() == kvs.Value.ShipTypeID);
                if (curShippingInfo != null)
                {
                    kvs.Value.ShippingAmount = 0m;
                }
            }

            order.ShippingAmount = order.SubOrderList.Sum(x => x.Value.ShippingAmount);

            #endregion
        }

        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="order"></param>
        private void CalculateCommissionAmount(OrderInfo order)
        {
            //先计算子单的手续费
            PayTypeInfo payTypeInfo = PipelineDA.GetPayTypeBySysNo(order.PayTypeID);
            order.PayTypeName = payTypeInfo.PayTypeName;

            foreach (KeyValuePair<string, OrderInfo> sub in order.SubOrderList)
            {
                sub.Value.CommissionAmount = decimal.Round(payTypeInfo.PayRate * sub.Value.CashPayAmount, 2, MidpointRounding.AwayFromZero);
                sub.Value.PayTypeName = payTypeInfo.PayTypeName;
            }

            order.CommissionAmount = order.SubOrderList.Sum(subOrder => subOrder.Value.CommissionAmount);
        }
    }
}
