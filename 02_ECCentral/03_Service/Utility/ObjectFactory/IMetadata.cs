using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public interface IMetadata
    {
        string Version { get; }

        string[] Filter { get; }
    }
}
