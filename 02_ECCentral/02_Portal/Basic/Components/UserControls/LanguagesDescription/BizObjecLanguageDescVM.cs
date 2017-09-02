using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription
{
    public class BizObjecLanguageDescVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string bizObjectType;
        public string BizObjectType
        {
            get { return bizObjectType; }
            set { base.SetValue("BizObjectType", ref bizObjectType, value); }
        }

        public string ShowBizObjectTypeName { get; set; }

        /// <summary>
        /// 改为string类型。商品ID以及商家ID都为string，不改变原有的逻辑
        /// </summary>
        private int? _bizObjectSysNo;
        public int? BizObjectSysNo
        {
            get { return _bizObjectSysNo; }
            set { base.SetValue("BizObjectSysNo", ref _bizObjectSysNo, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _bizObjectId;
        public string BizObjectId
        {
            get { return _bizObjectId; }
            set { base.SetValue("BizObjectId", ref _bizObjectId, value); }
        }

        private string _languageCode;
        public string LanguageCode
        {
            get { return _languageCode; }
            set { base.SetValue("LanguageCode", ref _languageCode, value); }
        }

        public string ShowLanguageTypeName { get; set; }

        
        public string ShowBizObjectNo
        {
            get
            {
                string showBizObjectNo=string.Empty;
                
                if (BizObjectType == "Product")
                {
                    showBizObjectNo = BizObjectId;
                }
                if (BizObjectType == "Merchant" && BizObjectSysNo.HasValue)
                {
                    showBizObjectNo = BizObjectSysNo.Value.ToString();
                }
                return showBizObjectNo;
            }
        }

        private string _description;
        [Validate(ValidateType.Required)]
        public string Description
        {
            get { return _description; }
            set { base.SetValue("Description", ref _description, value); }
        }




    }
}
