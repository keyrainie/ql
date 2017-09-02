using System;
using System.Collections.Generic;

namespace ECommerce.Utility
{
    public interface ILogEmitter
    {
        void Init(Dictionary<string, string> param);

        void EmitLog(LogEntry log);
    }
}
