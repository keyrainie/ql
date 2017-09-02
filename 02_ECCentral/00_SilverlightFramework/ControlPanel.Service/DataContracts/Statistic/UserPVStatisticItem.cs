using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class UserPVStatisticItem
    {
       [DataMapping("UserID", DbType.AnsiString)]
        public string UserId { get; set; }

        [DataMapping("PV", DbType.Int32)]
        public int Pageviews { get; set; }

        [DataMapping("UniquePV", DbType.Int32)]
        public int UniquePageviews { get; set; }
    }
}
