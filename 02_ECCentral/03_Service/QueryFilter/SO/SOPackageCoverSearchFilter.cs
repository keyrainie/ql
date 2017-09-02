using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
   public class SOPackageCoverSearchFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 销售订单编号  SONumber
        /// </summary>
        public string SONumber { get; set; }

        /// <summary>
        /// 包裹单号  TrackingNumber
        /// </summary>
        public string TrackingNumber { get; set; }

               
       /// <summary>
       /// 出库日期(从)
       /// </summary>
        public DateTime? ShippedOutTimeFrom { get; set; }
       
       /// <summary>
       /// 出库日期(至)
       /// </summary>
        public DateTime? ShippedOutTimeTo { get; set; }

        /// <summary>
        /// 收货人  ReceiveName
        /// </summary>
        public string ReceiveName { get; set; }

        /// <summary>
        /// 地址	 ReceiveAddress
        /// </summary>
        public string ReceiveAddress { get; set; }

        ///// <summary>
        ///// 省  ProvinceName
        ///// </summary>
        //public string ProvinceName { get; set; }

        ///// <summary>
        ///// 城市  CityName
        ///// </summary>
        //public string CityName { get; set; }

        /// <summary>
        /// 区域  DistrictName
        /// </summary>
        public string DistrictName { get; set; }


        ///<summary>
        ///送货区域
        ///</summary>
        public int? ReceiveAreaSysNo { get; set; }

        /// <summary>
        /// 3PL重量	 Weight3PL
        /// </summary>
        public double Weight3PL { get; set; }


        /// <summary>
        /// 仓库编号  WarehouseNumber
        /// </summary>
        public int? WareHouseNumber { get; set; }

        /// <summary>
        /// 创建时间	CreateTime
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人	CreateUserID
        /// </summary>
        public string CreateUserID { get; set; }

        /// <summary>
        /// 更新时间  UpdateTime
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新人 UpdateUserID
        /// </summary>
        public string UpdateUserID { get; set; }

      
        /// <summary>
        /// 配送类型
        /// </summary>
        public int? ShipTypeSysNo { get; set; }

        ///<summary>
        /// DropshipID
        ///</summary>
        public int? DropshipID { get; set; }

        ///<summary>
        /// SubCode
        ///</summary>
        public int? SubCode { get; set; }


        /// <summary>
        /// 签收状态	SignStatus
        /// </summary>
        public PackageSignStatus? SignStatus { get; set; }

        ///<summary>
        ///系统核对时间	Check3PLTime
        ///</summary>
        public DateTime? Check3PLTime { get; set; }

        public string CompanyCode { get; set; }
    }
}
