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
using ECCentral.QueryFilter.Common;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SODeliveryAssignTaskSearchVM : ModelBase
    {
        public PagingInfo PageInfo { get; set; }

        public SODeliveryAssignTaskSearchVM()
        {
            soSysNos = new List<int>();
        }

        #region 查询条件

        private int? shipType;

        public int? ShipType
        {
            get { return shipType; }
            set { SetValue<int?>("ShipType", ref shipType, value); }
        }

        private int? deliveryMan;

        public int? DeliveryMan
        {
            get { return deliveryMan; }
            set { SetValue<int?>("DeliveryMan", ref deliveryMan, value); }
        }

        private DateTime? deliveryTime = DateTime.Now.Date;
        /// <summary>
        /// 配送日期
        /// </summary>
        public DateTime? DeliveryTime
        {
            get { return deliveryTime; }
            set { SetValue<DateTime?>("DeliveryTime", ref deliveryTime, value); }
        }

        private ECCentral.BizEntity.SO.DeliveryTimeRange? deliveryTimeRange;

        public ECCentral.BizEntity.SO.DeliveryTimeRange? DeliveryTimeRange
        {
            get { return deliveryTimeRange; }
            set { SetValue<ECCentral.BizEntity.SO.DeliveryTimeRange?>("DeliveryTimeRange", ref deliveryTimeRange, value); }
        }

        private DeliveryType? orderType;
        public DeliveryType? OrderType
        {
            get { return orderType; }
            set { SetValue<DeliveryType?>("OrderType", ref orderType, value); } 
        }

        private int? orderSysNo;

        public int? OrderSysNo
        {
            get { return orderSysNo; }
            set { SetValue<int?>("OrderSysNo", ref orderSysNo, value); } 
        }

        private int? area;

        public int? Area
        {
            get { return area; }
            set { SetValue<int?>("Area", ref area, value); } 
        }

        private List<int> soSysNos;

        public List<int> SOSysNos
        {
            get { return soSysNos; }
            set { SetValue<List<int>>("SOSysNos", ref soSysNos, value); } 
        }

        private int? payType;

        public int? PayType
        {
            get { return payType; }
            set { SetValue<int?>("PayType", ref payType, value); } 
        }

        private DateTime? outStockTimeFrom;

        public DateTime? OutStockTimeFrom
        {
            get { return outStockTimeFrom; }
            set { SetValue<DateTime?>("OutStockTimeFrom", ref outStockTimeFrom, value); } 
        }

        private DateTime? outStockTimeTo;

        public DateTime? OutStockTimeTo
        {
            get { return outStockTimeTo; }
            set { SetValue<DateTime?>("OutStockTimeTo", ref outStockTimeTo, value); } 
        }

        private decimal totalAmt;

        public decimal TotalAmt
        {
            get { return totalAmt; }
            set { SetValue<decimal>("TotalAmt", ref totalAmt, value); } 
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue<string>("CompanyCode", ref companyCode, value); } 
        }

        #endregion

        #region 查询条件绑定数据源

        private List<KeyValuePair<DeliveryType?, string>> orderTypeList;
        /// <summary>
        /// 单据类型
        /// </summary>
        public List<KeyValuePair<DeliveryType?, string>> OrderTypeList
        {
            get
            {
                orderTypeList = orderTypeList ?? EnumConverter.GetKeyValuePairs<DeliveryType>(EnumConverter.EnumAppendItemType.None);
                return orderTypeList;
            }
        }

        private List<KeyValuePair<ECCentral.BizEntity.SO.DeliveryTimeRange?, string>> deliveryTimeRangeTypeList;
        /// <summary>
        /// 送货时段
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.SO.DeliveryTimeRange?, string>> DeliveryTimeRangeTypeList
        {
            get
            {
                deliveryTimeRangeTypeList = deliveryTimeRangeTypeList ?? EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.SO.DeliveryTimeRange>(EnumConverter.EnumAppendItemType.All);
                return deliveryTimeRangeTypeList;
            }
        }

        private List<UserInfo> freightMenList;
        /// <summary>
        /// 投递员列表
        /// </summary>
        public List<UserInfo> FreightMenList
        {
            get
            {
                return freightMenList;
            }
            set
            {
                SetValue<List<UserInfo>>("FreightMenList", ref freightMenList, value);
            }
        }

        private List<KeyValuePair<string, string>> deliveryTimeRangeBeginList;  
        /// <summary>
        /// 出库时间范围开始
        /// </summary>
        public List<KeyValuePair<string, string>> DeliveryTimeRangeBeginList
        {
            get
            {
                if(deliveryTimeRangeBeginList == null)
                {
                    deliveryTimeRangeBeginList = new List<KeyValuePair<string,string>>();
                    deliveryTimeRangeBeginList.Add(new KeyValuePair<string,string>("00:00", "上午(00:00)"));
                    deliveryTimeRangeBeginList.Add(new KeyValuePair<string,string>("08:00", "上午(08:00)"));
                    deliveryTimeRangeBeginList.Add(new KeyValuePair<string,string>("12:00", "下午(12:00)"));
                }
                return deliveryTimeRangeBeginList;
            }
        }

        private List<KeyValuePair<string, string>> deliveryTimeRangeEndList;  
        /// <summary>
        /// 出库时间范围截至
        /// </summary>
        public List<KeyValuePair<string, string>> DeliveryTimeRangeEndList
        {
            get
            {
                if(deliveryTimeRangeEndList == null)
                {
                    deliveryTimeRangeEndList = new List<KeyValuePair<string,string>>();
                    deliveryTimeRangeEndList.Add(new KeyValuePair<string,string>("11:59", "上午(11:59)"));
                    deliveryTimeRangeEndList.Add(new KeyValuePair<string,string>("23:59", "下午(23:59)"));
                    deliveryTimeRangeEndList.Add(new KeyValuePair<string,string>("00:00", "上午(00:00)"));
                }
                return deliveryTimeRangeEndList;
            }
        }

        #endregion
    }
}
