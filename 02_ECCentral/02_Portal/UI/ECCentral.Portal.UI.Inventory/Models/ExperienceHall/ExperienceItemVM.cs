using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ExperienceItemVM : ModelBase
    {
        public ExperienceItemVM()
        {

        }

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

        private int? productSysNo;
        public int? ProductSysNo
        {
            get
            {
                return productSysNo;
            }
            set
            {
                base.SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                base.SetValue("ProductID", ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                base.SetValue("ProductName", ref productName, value);
            }
        }

        private int? allocateSysNo;
        /// <summary>
        /// 调拨单号
        /// </summary>
        public int? AllocateSysNo
        {
            get
            {
                return allocateSysNo;
            }
            set
            {
                base.SetValue("AllocateSysNo", ref allocateSysNo, value);
            }
        }

        private int? allocateQty;
        public int? AllocateQty
        {
            get
            {
                return allocateQty;
            }
            set
            {
                base.SetValue("AllocateQty", ref allocateQty, value);
            }
        }

        private bool? isEditMode;
        public bool? IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                base.SetValue("IsEditMode", ref isEditMode, value);
            }
        }
    }
}
