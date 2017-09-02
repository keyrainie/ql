using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Promotion.GroupBuying;
using ECommerce.Facade.GroupBuying;
using ECommerce.Facade.GroupBuying.Models;
using ECommerce.Facade.Topic;

namespace ECommerce.UI.Controllers
{
    /// <summary>
    /// 团购
    /// </summary>
    public class GroupBuyingController : WWWControllerBase
    {
        /// <summary>
        /// 团购频道页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int pageIndex = 0;
            if (int.TryParse(Request.Params["page"], out pageIndex))
                pageIndex--;
            int category = 0;
            int.TryParse(Request.Params["category"], out category);
            int sort = 0;
            int.TryParse(Request.Params["sort"], out sort);

            GroupBuyingQueryInfo queryInfo = new GroupBuyingQueryInfo()
            {
                PageInfo = new Entity.PageInfo()
                {
                    PageIndex = pageIndex,
                    PageSize = 10                    
                },
                SortType = sort
            };
            if (category > 0)
                queryInfo.CategorySysNo = category;
            else
                queryInfo.CategorySysNo = null;

            GroupBuyingQueryResult result = new GroupBuyingQueryResult();
            result.QueryInfo = queryInfo;
            result.CategoryList = GroupBuyingFacade.GetGroupBuyingCategory();
            result.Result = GroupBuyingFacade.QueryGroupBuyingInfo(queryInfo);
            result.Result.PageInfo.PageIndex++;
            return View(result);
        }

        /// <summary>
        /// 团购详情页
        /// </summary>
        /// <param name="GroupBuyingSysNo">团购系统编号</param>
        /// <returns></returns>
        public ActionResult Detail(int GroupBuyingSysNo)
        {
            var result = GroupBuyingFacade.GetGroupBuyingInfoBySysNo(GroupBuyingSysNo);
            if (result == null || result.SysNo <= 0)
            {
                TempData["ErrorMessage"] = "团购信息不存在！";
                return RedirectToRoute("Web_Error");
            }
            return View(result);
        }

        /// <summary>
        /// 团购订阅邮件
        /// </summary>
        /// <returns></returns>
        public JsonResult SubscriptEmail()
        {
            string email = Request["email"];
            if (string.IsNullOrWhiteSpace(email))
            {
                var data = new
                {
                    error = true,
                    message = "输入错误：邮箱为空！"
                };
                return new JsonResult { Data = data };
            }
            else
            {
                string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(email))
                {
                    var data = new
                    {
                        error = true,
                        message = "输入错误：邮箱格式错误！"
                    };
                    return new JsonResult { Data = data };
                }
            }

            //订阅类别：8-团购订阅
            List<int> list = new List<int>() { 8 };
            TopicFacade.InsertSubscriptionEmail(list, email);

            return new JsonResult() { Data = "订阅成功，谢谢！" };
        }
    }
}
