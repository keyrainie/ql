using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Promotion.GroupBuying;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.GroupBuying;
using Nesoft.ECWeb.Facade.Product;

namespace Nesoft.ECWeb.M.Controllers
{
    public class GroupbuyingController : Controller
    {
        //
        // GET: /Groupbuying/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Ajax_QueryGroupbuying()
        {
            int category = 0;
            int pageIndex = 0;
            int.TryParse(Request["category"], out category);
            int.TryParse(Request["page"], out pageIndex);

            GroupBuyingQueryInfo queryInfo = new GroupBuyingQueryInfo()
            {
                PageInfo = new Entity.PageInfo()
                {
                    PageIndex = pageIndex,
                    PageSize = int.Parse(Request["size"])
                },
                SortType = 0,
                GroupBuyingTypeSysNo=0
            };
            if (category > 0)
                queryInfo.CategorySysNo = category;
            else
                queryInfo.CategorySysNo = null;

            var data = GroupBuyingFacade.QueryGroupBuyingInfo(queryInfo);
            data.ResultList.ForEach(p =>
            {
                p.DefaultImage = ProductFacade.BuildProductImage(ImageSize.P200, p.DefaultImage);
            });
            var result = new AjaxResult
            {
                Success = true,
                Data = data
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
