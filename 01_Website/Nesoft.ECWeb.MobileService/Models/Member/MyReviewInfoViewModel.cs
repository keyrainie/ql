using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Member;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class MyReviewInfoViewModel
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

        public string OrderDateString
        {
            get
            { return OrderDate.ToString("yyyy年MM月dd日 HH:mm:ss"); }
        }

        public int SysNo
        {
            get;
            set;
        }

        public int ProductSysNo
        {
            get;
            set;
        }

        public string ProductCode
        {
            get;
            set;
        }

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

        public int MostUseful
        {
            get;
            set;
        }

        public DateTime InDate
        {
            get;
            set;
        }

        public string InDateString
        {
            get
            {
                return InDate.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }

        public string ProductTitle
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public string ProductImage { get; set; }

        public string ReviewTitle
        {
            get;
            set;
        }

        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 评论次数（0从来就没有评论过；1评论了一次，还可以评论一次；2不能在进行评论）
        /// </summary>
        public int ReviewNumble { get; set; }  
    }
}