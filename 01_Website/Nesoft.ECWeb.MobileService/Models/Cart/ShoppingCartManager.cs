using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.WebFramework;
using Nesoft.ECWeb.MobileService.Models.Order;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.Utility;
using Nesoft.ECWeb.UI;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class ShoppingCartManager
    {

        #region 获取购物车

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <returns></returns>
        public static CartResultModel GetCart(string proSysNos = null, string packSysNos = null)
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            string checkResultMessage = "";
            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        #endregion

        #region 加入购物车

        public static AjaxResult AddToShoppingCart(UpdateCartReqModel req)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;
            int totalProduct = 0;

            #region Check
            string checkResultMessage = "";
            if (req.SysNo <= 0)
            {
                if (req.IsPackage)
                {
                    checkResultMessage = "请输入正确的套餐编号！";
                }
                else
                {
                    checkResultMessage = "请输入正确的商品编号！";
                }
            }
            else if (req.Qty <= 0)
            {
                checkResultMessage = "请输入正确的商品数量";
            }

            #endregion

            #region 加入购物车
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                result.Success = true;
                checkResultMessage = "加入购物车失败";
                ShoppingItemGroup shoppingItemGroup = ShoppingFacade.BuildShoppingItemGroup(req.IsPackage ? "package" : "product", req.SysNo, req.Qty);
                ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();

                if (shoppingItemGroup != null)
                {
                    ShoppingFacade.AddToShoppingCart(shoppingItemGroup, shoppingCart);
                    ShoppingStorageManager.SaveShoppingCart(shoppingCart);
                    checkResultMessage = "加入购物车成功";
                }
                //计算购物商品数量
                if (shoppingCart != null && shoppingCart.ShoppingItemGroupList != null)
                {
                    foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
                    {
                        foreach (var item in itemGroup.ShoppingItemList)
                        {
                            totalProduct += itemGroup.Quantity * item.UnitQuantity;


                        }
                    }
                }
            }
            result.Data = totalProduct;
            result.Message = checkResultMessage;

            #endregion

            return result;
        }

        #endregion

        #region 修改购物车数量
        /// <summary>
        /// 修改数量
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel UpdateCart(UpdateCartReqModel req, string proSysNos = null, string packSysNos = null)
        {
            if (req != null)
            {
                if (req.IsPackage)
                {
                    return UpdatePackage(req.SysNo, req.Qty, proSysNos, packSysNos);
                }
                else
                {
                    return UpdateProduct(req.SysNo, req.Qty, proSysNos, packSysNos);
                }
            }

            return null;
        }

        /// <summary>
        /// 修改数量
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        private static CartResultModel UpdateProduct(int productSysNo, int qty, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (productSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (qty <= 0)
            {
                checkResultMessage = "请输入正确的商品数量";
            }

            #endregion

            #region 更改数量

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //更改数量
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageType.Equals(0))
                    {
                        item.ShoppingItemList.ForEach(m =>
                        {
                            if (m.ProductSysNo.Equals(productSysNo))
                            {
                                item.Quantity = qty;
                            }
                        });
                    }
                });
            }

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);

            #endregion
        }

        /// <summary>
        /// 修改套餐数量
        /// </summary>
        /// <param name="packageSysNo"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        private static CartResultModel UpdatePackage(int packageSysNo, int qty, string proSysNos = null, string packSysNos = null)
        {
            #region Check
            string checkResultMessage = "";
            if (packageSysNo <= 0)
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (qty <= 0)
            {
                checkResultMessage = "请输入正确的商品数量";
            }
            #endregion

            #region 更改数量

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //更改数量
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageType.Equals(1) && item.PackageNo.Equals(packageSysNo))
                    {
                        item.Quantity = qty;
                    }
                });
            }

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);

            #endregion
        }

        #endregion

        #region 删除购物商品

        public static CartResultModel DelCart(DelCartReqModel req,string proSysNos = null, string packSysNos = null)
        {
            if (req != null)
            {
                if (req.ProductSysNo > 0 && req.PackageSysNo > 0)
                {
                    return DelPackageProduct(req.PackageSysNo, req.ProductSysNo, proSysNos, packSysNos);
                }
                else
                {
                    if (req.PackageSysNo > 0)
                    {
                        return DelPackage(req.PackageSysNo, proSysNos, packSysNos);
                    }
                    else if (req.ProductSysNo > 0)
                    {
                        return DelProduct(req.ProductSysNo, proSysNos, packSysNos);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 删除购物车中指定的商品
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private static CartResultModel DelProduct(int productSysNo, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (productSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }

            #endregion

            #region 删除商品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();

            shoppingCart.ShoppingItemGroupList =
                shoppingCart.ShoppingItemGroupList.FindAll(m
                    => (m.PackageType.Equals(0) && !m.ShoppingItemList[0].ProductSysNo.Equals(productSysNo))
                    || m.PackageType.Equals(1));

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <param name="packageSysNo"></param>
        /// <returns></returns>
        private static CartResultModel DelPackage(int packageSysNo, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (packageSysNo <= 0)
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }

            #endregion

            #region 删除套餐商品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除套餐
                shoppingCart.ShoppingItemGroupList =
                    shoppingCart.ShoppingItemGroupList.FindAll(m
                        => (m.PackageType.Equals(1) && !m.PackageNo.Equals(packageSysNo))
                        || m.PackageType.Equals(0));
            }

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 删除购物车中指定套餐中的某商品
        /// </summary>
        /// <note>Request Param：
        /// PackageSysNo-套餐编号
        /// ProductSysNo-商品编号
        /// </note>
        /// <returns></returns>
        private static CartResultModel DelPackageProduct(int packageSysNo, int productSysNo, string proSysNos = null, string packSysNos = null)
        {
            #region 1.Check
            string checkResultMessage = "";
            if (packageSysNo <= 0)
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (productSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            #endregion

            #region 2.删除套餐中的商品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();

            ShoppingCart newShoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除套餐中的商品
                newShoppingCart = DelPackageProductCalcShoppingCart(shoppingCart, packageSysNo, productSysNo);
            }
            #endregion

            return BuilderOrderResultModel(newShoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 删除套餐中的商品
        /// </summary>
        /// <param name="shoppingCart">购物车Cookie</param>
        /// <param name="packageSysNo">套餐编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        private static ShoppingCart DelPackageProductCalcShoppingCart(ShoppingCart shoppingCart, int packageSysNo, int productSysNo)
        {
            ShoppingCart newShoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            ShoppingItemGroup delItem = new ShoppingItemGroup();
            shoppingCart.ShoppingItemGroupList.ForEach(item =>
            {
                if (item.PackageType.Equals(1) && item.PackageNo.Equals(packageSysNo))
                {
                    delItem = item;
                }
                else
                {
                    newShoppingCart.ShoppingItemGroupList.Add(item);
                }
            });
            //已合并的商品列表
            List<int> mergeProductList = new List<int>();
            //删除该套餐中的指定商品
            delItem.ShoppingItemList = delItem.ShoppingItemList.FindAll(m => !m.ProductSysNo.Equals(productSysNo));
            //检查是否需要合并
            newShoppingCart.ShoppingItemGroupList.ForEach(item =>
            {
                //只有单个商品才合并，套餐中的该商品不合并在一起
                if (item.PackageType.Equals(0))
                {
                    delItem.ShoppingItemList.ForEach(m =>
                    {
                        if (item.ShoppingItemList[0].ProductSysNo.Equals(m.ProductSysNo))
                        {
                            item.Quantity += delItem.Quantity * m.UnitQuantity;
                            mergeProductList.Add(m.ProductSysNo);
                        }
                    });
                }
            });
            delItem.ShoppingItemList.ForEach(m =>
            {
                if (!mergeProductList.Exists(x => x.Equals(m.ProductSysNo)))
                {
                    //该商品未被合并，需要单独新加一个商品
                    ShoppingItemGroup needSingleAddItemGroup = new ShoppingItemGroup()
                    {
                        PackageType = 0,
                        PackageNo = 0,
                        Quantity = delItem.Quantity,
                        ShoppingItemList = delItem.ShoppingItemList.FindAll(y => y.ProductSysNo.Equals(m.ProductSysNo))
                    };
                    needSingleAddItemGroup.Quantity = needSingleAddItemGroup.Quantity * needSingleAddItemGroup.ShoppingItemList[0].UnitQuantity;
                    needSingleAddItemGroup.ShoppingItemList[0].UnitQuantity = 1;

                    newShoppingCart.ShoppingItemGroupList.Add(needSingleAddItemGroup);
                }
            });
            return newShoppingCart;
        }

        #endregion

        #region 商品赠品

        /// <summary>
        /// 删除购物车中某商品的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel DelGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ProductSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (req.GiftSysNo <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 删除商品的赠品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除商品的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(req.PackageSysNo))
                    {
                        if (req.PackageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(req.ProductSysNo))
                            {
                                if (!item.ShoppingItemList[0].DeleteGiftSysNo.Exists(x => x.Equals(req.GiftSysNo)))
                                {
                                    item.ShoppingItemList[0].DeleteGiftSysNo.Add(new ShoppingOrderGift()
                                    {
                                        ActivityNo = req.ActivityNo,
                                        GiftSysNo = req.GiftSysNo
                                    });
                                }
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(req.ProductSysNo))
                                {
                                    if (!m.DeleteGiftSysNo.Exists(x => x.Equals(req.GiftSysNo)))
                                    {
                                        m.DeleteGiftSysNo.Add(new ShoppingOrderGift()
                                        {
                                            ActivityNo = req.ActivityNo,
                                            GiftSysNo = req.GiftSysNo
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            }
            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 选择购物车中某商品的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel SltGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ProductSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (req.GiftSysNo <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 选择商品的赠品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //选择商品的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(req.PackageSysNo))
                    {
                        if (req.PackageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(req.ProductSysNo))
                            {
                                item.ShoppingItemList[0].SelectGiftSysNo =
                                    item.ShoppingItemList[0].SelectGiftSysNo.FindAll(m =>
                                        m.ActivityNo != req.ActivityNo);
                                if (item.ShoppingItemList[0].SelectGiftSysNo == null)
                                    item.ShoppingItemList[0].SelectGiftSysNo = new List<ShoppingOrderGift>();
                                item.ShoppingItemList[0].SelectGiftSysNo.Add(new ShoppingOrderGift()
                                {
                                    ActivityNo = req.ActivityNo,
                                    GiftSysNo = req.GiftSysNo
                                });
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(req.ProductSysNo))
                                {
                                    m.SelectGiftSysNo = m.SelectGiftSysNo.FindAll(x =>
                                        x.ActivityNo != req.ActivityNo);
                                    if (m.SelectGiftSysNo == null)
                                        m.SelectGiftSysNo = new List<ShoppingOrderGift>();
                                    item.ShoppingItemList[0].SelectGiftSysNo.Add(new ShoppingOrderGift()
                                    {
                                        ActivityNo = req.ActivityNo,
                                        GiftSysNo = req.GiftSysNo
                                    });
                                }
                            });
                        }
                    }
                });
            }

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 删除购物车中某商品选择的赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel DelSltGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ProductSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (req.GiftSysNo <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 删除商品选择的赠品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除商品选择的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(req.PackageSysNo))
                    {
                        if (req.PackageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(req.ProductSysNo))
                            {
                                item.ShoppingItemList[0].SelectGiftSysNo =
                                    item.ShoppingItemList[0].SelectGiftSysNo.FindAll(m =>
                                        m.ActivityNo != req.ActivityNo
                                        && m.GiftSysNo != req.GiftSysNo);
                                if (item.ShoppingItemList[0].SelectGiftSysNo == null)
                                    item.ShoppingItemList[0].SelectGiftSysNo = new List<ShoppingOrderGift>();
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(req.ProductSysNo))
                                {
                                    m.SelectGiftSysNo = m.SelectGiftSysNo.FindAll(x =>
                                        x.ActivityNo != req.ActivityNo
                                        && x.GiftSysNo != req.GiftSysNo);
                                    if (m.SelectGiftSysNo == null)
                                        m.SelectGiftSysNo = new List<ShoppingOrderGift>();
                                }
                            });
                        }
                    }
                });
            }

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        #endregion

        #region 订单赠品

        /// <summary>
        /// 删除购物车中订单上某活动的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel DelOrderGift(OrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ActivityNo <= 0)
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (req.GiftSysNo <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 2.删除订单上某活动的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除订单上某活动的赠品
                if (shoppingCart.OrderDeleteGiftSysNo == null)
                    shoppingCart.OrderDeleteGiftSysNo = new List<ShoppingOrderGift>();
                shoppingCart.OrderDeleteGiftSysNo.Add(new ShoppingOrderGift()
                {
                    ActivityNo = req.ActivityNo,
                    GiftSysNo = req.GiftSysNo
                });
            }
            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 删除购物车中订单上某活动选择的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel DelOrderSltGift(OrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ActivityNo <= 0)
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (req.GiftSysNo <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 删除订单上某活动选择的赠品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除订单上某活动选择的赠品
                if (shoppingCart.OrderSelectGiftSysNo == null)
                    shoppingCart.OrderSelectGiftSysNo = new List<ShoppingOrderGift>();
                shoppingCart.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo.FindAll(m => m.ActivityNo != req.ActivityNo || m.GiftSysNo != req.GiftSysNo);
            }

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 选择购物车中订单上某活动的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static CartResultModel SltOrderGift(SltOrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            #region Check

            string checkResultMessage = "";
            if (req.ActivityNo <= 0)
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (req.GiftSysNos == null || req.GiftSysNos.Count <= 0)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }

            #endregion

            #region 选择订单上某活动的赠品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //选择订单上某活动的赠品
                if (shoppingCart.OrderSelectGiftSysNo == null)
                    shoppingCart.OrderSelectGiftSysNo = new List<ShoppingOrderGift>();
                //剔除当前活动已选择的赠品
                shoppingCart.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo.FindAll(m => !m.ActivityNo.Equals(req.ActivityNo));
                foreach (int sysNo in req.GiftSysNos)
                {
                    shoppingCart.OrderSelectGiftSysNo.Add(new ShoppingOrderGift()
                    {
                        ActivityNo = req.ActivityNo,
                        GiftSysNo = sysNo
                    });
                }
            }

            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        #endregion

        #region 加够商品

        /// <summary>
        /// 选择加够商品
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static CartResultModel SltPlusBuyProduct(List<int> productSysNoList, string proSysNos = null, string packSysNos = null)
        {
            #region Check
            string checkResultMessage = "";
            if (productSysNoList == null || productSysNoList.Count <= 0)
            {
                checkResultMessage = "请选择加够商品！";
            }
            #endregion

            #region 选择加够商品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew(); //此处于KJT逻辑不通，保留了TL的逻辑，如果有问题，请修改
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                if (shoppingCart.PlusPriceProductSelectList == null)
                    shoppingCart.PlusPriceProductSelectList = new List<int>();
                foreach (int sysNo in productSysNoList)
                {
                    if (!shoppingCart.PlusPriceProductSelectList.Exists(m => m == sysNo))
                        shoppingCart.PlusPriceProductSelectList.Add(sysNo);
                }
            }
            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        /// <summary>
        /// 移除加够商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static CartResultModel DelPlusBuyProduct(int productSysNo, string proSysNos = null, string packSysNos = null)
        {
            #region Check
            string checkResultMessage = "";
            if (productSysNo <= 0)
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            #endregion

            #region 选择加够商品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew(); //此处于KJT逻辑不通，保留了TL的逻辑，如果有问题，请修改
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                if (shoppingCart.PlusPriceProductSelectList == null)
                    shoppingCart.PlusPriceProductSelectList = new List<int>();
                shoppingCart.PlusPriceProductSelectList = shoppingCart.PlusPriceProductSelectList.FindAll(m => m != productSysNo);
            }
            #endregion

            return BuilderOrderResultModel(shoppingCart, checkResultMessage, proSysNos, packSysNos);
        }

        #endregion

        #region 私有方法

        private static CartResultModel BuilderOrderResultModel(ShoppingCart shoppingCart, string errorMessage = null, string proSysNos = null, string packSysNos = null)
        {
            
            if (proSysNos != null || packSysNos != null)
            {
                foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
                {
                    itemGroup.PackageChecked = false;
                    foreach (ShoppingItem ProductItem in itemGroup.ShoppingItemList)
                    {
                        ProductItem.ProductChecked = false;
                    }
                }
                //checkbox单个商品购买
                if (!string.IsNullOrEmpty(proSysNos))
                {
                    string[] array = proSysNos.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
                            {
                                if (itemGroup.PackageType.Equals(0))
                                {
                                    if (!itemGroup.PackageChecked)
                                    {
                                        foreach (ShoppingItem ProductItem in itemGroup.ShoppingItemList)
                                        {

                                            if (ProductItem.ProductSysNo == sysNo && !ProductItem.ProductChecked)
                                            {
                                                itemGroup.PackageChecked = true;
                                                ProductItem.ProductChecked = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //checkbox套餐商品购买
                if (!string.IsNullOrEmpty(packSysNos))
                {
                    string[] array = packSysNos.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
                            {
                                if (itemGroup.PackageType.Equals(1))
                                {
                                    if (!itemGroup.PackageChecked)
                                    {
                                        if (itemGroup.PackageNo == sysNo)
                                        {
                                            itemGroup.PackageChecked = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ////ShoppingCart pipelineShoppingCart = shoppingCart;
            //if (shoppingCart.ShoppingItemGroupList.Count == 1)
            //{
            //    foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
            //    {
            //        foreach (ShoppingItem ProductItem in itemGroup.ShoppingItemList)
            //        {
            //            itemGroup.PackageChecked = true;
            //            ProductItem.ProductChecked = true;
            //        }
            //    }
            //}
            //if (shoppingCart.ShoppingItemGroupList.Count > 1)
            //{
            //    if (shoppingCart.ShoppingItemGroupList.FindAll(m => m.PackageChecked).Count == 0)
            //    {
            //        if (shoppingCart.ShoppingItemGroupList[0].PackageType.Equals(1))
            //        {
            //            shoppingCart.ShoppingItemGroupList[0].PackageChecked = true;
            //        }
            //        if (shoppingCart.ShoppingItemGroupList[0].PackageType.Equals(0))
            //        {
            //            foreach (ShoppingItem ProductItem in shoppingCart.ShoppingItemGroupList[0].ShoppingItemList)
            //            {
            //                shoppingCart.ShoppingItemGroupList[0].PackageChecked = true;
            //                ProductItem.ProductChecked = true;
            //            }
            //        }
            //    }
            //}
            LoginUser userInfo = UserMgr.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            OrderPipelineProcessResult shoppingResult = ShoppingFacade.BuildShoppingCart(shoppingCart);
            ShoppingCart pipelineShoppingCart = (shoppingResult.ReturnData != null
                && shoppingResult.ReturnData["ShoppingCart"] != null)
                ? shoppingResult.ReturnData["ShoppingCart"] as ShoppingCart
                : new ShoppingCart();
            shoppingCart.OrderSelectGiftSysNo = pipelineShoppingCart.OrderSelectGiftSysNo;
            shoppingCart.OrderDeleteGiftSysNo = pipelineShoppingCart.OrderDeleteGiftSysNo;

            CartResultModel model = new CartResultModel();
            model.HasSucceed = shoppingResult.HasSucceed;
            model.ErrorMessages = shoppingResult.ErrorMessages;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                model.ErrorMessages.Add(errorMessage);
            }

            OrderInfo orderInfo = shoppingResult.ReturnData;
            if (orderInfo != null)
            {
                OrderInfoModel orderInfoModel = OrderMapping.MappingOrderInfo(orderInfo,"Cart");
                if (!string.IsNullOrEmpty(orderInfo.WarmTips))
                {
                    model.ErrorMessages.Add(orderInfo.WarmTips);
                }

                model.ReturnData = orderInfoModel;
            }

            ShoppingStorageManager.SaveShoppingCart(shoppingCart);

            return model;
        }

        #endregion
    }
}