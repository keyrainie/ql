using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Category;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class CategoryController : BaseApiController
    {
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCategoryList()
        {
            var categoryManager = new CategoryManager();
            List<CategoryModel> catList = categoryManager.GetCategoryList();
            var result= new AjaxResult
            {
                Success=true,
                Data = catList
            };

            return Json(result);
        }

    }
}
