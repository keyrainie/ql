using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 处理套餐
    /// </summary>
    public class ComboCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            if (order.DiscountDetailList == null)
            {
                order.DiscountDetailList = new List<OrderItemDiscountInfo>();
            }

            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                if (itemGroup.PackageNo > 0 && itemGroup.PackageType == 1)
                {
                    ComboInfo combo = PromotionDA.GetComboByComboSysNo(itemGroup.PackageNo);
                    if (combo != null && combo.Items != null && combo.Items.Count > 0)
                    {                        
                        foreach (ComboItem comboItem in combo.Items)
                        {
                            OrderItemDiscountInfo itemDiscount = new OrderItemDiscountInfo();
                            itemDiscount.DiscountActivityName = combo.SaleRuleName;
                            itemDiscount.DiscountActivityNo = itemGroup.PackageNo;
                            itemDiscount.DiscountType = 1;
                            itemDiscount.PackageNo = itemGroup.PackageNo;
                            itemDiscount.ProductSysNo = comboItem.ProductSysNo;
                            itemDiscount.Quantity = itemGroup.Quantity * comboItem.Quantity;
                            itemDiscount.UnitDiscount = Math.Abs(comboItem.Discount);
                            itemDiscount.UnitRewardedBalance = 0;
                            itemDiscount.UnitRewardedPoint = 0;
                            itemDiscount.UnitShipFeeDiscountAmt = 0;

                            order.DiscountDetailList.Add(itemDiscount);                            
                        }
                    }

                }
            }
        }
    }
}
