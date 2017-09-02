using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Utility
{
    [Serializable]
    [DataContract]
    public class OneTimeSchedule : Schedule
    {
        private DateTime m_ExecuteDateTime;

        [DataMember]
        public DateTime ExecuteDateTime
        {
            get { return m_ExecuteDateTime; }
            set { m_ExecuteDateTime = value; }
        }

        public override bool Check(DateTime dateTime)
        {
            DateTime newTime = GetSpecialTimeZoneTime(dateTime);
            return (newTime >= ExecuteDateTime && newTime < ExecuteDateTime.AddMilliseconds(DisparityMilliseconds));
        }

        public override DateTime? GetNextExecuteTimeAfterSpecificTime(DateTime time)
        {
            if (time > m_ExecuteDateTime)
            {
                return default(DateTime?);
            }
            return m_ExecuteDateTime;
        }

        public override DateTime? GetLastExecuteTimeAfterSpecificTime(DateTime time)
        {
            if (time < m_ExecuteDateTime)
            {
                return default(DateTime?);
            }
            return m_ExecuteDateTime;
        }

        public override string ScheduleTypeName
        {
            get { return "One Time Schedule"; }
        }
    }
}
