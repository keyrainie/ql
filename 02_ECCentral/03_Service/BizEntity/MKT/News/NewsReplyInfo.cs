using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 新闻回复内容
    /// </summary>
    public class NewsReplyInfo : IIdentity, IWebChannel, ILanguage
    {
        /// <summary>
        /// 新闻系统编号
        /// </summary>
        public int  NewsSysNo{ get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public LanguageContent ReplyContent { get; set; }

        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 顾客回复主题时的IP地址
        /// </summary>
        public string CustomerIP { get; set; }

        /// <summary>
        /// 新闻类型
        /// </summary>
        public int NewsType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// CS是否已经回复
        /// </summary>
        public bool IsAnswered { get; set; }

        /// <summary>
        /// 客服回复内容
        /// </summary>
        public LanguageContent AnswerContent { get; set; }

        /// <summary>
        /// 顾客评论时上传的图片链接
        /// </summary>
        public string ImageUrl { get; set; }

        #region IIdentity Members

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        #endregion

        /// <summary>
        /// 多语言
        /// </summary>
        public string LanguageCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
