using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.KeystoneAuthService;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class KeystoneAuthManager : IAuth, ILogin
    {
        private ObservableCollection<AuthMenuItem> m_cachedMenuItems;
        private ObservableCollection<AuthMenuItem> m_cachedNavigateItems;
        private ObservableCollection<Role> m_cachedRoles;
        private ObservableCollection<AuthMenuItem> m_cachedAuthUris;
        private List<AuthFunction> m_cachedAuthFunctionList;
        private List<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute> m_roleAttributes;
        private IPageBrowser m_browser;

        private KeystoneAuthV41Client m_authService;


        #region 用户失败登录次数管理
       

        #region IAuth Members

        public KeystoneAuthManager()
        {
            this.m_authService = new KeystoneAuthV41Client();

            this.m_authService.GetAuthDataCompleted += new EventHandler<GetAuthDataCompletedEventArgs>(m_authService_GetAuthDataCompleted);
            this.m_authService.GetAuthUserByRoleNameCompleted += new EventHandler<GetAuthUserByRoleNameCompletedEventArgs>(m_authService_GetAuthUserByRoleNameCompleted);
            this.m_authService.GetAuthUserByFunctionNameCompleted += new EventHandler<GetAuthUserByFunctionNameCompletedEventArgs>(m_authService_GetAuthUserByFunctionNameCompleted);
            this.m_authService.AutoLoginCompleted += new EventHandler<AutoLoginCompletedEventArgs>(m_authService_AutoLoginCompleted);
            this.m_authService.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(m_authService_LoginCompleted);
            this.m_authService.LogoutCompleted += new EventHandler<LogoutCompletedEventArgs>(m_authService_LogoutCompleted);
            m_authService.BatchGetUserInfoCompleted += new EventHandler<BatchGetUserInfoCompletedEventArgs>(m_authService_BatchGetUserInfoCompleted);
        }

        public void LoadAuthData(LoadAuthDataCompletedCallback callback)
        {
            DefaultDataContract msg = new DefaultDataContract()
            {
                Header = new MessageHeader()
                {
                    Language = CPApplication.Current.LanguageCode
                }
            };

            this.m_authService.GetAuthDataAsync(msg, callback);
        }

        public ObservableCollection<AuthMenuItem> GetAuthorizedMenuItems()
        {
            return m_cachedMenuItems;
        }

        public ObservableCollection<AuthMenuItem> GetAuthorizedNavigateItems()
        {
            return m_cachedNavigateItems;
        }

        public ObservableCollection<AuthMenuItem> AuthorizedNavigateToList()
        {
            return m_cachedAuthUris;
        }

        public bool HasFunction(string functionKey)
        {
            if (UtilityHelper.IsNullOrEmpty(functionKey))
            {
                throw new ArgumentNullException("functionKey");
            }
            if (m_cachedAuthFunctionList == null)
            {
                m_cachedAuthFunctionList = new List<AuthFunction>();
            }

            foreach (AuthFunction function in m_cachedAuthFunctionList)
            {
                if (UtilityHelper.AreEqualIgnoreCase(functionKey, function.Key))
                {
                    return true;
                }
            }

            return false;
        }

        //[Obsolete("该方法已经过期，请使用新的HasFunctionByAppName(string key,string appName)来替代该方法。")]
        //public bool HasFunction(string functionKey,string applicationId)
        //{
        //    if (UtilityHelper.IsNullOrEmpty(functionKey))
        //    {
        //        throw new ArgumentNullException("functionKey");
        //    }
        //    if (m_cachedAuthFunctionList == null)
        //    {
        //        m_cachedAuthFunctionList = new List<AuthFunction>();
        //    }

        //    foreach (AuthFunction function in m_cachedAuthFunctionList)
        //    {
        //        if (UtilityHelper.AreEqualIgnoreCase(functionKey, function.Key) && UtilityHelper.AreEqualIgnoreCase(function.ApplicationId, applicationId))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public bool HasFunctionByAppName(string functionKey, string appName)
        //{
        //    if (UtilityHelper.IsNullOrEmpty(functionKey))
        //    {
        //        throw new ArgumentNullException("functionKey");
        //    }

        //    KS_Application result = CPApplication.Current.KS_Applications.SingleOrDefault(app => UtilityHelper.AreEqualIgnoreCase(app.Name, appName));

        //    if (result == null)
        //    {
        //        throw new ArgumentException("Application Name can not found, please pre-load function list.");
        //    }

        //    if (m_cachedAuthFunctionList == null)
        //    {
        //        m_cachedAuthFunctionList = new List<AuthFunction>();
        //    }

        //    foreach (AuthFunction function in m_cachedAuthFunctionList)
        //    {
        //        if (UtilityHelper.AreEqualIgnoreCase(functionKey, function.Key) && UtilityHelper.AreEqualIgnoreCase(function.ApplicationId, result.Id))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public bool HasRole(string roleName, string appName)
        //{
        //    if (UtilityHelper.IsNullOrEmpty(roleName))
        //    {
        //        throw new ArgumentNullException("applicationId");
        //    }
        //    if (m_cachedRoles == null)
        //    {
        //        m_cachedRoles = new ObservableCollection<Role>();
        //    }

        //    KS_Application result = CPApplication.Current.KS_Applications.SingleOrDefault(app => UtilityHelper.AreEqualIgnoreCase(app.Name, appName));

        //    if (result == null)
        //    {
        //        throw new ArgumentException("Application Name can not found, please pre-load function list.");
        //    }

        //    foreach (Role role in m_cachedRoles)
        //    {
        //        if (UtilityHelper.AreEqualIgnoreCase(roleName, role.RoleName)
        //            && UtilityHelper.AreEqualIgnoreCase(role.ApplicationID, result.Id))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        public bool HasFunctionForPage(string url)
        {
            if (UtilityHelper.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (m_cachedAuthUris == null)
            {
                throw new ArgumentException("Page list isn't ready, please pre-load page list.");
            }

            foreach (var item in m_cachedAuthUris)
            {
                if (UtilityHelper.AreEqualIgnoreCase(url, item.URL))
                {
                    return true;
                }
            }

            return false;
        }

        public ObservableCollection<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute> GetAttributesByRoleName(string roleName, string applicationId)
        {
            ObservableCollection<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute> result = null;
            if (this.m_roleAttributes != null && this.m_roleAttributes.Count > 0)
            {
                result = new ObservableCollection<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute>();
                foreach (var attr in this.m_roleAttributes)
                {
                    if (UtilityHelper.AreEqualIgnoreCase(roleName, attr.RoleName) && UtilityHelper.AreEqualIgnoreCase(attr.ApplicationId, applicationId))
                    {
                        result.Add(attr);
                    }
                }
            }
            return result;
        }

        //public void GetAuthUserByRoleName(string roleName, string applicationId, Action<ObservableCollection<KeystoneAuthUser>> callback)
        //{
        //    var query = new AuthUserQueryV41
        //    {
        //        Header = new MessageHeader(),
        //        Body = new AuthUserQueryMsg
        //        {
        //            RoleName = roleName,
        //            ApplicationId = applicationId
        //        }
        //    };

        //    m_authService.GetAuthUserByRoleNameAsync(query, callback);
        //}

        //public void GetAuthUserByFunctionName(List<string> functionNames, string applicationId, Action<ObservableCollection<KeystoneAuthUser>> callback)
        //{
        //    var funcNames = new ObservableCollection<string>();

        //    functionNames.ForEach(item => 
        //    {
        //        funcNames.Add(item);
        //    });

        //    var query = new AuthUserQueryV41
        //    {
        //        Header = new MessageHeader(),
        //        Body = new AuthUserQueryMsg
        //        {
        //            FunctionNames = funcNames,
        //            ApplicationId = applicationId
        //        }
        //    };

        //    m_authService.GetAuthUserByFunctionNameAsync(query, callback);
        //}

        //public void GetAuthUserInfo(List<string> userIDList, Action<ObservableCollection<AuthUser>> callback)
        //{
        //    m_authService.BatchGetUserInfoAsync(new ObservableCollection<string>(userIDList), callback);
        //}

        void m_authService_BatchGetUserInfoCompleted(object sender, BatchGetUserInfoCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count() > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }

                var callback = e.UserState as Action<ObservableCollection<AuthUser>>;
                if (e.Result.Body != null)
                {
                    if (callback != null)
                    {
                        ObservableCollection<AuthUser> user = new ObservableCollection<AuthUser>();
                        foreach (var item in e.Result.Body)
                        {
                            user.Add(new AuthUser()
                            {
                                DepartmentName = item.DepartmentName,
                                DepartmentNumber = item.DepartmentNumber,
                                DisplayName = item.DisplayName,
                                Domain = item.Domain,
                                ID = item.UserName,
                                LoginName = item.UserName,
                                UserEmailAddress = item.EmailAddress,
                            });
                        }
                        callback(user);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
        }

        #endregion

        #region ILogin Members

        public void AutoLogin(Action<bool> callBack)
        {
            m_authService.AutoLoginAsync(new DefaultDataContract
            {
                Header = new MessageHeader { Language = CPApplication.Current.LanguageCode }
            }, callBack);
        }

        public void Login(string userName, string password, Action<bool> callBack)
        {
            m_authService.LoginAsync(new KeystoneAuthUserV41
            {
                Header = new MessageHeader { Language = CPApplication.Current.LanguageCode },
                Body = new KeystoneAuthUserMsg
                {
                    UserName = userName,
                    Password = password
                }
            }, callBack);
        }

        public void Logout(Action callback)
        {
            CPApplication.Current.Browser.LoadingSpin.Show();
            m_authService.LogoutAsync(callback);
        }

        #endregion



        #endregion

        void m_authService_GetAuthUserByFunctionNameCompleted(object sender, GetAuthUserByFunctionNameCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count() > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }

                var callback = e.UserState as Action<ObservableCollection<KeystoneAuthUser>>;
                if (e.Result.Body != null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.Body.ToEntity());
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
        }

        void m_authService_GetAuthUserByRoleNameCompleted(object sender, GetAuthUserByRoleNameCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }

                var callback = e.UserState as Action<ObservableCollection<KeystoneAuthUser>>;
                if (e.Result.Body != null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.Body.ToEntity());
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
        }

        void m_authService_GetAuthDataCompleted(object sender, GetAuthDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }

                KeystoneAuthDataMsg authData = e.Result.Body;

                GenerateAuthData(authData);
            }

            LoadAuthDataCompletedCallback callback = e.UserState as LoadAuthDataCompletedCallback;
            if (callback != null)
            {
                callback();
            }
        }

        void m_authService_LoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            bool isSuccess = false;
            if (e.Error == null && e.Result != null && e.Result.Body != null)
            {
                isSuccess = true;
                GenerateAuthData(e.Result.Body);
            }
            Action<bool> callback = e.UserState as Action<bool>;
            if (callback != null)
            {
                callback(isSuccess);
            }
            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }
            }
            else
            {
                throw e.Error;
            }
        }

        void m_authService_AutoLoginCompleted(object sender, AutoLoginCompletedEventArgs e)
        {
            bool isSuccess = false;
            Action<bool> callback = e.UserState as Action<bool>;
            if (e.Error != null)
            {
                if (callback != null)
                {
                    callback(isSuccess);
                }
            }
            else
            {
                if (e.Result != null && e.Result.Body != null)
                {
                    isSuccess = true;
                    GenerateAuthData(e.Result.Body);
                }

                if (callback != null)
                {
                    callback(isSuccess);
                }

                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }
            }
        }

        void m_authService_LogoutCompleted(object sender, LogoutCompletedEventArgs e)
        {
            CPApplication.Current.Browser.LoadingSpin.Hide();

            if (e.Error == null)
            {
                if (e.Result.Faults != null && e.Result.Faults.Count > 0)
                {
                    throw new Exception(e.Result.Faults[0].ErrorDescription);
                }
            }
            else
            {
                throw e.Error;
            }

            Action callback = e.UserState as Action;

            if (callback != null)
            {
                callback();
            }
        }

        void GenerateAuthData(KeystoneAuthDataMsg authData)
        {
            CPApplication.Current.Application = authData.Application.ToEntity();
            CPApplication.Current.KS_Applications = authData.KS_Applications.ToEntity();
            CPApplication.Current.LoginUser = authData.AuthUser.ToEntity();

            this.m_cachedAuthFunctionList = authData.Functions.ToEntity();
            this.m_roleAttributes = authData.RoleAttributes.ToEntity();
            this.m_cachedRoles = authData.Roles;
            this.m_cachedAuthUris = new ObservableCollection<AuthMenuItem>();
            authData.MenuData.GenerateAuthItems(null, ref this.m_cachedMenuItems, ref this.m_cachedNavigateItems, ref this.m_cachedAuthUris);


        }

        #region IComponent Members

        public string Name
        {
            get { return "KeystoneAuthManager"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(Controls.IPageBrowser browser)
        {
            m_browser = browser;
            m_browser.Navigating += new EventHandler<LoadedMoudleEventArgs>(m_browser_Navigating);
        }

        void m_browser_Navigating(object sender, LoadedMoudleEventArgs e)
        {
            if (e.Status == LoadModuleStatus.End)
            {
                IModuleInfo module = e.Request.ModuleInfo;
                IViewInfo view = module.GetViewInfoByName(e.Request.ViewName);

                if (view != null && view.NeedAccess && !this.HasFunctionForPage(string.Format("/{0}/{1}", e.Request.ModuleName, e.Request.ViewName)))
                {
                    throw new PageException(MessageResource.PageException_DeniedAccess_Title, MessageResource.PageException_DeniedAccess_Message, e.Request);
                }
            }
        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

    }

    public static class KeystoneAuthManagerExtesion
    {
        public static AuthApplication ToEntity(this AuthApplicationMsg msg)
        {
            AuthApplication entity = null;

            if (msg != null)
            {
                entity = new AuthApplication()
                {
                    Id = msg.Id,
                    Name = msg.Name,
                    DefaultLanguage = msg.DefaultLanguage,
                };
            }

            return entity;
        }


        public static KS_Application ToEntity(this KS_ApplicationMsg msg)
        {
            KS_Application entity = null;

            if (msg != null)
            {
                entity = new KS_Application()
                {
                    Id = msg.Id,
                    Name = msg.Name
                };
            }

            return entity;
        }

        public static List<KS_Application> ToEntity(this ObservableCollection<KS_ApplicationMsg> msg)
        {
            List<KS_Application> entity = null;

            if (msg != null)
            {
                entity = new List<KS_Application>();

                foreach (KS_ApplicationMsg item in msg)
                {
                    entity.Add(item.ToEntity());
                };
            }

            return entity;
        }


        public static AuthUser ToEntity(this KeystoneAuthUserMsg msg)
        {
            AuthUser entity = null;

            if (msg != null)
            {
                entity = new AuthUser()
                {
                    ID = msg.UserName,
                    LoginName = msg.UserName,
                    Domain = msg.Domain,
                    DisplayName = msg.DisplayName,
                    UserEmailAddress = msg.EmailAddress,
                    DepartmentName = msg.DepartmentName,
                    DepartmentNumber = msg.DepartmentNumber,
                    UserSysNo = msg.UserSysNo
                };
            }

            return entity;
        }


        public static AuthFunction ToEntity(this AuthFunctionMsg msg)
        {
            AuthFunction entity = null;

            if (msg != null)
            {
                entity = new AuthFunction()
                {
                    Key = msg.Name,
                    Name = msg.Name,
                    ApplicationId = msg.ApplicationId
                };
            }

            return entity;
        }

        public static List<AuthFunction> ToEntity(this ObservableCollection<AuthFunctionMsg> msgList)
        {
            List<AuthFunction> entityList = new List<AuthFunction>();

            if (msgList != null)
            {
                foreach (AuthFunctionMsg item in msgList)
                {
                    AuthFunction entity = item.ToEntity();

                    entityList.Add(entity);
                }
            }

            return entityList;
        }

        public static List<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute> ToEntity(this ObservableCollection<Newegg.Oversea.Silverlight.ControlPanel.Impl.KeystoneAuthService.RoleAttribute> msgList)
        {
            var entityList = new List<Newegg.Oversea.Silverlight.Core.Components.RoleAttribute>();

            if (msgList != null)
            {
                foreach (var item in msgList)
                {
                    var entity = item.ToEntity();

                    entityList.Add(entity);
                }
            }

            return entityList;
        }

        public static AuthMenuItemType ToEntity(this MenuTypeEnum msg)
        {
            switch (msg)
            {
                case MenuTypeEnum.Category: return AuthMenuItemType.Category;
                case MenuTypeEnum.Page: return AuthMenuItemType.Page;
                case MenuTypeEnum.Link: return AuthMenuItemType.Link;
                default: throw new NotSupportedException(String.Format("AuthMenuItemType.{0}", msg));
            }
        }

        public static AuthMenuItem ToEntity(this AuthMenuItemMsg msg)
        {
            AuthMenuItem entity = null;

            if (msg != null)
            {
                entity = new AuthMenuItem()
                {
                    Id = msg.MenuId.ToString(),
                    Name = msg.DisplayName,
                    Description = msg.Description,
                    IconStyle = msg.IconStyle,
                    IsDisplay = msg.IsDisplay,
                    URL = msg.LinkPath,
                    Type = msg.Type.ToEntity(),
                    AuthKey = msg.AuthKey
                };
            }

            return entity;
        }

        public static Newegg.Oversea.Silverlight.Core.Components.RoleAttribute ToEntity(this Newegg.Oversea.Silverlight.ControlPanel.Impl.KeystoneAuthService.RoleAttribute msg)
        {
            Newegg.Oversea.Silverlight.Core.Components.RoleAttribute entity = null;

            if (msg != null)
            {
                entity = new Newegg.Oversea.Silverlight.Core.Components.RoleAttribute
                {
                    ApplicationId = msg.ApplicationId,
                    Name = msg.Name,
                    RoleName = msg.RoleName,
                    Value = msg.Value,
                    Type = msg.Type
                };
            }

            return entity;
        }


        public static ObservableCollection<AuthMenuItemMsg> LinkPageProcess(this ObservableCollection<AuthMenuItemMsg> dataItems)
        {
            var linkList = new ObservableCollection<AuthMenuItemMsg>();
            var result = new ObservableCollection<AuthMenuItemMsg>();

            dataItems.ToList().ForEach(item =>
            {
                if (item.Type == MenuTypeEnum.Link)
                {
                    linkList.Add(item);
                }
            });

            foreach (var item in dataItems)
            {
                if (item.Type == MenuTypeEnum.Category)
                {
                    result.Add(item);
                }
                else if (item.Type == MenuTypeEnum.Page)
                {
                    foreach (var link in linkList)
                    {
                        if (link.LinkPath == item.MenuId.ToString())
                        {
                            result.Add(link);
                            linkList.Remove(link);
                            break;
                        }
                    }
                    result.Add(item);
                }
            }

            return result;
        }

        public static void GenerateAuthItems(this ObservableCollection<AuthMenuItemMsg> dataItems, AuthMenuItemMsg parentItem, ref ObservableCollection<AuthMenuItem> menuItems, ref ObservableCollection<AuthMenuItem> navigateItems, ref ObservableCollection<AuthMenuItem> authUris)
        {
            menuItems = new ObservableCollection<AuthMenuItem>();
            navigateItems = new ObservableCollection<AuthMenuItem>();

            if (dataItems != null)
            {
                // 1、Find matched items
                List<AuthMenuItemMsg> matchedItems = new List<AuthMenuItemMsg>();
                foreach (AuthMenuItemMsg item in dataItems)
                {
                    if ((parentItem == null && item.ParentMenuId == null) || (parentItem != null && item.ParentMenuId == parentItem.MenuId))
                    {
                        matchedItems.Add(item);
                    }
                }
                // 2、Remove matched items from original collection
                foreach (AuthMenuItemMsg item in matchedItems)
                {
                    dataItems.Remove(item);
                }
                // 3、Find children for matched items
                foreach (AuthMenuItemMsg item in matchedItems)
                {
                    ObservableCollection<AuthMenuItem> subMenuItems = null;
                    ObservableCollection<AuthMenuItem> subNavigateItems = null;

                    GenerateAuthItems(dataItems, item, ref subMenuItems, ref subNavigateItems, ref authUris);

                    if (item.Type == MenuTypeEnum.Page || item.Type == MenuTypeEnum.Link || subMenuItems.Count > 0)
                    {
                        if (item.IsDisplay)
                        {
                            AuthMenuItem menuItem = item.ToEntity();

                            menuItem.Items = subMenuItems;

                            menuItems.Add(menuItem);
                        }
                    }

                    if (item.Type == MenuTypeEnum.Page || item.Type == MenuTypeEnum.Link || subNavigateItems.Count > 0)
                    {
                        AuthMenuItem menuItem = item.ToEntity();

                        menuItem.Items = subNavigateItems;

                        navigateItems.Add(menuItem);
                    }

                    if (item.Type == MenuTypeEnum.Category || item.Type == MenuTypeEnum.Page || item.Type == MenuTypeEnum.Link)
                    {
                        var a = item.ToEntity();
                        a.Parent = parentItem.ToEntity();
                        authUris.Add(a);
                    }
                }
            }
        }


        public static KeystoneAuthUser ToEntity(this AuthUserMsg msg)
        {
            return new KeystoneAuthUser
            {
                UniqueName = msg.UniqueName,
                LastName = msg.LastName,
                FirstName = msg.FirstName,
                FullName = msg.FullName
            };
        }

        public static ObservableCollection<KeystoneAuthUser> ToEntity(this ObservableCollection<AuthUserMsg> msg)
        {
            var result = new ObservableCollection<KeystoneAuthUser>();

            msg.ToList().ForEach(item =>
            {
                result.Add(item.ToEntity());
            });

            return result;
        }
    }

}
