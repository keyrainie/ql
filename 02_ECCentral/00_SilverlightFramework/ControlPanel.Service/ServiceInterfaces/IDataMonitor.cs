using System;
using System.ServiceModel;

using Newegg.Oversea.Framework.Contract;

using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface IDataMonitor
    {
        [OperationContract(IsOneWay=true)]
        void PersistDataLog(string dataLog);
    }
}
