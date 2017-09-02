using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Threading;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class DateTimeHelper
    {
        #region 工作时间的计算

        private static TimeSpan WorkStart = new TimeSpan(8, 30, 0);
        private static TimeSpan WorkEnd = new TimeSpan(17, 30, 0);

        public static DateTime AddWorkMinute(DateTime time, double mins)
        {
            if (mins == 0)
                return time;

            DateTime currentDay = DateTime.Parse(time.ToString("yyyy-MM-dd"));

            // 判断传入的起始时间是否是工作日
            if (time.DayOfWeek == DayOfWeek.Saturday || // 如果是周六
                time.DayOfWeek == DayOfWeek.Sunday) // 如果是周日
            {
                // 如果是加就往前推一天，否则往后倒一天
                time = (mins > 0) ? currentDay.AddDays(1) : currentDay.AddSeconds(-1);
                return AddWorkMinute(time, mins);
            }

            // 计算当前日期的工作时间
            double tempMin = 0;
            if (mins > 0)
            {
                tempMin = CalWorkMinute(time, currentDay.Add(WorkEnd), 0);
            }
            else
            {
                tempMin = CalWorkMinute(currentDay.Add(WorkStart), time, 0);
            }

            if (Math.Abs(mins) > tempMin) // 当天工作时间不能满足
            {
                time = (mins > 0) ? currentDay.AddDays(1) : currentDay.AddSeconds(-1);
                return AddWorkMinute(time, Math.Sign(mins) * (Math.Abs(mins) - tempMin));
            }
            else // 当天时间可以满足
            {
                if (mins > 0)
                {
                    if (time.TimeOfDay > WorkStart) //工作时间之前
                    {
                        return time.AddMinutes(mins);
                    }
                    else
                    {
                        return currentDay.Add(WorkStart).AddMinutes(mins);
                    }
                }
                else
                {
                    if (time.TimeOfDay < WorkEnd) //工作时间以后
                    {
                        return time.AddMinutes(mins);
                    }
                    else
                    {
                        return currentDay.Add(WorkEnd).AddMinutes(mins);
                    }
                }
            }
        }

        /// <summary>
        /// 计算传入的时间段之间有多少时间属于工作时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="totalMinutes"></param>
        /// <returns></returns>
        public static double CalWorkMinute(DateTime startTime, DateTime endTime, double totalMinutes)
        {
            // 开始时间>=终止时间
            if (startTime >= endTime)
                return totalMinutes;

            // 判断传入的开始时间是否是工作日
            if (startTime.DayOfWeek == DayOfWeek.Saturday || // 如果是周六
                startTime.DayOfWeek == DayOfWeek.Sunday) // 如果是周日
            {
                startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
                return CalWorkMinute(startTime, endTime, totalMinutes);
            }

            TimeSpan dayStartTime = startTime.TimeOfDay;
            TimeSpan dayEndTime = endTime.TimeOfDay;
            if (startTime.ToString("yyyyMMdd") != endTime.ToString("yyyyMMdd")) // 开始时间和结束时间不是同一天
            {
                dayEndTime = WorkEnd;
            }

            if (dayStartTime > WorkEnd) // 开始时间在下班以后
            {
                startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
                return CalWorkMinute(startTime, endTime, totalMinutes);
            }

            double allMinutes = ((TimeSpan)(WorkEnd - WorkStart)).TotalMinutes;
            double beforeStartMinutes = ((TimeSpan)(dayStartTime - WorkStart)).TotalMinutes;
            double aboveEndMinutes = ((TimeSpan)(WorkEnd - dayEndTime)).TotalMinutes;

            double dayMinutes = allMinutes - Math.Max(Math.Min(beforeStartMinutes, allMinutes), 0) -
                                              Math.Max(Math.Min(aboveEndMinutes, allMinutes), 0);

            totalMinutes += dayMinutes;

            startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd")).AddDays(1);
            return CalWorkMinute(startTime, endTime, totalMinutes);
        }

        #endregion 工作时间的计算

        public static DateTime GetServerTimeNowSync(string domainName)
        {
            string relativeUrl = "/GetServerTimeNow";
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");

            string error = null;
            DateTime t = default(DateTime);
            ManualResetEvent wait = new ManualResetEvent(false); 
            restClient.Query<DateTime>(relativeUrl, (s, e) =>
            {
                if (e.Error != null)
                {
                    bool isBizException = true;
                    error = e.GetError(ref isBizException);
                }
                else
                {
                    error = null;
                    t = e.Result;
                }
                wait.Set();
            });
            wait.WaitOne();

            if (error != null)
            {
                throw new Exception(error);
            }
            return t;
        }

        public static void GetServerTimeNow(string domainName, Action<DateTime> callback)
        {
            string relativeUrl = "/GetServerTimeNow";
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");

            restClient.Query<DateTime>(relativeUrl, (s, e) =>
                {
                    if (!e.FaultsHandle())
                    {
                        callback(e.Result);
                    }
                });
        }
    }
}
