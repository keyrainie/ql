using ECommerce.Entity.Member;
using ECommerce.Facade.GiftCard;
using ECommerce.Facade.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.GiftCard;
using ECommerce.Facade.GiftCard;
using ECommerce.WebFramework;

namespace ECommerce.UI.Controllers
{
    public class GiftCardController : WWWControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 我的礼品卡
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult MyGiftCard()
        {
            return View();
        }

        [HttpPost]
        [Auth(NeedAuth = true)]
        public ActionResult LookPassword()
        {
            string Code = Request["Code"].ToString();

            LoginUser suer = UserMgr.ReadUserInfo();
            //CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);

            string password = GiftCardFacade.LookPassword(Code, suer.UserSysNo);
            if (password != null)
                return Json(password, JsonRequestBehavior.AllowGet);
            else
                return Json("未能获取礼品卡密码", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 礼品卡消费记录
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult UsedRecord()
        {
            int pageIndex = 0;
            if (!int.TryParse(Request.Params["page"], out pageIndex))
            {
                pageIndex = 1;
            };

            string code = Request["txtSearch"];
            code = string.IsNullOrEmpty(code) ? code : code.Trim();

            UsedRecordQuery query = new UsedRecordQuery();
            query.PageInfo.PageIndex = pageIndex;
            query.PageInfo.PageSize = 10;
            query.Code = code;
            query.CustomerSysNo = this.CurrUser.UserSysNo;

            var result = GiftCardFacade.QueryUsedRecord(query);
            return View(result);
        }

        /// <summary>
        /// 礼品卡绑定
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult Binding()
        {
            return View();
        }

        /// <summary>
        /// 礼品卡修改密码
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = true)]
        public ActionResult ModifyPwd()
        {
            return View();
        }

        #region AJAX提交处理

        /// <summary>
        /// 加载礼品卡信息
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxLoadGiftCard(FormCollection form)
        {
            string code = Request["Code"].Trim();
            string password = Request["Password"];

            var temp = GiftCardFacade.LoadGiftCard(code, password);
            if (temp == null)
            {
                return Json(new
                {
                    Result = false,
                    Message = "卡号或密码错误，请输入正确的卡号和密码"
                });
            }
            else
            {
                var newTemp = new
                {
                    Code = temp.Code,
                    EndDate = temp.EndDate.Value.ToString("yyyy-MM-dd"),
                    AvailAmount = temp.AvailAmount.ToString("F2"),
                    Status = temp.BindingCustomerSysNo > 0 ? "已绑定" : "未绑定"
                };
                return Json(new
                {
                    Result = true,
                    Message = newTemp
                });
            }
        }

        /// <summary>
        /// 礼品卡绑定帐号
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxBindGiftCard(FormCollection form)
        {
            string code = Request["Code"].Trim();
            string password = Request["Password"];

            var temp = GiftCardFacade.LoadGiftCard(code, password);
            if (temp == null)
            {
                return Json(new
                {
                    Result = false,
                    Message = "卡号或密码错误，请输入正确的卡号和密码"
                });
            }
            else if (temp.BindingCustomerSysNo > 0)
            {
                return Json(new
                    {
                        Result = false,
                        Message = "礼品卡已经被绑定，不能重复绑定"
                    });
            }
            else if (temp.EndDate < DateTime.Now)
            {
                return Json(new
                {
                    Result = false,
                    Message = "礼品卡已过期，不能绑定"
                });
            }
            else
            {
                GiftCardFacade.BindGiftCard(code, this.CurrUser.UserSysNo);
                return Json(new
                {
                    Result = true,
                    Message = "礼品卡绑定成功"
                });
            }
        }

        /// <summary>
        /// 礼品卡修改密码
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxModifyGiftCardPwd(FormCollection form)
        {
            string code = Request["Code"].Trim();
            string password = Request["Password"];
            string newPassword = Request["NewPassword"];

            var temp = GiftCardFacade.LoadGiftCard(code, password);
            if (temp == null)
            {
                return Json(new
                {
                    Result = false,
                    Message = "卡号或密码错误，请输入正确的卡号和密码"
                });
            }
            else if (temp.EndDate < DateTime.Now)
            {
                return Json(new
                {
                    Result = false,
                    Message = "礼品卡已过期，不能修改密码"
                });
            }
            else if ((temp.BindingCustomerSysNo > 0 && temp.BindingCustomerSysNo != CurrUser.UserSysNo) || (temp.BindingCustomerSysNo == 0 && temp.CustomerSysNo != CurrUser.UserSysNo))
            {
                return Json(new
                {
                    Result = false,
                    Message = "礼品卡未绑定或不属于此帐号，不能修改密码"
                });
            }
            else
            {
                GiftCardFacade.ModifyGiftCardPwd(code, newPassword);
                return Json(new
                {
                    Result = true,
                    Message = "礼品卡密码修改成功"
                });
            }
        }

        #endregion
    }
}
