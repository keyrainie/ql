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

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductRecommendLocationVM : ModelBase
    {
        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }
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
        private int? _pageType;
        /// <summary>
        /// 页面类型ID
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? PageType
        {
            get { return _pageType; }
            set
            {
                base.SetValue("PageType", ref _pageType, value);
            }
        }
        private int? _pageID;
        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID
        {
            get { return _pageID; }
            set
            {
                base.SetValue("PageID", ref _pageID, value);
            }
        }
        private string _positionID;
        /// <summary>
        /// 位置编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string PositionID
        {
            get { return _positionID; }
            set
            {
                base.SetValue("PositionID", ref _positionID, value);
            }
        }
        private string _description;
        /// <summary>
        /// 推荐位置描述
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                base.SetValue("Description", ref _description, value);
            }
        }

    }
}
