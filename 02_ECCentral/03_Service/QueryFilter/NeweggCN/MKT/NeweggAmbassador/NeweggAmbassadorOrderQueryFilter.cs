using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 
    /// </summary>
    public class NeweggAmbassadorOrderQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CompanyCode { get; set; }


        /// <summary>
        /// 大区
        /// </summary>
        public int? BigAreaSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType SelectedUserType
        {
            get;
            set;
        }


        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }


        /// <summary>
        /// 下单时间从。
        /// </summary>
        public string OrderTimeFrom
        {
            get;
            set;
        }


        /// <summary>
        /// 下单时间到。
        /// </summary>
        public string OrderTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 积分发放时间从。
        /// </summary>
        public string PointTimeFrom
        {
            get;
            set;
        }


        /// <summary>
        /// 积分发放时间到。
        /// </summary>
        public string PointTimeTo
        {
            get;
            set;
        }


        /// <summary>
        /// 订单状态。
        /// </summary>
        public SOStatus? SelectedSOStatus
        {
            get;
            set;
        }


        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderSysNo
        {
            get;
            set;
        }


        /// <summary>
        /// 积分发放状态。
        /// </summary>
        public PointStatus? SelectedPointStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 下单ID。
        /// </summary>
        public string CreateSOCustomerID
        {
            get;
            set;
        }


    }
}
