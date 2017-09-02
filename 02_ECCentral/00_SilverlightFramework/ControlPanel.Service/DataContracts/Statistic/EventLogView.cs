using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class EventLogView : EventLog
    {
        [DataMapping("Page", DbType.String)]
        public string Page { get; set; }

        public string EventDateForUI 
        {
            get 
            {
                return this.EventDate.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    EventDate = Convert.ToDateTime(value);
                }
            }
        }
    }
}
