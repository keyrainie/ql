using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Utility;


namespace ECommerce.Entity.Product
{
    public class Product_ReviewList
    {





        /// <summary>
        /// 分页总数
        /// </summary>

       
        public int TotalCount
        {
            get;
            set;
        }

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
        /// 晒单数
        /// </summary>
       
        public int TotalCount6
        {
            get;
            set;
        }

        /// <summary>
        /// 商品评论总分
        /// </summary>
       
        public Product_ReviewMaster ProductReviewScore
        {
            get;
            set;
        }

        /// <summary>
        /// 最有用评论
        /// </summary>
       
        public Product_ReviewDetail UsefulReviewDetail
        {
            get;
            set;
        }

        /// <summary>
        /// 评论列表
        /// </summary>
       
        public PagedResult<Product_ReviewDetail> ProductReviewDetailList
        {
            get;
            set;
        }

        /// <summary>
        /// 晒单列表
        /// </summary>
        public PagedResult<Product_ReviewDetail> ProductOrderShowList
        {
            get;
            set;
        }

        /// <summary>
        /// 产品信息
        /// </summary>
        public ProductCellInfo ProductInfo
        {
            get;
            set;
        }

    }
}
