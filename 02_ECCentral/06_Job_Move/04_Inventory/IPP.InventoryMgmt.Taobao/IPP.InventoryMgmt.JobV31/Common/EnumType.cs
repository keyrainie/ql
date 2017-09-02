/*****************************************
 * Author: Kilin.H.Zhang
*****************************************/
using System;

namespace IPP.InventoryMgmt.JobV31.Common
{
    /// <summary>
    /// 同步方式
    /// </summary>
    public enum SynType
    {
        /// <summary>
        /// 当一批数据同步失败后，不影响其他批次数据的同步
        /// </summary>
        Default,
        /// <summary>
        /// 队列同步，当一批数据同步失败后，其后的所有数据将自动退出同步
        /// </summary>
        Queue
    }
}
