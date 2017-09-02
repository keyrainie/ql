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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAccessoriesConditionValueVM : ModelBase
    {

        public ProductAccessoriesConditionValueVM() 
        {
            Condition = new AccessoriesQueryCondition();
            ConditionValue = new AccessoriesConditionValue();
        }

        /// <summary>
        /// 条件
        /// </summary>
        private AccessoriesQueryCondition condition;
        public AccessoriesQueryCondition Condition
        {
            get { return condition; }
            set { SetValue("Condition", ref condition, value); }
        }
        /// <summary>
        /// 条件集合
        /// </summary>
        private List<AccessoriesQueryCondition> conditionList;
        public List<AccessoriesQueryCondition> ConditionList
        {
            get { return conditionList; }
            set { SetValue("ConditionList", ref conditionList, value); }
        }
        /// <summary>
        /// 选项值
        /// </summary>
        private AccessoriesConditionValue conditionValue;
        public AccessoriesConditionValue ConditionValue
        {
            get { return conditionValue; }
            set { SetValue("ConditionValue", ref conditionValue, value); }
        }

        /// <summary>
        /// 父级
        /// </summary>
        private AccessoriesQueryCondition parentCondition;
        public AccessoriesQueryCondition ParentCondition
        {
            get { return parentCondition; }
            set { SetValue("ParentCondition", ref parentCondition, value); }
        }
        /// <summary>
        /// 父级选项值
        /// </summary>
        private AccessoriesConditionValue parentConditionValue;
        public AccessoriesConditionValue ParentConditionValue
        {
            get { return parentConditionValue; }
            set { SetValue("ParentConditionValue", ref parentConditionValue, value); }
        }
        /// <summary>
        /// 父级选项值集合
        /// </summary>
        private List<AccessoriesConditionValue> parentConditionValueList;
        public List<AccessoriesConditionValue> ParentConditionValueList
        {
            get { return parentConditionValueList; }
            set { SetValue("ParentConditionValueList", ref parentConditionValueList, value); }
        }

        public int MasterSysNo { get; set; }
        public int SysNo { get; set; }
    }
}
