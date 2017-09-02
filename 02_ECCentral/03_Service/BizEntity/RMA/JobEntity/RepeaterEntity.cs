using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    public class RepeaterEntity
    {
        public int SOSysNo
        {
            get;
            set;
        }

        public int ProductSysNo
        {
            get;
            set;
        }

        public int RequestSysNo
        {
            get;
            set;
        }

        public int RegisterSysNo
        {
            get;
            set;
        }

        public int? RepeaterSysNo
        {
            get;
            set;
        }

        public int RepeatCount
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }

    public class RepeaterHistoryData
    {
        public DateTime? HistoryDataBeginDate
        {
            get;
            set;
        }

        public DateTime? HistoryDataEndDate
        {
            get;
            set;
        }

        public int? LastTransformMonth
        {
            get;
            set;
        }

        public int? FirstRun
        {
            get;
            set;
        }
    }
}
