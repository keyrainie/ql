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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAccessoriesQueryConditionVM : ModelBase
    {

        public ProductAccessoriesQueryConditionVM()
        {
            ParentConditionList = new List<AccessoriesQueryCondition>(); 
            PriorityList = EnumConverter.GetKeyValuePairs<PriorityType>(EnumConverter.EnumAppendItemType.None);
            Condition = new AccessoriesQueryCondition();
        }
        /// <summary>
        /// 条件
        /// </summary>
        private AccessoriesQueryCondition condition;
         [Validate(ValidateType.Required)]
        public AccessoriesQueryCondition Condition 
        {
            get { return condition; }
            set { SetValue("Condition", ref condition, value); }
        }

        /// <summary>
        /// 父级条件
        /// </summary>
        private AccessoriesQueryCondition parentCondition;
        public AccessoriesQueryCondition ParentCondition
        {
            get { return parentCondition; }
            set { SetValue("ParentCondition", ref parentCondition, value); }
        }

         /// <summary>
        /// 优先级
        /// </summary>
        private PriorityType? priority = PriorityType.One;
        public PriorityType? Priority 
        {
            get { return priority; }
            set { SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 所有父级
        /// </summary>
        private List<AccessoriesQueryCondition> parentConditionList;
        public List<AccessoriesQueryCondition> ParentConditionList
        {
            get { return parentConditionList; }
            set { SetValue("ParentConditionList", ref parentConditionList, value); }
        }
        public List<KeyValuePair<PriorityType?, string>> PriorityList { get; set; }
    }
}
