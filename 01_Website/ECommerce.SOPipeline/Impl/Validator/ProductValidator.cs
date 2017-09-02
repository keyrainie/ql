using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Order;

namespace ECommerce.SOPipeline.Impl
{
    public class ProductValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            //检查团购商品和普通商品混合下单
            if (order.OrderItemGroupList != null)
            {
                var groups = order.OrderItemGroupList.SelectMany(g => g.ProductItemList).GroupBy(k => k.SpecialActivityType);
                if (groups.Count() > 1 && groups.Any(x => x.Key == 1 || x.Key == 3))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in groups.Where(x => x.Key == 1 || x.Key == 3).First())
                    {
                        sb.AppendLine(String.Format("【{0}】是团购商品，团购商品只能单独购买！", item["ProductTitle"]));
                    }

                    errorMsg = sb.ToString();
                    return false;
                }
            }

            //检查拆分前的订单
            if (!InnerValidate(order,true, out errorMsg))
            {
                return false;
            }

            //检查拆分后的订单，同一个item不会被拆分到不同的suborder中去，可以复用check逻辑
            if (order.SubOrderList != null && order.SubOrderList.Count > 0)
            {
                foreach (var kvs in order.SubOrderList)
                {
                    if (!InnerValidate(kvs.Value,false, out errorMsg))
                    {
                        return false;
                    }
                }
            }

            errorMsg = null;
            return true;
        }

        private bool InnerValidate(OrderInfo order, bool isMasterSO, out string errorMsg)
        {
            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                foreach (OrderProductItem item in itemGroup.ProductItemList)
                {
                    #region 1.商品状态检查
                    //fixbug: 主商品必须是上架状态
                    int productStatus = int.Parse(item["ProductStatus"].ToString());
                    if(productStatus != (int)ECommerce.Enums.ProductStatus.Show)
                        //&& productStatus != (int)ECommerce.Enums.ProductStatus.OnlyShow)
                    {
                        errorMsg = LanguageHelper.GetText("商品【{0}】未上架！", order.LanguageCode);
                        errorMsg = string.Format(errorMsg, item["ProductTitle"]);
                        return false;
                    }
                    #endregion

                    if (isMasterSO)
                    {
                        #region 2.每单限购最小数量检查
                        int minCountPerOrder = 0;
                        int.TryParse(item["MinCountPerOrder"].ToString(), out minCountPerOrder);
                        if (itemGroup.Quantity * item.UnitQuantity < minCountPerOrder)
                        {
                            errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                            errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                            return false;
                        }
                        #endregion

                        #region 3.每单限购最大数量检查
                        int maxCountPerOrder = 0;
                        int.TryParse(item["MaxCountPerOrder"].ToString(), out maxCountPerOrder);
                        if (itemGroup.Quantity * item.UnitQuantity > maxCountPerOrder)
                        {
                            errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                            errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                            return false;
                        }
                        #endregion
                    }
                }
            }

            errorMsg = null;
            return true;
        }
    }
}
