using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// OpenAPI
    /// </summary>
    public class OpenAPIMasterInfo : IIdentity, ICompany, ILanguage
    {
        #region 基础
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 合作网站名
        /// </summary>
        public string WebSiteName { get; set; }

        /// <summary>
        /// 追踪代码
        /// </summary>
        public string TraceCode { get; set; }

        /// <summary>
        /// API生成文件名称
        /// </summary>
        public string GenerateFileName { get; set; }

        /// <summary>
        /// API生成频率
        /// </summary>
        public OpenAPIGenerateInterval GenerateInterval { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        public string WebSiteUrl { get; set; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public int ProductImgSize { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime? GenerateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OpenAPIStatus Status { get; set; }

        

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperateUser { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? OperateDate { get; set; }


        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 分类清单
        /// </summary>
        public List<OpenAPICategoryInfo> OpenAPICategorys { get; set; }

        /// <summary>
        /// 语言Code
        /// </summary>
        public string LanguageCode{ get; set; }
    }

    /// <summary>
    /// OpenAPICategory
    /// </summary>
    public class OpenAPICategoryInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 分类信息
        /// </summary>
        public CategoryInfo Category { get; set; }

        /// <summary>
        /// 分类级别
        /// </summary>
        public int CategoryLevel { get; set; }

    }
}
