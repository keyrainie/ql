using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{
    [Serializable]
    public class BBSTopicMaster
    {
        [DataMapping("CommentSysNo", DbType.Int32)]
        public int CommentSysNo { get; set; }

        [DataMapping("TopicType", DbType.Int32)]
        public int TopicType { get; set; }

        [DataMapping("Title", DbType.String)]
        public string Title { get; set; }

        [DataMapping("TopicContent", DbType.String)]
        public string TopicContent { get; set; }

        [DataMapping("IsTop", DbType.Int32)]
        public int IsTop { get; set; }

        [DataMapping("IsDigest", DbType.Int32)]
        public int IsDigest { get; set; }

        [DataMapping("ReferenceType", DbType.Int32)]
        public int ReferenceType { get; set; }

        [DataMapping("ReferenceSysNo", DbType.Int32)]
        public int ReferenceSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo { get; set; }

        [DataMapping("Score", DbType.Int32)]
        public int Score { get; set; }

        [DataMapping("RemarkModeStatus", DbType.Int32)]
        public int RemarkModeStatus { get; set; }

        [DataMapping("WeekendRule", DbType.Int32)]
        public int WeekendRule { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CanRandomSelected", DbType.Int32)]
        public int CanRandomSelected { get; set; }

        [DataMapping("CreateCustomerSysNo", DbType.Int32)]
        public int CreateCustomerSysNo { get; set; }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime CreateDate { get; set; }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int LastEditUserSysNo { get; set; }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime LastEditDate { get; set; }

        [DataMapping("HasReviewed", DbType.Int32)]
        public int HasReviewed { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("IsAddPoint", DbType.Int32)]
        public int IsAddPoint { get; set; }

        [DataMapping("PMUserName", DbType.String)]
        public string PMUserName { get; set; }

        [DataMapping("PMEmailAddress", DbType.String)]
        public string PMEmailAddress { get; set; }

        [DataMapping("customerid", DbType.String)]
        public string CustomerID { get; set; }

        [DataMapping("customername", DbType.String)]
        public string CustomerName { get; set; }

        [DataMapping("CustomerEmail", DbType.String)]
        public string CustomerEmail { get; set; }

        [DataMapping("IsSubscribe", DbType.Int32)]
        public int IsSubscribe { get; set; }
    }

    public enum CommentType : int
    {
        [Description("经验")]
        Experience = 1,
        [Description("讨论")]
        Discuss = 2,
        [Description("推荐")]
        Suggestion = 3,
        [Description("询问")]
        Question = 4,
        [Description("经验(可疑)")]
        OnLineNotShowWord = 5
    }

    /// <summary>
    /// Comment 的状态
    /// </summary>   
    public enum CommentStatus:int
    {
        [Description("屏蔽")]
        SystemHide = -2,
        [Description("普通")]
        Normal = 0,
        [Description("所有已回复")]
        Replyed = 1,
        [Description("已作废")]
        Abandon = 2,
        [Description("普通(询问)")]
        Question = 3,
        [Description("(询问)已回复")]
        QuestionReplayed = 4,
        [Description("经验(可疑)")]
        OnLineNotShowWord = 5
    }
}
