using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class LendRequestVM : ModelBase
    {
        public LendRequestVM()
        {
            this.RequestStatusList = EnumConverter.GetKeyValuePairs<LendRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.LendItemInfoList = new List<LendRequestItemVM>();
            this.ReturnItemInfoList = new List<LendRequestReturnItemInfo>();
            this.RequestStatus = LendRequestStatus.Origin;
        }

        #region 属性

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private string productLineSysno;
        /// <summary>
        /// 产品线
        /// </summary>
        public string ProductLineSysno
        {
            get { return productLineSysno; }
            set { base.SetValue("ProductLineSysno", ref productLineSysno, value); }
        }

        private string requestID;
        public string RequestID
        {
            get
            {
                return requestID;
            }
            set
            {
                base.SetValue("RequestID", ref requestID, value);
            }
        }

        private int? lendUserSysNo;
        public int? LendUserSysNo
        {
            get
            {
                return lendUserSysNo;
            }
            set
            {
                base.SetValue("LendUserSysNo", ref lendUserSysNo, value);
            }
        }

        private int? createUserSysNo;
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private DateTime? createDate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        private int? editUserSysNo;
        /// <summary>
        /// 更新人
        /// </summary>
        public int? EditUserSysNo
        {
            get { return editUserSysNo; }
            set { base.SetValue("EditUserSysNo", ref editUserSysNo, value); }
        }

        private DateTime? editDate;
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        private int? auditUserSysNo;
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { base.SetValue("AuditUserSysNo", ref auditUserSysNo, value); }
        }

        private DateTime? auditDate;
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { base.SetValue("AuditDate", ref auditDate, value); }
        }

        private int? outStockUserSysNo;
        /// <summary>
        /// 出库人
        /// </summary>
        public int? OutStockUserSysNo
        {
            get { return outStockUserSysNo; }
            set { base.SetValue("OutStockUserSysNo", ref outStockUserSysNo, value); }
        }

        private DateTime? outStockDate;
        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? OutStockDate
        {
            get { return outStockDate; }
            set { base.SetValue("OutStockDate", ref outStockDate, value); }
        }

        private int? stockSysNo;
        public int? StockSysNo
        {
            get
            {
                return stockSysNo;
            }
            set
            {
                base.SetValue("StockSysNo", ref stockSysNo, value);
            }
        }

        private string stockName;
        public string StockName
        {
            get
            {
                return stockName;
            }
            set
            {
                base.SetValue("StockName", ref stockName, value);
            }
        }

        private string webChannelName;
        public string WebChannelName
        {
            get
            {
                return webChannelName;
            }
            set
            {
                base.SetValue("WebChannelName", ref webChannelName, value);
            }
        }

        private LendRequestStatus? requestStatus;
        public LendRequestStatus? RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                base.SetValue("RequestStatus", ref requestStatus, value);
            }
        }


        private string note;
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                base.SetValue("Note", ref note, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                base.SetValue("CompanyCode", ref companyCode, value);
            }
        }

        public List<KeyValuePair<LendRequestStatus?, string>> RequestStatusList { get; set; }

        public List<LendRequestItemVM> LendItemInfoList { get; set; }

        public List<LendRequestReturnItemInfo> ReturnItemInfoList { get; set; }

        #endregion 属性

        #region 单据操作状态图设置

        public bool IsCreateMode
        {
            get
            {
                return (this.SysNo == null || this.SysNo <= 0);
            }
        }

        public bool IsEditMode
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.Origin;
            }
        }

        public bool IsAbandonEnabled
        {
            get 
            {
                return this.RequestStatus == LendRequestStatus.Origin
                    && !IsCreateMode;
            }
        }


        public bool IsCancelAbandonEnabled
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.Abandon;
            }
        }

        public bool IsVerifyEnabled
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.Origin
                   && !IsCreateMode;
            }
        }

        public bool IsCancelVerifyEnabled
        {
            get
            {
                return this.requestStatus == LendRequestStatus.Verified;
            }
        }

        public bool IsOutStockEnabled
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.Verified;
            }
        }       

        public bool IsPrintEnabled
        {
            get
            {
                return !IsCreateMode;
            }
        }

        public bool IsReturnEnabled
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.OutStock
                    || this.RequestStatus == LendRequestStatus.ReturnPartly;
            }
        }

        public bool IsResetEnabled
        {
            get
            {
                return IsEditMode || IsReturnEnabled;
            }
        }
        public Visibility SaveActionVisibility
        {
            get
            {
                return ReturnActionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ReturnActionVisibility
        {
            get
            {
                return (IsReturnEnabled || this.RequestStatus == LendRequestStatus.ReturnAll) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion UI Model
    }
}
