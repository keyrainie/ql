using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class EventLog
    {
        [DataMapping("EventLogID", DbType.String)]
        public string EventLogID { get; set; }

        [DataMapping("UserID", DbType.AnsiString)]
        public string UserID { get; set; }

        [DataMapping("IP", DbType.AnsiString)]
        public string IP { get; set; }

        [DataMapping("EventDate", DbType.DateTime)]
        public DateTime EventDate { get; set; }

        [DataMapping("Url", DbType.AnsiString)]
        public string Url { get; set; }

        [DataMapping("Action", DbType.String)]
        public string Action { get; set; }

        [DataMapping("Label", DbType.String)]
        public string Label { get; set; }
    }
}
