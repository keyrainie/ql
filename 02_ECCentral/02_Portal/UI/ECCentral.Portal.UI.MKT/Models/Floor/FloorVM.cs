using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models.Floor
{
    public class FloorVM : ModelBase
    {
        public FloorVM()
        {
            ADStatusList = EnumConverter.GetKeyValuePairs<ADStatus>();
            PageTypes = new List<KeyValuePair<string, string>>();
            Status = ADStatus.Active;
        }

        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private int templateSysNo;

        public int TemplateSysNo
        {
            get { return templateSysNo; }
            set { base.SetValue("TemplateSysNo", ref templateSysNo, value); }
        }

        private string floorName;
        [Validate(ValidateType.Required)]
        public string FloorName
        {
            get { return floorName; }
            set { base.SetValue("FloorName", ref floorName, value); }
        }

        private string priority;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public string Priority
        {
            get { return priority; }
            set { base.SetValue("Priority", ref priority, value); }
        }

        private string floorLogoSrc;
        public string FloorLogoSrc
        {
            get { return floorLogoSrc; }
            set { base.SetValue("FloorLogoSrc", ref floorLogoSrc, value); }
        }

        private string remark;
        public string Remark
        {
            get { return remark; }
            set { base.SetValue("Remark", ref remark, value); }
        }

        private PageCodeType? pageType;
        [Validate(ValidateType.Required)]
        public PageCodeType? PageType
        {
            get { return pageType; }
            set { base.SetValue("PageType", ref pageType, value); }
        }

        private string pageCode;
        [Validate(ValidateType.Required)]
        public string PageCode
        {
            get { return pageCode; }
            set { base.SetValue("PageCode", ref pageCode, value); }
        }

        private string pageName;
        public string PageName
        {
            get { return pageName; }
            set { base.SetValue("PageName", ref pageName, value); }
        }

        public string PageCodeName { get; set; }

        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        public List<KeyValuePair<ADStatus?, string>> ADStatusList { get; set; }
        public string FloorNo { get; set; }
        public string TemplateName { get; set; }
        public List<FloorTemplate> Templates { get; set; }
        public List<KeyValuePair<string, string>> PageTypes { get; set; }
    }
}
