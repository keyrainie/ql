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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.SO;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SODeliveryDiffSearchVM:ModelBase
    {

        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        #region 查询条件


        private int? soSysNo;
        public int? SOSysNo
        {
            get { return soSysNo;}
            set { SetValue<int?>("SOSysNo", ref soSysNo, value); }
        }

        private SOStatus? soStatus;
        public SOStatus? SOStatus
        {
            get { return soStatus; }
            set { SetValue<SOStatus?>("SOStatus", ref soStatus, value); }
        }


        private DateTime? deliveryDateTimeFrom;
        public DateTime? DeliveryDateTimeFrom
        {
            get { return deliveryDateTimeFrom; }
            set { SetValue<DateTime?>("DeliveryDateTimeFrom", ref deliveryDateTimeFrom, value); }
        }

        private DateTime? deliveryDateTimeTo;
        public DateTime? DeliveryDateTimeTo
        {
            get { return deliveryDateTimeTo; }
            set { SetValue<DateTime?>("DeliveryDateTimeTo", ref deliveryDateTimeTo, value); }
        }

        private int? feightMen;
        public int? FreightMen
        {
            get { return feightMen; }
            set { SetValue<int?>("FreightMen", ref feightMen,value); }
        }


        private int? deliveryAreaNo;
        public int? DeliveryAreaNo
        {
            get { return deliveryAreaNo; }
            set { SetValue<int?>("DeliveryAreaNo",ref deliveryAreaNo, value);}
        }


       
        #endregion

    }

    public class SODeliveryDiffSearchDataVM : ModelBase
    {
        public string FreightMan { get; set; }

        public int? SOSysNo { get; set; }

        public SOStatus? SOStatus { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceiveDistrict { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? DeliveryDepartment { get; set; }

        public decimal? Weight { get; set; }

        public string DeliveryMemo { get; set; }

        public string Memo { get; set; }

        public int? DeliveryStatus { get; set; }

    }


    public class SODeliveryDiffQueryView : ModelBase
    {
        public SODeliveryDiffSearchVM QueryInfo { get; set; }
        private List<SODeliveryDiffSearchDataVM> result;
        public List<SODeliveryDiffSearchDataVM> Result
        {
            get { return result; }
            set { SetValue<List<SODeliveryDiffSearchDataVM>>("Result", ref result, value); }
        }
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set { SetValue<int>("TotalCount", ref totalCount, value); }
        }

        public SODeliveryDiffQueryView()
        {
            QueryInfo = new SODeliveryDiffSearchVM();
        }
    }
}
