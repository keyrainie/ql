using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单投诉
    /// </summary>
    public class SOComplaintInfo : IIdentity
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                this.sysNo = value;
                ComplaintCotentInfo.SysNo = ComplaintCotentInfo == null ? null : value;
                ProcessInfo.SysNo = ProcessInfo == null ? null : value;
            }
        }
        public SOComplaintInfo()
        {
            ComplaintCotentInfo = new SOComplaintCotentInfo();
            ProcessInfo = new SOComplaintProcessInfo();
            ReplyHistory = new List<SOComplaintReplyInfo>();
        }
        /// <summary>
        /// 投诉内容
        /// </summary>
        public SOComplaintCotentInfo ComplaintCotentInfo { get; set; }
        /// <summary>
        /// 订单投诉处理信息
        /// </summary>
        public SOComplaintProcessInfo ProcessInfo { get; set; }
        /// <summary>
        /// 投诉相关回复
        /// </summary>
        public List<SOComplaintReplyInfo> ReplyHistory { get; set; }
    }

    /// <summary>
    /// 订单投诉内容信息
    /// </summary>
    public class SOComplaintCotentInfo:ICompany
    {
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 投诉编号
        /// </summary>
        public string ComplainID { get; set; }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 投诉类型
        /// </summary>
        public string ComplainType { get; set; }

        /// <summary>
        /// 投诉来源
        /// </summary>
        public string ComplainSourceType { get; set; }

        /// <summary>
        /// 投诉主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 来投诉的客户系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 来投诉的客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户邮件
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 投诉内容
        /// </summary>
        public string ComplainContent { get; set; }

        /// <summary>
        /// 投诉时间
        /// </summary>
        public DateTime? ComplainTime { get; set; }

        /// <summary>
        /// 所在公司
        /// </summary>
        public string CompanyCode { get; set; }
    }

    /// <summary>
    /// 订单投诉处理信息
    /// </summary>
    public class SOComplaintProcessInfo
    {
        /// <summary>
        /// 订单投诉系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
     
        /// <summary>
        /// 处理状态 
        /// </summary>
        public SOComplainStatus Status { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessTime
        { get; set; }

        #region 投诉处理分配
        /// <summary>
        /// 投诉分配人系统编号
        /// </summary>
        public int? AssignerSysNo { get; set; }

        /// <summary>
        /// 投诉分配日期
        /// </summary>
        public DateTime? AssignDate { get; set; }

        /// <summary>
        /// 投诉分配给的处理人的系统编号
        /// </summary>
        public int? OperatorSysNo { get; set; }
        #endregion

        #region 责任归属

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品所属domain
        /// </summary>
        public int? DomainSysNo { get; set; }

        public int? ReasonCodeSysNo { get; set; }

        /// <summary>
        /// 投诉摘要
        /// </summary>
        public string ComplainNote { get; set; }

        /// <summary>
        /// CS确认后的投诉类型
        /// </summary>
        public string CSConfirmComplainType { get; set; }

        /// <summary>
        /// CS确认后的投诉类型详细
        /// </summary>
        public string CSConfirmComplainTypeDetail { get; set; }

        /// <summary>
        /// 责任归属部门
        /// </summary>
        public string ResponsibleDepartment //同:ResponsibleDept
        { get; set; }

        /// <summary>
        /// 责任归属人
        /// </summary>
        public string ResponsibleUser { get; set; }

        /// <summary>
        /// 责任是否已经认定  
        /// </summary>
        public bool ? IsSure //同:DutyIdentification
        { get; set; }
        #endregion

        #region  处理回复相关

        /// <summary>
        /// 回复方式，处理方式
        /// </summary>
        public SOComplainReplyType ReplyType //同:ReplySourceType
        {
            get;
            set;
        }

        /// <summary>
        /// 处理内容
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 处理情况，备注
        /// </summary>
        public string ProcessedNote
        {
            get;
            set;
        }

        /// <summary>
        /// 花费时间
        /// </summary>
        public int SpendHours { get; set; }

        #endregion

        /// <summary>
        /// 订单类型？
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 打开次数
        /// </summary>
        public int ReopenCount { get; set; }
        
        /// <summary>
        /// 扩展描述
        /// </summary>
        public string ExtensionFlag { get; set; }
    }

    /// <summary>
    /// 订单投诉回复历史记录
    /// </summary>
    public class SOComplaintReplyInfo
    {
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 主投诉编号
        /// </summary>
        public int ComplainSysNo { get; set; }

        /// <summary>
        /// 处理状态 
        /// </summary>
        public SOComplainStatus Status { get; set; }

        /// <summary>
        /// 回复方式，处理方式
        /// </summary>
        public SOComplainReplyType ReplyType //同:ReplySourceType
        {
            get;
            set;
        }

        /// <summary>
        /// 回复记录
        /// </summary>
        public string HistoryContent
        {
            get;
            set;
        }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent
        {
            get;
            set;
        }

        /// <summary>
        /// 处理人SysNo
        /// </summary>
        public int? ReplyUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 处理人
        /// </summary>
        public string ReplyUserName
        {
            get;
            set;
        }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime
        { get; set; }
    }
}
