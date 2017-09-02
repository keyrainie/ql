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
    public class DataMonitorService : IDataMonitor
    {
        public void PersistDataLog(string dataLog)
        {
            new DataMonitorDA().InsertDataLog(dataLog);
        }
    }
}
