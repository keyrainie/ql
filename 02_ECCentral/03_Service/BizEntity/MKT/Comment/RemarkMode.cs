using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 评论模式设置
    /// </summary>
    public class RemarkMode : IIdentity, IWebChannel
    {
        /// <summary>
        /// 类型
        /// P=评论
        ///R=公告及促销
        ///D=网友讨论
        ///C=购物咨询
        /// </summary>
        public RemarksType RemarkType { get; set; }

        /// <summary>
        /// 所有三级类别
        /// </summary>
        public int? RemarkID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public RemarkTypeShow Status { get; set; }

        /// <summary>
        /// 类别3
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 三级类别名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 节假日自动展示
        /// </summary>
        public YesOrNoBoolean WeekendRule { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
