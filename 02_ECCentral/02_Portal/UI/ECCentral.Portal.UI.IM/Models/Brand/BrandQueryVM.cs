using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class BrandQueryVM : ModelBase
    {
        public BrandQueryVM()
        {
            this.BrandStatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
            this.AuthorizedStatusList = EnumConverter.GetKeyValuePairs<AuthorizedStatus>(EnumConverter.EnumAppendItemType.All);
            this.IsAuthorizedList = EnumConverter.GetKeyValuePairs<BooleanEnum>(EnumConverter.EnumAppendItemType.All);
            this.IsBrandStoryList = EnumConverter.GetKeyValuePairs<BooleanEnum>(EnumConverter.EnumAppendItemType.All);
        }

        private string brandNameLocal;
        public string BrandNameLocal
        {
            get
            {
                return brandNameLocal;
            }
            set
            {
                base.SetValue("BrandNameLocal", ref brandNameLocal, value);
            }
        }

        private string brandNameGlobal;
        public string BrandNameGlobal
        {
            get
            {
                return brandNameGlobal;
            }
            set
            {
                base.SetValue("BrandNameGlobal", ref brandNameGlobal, value);
            }
        }

        private ValidStatus? status;
        public ValidStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        /// <summary>
        /// 生产商编号
        /// </summary>
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        private string _priority;
        [Validate(ValidateType.Regex, @"^\d+$", ErrorMessageResourceType=typeof(ResBrandQuery),ErrorMessageResourceName="Error_ValidateIntHint")]
        public string Priority
        {
            get { return _priority; }
            set { SetValue("Priority", ref _priority, value); }
        }

        /// <summary>
        /// 是否包含授权
        /// </summary>
        public BooleanEnum? IsAuthorized { get; set; }

        /// <summary>
        /// 是否包含品牌故事
        /// </summary>
        public BooleanEnum? IsBrandStory { get; set; }

        /// <summary>
        /// 类别1SysNo
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 类别2SysNo
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 类别3SysNo
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 授权牌状态
        /// </summary>
        public AuthorizedStatus? AuthorizedStatus { get; set; }

        public List<KeyValuePair<AuthorizedStatus?, string>> AuthorizedStatusList { get; set; }

        public List<KeyValuePair<BooleanEnum?, string>> IsAuthorizedList { get; set; }

        public List<KeyValuePair<BooleanEnum?, string>> IsBrandStoryList { get; set; }

        public List<KeyValuePair<ValidStatus?, string>> BrandStatusList { get; set; }


        public bool HasBrandRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Brand_BrandRequestApply); }
        }

    }
}
