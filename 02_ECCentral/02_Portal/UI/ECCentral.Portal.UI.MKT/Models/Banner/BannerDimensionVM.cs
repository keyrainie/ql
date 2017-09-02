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
    public class BannerDimensionVM : ModelBase
    {
        #region UI扩展属性

        public string DimensionDesc
        {
            get
            {
                string format = "({0} x {1})";
                string dimension = "";
                if (!string.IsNullOrWhiteSpace(Width) && !string.IsNullOrWhiteSpace(Height))
                {
                    dimension = string.Format(format, Width, Height);
                }
                return (PositionName ?? "") + dimension;
            }
        }

        #endregion

        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

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
        private int? _pageTypeID;
        /// <summary>
        /// 页面类型
        /// </summary>
        [Validate(ValidateType.Required)]
        [Integer]
        public int? PageTypeID
        {
            get { return _pageTypeID; }
            set
            {
                base.SetValue("PageTypeID", ref _pageTypeID, value);
            }
        }
        private string _positionID;
        /// <summary>
        /// 页面位置
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string PositionID
        {
            get { return _positionID; }
            set
            {
                base.SetValue("PositionID", ref _positionID, value);
            }
        }
        private string _positionName;
        /// <summary>
        /// 页面位置名称
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,15)]
        public string PositionName
        {
            get { return _positionName; }
            set
            {
                base.SetValue("PositionName", ref _positionName, value);
            }
        }
        private string _width;
        /// <summary>
        /// 长
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string Width
        {
            get { return _width; }
            set
            {
                base.SetValue("Width", ref _width, value);
            }
        }
        private string _height;
        /// <summary>
        /// 宽
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string Height
        {
            get { return _height; }
            set
            {
                base.SetValue("Height", ref _height, value);
            }
        }

    }
}
