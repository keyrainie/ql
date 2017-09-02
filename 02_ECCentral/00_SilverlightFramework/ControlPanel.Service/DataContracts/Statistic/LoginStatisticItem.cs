using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class LoginStatisticItem
    {
        private DateTime m_InDate;    

        [DataMapping("Count", DbType.Int32)]
        public int Count { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate 
        {
            get
            {
                return DateTime.SpecifyKind(this.m_InDate, DateTimeKind.Utc);
            }
            set
            {
                this.m_InDate = value;
            }
        }
    }
}
