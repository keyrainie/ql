using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.Inventory
{
    public class ShiftRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string UserName { get; set; }

        public int? UserSysNo { get; set; }

        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        /// <summary>
        /// 自己权限内能访问到的PM
        /// </summary>
        public string AuthorizedPMsSysNumber { get; set; }

        public string CompanyCode { get; set; }

        public string RequestID { get; set; }

        public int? ProductSysNo { get; set; }

        public int? SourceStockSysNo { get; set; }

        public int? TargetStockSysNo { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public ShiftRequestStatus? RequestStatus { get; set; }

        public DateTime? OutStockDateFrom { get; set; }

        public DateTime? OutStockDateTo { get; set; }

        public DateTime? InStockDateFrom { get; set; }

        public DateTime? InStockDateTo { get; set; }

        public string ShiftShippingType { get; set; }

        public ShiftRequestType? ShiftRquestType { get; set; }

        public bool? IsSpecialShift { get; set; }

        public SpecialShiftRequestType? SpecialShiftRequestStatus { get; set; }

        public RequestConsignFlag? ConsignFlag { get; set; }

        //public int? SpecialShiftType { get; set; }

        public SOStatus? SOStatus { get; set; }

        public List<int> SOSysNoList { get; set; }

        public int? CreateUserSysNo { get; set; }

        public List<int> ShiftRequestSysNoList { get; set; }

        public string SAPDocNo { get; set; }

        public DateTime? SAPPostDateFrom { get; set; }

        public DateTime? SAPPostDateTo { get; set; }

        public string SAPImportedStatus { get; set; }
      
        //public List<string> AuthorizedPMsSysNumberList { get; set; }
        public VirtualTransferType? IsVirtualTransfer { get; set; }
    }
}
