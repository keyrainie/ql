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
    public class ExperienceVM : ModelBase
    {
        public ExperienceVM()
        {
            this.RequestStatusList = EnumConverter.GetKeyValuePairs<ExperienceHallStatus>(EnumConverter.EnumAppendItemType.All);
            this.AllocateTypeList = EnumConverter.GetKeyValuePairs<AllocateType>(EnumConverter.EnumAppendItemType.None);

            this.ExperienceItemInfoList = new List<ExperienceItemVM>();

            stockSysNo = "51";
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

        private string stockSysNo;
        /// <summary>
        ///  仓库编号
        /// </summary>
        public string StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }

        /// <summary>
        /// 调拨类型
        /// </summary>
        private AllocateType allocateType;
        public AllocateType AllocateType
        {
            get
            {
                return allocateType;
            }
            set
            {
                base.SetValue("AllocateType", ref allocateType, value);
            }
        }

        private int? inUser;
        /// <summary>
        /// 创建人
        /// </summary>
        public int? InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }

        private DateTime? indate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? InDate
        {
            get { return indate; }
            set { base.SetValue("InDate", ref indate, value); }
        }

        private int? editUser;
        /// <summary>
        /// 更新人
        /// </summary>
        public int? EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
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

        private int? auditUser;
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUser 
        {
            get { return auditUser; }
            set { base.SetValue("AuditUser", ref auditUser, value); }
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

        private ExperienceHallStatus status;
        /// <summary>
        /// 状态
        /// </summary>
        public ExperienceHallStatus Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string meno;
        public string Meno
        {
            get
            {
                return meno;
            }
            set
            {
                base.SetValue("Meno", ref meno, value);
            }
        }

        public List<KeyValuePair<ExperienceHallStatus?, string>> RequestStatusList { get; set; }

        public List<KeyValuePair<AllocateType?, string>> AllocateTypeList { get; set; }

        public List<ExperienceItemVM> ExperienceItemInfoList { get; set; }

        #endregion 属性
    }
}
