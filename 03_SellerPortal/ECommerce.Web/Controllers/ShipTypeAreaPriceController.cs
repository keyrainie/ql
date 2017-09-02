using ECommerce.Entity.ControlPannel;
using ECommerce.Service.ControlPannel;
using ECommerce.Web.Models.ControlPanel;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace ECommerce.Web.Controllers
{
    public class ShipTypeAreaPriceController : SSLControllerBase
    {
        
        public ActionResult List()
        {
            return View();
        }

        
        public ActionResult Detail(int sysNo, bool? batch)
        {
            return View(sysNo);
        }

        #region Ajax Methods

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query()
        {
            var queryFilter = BuildQueryFilterEntity<ShipTypeAreaPriceQueryFilter>();
            queryFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var data = ShipTypeAreaPriceService.QueryShipTypeAreaPrice(queryFilter);
            return AjaxGridJson(data);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Del()
        {
            var sysNos = Request.Form["SysNos"].Split(',').Select(t => int.Parse(t)).ToList();
            var success = ShipTypeAreaPriceService.VoidShipTypeAreaPrice(sysNos, UserAuthHelper.GetCurrentUser().SellerSysNo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ShipTypeAreaPriceInfo shipTypeAreaPriceInfo)
        {
            shipTypeAreaPriceInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var sysNos = this.Request.Form["AreaSysNoList"];
            if (!string.IsNullOrEmpty(sysNos))
            {
                shipTypeAreaPriceInfo.AreaSysNoList = sysNos.Split(',').Select(t => int.Parse(t)).ToList();
            }
            var success = ShipTypeAreaPriceService.Create(shipTypeAreaPriceInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ShipTypeAreaPriceInfo shipTypeAreaPriceInfo)
        {
            shipTypeAreaPriceInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = ShipTypeAreaPriceService.Edit(shipTypeAreaPriceInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }


        /// <summary>
        /// 动态加载区域
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddArea(int index)
        {
            var url = Request.QueryString["url"];
            return PartialView("~/Views/UserControls/AreaSelecter.cshtml", new AreaSelecterParamVM() { Tag = string.Format("Info{0}", index), HideDistrict = true });
        }

        #endregion
    }
}
