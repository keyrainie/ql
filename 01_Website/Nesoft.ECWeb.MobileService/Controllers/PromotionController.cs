using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Models.Promotion;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.Entity;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class PromotionController : BaseApiController
    {
        /// <summary>
        /// 限时抢购查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCountDownList(int pageIndex, int pageSize)
        {
            CountdownManager manager = new CountdownManager();
            QueryResult<CountDownItemModel> list = manager.GetCountDownList(pageIndex, pageSize);
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }

        /// <summary>
        /// 团购查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGroupBuyList(GroupBuyQueryModel criteria)
        {
            GroupBuyManager manager = new GroupBuyManager();
            GroupBuyQueryResult list = manager.GetGroupBuyList(criteria);
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }

        /// <summary>
        /// 首页品牌推荐商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHomeBrandItemList()
        {
            RecommendManager manager = new RecommendManager();
            List<RecommendItemModel> list = manager.GetHomeBrandItemList();
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }

        /// <summary>
        /// 首页超低折扣商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHomeDiscountItemList()
        {
            RecommendManager manager = new RecommendManager();
            List<RecommendItemModel> list = manager.GetHomeDiscountItemList();
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }

        /// <summary>
        /// 获取促销模板信息
        /// </summary>
        /// <param name="id">促销模板编号</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSaleAdvInfo(int id)
        {
            SaleAdvManager manager = new SaleAdvManager();
            List<SaleAdvModel> list = manager.GetSaleAdvInfo(id);
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }
    }
}
