using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ECommerce.WebFramework;
using ECommerce.Entity;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Facade;
using ECommerce.Facade.Member.Models;
using ECommerce.Entity.Member;
using ECommerce.Facade.Member;
using System.Configuration;

namespace ECommerce.UI
{
    public static class UserMgr
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public static bool Login(LoginVM vm)//string userName, string pwd, string verifyCode)
        {
            //EncryptType encryptionStatus = EncryptType.Encryption;
            string newPassword = string.Empty;
            //string passwordSalt = string.Empty;


            //passwordSalt = LoginFacade.GetCustomerPasswordSalt(vm.CustomerID);
            //vm.Password = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(vm.Password.Replace("+", "%2b")) + passwordSalt);
            // [2014/12/22 by Swika]增加支持第三方系统导入的账号的密码验证

            CustomerInfo user = null;
            //if (vm.SourceType == CustomerSourceType.TLYH)
            //{
            //    user = LoginFacade.TLYHCustomerLogin(vm.CustomerID, vm.Password);
            //    CookieHelper.SaveCookie<int>("BankAccountPoint", user.BankAccountPoint);//存储网银用户积分到cookie
            //}
            //else
            //{
            var encryptMeta = LoginFacade.GetCustomerEncryptMeta(vm.CustomerID);
            if (encryptMeta == null)
            {
                return false;
            }
            vm.Password = PasswordHelper.GetEncryptedPassword(HttpUtility.UrlDecode(vm.Password.Replace("+", "%2b")), encryptMeta);
            user = LoginFacade.CustomerLogin(vm.CustomerID, vm.Password);
            //}

            if (user != null)
            {
                LoginFacade.UpdateLastLoginTime(user.SysNo);
                LoginUser lUser = new LoginUser();
                lUser.UserDisplayName = user.CustomerName;
                lUser.UserID = user.CustomerID;
                lUser.UserSysNo = user.SysNo;
                lUser.CustomerRank = user.CustomerRank;
                lUser.RememberLogin = vm.RememberLogin;
                lUser.LoginDateText = DateTime.Now.ToString();
                lUser.TimeoutText = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["LoginTimeout"].ToString())).ToString();
                WriteUserInfo(lUser);
                LoginFacade.ClearCustomerLoginFailedCount(vm.CustomerID);
                return true;
            }
            LoginFacade.IncrementCustomerLoginFailedCount(vm.CustomerID);
            return false;
        }

        /// <summary>
        /// 读取用户登录信息
        /// </summary>
        /// <returns></returns>
        public static LoginUser ReadUserInfo()
        {
            return CookieHelper.GetCookie<LoginUser>("LoginCookie");
        }

        /// <summary>
        /// 写用户登录信息
        /// </summary>
        /// <param name="userSysNo">用户编号</param>
        /// <param name="userID">用户名</param>
        /// <param name="userDisplayName">用户显示名</param>
        public static void WriteUserInfo(LoginUser user)
        {
            CookieHelper.SaveCookie<LoginUser>("LoginCookie", user);
        }



        /// <summary>
        /// 退出登录
        /// </summary>
        public static void Logout()
        {
            CookieHelper.SaveCookie<LoginUser>("LoginCookie", null);
            CookieHelper.SaveCookie<string>("FindPasswordCustomerID", null);
            CookieHelper.SaveCookie<string>("FindPasswordCustomerCellPhone", null);
            CookieHelper.SaveCookie<string>("FindPasswordCustomerSysNo", null);
            CookieHelper.SaveCookie<string>("FindPasswordSMSCode", null);
            CookieHelper.SaveCookie<string>("FindPasswordSMSCodeRight", null);
            CookieHelper.SaveCookie<bool?>("ValidatePhone", null);
            CookieHelper.SaveCookie<bool?>("CanceledPhoneValidate", null);
            CookieHelper.RemoveCookie("MyShoppingCartKey");
            CookieHelper.RemoveCookie("MyShoppingCartMini");
            CookieHelper.RemoveCookie("MyShoppingCart");
        }

        public static bool HasLogin()
        {
            var userAuth = UserMgr.ReadUserInfo();
            if (userAuth == null)
            {
                return false;
            }
            int userSysNo = userAuth.UserSysNo;
            string userID = userAuth.UserID;
            if (userSysNo <= 0 || string.IsNullOrWhiteSpace(userID))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 维护数据时为公共参数赋值
        /// </summary>
        /// <param name="bizEntity">继承EntityBase</param>
        /// <param name="isCreate">创建或更新</param>
        public static void SetBizEntityUserInfo(EntityBaseVM bizEntity, bool isCreate)
        {
            var user = ReadUserInfo();
            bizEntity.CompanyCode = ConstValue.CompanyCode;
            if (isCreate)
            {
                bizEntity.InUserSysNo = user.UserSysNo;
                bizEntity.InUserName = user.UserDisplayName;
                bizEntity.InDate = DateTime.Now;
            }
            else
            {
                bizEntity.EditUserSysNo = user.UserSysNo;
                bizEntity.EditUserName = user.UserDisplayName;
                bizEntity.EditDate = DateTime.Now;
            }
        }
    }

    public class UserAuth : IAuth
    {
        public bool ValidateAuth()
        {
            return UserMgr.HasLogin();
        }
    }

    [Serializable]
    public class LoginUser
    {
        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int UserSysNo { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>        
        public string UserDisplayName { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public CustomerRankType CustomerRank { get; set; }

        public bool RememberLogin { get; set; }


        public DateTime LoginDate { get { return DateTime.Parse(LoginDateText); } }

        public DateTime Timeout { get { return DateTime.Parse(TimeoutText); } }

        public string TimeoutText { get; set; }

        public string LoginDateText { get; set; }
    }
}
