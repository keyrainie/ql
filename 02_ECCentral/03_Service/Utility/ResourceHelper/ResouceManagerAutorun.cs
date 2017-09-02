using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public class ResouceManagerAutorun : IStartup, IShutdown
    {
        public void Start()
        {
            ResouceFileWatcher.BeginWatch();
        }

        public void Shut()
        {
            ResouceFileWatcher.EndWatch();
        }
    }
}
