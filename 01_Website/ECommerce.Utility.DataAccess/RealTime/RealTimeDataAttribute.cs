using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;

namespace Nesoft.Utility.DataAccess.RealTime
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RealTimeDataAttribute : Attribute
    {
        private BusinessDataType dataType;

        public RealTimeDataAttribute(BusinessDataType dataType)
        {
            this.dataType = dataType;
        }
    }
}
