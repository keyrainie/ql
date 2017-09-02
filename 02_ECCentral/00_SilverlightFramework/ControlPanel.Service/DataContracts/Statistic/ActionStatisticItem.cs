using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class ActionStatisticItem
    {
        [DataMapping("Action", DbType.String)]
        public string Action { get; set; }

        [DataMapping("Label", DbType.String)]
        public string Label { get; set; }

        [DataMapping("Count", DbType.Int32)]
        public int Count { get; set; }
    }
}
