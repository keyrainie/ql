using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Facade.Member;
using ECommerce.Facade.Product;

namespace ECommerce.UI.Controllers
{
    public class MemberServiceController : SSLControllerBase
    {
        #region 我的收藏

        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        public ActionResult MyFavorite()
        {
            return View();
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxAddProductToWishList()
        {
            int productSysNo = 0;
            int.TryParse(Request.Params["productSysNo"], out productSysNo);
            LoginUser user = UserMgr.ReadUserInfo();
            if (productSysNo <= 0 || user == null)
            {
                return (JsonResult)BuildAjaxErrorObject("服务端异常！");
            }
            if (!ProductFacade.IsProductWished(productSysNo, CurrUser.UserSysNo))
            {
                CustomerFacade.AddProductToWishList(user.UserSysNo, productSysNo);
            }
            return Json("操作已成功！");
        }

        [HttpPost]
        public ActionResult AjaxDeleteSelectedItems()
        {
            bool isSuccess = true;
            string message = "操作已成功，稍候生效！";
            string data = Request["SelectList"].ToString();
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    isSuccess = false;
                    message = "请选择要删除的收藏!";
                }
                else
                {
                    string[] strList = data.Split(',');
                    for (int i = 0; i < strList.Length; i++)
                    {
                        CustomerFacade.DeleteMyFavorite(Convert.ToInt32(strList[i]));
                    }
                }
            }
            catch
            {
                isSuccess = false;
                message = "系统异常，请稍候重试!";
            }
            return Json(new
            {
                Result = isSuccess,
                Message = message
            });
        }

        /// <summary>
        /// 清空我的收藏
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxDeleteAllFavoriteItems()
        {
            bool isSuccess = true;
            string message = "操作已成功，稍候生效！";
            //string data = Request["SelectList"].ToString();
            //if (string.IsNullOrEmpty(data))
            //{
            //    isSuccess = false;
            //    message = "请选择要删除的收藏!";
            //}
            //else
            //{
                LoginUser user = UserMgr.ReadUserInfo();
                CustomerFacade.DeleteMyFavoriteAll(user.UserSysNo);
            //}
            return Json(new
            {
                Result = isSuccess,
                Message = message
            });
        }

        #endregion

        #region 我的店铺收藏

        /// <summary>
        /// 我的店铺收藏
        /// </summary>
        /// <returns></returns>
        public ActionResult MyFavoriteSeller()
        {
            return View();
        }

        /// <summary>
        /// 添加店铺收藏
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxAddFavoriteSeller()
        {
            int productSysNo = 0;
            int.TryParse(Request.Params["sellerSysNo"], out productSysNo);
            LoginUser user = UserMgr.ReadUserInfo();
            if (productSysNo <= 0 || user == null)
            {
                return (JsonResult)BuildAjaxErrorObject("服务端异常！");
            }
            CustomerFacade.AddMyFavoriteSeller(user.UserSysNo, productSysNo);
            return Json("操作已成功！");
        }

        [HttpPost]
        public ActionResult AjaxDeleteSelectedSellers()
        {
            bool isSuccess = true;
            string message = "操作已成功，稍候生效！";
            string data = Request["SelectList"].ToString();
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    isSuccess = false;
                    message = "请选择要删除的收藏!";
                }
                else
                {
                    string[] strList = data.Split(',');
                    for (int i = 0; i < strList.Length; i++)
                    {
                        CustomerFacade.DeleteMyFavoriteSeller(Convert.ToInt32(strList[i]));
                    }
                }
            }
            catch
            {
                isSuccess = false;
                message = "系统异常，请稍候重试!";
            }
            return Json(new
            {
                Result = isSuccess,
                Message = message
            });
        }

        /// <summary>
        /// 清空我的店铺收藏
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxDeleteAllFavoriteSellers()
        {
            bool isSuccess = true;
            string message = "操作已成功，稍候生效！";
            //string data = Request["SelectList"].ToString();
            //if (string.IsNullOrEmpty(data))
            //{
            //    isSuccess = false;
            //    message = "请选择要删除的收藏!";
            //}
            //else
            //{
                LoginUser user = UserMgr.ReadUserInfo();
                CustomerFacade.DeleteMyFavoriteSellerAll(user.UserSysNo);
            //}
            return Json(new
            {
                Result = isSuccess,
                Message = message
            });
        }

        #endregion

        /// <summary>
        /// 我的晒单
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowOrder()
        {
            return View();
        }
    }
}
