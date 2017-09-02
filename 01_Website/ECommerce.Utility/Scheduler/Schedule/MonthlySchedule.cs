using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Utility
{
    [Serializable]
    [DataContract]
    public abstract class MonthlySchedule : RepeatlySchedule
    {
        private int m_IntervalMonths;

        [DataMember]
        public int IntervalMonths
        {
            get { return m_IntervalMonths; }
            set { m_IntervalMonths = value; }
        }

        private bool IsMonthValid(DateTime time)
        {
            int diff = (time.Year - FromDate.Year) * 12 + (time.Month - FromDate.Month);
            return (diff % m_IntervalMonths) == 0;
        }

        protected override bool IsValid(DateTime time)
        {
            return base.IsValid(time) && IsMonthValid(time);
        }

        protected override DateTime SubtractInterval(DateTime time)
        {
            return time.AddMonths(-1 * m_IntervalMonths);
        }
    }
}
