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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HelpCenterQueryVM : ModelBase
    {
        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]{0,4}$", ErrorMessage = "优先级为0-9999的整数")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private FeatureType? _type;
        /// <summary>
        /// 标识类型，比如New,Hot
        /// </summary>
        public FeatureType? Type
        {
            get { return _type; }
            set
            {
                base.SetValue("Type", ref _type, value);
            }
        }
        private ADStatus? _status;
        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private int? _categorySysNo;
        /// <summary>
        /// 所属类型系统编号
        /// </summary>
        public int? CategorySysNo
        {
            get { return _categorySysNo; }
            set
            {
                base.SetValue("CategorySysNo", ref _categorySysNo, value);
            }
        }
        private string _companyCode;
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private string _channelID = "1";
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
    }
}
