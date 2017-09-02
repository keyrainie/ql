using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class PVStatisticItem
    {
        [DataMapping("Page", DbType.String)]
        public string Page { get; set; }

        [DataMapping("Url", DbType.AnsiString)]
        public string Url { get; set; }

        [DataMapping("PV", DbType.Int32)]
        public int Pageviews { get; set; }

        [DataMapping("UniquePV", DbType.Int32)]
        public int UniquePageviews { get; set; }
    }
}
