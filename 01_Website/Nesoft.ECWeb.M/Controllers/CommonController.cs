using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.Facade.Member.Models;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.M;
using Nesoft.ECWeb.M.App_Code;
using Nesoft.ECWeb.M.Models.Search;
using Nesoft.ECWeb.WebFramework;
using Nesoft.Utility;

namespace Nesoft.ECWeb.M.Controllers
{
    public class CommonController : WWWControllerBase
    {
        //
        // GET: /Common/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Food()
        {
            return View();
        }
        public ActionResult Ajax_QueryFood()
        {
            var pageIndex = int.Parse(Request["page"]);
            
            var pageSize = int.Parse(Request["size"]);

            var result = new AjaxResult { Success = true };

            SearchCriteriaModel filter = new SearchCriteriaModel();
            filter.Category1ID = int.Parse(ConfigurationManager.AppSettings["ECCategoryID"]);

            NameValueCollection pageInfo = new NameValueCollection();
            pageInfo.Add("pageIndex", pageIndex.ToString());
            pageInfo.Add("pageSize", pageSize.ToString());
            ProductSearchResultVM data=SearchManager.Search(filter, pageInfo);
            data.ProductList.CurrentPageData.ForEach(p => {
                p.ProductDefaultImage = ProductFacade.BuildProductImage(ImageSize.P240, p.ProductDefaultImage);
            });
            result.Data = new { 
                List=data.ProductList.CurrentPageData,
                PageCount=data.ProductList.TotalPages
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Hot()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AjaxLogin()
        {
            var model = new LoginVM();
            this.TryUpdateModel<LoginVM>(model);
            var result = new AjaxResult
            {
                Success = false
            };
            if (UserManager.Login(model))
            {
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            result.Message = "登录账户名或密码不正确";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxRegister()
        {
            var model = new RegisterVM();
            this.TryUpdateModel<RegisterVM>(model);
            var result = new AjaxResult
            {
                Success = false
            };
            //判断此CustomerID是否被注册过
            if (LoginFacade.IsExistCustomer(model.CustomerID))
            {
                result.Message = "该账户名已经被注册";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (model.Password != model.RePassword)
            {
                result.Message = "密码与确认密码不相同";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            model.Password = HttpUtility.UrlDecode(model.Password.Replace("+", "%2b"));

            CustomerInfo item = EntityConverter<RegisterVM, CustomerInfo>.Convert(model);
            item.InitRank = 1;
            item.CustomerName = item.CustomerID;

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
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            result.Message = "用户注册失败，请稍后重试";
            return Json(result, JsonRequestBehavior.AllowGet);
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
    }
}
