using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Product;
using ECommerce.Entity.SolrSearch;
using ECommerce.Enums;
using ECommerce.Facade.Product;
using ECommerce.Utility.DataAccess.SearchEngine;
using ECommerce.Entity;
using ECommerce.Facade.Member;
using ECommerce.WebFramework;
using ECommerce.Entity.Common;
using ECommerce.Facade;
using ECommerce.Entity.Promotion;
using ECommerce.Facade.Shopping;
using ECommerce.Entity.Member;
using ECommerce.Facade.Common;

namespace ECommerce.UI.Controllers
{
    public class ProductController : WWWControllerBase
    {
        // 
        // GET: /Web/Product/

        public ActionResult ProductDetail(int? productSysNo)
        {
            if (!productSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "该商品不存在！";
                return View("Error");
            }
            return View(productSysNo);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ActionResult ProductReview(int? productSysNo)
        {
            if (!productSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "商品信息错误!";
            }
            ViewBag.ProductSysNo = productSysNo;
            return View();
        }
        /// <summary>
        /// 获取评论列表NEW
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ActionResult ProductReviewTwo(int? productSysNo)
        {
            string sosysno = Request.QueryString["sosysno"];
            if (!productSysNo.HasValue || string.IsNullOrEmpty(sosysno))
            {
                TempData["ErrorMessage"] = "商品信息错误!";
            }
            ViewBag.ProductSysNo = productSysNo;
            ViewBag.SoSysno = sosysno;
            return View();
        }
        

        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="reviewSysNo"></param>
        /// <returns></returns>
        public ActionResult ProductReviewDetail(int? reviewSysNo)
        {
            if (!reviewSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "查询信息错误！";
            }
            ViewBag.ReviewSysNo = reviewSysNo;
            return View();
        }

        /// <summary>
        /// 获取咨询列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ActionResult ProductConsult(int? productSysNo)
        {
            if (!productSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "商品信息错误!";
            }
            ViewBag.ProductSysNo = productSysNo;
            return View();
        }

        /// <summary>
        /// 获取咨询详情
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        public ActionResult ProductConsultDetail(int? consultSysNo)
        {
            if (!consultSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "商品信息错误!";
            }
            ViewBag.ConsultSysNo = consultSysNo;
            return View();
        }

        /// <summary>
        /// 获取商品评论
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AjaxGetProudctReview()
        {
            int productGroupSysNo = int.Parse(Request["ProductGroupSysNo"].ToString());
            //int serchType = int.Parse(Request["SearchType"].ToString());
            int productSysNo = int.Parse(Request["ProductSysNo"].ToString());
            Product_ReviewQueryInfo queryInfo = new Product_ReviewQueryInfo();
            queryInfo.ProductGroupSysNo = productGroupSysNo;
            //queryInfo.SearchType.Clear();
            //queryInfo.SearchType.Add((ReviewScoreType)serchType);
            string SearchType = Request["SearchType"];
            int tempSearchType;
            if (!string.IsNullOrWhiteSpace(SearchType))
            {
                var searchTypeArray = SearchType.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                queryInfo.SearchType.Clear();
                foreach (string searchType in searchTypeArray)
                {
                    if (int.TryParse(searchType, out tempSearchType))
                    {
                        queryInfo.SearchType.Add((ReviewScoreType)tempSearchType);
                    }
                }
            }
            queryInfo.NeedReplyCount = 5;
            queryInfo.ProductSysNo = productSysNo;
            queryInfo.PagingInfo = new PageInfo() { PageIndex = 0, PageSize = 5 };
            ViewBag.ProductSysNo = productSysNo;
            return PartialView("~/Views/UserControl/Product/UCProductReviewItem.cshtml", ReviewFacade.GetProductReviewListByProductGroupSysNoForProduct(queryInfo));

        }

        /// <summary>
        /// 评论列表页获取评论列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetProudctReviewList()
        {
            int productGroupSysNo = int.Parse(Request["ProductGroupSysNo"].ToString());
            int serchType = int.Parse(Request["SearchType"].ToString());
            int productSysNo = int.Parse(Request["ProductSysNo"].ToString());
            int pageIndex = 1;
            int.TryParse(Request.QueryString["page"], out pageIndex);
            Product_ReviewQueryInfo queryInfo = new Product_ReviewQueryInfo();
            queryInfo.ProductGroupSysNo = productGroupSysNo;
            queryInfo.SearchType.Clear();
            queryInfo.SearchType.Add((ReviewScoreType)serchType);
            queryInfo.NeedReplyCount = 5;
            queryInfo.ProductSysNo = productSysNo;
            queryInfo.PagingInfo = new PageInfo() { PageIndex = pageIndex == 0 ? 1 : pageIndex, PageSize = 10 };
            //ViewBag.ProductSysNo = productSysNo;
            Product_ReviewList list = ReviewFacade.GetProductReviewListByProductGroupSysNoForProduct(queryInfo);
            return PartialView("~/Views/UserControl/Product/UCProductReview.cshtml", list);

        }

        public ActionResult UCProductReviewList(string PageIndex, string PageSize, string SearchType, string ProductSysNo, string ProductGroupSysNo)
        {
            Product_ReviewQueryInfo queryInfo = new Product_ReviewQueryInfo();
            queryInfo.NeedReplyCount = 5;
            queryInfo.PagingInfo = new PageInfo();
            int tempIndex = 0;
            if (int.TryParse(PageIndex, out tempIndex))
            {
                queryInfo.PagingInfo.PageIndex = tempIndex;
            }
            int tempSize = 0;
            if (int.TryParse(PageSize, out tempSize))
            {
                queryInfo.PagingInfo.PageSize = tempSize;
            }

            int tempSearchType = 0;
            if (!string.IsNullOrWhiteSpace(SearchType))
            {
                var searchTypeArray = SearchType.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                queryInfo.SearchType.Clear();
                foreach (string searchType in searchTypeArray)
                {
                    if (int.TryParse(searchType, out tempSearchType))
                    {
                        queryInfo.SearchType.Add((ReviewScoreType)tempSearchType);
                    }
                }
            }

            int tempProductSysNo = 0;
            if (int.TryParse(ProductSysNo, out tempProductSysNo))
            {
                queryInfo.ProductSysNo = tempProductSysNo;
            }

            int tempProductGroupSysNo = 0;
            if (int.TryParse(ProductGroupSysNo, out tempProductGroupSysNo))
            {
                queryInfo.ProductGroupSysNo = tempProductGroupSysNo;
            }

            Product_ReviewList list = ReviewFacade.GetProductReviewListByProductGroupSysNoForProduct(queryInfo);
            ViewQueryResult<Product_ReviewDetail> result = new ViewQueryResult<Product_ReviewDetail>();
            result.ResultList = list.ProductReviewDetailList.CurrentPageData;

            result.PagingInfo.PageIndex = list.ProductReviewDetailList.PageNumber - 1;
            result.PagingInfo.PageSize = list.ProductReviewDetailList.PageSize;
            result.PagingInfo.TotalCount = list.ProductReviewDetailList.TotalRecords;
            return (ActionResult)View(result);

        }

        /// <summary>
        /// 是否登录，大于0则登录
        /// </summary>
        /// <returns></returns>
        public JsonResult CheckLogin()
        {
            if (this.CurrUser != null && this.CurrUser.UserSysNo > 0)
            {
                return new JsonResult() { Data = this.CurrUser.UserSysNo };
            }
            else
            {
                return new JsonResult() { Data = 0 };
            }
        }

        /// <summary>
        /// 商品大图
        /// </summary>
        /// <returns></returns>
        public ActionResult BigPic(int productSysNo, int productCommonSysNo)
        {
            ViewBag.ProductSysNo = productSysNo;
            ViewBag.ProductCommonSysNo = productCommonSysNo;
            int index = 0;
            int.TryParse(Request.QueryString["Index"], out index);
            ViewBag.Index = index;
            return View();
        }

        /// <summary>
        /// 商品搜索
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SearchResult()
        {
            ViewBag.IsSearchResultPage = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["enid"]))
            {
                ViewBag.IsSearchResultPage = 0;
                ViewBag.SubCategoryID = ProductSearchFacade.GetSubCategoryIDbyStrN(Request.QueryString["enid"]).ToString();
            }
            return View();
        }

