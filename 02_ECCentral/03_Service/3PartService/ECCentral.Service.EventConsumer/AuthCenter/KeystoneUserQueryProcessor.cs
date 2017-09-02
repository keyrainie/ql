using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.AuthCenter;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using System.ServiceModel;
using Newegg.Oversea.AuthCenter.ServiceInterface.ServiceContracts;
using Newegg.Oversea.AuthCenter.ServiceInterface.DataContracts;
using Newegg.Oversea.Framework.Contract;
using Newegg.AuthCenter.AuthCenterForECC.AuthCenterService;
using System.Configuration;

namespace ECCentral.Service.EventConsumer.AuthCenter
{
    public class KeystoneUserQueryProcessor:IConsumer<KeystoneUserQueryMessage>
    {
        public void HandleEvent(KeystoneUserQueryMessage eventMessage)
        {
            //IPP服务已弃用，改为AuthCenter Service 
            //IUserServiceV30 service = WCFAdapter<IUserServiceV30>.GetProxy();
            //QueryResultContract<List<KeystoneUserMsg>> result = service.LookupUsers(new KeystoneUserQueryCriteriaV30()
            //{
            //    LoginName = eventMessage.LoginName
            //    ,
            //    Header = new Newegg.Oversea.Framework.Contract.MessageHeader() { CompanyCode = "8601" }
            //    ,
            //    PagingInfo = new Newegg.Oversea.Framework.Contract.PagingInfo() { PageSize = int.MaxValue, StartRowIndex = 0 }
            //});

            //eventMessage.Result = result.ResultList;

            List<KeystoneUserMsg> userList = null;

            AuthServiceSoapClient auth = new AuthServiceSoapClient();
            var queryResult = auth.TrustedLogin(eventMessage.LoginName);
            if (queryResult != null)
            {
                userList = new List<KeystoneUserMsg>();
                userList.Add(new KeystoneUserMsg
                {
                    SourceDirectory = ConfigurationManager.AppSettings["SourceDirectory"] ?? "",
                    PhysicalUserId = Guid.NewGuid().ToString("D"),
                    PhysicalUserName = queryResult.LoginName,
                    LogicFirstName = queryResult.DisplayName,
                    LogicUserId = Guid.NewGuid().ToString("D")
                });
            }
            eventMessage.Result = userList;
        }

        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

}
