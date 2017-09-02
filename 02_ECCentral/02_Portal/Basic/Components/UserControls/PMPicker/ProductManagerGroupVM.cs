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
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;

namespace ECCentral.Portal.Basic.Components.UserControls.PMPicker
{
    public class ProductManagerGroupVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// PM组名称
        /// </summary>
        public string PMGroupName { get; set; }

        /// <summary>
        /// PM组Leader信息
        /// </summary>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PMGroupStatus Status { get; set; }
    }
}
