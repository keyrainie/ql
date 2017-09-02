using ECommerce.Entity.ControlPannel;
using ECommerce.Service.ControlPannel;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ECommerce.Web.Controllers
{
    public class ShipTypeController : SSLControllerBase
    {
        
        public ActionResult List()
        {
            return View();
        }
        
        public ActionResult Detail(int sysNo)
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
            var queryFilter = BuildQueryFilterEntity<ShipTypeQueryFilter>();
            queryFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var data = ShipTypeService.QueryShipType(queryFilter);
            return AjaxGridJson(data);
        }
        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="sysNo"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Del(int sysNo)
        //{
        //    var success = ShipTypeService.DelShipType(sysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);
        //    return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        //}
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ShipTypeInfo stockInfo)
        {
            stockInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = ShipTypeService.Create(stockInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ShipTypeInfo stockInfo)
        {
            stockInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = ShipTypeService.Edit(stockInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }

        #endregion
    }
}