        /// <summary>
        /// 品牌商品搜索
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult BrandProductSearch(string brandID)
        {
            int brandSysno;
            string brandName = string.Empty;
            int.TryParse(brandID, out brandSysno);
            var brand = ProductFacade.GetBrandBySysNo(brandSysno);
            if (brand != null)
            {
                brandName = brand.BrandName_Ch;
            }
            ViewBag.BrandID = brandID;
            ViewBag.BrandName = brandName;
            ViewBag.PageType = ((int)ECommerce.Enums.PageType.BrandProductSearch).ToString();
            ViewBag.IsSearchResultPage = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["enid"]))
            {
                ViewBag.SubCategoryID = ProductSearchFacade.GetSubCategoryIDbyStrN(Request.QueryString["enid"]).ToString();
            }
            return View("SearchResult");
        }

        [ValidateInput(false)]
        public ActionResult SearchNoResult()
        {
            return View();
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateReview()
        {
            Product_ReviewDetail info = new Product_ReviewDetail();
            info.ProductSysNo = int.Parse(Request["ProductSysNo"].ToString());
            info.Title = Request["Title"].ToString();
            info.Prons = Request["Prons"].ToString();
            //info.Cons = Request["Cons"].ToString();
            //info.Service = Request["Service"].ToString();
            info.Image = Request["Image"].ToString();
            int tempso = int.Parse(Request["SoSysNo"].ToString());
            if (tempso != 0 && tempso > 0)
            {
                info.SOSysno = tempso;
            }
            decimal s1 = Convert.ToDecimal(string.IsNullOrEmpty(Request["Score1"].ToString()) == true ? "5" : Request["Score1"].ToString());
            decimal s2 = Convert.ToDecimal(string.IsNullOrEmpty(Request["Score2"].ToString()) == true ? "5" : Request["Score2"].ToString());
            decimal s3 = Convert.ToDecimal(string.IsNullOrEmpty(Request["Score3"].ToString()) == true ? "5" : Request["Score3"].ToString());
            decimal s4 = Convert.ToDecimal(string.IsNullOrEmpty(Request["Score4"].ToString()) == true ? "5" : Request["Score4"].ToString());
            info.Score1 = Convert.ToInt32(s1);
            info.Score2 = Convert.ToInt32(s2);
            info.Score3 = Convert.ToInt32(s3);
            info.Score4 = Convert.ToInt32(s4);

            info.CustomerInfo.SysNo = this.CurrUser.UserSysNo;
            info.ReviewType = ReviewType.Common;
            bool IsSuccess = ReviewFacade.CreateProductReview(info);
            if (IsSuccess)
            {
                return new JsonResult() { Data = 1 };
            }
            else
            {
                return new JsonResult() { Data = 0 };
            }
        }
        /// <summary>
        /// 发表回复
        /// </summary>
        /// <param name="replyInfo"></param>
        /// <returns></returns>
        public JsonResult CreateProductReviewReply()
        {
            Product_ReplyDetail replyInfo = new Product_ReplyDetail();
            replyInfo.ReviewSysNo = int.Parse(Request["ReviewSysNo"].ToString());
            replyInfo.Content = Request["Content"].ToString();
            replyInfo.Customer.SysNo = this.CurrUser.UserSysNo;
            int ProductSysNo = int.Parse(Request["ProductSysNo"].ToString());
            replyInfo.Customer.CustomerID = this.CurrUser.UserID;
            int SoSysNo = int.Parse(Request["SOSysNo"].ToString());
            replyInfo.Customer.Email = Request["Mail"].ToString();
            bool IsSuccess = ReviewFacade.CreateProductReviewReply(replyInfo, SoSysNo);
            if (IsSuccess)
            {
                return Json("1", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("发表评论失败！", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 发表咨询
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateConsult()
        {
            ConsultationInfo info = new ConsultationInfo();
            info.ProductSysNo = int.Parse(Request["ProductSysNo"].ToString());
            info.CustomerSysNo = this.CurrUser.UserSysNo;
            info.Content = Request["Content"].ToString();
            info.Type = Request["Type"].ToString();
            bool IsSuccess = ConsultationFacade.CreateProductConsult(info);
            if (IsSuccess)
            {
                return new JsonResult() { Data = 1 };
            }
            else
            {
                return new JsonResult() { Data = 0 };

            }
        }

        /// <summary>
        /// 发表咨询回复
        /// </summary>
        /// <returns></returns>
        public JsonResult CreateProductConsultReply()
        {
            ProductConsultReplyInfo info = new ProductConsultReplyInfo();
            info.ConsultSysNo = int.Parse(Request["ConsultSysNo"].ToString());
            info.ReplyType = FeedbackReplyType.Web;
            info.CustomerSysNo = this.CurrUser.UserSysNo;
            info.CustomerInfo.CustomerID = this.CurrUser.UserID;
            info.Content = Request["Content"].ToString();
            info.CustomerInfo.Email = Request["Email"].ToString();
            int ProductSysNo = int.Parse(Request["ProductSysNo"].ToString());
            string ProductName = Request["ProductName"].ToString();
            bool IsSuccess = ConsultationFacade.CreateProductConsultReply(info, ProductSysNo, ProductName);
            if (IsSuccess)
            {
                //ConsultationFacade.SendMailConsultReply();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("发表评论失败！", JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <returns></returns>
        [Auth]
        public JsonResult AjaxAddProductToWishList()
        {
            int productSysNo = 0;
            int.TryParse(Request.Params["productSysNo"], out productSysNo);
            LoginUser user = UserMgr.ReadUserInfo();
            //if (user == null)
            //{
            //    return new JsonResult() { Data = BuildAjaxErrorObject("请先登录！") };
            //}
            //商品已经被收藏
            if (ProductFacade.IsProductWished(productSysNo, user.UserSysNo))
            {
                return new JsonResult() { Data = 0 };
            }
            CustomerFacade.AddProductToWishList(user.UserSysNo, productSysNo);
            return new JsonResult() { Data = 1 };
        }

        /// <summary>
        /// 添加店铺收藏
        /// </summary>
        /// <returns></returns>
        [Auth]
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

        /// <summary>
        /// 商品比较
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductCompare()
        {
            return View();
        }


        /// <summary>
        /// 评论有用没有
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxReviewVote()
        {
            int reviewSysNo = 0;
            int isUsefull = 0;
            int.TryParse(Request.Params["ReviewSysNo"], out reviewSysNo);
            int.TryParse(Request.Params["IsUsefull"], out isUsefull);
            LoginUser user = UserMgr.ReadUserInfo();
            if (user == null || user.UserSysNo <= 0)
            {
                return new JsonResult() { Data = BuildAjaxErrorObject("请先登录！") };
            }

            //成功
            if (ReviewFacade.UpdateReviewVote(user.UserSysNo, reviewSysNo, isUsefull))
            {
                return new JsonResult() { Data = 1 };
            }
            //失败
            return new JsonResult() { Data = 0 };

        }

        /// <summary>
        /// 商品到货通知
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult ArriveNotice(int productSysNo)
        {
            return View(productSysNo);
        }

        /// <summary>
        /// 商品降价通知
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult ProductDiscountNotify(int productSysNo)
        {
            return View(productSysNo);
        }

        /// <summary>
        /// 商品关税表
        /// </summary>
        /// <returns></returns>
        public ActionResult TariffRate()
        {
            return View();
        }

        /// <summary>
        /// 促销模板
        /// </summary>
        /// <param name="promotionSysNo"></param>
        /// <returns></returns>
        public ActionResult Promotion(int promotionSysNo)
        {
            return View(promotionSysNo);
        }

        /// <summary>
        /// 获取指定省的市列表
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxGetCityList()
        {
            int provinceSysNo = 0;
            int.TryParse(Request.Params["sysno"], out provinceSysNo);
            //return new JsonResult() { Data = CommonFacade.GetAllCity(provinceSysNo) };

            int productSysNo = 0;
            int.TryParse(Request.Params["productSysNo"], out productSysNo);
            List<Area> coll = ProductFacade.GetProductCitys(productSysNo, provinceSysNo);
            return new JsonResult() { Data = coll };
        }

        /// <summary>
        /// 获取商品运费
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxGetShippingPrices()
        {
            int areaSysNo = 0;
            int.TryParse(Request.Params["areaSysNo"], out areaSysNo);
            //return new JsonResult() { Data = CommonFacade.GetAllCity(provinceSysNo) };

            int productSysNo = 0;
            int.TryParse(Request.Params["productSysNo"], out productSysNo);
            List<ProductShippingPrice> shippingPrices = ProductFacade.GetProductShippingPrice(productSysNo, areaSysNo);
            List<ProductShippingPrice> result = new List<ProductShippingPrice>();
            foreach (var p in shippingPrices)
            {
                ProductShippingPrice temp = result.FirstOrDefault(c => c.ShipTypeSysNo == p.ShipTypeSysNo);
                if (temp == null)
                {
                    result.Add(p);
                }
                else
                {
                    if (temp.BaseWeight > p.BaseWeight)
                    {
                        temp.UnitPrice = p.UnitPrice;
                    }
                    if (temp.BaseWeight == p.BaseWeight && temp.UnitPrice > p.UnitPrice)
                    {
                        temp.UnitPrice = p.UnitPrice;
                    }
                }
            }
            return new JsonResult() { Data = result };
        }

        public ActionResult GetCouponPopContent(int MerchantSysNo)
        {
            CouponContentInfo Model = new CouponContentInfo();
            //优惠卷
            LoginUser user = UserMgr.ReadUserInfo();
            List<CustomerCouponInfo> CustomerCouponList = new List<CustomerCouponInfo>();
            if (user != null)
            {
                CustomerCouponList = ShoppingFacade.GetCustomerPlatformCouponCode(user.UserSysNo, MerchantSysNo);
            }
            //获取当前有效的优惠券活动
            List<CouponInfo> CouponList = new List<CouponInfo>();
            if (user != null)
            {
                CouponList = ShoppingFacade.GetCouponList(user.UserSysNo, MerchantSysNo);
            }
            if (user != null)
            {
                Model.UserSysNo = user.UserSysNo;
                Model.MerchantSysNo = MerchantSysNo;
                Model.customerCouponCodeList = CustomerCouponList;
                Model.couponList = CouponList;
            }
            PartialViewResult view = PartialView("~/Views/Shared/_CouponPop.cshtml", Model);
            return view;
        }

        /// <summary>
        /// 获取商品阶梯价格
        /// </summary>
        /// <param name="OldPrice"></param>
        /// <param name="ProductSysNo"></param>
        /// <param name="ProductCount"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxGetProductStepPrice(decimal OldPrice, int ProductSysNo, int ProductCount)
        {
            decimal price = OldPrice.GetProductStepPrice(ProductSysNo, ProductCount);
            return new JsonResult() { Data = price.ToString("f2") };
        }
    }
}
