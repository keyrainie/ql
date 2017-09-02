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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductNotifyQueryVM : ModelBase
    {

        public ProductNotifyQueryVM()
        {
            NotifyStatusList = EnumConverter.GetKeyValuePairs<NotifyStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public int? Category1SysNo
        {
            get;
            set;
        }


        public int? Category2SysNo
        {
            get;
            set;
        }


        public int? Category3SysNo
        {
            get;
            set;
        }


        public string CustomserID
        {
            get;
            set;
        }


        public string ProductSysNo
        {
            get;
            set;
        }


        public  NotifyStatus?  Status
        {
            get;
            set;
        }


        public string Email
        {
            get;
            set;
        }


        public DateTime? StartTime
        {
            get;
            set;
        }


        public DateTime? EndTime
        {
            get;
            set;
        }


        public int? PMSysNo
        {
            get;
            set;
        }

        public List<KeyValuePair<NotifyStatus?, string>> NotifyStatusList { get; set; }
    }
}
