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

namespace ECCentral.Portal.UI.MKT.Models
{
    public class InvalidReasonVM:ModelBase
    {
        private bool _isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
            }
        }
        private string _reasonID;
        /// <summary>
        /// 无效原因编号
        /// </summary>
        public string ReasonID
        {
            get { return _reasonID; }
            set
            {
                base.SetValue("ReasonID", ref _reasonID, value);
            }
        }
        private string _reasonDesc;
        /// <summary>
        /// 无效原因描述
        /// </summary>
        public string ReasonDesc
        {
            get { return _reasonDesc; }
            set
            {
                base.SetValue("ReasonDesc", ref _reasonDesc, value);
            }
        }

    }
}
