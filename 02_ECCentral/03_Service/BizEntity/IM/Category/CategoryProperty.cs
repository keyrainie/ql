//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理实体
// 子系统名		        分类属性基本信息实体
// 作成者				Tom.H.Li
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************
using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 分类属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryProperty : IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 属性
        /// </summary>
        [DataMember]
        public PropertyInfo Property { get; set; }


        /// <summary>
        /// 属性组
        /// </summary>
        [DataMember]
        public PropertyGroupInfo PropertyGroup { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 搜索优先级
        /// </summary>
        [DataMember]
        public int SearchPriority { get; set; }

        /// <summary>
        ///类型
        /// </summary>
        [DataMember]
        public PropertyType PropertyType { get; set; }

        /// <summary>
        /// 前台展示样式
        /// </summary>
        [DataMember]
        public WebDisplayStyle DisplayStyle { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [DataMember]
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 必选
        /// </summary>
        [DataMember]
        public CategoryPropertyStatus IsMustInput { get; set; }

        /// <summary>
        /// 商品管理查询
        /// </summary>
        [DataMember]
        public CategoryPropertyStatus IsItemSearch { get; set; }

        /// <summary>
        /// 高级搜索
        /// </summary>
        [DataMember]
        public CategoryPropertyStatus IsInAdvSearch { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 复制源的类别SysNo
        /// </summary>
        [DataMember]
        public int? SourceCategorySysNo { get; set; }

        /// <summary>
        /// 复制的类别SysNO
        /// </summary>
        [DataMember]
        public int? TargetCategorySysNo { get; set; }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
    }
}
