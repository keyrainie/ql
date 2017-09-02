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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    /// <summary>
    /// 抽象出的条件实体，用于设置配件查询条件
    /// </summary>
    public class AccessoriesQueryCondition : ModelBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        private string conditionName;
          [Validate(ValidateType.Required)]     
        public string ConditionName 
        {
            get { return conditionName; }
            set { SetValue("ConditionName", ref conditionName, value); }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public PriorityType? Priority { get; set; }

        /// <summary>
        /// 条件编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 选项值
        /// </summary>
        public string SelectItemValue { get; set; }
        /// <summary>
        /// 配件查询SysNO
        /// </summary>
        public int MasterSysNo { get; set; }

        //父节点SysNo
        public int ParentSysNo { get; set; }
    }
}
