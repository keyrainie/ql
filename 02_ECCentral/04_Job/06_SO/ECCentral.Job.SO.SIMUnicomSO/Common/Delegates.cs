using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Job.SO.SIMUnicomSO.Logic;

namespace ECCentral.Job.SO.SIMUnicomSO.Common
{
    public delegate void SendMailDelegate<T>(T source);

    public delegate void OnSendMailDelegate(object sender, OnSendingMailArgs args);

    public delegate void SendMailCallback(OnSendingMailArgs args);

    public delegate void OnSendMailExceptionHandle(Exception ex, OnSendingMailArgs args);

    public class OnSendingMailArgs : EventArgs
    {
        public SOSIMCardEntity SIMCardEntity { get; set; }
    }
}
