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
    public class AmbassadorNewsQueryVM : ModelBase
    {
        public string CompanyCode { get; set; }


      

        /// <summary>
        /// 标题
        /// </summary>
        private string title;

        [Validate(ValidateType.MaxLength,100)]
        public string Title
        {
            get { return title; }
            set { base.SetValue("Title", ref title, value); }
        }

        /// <summary>
        /// 编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }


      

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? inDateFromTo;
        public DateTime? InDateFromTo
        {
            get { return inDateFromTo; }
            set { base.SetValue("InDateFromTo", ref inDateFromTo, value); }
        }




        private int? _ReferenceSysNo;

        /// <summary>
        /// 大区
        /// </summary>
        public int? ReferenceSysNo
        {
            get { return _ReferenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref _ReferenceSysNo, value); }
        }
    }
}
