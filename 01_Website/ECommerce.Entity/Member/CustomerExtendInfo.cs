using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class CustomerExtendInfo
    {
        #region [ fields ]

        private int customerSysNo;
        private int buyCount;
        private DateTime lastBuyDate;
        private DateTime sendCustomerRankEmailDate;
        private int payDays;
        private decimal totalCreditLimit;
        private decimal availableCreditLimit;
        private int lastReceiveAreaSysNo;
        private int lastShipTypeSysNo;
        private int lastPayTypeSysNo;

        private CustomerMark customerMark;
        private DateTime customerMarkDate;

        private string avtarImage;
        private string avtarImageDBStatus;
        private string regionName;
        private string dBContributionType = "";

        private int question1;
        private int question2;
        private int question3;
        private string answer1;
        private string answer2;
        private string answer3;
        private int payVerification;

        #endregion

        #region [ properties ]

        public int CustomerSysNo
        {
            get { return customerSysNo; }
            set { customerSysNo = value; }
        }

        /// <summary>
        /// 购买次数
        /// </summary>
        public int BuyCount
        {
            get { return this.buyCount; }
            set { this.buyCount = value; }
        }

        /// <summary>
        /// 上次购买时间
        /// </summary>
        public DateTime LastBuyDate
        {
            get { return this.lastBuyDate; }
            set { this.lastBuyDate = value; }
        }

        /// <summary>
        /// 发送用户等级邮件时间
        /// </summary>
        public DateTime SendCustomerRankEmailDate
        {
            get { return this.sendCustomerRankEmailDate; }
            set { this.sendCustomerRankEmailDate = value; }
        }

        /// <summary>
        /// 付款天数
        /// </summary>
        public int PayDays
        {
            get { return this.payDays; }
            set { this.payDays = value; }
        }

        /// <summary>
        /// 总信用额
        /// </summary>
        public decimal TotalCreditLimit
        {
            get { return this.totalCreditLimit; }
            set { this.totalCreditLimit = value; }
        }

        /// <summary>
        /// 可用的信用额
        /// </summary>
        public decimal AvailableCreditLimit
        {
            get { return this.availableCreditLimit; }
            set { this.availableCreditLimit = value; }
        }

        /// <summary>
        /// 上次收货地区
        /// </summary>
        public int LastReceiveAreaSysNo
        {
            get { return this.lastReceiveAreaSysNo; }
            set { this.lastReceiveAreaSysNo = value; }
        }

        /// <summary>
        /// 上次配送方式
        /// </summary>
        public int LastShipTypeSysNo
        {
            get { return this.lastShipTypeSysNo; }
            set { this.lastShipTypeSysNo = value; }
        }

        /// <summary>
        /// 上次支付方式
        /// </summary>
        public int LastPayTypeSysNo
        {
            get { return this.lastPayTypeSysNo; }
            set { this.lastPayTypeSysNo = value; }
        }

        /// <summary>
        /// 客户类型，为新蛋大使添加，也考虑后续扩展
        /// </summary>
        public CustomerMark CustomerMark
        {
            get { return customerMark; }
            set { customerMark = value; }
        }

        /// <summary>
        /// customerType日期相关记录字段，为新蛋大使添加，来标记用户申请日期
        /// </summary>
        public DateTime CustomerMarkDate
        {
            get { return customerMarkDate; }
            set { customerMarkDate = value; }
        }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvtarImage
        {
            get { return avtarImage; }
            set { avtarImage = value; }
        }

        public string AvtarImageDBStatus
        {
            get { return avtarImageDBStatus; }
            set { avtarImageDBStatus = value; }
        }

        /// <summary>
        /// 头像状态,status='A' or 'D'
        /// </summary>

        public AvtarImageStatus AvtarImageStatus
        {
            get
            {
                if (avtarImageDBStatus == "A" || avtarImageDBStatus == "a")
                {
                    return AvtarImageStatus.A;// ctive;
                }
                else
                {
                    return AvtarImageStatus.D;//eactive;
                }
            }
            set
            {
                avtarImageDBStatus = (value == AvtarImageStatus.A ? "A" : "D");
            }
        }
        /// <summary>
        /// 所属地区
        /// </summary>

        public string RegionName
        {
            get { return regionName; }
            set { regionName = value; }
        }

        /// <summary>
        /// 贡献级别 助教(T),讲师(L),副教授(A),教授(P)
        /// </summary>

        public string DBContributionType
        {
            get { return dBContributionType; }
            set { dBContributionType = value; }
        }

        /// <summary>
        /// 贡献级别 助教(T),讲师(L),副教授(A),教授(P)
        /// </summary>
 
        public CustomerContributionType ContributeRank
        {
            set
            {
                switch (value)
                {
                    case CustomerContributionType.Assistant:
                        dBContributionType = "T";
                        break;
                    case CustomerContributionType.Docent:
                        dBContributionType = "L";
                        break;
                    case CustomerContributionType.AdjunctProfessor:
                        dBContributionType = "A";
                        break;
                    case CustomerContributionType.Professor:
                        dBContributionType = "P";
                        break;
                    default:
                        dBContributionType = "";
                        break;
                }

            }
            get
            {
                switch (dBContributionType.ToUpper())
                {
                    case "T":
                        return CustomerContributionType.Assistant;
                    case "L":
                        return CustomerContributionType.Docent;
                    case "A":
                        return CustomerContributionType.AdjunctProfessor;
                    case "P":
                        return CustomerContributionType.Professor;
                    default:
                        return CustomerContributionType.None;
                }
            }

        }

        /// <summary>
        /// 密码提示问题1
        /// </summary>
      
        public int Question1
        {
            get { return this.question1; }
            set { this.question1 = value; }
        }
        /// <summary>
        /// 密码提示问题2
        /// </summary>
       
        public int Question2
        {
            get { return this.question2; }
            set { this.question2 = value; }
        }
        /// <summary>
        /// 密码提示问题3
        /// </summary>
        
        public int Question3
        {
            get { return this.question3; }
            set { this.question3 = value; }
        }
        /// <summary>
        /// 密码提示问题答案1
        /// </summary>
        
        public string Answer1
        {
            get { return this.answer1; }
            set { this.answer1 = value; }
        }
        /// <summary>
        /// 密码提示问题答案2
        /// </summary>
       
        public string Answer2
        {
            get { return this.answer2; }
            set { this.answer2 = value; }
        }
        /// <summary>
        /// 密码提示问题答案3
        /// </summary>
       
        public string Answer3
        {
            get { return this.answer3; }
            set { this.answer3 = value; }
        }
        /// <summary>
        /// 支付设置
        /// </summary>
       
        public int PayVerification
        {
            get { return this.payVerification; }
            set { this.payVerification = value; }
        }


        #endregion
    }
}
