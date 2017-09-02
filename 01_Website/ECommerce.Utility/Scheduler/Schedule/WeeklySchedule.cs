using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Utility
{
    [Serializable]
    [DataContract]
    public class WeeklySchedule : RepeatlySchedule
    {
        private int m_IntervalWeeks;

        [DataMember]
        public int IntervalWeeks
        {
            get { return m_IntervalWeeks; }
            set { m_IntervalWeeks = value; }
        }

        private DayOfWeek m_ExecuteWeekDay;

        [DataMember]
        public DayOfWeek ExecuteWeekDay
        {
            get { return m_ExecuteWeekDay; }
            set { m_ExecuteWeekDay = value; }
        }

        private bool IsWeekValid(DateTime time)
        {
            DateTime fromDate = FromDate.AddDays(DayOfWeek.Sunday - FromDate.DayOfWeek).Date;
            DateTime nowDate = time.AddDays(DayOfWeek.Sunday - time.DayOfWeek).Date;
            int diff = nowDate.Subtract(fromDate).Days;
            return (diff % (m_IntervalWeeks * 7)) == 0;
        }

        private bool IsWeekDayValid(DateTime time)
        {
            return (time.DayOfWeek == m_ExecuteWeekDay);
        }

        protected override bool IsValid(DateTime time)
        {
            return base.IsValid(time) && IsWeekValid(time) && IsWeekDayValid(time);
        }

        public override DateTime? GetNextExecuteTimeAfterSpecificTime(DateTime time)
        {
            if (time > ToDate)
            {
                return default(DateTime?);
            }
            if (time < FromDate)
            {
                time = FromDate;
            }
            DateTime fromDate = FromDate.AddDays(DayOfWeek.Sunday - FromDate.DayOfWeek).Date;
            DateTime nowDate = time.AddDays(DayOfWeek.Sunday - time.DayOfWeek).Date;
            int diff = nowDate.Subtract(fromDate).Days;
            int x = diff % (m_IntervalWeeks * 7);
            DateTime rst;
            if (x != 0)
            {
                rst = nowDate.AddDays(m_IntervalWeeks * 7 - x);
            }
            else
            {
                rst = nowDate;
            }
            rst.AddDays(ExecuteWeekDay - rst.DayOfWeek);
            rst = new DateTime(rst.Year, rst.Month, rst.Day).Add(TimeSpan.Parse(ExecuteTime));
            if (rst < time)
            {
                rst = rst.AddDays(m_IntervalWeeks * 7);
            }
            return rst;
        }

        protected override DateTime SubtractInterval(DateTime time)
        {
            return time.AddDays(-1 * 7 * m_IntervalWeeks);
        }

        public override string ScheduleTypeName
        {
            get { return "Weekly Schedule"; }
        }
    }
}
