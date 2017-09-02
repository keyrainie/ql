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
using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SODeliveryHistorySearchVM : ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        #region 查询条件

        private DeliveryType? orderType;
        /// <summary>
        /// 单据类型
        /// </summary>
        public DeliveryType? OrderType
        {
            get 
            {
                return orderType.HasValue && orderType.Value == DeliveryType.ALL ? null : orderType;
            }
            set { SetValue<DeliveryType?>("OrderType", ref orderType, value); }
        }

        private string districtName;
        /// <summary>
        /// 
        /// </summary>
        public string DistrictName
        {
            get { return districtName; }
            set { SetValue<string>("DistrictName", ref districtName, value); }
        }

        private int? areaSysNo;
        /// <summary>
        /// 送货区域
        /// </summary>
        public int? AreaSysNo
        {
            get { return areaSysNo; }
            set { SetValue<int?>("AreaSysNo", ref areaSysNo, value); }
        }

        private DateTime? deliveryDateFrom = DateTime.Now.Date.AddMonths(-1);
        /// <summary>
        /// 配送日期（从）
        /// </summary>
        public DateTime? DeliveryDateFrom
        {
            get { return deliveryDateFrom; }
            set { SetValue<DateTime?>("DeliveryDateFrom", ref deliveryDateFrom, value); }
        }

        private DateTime? deliveryDateTo = DateTime.Now.Date;
        /// <summary>
        /// 配送日期（至）
        /// </summary>
        public DateTime? DeliveryDateTo
        {
            get { return deliveryDateTo; }
            set { SetValue<DateTime?>("DeliveryDateTo", ref deliveryDateTo, value); }
        }

        private int? orderSysNo;
        /// <summary>
        /// 单据号码
        /// </summary>
        public int? OrderSysNo
        {
            get { return orderSysNo; }
            set { SetValue<int?>("OrderSysNo", ref orderSysNo, value); }
        }

        private DeliveryStatus? status;
        /// <summary>
        /// 配送状态
        /// </summary>
        public DeliveryStatus? Status
        {
            get { return status; }
            set { SetValue<DeliveryStatus?>("Status", ref status, value); }
        }

        private int? deliveryManUserSysNo;
        /// <summary>
        /// 配送员
        /// </summary>
        public int? DeliveryManUserSysNo
        {
            get { return deliveryManUserSysNo; }
            set { SetValue<int?>("DeliveryManUserSysNo", ref deliveryManUserSysNo, value); }
        }

        private String companyCode;
        public String CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
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

        private List<KeyValuePair<DeliveryStatus?, string>> statusTypeList;
        /// <summary>
        /// 配送状态
        /// </summary>
        public List<KeyValuePair<DeliveryStatus?, string>> StatusTypeList
        {
            get
            {
                statusTypeList = statusTypeList ?? EnumConverter.GetKeyValuePairs<DeliveryStatus>(EnumConverter.EnumAppendItemType.All);
                return statusTypeList;
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

        #endregion
    }
}
