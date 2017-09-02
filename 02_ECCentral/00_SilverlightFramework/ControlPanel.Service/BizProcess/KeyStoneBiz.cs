using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Text;
using System.Xml.Linq; 


using BitKoo.Keystone;
using Newegg.Oversea.Framework.Cache;
using Newegg.Oversea.Framework.Log;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.DirectoryServices;
using Newegg.Oversea.Framework.ExceptionHandler;
using System.Web;
using System.Web.SessionState;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess
{
    public class KeyStoneBiz : IAuth
    {
        private string m_applicationId;
        private string[] m_KS_ApplicationIds;
        private string[] m_KS_SourceDirectories;
        private string m_KS_PrimaryAuthUrl;
        private string m_KS_SecondaryAuthUrl;
        private string m_KS_TrustedDirectory;
        private string m_KS_TrustedUserName;
        private string m_KS_TrustedPassword;

        private HttpSessionState m_Session;
        private ICacher m_localCacher;

        public KeyStoneBiz()
        {
            m_Session = HttpContext.Current.Session;

            m_applicationId = CPConfig.Application.Id;
            m_KS_ApplicationIds = CPConfig.Keystone.ApplicationIds;
            m_KS_SourceDirectories = CPConfig.Keystone.SourceDirectories;
            m_KS_PrimaryAuthUrl = CPConfig.Keystone.PrimaryAuthUrl;
            m_KS_SecondaryAuthUrl = CPConfig.Keystone.SecondaryAuthUrl;
            m_KS_TrustedDirectory = CPConfig.Keystone.TrustedDirectory;
            m_KS_TrustedUserName = CPConfig.Keystone.TrustedUserName;
            m_KS_TrustedPassword = CPConfig.Keystone.TrustedPassword;

            m_localCacher = CacherFactory.LocalCache;
        }

        #region IAuth

        public void GetAuthData(string acct, string domain, out KeystoneAuthUserMsg userInfo,
            out List<RoleAttribute> roleAttributeList, out List<Role> roleList, out List<AuthFunctionMsg> functionList)
        {
            userInfo = this.GetAuthUser(acct, domain);

            List<RoleAttribute> roleAttrs = null;
            Task getRoleAttributesTask = new Task(() =>
            {
                roleAttrs = this.GetRoleAttributes(acct, domain);
            });

            List<Role> roles = null;
            Task getRolesTask = new Task(() =>
            {
                roles = this.GetRoles(acct, domain);
            });

            List<AuthFunctionMsg> funs = null;
            Task getFunctionTask = new Task(() =>
            {
                funs = this.GetFunctions(acct, domain);
            });
            getRoleAttributesTask.Start();
            getRolesTask.Start();
            getFunctionTask.Start();
            Task.WaitAll(new Task[] { getRoleAttributesTask, getRolesTask, getFunctionTask });

            roleAttributeList = roleAttrs;
            roleList = roles;
            functionList = funs;
        }

        public bool Login(string userName, string password, ref string domain, bool isTrusted)
        {
            bool isLogin = false;

            string[] loginUserParams = new string[] { userName, password };
            string[] sourceDirectories = domain != null ? new string[] { domain } : m_KS_SourceDirectories;

            string trustedDirectory = m_KS_TrustedDirectory;
            string[] trustedUserParams = new string[] { m_KS_TrustedUserName, m_KS_TrustedPassword };

            Auth auth = new Auth();
            auth.PrimaryAuthUrl = m_KS_PrimaryAuthUrl;
            auth.SecondaryAuthUrl = m_KS_SecondaryAuthUrl;

            if (TryLoginToKeystoneByCache(userName, ref domain, isTrusted, auth, sourceDirectories, loginUserParams, trustedDirectory, trustedUserParams))
            {
                return true;
            }
            else
            {
                //该判断是为了防止AD账号3次登录失败后，把账号给locked了；
                if (auth.Message != null && auth.Message.Equals("Authentication Failed, Message: Invalid Credentials"))
                {
                    return false;
                }
            }

            foreach (string appId in this.m_KS_ApplicationIds)
            {
                auth.ApplicationId = appId;

                bool b = false;
                List<string> loginResultMessage = new List<string>();
                foreach (string sourceDirectory in sourceDirectories)
                {
                    if (isTrusted)
                    {
                        b = auth.TrustedLogin(sourceDirectory, loginUserParams, trustedDirectory, trustedUserParams);
                    }
                    else
                    {
                        b = auth.Login(sourceDirectory, loginUserParams);

                        //该判断是为了防止AD账号3次登录失败后，把账号给locked了；
                        if (auth.Message != null && auth.Message.Equals("Authentication Failed, Message: Invalid Credentials"))
                        {
                            return false;
                        }
                    }

                    if (b)
                    {
                        domain = sourceDirectory.ToLower();

                        break;
                    }
                }

                if (b)
                {
                    isLogin = true;
                    auth.Logout();
                    string key = GetCacheKeyForAppId(userName, domain.ToLower());
                    m_localCacher.Add(key, appId);
                    return isLogin;
                }
            }
            return isLogin;
        }

        public void Logout()
        {
            m_Session.Clear();
        }

        #endregion

        private KeystoneAuthUserMsg GetAuthUser(string userName, string domain)
        {
            KeystoneAuthUserMsg authUser = new KeystoneAuthUserMsg();
            authUser.CompanyCode = CPConfig.Application.BusinessCode;
            authUser.Domain = domain;
            authUser.UserName = userName;
            authUser.CompanyName = "NeweggSoft";

            UserInfo user = CPContext.GetUserInfoFromAD(userName, domain);
            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                authUser.DisplayName = user.FullName;
                authUser.EmailAddress = user.EmailAddress;
                authUser.DepartmentName = user.Department;
                authUser.DepartmentNumber = user.Department;
            }
            else
            {
                authUser.DisplayName = userName;
            }

            if (CPConfig.ECCentral != null)
            {
                string serviceUrl = CPConfig.ECCentral.ServiceURL;
                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    serviceUrl = serviceUrl.TrimEnd('/');
                    string url = serviceUrl + "/CommonService/User/{0}";

                    url = string.Format(url, userName);
                    WebClient web = new WebClient();
                    web.Encoding = Encoding.UTF8;
                    string resultString = web.DownloadString(new Uri(url));

                    int userSysNo = -1;
                    int.TryParse(resultString, out userSysNo);

                    if (userSysNo != -1)
                    {
                        authUser.UserSysNo = int.Parse(resultString);
                    }
                    else
                    {
                        throw new Exception("Don't found this user system number by current account!");
                    }
                }
                else
                {
                    authUser.UserSysNo = -1;
                }
            }
            else
            {
                authUser.UserSysNo = -1;
            }



            return authUser;
        }

        private List<AuthFunctionMsg> GetFunctions(string userName, string domain)
        {
            string key = this.GetCacheKeyForFunctions(userName, domain);
            List<AuthFunctionMsg> functions = m_Session[key] as List<AuthFunctionMsg>;
            if (functions != null)
            {
                return functions;
            }
            functions = KeystoneDA.GetAuthFunctionalAbilities(userName, domain, m_KS_ApplicationIds.ToList()).ToMsg();
            m_Session.Add(key, functions);

#if TRACE
            System.Diagnostics.Trace.WriteLine(string.Format("{0}", functions.Count));
#endif

            return functions;
        }

        private List<RoleAttribute> GetRoleAttributes(string userName, string domain)
        {
            string key = this.GetCacheKeyForRoleAttributes(userName, domain);
            List<RoleAttribute> roleAttributes = m_Session[key] as List<RoleAttribute>;
            if (roleAttributes != null)
            {
                return roleAttributes;
            }

            roleAttributes = KeystoneDA.GetAuthRoleAttributes(userName, domain, m_KS_ApplicationIds.ToList()).ToMsg();
            m_Session.Add(key, roleAttributes);
            return roleAttributes;
        }

        private List<Role> GetRoles(string userName, string domain)
        {
            string key = this.GetCacheKeyForRoles(userName, domain);
            List<Role> roles = m_Session[key] as List<Role>;
            if (roles != null)
            {
                return roles;
            }
            roles = KeystoneDA.GetAuthRolesByUser(userName, domain, m_KS_ApplicationIds.ToList()).ToMsg();
            m_Session.Add(key, roles);
            return roles;
        }

        public List<AuthMenuItemMsg> GetMenuItems(string userName, string domain, string languageCode, List<AuthFunctionMsg> functions)
        {
            string key = this.GetCacheKeyForMenuItems(userName, domain, languageCode);

            List<AuthMenuItemMsg> menuItems = m_Session[key] as List<AuthMenuItemMsg>;
            if (menuItems != null)
            {
                return menuItems;
            }
            //List<AuthFunctionMsg> functions = GetFunctions(userName, domain);
            List<ControlPanelLocalizedMenuEntity> menuData = GetLocalizedMenuItems(m_KS_ApplicationIds, languageCode);
            List<ControlPanelLocalizedMenuEntity> matchedItems = this.GetMatchedAuthMenuData(menuData, functions);

            menuItems = matchedItems.ToMessage();

            m_Session.Add(key, menuItems);

            return menuItems;
        }

        //public List<AuthUser> GetAuthUserByRoleName(string roleName, string applicationId)
        //{
        //    return KeystoneDA.GetAuthUserByRoleName(roleName, applicationId);
        //}

        //public List<AuthUser> GetAuthUserByFunctionName(List<string> functionNames, string applicationId)
        //{
        //    return KeystoneDA.GetAuthUserByFunctionName(functionNames, applicationId);
        //}

        private bool TryLoginToKeystoneByCache(string userName, ref string domain, bool isTrusted, Auth auth, string[] sourceDirectories, string[] loginUserParams, string trustedDirectory, string[] trustedUserParams)
        {
            bool isLogin = false;
            if (sourceDirectories == null)
            {
                return isLogin;
            }

            foreach (string sourceDirectory in sourceDirectories)
            {
                string key = GetCacheKeyForAppId(userName, sourceDirectory.ToLower());
                string keyAppId = m_localCacher.GetData<string>(key);
                if (keyAppId == null)
                {
                    continue;
                }
                auth.ApplicationId = keyAppId;
                if (isTrusted)
                {
                    isLogin = auth.TrustedLogin(sourceDirectory, loginUserParams, trustedDirectory, trustedUserParams);
                }
                else
                {
                    isLogin = auth.Login(sourceDirectory, loginUserParams);

                    //该判断是为了防止AD账号3次登录失败后，把账号给locked了；
                    if (auth.Message != null && auth.Message.Equals("Authentication Failed, Message: Invalid Credentials"))
                    {
                        return false;
                    }
                }

                if (isLogin)
                {
                    domain = sourceDirectory.ToLower();

                    break;
                }
            }
            return isLogin;
        }

        private List<ControlPanelLocalizedMenuEntity> GetLocalizedMenuItems(string[] appIds, string languageCode)
        {
            ControlPanelLocalizedMenuQueryEntity queryEntity = new ControlPanelLocalizedMenuQueryEntity()
            {
                LanguageCode = languageCode,
                ApplicationIds = appIds,
                StatusCode = ((MenuStatus?)MenuStatus.Active).ToCharValue()
            };

            List<ControlPanelLocalizedMenuEntity> menuItems = new ControlPanelMenuBiz().GetLocalizedMenuItems(queryEntity);

            return menuItems;
        }

        private List<ControlPanelLocalizedMenuEntity> GetMatchedAuthMenuData(List<ControlPanelLocalizedMenuEntity> menuList, List<AuthFunctionMsg> functionList)
        {
            List<ControlPanelLocalizedMenuEntity> pageItems = menuList.FindAll(menu =>
            {
                if (menu.Type == MenuType.Category)
                {
                    return true;
                }
                if (menu.Type == MenuType.Link)
                {
                    //保证当前的Link有对应的Page存在
                    var page = menuList.FirstOrDefault(p => p.MenuId.ToString() == menu.LinkPath);
                    if (page != null)
                    {
                        var isExists = false;
                        if (StringUtility.IsNullOrEmpty(page.AuthKey))
                        {
                            isExists = true;
                        }
                        else
                        {
                            isExists = functionList.Exists(function => StringUtility.AreTheSameIgnoreCase(page.ApplicationId, function.ApplicationId)
                            && StringUtility.AreTheSameIgnoreCase(page.AuthKey, function.Name));
                        }

                        if (isExists)
                        {
                            menu.DisplayName = page.DisplayName;
                            menu.LinkPath = page.LinkPath;
                            menu.IsDisplayCode = page.IsDisplayCode;
                            menu.StatusCode = page.StatusCode;
                            return true;
                        }
                        return false;
                    }
                    return false;
                }

                if (menu.Type == MenuType.Page)
                {
                    if (StringUtility.IsNullOrEmpty(menu.AuthKey))
                    {
                        return true;
                    }
                    var isExists = functionList.Exists(function => StringUtility.AreTheSameIgnoreCase(menu.ApplicationId, function.ApplicationId) && StringUtility.AreTheSameIgnoreCase(menu.AuthKey, function.Name));
                    return isExists;
                }

                return false;
            });

            return pageItems;
        }

        private string GetCacheKeyForAuthUser(string userName, string domain)
        {
            return String.Format(@"[{0}]_AuthUser_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheKeyForAppId(string userName, string domain)
        {
            return String.Format(@"[{0}]_AppId_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheKeyForFunctions(string userName, string domain)
        {
            return String.Format(@"[{0}]_Functions_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheKeyForRoleAttributes(string userName, string domain)
        {
            return String.Format(@"[{0}]_RoleAttributes_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheKeyForRoles(string userName, string domain)
        {
            return String.Format(@"[{0}]_Roles_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheGroupForMenuItems(string userName, string domain)
        {
            return String.Format(@"[{0}]_MenuItems_[{1}\{2}]", m_applicationId, domain, userName);
        }

        private string GetCacheKeyForMenuItems(string userName, string domain, string languageCode)
        {
            return String.Format(@"[{0}]_MenuItems_[{1}\{2}]_[{3}]", m_applicationId, domain, userName, languageCode);
        }

        public bool Login(string acct, string password, ref string domain, bool isTrusted, out string identityToken)
        {
            throw new NotImplementedException();
        }



    }
}