﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存单据-移仓单
    /// </summary>
    public class ShiftRequestInfo : IIdentity, ICompany
    {
        public ShiftRequestInfo()
        {
            SourceStock = new StockInfo();
            TargetStock = new StockInfo();
        }
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members

        /// <summary>
        /// 产品线编号
        /// </summary>
        public string ProductLineSysno { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 出库人
        /// </summary>
        public UserInfo OutStockUser { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? OutStockDate { get; set; }

        /// <summary>
        /// 入库人
        /// </summary>
        public UserInfo InStockUser { get; set; }

        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime? InStockDate { get; set; }

        /// <summary>
        /// 移出渠道仓库
        /// </summary>
        public StockInfo SourceStock { get; set; }

        /// <summary>
        /// 移入渠道仓库
        /// </summary>
        public StockInfo TargetStock { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public ShiftRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 代销标识
        /// </summary>
        public RequestConsignFlag ConsignFlag { get; set; }

        /// <summary>
        /// 单据备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 移仓单类型
        /// </summary>
        public ShiftRequestType ShiftType { get; set; }

        /// <summary>
        /// 移仓配送方式
        /// </summary>
        public string ShiftShippingType { get; set; }

        /// <summary>
        /// 移仓商品列表
        /// </summary>
        public List<ShiftRequestItemInfo> ShiftItemInfoList { get; set; }

        #region original properties to confirm

        #region 特殊移仓单相关属性

        public bool IsSpecialShift { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public SpecialShiftRequestType SpecialShiftType { get; set; }

        /// <summary>
        /// 特殊移仓单设置人
        /// </summary>
        public UserInfo SpecialShiftSetUser { get; set; }

        /// <summary>
        /// 特殊移仓单设置日期
        /// </summary>
        public DateTime? SpecialShiftSetDate { get; set; }
        
        public bool HasSpecialRightforCreate { get; set; }

        #endregion 特殊移仓单相关属性

        public ShiftRequestInfo RelatedShiftRequest { get; set; }

        public PurchaseOrderInfo RelatedPO { get; set; }        

        public bool? IsScanned { get; set; }

        public decimal? TotalAmount { get; set; }

        #endregion original properties to confirm
    }
}
