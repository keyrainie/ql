using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.MobileService.Models.Cart;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class CartController : BaseApiController
    {

        #region 购物车

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCart(string proSysNos = null, string packSysNos = null)
        {
            if (proSysNos == "null") { proSysNos = null; }
            if (packSysNos == "null") { packSysNos = null; }
            CartResultModel model = ShoppingCartManager.GetCart(proSysNos, packSysNos);
            
            return BulidJsonResult(model);
        }
        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public JsonResult AddCart(UpdateCartReqModel req)
        {
            AjaxResult ajaxResult = ShoppingCartManager.AddToShoppingCart(req);
            return BulidJsonResult(ajaxResult);
        }

        /// <summary>
        /// 修改购物车数量
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCart(UpdateCartReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.UpdateCart(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 删除购物
        /// 删除购物单个商品ProductSysNo>0 PackageSysNo=0
        /// 删除购物车套餐ProductSysNo=0 PackageSysNo>0
        /// 删除购物车套餐商品某个商品 ProductSysNo>0 PackageSysNo>0
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelCart(DelCartReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelCart(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        #endregion

        #region 商品赠品

        /// <summary>
        /// 删除购物车中某商品的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 删除购物车中某商品选择的赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelSltGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelSltGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 选择购物车中某商品的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SltGift(ProductGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.SltGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        #endregion

        #region 订单赠品

        /// <summary>
        /// 删除购物车中订单上某活动的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelOrderGift(OrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelOrderGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 删除购物车中订单上某活动选择的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelOrderSltGift(OrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelOrderSltGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 选择购物车中订单上某活动的某赠品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SltOrderGift(SltOrderGiftReqModel req, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.SltOrderGift(req, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        #endregion

        #region 加够商品
        /// <summary>
        /// 选择加够商品
        /// </summary>
        /// <param name="list">商品编号列表</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SltPlusBuyProduct(List<int> list, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.SltPlusBuyProduct(list, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        /// <summary>
        /// 移除加够商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DelPlusBuyProduct(int productSysNo, string proSysNos = null, string packSysNos = null)
        {
            CartResultModel model = ShoppingCartManager.DelPlusBuyProduct(productSysNo, proSysNos, packSysNos);
            return BulidJsonResult(model);
        }

        #endregion

        #region 私有方法

        private JsonResult BulidJsonResult(CartResultModel model)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Data = model;
            if (model == null)
            {
                ajaxResult.Success = false;
                ajaxResult.Code = -1;
            }
            else
            {
                ajaxResult.Success = true;
                ajaxResult.Code = 0;
            }

            return BulidJsonResult(ajaxResult);
        }

        private JsonResult BulidJsonResult(AjaxResult ajaxResult)
        {
            JsonResult result = new JsonResult();
            result.Data = ajaxResult;

            return result;

        }

        #endregion

    }
}
