using ECommerce.Entity.Member;
using ECommerce.Facade.Member;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.Facade.Common;
using System.Configuration;
using ECommerce.Entity.Order;
using ECommerce.Entity.Shipping;
using System.Web.Script.Serialization;
using ECommerce.Facade.Shopping;
using ECommerce.Facade.Shipping;
using ECommerce.Facade.Product;
using ECommerce.Entity.Product;
using ECommerce.Facade;
using ECommerce.Entity;
using ECommerce.Utility;
using ECommerce.Entity.RMA;
using ECommerce.Facade.RMA;

namespace ECommerce.UI.Controllers
{
    public class MemberAccountController : SSLControllerBase
    {
        //
        // GET: /Web/MemberAccount/


        /// <summary>
        /// 帐户中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerInfo()
        {
            return View();
        }
        public ActionResult CustomerExtendedInfo()
        {
            return View();
        }

        /// <summary>
        /// 收货地址
        /// </summary>
        /// <returns></returns>
        public ActionResult ShippingInfo()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeAvatar()
        {
            return View();
        }

        /// <summary>
        /// 账户余额
        /// </summary>
        /// <returns></returns>
        public ActionResult PrepayHistory()
        {
            return View();
        }
        /// <summary>
        /// 我的积分
        /// </summary>
        /// <returns></returns>
        public ActionResult MyPoint()
        {
            return View();
        }

        /// <summary>
        /// 经验值历史
        /// </summary>
        /// <returns></returns>
        public ActionResult ExperienceHistory()
        {
            return View();
        }

        /// <summary>
        /// 到货通知
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductNotify()
        {
            return View();
        }

        /// <summary>
        /// 降价通知
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceNotify()
        {
            return View();
        }

