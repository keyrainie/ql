using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    [Serializable]
    [DataContract]
    public class Product_ReviewConditionsInfo
    {
        /// <summary>
        /// 导航ID,用于all review page小类过滤
        /// </summary>
       
        public string NavigationKey
        {
            get;
            set;
        }

        /// <summary>
        /// 分页信息
        /// </summary>

        public PageInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 排序方式
        /// </summary>
       
        public SortingInfo SortingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// review search 关键字
        /// </summary>
       
        public string Keyword
        {
            get;
            set;
        }

        /// <summary>
        /// review是否有用
        /// </summary>
       
        public int Useful
        {
            get;
            set;
        }

        /// <summary>
        /// product sysno
        /// </summary>
       
        public int ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品分组Sysno
        /// </summary>
       
        public int ProductGroupSysno
        {
            get;
            set;
        }
    }
}
