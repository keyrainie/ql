using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Models.More;
using Nesoft.ECWeb.MobileService.Core;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class MoreController : BaseApiController
    {
        /// <summary>
        /// 获取关于我们
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAboutUs()
        {
            MoreManager homeManager = new MoreManager();
            NewsContentModel data = homeManager.GetAboutUs();
            var result = new AjaxResult
            {
                Success = true,
                Data = data
            };
            return Json(result);
        }

        /// <summary>
        /// 帮助中心分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHelpCategories()
        {
            MoreManager homeManager = new MoreManager();
            List<HelpCategoryModel> data = homeManager.GetHelpCategories();
            var result = new AjaxResult
            {
                Success = true,
                Data = data
            };
            return Json(result);
        }

        /// <summary>
        /// 根据系统编号获取帮助详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHelpContent(int id)
        {
            MoreManager homeManager = new MoreManager();
            NewsContentModel data = homeManager.GetNewsContent(id);
            var result = new AjaxResult
            {
                Success = true,
                Data = data
            };
            return Json(result);
        }

        /// <summary>
        /// 插入用户留言
        /// </summary>
        /// <param name="leaveWordsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertLeaveWords(UserLeaveWordsModel leaveWordsModel)
        {
            MoreManager homeManager = new MoreManager();
            bool data = homeManager.InsertLeaveWords(leaveWordsModel);
            var result = new AjaxResult
            {
                Success = true,
                Data = data
            };
            return Json(result);
        }
        /// <summary>
        /// 手机崩溃报告
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CrashLog(string content)
        {
            bool success = false;
            if (content != null && content.Length > 0)
            {
                MoreManager moreManager = new MoreManager();
                moreManager.AddCrashLog(content);
                success = true;
            }
            var result = new AjaxResult
            {
                Success = success
            };
            return Json(result);
        }
    }
}
