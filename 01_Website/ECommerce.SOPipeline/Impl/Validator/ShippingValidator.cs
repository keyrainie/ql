using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using ECommerce.Entity.Shipping;
using ECommerce.Entity.Product;

namespace ECommerce.SOPipeline.Impl
{
    public class ShippingValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            bool pass = true;

            //先检查订单上的配送地址
            if (order.Contact.AddressAreaID <= 0)
            {
                errorMsg = "请先保存收货人信息！";
                return false;
            }

            List<ShipTypeInfo> allShipTypeList = PipelineDA.GetAllShippingTypeList();
            //先检查主单
            pass = InnerValidate(order, allShipTypeList, true, out errorMsg);

            if (!pass) return pass;

            //检查拆分后的子单
            if (order.SubOrderList != null && order.SubOrderList.Count > 0)
            {
                OrderInfo subOrder = null;
                foreach (var kvs in order.SubOrderList)
                {
                    subOrder = kvs.Value;
                    pass = InnerValidate(subOrder, allShipTypeList, false, out errorMsg);
                    if (!pass) return pass;
                }
            }
            //因配送区域删除，所以去掉对商品配送区域的判断
            //errorMsg = CheckShippingType(order);
            if (!string.IsNullOrWhiteSpace(errorMsg))
                return false;

            errorMsg = null;
            return true;
        }

        private bool InnerValidate(OrderInfo order, List<ShipTypeInfo> allShipTypeList, bool isMasterSO, out string errorMsg)
        {
            if (!isMasterSO)
            {
                if (string.IsNullOrEmpty(order.ShipTypeID))
                {
                    string merchantName = "";
                    if (order.OrderItemGroupList != null && order.OrderItemGroupList.Count > 0
                        && order.OrderItemGroupList[0].ProductItemList != null
                        && order.OrderItemGroupList[0].ProductItemList.Count > 0)
                    {
                        merchantName = order.OrderItemGroupList[0].ProductItemList[0].MerchantName;
                    }
                    errorMsg = string.Format("商家【{0}】有订单未选择配送方式！", merchantName);
                    return false;
                }

                ShipTypeInfo curShipTypeInfo = allShipTypeList.Find(x => x.ShipTypeID == order.ShipTypeID);
                if (curShipTypeInfo == null)
                {
                    errorMsg = "订单存在不支持的配送方式，请重新选择！";
                    return false;
                }
            }

            errorMsg = null;
            return true;
        }

        /// <summary>
        /// 检查是否有不支持的配送区域
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private string CheckShippingType(OrderInfo order)
        {
            string shippingErrorMsg = "";
            foreach (var kvs in order.SubOrderList)
            {
                OrderInfo subOrderInfo = kvs.Value;
                
                List<ProductShippingPrice> ProductSysNoList = PipelineDA.Pipeline_GetAllProductRestrictedAreaByAreaSysNo(subOrderInfo.Contact.AddressAreaID);
                if (ProductSysNoList == null || ProductSysNoList.Count <= 0)
                {
                    shippingErrorMsg = "订单无法配送到您选择的送货区域，请重新选择！";
                    break;
                }
                else
                { 

                    List<int> ProductSysNo = new List<int>();
                    foreach (OrderItemGroup ItemGroup in subOrderInfo.OrderItemGroupList)
                    {
                        foreach (OrderProductItem ProductItem in ItemGroup.ProductItemList)
                        {
                            if (!ProductSysNoList.Exists(p => p.ProductSysNo == ProductItem.ProductSysNo))
                            {
                                shippingErrorMsg = "商品（" + ProductItem.ProductName + "）无法配送到您选择的送货区域，请重新选择！";
                                break;
                            }
                        }
                    }
                    
                }

                if (!PipelineDA.Pipeline_ExistsShipTypeAreaPrice(int.Parse(subOrderInfo.ShipTypeID), subOrderInfo.Contact.AddressAreaID))
                {
                    shippingErrorMsg += "该收货地区配置有误，请重新选择！";
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(shippingErrorMsg))
            {
                return shippingErrorMsg;
            }
            return "";
        }
    }
}
