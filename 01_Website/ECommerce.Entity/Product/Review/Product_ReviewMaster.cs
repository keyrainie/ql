using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Entity.Member;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品品论
    /// </summary>
    [Serializable]
    [DataContract]
    public class Product_ReviewMaster
    {


       
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
       
        public int ReviewCount
        {
            get;
            set;
        }
       
        public decimal AvgScore
        {
            get;
            set;
        }
       
        public decimal AvgScore1
        {
            get;
            set;
        }
       
        public decimal AvgScore2
        {
            get;
            set;
        }
       
        public decimal AvgScore3
        {
            get;
            set;
        }
       
        public decimal AvgScore4
        {
            get;
            set;
        }

        /// <summary>
        /// 前5位评论用户可获得更多的积分
        /// </summary>
        public List<CustomerInfo> CustomerList
        {
            get;
            set;
        }

        /// <summary>
        /// 评分名称
        /// </summary>
        public List<string> ScoreNameList
        {
            get;
            set;
        }
    }
}
