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
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class CommissionToCashQueryVM : ModelBase
    {

        public CommissionToCashQueryVM()
        {
            ToCashStatusList = EnumConverter.GetKeyValuePairs<ToCashStatus>(EnumConverter.EnumAppendItemType.All);
        }
        /// <summary>
        /// 会员账号
        /// </summary>
        private string customerID;
        public string CustomerID 
        {
            get { return customerID; }
            set { SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 申请日期开始
        /// </summary>
        private DateTime? applicationDateFrom;
        public DateTime? ApplicationDateFrom 
        {
            get { return applicationDateFrom; }
            set { SetValue("ApplicationDateFrom", ref applicationDateFrom, value); }
        }

        /// <summary>
        /// 申请日期到
        /// </summary>
        private DateTime? applicationDateTo;
        public DateTime? ApplicationDateTo 
        {
            get { return applicationDateTo; }
            set { SetValue("ApplicationDateTo", ref applicationDateTo, value); }
        }

        /// <summary>
        /// 申请状态
        /// </summary>
        public ToCashStatus? Status { get; set; }

        public Visibility CustomerIDVisibility 
        {
            get { return Visibility.Collapsed; }
        }

        public List<KeyValuePair<ToCashStatus?, string>> ToCashStatusList { get; set; }
    }
}
