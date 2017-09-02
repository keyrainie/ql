using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Shopping;
using ECommerce.SOPipeline;
using ECommerce.WebFramework;
using ECommerce.DataAccess.Shopping;
using ECommerce.Utility;

namespace ECommerce.Facade.Shopping
{
    /// <summary>
    /// 购物存储Cookie操作
    /// </summary>
    public class ShoppingStorageManager
    {
        static string MY_SHOPPINGCART_COOKIE_NAME = "MyShoppingCart";
        static string MY_SHOPPINGCARTMINI_COOKIE_NAME = "MyShoppingCartMini";
        static string LATEST_SO_COOKIE_NAME = "SoSysNo";
        static string MY_GIFTCARDCART_COOKIE_NAME = "MyGiftCardCart";
        static string MY_SHOPPINGCART_SIGN_KEY = "MyShoppingCartKey";

        public static ShoppingCart GetShoppingCartByCustomer(int userSysNo)
        {
            ShoppingCart shoppingCart  = GetShoppingCartFromCookieOrCreateNew();
            if (shoppingCart == null)
                shoppingCart = new ShoppingCart();
            string key = CookieHelper.GetCookie<string>(MY_SHOPPINGCART_SIGN_KEY);
            if (String.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString("N");
                CookieHelper.SaveCookie<string>(MY_SHOPPINGCART_SIGN_KEY, key);
            }
            shoppingCart.ShoppingCartID = key;
            if (userSysNo > 0)
            {
                //获取数据库最新数据购物车
                ShoppingCartPersistent shoppingCartPersistent = ShoppingPersistentDA.GetShoppingCartByCustomer(userSysNo);
                ShoppingCart newShoppingCart = new ShoppingCart();
                if (shoppingCartPersistent != null && !string.IsNullOrEmpty(shoppingCartPersistent.ShoppingCart))
                {
                    newShoppingCart = SerializationUtility.JsonDeserialize2<ShoppingCart>(shoppingCartPersistent.ShoppingCart);
                }
                //判断购物车是否发生变化
                if (newShoppingCart.ShoppingCartID != shoppingCart.ShoppingCartID)
                {
                    newShoppingCart.ShoppingCartID = shoppingCart.ShoppingCartID;
                    //合并购物车
                    MergeShoppingCart(shoppingCart, newShoppingCart);
                }
                //写入DB
                ShoppingPersistentDA.SaveShoppingCart(new ShoppingCartPersistent()
                {
                    CustomerSysNo = userSysNo,
                    ShoppingCart = SerializationUtility.JsonSerialize2(shoppingCart),
                    ShoppingCartMini = ""
                });
            }
            else
            {
                shoppingCart = GetShoppingCartFromCookie();
            }
            //写入Cookie
            CookieHelper.SaveCookie<ShoppingCart>(MY_SHOPPINGCART_COOKIE_NAME, shoppingCart);
            return shoppingCart;
        }
        /// <summary>
        /// 保存购物车到cookie和db
        /// </summary>
        /// <param name="shoppingCart"></param>
        public static void SaveShoppingCart(ShoppingCart shoppingCart)
        {
            string key = CookieHelper.GetCookie<string>(MY_SHOPPINGCART_SIGN_KEY);
            if (String.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString("N");
                CookieHelper.SaveCookie<string>(MY_SHOPPINGCART_SIGN_KEY, key);
            }
            shoppingCart.ShoppingCartID = key;
            if (shoppingCart.CustomerSysNo > 0)
            {
                //获取数据库最新数据购物车
                ShoppingCartPersistent shoppingCartPersistent = ShoppingPersistentDA.GetShoppingCartByCustomer(shoppingCart.CustomerSysNo);
                ShoppingCart newShoppingCart = new ShoppingCart();
                if (shoppingCartPersistent != null && !string.IsNullOrEmpty(shoppingCartPersistent.ShoppingCart))
                {
                    newShoppingCart = SerializationUtility.JsonDeserialize2<ShoppingCart>(shoppingCartPersistent.ShoppingCart);
                }
                //判断购物车是否发生变化
                if (newShoppingCart.ShoppingCartID != shoppingCart.ShoppingCartID)
                {
                    newShoppingCart.ShoppingCartID = shoppingCart.ShoppingCartID;
                    //合并购物车
                    MergeShoppingCart(shoppingCart, newShoppingCart);
                }
                //写入DB
                ShoppingPersistentDA.SaveShoppingCart(new ShoppingCartPersistent()
                {
                    CustomerSysNo = shoppingCart.CustomerSysNo,
                    ShoppingCart = SerializationUtility.JsonSerialize2(shoppingCart),
                    ShoppingCartMini = ""
                });
            }
            //写入Cookie
            CookieHelper.SaveCookie<ShoppingCart>(MY_SHOPPINGCART_COOKIE_NAME, shoppingCart);
            
        }
        /// <summary>
        /// 保存购物车Mini到cookie和db
        /// </summary>
        /// <param name="shoppingCartMini"></param>
        public static void SaveShoppingCartMini(ShoppingCartMiniResult shoppingCartMini)
        {
            //写入Cookie
            CookieHelper.SaveCookie<ShoppingCartMiniResult>(MY_SHOPPINGCARTMINI_COOKIE_NAME, shoppingCartMini);
        }

