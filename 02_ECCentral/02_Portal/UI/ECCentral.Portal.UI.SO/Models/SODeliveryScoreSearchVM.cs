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
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SODeliveryScoreSearchVM : ModelBase
    {
        public PagingInfo PageInfo { get; set; }

        #region 查询条件

        private int? deliveryManUserSysNo;
        /// <summary>
        /// 配送员
        /// </summary>
        public int? DeliveryManUserSysNo 
        {
            get { return deliveryManUserSysNo; }
            set { SetValue<int?>("DeliveryManUserSysNo", ref deliveryManUserSysNo, value); }
        }

        private int? shipTypeSysNo;
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? ShipTypeSysNo
        {
            get { return shipTypeSysNo; }
            set { SetValue<int?>("ShipTypeSysNo", ref shipTypeSysNo, value); }
        }

        private DateTime? deliveryDateFrom;
        /// <summary>
        /// 配送日期（从）
        /// </summary>
        public DateTime? DeliveryDateFrom
        {
            get { return deliveryDateFrom; }
            set { SetValue<DateTime?>("DeliveryDateFrom", ref deliveryDateFrom, value); }
        }

        private DateTime? deliveryDateTo;
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
        /// 订单号
        /// </summary>
        public int? OrderSysNo
        {
            get { return orderSysNo; }
            set { SetValue<int?>("OrderSysNo", ref orderSysNo, value); }
        }

        private SYNStatus? vipRank;
        /// <summary>
        /// 会员类型
        /// </summary>
        public SYNStatus? VIPRank
        {
            get { return vipRank; }
            set { SetValue<SYNStatus?>("VIPRank", ref vipRank, value); }
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

        private string companyCode;
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue<string>("CompanyCode", ref companyCode, value); }
        }

        #endregion

        #region 查询条件绑定数据源


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


        private List<KeyValuePair<SYNStatus?, string>> vipRankTypeList;
        /// <summary>
        /// 会员类型
        /// </summary>
        public List<KeyValuePair<SYNStatus?, string>> VIPRankTypeList
        {
            get
            {
                vipRankTypeList = vipRankTypeList ?? EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
                return vipRankTypeList;
            }
        }

        #endregion
    }
}
