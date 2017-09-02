using System;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Net;
using System.DirectoryServices;

using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    public partial class CPContext
    {
        private static Dictionary<string, UserInfo> s_UserList = new Dictionary<string, UserInfo>();

        public static CPContext Current
        {
            get
            {
                CPContext cpContext = HttpContext.Current.Items["WebRuntime_CPContext"] as CPContext;

                if (cpContext == null)
                {
                    cpContext = new CPContext();

                    HttpContext.Current.Items.Add("WebRuntime_CPContext", cpContext);
                }

                return cpContext;
            }
        }


        public string GetUserName(out string loginName, out string domain)
        {
            HttpContext context = HttpContext.Current;

            string userName = null;

            loginName = null;
            domain = null;

            if (context.User != null)
            {
                userName = context.User.Identity.Name;
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                string[] array = userName.Split('\\');
                if (array.Length == 2)
                {
                    domain = array[0];
                    loginName = array[1];
                }
                else if (array.Length == 1)
                {
                    loginName = array[0];
                }
            }

            return userName;
        }


        public string GetClientIP()
        {
            if (HttpContext.Current == null)
            {
                return "127.0.0.1";
            }
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public string GetClientComputerName()
        {
            if (HttpContext.Current == null)
            {
                return String.Empty;
            }
            string clientComputerName = HttpContext.Current.Request.ServerVariables["Remote_Host"];

            if (string.IsNullOrWhiteSpace(clientComputerName))
            {
                clientComputerName = String.Empty;
            }
            return clientComputerName;
        }

        public static string GetADDomain()
        {
            foreach (string s in CPConfig.Keystone.SourceDirectories)
            {
                if (s.Trim().ToLower() != CPConfig.Keystone.TrustedDirectory.Trim().ToLower())
                {
                    return s;
                }
            }
            return "";
        }

        public static UserInfo GetUserInfoFromAD(string userID, string domain)
        {
            userID = userID.ToLower().Trim();
            UserInfo user;

            if (!s_UserList.TryGetValue(userID, out user))
            {
                user = new UserInfo();
                user.UserID = userID;

                try
                {
                    DirectorySearcher ds = new DirectorySearcher()
                    {
                        SearchRoot = new DirectoryEntry("LDAP://" + domain),
                        Filter = String.Format("(&(objectCategory=User)(sAMAccountName={0}))", userID)
                    };

                    SearchResult sr = ds.FindOne();

                    if (sr != null)
                    {
                        DirectoryEntry entry = sr.GetDirectoryEntry();

                        if (entry != null)
                        {
                            user.FullName = entry.Properties["givenName"].Value as String;
                            user.EmailAddress = entry.Properties["mail"].Value as String;
                            user.Department = entry.Properties["department"].Value as String;
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    //如果为DB帐号登录，将抛出此COMException,我们让这种异常通过。
                }

                lock (s_UserList)
                {
                    if (!s_UserList.ContainsKey(userID))
                    {
                        s_UserList.Add(userID, user);
                    }
                }
            }
            return user;
    
        }


    }

}
