using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Search;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class SearchController : BaseApiController
    {
        /// <summary>
        /// 获取搜索页面的热门搜索关键字列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotKeywords()
        {
            SearchManager manager = new SearchManager();
            List<HotSearchKeywordModel> list = manager.GetHotKeywords();
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }

        /// <summary>
        /// 获取搜索结果(可通过分类ID,关键字，Barcode搜索)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Search(SearchCriteriaModel criteria)
        {
            SearchManager manager = new SearchManager();
            SearchResultModel list = manager.Search(criteria);
            var result = new AjaxResult
            {
                Success = true,
                Data = list
            };
            return Json(result);
        }
    }
}
