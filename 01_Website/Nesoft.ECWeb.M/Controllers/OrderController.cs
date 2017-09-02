using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Order;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.WebFramework;

namespace Nesoft.ECWeb.M.Controllers
{
    public class OrderController : SSLControllerBase
    {
        //
        // GET: /Order/

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Detail(int? OrderSysNo)
        {
            if (OrderSysNo.HasValue)
            {
                return View(OrderSysNo.Value);
            }
            return View();
        }
        public ActionResult Track(int? OrderSysNo)
        {
            if (OrderSysNo.HasValue)
            {
                return View(OrderSysNo.Value);
            }
            return View();
        }

        public ActionResult Ajax_QueryOrder()
        {
            var pageIndex = int.Parse(Request["PageIndex"]);
            var queryType = int.Parse(Request["QueryType"]);

            SOQueryInfo query = new SOQueryInfo();
            //搜索类型,默认是搜索[最近三个月的-15][14-所有订单]
            query.SearchType = queryType == 1 ?
                SOSearchType.LastThreeMonths : SOSearchType.ALL;
            query.PagingInfo = new PageInfo();
            query.PagingInfo.PageSize = 5;
            query.PagingInfo.PageIndex = pageIndex;
            query.CustomerID = CurrUser.UserSysNo;
            query.Status = null;

            QueryResult<OrderInfo> orders = CustomerFacade.GetOrderList(query);

            //如果查询类型是【三个月内】下单，则需要合并最近下单的数据
            if (query.SearchType == SOSearchType.LastThreeMonths && pageIndex == 0)
            {
                var recentOrderSysNoes = CookieHelper.GetCookie<string>("SoSysNo");
                if (!string.IsNullOrWhiteSpace(recentOrderSysNoes))
                {
                    var soSysNoes = recentOrderSysNoes.Split(',').ToList<string>();
                    var recentOrders = CustomerFacade.GetCenterOrderMasterList(query.CustomerID, soSysNoes);
                    if (recentOrders != null && orders != null && orders.ResultList != null)
                    {
                        //排除掉orders中已经存在的数据
                        recentOrders.RemoveAll(p => orders.ResultList.Any(q => q.SoSysNo == p.SoSysNo));
                        //将最近的订单加载到orders中
                        for (var i = recentOrders.Count - 1; i >= 0; i--)
                        {
                            orders.ResultList.Insert(0, recentOrders[i]);
                        }
                    }
                }
            }
            if (orders != null)
            {
                if (orders.ResultList != null)
                {
                    for (var i = 0; i < orders.ResultList.Count; i++)
                    {
                        orders.ResultList[i] = CustomerFacade.GetCenterSODetailInfo(CurrUser.UserSysNo, orders.ResultList[i].SoSysNo);
                        orders.ResultList[i].SOItemList.ForEach(q =>
                        {
                            q.DefaultImage = ProductFacade.BuildProductImage(ImageSize.P60, q.DefaultImage);
                        });
                    }
                }
            }


            var result = new AjaxResult { Success = true, Data = orders };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ajax_VoidOrder()
        {
            var result = new AjaxResult { Success = true };

            string strOrderSysNo = Request["OrderSysNo"];
            string message = Request["Message"] ?? "";
            int orderSysNo;
            if (int.TryParse(strOrderSysNo, out orderSysNo))
            {
                LoginUser suer = UserManager.ReadUserInfo();
                var m = CustomerFacade.VoidedOrder(orderSysNo, message, suer.UserSysNo);
                if (!string.IsNullOrWhiteSpace(m))
                {
                    result.Success = false;
                    result.Message = string.Format("作废失败:{0}", m);
                }
            }
            else
            {
                result.Success = false;
                result.Data = "无效的订单编号";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
