using System.Configuration;
using ECommerce.Enums;
using ECommerce.Facade.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Facade.Keyword;
using ECommerce.WebFramework.Mail;
using ECommerce.WebFramework;
using System.Drawing;
using ECommerce.Utility;
using ECommerce.Entity.Member;
using ECommerce.Facade.Member.Models;
using ECommerce.Facade.Member;
using ECommerce.Facade.Product;
using System.Collections;
using ECommerce.Facade.Recommend;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Facade;
using ECommerce.Entity.Store;
using ECommerce.Facade.Store;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Facade.Shopping;
using ECommerce.Entity.Promotion;
using System.Web.Script.Serialization;

namespace ECommerce.UI.Controllers
{
    public class HomeController : WWWControllerBase
    {
        #region ERROR相关
        public ActionResult Error()
        {
            Response.StatusCode = 503;
            return View();
        }

        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult AuthError()
        {
            Response.StatusCode = 403;
            return View();
        }
        #endregion

        //
        // GET: /Web/Home/
        public ActionResult Logout()
        {
            UserMgr.Logout();
            string url = PageHelper.BuildUrl("Web_Index");
            return Redirect(url);
        }

        public ActionResult Index()
        {
            var routeValues = RouteData.Values;
            if (routeValues.Keys != null && routeValues.Keys.Count > 0)
            {
                if (routeValues.Keys.ToList<string>().Exists(f => f.ToLower() == "subdomain"))
                {
                    string subdomain = routeValues["subdomain"].ToString();
                    if (string.IsNullOrWhiteSpace(subdomain))
                    {
                        return View();
                    }

                    var forbiddenSD = AppSettingManager.GetSetting("Store", "ForbiddenSecondDomain").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (forbiddenSD.ToList<string>().Exists(f => f.Trim().ToLower() == subdomain.Trim().ToLower()))
                    {
                        //该二级域名为平台专用，不可申请，所以将直接转到首页!
                        return View();
                    }

                    StoreDomainPage storePage = StoreFacade.GetStoreIndexPageBySubDomain(subdomain.Trim());
                    if (storePage != null)
                    {
                        return new StoreController().Index(storePage.SellerSysNo, storePage.HomePageSysNo, false);
                    }

                }
            }


            return View();
        }

        public ActionResult EmailVerifySucceed()
        {
            if (Request["sysno"] != null && Request["email"] != null)
            {
                if (CustomerFacade.CheckCustomerEmail(int.Parse(Request["sysno"].ToString()), HttpUtility.HtmlDecode(Request["email"].ToString())))
                {
                    //送积分
                    ECommerce.Entity.Common.Point point = new ECommerce.Entity.Common.Point();

                    var user = CustomerFacade.GetCustomerByEmail(Request["email"]);
                    if (user == null)
                    {
                        ViewBag.Message = "没有找到该邮箱的对应用户";
                    }
                    else
                    {
                        point.CustomerSysNo = user.SysNo;
                        point.AvailablePoint = ConstValue.GetPointByValidateEmail;
                        point.ExpireDate = DateTime.Now.AddYears(1);
                        point.InDate = DateTime.Now;
                        point.InUser = user.CustomerID;
                        point.Memo = EnumHelper.GetDescription(PointType.EmailVerification);
                        point.ObtainType = (int)PointType.EmailVerification;
                        point.Points = ConstValue.GetPointByValidateEmail;
                        point.IsFromSysAccount = 1;
                        point.SysAccount = int.Parse(ConstValue.PointAccountSysNo);

                        CustomerFacade.CustomerEmailValidated(int.Parse(Request["sysno"].ToString()), point);
                        ViewBag.Message = "s";
                    }
                }
                else
                {
                    ViewBag.Message = "用户邮箱地址没有通过验证！";
                }
            }
            else
            {
                ViewBag.Message = "没有需要验证的用户邮箱地址！";
            }
            return View();
        }

