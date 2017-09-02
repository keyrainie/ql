using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Promotion;
using ECommerce.Service.Promotion;
using System.Web.Script.Serialization;
using ECommerce.Enums.Promotion;
using ECommerce.Utility;
using ECommerce.Web.Models;
using ECommerce.WebFramework;

namespace ECommerce.Web.Controllers
{
    public partial class PromotionController : SSLControllerBase
    {
        #region 团购相关
        private GroupBuyingService _groupBuyService = new GroupBuyingService();

        public ActionResult GroupBuyList()
        {
            return View();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GroupBuyQuery()
        {
            var queryFilter = BuildQueryFilterEntity<GroupBuyingQueryFilter>();
            var loginUser = UserAuthHelper.GetCurrentUser();
            queryFilter.SellerSysNo = loginUser.SellerSysNo;
            var data = _groupBuyService.Query(queryFilter);
            return AjaxGridJson(data);
        }

        /// <summary>
        /// 团购批量操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GroupBuyBatchAction()
        {
            BatchActionVM batchInfo = SerializationUtility.JsonDeserialize<BatchActionVM>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            string msg = LanguageHelper.GetText("操作成功。");
            switch (batchInfo.Action)
            {
                case "Submit":
                    _groupBuyService.Submit(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("提交审核成功。");
                    break;
                case "Void":
                    _groupBuyService.Void(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("作废成功。");
                    break;
                case "Stop":
                    _groupBuyService.Stop(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("终止成功。");
                    break;
                default:
                    break;
            }
            return Json(msg);
        }


        public ActionResult GroupBuyMaintain()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            GroupBuyingQueryResult data = null;
            if (sysNo != null && int.TryParse(sysNo, out id))
            {
                data = _groupBuyService.Load(id);
            }
            ViewBag.Data = new JavaScriptSerializer().Serialize(data);
            return View();
        }

        [HttpGet]
        public JsonResult GetProductAvailableSaleQty()
        {
            int productSysNo = int.Parse(Request.QueryString["productSysNo"]);
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = _groupBuyService.GetProductAvailableSaleQty(productSysNo);
            return result;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult GroupBuyMaintain(GroupBuyingInfo info)
        {
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _groupBuyService.Update(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("更新成功。"));
            }
            else
            {
                _groupBuyService.Create(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("创建成功。"));
            }
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GroupBuySubmit(GroupBuyingInfo info)
        {
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.Status = GroupBuyingStatus.WaitingAudit;
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _groupBuyService.Update(info);
            }
            else
            {
                _groupBuyService.Create(info);
            }
            return JsonSuccess(info.SysNo, LanguageHelper.GetText("提交审核成功。"));
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult GroupBuyVoid()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                _groupBuyService.Void(loginUser.SellerSysNo, loginUser.UserDisplayName, id);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("作废成功。"));
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult GroupBuyStop()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                _groupBuyService.Stop(loginUser.SellerSysNo, loginUser.UserDisplayName, id);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("终止成功。"));
        }

        #endregion

        #region 限时抢购相关


        private CountdownService _countdownService = new CountdownService();


        public ActionResult CountdownList()
        {
            return View();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CountdownQuery()
        {
            var queryFilter = BuildQueryFilterEntity<CountdownQueryFilter>();
            var loginUser = UserAuthHelper.GetCurrentUser();
            queryFilter.SellerSysNo = loginUser.SellerSysNo;
            var data = _countdownService.Query(queryFilter);
            return AjaxGridJson(data);
        }

        /// <summary>
        /// 限时抢购批量操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CountdownBatchAction()
        {
            BatchActionVM batchInfo = SerializationUtility.JsonDeserialize<BatchActionVM>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            string msg = LanguageHelper.GetText("操作成功。");
            switch (batchInfo.Action)
            {
                case "Submit":
                    _countdownService.Submit(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("提交审核成功。");
                    break;
                case "Void":
                    _countdownService.Void(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("作废成功。");
                    break;
                case "Stop":
                    _countdownService.Stop(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("终止成功。");
                    break;
                default:
                    break;
            }
            return Json(msg);
        }

        public ActionResult CountdownMaintain()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            CountdownQueryResult data = null;
            if (sysNo != null && int.TryParse(sysNo, out id))
            {
                data = _countdownService.Load(id);
            }
            ViewBag.Data = new JavaScriptSerializer().Serialize(data);
            return View();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CountdownMaintain(CountdownInfo info)
        {
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _countdownService.Update(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("更新成功。"));
            }
            else
            {
                _countdownService.Create(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("创建成功。"));
            }
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CountdownSubmit(CountdownInfo info)
        {
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.Status = CountdownStatus.WaitForPrimaryVerify;
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _countdownService.Update(info);
            }
            else
            {
                _countdownService.Create(info);
            }
            return JsonSuccess(info.SysNo, LanguageHelper.GetText("提交审核成功。"));
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult CountdownVoid()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                _countdownService.Void(loginUser.SellerSysNo, loginUser.UserDisplayName, id);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("作废成功。"));
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult CountdownStop()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            if (int.TryParse(sysNo, out id))
            {
                var loginUser = UserAuthHelper.GetCurrentUser();
                _countdownService.Stop(loginUser.SellerSysNo, loginUser.UserDisplayName, id);
            }
            else
            {
                return Json(LanguageHelper.GetText("参数错误。"));
            }

            return JsonSuccess(id, LanguageHelper.GetText("终止成功。"));
        }


        #endregion

        #region 捆绑促销相关


        private ComboService _comboService = new ComboService();


        public ActionResult ComboList()
        {
            return View();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ComboQuery()
        {
            var queryFilter = BuildQueryFilterEntity<ComboQueryFilter>();
            var loginUser = UserAuthHelper.GetCurrentUser();
            queryFilter.SellerSysNo = loginUser.SellerSysNo;
            var data = _comboService.Query(queryFilter);
            return AjaxGridJson(data);
        }

        /// <summary>
        /// 团购批量操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ComboBatchAction()
        {
            BatchActionVM batchInfo = SerializationUtility.JsonDeserialize<BatchActionVM>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            string msg = LanguageHelper.GetText("操作成功。");
            switch (batchInfo.Action)
            {
                case "Submit":
                    _comboService.Submit(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("提交审核成功。");
                    break;
                case "Void":
                    _comboService.Void(loginUser.SellerSysNo, loginUser.UserDisplayName, batchInfo.Ids);
                    msg = LanguageHelper.GetText("作废成功。");
                    break;
                default:
                    break;
            }
            return Json(msg);
        }

        public ActionResult ComboMaintain()
        {
            var sysNo = Request.QueryString["sysNo"];
            int id;
            ComboQueryResult data = null;
            if (sysNo != null && int.TryParse(sysNo, out id))
            {
                data = _comboService.Load(id);
            }
            ViewBag.Data = new JavaScriptSerializer().Serialize(data);
            return View();
        }

        /// <summary>
        /// 创建或编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ComboSave()
        {
            ComboInfo info = SerializationUtility.JsonDeserialize<ComboInfo>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _comboService.Update(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("更新成功。"));
            }
            else
            {
                _comboService.Create(info);
                return JsonSuccess(info.SysNo, LanguageHelper.GetText("创建成功。"));
            }
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ComboSubmit()
        {
            ComboInfo info = SerializationUtility.JsonDeserialize<ComboInfo>(Request.Form["data"]);
            var loginUser = UserAuthHelper.GetCurrentUser();
            info.Status = ComboStatus.WaitingAudit;
            info.SellerSysNo = loginUser.SellerSysNo;
            info.InUserSysNo = loginUser.UserSysNo;
            info.InUserName = loginUser.UserDisplayName;
            info.EditUserSysNo = loginUser.UserSysNo;
            info.EditUserName = loginUser.UserDisplayName;
            if (info.SysNo > 0)
            {
                _comboService.Update(info);
            }
            else
            {
                _comboService.Create(info);
            }
            return JsonSuccess(info.SysNo, LanguageHelper.GetText("提交审核成功。"));
        }

        #endregion

        private JsonResult JsonSuccess(int sysNo, string msg)
        {
            return Json(new { sysNo = sysNo, msg = msg });
        }
    }
}
