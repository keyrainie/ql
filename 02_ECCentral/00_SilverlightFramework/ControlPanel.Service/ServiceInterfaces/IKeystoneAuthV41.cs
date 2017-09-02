using System;
using System.ServiceModel;

using Newegg.Oversea.Framework.Contract;

using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface IKeystoneAuthV41
    {
        [OperationContract]
        KeystoneAuthDataV41 GetAuthData(DefaultDataContract msg);

        [OperationContract]
        AuthUserListV41 GetAuthUserByRoleName(AuthUserQueryV41 msg);

        [OperationContract]
        AuthUserListV41 GetAuthUserByFunctionName(AuthUserQueryV41 msg);

        [OperationContract]
        DefaultDataContract Logout();

        [OperationContract]
        KeystoneAuthDataV41 Login(KeystoneAuthUserV41 msg);

        [OperationContract]
        KeystoneAuthDataV41 AutoLogin(DefaultDataContract msg);

        [OperationContract]
        KeystoneAuthUserListV41 BatchGetUserInfo(List<string> userIDList);
    }
}
