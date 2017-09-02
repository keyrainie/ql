using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.ControlPannel;
using ECommerce.Service.ControlPannel;

namespace ECommerce.Web.Controllers
{
    public class StockShipTypeController : SSLControllerBase
    {
        //
        // GET: /StockShipType/
        
        public ActionResult List()
        {
            var stockQueryFilter = new StockQueryFilter();
            //获得自贸仓库 ContainKJT设置为true
            stockQueryFilter.ContainKJT = false;//UserAuthHelper.GetCurrentUser().VendorStockType == Entity.Store.Vendor.VendorStockType.NEG;
            stockQueryFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            stockQueryFilter.PageIndex = 0;
            stockQueryFilter.PageSize = 100000;
            var StockList = StockService.QueryStock(stockQueryFilter);
            ViewBag.StockList = StockList.ResultList;


            var shipTypeQueryFilter = new ShipTypeQueryFilter();
            shipTypeQueryFilter.MerchantSysNo = 1;
            shipTypeQueryFilter.PageIndex = 0;
            shipTypeQueryFilter.PageSize = 100000;
            var ShipTypeList = ShipTypeService.QueryShipType(shipTypeQueryFilter);
            ViewBag.ShipTypeList = ShipTypeList.ResultList;

            return View();
        }
        
        public ActionResult Maintain()
        {
            var stockQueryFilter = new StockQueryFilter();
            //获得自贸仓库 ContainKJT设置为true
            stockQueryFilter.ContainKJT = false;//UserAuthHelper.GetCurrentUser().VendorStockType == Entity.Store.Vendor.VendorStockType.NEG;
            stockQueryFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            stockQueryFilter.PageIndex = 0;
            stockQueryFilter.PageSize = 100000;
            var StockList = StockService.QueryStock(stockQueryFilter);
            ViewBag.StockList = StockList.ResultList;


            var shipTypeQueryFilter = new ShipTypeQueryFilter();
            shipTypeQueryFilter.MerchantSysNo = 1;
            shipTypeQueryFilter.PageIndex = 0;
            shipTypeQueryFilter.PageSize = 100000;
            var ShipTypeList = ShipTypeService.QueryShipType(shipTypeQueryFilter);
            ViewBag.ShipTypeList = ShipTypeList.ResultList;

            if (Request["sysno"] == null || int.Parse(Request["sysno"]) == 0)
            {
                ViewBag.StockShipTypeInfo = new StockShipTypeInfo();
            }
            else
            {
                ViewBag.StockShipTypeInfo = StockService.GetStockShipTypeInfo(int.Parse(Request["sysno"]));
            }

            return View();
        }

        public ContentResult Query()
        {
            var queryFilter = BuildQueryFilterEntity<StockShipTypeQueryFilter>();
            queryFilter.SellerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            return AjaxGridJson(StockService.QueryStockShipType(queryFilter));
        }

        public JsonResult Create(StockShipTypeInfo info)
        {
            SetBizEntityUserInfo(info, true);
            SetBizEntityUserInfo(info, false);
            StockService.CreateStockShipType(info);
            return new JsonResult();
        }


        public JsonResult Update(StockShipTypeInfo info)
        {
            SetBizEntityUserInfo(info, true);
            SetBizEntityUserInfo(info, false);
            StockService.UpdateStockShipType(info);
            return new JsonResult();

        }
    }
}
