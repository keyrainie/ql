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
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductBatchEntryVM : ModelBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { SetValue("Title", ref _Title, value); }
        }

        private string _BtnPassTitle;
        public string BtnPassTitle
        {
            get { return _BtnPassTitle; }
            set { SetValue("BtnPassTitle", ref _BtnPassTitle, value); }
        }

        private string _BtnRejectTitle;
        public string BtnRejectTitle
        {
            get { return _BtnRejectTitle; }
            set { SetValue("BtnRejectTitle", ref _BtnRejectTitle, value); }
        }

        private string _Note;
        public string Note
        {
            get { return _Note; }
            set { SetValue("Note", ref _Note, value); }
        }

        public bool? AuditPass { get; set; }
    }
}
