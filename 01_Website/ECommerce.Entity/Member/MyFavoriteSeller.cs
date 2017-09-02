using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class MyFavoriteSeller
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 卖家编号
        /// </summary>
        public int SellerSysNo { get; set; }

        /// <summary>
        /// 卖家Logo
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// 卖家名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 卖家可售商品个数
        /// </summary>
        public int ItemCount { get; set; }

        public string UIItemCount
        {
            get
            {
                return string.Format("共{0}件商品", ItemCount);
            }
        }
    }
}
