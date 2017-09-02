using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.SOPipeline;
using ECommerce.SOPipeline.Impl;

namespace ECommerce.Facade.Shopping
{
    public static partial class ShoppingFacade
    {
        /// <summary>
        /// 构造购物车添加商品链接
        /// </summary>
        /// <param name="url">添加购物车Url，在View通过BuildUrl("AddShoppingCartRoute")获取</param>
        /// <param name="sysNo">商品编号</param>
        /// <param name="qty">数量，可不传，默认1</param>
        /// <returns></returns>
        public static string BuildAddProductUrl(string url, int sysNo, int qty = 1)
        {
            return string.Format("{0}?Category=Product&SysNo={1}&Qty={2}", url, sysNo, qty);
        }

        /// <summary>
        /// 构造购物车添加套餐链接
        /// </summary>
        /// <param name="url">添加购物车Url，在View通过BuildUrl("AddShoppingCartRoute")获取</param>
        /// <param name="sysNo">套餐编号</param>
        /// <param name="qty">数量，可不传，默认1</param>
        /// <returns></returns>
        public static string BuildAddPackageUrl(string url, int sysNo, int qty = 1)
        {
            return string.Format("{0}?Category=Package&SysNo={1}&Qty={2}", url, sysNo, qty);
        }

        /// <summary>
        /// 构建购物车商品分组
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sysNo"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public static ShoppingItemGroup BuildShoppingItemGroup(string category, int sysNo, int qty)
        {
            ShoppingItemGroup result = new ShoppingItemGroup();

            if (category.ToLower().Equals("product"))
            {
                result.ShoppingItemList = new List<ShoppingItem>()
                {
                    new ShoppingItem()
                    {
                        ProductSysNo = sysNo,
                        UnitQuantity = 1,
                        ProductChecked = true
                    }
                };
                result.PackageType = 0;
                result.PackageNo = 0;
                result.Quantity = qty;
                result.PackageChecked = true;
            }
            else if (category.ToLower().Equals("package"))
            {
                result.ShoppingItemList = new List<ShoppingItem>();
                //根据套餐编号加载套餐内的商品和数量
                ComboInfo comb = PromotionDA.GetComboByComboSysNo(sysNo);
                result.ShoppingItemList = new List<ShoppingItem>();
                if (comb != null && comb.Items != null && comb.Items.Count > 0)
                {
                    foreach (var combItem in comb.Items)
                    {
                        result.ShoppingItemList.Add(new ShoppingItem()
                        {
                            ProductSysNo = combItem.ProductSysNo,
                            UnitQuantity = combItem.Quantity
                        });
                    }
                }
                result.PackageChecked = true;
                result.PackageType = 1;
                result.PackageNo = sysNo;
                result.Quantity = qty;
            }

            return result;
        }

        /// <summary>
        /// 将本次加入购物车操作写入Cookie
        /// </summary>
        /// <param name="items"></param>
        public static void AddToShoppingCart(ShoppingItemGroup shoppingItemGroup, ShoppingCart shoppingCart)
        {
            bool needCreateNewGroup = true;
            foreach (ShoppingItemGroup itemGroup in shoppingCart.ShoppingItemGroupList)
            {
                if (shoppingItemGroup.PackageType.Equals(0))
                {
                    //加入单个商品
                    var currShoppingItem = itemGroup.ShoppingItemList[0];
                    var addShoppingItem = shoppingItemGroup.ShoppingItemList[0];
                    if (itemGroup.PackageType.Equals(0) &&
                        currShoppingItem.ProductSysNo.Equals(addShoppingItem.ProductSysNo))
                    {
                        itemGroup.Quantity += shoppingItemGroup.Quantity;
                        needCreateNewGroup = false;
                    }
                }
                else if (shoppingItemGroup.PackageType.Equals(1))
                {
                    //加入套餐
                    if (itemGroup.PackageType.Equals(1) &&
                        itemGroup.PackageNo.Equals(shoppingItemGroup.PackageNo))
                    {
                        itemGroup.Quantity += shoppingItemGroup.Quantity;
                        needCreateNewGroup = false;
                    }
                }
            }
            if (needCreateNewGroup)
            {
                shoppingCart.ShoppingItemGroupList.Add(shoppingItemGroup);
            }
        }

        /// <summary>
        /// 构造购物车
        /// </summary>
        /// <returns></returns>
        public static OrderPipelineProcessResult BuildShoppingCart(ShoppingCart shoppingCart)
        {
            OrderPipelineProcessResult shoppingResult = SOPipelineProcessor.BuildShoppingCart(shoppingCart);

            if (shoppingResult.ReturnData != null && shoppingResult.ReturnData.OrderItemGroupList != null)
            {
                //修正购物车中商品购买数量
                //foreach (var group in shoppingCart.ShoppingItemGroupList)
                //{
                //    OrderItemGroup newShoppingGroup = null;
                //    if (group.PackageType.Equals(0))
                //    {
                //        //单个商品
                //        newShoppingGroup = shoppingResult.ReturnData.OrderItemGroupList.Find(x
                //            => x.PackageType == group.PackageType
                //            && x.PackageNo == group.PackageNo
                //            && x.ProductItemList[0].ProductSysNo == group.ShoppingItemList[0].ProductSysNo);
                //    }
                //    else if (group.PackageType.Equals(1))
                //    {
                //        //套餐
                //        newShoppingGroup = shoppingResult.ReturnData.OrderItemGroupList.Find(x
                //            => x.PackageType == group.PackageType
                //            && x.PackageNo == group.PackageNo);
                //    }
                //    if (newShoppingGroup != null)
                //    {
                //        //最多购买newShoppingGroup.MaxCountPerSO个
                //        if (group.Quantity > newShoppingGroup.MaxCountPerSO)
                //        {
                //            group.Quantity = newShoppingGroup.MaxCountPerSO;
                //            newShoppingGroup.Quantity = newShoppingGroup.MaxCountPerSO;
                //        }
                //        //至少需要购买newShoppingGroup.MinCountPerSO个
                //        if (group.Quantity < newShoppingGroup.MinCountPerSO)
                //        {
                //            group.Quantity = newShoppingGroup.MinCountPerSO;
                //            newShoppingGroup.Quantity = newShoppingGroup.MinCountPerSO;
                //        }
                //    }
                //}
                //排序，套餐排在最前面
                var query = from r in shoppingResult.ReturnData.OrderItemGroupList
                            orderby r.PackageType //descending
                            select r;
                shoppingResult.ReturnData.OrderItemGroupList = query.ToList();
            }
            return shoppingResult;
        }

