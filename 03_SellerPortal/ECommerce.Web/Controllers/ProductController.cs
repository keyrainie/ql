using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Service.Product;
using ECommerce.Enums;

namespace ECommerce.Web.Controllers
{
    public class ProductController : SSLControllerBase
    {
        
        public ActionResult ProductCategory()
        {
            return View();
        }
        public ActionResult ProductCommon(string mode, string callbackFuncName)
        {
            ViewData["mode"] = mode;
            ViewData["callbackFuncName"] = callbackFuncName;
            return View();
        }

        [HttpPost]
        public JsonResult QueryProductCommon()
        {
            ProducCommonQueryFilter qFilter = BuildQueryFilterEntity<ProducCommonQueryFilter>();
            qFilter.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo.ToString();

            var result = ProductService.QueryCommonProduct(qFilter);

            return AjaxGridJson(result);
        }
        
        public ActionResult Query()
        {
            int currentSellerSysNo = ECommerce.Web.UserAuthHelper.GetCurrentUser().SellerSysNo;

            ViewBag.RootFrontProductCategory = ProductMaintainService.GetParentFrontProductCategory(currentSellerSysNo);

            return View();
        }

        [HttpPost]
        public ActionResult AjaxQuery()
        {
            ProductQueryFilter queryFilter = BuildQueryFilterEntity<ProductQueryFilter>();

            int currentSellerSysNoValue = UserAuthHelper.GetCurrentUser().SellerSysNo;
            if (currentSellerSysNoValue > 0)
            {
                queryFilter.VendorSysNo = currentSellerSysNoValue.ToString();
            }

            if (queryFilter.CategorySysNo == "0")
            {
                queryFilter.CategorySysNo = string.Empty;
            }

            if (queryFilter.StatusCondition == -1)
            {
                queryFilter.StatusCondition = 0;
                queryFilter.Status = string.Empty;
            }
            //假设我拿到的是类别的CategoryCode，我需要根据他的值“CategoryCode-层级数-CategorySysNo” 拿到对应的三级CategorySysNo字符列表
            if ((!string.IsNullOrWhiteSpace(queryFilter.CategoryCode)) && queryFilter.CategoryCode != "0")
            {
                string CategoryCode = queryFilter.CategoryCode.Split('-')[0].Trim();
                string CategorySysNo = queryFilter.CategoryCode.Split('-')[2];
                string IsLeaf=queryFilter.CategoryCode.Split('-')[3];
                switch (queryFilter.CategoryCode.Split('-')[1])
                {
                    case "1":
                        if (IsLeaf == "0")
                        {
                            var CateGory2List_One = ProductMaintainService.GetFrontProductCategoryByParentCode(CategoryCode);
                            foreach (var CateGory2 in CateGory2List_One)
                            {
                                if(CateGory2.IsLeaf==CommonYesOrNo.Yes)
                                {
                                    queryFilter.CategorySysNo += CateGory2.SysNo + ",";
                                }
                                var Category3List_One = ProductMaintainService.GetFrontProductCategoryByParentCode(CateGory2.CategoryCode);
                                foreach (var item in Category3List_One)
                                {
                                    queryFilter.CategorySysNo += item.SysNo + ",";
                                }
                            }
                            //去掉最后余出来的“，”
                            if (!string.IsNullOrWhiteSpace(queryFilter.CategorySysNo))
                            {
                                int RemoveIndex_One = queryFilter.CategorySysNo.Length - 1;
                                queryFilter.CategorySysNo = queryFilter.CategorySysNo.Remove(RemoveIndex_One);
                            }
                            else
                            {
                                queryFilter.CategorySysNo = CategorySysNo;
                            }
                        }
                        else
                        {
                            queryFilter.CategorySysNo = CategorySysNo;
                        }

                        break;
                    case "2":
                        if (IsLeaf == "0")
                        {
                            var Category3List_Two = ProductMaintainService.GetFrontProductCategoryByParentCode(CategoryCode);
                            foreach (var item in Category3List_Two)
                            {
                                queryFilter.CategorySysNo += item.SysNo + ",";
                            }
                            //去掉最后余出来的“，”
                            if (!string.IsNullOrWhiteSpace(queryFilter.CategorySysNo))
                            {
                                int RemoveIndex_One = queryFilter.CategorySysNo.Length - 1;
                                queryFilter.CategorySysNo = queryFilter.CategorySysNo.Remove(RemoveIndex_One);
                            }
                            else
                            {
                                queryFilter.CategorySysNo = CategorySysNo;
                            }
                        }
                        else
                        {
                            queryFilter.CategorySysNo = CategorySysNo;
                        }

                        break;
                    case "3":
                        queryFilter.CategorySysNo = CategorySysNo;
                        break;
                    default:
                        queryFilter.CategorySysNo = string.Empty;
                        break;
                }
            }
            

            QueryResult<ProductQueryInfo>
                result = ProductService.QueryProduct(queryFilter);

            return AjaxGridJson(result);
        }


        [HttpPost]
        public JsonResult AjaxBatchStatusChange()
        {
            List<int> sysNoList = new List<int>();

            string sysNoText = Request.Form["SysNo"];
            string statusText = Request.Form["Status"];

            string[] array = sysNoText.Split(',');

            foreach (var item in array)
            {
                int sysNo = 0;
                if (int.TryParse(item, out sysNo))
                {
                    sysNoList.Add(sysNo);
                }
            }

            if (sysNoList.Count > 0)
            {
                if (statusText == "Offline")
                {
                    ProductMaintainService.ProductBatchOffline(sysNoList);
                }
                else if (statusText == "Online")
                {
                    ProductMaintainService.ProductBatchOnline(sysNoList);
                }
                else if (statusText == "Void")
                {
                    ProductMaintainService.ProductBatchAbandon(sysNoList);
                }
                else if (statusText == "OnlineNotShow")
                {
                    ProductMaintainService.ProductBatchOnlineNotShow(sysNoList);
                }
            }

            return Json(1);
        }

        [HttpPost]
        public JsonResult AjaxLoadLeafCategory()
        {
            List<FrontProductCategoryInfo> categories = new List<FrontProductCategoryInfo>(); ;

            string categoryCode = Request.Form["categoryCode"];
            if (categoryCode != null && categoryCode.Trim() != "0")
            {
                categories = ProductMaintainService.GetFrontProductCategoryByParentCode(categoryCode.Trim());
            }

            return Json(categories);
        }

        /// <summary>
        /// 加载树结构
        /// </summary>
        /// <param name="sysNo">当前选中的节点SysNo</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Tree(int sysNo)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = ProductService.GetTreeInfo(sysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);

            return result;
        }

        /// <summary>
        /// 根据SysNo获取前台类别
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFrontProductCategoryBySysNo(int sysNo)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = ProductService.GetFrontProductCategoryBySysNo(sysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);

            return result;
        }
        /// <summary>
        /// 保存前台类别
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveFrontProductCategory(FrontProductCategoryInfo info)
        {
            JsonResult result = new JsonResult();
            if (info != null)
            {
                UserAuthVM user = UserAuthHelper.GetCurrentUser();
                if (user != null)
                {
                    info.SellerSysNo = user.SellerSysNo;
                    info.InUserSysNo = user.UserSysNo;
                    info.InUserName = user.UserDisplayName;
                    info.EditUserSysNo = user.UserSysNo;
                    info.EditUserName = user.UserDisplayName;
                }
            }
            result.Data = ProductService.SaveFrontProductCategory(info);
            return result;
        }
    }
}
