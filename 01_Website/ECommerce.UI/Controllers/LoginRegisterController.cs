using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Facade.Member;
using ECommerce.Entity.Member;
using ECommerce.WebFramework;

namespace ECommerce.UI.Controllers
{
    public class LoginRegisterController : WWWControllerBase
    {
        //
        // GET: /Web/Customer/
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(Request["returnurl"]))
            {
                ViewBag.ReturnUrl = Request["returnurl"];
            }
            else
            {
                ViewBag.ReturnUrl = PageHelper.BuildUrl("Web_Index");
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterNote()
        {
            return View();
        }

        /// <summary>
        /// 找回密码-重置密码
        /// </summary>
        /// <returns></returns>
        public ActionResult FindPassword()
        {
            //if (Request["token"] != null && !string.IsNullOrEmpty(Request["token"].ToString().Trim()))
            //{
            //    string token = Request["token"].ToString().Trim();
            //    CustomerPasswordToken passwordToken = ServiceHelper.GetService<ICustomerService>().GetCustomerPasswordToken(token, "E");
            //    if (passwordToken != null)
            //        ServiceHelper.GetService<ICustomerService>().UpdatePasswordTokenStatus(token);
            //    else
            //        ViewBag.Message = LanguageHelper.GetText("Token已经过期或不存在！");
            //}
            //else
            //    ViewBag.Message = LanguageHelper.GetText("Token已经过期或不存在！");
            string FindPasswordSMSCodeRight = CookieHelper.GetCookie<String>("FindPasswordSMSCodeRight");
            if (!string.IsNullOrEmpty(FindPasswordSMSCodeRight) && FindPasswordSMSCodeRight == "FindPasswordSMSCodeRight")
            {
                ViewBag.Step = "step2";
                CookieHelper.SaveCookie("FindPasswordSMSCodeRight", "");
            }
            else if (Request["token"] != null && !string.IsNullOrEmpty(Request["token"].ToString()))
            {
                string token = Request["token"].ToString().Trim();
                CustomerPasswordTokenInfo passwordToken = LoginFacade.GetPasswordTokenInfo(token, "E");
                if (passwordToken != null)
                {
                    CookieHelper.SaveCookie<String>("FindPasswordCustomerID", Request["CustomerID"].ToString());
                    LoginFacade.UpdatePasswordToken(token);
                }
                else
                    ViewBag.Message = "Token已经过期或不存在！";
                ViewBag.Step = "step2";//验证邮箱过来的验证地址
            }
            else
                ViewBag.Step = "step1";
            return View("FindPassword");
        }

        /// <summary>
        /// 商家入驻申请
        /// </summary>
        /// <returns></returns>
        public ActionResult SellerRegister()
        {
            return View();
        }


        public ActionResult Welcome()
        {
            return View();
        }


        /// <summary>
        /// 判断此手机号码是否已经被验证过;
        /// 是:则不能对此手机号码再次进行验证
        /// 否:验证手机号码发送短信
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSendValidateCellphoneByCode(FormCollection form)
        {
            //return Json("s", JsonRequestBehavior.AllowGet);
            string validatedCode = Request["ValidateCode"].ToString();
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() != validatedCode.ToLower())
            {
                return Json(new JsonResult() { ContentType = "验证码不正确", Data = "" }, JsonRequestBehavior.AllowGet);
            }


            string cell = Request["CellPhoneNumber"].ToString();
            if (!string.IsNullOrEmpty(cell))
            {

                //判断手机号码是否被验证过
                //if (CustomerFacade.PhoneIsValidate(cell))
                //{
                //    return Json(new JsonResult(){ContentType="此手机号码已经被验证过,不能进行重复验证"} , JsonRequestBehavior.AllowGet);
                //}
                CellPhoneConfirm item = new CellPhoneConfirm();
                item.CustomerSysNo = 0;
                item.CellPhone = cell;

                string code = VerifyImage.CreateRandomNumber();

                item.ConfirmKey = code;
                int CellPhoneSysNo=CustomerFacade.CreateCellPhoneConfirm(item).SysNo;
                if (CellPhoneSysNo > 0)
                    return Json(new JsonResult() { ContentType = "s", Data = CellPhoneSysNo },JsonRequestBehavior.AllowGet);
                if(CellPhoneSysNo==-99999)
                {
                    return Json(new JsonResult() { ContentType = "同一个IP地址24小时内只能请求验证码10次，同一个手机号码请求验证码5次。" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new JsonResult() { ContentType = "服务器忙,稍后重试" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResult() { ContentType = "服务器忙,稍后重试" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 验证用户的手机号码
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxValidateCellphoneByCode(FormCollection form)
        {
            string CellPhoneNumber = Request["CellPhoneNumber"].ToString();
            string SmsCode = Request["SmsCode"].ToString();

            if (!string.IsNullOrEmpty(SmsCode) && !string.IsNullOrEmpty(CellPhoneNumber))
            {
                //送积分因为注册手机验证时用户还不存在所以没办法这个时候送积分
                //Point point = new Point();
                //point.CustomerSysNo = CurrUser.UserSysNo;
                //point.AvailablePoint = ConstValue.GetPointByValidatePhone;
                //point.ExpireDate = DateTime.Now.AddYears(1);
                //point.InDate = DateTime.Now;
                //point.InUser = CurrUser.UserID;
                //point.Memo = EnumHelper.GetDescription(PointType.MobileVerification);
                //point.ObtainType = (int)PointType.MobileVerification;
                //point.Points = ConstValue.GetPointByValidatePhone;
                //point.IsFromSysAccount = 1;
                //point.SysAccount = int.Parse(ConstValue.PointAccountSysNo);

                if (CustomerFacade.ValidateCustomerPhoneWithoutPoint(CellPhoneNumber, SmsCode.ToLower()))
                {
                    CookieHelper.SaveCookie("ValidatePhone", true);
                    CookieHelper.SaveCookie("CanceledPhoneValidate", false);

                    return Json("s", JsonRequestBehavior.AllowGet);
                }
                return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
            }
            return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
        }
    }
}
