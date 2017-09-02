using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;

namespace IPP.OrderMgmt.JobV31
{
    public class DeliveryHelpter
    {






    }


    public class DeliveryIteration
    {
        //隔日配送
        private int IntervalDays { get; set; }

        //迭代开始时间
        private DateTime StartTime { get; set; }

        //配送方式对应的配送类型
        private int OriginalDeliveryType { get; set; }

        //开始时间这天的配送类型 
        private int DeliveryType { get; set; }

        //节假日信息
        private List<HolidayEntity> Holidays { get; set; }

        //配送方式的时间节点
        private List<TimeSpan> TimePoints { get; set; }

        //备货需要的周期
        private int? PrepareSections { get; set; }

        //这天的周期
        private int Sections
        {
            get
            {
                if (DeliveryType == 2)
                    return 2;
                else if (DeliveryType == 1)
                    return 1;
                else
                    return 0;
            }
        }

        //最早配送日
        public DateTime LatestDate { get; set; }
        //最早配送周期
        public int FinalSection { get; set; }

        public DeliveryIteration(DateTime startTime, int deliveryType, List<HolidayEntity> holidays, List<TimeSpan> timePoints, int? prepareSections)
            : this(startTime, deliveryType, holidays, timePoints, 0, prepareSections)
        {
        }

        public DeliveryIteration(DateTime startTime, int deliveryType, List<HolidayEntity> holidays, List<TimeSpan> timePoints, int IntervalDays)
            : this(startTime, deliveryType, holidays, timePoints, IntervalDays, null)
        { }


        public DeliveryIteration(DateTime startTime, int deliveryType, List<HolidayEntity> holidays, List<TimeSpan> timePoints, int IntervalDays, int? prepareSections)
        {
            this.StartTime = startTime;
            this.OriginalDeliveryType = deliveryType;
            this.Holidays = holidays;
            this.IntervalDays = IntervalDays;
            this.PrepareSections = prepareSections;
            this.TimePoints = timePoints;
        }



        //重置属性
        private void ReSetProperty(DateTime startTime, int IntervalDays, int? prepareSections)
        {
            this.StartTime = startTime;
            this.IntervalDays = IntervalDays;
            this.PrepareSections = prepareSections;
        }


        public void Roll()
        {
            //获取这天的配送类型
            GetDeliveryType();

            //获取备货周期
            if (PrepareSections == null)
                GetPrepareSections();

            //隔日配送
            if (IntervalDays > 0)
            {
                DateTime newDate = StartTime.AddDays(IntervalDays);
                int presc = PrepareSections.HasValue ? PrepareSections.Value : 0;//?
                //重置属性
                ReSetProperty(newDate, 0, presc);
                //递归
                Roll();
            }
            else
            {
                int secs = PrepareSections.Value - Sections;

                if (secs > 0)
                {

                    ReSetProperty(StartTime.AddDays(1), 0, secs);
                    //递归
                    Roll();
                }
                else
                {
                    LatestDate = StartTime.Date;
                    FinalSection = DeliveryType == 2 ? PrepareSections.Value : 0;
                }
            }
        }



        //获取这天的配送类型
        public void GetDeliveryType()
        {

            if (OriginalDeliveryType == 1)
            {
                //检查节假日
                HolidayEntity hd = Holidays.Find(x => x.HolidayDate == StartTime.Date);
                DeliveryType = hd == null ? 1 : hd.DeliveryType;
            }
            else if (OriginalDeliveryType == 2)
            {
                //检查节假日和星期日
                DeliveryType = StartTime.DayOfWeek == DayOfWeek.Sunday ? 1 : 2;

                //检查节假日
                HolidayEntity hd = Holidays.Find(x => x.HolidayDate.Date == StartTime.Date);
                if (hd != null)
                    DeliveryType = hd.DeliveryType;
            }



        }

        //获取备货周期
        public void GetPrepareSections()
        {
            if (DeliveryType == 1)
            {

                List<TimeSpan> points = new List<TimeSpan>();

                if (TimePoints.Count > 1)
                    points.Add(TimePoints[1]);
                else
                    points.Add(TimePoints[0]);

                GetType1PrepareSections(points, StartTime.TimeOfDay);

            }
            else if (DeliveryType == 2)
            {
                GetType2PrepareSections(TimePoints, StartTime.TimeOfDay);
            }

        }

        //获取备货需跨越的Section
        public void GetType1PrepareSections(List<TimeSpan> times, TimeSpan auditTime)
        {
            if (auditTime < times[0])
            {
                PrepareSections = 2;
            }
            else if (times[0] <= auditTime)
            {
                PrepareSections = 3;
            }
        }

        //获取备货需跨越的Section
        public void GetType2PrepareSections(List<TimeSpan> times, TimeSpan auditTime)
        {

            if (auditTime < times[0])
            {
                PrepareSections = 2;
            }
            else if (times[0] <= auditTime && auditTime < times[1])
            {
                PrepareSections = 3;
            }
            else if (times[1] <= auditTime)
            {
                PrepareSections = 4;
            }

        }





    }

}
