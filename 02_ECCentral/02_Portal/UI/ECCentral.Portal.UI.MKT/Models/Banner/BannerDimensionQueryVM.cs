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
    public class BannerDimensionQueryVM : ModelBase
    {
        private string _channelID;
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

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageTypeID
        {
            get;
            set;
        }

        private string _positionID;
        /// <summary>
        /// 页面位置
        /// </summary>
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string PositionID
        {
            get { return _positionID; }
            set
            {
                base.SetValue("PositionID", ref _positionID, value);
            }
        }

        /// <summary>
        /// 页面位置名称
        /// </summary>
        public string PositionName
        {
            get;
            set;
        }
    }
}
