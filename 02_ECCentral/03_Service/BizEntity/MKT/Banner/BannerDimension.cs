using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 广告尺寸
    /// </summary>
    public class BannerDimension : IIdentity,IWebChannel
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
        /// 页面类别
        /// </summary>
        public int? PageTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 广告尺寸编号
        /// </summary>
        public int? PositionID
        {
            get;
            set;
        }

        /// <summary>
        /// 广告尺寸名称
        /// </summary>
        public string PositionName
        {
            get;
            set;
        }

        /// <summary>
        /// 广告尺寸长
        /// </summary>
        public int? Width
        {
            get;
            set;
        }

        /// <summary>
        /// 广告尺寸宽
        /// </summary>
        public int? Height
        {
            get;
            set;
        }
    }
}