        public JsonResult CreateDynamicConfirmInfo()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_SOURCE_IP"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                    }
                }
                else
                {
                    // 取逗号分隔第一个 IP 为客户端IP
                    string[] tmp = ip.Split(new char[] { ',' });
                    ip = tmp[0];
                }
            }
            else
            {
                // 取逗号分隔第一个 IP 为客户端IP
                string[] tmp = ip.Split(new char[] { ',' });
                ip = tmp[0];
            }
            int CustomerSysNo = CookieHelper.GetCookie<int>("CustomerID");
            string CellPhone = Request["CellPhone"];
            string ConfirmKey = Request["ConfirmKey"];
            CustomerInfo customer = CustomerFacade.GetCustomerInfo(CustomerSysNo);

            JsonResult result = new JsonResult();
            /*PhoneDynamicValidationInfo dynamicValidation = new PhoneDynamicValidationInfo();
            dynamicValidation.CustomerSysNo = CookieHelper.GetCookie<int>("CustomerID");
            dynamicValidation.FromIP = ip;
            dynamicValidation.CellPhone = CellPhone;
            dynamicValidation.ConfirmKey = ConfirmKey;
           
            dynamicValidation.IntervalMinute = 5;
            dynamicValidation.IntervalSecond = 10;
            dynamicValidation.IsRepeatTimes = 10;
            dynamicValidation.TotalSendTimes = 100;
            dynamicValidation.InvalidateMinute = 3;
            CreateDynamicStatus dynamicStatus = CustomerFacade.CreateDynamicConfirmInfo(dynamicValidation);
            if(dynamicStatus== CreateDynamicStatus.ValidatePass)
            {

            }
            */
            //您已验证过手机，请先取消验证InvalidAuth
            //您在短时间内获取短信验证码次数过多，请稍后再试OverTotalTimes
            //{0}秒内不能重复获取，请稍后再获取RepeatClick
            //手机验证码稍后会以短信形式发送到您的手机上，请耐心等待！
            return result;
        }

        #region AJAX提交处理
        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxUpdateCustomerInfo(FormCollection form)
        {
            string PersonalInfo = Request["PersonalInfo"].ToString();
            PersonalInfo = System.Web.HttpUtility.UrlDecode(PersonalInfo);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            CustomerInfo PersonInfo = jss.Deserialize<CustomerInfo>(PersonalInfo);
            PersonInfo.CustomerID = CurrUser.UserID;
            if (CustomerFacade.UpdateCustomerPersonInfo(PersonInfo))
                return Json("s", JsonRequestBehavior.AllowGet);
            return Json("服务器繁忙，稍后重试", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 发送验证邮件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSendValidateEmail(FormCollection form)
        {
            string email = Request["Email"].ToString();
            string imgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();//图片根目录
            string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();

            CustomerInfo info = CustomerFacade.GetCustomerByID(CurrUser.UserID);
            if (!string.IsNullOrEmpty(info.Email) && email != info.Email)
            {
                return Json("修改邮件地址，请先保存再发送验证邮件", JsonRequestBehavior.AllowGet);
            }
            if (CustomerFacade.CheckEmail(email))
            {
                return Json("此邮箱已经被验证过，请使用其它邮箱", JsonRequestBehavior.AllowGet);
            }
            if (CustomerFacade.SendEmailValidatorMail(CurrUser.UserID, email, imgBaseUrl, domain))
            {
                if (email != info.Email)
                {
                    CustomerFacade.UpdateCustomerEmailAddress(CurrUser.UserID, email);
                }
                return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("服务器繁忙，稍后重试", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建或修改收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxUpdateCustomerExtendedInfo(FormCollection form)
        {
            string PExtendInfo = Request["PExtendInfo"].ToString();
            PExtendInfo = System.Web.HttpUtility.UrlDecode(PExtendInfo);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            CustomerExtendPersonInfo PersonInfo = jss.Deserialize<CustomerExtendPersonInfo>(PExtendInfo);
            PersonInfo.CustomerSysNo = CurrUser.UserSysNo;
            if (CustomerFacade.UpdateCustomerPersonExtendInfo(PersonInfo))
                return Json("s", JsonRequestBehavior.AllowGet);
            return Json("服务器繁忙，稍后重试", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建或修改收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCustomerShippingInfo(FormCollection form)
        {
            string Action = Request["Action"].ToString();
            string Data = Request["Data"].ToString();
            Data = System.Web.HttpUtility.UrlDecode(Data);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ShippingContactInfo shippingAddress = jss.Deserialize<ShippingContactInfo>(Data); // JsonHelper.JsonToObject<ShippingContactInfo>(Data);
            shippingAddress.ReceiveName = shippingAddress.ReceiveContact;
            if (Action == "2")
                CustomerShippingAddresssFacade.DeleteCustomerContactInfo(shippingAddress.SysNo, CurrUser.UserSysNo);
            else
                CustomerShippingAddresssFacade.EditCustomerContactInfo(shippingAddress, CurrUser.UserSysNo);
            return Json("s", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建售后申请单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateRMARequestInfo(FormCollection form)
        {
            string Data = Request["Data"].ToString();
            string Action = Request["Action"].ToString();
            Data = System.Web.HttpUtility.UrlDecode(Data);
            Action = System.Web.HttpUtility.UrlDecode(Action);
            
            JavaScriptSerializer jss = new JavaScriptSerializer();
            RMARequestInfo requestInfo = jss.Deserialize<RMARequestInfo>(Data);
            if (Action.Trim().Equals("wait"))
            {
                requestInfo.IsSubmit = false;
            }
            else
            {
                requestInfo.IsSubmit = true;
            }
            requestInfo.Registers.RemoveAll(s=>s==null);
            string requestrSysno = RMARequestFacade.CreateRMARequest(requestInfo);
            return Json(new {Result = "s", RequestrSysno = requestrSysno}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交售后申请单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitRMARequestInfo(FormCollection form)
        {
            string Data = Request["Data"].ToString();
            Data = System.Web.HttpUtility.UrlDecode(Data);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            RMARequestInfo requestInfo = jss.Deserialize<RMARequestInfo>(Data);
            requestInfo.IsSubmit = true;
            string requestrSysno = RMARequestFacade.SubmitRMARequest(requestInfo);
            return Json(new { Result = "s", RequestrSysno = requestrSysno }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxChangePassword(FormCollection form)
        {
            string OldPassword = Request["OldPassword"].ToString();
            string Password = Request["Password"].ToString();
            string RePassword = Request["RePassword"].ToString();


            //string salt = LoginFacade.GetCustomerPasswordSalt(CurrUser.UserID);
            //OldPassword = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(OldPassword.Replace("+", "%2b")) + salt);
            // [2014/12/22 by Swika]增加支持第三方系统导入的账号的密码验证
            var encryptMeta = LoginFacade.GetCustomerEncryptMeta(CurrUser.UserID);
            OldPassword = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(OldPassword.Replace("+", "%2b")), encryptMeta);


            if (LoginFacade.CustomerLogin(CurrUser.UserID, OldPassword) == null)
                return Json("旧密码不正确", JsonRequestBehavior.AllowGet);
            else
            {
                string encryptPassword = string.Empty;
                string passwordSalt = string.Empty;
                PasswordHelper.GetNewPasswordAndSalt(ref Password, ref encryptPassword, ref passwordSalt);
                //重置密码
                if (CustomerFacade.UpdateCustomerPassword(CurrUser.UserID, encryptPassword, passwordSalt))
                {
                    return Json("s", JsonRequestBehavior.AllowGet);
                }
                return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新用户的头像图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxChangeCustomerAvatar(FormCollection form)
        {
            string AvatarImg = Request["AvatarImg"].ToString();
            if (CustomerFacade.ChangeCustomerAvatarImg(AvatarImg, CurrUser.UserSysNo, AvtarImageStatus.D))
                return Json("s", JsonRequestBehavior.AllowGet);
            return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询订单的日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QuerySOLogBySOSysNo(FormCollection form)
        {
            int sosysno = int.Parse(Request["SOSysNo"].ToString());
            List<SOLog> log = CustomerFacade.GetOrderLogBySOSysNo(sosysno).Where(p => p.OptType > 0).ToList();
            for (var i = 0; i < log.Count; i++)
            {
                if (log[i].OptType == 600606 && (i + 1) < log.Count && log[i + 1].OptType == 201)
                {
                    log[i].Note += string.Format(" {0}", log[i + 1].Note);
                    log.Remove(log[i + 1]);
                }
            }
            return Json(log, JsonRequestBehavior.AllowGet);
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
            string cell = Request["CellPhoneNumber"].ToString();
            if (!string.IsNullOrEmpty(cell))
            {
                //判断手机号码是否被验证过
                if (CustomerFacade.PhoneIsValidate(cell))
                {
                    return Json("此手机号码已经被验证过,不能进行重复验证", JsonRequestBehavior.AllowGet);
                }
                CellPhoneConfirm item = new CellPhoneConfirm();
                item.CustomerSysNo = CurrUser.UserSysNo;
                item.CellPhone = cell;

                string code = VerifyImage.CreateRandomNumber();

                item.ConfirmKey = code;
                int CellPhoneSysNo = CustomerFacade.CreateCellPhoneConfirm(item).SysNo;
                if (CellPhoneSysNo > 0)
                    return Json("s", JsonRequestBehavior.AllowGet);

                if (CellPhoneSysNo == -99999)
                {
                    return Json("同一个IP地址24小时内只能请求验证码10次，同一个手机号码请求验证码5次。", JsonRequestBehavior.AllowGet);
                }
                return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
            }
            return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
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
            string ValidatedCode = Request["ValidatedCode"].ToString();
            string SmsCode = Request["SmsCode"].ToString();

            //在验证手机验证码前对手机号码进行验证避免出现个人恶意攒一堆验证码绑定多个账户
            //判断手机号码是否被验证过
            if (CustomerFacade.PhoneIsValidate(CellPhoneNumber))
            {
                return Json("此手机号码已经被验证过,不能进行重复验证", JsonRequestBehavior.AllowGet);
            }

            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == ValidatedCode.ToLower())
            {
                if (!string.IsNullOrEmpty(SmsCode) && !string.IsNullOrEmpty(CellPhoneNumber))
                {
                    //送积分
                    Point point = new Point();
                    point.CustomerSysNo = CurrUser.UserSysNo;
                    point.AvailablePoint = ConstValue.GetPointByValidatePhone;
                    point.ExpireDate = DateTime.Now.AddYears(1);
                    point.InDate = DateTime.Now;
                    point.InUser = CurrUser.UserID;
                    point.Memo = EnumHelper.GetDescription(PointType.MobileVerification);
                    point.ObtainType = (int)PointType.MobileVerification;
                    point.Points = ConstValue.GetPointByValidatePhone;
                    point.IsFromSysAccount = 1;
                    point.SysAccount = int.Parse(ConstValue.PointAccountSysNo);

                    if (CustomerFacade.ValidateCustomerPhone(CellPhoneNumber, SmsCode.ToLower(), point))
                    {
                        CookieHelper.SaveCookie("ValidatePhone", true);
                        CookieHelper.SaveCookie("CanceledPhoneValidate", false);


                        return Json("s", JsonRequestBehavior.AllowGet);
                    }
                    return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
                }
                return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
            }
            return Json("验证码不正确", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AjaxValidateCellphoneByCode2(FormCollection form)
        {
            string CellPhoneNumber = Request["CellPhoneNumber"].ToString();
            string SmsCode = Request["SmsCode"].ToString();
            //在验证手机验证码前对手机号码进行验证避免出现个人恶意攒一堆验证码绑定多个账户
            //判断手机号码是否被验证过
            if (CustomerFacade.PhoneIsValidate(CellPhoneNumber))
            {
                return Json("此手机号码已经被验证过,不能进行重复验证", JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(SmsCode) && !string.IsNullOrEmpty(CellPhoneNumber))
            {
                //送积分
                Point point = new Point();
                point.CustomerSysNo = CurrUser.UserSysNo;
                point.AvailablePoint = ConstValue.GetPointByValidatePhone;
                point.ExpireDate = DateTime.Now.AddYears(1);
                point.InDate = DateTime.Now;
                point.InUser = CurrUser.UserID;
                point.Memo = EnumHelper.GetDescription(PointType.MobileVerification);
                point.ObtainType = (int)PointType.MobileVerification;
                point.Points = ConstValue.GetPointByValidatePhone;
                point.IsFromSysAccount = 1;
                point.SysAccount = int.Parse(ConstValue.PointAccountSysNo);

                if (CustomerFacade.ValidateCustomerPhone(CellPhoneNumber, SmsCode.ToLower(), point))
                {
                    CookieHelper.SaveCookie("ValidatePhone", true);
                    CookieHelper.SaveCookie("CanceledPhoneValidate", false);
                    return Json("s", JsonRequestBehavior.AllowGet);
                }
                return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
            }
            return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证用户的邮箱地址
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxValidateCustomerEmail(FormCollection form)
        {
            string Email = Request["Email"].ToString();
            string ValidatedCode = Request["ValidatedCode"].ToString();
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == ValidatedCode.ToLower())
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    string imgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();//图片根目录
                    string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();

                    if (CustomerFacade.CheckEmail(Email))
                    {
                        return Json("此邮箱已经被验证过，请使用其它邮箱", JsonRequestBehavior.AllowGet);
                    }
                    if (CustomerFacade.SendEmailValidatorMail(CurrUser.UserID, Email, imgBaseUrl, domain))
                    {
                        CustomerInfo info = CustomerFacade.GetCustomerByID(CurrUser.UserID);
                        if (Email != info.Email)
                        {
                            CustomerFacade.UpdateCustomerEmailAddress(CurrUser.UserID, Email);
                        }
                        return Json("s", JsonRequestBehavior.AllowGet);
                    }
                    return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
                }
                return Json("短信校验码不正确或不存在", JsonRequestBehavior.AllowGet);
            }
            return Json("验证码不正确", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建商品到货通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCreateProductNotify(FormCollection form)
        {
            int productSysNo = Convert.ToInt32(Request["ProductSysNo"]);
            string email = HttpUtility.UrlDecode(Request["Email"].ToString());

            var temp = ProductNotifyFacade.GetProductNotify(email, productSysNo);
            if (temp != null)
            {
                return Json(new
                {
                    Result = false,
                    Message = "该邮箱已订阅此商品！"
                });
            }
            ProductNotifyInfo entity = new ProductNotifyInfo()
            {
                CustomerSysNo = CurrUser.UserSysNo,
                ProductSysNo = productSysNo,
                Email = email,
                Status = 0
            };
            ProductNotifyFacade.CreateProductNotify(entity);
            return Json(new
            {
                Result = true,
                Message = "订阅到货通知成功！"
            });
        }

        /// <summary>
        /// 继续提醒到货通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxUpdateProductNotify(FormCollection form)
        {
            string data = Request["SelectList"].ToString();
            string[] strList = data.Split(',');

            for (int i = 0; i < strList.Length; i++)
            {
                if (!string.IsNullOrEmpty(strList[i]))
                {
                    var sysNo = Convert.ToInt32(strList[i]);
                    ProductNotifyFacade.UpdateProductNotify(sysNo, CurrUser.UserSysNo);
                }
            }
            return Json("操作已成功，稍候生效！");
        }

        /// <summary>
        /// 删除商品到货通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxDeleteProductNotify(FormCollection form)
        {
            string data = Request["SelectList"].ToString();
            string[] strList = data.Split(',');

            for (int i = 0; i < strList.Length; i++)
            {
                if (!string.IsNullOrEmpty(strList[i]))
                {
                    var sysNo = Convert.ToInt32(strList[i]);
                    ProductNotifyFacade.DeleteProductNotify(sysNo, CurrUser.UserSysNo);
                }
            }
            return Json("操作已成功，稍候生效！");
        }

        /// <summary>
        ///清空商品到货通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxClearProductNotify(FormCollection form)
        {
            ProductNotifyFacade.ClearProductNotify(CurrUser.UserSysNo);
            return Json("操作已成功，稍候生效！");
        }


        /// <summary>
        /// 创建商品降价通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCreateProductPriceNotify(FormCollection form)
        {
            int productSysNo = Convert.ToInt32(Request["ProductSysNo"]);
            decimal expectedPrice = Convert.ToDecimal(Request["ExpectedPrice"]);
            decimal instantPrice = Convert.ToDecimal(Request["InstantPrice"]);
            bool isFavorite = Convert.ToBoolean(Request["IsFavorite"]);

            var temp = ProductPriceNotifyFacade.GetProductPriceNotify(CurrUser.UserSysNo, productSysNo);
            if (temp != null)
            {
                return Json(new
                {
                    Result = false,
                    Message = "您已经订阅了此商品的降价通知！"
                });
            }

            LoginUser suer = UserMgr.ReadUserInfo();
            CustomerInfo customerInfo = CustomerFacade.GetCustomerInfo(suer.UserSysNo);
            if (string.IsNullOrEmpty(customerInfo.Email))
            {
                return Json(new
                {
                    Result = false,
                    Message = "您的帐号未关联邮箱，请到帐户中心个人信息中设置邮箱！"
                });
            }

            if (isFavorite && !ProductFacade.IsProductWished(productSysNo, CurrUser.UserSysNo))
            {
                CustomerFacade.AddProductToWishList(CurrUser.UserSysNo, productSysNo);//加入收藏
            }

            ProductPriceNotifyInfo entity = new ProductPriceNotifyInfo()
            {
                CustomerSysNo = CurrUser.UserSysNo,
                ProductSysNo = productSysNo,
                ExpectedPrice = expectedPrice,
                InstantPrice = instantPrice
            };
            ProductPriceNotifyFacade.CreateProductPriceNotify(entity);
            return Json(new
            {
                Result = true,
                Message = "订阅降价通知成功！"
            });
        }

        /// <summary>
        /// 继续订阅降价通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxUpdateProductPriceNotify(FormCollection form)
        {
            int sysNo = Convert.ToInt32(Request["SysNo"].ToString());
            decimal expectedPrice = Convert.ToDecimal(Request["ExpectedPrice"]);
            decimal instantPrice = Convert.ToDecimal(Request["InstantPrice"]);

            ProductPriceNotifyInfo entity = new ProductPriceNotifyInfo()
            {
                SysNo = sysNo,
                CustomerSysNo = CurrUser.UserSysNo,
                ExpectedPrice = expectedPrice,
                InstantPrice = instantPrice
            };

            ProductPriceNotifyFacade.UpdateProductPriceNotify(entity);
            return Json("操作已成功，稍候生效！");
        }

        /// <summary>
        /// 取消订阅降价通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCancelProductPriceNotify(FormCollection form)
        {
            int sysNo = Convert.ToInt32(Request["SysNo"].ToString());
            ProductPriceNotifyFacade.CancelProductPriceNotify(sysNo, CurrUser.UserSysNo);
            return Json("操作已成功，稍候生效！");
        }

        /// <summary>
        /// 删除商品降价通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxDeleteProductPriceNotify(FormCollection form)
        {
            string data = Request["SelectList"].ToString();
            string[] strList = data.Split(',');

            for (int i = 0; i < strList.Length; i++)
            {
                if (!string.IsNullOrEmpty(strList[i]))
                {
                    var sysNo = Convert.ToInt32(strList[i]);
                    ProductPriceNotifyFacade.DeleteProductPriceNotify(sysNo, CurrUser.UserSysNo);
                }
            }
            return Json("操作已成功，稍候生效！");
        }

        /// <summary>
        ///清空商品降价通知
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxClearProductPriceNotify(FormCollection form)
        {
            ProductPriceNotifyFacade.ClearProductPriceNotify(CurrUser.UserSysNo);
            return Json("操作已成功，稍候生效！");
        }

        public ActionResult AjaxCancelPhoneValidate()
        {
            var customer = CustomerFacade.GetCustomerInfo(CurrUser.UserSysNo);
            if (customer != null)
            {
                CustomerFacade.CancelCustomerPhone(customer.CellPhone, customer.SysNo);
            }
            else
            {
                return Json("此用户不存在", JsonRequestBehavior.AllowGet);
            }
            CookieHelper.SaveCookie("CanceledPhoneValidate", true);
            CookieHelper.SaveCookie("ValidatePhone", false);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 我的优惠券
        /// </summary>
        /// <returns></returns>
        public ActionResult Coupon()
        {
            int pageIndex = 0;
            if (int.TryParse(Request.Params["page"], out pageIndex))
                pageIndex--;

            CustomerCouponCodeQueryInfo query = new CustomerCouponCodeQueryInfo();
            query.PageInfo.PageIndex = pageIndex;
            query.PageInfo.PageSize = 10;
            query.CustomerSysNo = this.CurrUser.UserSysNo;
            query.Status = "A";

            var result = CustomerFacade.QueryCouponCode(query);
            result.PageInfo.PageIndex++;
            return View(result);
        }

        /// <summary>
        /// 获取账户中心 我的评论
        /// </summary>
        /// <returns></returns>
        public ActionResult MyReview()
        {
            return View();
        }

        /// <summary>
        /// 获取账户中心我的咨询
        /// </summary>
        /// <returns></returns>
        public ActionResult MyConsult()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UCNoReviewOrderProducts(int pageIndex, int pageSize)
        {
            LoginUser suer = UserMgr.ReadUserInfo();
            var noReviewOrders = ReviewFacade.QueryCustomerNoReviewOrderProducts(suer.UserSysNo, pageIndex, pageSize);
            return PartialView("_NoReviewOrderProducts", noReviewOrders);
        }

        public ActionResult CustomerAuthenticationInfo()
        {
            return View();
        }
    }
}
