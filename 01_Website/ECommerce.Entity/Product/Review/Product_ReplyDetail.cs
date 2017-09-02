using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Enums;
using ECommerce.Entity.Member;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 回复
    /// </summary>
    [Serializable]
    [DataContract]
    public class Product_ReplyDetail
    {
        public Product_ReplyDetail()
        {
            Customer = new CustomerInfo();
            CustomerExtend = new CustomerExtendInfo();
        }

       
        public int SysNo
        {
            get;
            set;
        }


       
        public int ReviewSysNo
        {
            get;
            set;
        }


       
        public string Content
        {
            get;
            set;
        }


        public string Status
        {
            get;
            set;
        }

       
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 显示Type
        /// </summary>
       
        public FeedbackReplyType ReplyType
        {
            get
            {
                if (String.IsNullOrEmpty(this.Type))
                {
                    return FeedbackReplyType.Newegg;
                }
                else
                {
                    switch (this.Type.ToUpper())
                    {
                        case "M":
                            return FeedbackReplyType.Manufacturer;
                        case "N":
                            return FeedbackReplyType.Newegg;
                        case "W":
                            return FeedbackReplyType.Web;
                        default:
                            return FeedbackReplyType.Newegg;
                    }
                }
            }
            set
            {
                switch (value)
                {
                    case FeedbackReplyType.Manufacturer:
                        this.Type = "M";
                        break;
                    case FeedbackReplyType.Newegg:
                        this.Type = "N";
                        break;
                    case FeedbackReplyType.Web:
                        this.Type = "W";
                        break;
                    default:
                        this.Type = "N";
                        break;
                }
            }
        }

 
       
        public string NeedAdditionalText
        {
            get;
            set;
        }


        public DateTime InDate
        {
            get;
            set;
        }

        /// <summary>
        /// 评论回复人信息
        /// </summary>
       
        public CustomerInfo Customer
        {
            get;
            set;
        }
        /// <summary>
        /// 评论回复人扩展信息
        /// </summary>
       
        public CustomerExtendInfo CustomerExtend
        {
            get;
            set;
        }
    }
}
