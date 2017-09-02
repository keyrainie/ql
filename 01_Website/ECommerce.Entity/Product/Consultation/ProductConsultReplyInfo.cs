using System;
using ECommerce.Entity.Member;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductConsultReplyInfo 
    {
        public ProductConsultReplyInfo()
        {
            CustomerInfo = new CustomerInfo();
            CustomerExtendInfo = new CustomerExtendInfo();
        }

        #region private fileds
         
        private int sysNo;
        private int consultSysNo;
        private CustomerInfo customer;
        private CustomerExtendInfo customerExtend;

        private string content;
        private string status;
        private string type;
        private string isTop = "F";
        private DateTime inDate;
        private int customerSysno;
        private DateTime editDate;
        private string needAdditionalText = "Y";
        #endregion

        #region public property

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
      
        public int SysNo
        {
            get { return sysNo; }
            set { sysNo = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public int ConsultSysNo
        {
            get { return consultSysNo; }
            set { consultSysNo = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public CustomerInfo CustomerInfo
        {
            get { return this.customer; }
            set { this.customer = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
      
        public CustomerExtendInfo CustomerExtendInfo
        {
            get { return this.customerExtend; }
            set { this.customerExtend = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        
        public string Status
        {
            get { return status; }
            set { status = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public string Type
        {
            get { return type; }
            set { type = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public string IsTop
        {
            get { return isTop; }
            set { isTop = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public DateTime InDate
        {
            get { return inDate; }
            set { inDate = value; }
        }

        /// <summary>
        /// 用户表系统唯一编号
        /// </summary>
      
        public int CustomerSysNo
        {
            get { return this.customerSysno; }
            set { this.customerSysno = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public DateTime EditDate
        {
            get { return editDate; }
            set { editDate = value; }
        }

        /// <summary>
        /// 显示Type
        /// </summary>
       
        public FeedbackReplyType ReplyType
        {
            get
            {
                switch (this.type.ToUpper())
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
            set
            {
                switch (value)
                {
                    case FeedbackReplyType.Manufacturer:
                        this.type = "M";
                        break;
                    case FeedbackReplyType.Newegg:
                        this.type = "N";
                        break;
                    case FeedbackReplyType.Web:
                        this.type = "W";
                        break;
                    default:
                        this.type = "N";
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       
        public string NeedAdditionalText
        {
            get { return needAdditionalText; }
            set { needAdditionalText = value; }
        }
        #endregion
    }
}
