using System;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class BrandAuthorizedVM : ModelBase
    {
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        private string imageName;
        [Validate(ValidateType.Required)]
        public string ImageName
        {
            get { return imageName; }
            set { SetValue("ImageName", ref imageName, value); }
        }

        private DateTime? startActiveTime;
        [Validate(ValidateType.Required)]
        public DateTime? StartActiveTime
        {

            get { return startActiveTime; }
            set
            {

                SetValue("StartActiveTime", ref startActiveTime, value);
            }
        }
        private DateTime? endActiveTime;
        [Validate(ValidateType.Required)]
        public DateTime? EndActiveTime
        {

            get { return endActiveTime; }
            set
            {
                SetValue("EndActiveTime", ref endActiveTime, value);
            }
        }
        /// <summary>
        /// 有效
        /// </summary>
        private bool authorizedAcive = true;
        public bool AuthorizedAcive
        {
            get { return authorizedAcive; }
            set { SetValue("AuthorizedAcive", ref authorizedAcive, value); }
        }
        /// <summary>
        /// 无效
        /// </summary>
        private bool authorizedDeAcive = false;
        public bool AuthorizedDeAcive
        {
            get { return authorizedDeAcive; }
            set { SetValue("AuthorizedDeAcive", ref authorizedDeAcive, value); }
        }


        public int BrandSysNo { get; set; }

        public int? ReferenceSysNo { get; set; }

        /// <summary>
        /// 类别的名称，展示用的
        /// </summary>
        public string ReferenceName { get; set; }
        public int Type { get; set; }

        public int SysNo { get; set; }
        /// <summary>
        /// 是否已经存在该授权
        /// </summary>
        public bool IsExist { get; set; }

        public bool HasBrandStoreMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Brand_BrandStoreMaintain); }
        }
    }
}
