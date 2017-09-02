using System;
using System.Collections.Generic;

using System.Text;
using System.Messaging;


namespace ECCentral.Job.Utility
{
    public interface ILogEmitter
    {
        void Init(string configParam);

        void EmitLog(LogEntry log);
    }
}
