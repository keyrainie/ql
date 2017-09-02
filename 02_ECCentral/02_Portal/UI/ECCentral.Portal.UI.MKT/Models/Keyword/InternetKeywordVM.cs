using System;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class InternetKeywordVM: ModelBase
    {
        public InternetKeywordVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<IsDefaultStatus>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 状态队列
        /// </summary>
        public List<KeyValuePair<IsDefaultStatus?, string>> StatusList { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        private string _searchKeyword;
        [Validate(ValidateType.Required)]
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { base.SetValue("SearchKeyword", ref _searchKeyword, value); }
        }

        /// <summary>
        /// 状态  
        /// </summary>
        private IsDefaultStatus _status = IsDefaultStatus.Deactive;
        public IsDefaultStatus Status
        {
            get { return _status; }
            set { base.SetValue("Status", ref _status, value); }
        }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperateUser { get; set; }

        
     
    }
}
