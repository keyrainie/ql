using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage
{
    public class ThirdPartBizException : Exception
    {
        public ThirdPartBizException(string message) : base(message)
        {

        }
    }
}
