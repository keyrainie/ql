using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.WebFramework;

namespace Nesoft.ECWeb.M.Controllers
{
    public class ShoppingCartController : Controller
    {
        //
        // GET: /Shopping/

        public ActionResult Index()
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            OrderPipelineProcessResult shoppingResult = ShoppingFacade.BuildShoppingCart(shoppingCart);
            ShoppingCart pipelineShoppingCart = (shoppingResult.ReturnData != null
                && shoppingResult.ReturnData["ShoppingCart"] != null)
                ? shoppingResult.ReturnData["ShoppingCart"] as ShoppingCart
                : ShoppingStorageManager.GetShoppingCartFromCreateNew();
            shoppingCart.OrderSelectGiftSysNo = pipelineShoppingCart.OrderSelectGiftSysNo;
            shoppingCart.OrderDeleteGiftSysNo = pipelineShoppingCart.OrderDeleteGiftSysNo;

            //Key添加CustomerSysNo
            LoginUser userInfo = UserManager.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            ShoppingStorageManager.SaveShoppingCart(shoppingCart);

            return View(shoppingResult);
        }

        #region 加入购物车

        /// <summary>
        /// 加入购物车
        /// </summary>
        /// <note>
        /// Request Param：
        /// Category-加入的对象类型；Product-商品，Package-套餐
        /// SysNo-根据加入的对象类型对应的系统编号
        /// Qty-数量
        /// e.g：?Category=Package&SysNo=1006&Qty=2
        /// </note>
        /// <returns></returns>
        public ActionResult AddToShoppingCart()
        {
            string shoppingCartUrl = PageHelper.BuildUrl("ShoppingCart");
            #region 1.Check
            int sysNo = 0;
            int qty = 0;
            string category = Request.Params["Category"];
            if (string.IsNullOrWhiteSpace(category) || (category.Equals("Product") && category.Equals("Package")))
            {
                return Redirect(shoppingCartUrl);
            }
            else if (!int.TryParse(Request.Params["SysNo"], out sysNo) || sysNo <= 0)
            {
                return Redirect(shoppingCartUrl);
            }
            else if (!int.TryParse(Request.Params["Qty"], out qty) || qty <= 0)
            {
                return Redirect(shoppingCartUrl);
            }
            #endregion

            #region 加入购物车
            ShoppingItemGroup shoppingItemGroup = ShoppingFacade.BuildShoppingItemGroup(category, sysNo, qty);
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            //Key添加CustomerSysNo
            LoginUser userInfo = UserManager.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            if (shoppingItemGroup != null)
            {
                ShoppingFacade.AddToShoppingCart(shoppingItemGroup, shoppingCart);
                ShoppingStorageManager.SaveShoppingCart(shoppingCart);

            }
            #endregion

            return Redirect(shoppingCartUrl);
        }

        #endregion

        #region 编辑购物车

        /// <summary>
        /// 加载购物车信息
        /// </summary>
        /// <returns>购物车信息</returns>
        private PartialViewResult LoadShoppingCartData(ShoppingCart shoppingCart, string errorMessage = "")
        {
            //Key添加CustomerSysNo
            LoginUser userInfo = UserManager.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            OrderPipelineProcessResult shoppingResult = ShoppingFacade.BuildShoppingCart(shoppingCart);
            ShoppingCart pipelineShoppingCart = (shoppingResult.ReturnData != null
                && shoppingResult.ReturnData["ShoppingCart"] != null)
                ? shoppingResult.ReturnData["ShoppingCart"] as ShoppingCart
                : ShoppingStorageManager.GetShoppingCartFromCreateNew();
            shoppingCart.OrderSelectGiftSysNo = pipelineShoppingCart.OrderSelectGiftSysNo;
            shoppingCart.OrderDeleteGiftSysNo = pipelineShoppingCart.OrderDeleteGiftSysNo;
            ShoppingStorageManager.SaveShoppingCart(shoppingCart);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                //更改失败返回错误信息
                if (shoppingResult.ErrorMessages != null)
                {
                    shoppingResult.ErrorMessages = new List<string>() { errorMessage };
                }
                else
                {
                    shoppingResult.ErrorMessages.Add(errorMessage);
                }
            }

