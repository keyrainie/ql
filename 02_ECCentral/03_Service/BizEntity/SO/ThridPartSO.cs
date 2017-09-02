using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    ///第三方订单与本系统订单关系实体
    /// </summary>
    public class ThridPartSOInfo : IIdentity
    {

        /// <summary>
        ///  第三方类型,TAO 表示淘宝,DF:东方网
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// IPP订单号        
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 创建IPP订单结果, S:创建成功、F:创建失败,N:未创建
        /// </summary>
        public string CreateResult { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string SOStatus { get; set; }

        /// <summary>
        /// 同步结果 , S:同步成功、F:同步失败,N:未同步
        /// </summary>
        public string StatusSyncResult { get; set; }

        /// <summary>
        /// 出错的信息
        /// </summary>
        public string Memo { get; set; }


        // SyncLogSysNo, JobAssignmentNo

        public int? SysNo
        {
            get;
            set;
        }
    }
}
