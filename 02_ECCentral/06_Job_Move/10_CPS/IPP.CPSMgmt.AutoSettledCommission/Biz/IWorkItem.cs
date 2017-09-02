using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{
    /// <summary>
    /// 操作对象接口【对账过程中每个步骤应当实现的接口】
    /// </summary>
    interface IWorkItem
    {
        #region 属性
        /// <summary>
        /// 作业过程中的待处理数据
        /// </summary>
        Dictionary<string, object> ContextData { get; }
        #endregion 属性

        #region 方法
        /// <summary>
        /// 开始具体操作
        /// </summary>
        void ProcessWork();
        /// <summary>
        /// 校验必要的数据信息
        /// </summary>
        /// <returns>校验结果</returns>
        ValidateResult ValidateData();
        #endregion 方法
        
    }
}
