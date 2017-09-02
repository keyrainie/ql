using System;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GiftCardFabricationVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        ///时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 时间
        /// </summary>
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                base.SetValue("Title", ref title, value);
            }
        }

        /// <summary>
        /// POSysNo
        /// </summary>
        private string poSysNo;
        [Validate(ValidateType.Interger)]
        public string POSysNo
        {
            get { return poSysNo; }
            set
            {
                base.SetValue("POSysNo", ref poSysNo, value);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private string showStatus;
        public string ShowStatus
        {
            get { return showStatus; }
            set
            {
                base.SetValue("ShowStatus", ref showStatus, value);
            }
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }


        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }

        public decimal TotalPrice { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 采购单状态
        /// </summary>
        public ECCentral.BizEntity.PO.PurchaseOrderStatus? POStatus { get; set; }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        public DateTime? InPoDate { get; set; }

        public bool HasFabricationCreateMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GiftCard_FabricationCreateMaintain); }
        }

        public bool HasFabricationUpdateMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GiftCard_FabricationUpdateMaintain); }
        }

        public bool HasFabricationPOMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GiftCard_FabricationPOMaintain); }
        }

        public bool HasFabricationExportMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GiftCard_FabricationExportMaintain); }
        }

        public bool HasFabricationDeleteMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GiftCard_FabricationDeleteMaintain); }
        }
        
    }


    public class GiftCardFabricationItemVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        //private bool isChecked;
        //public bool IsChecked
        //{
        //    get { return isChecked; }
        //    set { base.SetValue("IsChecked", ref isChecked, value); }
        //}

        /// <summary>
        /// 数量
        /// </summary>
        private int? quantity;
        public int? Quantity
        {
            get { return quantity; }
            set { base.SetValue("Quantity", ref quantity, value); }
        }

        public string ProductName { get; set; }

        public string ProductID { get; set; }

        public int? ProductSysNo { get; set; }

        public decimal? CurrentPrice { get; set; }
        /// <summary>
        /// 采购单所属PM
        /// </summary>
        public int? PMUserSysNo { get; set; }
    }
}
