using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Member;
using Nesoft.ECWeb.MobileService.Models.MemberService;
using Nesoft.ECWeb.MobileService.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class MemberServiceController : BaseApiController
    {
        /// <summary>
        /// 收藏店铺List
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [RequireAuthorize]
        public JsonResult FavoriteSellerInfoList(int pageIndex, int pageSize)
        {
            return Json(new AjaxResult() { Success = true, Data = CustomerManager.GetSellerFavoriteList(pageIndex + 1, pageSize) });
        }

        /// <summary>
        /// 店铺收藏
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult AddFavoriteSeller(AddFavoriteSellerRequsetViewModel requst)
        {
            CustomerManager.AddFavoriteSeller(requst.SellerSysNo);
            return Json(new AjaxResult() { Success = true, Data = "加入收藏成功" });
        }

        /// <summary>
        /// 取消店铺收藏
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult DeleteSelectedSellers(DeleteFavoriteSellerRequsetViewModel request)
        {

            try
            {
                CustomerManager.DeleteSelectedSellers(request.SysNo);
                return Json(new AjaxResult() { Success = true, Data = "删除收藏成功!", Code = 0 });
            }
            catch
            {
                return Json(new AjaxResult() { Success = false, Data = "删除收藏失败，请稍后再试!", Code = -1 });
            }
        }

        /// <summary>
        /// 获得商铺信息
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetStoreDetailInfo(int sellerSysNo)
        {
            var storeManager = new StoreManager();
            StoreDetailModel storeDetail = storeManager.GetStoreDetailInfo(sellerSysNo);

            var result = new AjaxResult
            {
                Success = true,
                Data = storeDetail
            };

            return Json(result);
        }

        /// <summary>
        /// 获得商铺前台类别
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFrontProductCategoryByVendorSysNo(int sellerSysNo)
        {
            var storeManager = new StoreManager();
            List<FrontProductCategoryInfoModel> FrontProductCategoryList = storeManager.GetFrontProductCategoryByVendorSysNo(sellerSysNo);

            var result = new AjaxResult
            {
                Success = true,
                Data = FrontProductCategoryList
            };

            return Json(result);
        }

        /// <summary>
        /// 店铺新品商品10个
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult QueryStoreNewRecommendProduct(int sellerSysNo)
        {
            var storeManager = new StoreManager();
            List<StoreNewProductRecommendModel> FrontProductCategoryList = storeManager.QueryStoreNewRecommendProduct(sellerSysNo);

            var result = new AjaxResult
            {
                Success = true,
                Data = FrontProductCategoryList
            };

            return Json(result);
        }

        /// <summary>
        /// 一周排行10个
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult QueryWeekRankingForCategoryCode(int sellerSysNo)
        {
            var storeManager = new StoreManager();
            List<StoreWeekRankingProductModel> FrontProductCategoryList = storeManager.QueryWeekRankingForCategoryCode(sellerSysNo);

            var result = new AjaxResult
            {
                Success = true,
                Data = FrontProductCategoryList
            };

            return Json(result);
        }

        /// <summary>
        /// 根据类别查询店铺商品
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <param name="categoryCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetVendorProductByCategoryCode(int sellerSysNo, int categoryCode)
        {
            var storeManager = new StoreManager();
            SearchResultModel ProductByCategoryCode = storeManager.GetVendorProductByCategoryCode(sellerSysNo, categoryCode);

            var result = new AjaxResult
            {
                Success = true,
                Data = ProductByCategoryCode
            };

            return Json(result);
        }


        /// <summary>
        /// 店铺列表页面信息
        /// </summary>
        /// <param name="sellerSysNo">商铺编号</param>
        /// <param name="categoryCode">分类系统编号</param>
        /// <param name="sort">价格升序40、价格降序50，销量降序20</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SearchVendorProduct(int sellerSysNo)
        {
            var storeManager = new StoreManager();
            StoreProductListModel SearchVendorProductList = storeManager.SearchVendorProduct(sellerSysNo);

            var result = new AjaxResult
            {
                Success = true,
                Data = SearchVendorProductList
            };

            return Json(result);
        }
    }
}
