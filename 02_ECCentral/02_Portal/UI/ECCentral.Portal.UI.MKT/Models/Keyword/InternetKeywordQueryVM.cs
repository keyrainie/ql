using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class InternetKeywordQueryVM: ModelBase
    {
        public InternetKeywordQueryVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<IsDefaultStatus>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 状态队列
        /// </summary>
        public List<KeyValuePair<IsDefaultStatus?, string>> StatusList { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { base.SetValue("SearchKeyword", ref _searchKeyword, value); }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        private IsDefaultStatus? _status;
        public IsDefaultStatus? Status
        {
            get { return _status; }
            set { base.SetValue("Status", ref _status, value); }
        }

        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        private DateTime? _beginDate;
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set { base.SetValue("BeginDate", ref _beginDate, value); }
        }

        /// <summary>
        /// 编辑时间结束于
        /// </summary>
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { base.SetValue("EndDate", ref _endDate, value); }
        }

    }


}
