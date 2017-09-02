using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity;
using ECommerce.Entity.Order;
using ECommerce.Facade.GroupBuying;
using ECommerce.Facade.Member;

namespace ECommerce.UI.Controllers
{
    public class MemberOrderController : SSLControllerBase
    {
        //
        // GET: /Web/MemberOrder/

        [ValidateInput(false)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }

        public JsonResult VoidedOrder()
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            string strOrderSysNo = Request["OrderSysNo"];
            string message = Request["Message"] ?? "";
            int orderSysNo;
            if (int.TryParse(strOrderSysNo, out orderSysNo))
            {
                LoginUser suer = UserMgr.ReadUserInfo();
                result.Data = CustomerFacade.VoidedOrder(orderSysNo, message, suer.UserSysNo);
            }
            else
            {
                result.Data = "";
            }


            return result;
        }

        public ActionResult GetSOLogHtml()
        {
            string strSOSysNo = Request["SOSysNo"] ?? "";
            int SOSysNo;
            if (int.TryParse(strSOSysNo, out SOSysNo))
            {
                var log = CustomerFacade.GetOrderLogBySOSysNo(SOSysNo).Where(p => p.OptType > 0).ToList();
                for (var i = 0; i < log.Count; i++)
                {
                    if (log[i].OptType == 600606 && (i + 1) < log.Count && log[i + 1].OptType == 201)
                    {
                        log[i].Note += string.Format(" {0}", log[i + 1].Note);
                        log.Remove(log[i + 1]);
                    }
                }
                return View("_SOLog", log);
            }
            return View("_SOLog", new List<SOLog>());
        }

        public JsonResult ModifyOrderMemo()
        {
            var strSOSysNo = Request["SOSysNo"] ?? "";
            int SOSysNo;
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (int.TryParse(strSOSysNo, out SOSysNo))
            {
                result.Data = CustomerFacade.ModifyOrderMemo(SOSysNo, Request["Memo"] ?? "");
            }
            else
            {
                result.Data = "";
            }
            return result;
        }

        public ActionResult GroupBuyTicket()
        {
            int pageIndex = 0;
            if (int.TryParse(Request.Params["page"], out pageIndex))
                pageIndex--;

            PageInfo pageInfo = new PageInfo()
            {
                PageIndex = pageIndex,
                PageSize = 10
            };

            QueryResult<GroupBuyingTicketInfo> result = GroupBuyingFacade.QueryGroupBuyingTicketInfo(pageInfo, this.CurrUser.UserSysNo);
            result.PageInfo.PageIndex++;

            return View(result);
        }

        public JsonResult VoidedTicket()
        {
            int sysNo = 0;
            if (!int.TryParse(Request.Params["SysNo"], out sysNo))
            {
                return new JsonResult() { Data = this.BuildAjaxErrorObject("请输入正确的编号！") };
            }
            GroupBuyingFacade.VoidedTicketBySysNo(sysNo);

            return new JsonResult() { Data = "" };        
        }
    }
}