        /// <summary>
        /// 合并购物车
        /// </summary>
        /// <param name="toShoppingCart">合并至</param>
        /// <param name="fromShoppingCart">从合并</param>
        /// <returns></returns>
        private static ShoppingCart MergeShoppingCart(ShoppingCart toShoppingCart, ShoppingCart fromShoppingCart)
        {
            if (toShoppingCart == null
                || toShoppingCart.ShoppingItemGroupList == null)
                return fromShoppingCart;

            foreach (ShoppingItemGroup itemGroup in fromShoppingCart.ShoppingItemGroupList)
            {
                if (itemGroup.PackageType == 1)
                {
                    #region 套餐合并
                    if (toShoppingCart.ShoppingItemGroupList.Exists(m => m.PackageNo == itemGroup.PackageNo))
                    {
                        //存在相同的套餐，则直接累加数量
                        foreach (ShoppingItemGroup loginItemGroup in toShoppingCart.ShoppingItemGroupList)
                        {
                            if (loginItemGroup.PackageNo == itemGroup.PackageNo)
                            {
                                loginItemGroup.PackageChecked = true;
                                loginItemGroup.Quantity += itemGroup.Quantity;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //不存在相同的套餐，则直接添加
                        toShoppingCart.ShoppingItemGroupList.Add(itemGroup);
                    }
                    #endregion
                }
                else
                {
                    #region 商品合并
                    //存在相同的商品，则直接累加数量
                    bool isExistsCurrProduct = false;
                    foreach (ShoppingItemGroup loginItemGroup in toShoppingCart.ShoppingItemGroupList)
                    {
                        foreach (ShoppingItem loginItem in loginItemGroup.ShoppingItemList)
                        {
                            if (loginItemGroup.PackageType == 0
                                && loginItem.ProductSysNo == itemGroup.ShoppingItemList[0].ProductSysNo)
                            {
                                loginItemGroup.PackageChecked = true;
                                loginItemGroup.Quantity += itemGroup.Quantity;
                                isExistsCurrProduct = true;
                                break;
                            }
                        }
                    }
                    if (!isExistsCurrProduct)
                    {
                        //不存在相同的商品，则直接添加
                        toShoppingCart.ShoppingItemGroupList.Add(itemGroup);
                    }
                    #endregion
                }
            }
            return toShoppingCart;
        }

        /// <summary>
        /// 从cookie中移除购物车
        /// </summary>
        public static void RemoveShoppingCartCookie()
        {
            SaveShoppingCart(GetShoppingCartFromCreateNew());
            //CookieHelper.RemoveCookie(MY_SHOPPINGCART_COOKIE_NAME);
        }
        /// <summary>
        /// 从cookie中移除mini购物车
        /// </summary>
        public static void RemoveShoppingCartMiniCookie()
        {
            ShoppingCartMiniResult shoppingCartMini = new ShoppingCartMiniResult();
            shoppingCartMini.ItemList = new List<ShoppingCartMiniItem>();
            SaveShoppingCartMini(shoppingCartMini);
        }

        /// <summary>
        /// 从cookie里创建一个购物车对象
        /// </summary>
        /// <returns></returns>
        public static ShoppingCart GetShoppingCartFromCookie()
        {
            ShoppingCart shoppingCart = CookieHelper.GetCookie<ShoppingCart>(MY_SHOPPINGCART_COOKIE_NAME);
            if (shoppingCart != null)
            {
                shoppingCart.ChannelID = ECommerce.Entity.ConstValue.ChannelID;
                shoppingCart.LanguageCode = ECommerce.WebFramework.LanguageHelper.GetLanguageCode();
            }
            return shoppingCart;
        }

        /// <summary>
        /// 从Mini购物车里创建一个Mini购物车对象
        /// </summary>
        /// <returns></returns>
        public static ShoppingCartMiniResult GetShoppingCartMiniFromCookie()
        {
            ShoppingCartMiniResult shoppingCartMini = CookieHelper.GetCookie<ShoppingCartMiniResult>(MY_SHOPPINGCARTMINI_COOKIE_NAME);
            if (shoppingCartMini == null)
            {
                shoppingCartMini = new ShoppingCartMiniResult();
                shoppingCartMini.ItemList = new List<ShoppingCartMiniItem>();
            }
            return shoppingCartMini;
        }

        /// <summary>
        /// 从cookie里或者新创建一个购物车对象
        /// </summary>
        /// <returns></returns>
        public static ShoppingCart GetShoppingCartFromCookieOrCreateNew()
        {
            ShoppingCart shoppingCart = GetShoppingCartFromCookie() ?? GetShoppingCartFromCreateNew();
            return shoppingCart;
        }

        /// <summary>
        /// 重新创建一个购物车对象
        /// </summary>
        /// <returns></returns>
        public static ShoppingCart GetShoppingCartFromCreateNew()
        {

            ShoppingCart shoppingCart = new ShoppingCart()
            {

                ChannelID = ECommerce.Entity.ConstValue.ChannelID,
                LanguageCode = ECommerce.WebFramework.LanguageHelper.GetLanguageCode()
            };

            return shoppingCart;
        }

        /// <summary>
        /// 从传入的商品参数创建一个购物车对象
        /// </summary>
        /// <param name="productSysNo">商品编号，多个商品编号用逗号（,）隔开</param>
        /// <param name="quantity">商品数量，多个购买数量用逗号（,）隔开</param>
        /// <returns></returns>
        public static ShoppingCart GetShoppingCartFromParam(string productSysNo, string quantity)
        {
            if (String.IsNullOrWhiteSpace(productSysNo) || String.IsNullOrWhiteSpace(quantity))
            {
                return null;
            }

            String[] productSysnoArray = productSysNo.Split(new char[] { ',' });
            String[] quantityArray = quantity.Split(new char[] { ',' });

            if (productSysnoArray.Length != quantityArray.Length)
            {
                return null;
            }

            int temp;
            if (productSysnoArray.Any(x => !int.TryParse(x, out temp) || temp <= 0)
                || quantityArray.Any(x => !int.TryParse(x, out temp) || temp <= 0))
            {
                return null;
            }

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            shoppingCart.ShoppingItemGroupList = new List<ShoppingItemGroup>();

            for (int i = 0; i < productSysnoArray.Length; i++)
            {
                var psysno = int.Parse(productSysnoArray[i]);
                var qty = int.Parse(quantityArray[i]);

                if (!shoppingCart.ShoppingItemGroupList.Exists(group =>
                {
                    if (group.ShoppingItemList == null)
                    {
                        return false;
                    }
                    return group.ShoppingItemList.Exists(x =>
                    {
                        if (x.ProductSysNo == psysno)
                        {
                            group.Quantity += qty;
                            return true;
                        }
                        return false;
                    });
                }))
                {
                    shoppingCart.ShoppingItemGroupList.Add(new ShoppingItemGroup()
                    {
                        Quantity = qty,
                        ShoppingItemList = new List<ShoppingItem>()
                          {
                              new ShoppingItem() {
                                  ProductSysNo = psysno,
                                  UnitQuantity = 1
                              }
                          }
                    });
                }
            }

            return shoppingCart;
        }

        /// <summary>
        /// 保存最近订单编号Cookie
        /// </summary>
        /// <param name="soSysNoList"></param>
        public static void SaveLatestSO(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count <= 0) return;

            String latestSO = CookieHelper.GetCookie<String>(LATEST_SO_COOKIE_NAME);

            if (!string.IsNullOrEmpty(latestSO))
            {
                var strs = latestSO.Split(new[] { ',' }).ToList();
                strs.ForEach(p =>
                {
                    int sysNo;
                    if (int.TryParse(p, out sysNo))
                    {
                        soSysNoList.Add(sysNo);
                    }
                });
            }
            String newLastSOString = String.Join(",", soSysNoList);

            CookieHelper.SaveCookie<String>(LATEST_SO_COOKIE_NAME, newLastSOString);
        }

        public static int GetShoppingCartCount()
        {
            int totalProductCount = 0;
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
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
            return totalProductCount;
        }

        /// <summary>
        /// 保存礼品卡cookie
        /// </summary>
        /// <param name="cart"></param>
        public static void SaveGiftCardCart(List<GiftCardCart> cart)
        {
            CookieHelper.SaveCookie<List<GiftCardCart>>(MY_GIFTCARDCART_COOKIE_NAME, cart);
        }
        /// <summary>
        /// 获取礼品卡cookie
        /// </summary>
        /// <returns></returns>
        public static List<GiftCardCart> GetGiftCardCart()
        {
            List<GiftCardCart> cart = CookieHelper.GetCookie<List<GiftCardCart>>(MY_GIFTCARDCART_COOKIE_NAME);
            if (cart == null)
            {
                cart = new List<GiftCardCart>();
            }
            return cart;
        }
        /// <summary>
        /// 清除礼品卡cookie
        /// </summary>
        public static void ClearGiftCardCart()
        {
            List<GiftCardCart> cart = new List<GiftCardCart>();
            SaveGiftCardCart(cart);
        }
        /// <summary>
        /// 获取CartcookieName
        /// </summary>
        /// <returns></returns>
        public static string GetConfigCartName()
        {
            return CookieHelper.GetConfigCartName(MY_SHOPPINGCART_COOKIE_NAME);
        }
    }
}
