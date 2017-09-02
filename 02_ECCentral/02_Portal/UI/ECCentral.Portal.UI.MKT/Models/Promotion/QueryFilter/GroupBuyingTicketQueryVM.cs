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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingTicketQueryVM : ModelBase
    {
        private string groupBuyingTitle;
        public string GroupBuyingTitle
        {
            get { return groupBuyingTitle; }
            set
            {
                base.SetValue("GroupBuyingTitle", ref groupBuyingTitle, value);
            }
        }

        private GroupBuyingTicketStatus? _status;
        public GroupBuyingTicketStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private DateTime? _createDateFrom;      
        public DateTime? CreateDateFrom
        {
            get { return _createDateFrom; }
            set
            {
                base.SetValue("CreateDateFrom", ref _createDateFrom, value);
            }
        }

        private DateTime? _createDateTo;       
        public DateTime? CreateDateTo
        {
            get { return _createDateTo; }
            set
            {
                base.SetValue("CreateDateTo", ref _createDateTo, value);
            }
        }
       
        private DateTime? usedDateFrom;
        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? UsedDateFrom
        {
            get { return usedDateFrom; }
            set
            {
                base.SetValue("UsedDateFrom", ref usedDateFrom, value);
            }
        }
        private DateTime? usedDateTo;
        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? UsedDateTo
        {
            get { return usedDateTo; }
            set
            {
                base.SetValue("UsedDateTo", ref usedDateTo, value);
            }
        }

        private string ticketID;      
        public string TicketID
        {
            get { return ticketID; }
            set
            {
                base.SetValue("TicketID", ref ticketID, value);
            }
        }

        private string usedStoreName;
        public string UsedStoreName
        {
            get { return usedStoreName; }
            set
            {
                base.SetValue("UsedStoreName", ref usedStoreName, value);
            }
        }       

        public List<KeyValuePair<GroupBuyingTicketStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<GroupBuyingTicketStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }        
    }
}
