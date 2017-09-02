using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Entity.Member;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Entity.Product;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 评论详情
    /// </summary>
    [Serializable]
    [DataContract]
    public class Product_ReviewDetail
    {
        public Product_ReviewDetail()
        {
            CustomerExtendInfo=new CustomerExtendInfo();
            CustomerInfo = new CustomerInfo();             
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
        /// 产品基本信息
        /// </summary>

        public ProductBasicInfo ProductBaseInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string Prons
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string Cons
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string Service
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public decimal Score
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int RankValue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string Image
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int Score1
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int Score2
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int Score3
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int Score4
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public decimal MerchantScore
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int MerchantScore1
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int MerchantScore2
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int MerchantScore3
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int UsefulCount
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int UnusefulCount
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int ReplyCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string IsTop
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public ReviewType ReviewType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string IsBottom
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public string IsDigest
        {
            get;
            set;
        }

        /// <summary>
        /// 网友回复列表--分页
        /// </summary>

        public PagedResult<Product_ReplyDetail> Replies
        {
            get;
            set;
        }


        /// <summary>
        /// 网友回复(商品详情)
        /// </summary>
        public List<Product_ReplyDetail> WebReplayList { get; set; }

        /// <summary>
        /// 厂商和买家回复列表
        /// </summary>

        public List<Product_ReplyDetail> ReplieList
        {
            get;
            set;
        }

        /// <summary>
        /// 用户基本信息
        /// </summary>

        public CustomerInfo CustomerInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户扩展信息
        /// </summary>

        public CustomerExtendInfo CustomerExtendInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户评论信息
        /// </summary>

        public CustomerReviewMaster CustomerReviewInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 属性
        /// </summary>
        public List<string> ScoreNameList
        {
            get;
            set;
        }

        public int ReviewCount { get; set; }

        public decimal AvgScore { get; set; }
    }
}
