using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Member;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.UI;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class ProductController : BaseApiController
    {
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="id">系统编号</param>
        /// <param name="isGroupBuy">0--标识id为商品系统编号，1--标识id为团购活动系统编号</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProductDetails(int id, int isGroupBuy)
        {
            var productManager = new ProductManager();
            ProductDetailModel productDetail = productManager.GetProductDetails(id, isGroupBuy);
            var result = new AjaxResult
            {
                Success = true,
                Data = productDetail
            };

            return Json(result);
        }

        #region [商品评论:]
        /// <summary>
        /// 获取商品评论列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="scoreType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ReviewList(int productSysNo, int productGroupSysNo, int scoreType, int pageIndex, int pageSize)
        {
            List<ReviewScoreType> scoreTypeList = new List<ReviewScoreType>();
            switch (scoreType)
            {
                case 0:
                    //scoreTypeList.Add(ReviewScoreType.None);
                    break;
                case 11:
                    scoreTypeList.Add(ReviewScoreType.ScoreType11);
                    scoreTypeList.Add(ReviewScoreType.ScoreType12);
                    break;
                case 13:
                    scoreTypeList.Add(ReviewScoreType.ScoreType13);
                    scoreTypeList.Add(ReviewScoreType.ScoreType14);
                    break;

                case 15:
                    scoreTypeList.Add(ReviewScoreType.ScoreType15);
                    break;
                default:
                    scoreTypeList.Add(ReviewScoreType.ScoreType11);
                    scoreTypeList.Add(ReviewScoreType.ScoreType12);
                    break;
            }


            return Json(new AjaxResult() { Success = true, Data = ProductManager.GetProductReviewList(productSysNo, productGroupSysNo, scoreTypeList, pageIndex + 1, pageSize) });
        }


        /// <summary>
        /// 创建评论
        /// </summary>w
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [RequireAuthorize]
        public JsonResult CreateReview(AddProductReviewRequestViewModel request)
        {
            bool result = CustomerManager.AddReview(request);
            if (result)
            {
                return Json(new AjaxResult() { Success = true, Data = "发布评论成功!" });
            }
            else
            {
                return Json(new AjaxResult() { Success = false, Data = "发布评论失败!请稍后再试." });
            }
        }
        #endregion

        #region [商品咨询:]
        [HttpPost]
        [RequireAuthorize]
        public JsonResult CreateConsult(AddProductConsultInfoViewModel request)
        {
            request.CustomerSysNo = UserMgr.ReadUserInfo().UserSysNo;

            bool result = ProductManager.CreateProductConsultInfo(request);
            if (result)
            {
                return Json(new AjaxResult() { Success = true, Data = "发表咨询成功!" });
            }
            else
            {
                return Json(new AjaxResult() { Success = false, Data = "发表咨询失败!" });
            }
        }
        #endregion

        #region [添加收藏:]
        [HttpPost]
        [RequireAuthorize]
        public JsonResult AddProductFavorite(AddProductFavoriteRequsetViewModel requst)
        {
            CustomerManager.AddProductFavorite(requst.ProductSysNo);
            return Json(new AjaxResult() { Success = true, Data = "加入收藏成功" });
        }
        #endregion
    }
}
