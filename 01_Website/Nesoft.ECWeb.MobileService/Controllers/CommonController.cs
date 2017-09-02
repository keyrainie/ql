using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Common;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class CommonController : BaseApiController
    {

        /// <summary>
        /// 获取所有省
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllProvinces()
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetAllProvinces() });
        }

        /// <summary>
        /// 根据省编号获取所有城市
        /// </summary>
        /// <param name="provinceSysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCitiesByProvinceSysNo(int provinceSysNo)
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetCitiesByProvinceSysNo(provinceSysNo) });
        }

        /// <summary>
        /// 根据城市编号获取所有区域
        /// </summary>
        /// <param name="citySysNo"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDistrictsByCitySysNo(int citySysNo)
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetDistrictsByCitySysNo(citySysNo) });
        }

        /// <summary>
        /// 根据支付方式ID获取支付配置信息
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetPaymentSettingInfo(int payTypeSysNo)
        {
            var settingInfo = CommonManager.GetPaymentSettingInfo(payTypeSysNo);
            var sectionInfo = CommonManager.GetPaymentSectionInfo(payTypeSysNo);
            return Json(new AjaxResult() { Success = true, Data = new { PaymentBase = settingInfo.PaymentBase, PaymentMode = settingInfo.PaymentMode, PaySectionInfo = sectionInfo } });
        }

        /// <summary>
        /// 获得售后请求
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRequests()
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetRequests() });
        }

        /// <summary>
        /// 获得寄回方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetShipTypes()
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetShipTypes() });
        }

        /// <summary>
        /// 获得申请理由
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRMAReasons()
        {
            return Json(new AjaxResult() { Success = true, Data = CommonManager.GetRMAReasons() });
        }
    }
}
