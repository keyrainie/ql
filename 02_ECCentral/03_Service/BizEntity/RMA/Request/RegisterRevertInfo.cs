using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 单件发货信息
    /// </summary>
    public class RegisterRevertInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        public RMARevertStatus? RevertStatus { get; set; }

        /// <summary>
        /// 新品类型
        /// </summary>
        public RMANewProductStatus? NewProductStatus { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public int? RevertStockSysNo { get; set; }

        /// <summary>
        /// 发货产品系统编号
        /// </summary>
        public int? RevertProductSysNo { get; set; }

        /// <summary>
        /// 发货产品ID
        /// </summary>
        public string RevertProductID { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string RevertAuditMemo { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public int? RevertAuditUserSysNo { get; set; }

        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string RevertAuditUserName { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? RevertAuditTime { get; set; }

        /// <summary>
        /// 设置待发货时间
        /// </summary>
        public DateTime? SetWaitingRevertTime { get; set; }

        /// <summary>
        /// 发货人系统编号
        /// </summary>
        public int? RevertUserSysNo { get; set; }
        /// <summary>
        /// 发货人姓名
        /// </summary>
        public string RevertUserName { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 发货运送方式
        /// </summary>
        public string RevertShipTypeName { get; set; }

        /// <summary>
        /// 发货包裹号
        /// </summary>
        public string RevertPackageID { get; set; }

        /// <summary>
        /// 预约返还时间
        /// </summary>
        public DateTime? OutTime { get; set; }
    }
}
