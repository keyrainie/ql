using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 分组属性信息
    /// </summary>
    public class GroupPropertyInfo
    {
        #region [ properties ]

        public int PropertySysNo1 { get; set; }


        public string PropertyDescription1 { get; set; }


        public int ValueSysNo1 { get; set; }


        public string ValueDescription1 { get; set; }


        public string IsPolymeric1 { get; set; }


        public int PropertySysNo2 { get; set; }


        public string PropertyDescription2 { get; set; }


        public int ValueSysNo2 { get; set; }


        public string ValueDescription2 { get; set; }


        public string IsPolymeric2 { get; set; }


        public string ProductGroupKeyword { get; set; }

        #endregion
    }
}
