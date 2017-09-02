//************************************************************************
// 用户名				泰隆优选
// 系统名				查询模板管理
// 子系统名		        查询模板管理QueryModels
// 作成者				TOM
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;


namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductProfileTemplateInfoVM : ModelBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string TemplateType
        {
            get;
            set;
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 模板值
        /// </summary>
        public string TemplateValue
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 查询模板外部SysNo
        /// </summary>
        public int? ReferenceSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

    }
}
