using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品价格举报
    /// </summary>
    public class ProductPriceCompareEntity : IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户提交的价格
        /// </summary>
        public decimal UserSubmittedPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal SellPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string InternetURL
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string CustomerName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        ///客户邮件地址
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// 无效的原因
        /// </summary>
        public int InvalidReason
        {
            get;
            set;
        }

        /// <summary>
        /// 举报状态
        /// </summary>
        public ProductPriceCompareStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// 显示链接的状态
        /// </summary>
        public DisplayLinkStatus DisplayLinkStatus
        {
            get;
            set;
        }
    }
}
