using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface ICommonServiceV40
    {

        [OperationContract]
        AppVersionV40 CheckAppVersion();

        [OperationContract]
        XapVersionV40 GetFrameworkVersion();

        [OperationContract]
        AppParamsV41 GetAppParams();
    }
}
