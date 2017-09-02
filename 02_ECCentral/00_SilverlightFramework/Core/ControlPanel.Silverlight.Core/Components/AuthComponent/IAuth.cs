using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface IAuth : IComponent
    {
        void LoadAuthData(LoadAuthDataCompletedCallback callback);

        ObservableCollection<AuthMenuItem> GetAuthorizedMenuItems();

        ObservableCollection<AuthMenuItem> GetAuthorizedNavigateItems();

        ObservableCollection<AuthMenuItem> AuthorizedNavigateToList();

        bool HasFunction(string functionKey);

        //[Obsolete("该方法已经过期，请使用新的HasFunctionByAppName(string key,string appName)来替代该方法。")]
        //bool HasFunction(string functionKey, string applicationId);

        //bool HasFunctionByAppName(string functionKey, string appName);

        //bool HasRole(string roleName, string appName);

        bool HasFunctionForPage(string url);

        /// <summary>
        /// get key attribute of current login user by specific application id.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="ApplicationIds"></param>
        /// <returns></returns>
        //ObservableCollection<RoleAttribute> GetAttributesByRoleName(string roleName, string ApplicationId);


        /// <summary>
        /// get the keystone auth user by role name and application id.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        //void GetAuthUserByRoleName(string roleName, string applicationId, Action<ObservableCollection<KeystoneAuthUser>> callback);


        /// <summary>
        /// get the keystone auth user by function names and application id.
        /// </summary>
        /// <param name="functionNames"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        //void GetAuthUserByFunctionName(List<string> functionNames, string applicationId, Action<ObservableCollection<KeystoneAuthUser>> callback);


        //void GetAuthUserInfo(List<string> userIDList, Action<ObservableCollection<AuthUser>> callback);
    }

    public delegate void LoadAuthDataCompletedCallback();
}
