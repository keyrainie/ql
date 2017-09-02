using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using ECommerce.Service.Product;
using ECommerce.Entity.Product;
using ECommerce.WebFramework;
using ECommerce.Utility;
using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Enums;
using System.Text;

namespace ECommerce.Web.Controllers
{
    /// <summary>
    /// 商品维护
    /// </summary>
    public class ProductMaintainController : SSLControllerBase
    {
        #region 页面View

        /// <summary>
        /// 选择分类
        /// </summary>
        /// <returns></returns>

        public ActionResult SelectCategory()
        {
            return View();
        }

        /// <summary>
        /// 基础信息
        /// </summary>
        /// <returns></returns>

        public ActionResult BasicInfo()
        {
            return View();
        }

        /// <summary>
        /// 同组商品
        /// </summary>
        /// <returns></returns>

        public ActionResult GroupProduct()
        {
            return View();
        }

        /// <summary>
        /// 图片信息
        /// </summary>
        /// <returns></returns>

        public ActionResult ImageInfo()
        {
            return View();
        }

        /// <summary>
        /// 价格信息
        /// </summary>
        /// <returns></returns>

        public ActionResult PriceInfo()
        {
            return View();
        }

        /// <summary>
        /// 备案信息
        /// </summary>
        /// <returns></returns>

        public ActionResult EntryInfo()
        {
            return View();
        }

        /// <summary>
        /// 商品销售区域
        /// </summary>
        /// <returns></returns>

        public ActionResult SalesArea()
        {
            return View();
        }
        /// <summary>
        /// 商品批号管理
        /// </summary>
        /// <returns></returns>

        public ActionResult BatchManagement()
        {
            return View();
        }


        #endregion

        #region Ajax请求

        #region 选择分类

        /// <summary>
        /// 获取C2
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxCategory2List()
        {
            string sysNo = Request.Params["sysno"].Trim();
            var user = UserAuthHelper.GetCurrentUser();
            return new JsonResult() { Data = ProductMaintainService.GetCategory2List(user.SellerSysNo, int.Parse(sysNo)) };
        }

        /// <summary>
        /// 获取所有C2
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxAllCategory2List()
        {
            string sysNo = Request.Params["sysno"].Trim();
            return new JsonResult() { Data = ProductMaintainService.GetAllCategory2List(int.Parse(sysNo)) };
        }

        /// <summary>
        /// 获取C3
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxCategory3List()
        {
            string sysNo = Request.Params["sysno"].Trim();
            var user = UserAuthHelper.GetCurrentUser();
            return new JsonResult() { Data = ProductMaintainService.GetCategory3List(user.SellerSysNo, int.Parse(sysNo)) };
        }

        /// <summary>
        /// 获取所有C3
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxAllCategory3List()
        {
            string sysNo = Request.Params["sysno"].Trim();
            return new JsonResult() { Data = ProductMaintainService.GetAllCategory3List(int.Parse(sysNo)) };
        }

        /// <summary>
        /// 获取末级分类
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxLoadLeafCategory()
        {
            string categoryCode = Request.Params["code"].Trim();
            return new JsonResult() { Data = ProductMaintainService.GetFrontProductCategoryByParentCode(categoryCode) };
        }

        #endregion

        #region 基础信息

        /// <summary>
        /// 保存商品基础信息
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult AjaxSaveProductBasicInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductMaintainBasicInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductMaintainBasicInfo>(dataString);
            SetBizEntityUserInfo(data, data.ProductGroupSysNo.HasValue && data.ProductGroupSysNo > 0 ? false : true);
            foreach (var item in data.SelectNormalProperties)
            {
                SetBizEntityUserInfo(item, true);
            }
            if (data.ProductGroupSysNo.HasValue && data.ProductGroupSysNo > 0)
            {
                ProductMaintainService.UpdateProductBasicInfoByProductGroupSysNo(data);
                return new JsonResult() { Data = data.ProductGroupSysNo.Value };
            }
            else
            {
                return new JsonResult() { Data = ProductMaintainService.CreateProductBasicInfo(data).ProductGroupSysNo.Value };
            }
        }

        #endregion

        #region 图片信息

        /// <summary>
        /// 保存商品图片信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveImageInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<ProductImageInfo> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<ProductImageInfo>>(dataString);
            foreach (var item in data)
            {
                SetBizEntityUserInfo(item, true);
            }

