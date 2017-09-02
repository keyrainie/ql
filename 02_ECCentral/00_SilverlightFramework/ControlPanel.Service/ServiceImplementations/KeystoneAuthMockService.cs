using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [InternationalBehavior]
    [ServiceErrorHandling]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class KeystoneAuthMockService : IKeystoneAuthV41
    {
        #region IKeystoneAuthV41 Members

        public KeystoneAuthDataV41 GetAuthData(DefaultDataContract msg)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockDataForKeystoneAuth.xml");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            KeystoneAuthDataV41 serviceResult = SerializationUtility.LoadFromXml<KeystoneAuthDataV41>(filePath);

            return serviceResult;
        }

        public AuthUserListV41 GetAuthUserByRoleName(AuthUserQueryV41 msg)
        {
            return null;
        }

        public AuthUserListV41 GetAuthUserByFunctionName(AuthUserQueryV41 msg)
        {
            return null;
        }

        #endregion


        public KeystoneAuthDataV41 Login(KeystoneAuthUserV41 msg)
        {
            return GetAuthData(null);
        }

        public KeystoneAuthDataV41 AutoLogin(DefaultDataContract msg)
        {
            return GetAuthData(null);
        }

        public DefaultDataContract Logout()
        {
            return new DefaultDataContract();
        }

        public KeystoneAuthUserListV41 BatchGetUserInfo(List<string> userIDList)
        {
            return new KeystoneAuthService().BatchGetUserInfo(userIDList);
        }
    }
}
