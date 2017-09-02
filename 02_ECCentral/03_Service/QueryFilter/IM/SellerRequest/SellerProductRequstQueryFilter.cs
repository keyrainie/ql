using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using System;

namespace ECCentral.QueryFilter.IM
{
    /// <summary>
    /// 商品信息
    /// </summary>
    public class SellerProductRequestQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime? RequestStartDate { get; set; }

        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime? RequestEndDate { get; set; }    

        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 需求类型
        /// </summary>
        public SellerProductRequestType Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public  SellerProductRequestStatus Status { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// CommonSKUNumber
        /// </summary>
        public string CommonSKUNumber { get; set; }

        /// <summary>
        /// 是否含有图片
        /// </summary>
        public SellerProductRequestTakePictures IsTakePictures { get; set; }

    }
} 