            return PartialView("_ShoppingCartPanel", shoppingResult);
        }

        /// <summary>
        /// 加载购物车商品信息
        /// </summary>
        /// <note>Request Param：N/A</note>
        /// <returns></returns>
        public PartialViewResult Load()
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            return LoadShoppingCartData(shoppingCart);
        }

        /// <summary>
        /// 更改购物车商品数量
        /// </summary>
        /// <note>Request Param：
        /// ProductSysNo-商品编号
        /// Qty-更改后的数量
        /// </note>
        /// <returns></returns>
        public PartialViewResult UpdateProduct()
        {
            #region 1.Check
            string checkResultMessage = "";
            int productSysNo = 0;
            int qty = 0;
            if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (!int.TryParse(Request.Params["Qty"], out qty) || qty <= 0)
            {
                checkResultMessage = "请输入正确的商品数量";
            }
            #endregion

            #region 2.更改数量
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
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 更改购物车套餐数量
        /// </summary>
        /// <note>Request Param：
        /// PackageSysNo-套餐编号
        /// Qty-更改后的数量
        /// </note>
        /// <returns></returns>
        public PartialViewResult UpdatePackage()
        {
            #region 1.Check
            string checkResultMessage = "";
            int packageSysNo = 0;
            int qty = 0;
            if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (!int.TryParse(Request.Params["Qty"], out qty) || qty <= 0)
            {
                checkResultMessage = "请输入正确的商品数量";
            }
            #endregion

            #region 2.更改数量
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
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 删除购物车中指定的商品
        /// </summary>
        /// <note>Request Param：
        /// ProductSysNo-商品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelProduct()
        {
            #region 1.Check
            string checkResultMessage = "";
            int productSysNo = 0;
            if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            #endregion

            #region 2.删除商品

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除商品
                shoppingCart = DelProductCalcShoppingCart(shoppingCart, productSysNo);
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="shoppingCart">购物车Cookie</param>
        /// <param name="productSysNo">需要删除的商品编号</param>
        /// <returns></returns>
        private ShoppingCart DelProductCalcShoppingCart(ShoppingCart shoppingCart, int productSysNo)
        {
            shoppingCart.ShoppingItemGroupList =
                shoppingCart.ShoppingItemGroupList.FindAll(m
                    => (m.PackageType.Equals(0) && !m.ShoppingItemList[0].ProductSysNo.Equals(productSysNo))
                    || m.PackageType.Equals(1));
            return shoppingCart;
        }

        /// <summary>
        /// 删除购物车中指定的套餐
        /// </summary>
        /// <note>Request Param：
        /// PackageSysNo-套餐编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelPackage()
        {
            #region 1.Check
            string checkResultMessage = "";
            int packageSysNo = 0;
            if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            #endregion

            #region 2.删除套餐
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

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 删除购物车中指定套餐中的某商品
        /// </summary>
        /// <note>Request Param：
        /// PackageSysNo-套餐编号
        /// ProductSysNo-商品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelPackageProduct()
        {
            #region 1.Check
            string checkResultMessage = "";
            int packageSysNo = 0;
            int productSysNo = 0;
            if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
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

            return LoadShoppingCartData(newShoppingCart, checkResultMessage);
        }
        /// <summary>
        /// 删除套餐中的商品
        /// </summary>
        /// <param name="shoppingCart">购物车Cookie</param>
        /// <param name="packageSysNo">套餐编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        private ShoppingCart DelPackageProductCalcShoppingCart(ShoppingCart shoppingCart, int packageSysNo, int productSysNo)
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

        /// <summary>
        /// 删除购物车中某商品的某赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// PackageSysNo-套餐编号
        /// ProductSysNo-商品编号
        /// GiftSysNo-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            int activityNo = 0;
            int packageSysNo = 0;
            int productSysNo = 0;
            int giftSysNo = 0;
            if (!int.TryParse(Request.Params["ActivityNo"], out activityNo))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (!int.TryParse(Request.Params["GiftSysNo"], out giftSysNo))
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }
            #endregion

            #region 2.删除商品的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除商品的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(packageSysNo))
                    {
                        if (packageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(productSysNo))
                            {
                                if (!item.ShoppingItemList[0].DeleteGiftSysNo.Exists(x => x.Equals(giftSysNo)))
                                {
                                    item.ShoppingItemList[0].DeleteGiftSysNo.Add(new ShoppingOrderGift()
                                    {
                                        ActivityNo = activityNo,
                                        GiftSysNo = giftSysNo
                                    });
                                }
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(productSysNo))
                                {
                                    if (!m.DeleteGiftSysNo.Exists(x => x.Equals(giftSysNo)))
                                    {
                                        m.DeleteGiftSysNo.Add(new ShoppingOrderGift()
                                        {
                                            ActivityNo = activityNo,
                                            GiftSysNo = giftSysNo
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 选择购物车中某商品的某赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// PackageSysNo-套餐编号
        /// ProductSysNo-商品编号
        /// GiftSysNo-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult SltGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            int activityNo = 0;
            int packageSysNo = 0;
            int productSysNo = 0;
            int giftSysNo = 0;
            if (!int.TryParse(Request.Params["ActivityNo"], out activityNo))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (!int.TryParse(Request.Params["GiftSysNo"], out giftSysNo))
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }
            #endregion

            #region 2.选择商品的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //选择商品的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(packageSysNo))
                    {
                        if (packageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(productSysNo))
                            {
                                item.ShoppingItemList[0].SelectGiftSysNo =
                                    item.ShoppingItemList[0].SelectGiftSysNo.FindAll(m =>
                                        m.ActivityNo != activityNo);
                                if (item.ShoppingItemList[0].SelectGiftSysNo == null)
                                    item.ShoppingItemList[0].SelectGiftSysNo = new List<ShoppingOrderGift>();
                                item.ShoppingItemList[0].SelectGiftSysNo.Add(new ShoppingOrderGift()
                                {
                                    ActivityNo = activityNo,
                                    GiftSysNo = giftSysNo,
                                });
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(productSysNo))
                                {
                                    m.SelectGiftSysNo = m.SelectGiftSysNo.FindAll(x =>
                                        x.ActivityNo != activityNo);
                                    if (m.SelectGiftSysNo == null)
                                        m.SelectGiftSysNo = new List<ShoppingOrderGift>();
                                    item.ShoppingItemList[0].SelectGiftSysNo.Add(new ShoppingOrderGift()
                                    {
                                        ActivityNo = activityNo,
                                        GiftSysNo = giftSysNo,
                                    });
                                }
                            });
                        }
                    }
                });
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 删除购物车中某商品选择的赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// PackageSysNo-套餐编号
        /// ProductSysNo-商品编号
        /// GiftSysNo-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelSltGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            int activityNo = 0;
            int packageSysNo = 0;
            int productSysNo = 0;
            int giftSysNo = 0;
            if (!int.TryParse(Request.Params["ActivityNo"], out activityNo))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (!int.TryParse(Request.Params["PackageSysNo"], out packageSysNo))
            {
                checkResultMessage = "请输入正确的套餐编号！";
            }
            else if (!int.TryParse(Request.Params["ProductSysNo"], out productSysNo))
            {
                checkResultMessage = "请输入正确的商品编号！";
            }
            else if (!int.TryParse(Request.Params["GiftSysNo"], out giftSysNo))
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }
            #endregion

            #region 2.删除商品选择的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除商品选择的赠品
                shoppingCart.ShoppingItemGroupList.ForEach(item =>
                {
                    if (item.PackageNo.Equals(packageSysNo))
                    {
                        if (packageSysNo.Equals(0))
                        {
                            if (item.ShoppingItemList[0].ProductSysNo.Equals(productSysNo))
                            {
                                item.ShoppingItemList[0].SelectGiftSysNo =
                                    item.ShoppingItemList[0].SelectGiftSysNo.FindAll(m =>
                                        m.ActivityNo != activityNo
                                        && m.GiftSysNo != giftSysNo);
                                if (item.ShoppingItemList[0].SelectGiftSysNo == null)
                                    item.ShoppingItemList[0].SelectGiftSysNo = new List<ShoppingOrderGift>();
                            }
                        }
                        else
                        {
                            item.ShoppingItemList.ForEach(m =>
                            {
                                if (m.ProductSysNo.Equals(productSysNo))
                                {
                                    m.SelectGiftSysNo = m.SelectGiftSysNo.FindAll(x =>
                                        x.ActivityNo != activityNo
                                        && x.GiftSysNo != giftSysNo);
                                    if (m.SelectGiftSysNo == null)
                                        m.SelectGiftSysNo = new List<ShoppingOrderGift>();
                                }
                            });
                        }
                    }
                });
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 删除购物车中订单上某活动的某赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// GiftSysNo-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelOrderGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            int activityNo = 0;
            int giftSysNo = 0;
            if (!int.TryParse(Request.Params["ActivityNo"], out activityNo))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (!int.TryParse(Request.Params["GiftSysNo"], out giftSysNo))
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
                    ActivityNo = activityNo,
                    GiftSysNo = giftSysNo
                });
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 删除购物车中订单上某活动选择的某赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// GiftSysNo-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult DelOrderSltGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            int activityNo = 0;
            int giftSysNo = 0;
            if (!int.TryParse(Request.Params["ActivityNo"], out activityNo))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            else if (!int.TryParse(Request.Params["GiftSysNo"], out giftSysNo))
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }
            #endregion

            #region 2.删除订单上某活动选择的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //删除订单上某活动选择的赠品
                if (shoppingCart.OrderSelectGiftSysNo == null)
                    shoppingCart.OrderSelectGiftSysNo = new List<ShoppingOrderGift>();
                shoppingCart.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo.FindAll(m => m.ActivityNo != activityNo || m.GiftSysNo != giftSysNo);
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 选择购物车中订单上某活动的某赠品
        /// </summary>
        /// <note>Request Param：
        /// ActivityNo-活动编号
        /// GiftSysNos-赠品编号
        /// </note>
        /// <returns></returns>
        public PartialViewResult SltOrderGift()
        {
            #region 1.Check
            string checkResultMessage = "";
            List<int> activityNoList = new List<int>();
            Dictionary<int, List<int>> giftSysNoList = new Dictionary<int, List<int>>();
            if (string.IsNullOrWhiteSpace(Request.Params["ActivityNos"]))
            {
                checkResultMessage = "请输入正确的活动编号！";
            }
            string activityNos = Request.Params["ActivityNos"];
            string giftSysNos = Request.Params["GiftSysNos"];
            if (string.IsNullOrWhiteSpace(giftSysNos) || activityNos.Split(',').Length != giftSysNos.Split('|').Length)
            {
                checkResultMessage = "请输入正确的赠品编号！";
            }
            string[] actGiftSysNos = giftSysNos.Split('|');
            for (int i = 0; i < activityNos.Split(',').Length; i++)
            {
                int activityNo = 0;
                if (!int.TryParse(activityNos.Split(',')[i], out activityNo))
                {
                    checkResultMessage = "请输入正确的活动编号！";
                    break;
                }
                activityNoList.Add(activityNo);
                giftSysNoList.Add(activityNo, new List<int>());

                foreach (string no in actGiftSysNos[i].Split(','))
                {
                    int giftSysNo = 0;
                    if (!int.TryParse(no, out giftSysNo))
                    {
                        checkResultMessage = "请输入正确的赠品编号！";
                        break;
                    }
                    giftSysNoList[activityNo].Add(giftSysNo);
                }
            }
            #endregion

            #region 2.选择订单上某活动的赠品
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            if (string.IsNullOrWhiteSpace(checkResultMessage))
            {
                //选择订单上某活动的赠品
                if (shoppingCart.OrderSelectGiftSysNo == null)
                    shoppingCart.OrderSelectGiftSysNo = new List<ShoppingOrderGift>();

                foreach (int activityNo in activityNoList)
                {
                    //剔除当前活动已选择的赠品
                    shoppingCart.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo.FindAll(m => !m.ActivityNo.Equals(activityNo));
                    foreach (int sysNo in giftSysNoList[activityNo])
                    {
                        shoppingCart.OrderSelectGiftSysNo.Add(new ShoppingOrderGift()
                        {
                            ActivityNo = activityNo,
                            GiftSysNo = sysNo
                        });
                    }
                }
            }
            #endregion

            return LoadShoppingCartData(shoppingCart, checkResultMessage);
        }

        /// <summary>
        /// 清空购物车
        /// <note>Request Param：N/A</note>
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ClearAll()
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            return LoadShoppingCartData(shoppingCart);
        }

        #endregion

        #region Mini购物车

        /// <summary>
        /// Mini购物车获取商品数量
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMiniShoppingCartCount()
        {
            int totalProductCount = 0;
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            //Key添加CustomerSysNo
            LoginUser userInfo = UserManager.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            if (shoppingCart != null && shoppingCart.ShoppingItemGroupList != null)
            {
                foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
                {
                    foreach (var item in itemGroup.ShoppingItemList)
                    {
                        totalProductCount += itemGroup.Quantity * item.UnitQuantity;
                    }
                }
            }
            ShoppingStorageManager.SaveShoppingCart(shoppingCart);
            return new JsonResult() { Data = totalProductCount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

    }
}
