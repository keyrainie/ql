using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.SendQuestionList.BusinessEntities
{
    [Serializable]
    public class QuestionList
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo
        {
            get;
            set;
        }

        [DataMapping("ReplyTime", DbType.DateTime)]
        public DateTime ReplyTime
        {
            get;
            set;
        }

        [DataMapping("NickName", DbType.String)]
        public string  NickName
        {
            get;
            set;
        }



        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        [DataMapping("Question", DbType.String)]
        public string Question
        {
            get;
            set;
        }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID
        {
            get;
            set;
        }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName
        {
            get;
            set;
        }


        [DataMapping("Status", DbType.String)]
        public string Status
        {
            get;
            set;
        }

        [DataMapping("Reply", DbType.String)]
        public string Reply
        {
            get;
            set;
        }

        [DataMapping("ReplyUserName", DbType.String)]
        public string ReplyUserName
        {
            get;
            set;
        }

        public override string ToString()
        {
            return ProductSysNo.ToString();
        }
    }
}
