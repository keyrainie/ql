using ECommerce.Entity.ControlPannel;
using ECommerce.Service.ControlPannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Enums;

namespace ECommerce.Web.Controllers
{
    public class StockController : SSLControllerBase
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
            var queryFilter = BuildQueryFilterEntity<StockQueryFilter>();
            //所有商家都可以查看泰隆优选仓库
            queryFilter.ContainKJT = false;
            queryFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var data = StockService.QueryStock(queryFilter);
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
        //    var success = StockService.DelStock(sysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);
        //    return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        //}
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(StockInfo stockInfo)
        {
            stockInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = StockService.Create(stockInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(StockInfo stockInfo)
        {
            stockInfo.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = StockService.Edit(stockInfo);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }

        #endregion
    }
}
