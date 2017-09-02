using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity
{
    public class BizException : Exception
    {
        public BizException(string message) : base(message)
        {

        }
    }
}
