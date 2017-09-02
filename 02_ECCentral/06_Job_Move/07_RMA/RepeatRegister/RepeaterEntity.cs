using System;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ServiceMgmt.Job.CalculateRepeat
{
    public class RepeaterEntity
    {
        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo
        {
            get;set;
        }

        [DataMapping("ProductSysNo", DbType.Int32)]
		public int ProductSysNo
        {
            get;set;
        }

        [DataMapping("RequestSysNo", DbType.Int32)]
        public int RequestSysNo
        {
            get;set;
        }

        [DataMapping("RegisterSysNo", DbType.Int32)]
		public int RegisterSysNo
        {
            get;set;
        }

        [DataMapping("RepeaterSysNo", DbType.Int32)]
		public int? RepeaterSysNo
        {
            get;set;
        }

        public int RepeatCount
        {
            get;
            set;
        }

        [DataMapping("Quantity", DbType.Int32)]
        public int RepeatProductCount
        {
            get;
            set;
        }
    }

    public class RepeaterHistoryData
    {
        [DataMapping("HistoryDataBeginDate", DbType.DateTime)]
        public DateTime? HistoryDataBeginDate
        {
            get;
            set;
        }


        [DataMapping("HistoryDataEndDate", DbType.DateTime)]
        public DateTime? HistoryDataEndDate
        {
            get;
            set;
        }

        [DataMapping("LastTransformMonth", DbType.Int32)]
        public int? LastTransformMonth
        {
            get;
            set;
        }

        [DataMapping("FirstRun", DbType.Int32)]
        public int? FirstRun
        {
            get;
            set;
        }
    }
}