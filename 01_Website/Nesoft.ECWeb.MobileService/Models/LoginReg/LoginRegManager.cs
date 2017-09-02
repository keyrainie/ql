using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Member;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.WebFramework;
using Nesoft.Utility;

namespace Nesoft.ECWeb.MobileService.Models.LoginReg
{
    public class LoginRegManager
    {

        public static CustomerInfoViewModel CustomerLogin(LoginViewModel request)
        {
            if (string.IsNullOrEmpty(request.CustomerID))
            {
                throw new BusinessException("登录账号不能为空!");
            }
            else if (string.IsNullOrEmpty(request.Password))
            {
                throw new BusinessException("登录密码不能为空!");
            }
            string newPassword = string.Empty;
            //string passwordSalt = string.Empty;
            //passwordSalt = LoginFacade.GetCustomerPasswordSalt(request.CustomerID);
            //request.Password = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(request.Password.Replace("+", "%2b")) + passwordSalt);
            // [2014/12/22 by Swika]增加支持第三方系统导入的账号的密码验证
            var encryptMeta = LoginFacade.GetCustomerEncryptMeta(request.CustomerID);
            try
            {
                request.Password = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(request.Password.Replace("+", "%2b")), encryptMeta);

                var loginResult = LoginFacade.CustomerLogin(request.CustomerID, request.Password);
                if (null != loginResult)
                {
                    CustomerInfoViewModel user = EntityConverter<CustomerInfo, CustomerInfoViewModel>.Convert(CustomerFacade.GetCustomerInfo(loginResult.SysNo), (s, t) =>
                    {
                        t.RegisterTimeString = s.RegisterTime.ToString("yyyy年MM月dd日 HH:mm:ss");
                        t.AvtarImage = s.ExtendInfo.AvtarImage;
                        t.AvtarImageDBStatus = s.ExtendInfo.AvtarImageDBStatus;
                    });
                    if (user != null)
                    {
                        LoginFacade.UpdateLastLoginTime(user.SysNo);
                        LoginUser lUser = new LoginUser();
                        lUser.UserDisplayName = user.CustomerName;
                        lUser.UserID = user.CustomerID;
                        lUser.UserSysNo = user.SysNo;
                        lUser.RememberLogin = true;
                        lUser.LoginDateText = DateTime.Now.ToString();
                        lUser.TimeoutText = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["LoginTimeout"].ToString())).ToString();
                        CookieHelper.SaveCookie<LoginUser>("LoginCookie", lUser);
                    }
                    System.Threading.Thread.Sleep(1000);
                    return user;
                }
                else
                {
                    throw new BusinessException("登录失败，用户名或者密码错误!");
                }
            }
            catch
            {
                throw new BusinessException("登录失败，用户名或者密码错误!");
            }

        }

        public static CustomerInfoViewModel CustomerRegister(CustomerRegisterRequestViewModel request)
        {
            if (string.IsNullOrEmpty(request.CustomerID))
            {
                throw new BusinessException("登录账号不能为空!");
            }
            else if (string.IsNullOrEmpty(request.Password))
            {
                throw new BusinessException("登录密码不能为空!");
            }
            else if (string.IsNullOrEmpty(request.RePassword))
            {
                throw new BusinessException("确认登录密码不能为空!");
            }
            else if (request.Password != request.RePassword)
            {
                throw new BusinessException("密码与确认密码不相同!");
            }
            else if (string.IsNullOrEmpty(request.CellPhone))
            {
                throw new BusinessException("手机号码不能为空!");
            }
            //else if (string.IsNullOrEmpty(request.Email))
            //{
            //    throw new BusinessException("电子邮箱不能为空!");
            //}
            string errorMsg;
            if (!CustomerManager.CheckPasswordFormat(request.Password, out errorMsg))
            {
                throw new BusinessException(errorMsg);
            }
            request.Password = HttpUtility.UrlDecode(request.Password.Replace("+", "%2b"));
            request.FromLinkSource = HeaderHelper.GetClientType().ToString();
            CustomerInfo item = EntityConverter<CustomerRegisterRequestViewModel, CustomerInfo>.Convert(request);
            var getExistsCustomerInfo = CustomerFacade.GetCustomerByID(item.CustomerID);
            if (null != getExistsCustomerInfo && getExistsCustomerInfo.SysNo > 0)
            {
                throw new BusinessException(string.Format("注册失败,用户名{0}已经被注册!", item.CustomerID));
            }
            item.InitRank = 1;
            item.CustomerName = item.CustomerID;
            //密码处理
            string encryptPassword = string.Empty;
            string password = item.Password;
            string passwordSalt = string.Empty;

            PasswordHelper.GetNewPasswordAndSalt(ref password, ref encryptPassword, ref passwordSalt);
            item.Password = encryptPassword;
            item.PasswordSalt = passwordSalt;
            item.CustomersType = 1;
            CustomerInfo regCustomer = LoginFacade.CreateCustomer(item);
            if (regCustomer.SysNo > 0)
            {
                LoginUser lUser = new LoginUser();
                lUser.UserDisplayName = item.CustomerName;
                lUser.UserID = item.CustomerID;
                lUser.UserSysNo = item.SysNo;
                lUser.RememberLogin = true;
                lUser.LoginDateText = DateTime.Now.ToString();
                lUser.TimeoutText = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["LoginTimeout"].ToString())).ToString();
                CookieHelper.SaveCookie<LoginUser>("LoginCookie", lUser);

                item.SysNo = regCustomer.SysNo;
                item.RegisterTime = DateTime.Now;
                return EntityConverter<CustomerInfo, CustomerInfoViewModel>.Convert(item, (s, t) =>
                {
                    t.RegisterTimeString = s.RegisterTime.ToString("yyyy年MM月dd日 HH:mm:ss");
                });
            }
            else
            {
                throw new BusinessException("用户注册失败，请稍后重试!");
            }
        }
    }
}