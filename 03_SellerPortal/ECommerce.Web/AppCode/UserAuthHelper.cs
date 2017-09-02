using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ECommerce.Entity;
using ECommerce.WebFramework;
using ECommerce.Entity.Common;
using ECommerce.Service.Common;
using ECommerce.Entity.ControlPannel;
using System.Text;
using System.Security.Cryptography;
using ECommerce.Service.ControlPannel;
using System.Web.Caching;

namespace ECommerce.Web
{
    public class UserAuthHelper
    {
        const string LOGIN_COOKIE = "SP_LoginCookie";
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public static UserAuthVM GetCurrentUser()
        {
            UserAuthVM user = ReadUserInfo();
            if (user == null || user.UserSysNo == 0 || string.IsNullOrWhiteSpace(user.UserID))
            {
                user = null;
            }
            return user;
        }

        private static UserAuthVM ReadUserInfo()
        {
            return CookieHelper.GetCookie<UserAuthVM>(LOGIN_COOKIE);
        }

        public static bool HasLogin()
        {
            UserAuthVM user = ReadUserInfo();
            if (user == null || user.UserSysNo == 0 || string.IsNullOrWhiteSpace(user.UserID))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 当前用户是否有权限
        /// </summary>
        /// <param name="controller">Controller名</param>
        /// <param name="action">Action名</param>
        /// <returns></returns>
        public static bool HasAuth(string controller, string action)
        {
            UserAuthVM user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }
            string authKey = controller.Trim() + "|" + action.Trim();
            return HasAuth(authKey);
        }

        /// <summary>
        /// 当前用户是否有权限
        /// </summary>
        /// <param name="controller">Authkey</param> 
        /// <returns></returns>
        public static bool HasAuth(string authKey)
        {
            UserAuthVM user = GetCurrentUser();
            
            List<PrivilegeInfo> allList = GetAllAuthKeyList();

            if (!allList.Exists(f => f.PrivilegeName.Trim().ToLower() == authKey.Trim().ToLower()))
            {
                //不需要做控制的
                return true;
            }
            
            //验证页面是否有权限:
            if (user == null || user.UserSysNo == 0 || string.IsNullOrWhiteSpace(user.UserID)
                || user.UserAuthKeyList == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(authKey) || user.UserAuthKeyList.FindIndex(f => f.Trim().ToUpper() == authKey.Trim().ToUpper()) >= 0)
            {
                return true;
            }
            return false;
        }

        public static List<PrivilegeInfo> GetAllAuthKeyList()
        {
            string cacheKey = CommonService.GenerateKey("GetAllAuthKeyList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<PrivilegeInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<PrivilegeInfo> allList = UserService.GetPrivilegeList(); 
            HttpRuntime.Cache.Insert(cacheKey, allList, null, DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);

            return allList;
             
        }

        /// <summary>
        /// 维护数据时为公共参数赋值
        /// </summary>
        /// <param name="bizEntity">继承EntityBase</param>
        /// <param name="isCreate">创建或更新</param>
        public static void SetBizEntityUserInfo(EntityBase bizEntity, bool isCreate)
        {
            UserAuthVM user = GetCurrentUser();
            bizEntity.CompanyCode = user.CompanyCode;
            bizEntity.SellerSysNo = user.SellerSysNo;
            bizEntity.LanguageCode = user.LanguageCode;
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

        /// <summary>
        /// 写用户登录信息
        /// </summary>
        /// <param name="userSysNo">用户编号</param>
        /// <param name="userID">用户名</param>
        /// <param name="userDisplayName">用户显示名</param>
        public static void WriteUserInfo(UserInfo user)
        {
            if (user == null || user.SysNo == 0 || string.IsNullOrWhiteSpace(user.UserID))
            {
                Logout();
                return;
            }
            var userAuth = new UserAuthVM();
            userAuth.UserSysNo = user.SysNo.Value;
            userAuth.UserID = user.UserID;
            userAuth.UserDisplayName = user.UserName;
            userAuth.SellerSysNo = user.VendorSysNo.Value;
            userAuth.SellerName = user.VendorName;
            userAuth.CompanyCode = user.CompanyCode;
            userAuth.LanguageCode = user.LanguageCode;
            userAuth.VendorStockType = user.VendorStockType;
            //userAuth.UserAuthKeyList = user.UserAuthKeyList;

            userAuth.UserAuthKeyList = new List<string>();
            var currnentUserInfo = UserService.GetUserInfo(user.SysNo.Value, user.VendorSysNo.Value);
            if (null != currnentUserInfo && currnentUserInfo.Roles.Count > 0)
            {
                foreach (var userRole in currnentUserInfo.Roles)
                {
                    var privilegeList = UserService.GetPrivilegeListByRoleSysNo(userRole.RoleSysNo.Value);
                    if (null != privilegeList && privilegeList.Count > 0)
                    {
                        userAuth.UserAuthKeyList.AddRange(privilegeList.Select(x => x.PrivilegeName).ToList());
                    }

                }
                userAuth.UserAuthKeyList = userAuth.UserAuthKeyList.Distinct().ToList();
            }
            CookieHelper.SaveCookie<UserAuthVM>(LOGIN_COOKIE, userAuth);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public static void Logout()
        {
            CookieHelper.SaveCookie<UserAuthVM>(LOGIN_COOKIE, new UserAuthVM());
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pwd"></param>
        /// <param name="verifyCode"></param>
        /// <param name="outMessageString"></param>
        /// <returns></returns>
        public static UserInfo Login(string sellerCode, string userID, string pwd, string verifyCode)
        {
            UserInfo user = LoginService.UserLogin(userID, EncryptPassword(pwd));
            if (null != user && user.SysNo > 0)
            {
                WriteUserInfo(user);
            }
            return user;
        }

        public static string EncryptPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(password));
                StringBuilder builder = new StringBuilder(0x20);
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
                }
                return builder.ToString();
            }
            return string.Empty;
        }

    }
}