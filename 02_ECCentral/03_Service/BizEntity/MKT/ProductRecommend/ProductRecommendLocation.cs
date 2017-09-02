using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 商品推荐位置
    /// </summary>
    public class ProductRecommendLocation : IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel
        {
            get;
            set;
        }


        /// <summary>
        /// 页面类型ID
        /// </summary>
        public int? PageType
        {
            get;
            set;
        }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID
        {
            get;
            set;
        }

        /// <summary>
        /// 位置编号
        /// </summary>
        public int PositionID { get; set; }

        /// <summary>
        /// 推荐位置描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}