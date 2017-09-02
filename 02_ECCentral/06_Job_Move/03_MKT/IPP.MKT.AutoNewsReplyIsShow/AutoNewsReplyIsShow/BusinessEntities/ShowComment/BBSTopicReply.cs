using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{
    [Serializable]
    public class BBSTopicReply
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int TopicReplySysNo { get; set; }

        [DataMapping("TopicSysNo", DbType.Int32)]
        public int TopicSysNo { get; set; }

        [DataMapping("ReplyContent", DbType.String)]
        public string ReplyContent { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CreateUserType", DbType.Int32)]
        public int CreateUserType { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNo { get; set; }

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

        [DataMapping("IsFirstShow", DbType.Int32)]
        public int IsFirstShow { get; set; }

        [DataMapping("ReplyCustomerID", DbType.String)]
        public string ReplyCustomerID { get; set; }

        [DataMapping("ReplyCustomerName", DbType.String)]
        public string ReplyCustomerName { get; set; }

        [DataMapping("ReplyCustomerEmail", DbType.String)]
        public string ReplyCustomerEmail { get; set; }

        [DataMapping("IsSubscribe", DbType.Int32)]
        public int IsSubscribe { get; set; }

        [DataMapping("WithAdditionalText", DbType.Int32)]
        public int WithAdditionalText { get; set; }
    }
}
