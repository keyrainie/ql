using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Facade.Topic;
using System.Text.RegularExpressions;

namespace ECommerce.UI.Controllers
{
    public class TopicController : WWWControllerBase
    {
        //
        // GET: /Web/Topic/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TopicList(int? categorySysNo)
        {
            if (!categorySysNo.HasValue)
            {
                TempData["ErrorMessage"] = "新闻信息错误!";
            }

            return View(categorySysNo);
        }

        public ActionResult TopicDetail(int? topicSysNo)
        {
            if (!topicSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "新闻信息错误!";
            }

            return View(topicSysNo);
        }

        public ActionResult HelpContent(int? topicSysNo)
        {
            if (!topicSysNo.HasValue)
            {
                TempData["ErrorMessage"] = "帮助中心信息错误!";
            }

            return View(topicSysNo);
        }

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


            //订阅类别：7-泰隆优选让利促销活动
            List<int> list = new List<int>() { 7 };
            TopicFacade.InsertSubscriptionEmail(list, email);

            return new JsonResult() { Data = "订阅成功，谢谢！" };
        }

    }
}
