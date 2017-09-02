using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ISysConfigDA
    {
        void Update(string key, string value, string channelID);
    }
}
