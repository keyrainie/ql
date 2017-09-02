using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Report;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    public class ImportTrackingNumberResp
    {
        public List<TrackingNumberInfo> SuccessList
        {
            get;
            set;
        }

        public List<TrackingNumberInfo> FailedList
        {
            get;
            set;
        }
    }
}