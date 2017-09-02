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
    public class RMAShiftVM : ModelBase
    {
        private int m_RegisterSysNo;
        public int RegisterSysNo
        {
            get { return m_RegisterSysNo; }
            set { SetValue("RegisterSysNo", ref m_RegisterSysNo, value); }
        }

        private string m_ProductID;
        public string ProductID
        {
            get { return m_ProductID; }
            set { SetValue("ProductID", ref m_ProductID, value); }
        }

        private string m_TargetBriefName;
        public string TargetBriefName
        {
            get { return m_TargetBriefName; }
            set { SetValue("TargetBriefName", ref m_TargetBriefName, value); }
        }

        private int m_TargetProductQty;
        public int TargetProductQty
        {
            get { return m_TargetProductQty; }
            set { SetValue("TargetProductQty", ref m_TargetProductQty, value); }
        }

        private string m_ShippedWarehouseName;
        public string ShippedWarehouseName
        {
            get { return m_ShippedWarehouseName; }
            set { SetValue("ShippedWarehouseName", ref m_ShippedWarehouseName, value); }
        }

        private int m_ShiftSysNo;
        public int ShiftSysNo
        {
            get { return m_ShiftSysNo; }
            set { SetValue("ShiftSysNo", ref m_ShiftSysNo, value); }
        }

        private RMAShiftType m_ShiftType;
        public RMAShiftType ShiftType
        {
            get { return m_ShiftType; }
            set { SetValue("ShiftType", ref m_ShiftType, value); }
        }

        private RMAShiftStatus m_Status;
        public RMAShiftStatus Status
        {
            get { return m_Status; }
            set { SetValue("Status", ref m_Status, value); }
        }

        private string m_StockSysNoAName;
        public string StockSysNoAName
        {
            get { return m_StockSysNoAName; }
            set { SetValue("StockSysNoAName", ref m_StockSysNoAName, value); }
        }

        private string m_StockSysNoBName;
        public string StockSysNoBName
        {
            get { return m_StockSysNoBName; }
            set { SetValue("StockSysNoBName", ref m_StockSysNoBName, value); }
        }

        private string m_Note;
        public string Note
        {
            get { return m_Note; }
            set { SetValue("Note", ref m_Note, value); }
        }
    }
}
