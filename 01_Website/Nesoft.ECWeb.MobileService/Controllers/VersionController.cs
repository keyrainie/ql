using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Version;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class VersionController : BaseApiController
    {
        /// <summary>
        /// 验证更新
        /// </summary>
        /// <param name="versionCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckVersionUpdate(string versionCode)
        {
            var data = new AjaxResult()
            {
                Code = 0,
                Data = VersionManager.CheckVersion(versionCode),
                Success = true
            };

            JsonResult result = new JsonResult();
            result.Data = data;
            return result;
        }
    }
}
