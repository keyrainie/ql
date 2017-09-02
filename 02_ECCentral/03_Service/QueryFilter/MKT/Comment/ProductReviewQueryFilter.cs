using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 产品评论
    /// </summary>
    public class ProductReviewQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        public int? ProductCategory { get; set; }

        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 蛋数
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 标题或正文关键字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 更新时间开始
        /// </summary>
        public DateTime? EditDateFrom { get; set; }

        /// <summary>
        /// 更新时间结束
        /// </summary>
        public DateTime? EditDateTo { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// CustomerID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// ProductID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 是否有用候选
        /// </summary>
        public int? MostUseFulCandidate { get; set; }

        /// <summary>
        /// 是否最有用
        /// </summary>
        public int? MostUseFul { get; set; }

        /// <summary>
        /// 用户有用评价数
        /// </summary>
        public int? UsefulCount { get; set; }

        /// <summary>
        /// 处理人员
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// CS处理状态
        /// </summary>
        public ReviewProcessStatus? ComplainStatus { get; set; }

        /// <summary>
        /// 按组查询 
        /// </summary>
        public bool IsByGroup { get; set; }
        /// <summary>
        /// 商品组编号
        /// </summary>
        public int? ProductGroupNo { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public int? ManufacturerName { get; set; }

        /// <summary>
        /// 顾客类型
        /// </summary>
        public int? CustomerCategory { get; set; }


        /// <summary>
        ///商家ID--对应数据库 VendorID
        /// </summary>
        public int? VendorType { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public int? PMUserSysNo { get; set; }


        /// <summary>
        /// 置顶
        /// </summary>
        public YNStatus? IsTop { get; set; }
        /// <summary>
        /// 置底
        /// </summary>
        public YNStatus? IsBottom { get; set; }

        /// <summary>
        /// 是否精华
        /// </summary>
        public YNStatus? IsDigest { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public int? ProductStatus { get; set; }
        /// <summary>
        /// 评论状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 首页服务热评
        /// </summary>
        public YNStatus? IsIndexPageServiceHotComment { get; set; }

        /// <summary>
        /// 首页热评
        /// </summary>
        public YNStatus? IsIndexPageHotComment { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        public ReviewType? ReviewType { get; set; }
    }
}
