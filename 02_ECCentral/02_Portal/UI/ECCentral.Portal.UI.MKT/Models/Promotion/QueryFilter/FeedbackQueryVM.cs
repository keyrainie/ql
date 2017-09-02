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
    public class FeedbackQueryVM : ModelBase
    {
        private int? _feedbackType;
        /// <summary>
        /// 中奖ID
        /// </summary>
        public int? FeedbackType
        {
            get { return _feedbackType; }
            set
            {
                base.SetValue("FeedbackType", ref _feedbackType, value);
            }
        }

        private GroupBuyingFeedbackStatus? _status;
        /// <summary>
        /// 抽奖活动名称
        /// </summary>
        public GroupBuyingFeedbackStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private DateTime? _createDateFrom;
        /// <summary>
        /// 是否中奖
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get { return _createDateFrom; }
            set
            {
                base.SetValue("CreateDateFrom", ref _createDateFrom, value);
            }
        }
        private DateTime? _createDateTo;
        /// <summary>
        /// 开始日期范围从
        /// </summary>
        public DateTime? CreateDateTo
        {
            get { return _createDateTo; }
            set
            {
                base.SetValue("CreateDateTo", ref _createDateTo, value);
            }
        }
       
        private DateTime? _readDateFrom;
        /// <summary>
        /// 结束时间范围从
        /// </summary>
        public DateTime? ReadDateFrom
        {
            get { return _readDateFrom; }
            set
            {
                base.SetValue("ReadDateFrom", ref _readDateFrom, value);
            }
        }
        private DateTime? _readDateTo;
        /// <summary>
        /// 结束时间范围到
        /// </summary>
        public DateTime? ReadDateTo
        {
            get { return _readDateTo; }
            set
            {
                base.SetValue("ReadDateTo", ref _readDateTo, value);
            }
        }       

        public List<KeyValuePair<GroupBuyingFeedbackStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<GroupBuyingFeedbackStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public List<CodeNamePair> FeedbackTypeList { get; set; }

    }
}