        #region  验证码

        public FileContentResult ImageValidator()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            VerifyImage v = new VerifyImage();
            string code = v.CreateVerifyCode();
            CookieHelper.SaveCookie<String>("VerifyCode", code);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Bitmap image = v.CreateImageCode(code);
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                code = CryptoManager.Encrypt(code.ToUpper());
                byte[] b = ms.GetBuffer();
                return File(b, @"image/jpg");
            }
        }
        #endregion

        #region 帮助方法
        // GetNewPasswordAndSalt(ref password, ref encryptPassword, ref passwordSalt);
        //public void GetNewPasswordAndSalt(ref string newPassword, ref string encryptPassword, ref string passwordSalt)
        //{
        //    passwordSalt = Guid.NewGuid().ToString("N");
        //    //EncryptType encryptionStatus = EncryptType.Off;
        //    encryptPassword = PasswordHelper.GetEncryptedPassword(newPassword.Trim() + passwordSalt);
        //    newPassword = string.Empty;
        //}

        /// <summary>
        /// 密码策略检查
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPasswordPolicy(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            password = password.Trim();
            if (password.Length < 6 || password.Length > 20)
                return false;

            int result = 0;
            bool noDigit = true, noLetter = true, noOther = true;
            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    if (noDigit)
                    {
                        noDigit = false;
                        result++;
                    }
                }
                else if (char.IsLetter(c))
                {
                    if (noLetter)
                    {
                        noLetter = false;
                        result++;
                    }
                }
                else if (noOther)
                {
                    noOther = false;
                    result++;
                }

                if (result > 1)
                    return true;
            }
            return false;
        }

        #endregion

        #region AJAX提交

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxLogin()
        {
            var model = new LoginVM();
            this.TryUpdateModel<LoginVM>(model);
            //判断是否需要输入验证码
            bool showAuthCode = LoginFacade.CheckShowAuthCode(model.CustomerID);
            if (showAuthCode)
            {
                string verifyCode = CookieHelper.GetCookie<String>("VerifyCode");
                if (!String.Equals(verifyCode, model.ValidatedCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new { type = "f", verifycode = "y", message = "验证码不正确" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (UserMgr.Login(model))
                return Json(new { type = "s" }, JsonRequestBehavior.AllowGet);
            if (showAuthCode)
            {
                return Json(new { type = "f", verifycode = "y", message = "登录账户名或密码不正确" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { type = "f", message = "登录账户名或密码不正确" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxCheckShowAuthCode(string customerID)
        {
            bool verifycode = false;
            verifycode = LoginFacade.CheckShowAuthCode(customerID);
            return Json(new { verifycode = verifycode });
        }

        /// <summary>
        /// 检查该用户是否已经注册过
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCheckRegisterID(string CustomerID)
        {
            if (LoginFacade.IsExistCustomer(CustomerID))
                return Json("该账户名已经被注册", JsonRequestBehavior.AllowGet);
            return Json("s", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxCheckRegisterEmail(string CustomerEmail)
        {
            if (LoginFacade.IsCustomerEmailExist(CustomerEmail))
            {
                return Json("该邮箱已经被注册", JsonRequestBehavior.AllowGet);
            }
            return Json("s", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult AjaxCheckRegisterPhoneNumber(string CellPhoneNumber)
        {
            if (LoginFacade.IsCustomerPhoneExist_Confirm(CellPhoneNumber))
            {
                return Json("该手机号码已被绑定", JsonRequestBehavior.AllowGet);
            }
            return Json("s", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AjaxCheckRegisterimgNumber(string ImgValidatedCode)
        {
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == ImgValidatedCode.ToLower())
            {
                return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResult() { ContentType = "y", Data = "验证码不正确" }, JsonRequestBehavior.AllowGet);
        } 
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxRegister()
        {
            var model = new RegisterVM();
            this.TryUpdateModel<RegisterVM>(model);
            //if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == model.ValidatedCode.ToLower())
            //{
            if (model.Password != model.RePassword)
                return Json("密码与确认密码不相同", JsonRequestBehavior.AllowGet);

            model.Password = HttpUtility.UrlDecode(model.Password.Replace("+", "%2b"));

            CustomerInfo item = EntityConverter<RegisterVM, CustomerInfo>.Convert(model);
            item.InitRank = 1;
            item.CustomerName = item.CustomerID;
            //if (!CheckPasswordPolicy(item.Password))
            //    return Json("密码格式不正确，密码必须是字母与数据的组合", JsonRequestBehavior.AllowGet);

            if (LoginFacade.IsExistCustomer(item.CustomerID))
                return Json(new JsonResult() { ContentType = "f", Data = "该账户名已经被注册" }, JsonRequestBehavior.AllowGet);
            if (LoginFacade.IsCustomerEmailExist(item.Email))
                return Json(new JsonResult() { ContentType = "f", Data = "该邮箱已经被注册" }, JsonRequestBehavior.AllowGet);

            //密码处理
            string encryptPassword = string.Empty;
            string password = item.Password;
            string passwordSalt = string.Empty;

            PasswordHelper.GetNewPasswordAndSalt(ref password, ref encryptPassword, ref passwordSalt);
            item.Password = encryptPassword;
            item.PasswordSalt = passwordSalt;

            if (LoginFacade.CreateCustomer(item).SysNo > 0)
            {
                LoginUser lUser = new LoginUser();
                lUser.UserDisplayName = item.CustomerName;
                lUser.UserID = item.CustomerID;
                lUser.UserSysNo = item.SysNo;
                lUser.RememberLogin = false;
                lUser.LoginDateText = DateTime.Now.ToString();                
                lUser.TimeoutText = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["LoginTimeout"].ToString())).ToString();

                CookieHelper.SaveCookie<LoginUser>("LoginCookie", lUser);

                //更新数据

                bool result= CustomerFacade.UpdateCellPhoneCustomerSysNoByID(int.Parse(model.CellPhoneCode),lUser.UserSysNo);
                return Json(new JsonResult() { ContentType = "s", Data = lUser.UserSysNo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResult() { ContentType = "f", Data = "用户注册失败，请稍后重试" }, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new JsonResult() { ContentType = "y", Data = "验证码不正确" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过邮箱地址找回
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCheckFindPasswordForCustomer(FormCollection form)
        {
            string validatedCode = Request["ValidatedCode"].ToString();
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == validatedCode.ToLower())
            {
                string customerStr = Request["CustomerID"].ToString();
                if (LoginFacade.IsExistCustomer(customerStr))//存在该用户名
                {
                    CustomerInfo customer = CustomerFacade.GetCustomerByID(customerStr);
                    if (string.IsNullOrEmpty(customer.Email))
                        return Json("该用户没有绑定邮箱地址", JsonRequestBehavior.AllowGet);

                    //邮箱是否被验证
                    if (customer.IsEmailConfirmed != 1)
                    {
                        return Json("对不起,您的邮箱还没有通过验证,请使用其他方式找回密码!", JsonRequestBehavior.AllowGet);
                    }
                    CookieHelper.SaveCookie<string>("FindPasswordCustomerID", customerStr);
                    //string customerid = CookieHelper.GetCookie<String>("FindPasswordCustomerID");
                    string imgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();//图片根目录
                    string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();
                    LoginFacade.SendFindPasswordMail(customerStr, imgBaseUrl, domain);
                    string email = customer.Email;
                    int x = email.IndexOf("@");
                    string account = email.Substring(0, x);
                    if (account.Length > 1)
                        account = account.Substring(1, account.Length - 1);
                    email = email.Replace(account, "******");
                    return Json(email, JsonRequestBehavior.AllowGet);
                }
                return Json("不存在该用户", JsonRequestBehavior.AllowGet);
            }
            return Json("验证码不正确", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得用户手机号码
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxFindPasswordByPhone(FormCollection form)
        {
            string validatedCode = Request["ValidatedCode"].ToString();
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == validatedCode.ToLower())
            {
                string customerStr = Request["CustomerID"].ToString();
                if (LoginFacade.IsExistCustomer(customerStr))//存在该用户名
                {
                    CustomerInfo customer = CustomerFacade.GetCustomerByID(customerStr);
                    if (string.IsNullOrEmpty(customer.CellPhone))
                        return Json("不存在该用户的手机号码", JsonRequestBehavior.AllowGet);
                    if (!CustomerFacade.CheckCustomerPhoneValided(customer.SysNo))
                        return Json("用户手机密码没有通过验证", JsonRequestBehavior.AllowGet);

                    CookieHelper.SaveCookie<string>("FindPasswordCustomerID", customerStr);
                    CookieHelper.SaveCookie<string>("FindPasswordCustomerCellPhone", customer.CellPhone);
                    CookieHelper.SaveCookie<string>("FindPasswordCustomerSysNo", customer.SysNo.ToString());
                    return Json(customer.CellPhone, JsonRequestBehavior.AllowGet);
                }
                return Json("不存在该用户", JsonRequestBehavior.AllowGet);
            }
            return Json("验证码不正确", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过手机短信找回
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendFindPasswordSMS(FormCollection form)
        {
            string cell = CookieHelper.GetCookie<String>("FindPasswordCustomerCellPhone");
            if (!string.IsNullOrEmpty(cell))
            {
                SMSInfo item = new SMSInfo();
                //item.CustomerSysNo = int.Parse(CookieHelper.GetCookie<String>("FindPasswordCustomerSysNo"));
                item.CreateUserSysNo = int.Parse(CookieHelper.GetCookie<String>("FindPasswordCustomerSysNo"));
                item.CellNumber = cell;
                item.Status = SMSStatus.NoSend;
                item.Type = SMSType.FindPassword;
                item.Priority = 100;
                item.RetryCount = 0;

                string code = VerifyImage.CreateRandomNumber();

                CookieHelper.SaveCookie<string>("FindPasswordSMSCode", code);
                item.SMSContent = string.Format(AppSettingManager.GetSetting("SMSTemplate", "AlertConfirmPhoneCode"),
                    DateTime.Now.ToString("MM月dd日 HH:mm"), code);
                if (CommonFacade.InsertNewSMS(item))
                    return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendLoginValidSMS(FormCollection form)
        {
            //step one :get customer cellphone from db
            string customerID = form["CustomerID"];
            if (string.IsNullOrEmpty(customerID))
            {
                return Json("账户名未提供，发送验证码失败。", JsonRequestBehavior.AllowGet);
            }
            var customer = CustomerFacade.GetCustomerByID(customerID);
            string customerCellphone = customer.CellPhone;

            //step two :send sms
            if (string.IsNullOrEmpty(customerCellphone) || customer.IsPhoneValided == 0)
            {
                return Json("账户未绑定手机或是未完成验证绑定，获取验证码失败。", JsonRequestBehavior.AllowGet);
            }
            else
            {
                SMSInfo item = new SMSInfo();
                item.CreateUserSysNo = customer.SysNo;
                item.CellNumber = customer.CellPhone;
                item.Status = SMSStatus.NoSend;
                item.Type = SMSType.VerifyPhone;
                item.Priority = 100;
                item.RetryCount = 0;

                string code = VerifyImage.CreateRandomNumber();
                CookieHelper.SaveCookie<string>("VerifyCode", code);
                item.SMSContent = string.Format(AppSettingManager.GetSetting("SMSTemplate", "CreateConfirmPhoneCode"),
                    DateTime.Now.ToString("MM月dd日 HH:mm"), code);
                if (CommonFacade.InsertNewSMS(item))
                    return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过手机短信找回
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxCheckFindPasswordBySMSCode(FormCollection form)
        {
            string ValidatedCode = Request["ValidatedCode"].ToString();
            if (CookieHelper.GetCookie<String>("FindPasswordSMSCode").ToLower() == ValidatedCode.ToLower())
            {
                CookieHelper.SaveCookie<string>("FindPasswordSMSCodeRight", "FindPasswordSMSCodeRight");
                return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("验证码不正确", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxResetPassword(FormCollection form)
        {
            string Password = Request["Password"].ToString();
            string customerID = CookieHelper.GetCookie<String>("FindPasswordCustomerID");
            if (!string.IsNullOrEmpty(customerID))
            {
                string encryptPassword = string.Empty;
                string passwordSalt = string.Empty;
                PasswordHelper.GetNewPasswordAndSalt(ref Password, ref encryptPassword, ref passwordSalt);
                //重置密码
                CustomerFacade.UpdateCustomerPassword(customerID, encryptPassword, passwordSalt);

                return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("没找到可重置密码的用户", JsonRequestBehavior.AllowGet);
        }

        #endregion



        /// <summary>
        /// 检查该用户是否存在
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerInfoBySysInfo(string userinfo)
        {
            CustomerInfo customer;
            customer = CustomerFacade.GetCustomerByEmail(userinfo);
            if (customer == null)
            {
                customer = CustomerFacade.GetCustomerByPhone(userinfo);
                if (customer == null)
                {
                    customer = CustomerFacade.GetCustomerByID(userinfo);
                }
            }

            if (customer == null)
                return Json("该用户不存在", JsonRequestBehavior.AllowGet);
            return Json("s", JsonRequestBehavior.AllowGet);
        }


        public ActionResult ClearCache()
        {
            //string cacheKey = "TestCache";
            //string list = "AAAAA1111233444";             
            //HttpRuntime.Cache.Insert(cacheKey, list, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);
            //string s = (string)HttpRuntime.Cache[cacheKey];


            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator.Key.ToString().Contains("AppStartPage"))
                {

                    keys.Add(enumerator.Key.ToString());
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                HttpRuntime.Cache.Remove(keys[i]);
            }



            //object obj = HttpRuntime.Cache[cacheKey];
            //if (obj != null)
            //{
            //    string ss = obj.ToString();
            //}


            return View();
        }

        [HttpPost]
        [ActionName("CreateCouponCode")]
        public JsonResult AjaxCreateCouponCode(string customerID,string couponSysNo )
        {
            string couponcode;
            //couponInfoStr格式为“优惠券活动编码|优惠券活动开始时间|优惠券活动结束时间”
            string result = ShoppingFacade.UserGetCouponCode(int.Parse(customerID), int.Parse(couponSysNo),out couponcode);
            return Json(new JsonResult { ContentType = result, Data = couponcode}, JsonRequestBehavior.AllowGet);
        }


        #region 获取城市三级

        [HttpPost]
        public ActionResult GetAllProvince()
        {
            var data = CommonFacade.GetAllProvince();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllCity(int proviceSysNo)
        {
            var data = CommonFacade.GetAllCity(proviceSysNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllDistrict(int citySysNo)
        {
            var data = CommonFacade.GetAllDistrict(citySysNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 是否登录
        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns>大于0则已登录</returns>
        public JsonResult CheckLogin()
        {
            if (this.CurrUser != null && this.CurrUser.UserSysNo > 0)
            {
                return new JsonResult() { Data = this.CurrUser.UserSysNo };
            }
            else
            {
                return new JsonResult() { Data = 0 };
            }
        }
        #endregion

        public JsonResult GetDefaultSearchKeyword()
        {
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = KeywordFacade.GetDefaultSearchKeyword(Convert.ToInt32(Request["PageType"]), Convert.ToInt32(Request["PageID"]));
            return result;
        }

        public JsonResult SearchAutoComplete()
        {
            string keywords = Request.Params["k"];
            return new JsonResult() { Data = ProductSearchFacade.GetProductACSearchResultBySolr(keywords) };
        }

        public ActionResult WebsiteMap()
        {
            return View();
        }

        public ActionResult GetCouponPopContent(int MerchantSysNo)
        {
            CouponContentInfo Model = new CouponContentInfo();
            //优惠卷
            LoginUser user = UserMgr.ReadUserInfo();
            List<CustomerCouponInfo> CustomerCouponList = new List<CustomerCouponInfo>();
            if (user != null)
            {
                CustomerCouponList = ShoppingFacade.GetCustomerPlatformCouponCode(user.UserSysNo, MerchantSysNo);
            }
            //获取当前有效的优惠券活动
            List<CouponInfo> CouponList = new List<CouponInfo>();
            if (user != null)
            {
                CouponList = ShoppingFacade.GetCouponList(user.UserSysNo, MerchantSysNo);
            }
            if (user != null)
            {
                Model.UserSysNo = user.UserSysNo;
                Model.MerchantSysNo = MerchantSysNo;
                Model.customerCouponCodeList = CustomerCouponList;
                Model.couponList = CouponList;
            }
            PartialViewResult view = PartialView("~/Views/Shared/_CouponPop.cshtml", Model);
            return view;
        }

        /// <summary>
        /// 商家注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxSellerRegister()
        {
            var vendorBasicInfo = new VendorBasicInfo();
            this.TryUpdateModel<VendorBasicInfo>(vendorBasicInfo);
            if (CookieHelper.GetCookie<String>("VerifyCode").ToLower() == vendorBasicInfo.ValidatedCode.ToLower())
            {
                if (string.IsNullOrEmpty(vendorBasicInfo.VendorName))
                {
                    return Json(new JsonResult() { ContentType = "f", Data = "商家名称不能为空" }, JsonRequestBehavior.AllowGet);
                }
                if (vendorBasicInfo.VendorName.Trim().Length > 100)
                {
                    return Json(new JsonResult() { ContentType = "f", Data = "商家名称长度不能超过100" }, JsonRequestBehavior.AllowGet);
                }
                if (vendorBasicInfo.EnglishName.Trim().Length > 100)
                {
                    return Json(new JsonResult() { ContentType = "f", Data = "商家英文名称长度不能超过100" }, JsonRequestBehavior.AllowGet);
                }
                if (StoreFacade.CheckExistsVendor(vendorBasicInfo.VendorName))
                {
                    return Json(new JsonResult() { ContentType = "f", Data = "该商家名称已存在,如果您已经申请，请耐心等待，我们将尽快与您联系" }, JsonRequestBehavior.AllowGet);
                }
                StoreFacade.CreateVendor(vendorBasicInfo);
                return Json(new JsonResult() { ContentType = "s", Data = "申请成功，请耐心等待，我们将尽快与您联系" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new JsonResult() { ContentType = "y", Data = "验证码不正确" }, JsonRequestBehavior.AllowGet);
        }

        #region APP

        public ActionResult Who()
        {
            return View();
        }

        #endregion

        #region
        public void aliPayRefound()
        {
            //RefoundContent = RefoundContent.Replace('-', '/');
            //RefoundContent = RefoundContent.Replace('_', '+');
            //RefoundContent = RefoundContent.Replace('.', '=');
            //byte[] outputb = Convert.FromBase64String(RefoundContent);
            //string orgStr = Encoding.UTF8.GetString(outputb);
            //Response.Write(UnZip(orgStr)); 
            //ASP.NET端获取到Silverlight传输过来的Cookie值      
            HttpCookie cookie = Request.Cookies["RefoundFormKey"];
            if (cookie.Value != null)
            {
                Response.Write(cookie.Value.ToString());
            }

        }
        #endregion

    }
}
