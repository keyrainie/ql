using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Configuration;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Transformers;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using System.Threading;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [InternationalBehavior]
    [ServiceErrorHandling]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class KeystoneAuthService : IKeystoneAuthV41
    {
        private string m_key_SignOut = "NeweggCentral_SignOut";

        #region IKeystoneAuthV41 Members

        [ErrorHandling]
        public KeystoneAuthDataV41 GetAuthData(DefaultDataContract msg)
        {
            CPContext context = CPContext.Current;
            string loginName = null;
            string domain = null;
            string userName = context.GetUserName(out loginName, out domain);
            string languageCode = msg.Header.Language;
            AuthApplicationMsg application = new AuthApplicationMsg()
            {
                Id = CPConfig.Application.Id,
                Name = CPConfig.Application.Name,
                DefaultLanguage = CPConfig.Application.DefaultLanguage,
            };
            List<KS_ApplicationMsg> ks_applications = new List<KS_ApplicationMsg>();
            KeystoneApplicationCollection applications = CPConfig.Keystone.Applications;
            if (applications != null)
            {
                foreach (KeystoneApplicationElement app in applications)
                {
                    ks_applications.Add(new KS_ApplicationMsg()
                    {
                        Id = app.Id,
                        Name = app.Name
                    });
                }
            }
            KeystoneAuthDataV41 result = new KeystoneAuthDataV41()
            {
                Header = msg.Header,
                Body = new KeystoneAuthDataMsg()
                {
                    Application = application,
                    KS_Applications = ks_applications,
                }
            };

            KeystoneAuthUserMsg userInfo;
            List<RoleAttribute> roleAttributeList;
            List<Role> roleList;
            List<AuthFunctionMsg> functionList;
            AuthFactory.GetInstance().GetAuthData(loginName, domain, out userInfo, out roleAttributeList, out roleList, out functionList);

            result.Body.AuthUser = userInfo;
            result.Body.RoleAttributes = roleAttributeList;
            result.Body.Roles = roleList;
            result.Body.Functions = functionList;
            result.Body.MenuData = new KeyStoneBiz().GetMenuItems(loginName, domain, languageCode,functionList);
            
            return result;
        }

        [ErrorHandling]
        public AuthUserListV41 GetAuthUserByRoleName(AuthUserQueryV41 msg)
        {
            if (msg == null || msg.Body == null)
            {
                throw new ArgumentNullException("msg");
            }

            if (StringUtility.IsNullOrEmpty(msg.Body.RoleName))
            {
                throw new ArgumentNullException("msg.Body.RoleName");
            }

            if (StringUtility.IsNullOrEmpty(msg.Body.ApplicationId))
            {
                throw new ArgumentNullException("msg.Body.ApplicationId");
            }

            var result = new AuthUserListV41 { Header = msg.Header };

            //result.Body = new KeyStoneBiz().GetAuthUserByRoleName(msg.Body.RoleName, msg.Body.ApplicationId).ToMessage();

            return result;
        }

        [ErrorHandling]
        public AuthUserListV41 GetAuthUserByFunctionName(AuthUserQueryV41 msg)
        {
            if (msg == null || msg.Body == null)
            {
                throw new ArgumentNullException("msg");
            }

            if (StringUtility.IsNullOrEmpty(msg.Body.ApplicationId))
            {
                throw new ArgumentNullException("msg.Body.ApplicationId");
            }

            if (msg.Body.FunctionNames == null)
            {
                throw new ArgumentNullException("msg.Body.FunctionNames");
            }

            var result = new AuthUserListV41 { Header = msg.Header };

            //result.Body = new KeyStoneBiz().GetAuthUserByFunctionName(msg.Body.FunctionNames, msg.Body.ApplicationId).ToMessage();

            return result;
        }

        #endregion

        [ErrorHandling]
        public KeystoneAuthDataV41 Login(KeystoneAuthUserV41 msg)
        {
            if (!this.CheckDataContractForLogin(msg))
            {
                return new KeystoneAuthDataV41 { Body = null };
            }
            string userName = msg.Body.UserName;
            #region 去满足能够支持带域名(abs_corp/rl53)登录；

            if (msg.Body.UserName.IndexOf('\\') > -1)
            {
                string[] tempStringArray= msg.Body.UserName.Split('\\');
                if (tempStringArray.Length >= 2)
                {
                    userName = tempStringArray[1];
                }
            }

            #endregion
            string password = msg.Body.Password;
            string domain = msg.Body.Domain;
            string identityToken = string.Empty;
            bool b = AuthFactory.GetInstance().Login(userName, password, ref domain, false, out identityToken);
            if (b)
            {
                TraceLoginEventLog(userName);
                SetAuthFormCookie(userName, domain);
                return GetAuthData(msg);
            }
            return new KeystoneAuthDataV41 { Body = null };
        }

        [ErrorHandling]
        public KeystoneAuthDataV41 AutoLogin(DefaultDataContract msg)
        {
            HttpContext context = HttpContext.Current;

            //判断是否是logout后的自动登录，如果是返回数据NULL，并阻止自动登录。
            HttpCookie cookie =context.Request.Cookies[m_key_SignOut];
            if (cookie != null)
            {
                var responseCookie = new HttpCookie(m_key_SignOut);
                responseCookie.Expires = DateTime.Now.AddDays(-30);
                context.Response.Cookies.Add(responseCookie);
                return new KeystoneAuthDataV41 { Body = null };
            }


            if (CPConfig.Application.AutoLogin && !context.Request.IsAuthenticated)
            {
                TryLogin();
            }
            if (context.Request.IsAuthenticated)
            {
                if (HttpContext.Current.Session["IsLoginSystem"] == null)
                {
                    string loginName = null;
                    string domain = null;
                    CPContext cpContext = CPContext.Current;
                    string userName = cpContext.GetUserName(out loginName, out domain);
                    TraceLoginEventLog(loginName);
                    HttpContext.Current.Session["IsLoginSystem"] = true;
                }
                KeystoneAuthDataV41 result = new KeystoneAuthDataV41
                {
                    Body = GetAuthData(msg).Body
                };
                return result;
            }
            return new KeystoneAuthDataV41 { Body = null };
        }

        [ErrorHandling]
        public DefaultDataContract Logout()
        {
            string name = HttpContext.Current.User.Identity.Name;
            string[] names = name.Split('\\');
            if (names.Length == 2)
            {
                AuthFactory.GetInstance().Logout();
                FormsAuthentication.SignOut();
                var cookie = new HttpCookie(m_key_SignOut);
                cookie.Expires = DateTime.Now.AddMinutes(10);
                cookie.Value = "SignOut_Value";
                HttpContext.Current.Response.Cookies.Add(cookie);
                HttpContext.Current.Session.Clear();
            }
            return new DefaultDataContract();
        }

        private void TryLogin()
        {
            HttpContext context = HttpContext.Current;
            string userName = context.Request.ServerVariables["LOGON_USER"];
            // 如果 userName 不为空，表明浏览器已取得通过 Windows Authentication 认证的用户名；
            if (!string.IsNullOrWhiteSpace(userName))
            {
                string[] names = userName.Split('\\');

                if (names.Length == 2)
                {
                    string loginName = names[1];
                    string loginDomain = names[0];

                    string identityToken = string.Empty;
                    bool b = AuthFactory.GetInstance().Login(loginName, null, ref loginDomain, true, out identityToken);
                    if (b)
                    {
                        SetAuthFormCookie(loginName, loginDomain);
                    }
                }
            }
            else
            {
                //返回401，未授权code，通知浏览器弹出域账号和密码输入框
                context.Items.Add("StatusCode", "401");
            }
        }

        private bool CheckDataContractForLogin(KeystoneAuthUserV41 user)
        {
            if (user == null)
            {
                return false;
            }
            if (user.Body == null)
            {
                return false;
            }
            if (user.Body.UserName == null || user.Body.UserName.Trim() == String.Empty)
            {
                return false;
            }
            if (user.Body.Password == null || user.Body.Password.Trim() == String.Empty)
            {
                return false;
            }
            return true;
        }

        private void TraceLoginEventLog(string loginName)
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                var biz = new StatisticBiz();
                biz.InsertEventLog(new EventLog
                {
                    Action = "Login",
                    EventDate = DateTime.Now,
                    IP = GetRealIP(obj as HttpRequest),
                    UserID = loginName
                });
            }, HttpContext.Current.Request);
        }

        private string GetRealIP(HttpRequest request)
        {
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        private void SetAuthFormCookie(string userName, string domain)
        {
            string authUserName = String.Format(@"{0}\{1}", domain, userName);
            FormsAuthentication.SetAuthCookie(authUserName, true);
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(authUserName, true);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(ticket), new string[0]);
        }

        [ErrorHandling]
        public KeystoneAuthUserListV41 BatchGetUserInfo(List<string> userIDList)
        {
            KeystoneAuthUserListV41 result = new KeystoneAuthUserListV41();
            result.Body = new List<KeystoneAuthUserMsg>();
            foreach (string userID in userIDList)
            {
                UserInfo user = CPContext.GetUserInfoFromAD(userID, CPContext.GetADDomain());
                result.Body.Add(
                    new KeystoneAuthUserMsg()
                    {
                        EmailAddress = user.EmailAddress,
                        DisplayName = user.FullName,
                        Domain = CPContext.GetADDomain(),
                        DepartmentName = user.Department,
                        DepartmentNumber = user.Department,
                        UserName= user.UserID,
                    });
            }
            return result;
        }
    }

}
