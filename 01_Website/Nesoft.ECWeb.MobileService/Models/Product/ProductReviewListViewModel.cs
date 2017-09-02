using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.Utility;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ProductReviewListViewModel
    {


        /// <summary>
        /// 全部评论
        /// </summary>

        public int TotalCount0
        {
            get;
            set;
        }

        /// <summary>
        /// 很好评论
        /// </summary>

        public int TotalCount1
        {
            get;
            set;
        }

        /// <summary>
        /// 较好评论
        /// </summary>

        public int TotalCount2
        {
            get;
            set;
        }

        /// <summary>
        ///一般评论
        /// </summary>

        public int TotalCount3
        {
            get;
            set;
        }

        /// <summary>
        /// 较差评论
        /// </summary>

        public int TotalCount4
        {
            get;
            set;
        }


        /// <summary>
        /// 很差评论
        /// </summary>

        public int TotalCount5
        {
            get;
            set;
        }

        /// <summary>
        /// 商品评论总分
        /// </summary>
        public Product_ReviewMasterViewModel ProductReviewScore
        {
            get;
            set;
        }

        /// <summary>
        /// 评论列表
        /// </summary>

        public ProductReviewDetailListViewModel ProductReviewDetailList
        {
            get;
            set;
        }

    }
    public class ProductReviewDetailListViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public List<ProductReviewDetailItemViewModel> CurrentPageData { get; set; }
    }
    public class ProductReviewDetailItemViewModel
    {

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

        public DateTime InDate
        {
            get;
            set;
        }

        public string InDateString
        {
            get
            {
                return InDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 评论人昵称
        /// </summary>
        public string NickName { get; set; }

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

        public int ReplyCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public int ReviewType
        {
            get;
            set;
        }

        public decimal AvgScore { get; set; }
    }

    public class Product_ReviewMasterViewModel
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
        /// 评分名称
        /// </summary>
        public List<string> ScoreNameList
        {
            get;
            set;
        }
    }
}