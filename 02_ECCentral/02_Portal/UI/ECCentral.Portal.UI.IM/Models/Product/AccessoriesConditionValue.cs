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

namespace ECCentral.Portal.UI.IM.Models
{
    /// <summary>
    /// 选项值实体 
    /// </summary>
    public class AccessoriesConditionValue : ModelBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        private string conditionValue;
         [Validate(ValidateType.Required)]     
        public string ConditionValue 
        {
            get { return conditionValue; }
            set { SetValue("ConditionValue", ref conditionValue, value); }
        }

         public int ParentSysNo { get; set; }

    }
}
