using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 欺诈审核配置子项
    /// </summary>
    public class FPCheckItem:IIdentity
    {

        /// <summary>
        /// Item数据类别
        /// </summary>
        public string FPCheckItemDataType { get; set; }

        /// <summary>
        /// Item数据
        /// </summary>
        public string FPCheckItemDataValue { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public FPCheckItemStatus FPCheckItemStatus { get; set; }


        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion

 
    }
}