        /// <summary>
        /// 构造Mini购物车
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public static ShoppingCartMiniResult BuildMiniShoppingCart(ShoppingCart shoppingCart, out OrderPipelineProcessResult pipleResult)
        {
            ShoppingCartMiniResult result = new ShoppingCartMiniResult()
            {
                ProductCount = 0,
                TotalAmount = 0m,
                ItemList = new List<ShoppingCartMiniItem>()
            };

            pipleResult = BuildShoppingCart(shoppingCart);
            if (pipleResult != null && pipleResult.ReturnData != null
                && pipleResult.ReturnData.OrderItemGroupList != null)
            {
                foreach (var itemGroup in pipleResult.ReturnData.OrderItemGroupList)
                {
                    foreach (var item in itemGroup.ProductItemList)
                    {
                        result.ProductCount += itemGroup.Quantity * item.UnitQuantity;

                        decimal totalUnitDiscount = 0m;
                        List<OrderItemDiscountInfo> discountList = null;
                        if (pipleResult.ReturnData.DiscountDetailList != null && pipleResult.ReturnData.DiscountDetailList.Count > 0)
                        {
                            discountList = pipleResult.ReturnData.DiscountDetailList.FindAll(m
                            => m.PackageNo == itemGroup.PackageNo
                            && m.ProductSysNo == item.ProductSysNo);
                            totalUnitDiscount = discountList.Sum(m => m.UnitDiscount);
                        }
                        result.TotalAmount += (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity);
                        result.TotalTaxFee += (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity);// *decimal.Parse(item["TariffRate"].ToString());

                        result.ItemList.Add(new ShoppingCartMiniItem()
                        {
                            PackageSysNo = itemGroup.PackageNo,
                            ProductSysNo = item.ProductSysNo,
                            ProductTitle = item["ProductTitle"].ToString(),
                            DefaultImage = ECommerce.Facade.Product.ProductFacade.BuildProductImage(Enums.ImageSize.P60, item.DefaultImage),
                            TaxFee = (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity),// * decimal.Parse(item["TariffRate"].ToString()),
                            ProductPrice = item.UnitSalePrice - totalUnitDiscount,
                            Quantity = item.UnitQuantity * itemGroup.Quantity
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 从Pipline构造Mini购物车
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public static ShoppingCartMiniResult BuildMiniShoppingCartFromPipeline(OrderPipelineProcessResult pipelineResult)
        {
            ShoppingCartMiniResult result = new ShoppingCartMiniResult()
            {
                ProductCount = 0,
                TotalAmount = 0m,
                ItemList = new List<ShoppingCartMiniItem>()
            };

            if (pipelineResult != null && pipelineResult.ReturnData != null
                && pipelineResult.ReturnData.OrderItemGroupList != null)
            {
                foreach (var itemGroup in pipelineResult.ReturnData.OrderItemGroupList)
                {
                    foreach (var item in itemGroup.ProductItemList)
                    {
                        result.ProductCount += itemGroup.Quantity * item.UnitQuantity;

                        decimal totalUnitDiscount = 0m;
                        List<OrderItemDiscountInfo> discountList = null;
                        if (pipelineResult.ReturnData.DiscountDetailList != null && pipelineResult.ReturnData.DiscountDetailList.Count > 0)
                        {
                            discountList = pipelineResult.ReturnData.DiscountDetailList.FindAll(m
                            => m.PackageNo == itemGroup.PackageNo
                            && m.ProductSysNo == item.ProductSysNo);
                            totalUnitDiscount = discountList.Sum(m => m.UnitDiscount);
                        }
                        result.TotalAmount += (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity);
                        //result.TotalTaxFee += (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity);// *decimal.Parse(item["TariffRate"].ToString());

                        result.ItemList.Add(new ShoppingCartMiniItem()
                        {
                            PackageSysNo = itemGroup.PackageNo,
                            ProductSysNo = item.ProductSysNo,
                            ProductTitle = item["ProductTitle"].ToString(),
                            DefaultImage = ECommerce.Facade.Product.ProductFacade.BuildProductImage(Enums.ImageSize.P60, item.DefaultImage),
                            TaxFee = (item.UnitSalePrice - totalUnitDiscount) * (item.UnitQuantity * itemGroup.Quantity),// * decimal.Parse(item["TariffRate"].ToString()),
                            ProductPrice = item.UnitSalePrice - totalUnitDiscount,
                            Quantity = item.UnitQuantity * itemGroup.Quantity
                        });
                    }
                }
            }

            return result;
        }
    }
}
