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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ECCategoryQueryVM : ModelBase
    {
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

        private int? _c1SysNo;
        /// <summary>
        /// 前台1级分类系统编号
        /// </summary>
        public int? C1SysNo
        {
            get { return _c1SysNo; }
            set
            {
                base.SetValue("C1SysNo", ref _c1SysNo, value);
            }
        }
        private int? _c2SysNo;
        /// <summary>
        /// 前台2级分类系统编号
        /// </summary>
        public int? C2SysNo
        {
            get { return _c2SysNo; }
            set
            {
                base.SetValue("C2SysNo", ref _c2SysNo, value);
            }
        }
        private int? _c3SysNo;
        /// <summary>
        /// 前台3级分类系统编号
        /// </summary>
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _c3SysNo, value);
            }
        }

    }
}
