using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class MyReview
    {

        public decimal ReviewScore
        {
            get;
            set;
        }


        public int ReplyCount
        {
            get;
            set;
        }


        public DateTime OrderDate
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int SysNo
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>

        public string ProductCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int CustomerSysNo
        {
            get;
            set;
        }

        public int SOSysno
        {
            get;
            set;
        }


        public string Status
        {
            get;
            set;
        }

        public int SoStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int MostUseful
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public DateTime InDate
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int ObtainPoint
        {
            get;
            set;
        }


        public string ProductTitle
        {
            get;
            set;
        }


        public string PromotionTitle
        {
            get;
            set;
        }


        public string ProductName
        {
            get;
            set;
        }



        public ProductStatus ProductStatus
        {
            get;
            set;
        }


        /// <summary>
        /// UI显示状态
        /// </summary>
        public AuditingStatus ShowAuditingStatus
        {
            get
            {
                if (string.IsNullOrEmpty(this.Status))
                {
                    return AuditingStatus.NoProcessing;
                }
                else
                {
                    switch (this.Status.ToUpper())
                    {
                        case "O":
                            return AuditingStatus.NoProcessing;
                        case "E":
                            return AuditingStatus.Peruse;
                        case "A":
                            return AuditingStatus.Pass;
                        case "D":
                            return AuditingStatus.NoPass;
                        default:
                            return AuditingStatus.NoProcessing;
                    }
                }
            }
        }


        public string ReviewTitle
        {
            get;
            set;
        }


        public string ProductImage
        {
            get;
            set;
        }


        public decimal CurrentPrice
        {
            get;
            set;
        }
    }
}
