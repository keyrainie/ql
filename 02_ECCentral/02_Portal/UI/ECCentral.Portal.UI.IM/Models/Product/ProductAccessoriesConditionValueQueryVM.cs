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
    public class ProductAccessoriesConditionValueQueryVM : ModelBase
    {
        public ProductAccessoriesConditionValueQueryVM()
        {
            Condition = new AccessoriesQueryCondition();
            ConditionValue = new AccessoriesConditionValue();
            ConditionList = new List<AccessoriesQueryCondition>();
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
        /// 选项值
        /// </summary>
        private AccessoriesConditionValue conditionValue;
        public AccessoriesConditionValue ConditionValue 
        {
            get { return conditionValue; }
            set { SetValue("ConditionValue", ref conditionValue, value); }
        }

        private List<AccessoriesQueryCondition> conditionList;
        public List<AccessoriesQueryCondition> ConditionList
        {
            get { return conditionList; }
            set { SetValue("ConditionList", ref conditionList, value); }
        }
        /// <summary>
        /// 查询功能SysNo
        /// </summary>
        public int MasterSysNo { get; set; }
      
    }
}
