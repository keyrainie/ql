using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单跟踪
    /// </summary>
    public class SOInternalMemoInfo : IIdentity
    {
        /// <summary>
        /// 自增长编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public int ReasonCodeSysNo { get; set; }

        /// <summary>
        /// 信息来源 ,请通过xml来配置
        /// </summary>
        public int SourceSysNo { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public SOInternalMemoStatus Status { get; set; }

        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime? RemindTime { get; set; }

        /// <summary>
        /// 提醒时间的日期部分
        /// </summary>
        public DateTime? RemindTime_Date { get; set; }

        /// <summary>
        /// 提醒时间的时间部分
        /// </summary>
        public DateTime? RemainTime_Time { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 回复类型
        /// </summary>
        public int? CallType { get; set; }

        /// <summary>
        /// 紧急程度 ,请通过xml来配置
        /// </summary>
        public int? Importance { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 创建人编号
        /// </summary>
        public int CreateUserSysNo { get; set; }

        /// <summary>
        /// 跟踪记录时间
        /// </summary>
        public DateTime? LogTime
        {
            get;
            set;
        }

        #region 分配跟踪
        /// <summary>
        /// 分配人系统编号
        /// </summary>
        public int? AssignerSysNo { get; set; }

        /// <summary>
        /// 分配日期
        /// </summary>
        public DateTime? AssignDate { get; set; }

        /// <summary>
        /// 分配给的处理人的系统编号
        /// </summary>
        public int? OperatorSysNo { get; set; }
        #endregion

        /// <summary>
        /// 公司编码（非继承接口ICompany，只是数据查询可能需要根据公司查询）
        /// </summary>
        public string CompanyCode { get; set; }
    }

}