            return new JsonResult() { Data = ProductMaintainService.SaveProductImageInfo(data) };
        }

        /// <summary>
        /// 更新商品图片优先级
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxUpdateProductImagePriority()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductImageInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductImageInfo>(dataString);
            SetBizEntityUserInfo(data, true);

            return new JsonResult() { Data = ProductMaintainService.UpdateProductImagePriority(data) };
        }

        /// <summary>
        /// 设置商品默认图片
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSetProductDefaultImage()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductImageInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductImageInfo>(dataString);
            SetBizEntityUserInfo(data, true);

            return new JsonResult() { Data = ProductMaintainService.SetProductDefaultImage(data) };
        }

        /// <summary>
        /// 删除指定商品图片
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxDeleteProductImageBySysNo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.DeleteProductImageBySysNo(data) };
        }

        #endregion

        #region 同款商品

        /// <summary>
        /// 创建同款商品
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxCreateGroupProduct()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<ProductMaintainGroupProductInfo> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<ProductMaintainGroupProductInfo>>(dataString);
            foreach (var item in data)
            {
                SetBizEntityUserInfo(item, true);
            }

            return new JsonResult() { Data = ProductMaintainService.CreateGroupProduct(data) };
        }

        /// <summary>
        /// 获取单个商品维护信息
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AjaxLoadEditProduct()
        {
            int productSysNo = 0;
            int.TryParse(Request["ProductSysNo"], out productSysNo);
            if (productSysNo == 0)
                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("请选择商品！"));
            var user = UserAuthHelper.GetCurrentUser();
            return PartialView("_ProductEdit", ProductMaintainService.GetSingleProductMaintainInfo(productSysNo, user.SellerSysNo));
        }

        /// <summary>
        /// 保存单个商品信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveSingleProductInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductMaintainInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductMaintainInfo>(dataString);
            SetBizEntityUserInfo(data, true);

            return new JsonResult() { Data = ProductMaintainService.UpdateSingleProductMaintainInfo(data) };
        }

        #endregion

        #region 价格信息

        /// <summary>
        /// 获取商品价格信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxGetProductPriceInfo()
        {
            string sysNo = Request.Params["sysno"].Trim();
            return new JsonResult() { Data = ProductMaintainService.GetProductPriceInfoByProductSysNo(int.Parse(sysNo)) };
        }

        /// <summary>
        /// 保存价格信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSavePriceInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<ProductPriceInfo> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<ProductPriceInfo>>(dataString);
            foreach (var item in data)
            {
                SetBizEntityUserInfo(item, true);
            }

            return new JsonResult() { Data = ProductMaintainService.MaintainProductPriceInfo(data) };
        }

        #endregion

        #region 备案信息

        /// <summary>
        /// 获取商品备案信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxGetProductEntryInfo()
        {
            string sysNo = Request.Params["sysno"].Trim();
            return new JsonResult() { Data = ProductMaintainService.GetProductEntryInfoByProductSysNo(int.Parse(sysNo)) };
        }

        /// <summary>
        /// 保存备案信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveEntryInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductEntryInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductEntryInfo>(dataString);

            return new JsonResult() { Data = ProductMaintainService.MaintainProductEntryInfo(data) };
        }

        /// <summary>
        /// 提交备案申请
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSubmitProductEntryAudit()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            ProductEntryInfo data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<ProductEntryInfo>(dataString);

            ProductMaintainService.MaintainProductEntryInfo(data);
            List<int> productSysNoList = new List<int>();
            productSysNoList.Add(data.ProductSysNo);

            return new JsonResult() { Data = ProductMaintainService.BatchSubmitProductEntryAudit(productSysNoList) };
        }

        #endregion

        #region 备案信息管理

        /// <summary>
        /// 查询备案信息
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryProductEntryInfo()
        {
            ProductEntryInfoQueryFilter filter = BuildQueryFilterEntity<ProductEntryInfoQueryFilter>();
            var user = UserAuthHelper.GetCurrentUser();
            filter.SellerSysNo = user != null ? user.SellerSysNo : 0;
            var result = ProductMaintainService.QueryProductEntryInfo(filter);
            return AjaxGridJson(result);
        }

        /// <summary>
        /// 批量提交备案申请
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxBatchSubmitProductEntryAudit()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.BatchSubmitProductEntryAudit(data) };
        }

        #endregion

        #region 商品上下架管理

        /// <summary>
        /// 批量上架商品
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxProductBatchOnline()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.ProductBatchOnline(data) };
        }

        /// <summary>
        /// 批量上架不展示商品
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxProductBatchOnlineNotShow()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.ProductBatchOnlineNotShow(data) };
        }

        /// <summary>
        /// 批量下架商品
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxProductBatchOffline()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.ProductBatchOffline(data) };
        }

        /// <summary>
        /// 批量作废商品
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxProductBatchAbandon()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            List<int> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<int>>(dataString);

            return new JsonResult() { Data = ProductMaintainService.ProductBatchAbandon(data) };
        }

        #endregion

        #region 商品销售区域管理
        /// <summary>
        /// 获取商品销售区域
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AjaxGetProductSalesAreaInfo()
        {
            var queryFilter = SerializationUtility.JsonDeserialize2<ProductQueryFilter>(this.Request["queryfilter[]"]);
            var data = ProductMaintainService.GetProductSalesAreaInfoBySysNo(queryFilter);
            return AjaxGridJson(data);
        }
        /// <summary>
        /// 新增商品销售区域
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveProductSalesAreaInfo()
        {
            string dataString = Request.Form["Data"];
            dataString = HttpUtility.UrlDecode(dataString);
            if (string.IsNullOrEmpty(dataString))
            {
                return Json(new { Error = true, Message = LanguageHelper.GetText("Excel没有数据") });
            }
            else
            {
                var user = UserAuthHelper.GetCurrentUser();
                string ProductSysNo = Request.QueryString["ProductSysNo"];
                ProductQueryInfo productInfo = ProductMaintainService.GetProductTitleByProductSysNo(Int32.Parse(ProductSysNo));

                List<AreaInfo> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<AreaInfo>>(dataString);
                List<ProductSalesAreaInfo> SalesAreaList = new List<ProductSalesAreaInfo>();
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.CityName) && item.CitySysNo.HasValue && !string.IsNullOrEmpty(item.ProvinceName) && item.ProvinceSysNo.HasValue)
                    {
                        ProductSalesAreaInfo productSalesAreaInfo = new ProductSalesAreaInfo();
                        UserInfo userInfo = new UserInfo();
                        //仓库信息
                        StockInfo stock = new StockInfo();
                        stock.Status = StockStatus.Actived;
                        stock.StockName = item.StockName;
                        stock.SysNo = item.SysNo;
                        stock.StockType = TradeType.DirectMail;
                        stock.MerchantSysNo = 0;
                        //销售区域
                        productSalesAreaInfo.Stock = stock;
                        productSalesAreaInfo.Province = item;
                        //商家信息
                        productSalesAreaInfo.CompanyCode = user.CompanyCode;
                        productSalesAreaInfo.LanguageCode = user.LanguageCode;
                        userInfo.UserName = user.UserDisplayName;
                        productSalesAreaInfo.OperationUser = userInfo;
                        SalesAreaList.Add(productSalesAreaInfo);
                    }
                }

                ProductMaintainService.InsertProductSalesArea(productInfo, SalesAreaList);
                return new JsonResult() { Data = true };
            }
        }
        #endregion

        #region 商品批号信息
        /// <summary>
        /// 获取商品批号信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxGetBatchManagementInfo()
        {
            string sysNo = Request.Params["sysno"].Trim();
            var data = ProductMaintainService.GetProductBatchManagementInfo(int.Parse(sysNo));
            return new JsonResult() { Data = data };
        }
        /// <summary>
        /// 保存商品批号信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AjaxSaveBatchManagementInfo()
        {
            string dataString = Request.Form["Data"];
            var user = UserAuthHelper.GetCurrentUser();
            dataString = HttpUtility.UrlDecode(dataString);
            List<ProductBatchManagementInfo> data = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<ProductBatchManagementInfo>>(dataString);
            data[0].EidtUser = user.UserDisplayName;
            var Result = ProductMaintainService.UpdateIsBatch(data[0]);
            return new JsonResult() { Data = Result };
        }
        #endregion

        #endregion
    }

}
