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
using ECCentral.BizEntity.Customer;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingLotteryQueryVM : ModelBase
    {
        private int? _groupBuyingSysNo;
        /// <summary>
        /// 团购系统编号
        /// </summary>
        public int? GroupBuyingSysNo
        {
            get { return _groupBuyingSysNo; }
            set
            {
                base.SetValue("GroupBuyingSysNo", ref _groupBuyingSysNo, value);
            }
        }
        
        private string _rankType;
        /// <summary>
        /// 会员等级
        /// </summary>
        public string RankType
        {
            get { return _rankType; }
            set
            {
                base.SetValue("RankType", ref _rankType, value);
            }
        }
        private DateTime? _beginDateFrom;
        /// <summary>
        /// 团购开始日期范围从
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get { return _beginDateFrom; }
            set
            {
                base.SetValue("BeginDateFrom", ref _beginDateFrom, value);
            }
        }
        private DateTime? _beginDateTo;
        /// <summary>
        /// 团购开始日期范围到
        /// </summary>
        public DateTime? BeginDateTo
        {
            get { return _beginDateTo; }
            set
            {
                base.SetValue("BeginDateTo", ref _beginDateTo, value);
            }
        }
        private DateTime? _endDateFrom;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateFrom
        {
            get { return _endDateFrom; }
            set
            {
                base.SetValue("EndDateFrom", ref _endDateFrom, value);
            }
        }
        private DateTime? _endDateTo;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTo
        {
            get { return _endDateTo; }
            set
            {
                base.SetValue("EndDateTo", ref _endDateTo, value);
            }
        }

        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }

        public List<KeyValuePair<CustomerRank?, string>> RankKVList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<CustomerRank>(EnumConverter.EnumAppendItemType.All);
            }
        }

    }
}
