﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Utility.Scheduler
{
    public class Scheduler
    {
        private ScheduleList m_ScheduleList;

        public Scheduler(ScheduleList scheduleList)
        {
            m_ScheduleList = scheduleList;
        }

        public ScheduleActionEnum GetAction(DateTime lastRunTime, DateTime specifiedTime, out DateTime? nextRunTime)
        {
            if (m_ScheduleList == null || m_ScheduleList.Schedules == null || m_ScheduleList.Schedules.Count <= 0)
            {
                nextRunTime = new DateTime?();
                return ScheduleActionEnum.End;
            }

            nextRunTime = m_ScheduleList.GetNextExecuteLocalTime(specifiedTime);
            if (lastRunTime.AddMilliseconds(Schedule.DisparityMilliseconds) > specifiedTime)
            {
                return ScheduleActionEnum.Wait;
            }
            if (m_ScheduleList.CanRun(specifiedTime))
            {
                //if (nextRunTime.HasValue)
                //{
                //    nextRunTime = m_ScheduleList.GetNextExecuteLocalTime(nextRunTime.Value);
                //}
                return ScheduleActionEnum.Run;
            }

            if (nextRunTime.HasValue)
            {
                return ScheduleActionEnum.Wait;
            }

            return ScheduleActionEnum.End;
        }
    }
}
