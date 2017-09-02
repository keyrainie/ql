using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    [Serializable]
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class NoProfilerTraceAttribute : Attribute
    {

    }
}