using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class Product_ReviewQueryInfo
    {
        public Product_ReviewQueryInfo()
        {
            PagingInfo = new PageInfo();
            SearchType = new List<ReviewScoreType>();
        }

        #region [ Properties ]

        /// <summary>
        /// 评论状态
        /// 0:所有，1：已评论，2：待评论
        /// </summary>
       
        public int ReviewStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 评论编号
        /// </summary>
       
        public int ReviewSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品编号
        /// </summary>
       
        public int ProductSysNo
        {
            get;
            set;
        }
      

        /// <summary>
        /// 商品Code
        /// </summary>
       
        public string ProductCode
        {
            get;
            set;
        }

        /// <summary>
        /// 商品组编号
        /// </summary>
       
        public int ProductGroupSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
       
        public int CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets paging info
        /// </summary>

        public PageInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 查询类别
        /// </summary>
       
        public List<ReviewScoreType> SearchType
        {
            get;
            set;
        }
        /// <summary>
        /// 评论类型
        /// </summary>
       
        public ReviewType ReviewType
        {
            get;
            set;
        }
        /// <summary>
        /// 有用
        /// </summary>
       
        public int Useful
        {
            get;
            set;
        }

        /// <summary>
        /// 排序
        /// </summary>
       
        public ReviewSortType SortType
        {
            get;
            set;
        }

        /// <summary>
        /// 需要很好评论个数、一般评论个数等等
        /// </summary>
       
        public bool NeedOtherTotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 需要显示回复信息条数
        /// </summary>
       
        public int NeedReplyCount
        {
            get;
            set;
        }

        /// <summary>
        /// 关键字
        /// </summary>
       
        public string KeyWord
        {
            get;
            set;
        }

        /// <summary>
        /// 查询类型
        /// </summary>
       
        public ReviewQueryType QueryType
        {
            get;
            set;
        }



        #endregion
    }
}
