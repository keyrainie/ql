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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorSectionItemVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private int floorMasterSysNo;
        public int FloorMasterSysNo
        {
            get { return floorMasterSysNo; }
            set { base.SetValue("FloorMasterSysNo", ref floorMasterSysNo, value); }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        private int floorSectionSysNo;
        public int FloorSectionSysNo
        {
            get { return floorSectionSysNo; }
            set { base.SetValue("FloorSectionSysNo", ref floorSectionSysNo, value); }
        }

        private int itemPosition;
        public int ItemPosition
        {
            get { return itemPosition; }
            set { base.SetValue("ItemPosition", ref itemPosition, value); }
        }

        private int isSelfPage;
        public int IsSelfPage
        {
            get { return isSelfPage; }
            set { base.SetValue("IsSelfPage", ref isSelfPage, value); }
        }

        private FloorItemType itemType;
        public FloorItemType ItemType
        {
            get { return itemType; }
            set { base.SetValue("ItemType", ref itemType, value); }
        }
    }
}
