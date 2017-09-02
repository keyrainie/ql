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
    public class LotteryQueryVM : ModelBase
    {
        private string _lotteryLabel;
        /// <summary>
        /// 中奖ID
        /// </summary>
        public string LotteryLabel
        {
            get { return _lotteryLabel; }
            set
            {
                base.SetValue("LotteryLabel", ref _lotteryLabel, value);
            }
        }

        private string _lotteryName;
        /// <summary>
        /// 抽奖活动名称
        /// </summary>
        public string LotteryName
        {
            get { return _lotteryName; }
            set
            {
                base.SetValue("LotteryName", ref _lotteryName, value);
            }
        }

        private string _isLucky;
        /// <summary>
        /// 是否中奖
        /// </summary>
        public string IsLucky
        {
            get { return _isLucky; }
            set
            {
                base.SetValue("IsLucky", ref _isLucky, value);
            }
        }
        private DateTime? _beginDateFrom;
        /// <summary>
        /// 开始日期范围从
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
        /// 开始日期范围到
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
        /// 结束时间范围从
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
        /// 结束时间范围到
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

        public List<KeyValuePair<NYNStatus?, string>> IsLuckyList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<NYNStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

    }
}
