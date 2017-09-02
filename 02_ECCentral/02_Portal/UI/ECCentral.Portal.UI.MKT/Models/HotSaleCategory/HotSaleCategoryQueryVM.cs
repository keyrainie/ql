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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HotSaleCategoryQueryVM:ModelBase
    {
        private string _companyCode;
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private string _channelID;
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
        private string _groupName;
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                base.SetValue("GroupName", ref _groupName, value);
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
        private int? _position;
        /// <summary>
        /// 位置
        /// </summary>
        public int? Position
        {
            get { return _position; }
            set
            {
                base.SetValue("Position", ref _position, value);
            }
        }
        private int? _pageType;
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get { return _pageType; }
            set
            {
                base.SetValue("PageType", ref _pageType, value);
            }
        }

    }
}
