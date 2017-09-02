using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility.WCF
{
    public interface IConvert
    {
        object Convert(object data, Type dataType, int rowIndex, int colsIndex);
    }
}
