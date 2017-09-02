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
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PropertyValueVM : ModelBase
    {

        public List<KeyValuePair<int, string>> PropertyValueStatusList { get; set; }

        public PropertyValueVM()
        {
            List<KeyValuePair<int, string>> statusList = new List<KeyValuePair<int, string>>();

            statusList.Add(new KeyValuePair<int, string>(1, ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<int, string>(0, ResCategoryKPIMaintain.SelectTextInvalid));

            this.PropertyValueStatusList = statusList;

        }

        
        /// <summary>
        /// 属性值SysNo
        /// </summary>
        public int SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 属性值编号
        /// </summary>
        //public int ValueID { get; set; }

        /// <summary>
        /// 属性SysNo
        /// </summary>
        public int PropertySysNo { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性值描述
        /// </summary>
        private string valueDescription;

        [Validate(ValidateType.Required)]
        public string ValueDescription 
        {
            get
            {
                return valueDescription;
            }
            set
            {
                base.SetValue("ValueDescription", ref valueDescription, value);
            }
        }

        /// <summary>
        /// 属性状态
        /// </summary>
        private string _status;

        /// <summary>
        /// 属性值状态
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                bool result = Enum.IsDefined(typeof(PropertyStatus), Convert.ToInt32(value));
                if (result)
                {
                    PropertyStatus status;
                    Enum.TryParse(value, out status);
                    _status = EnumExtension.ToDescription(status);
                }
                else
                {
                    _status = "";
                }
            }
        }

        /// <summary>
        /// 属性值优先级
        /// </summary>

        private string priority;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, new object[] {"^\\d+$"}, ErrorMessageResourceType = typeof (ResPropertyValueMaintain), ErrorMessageResourceName = "PriorityErrorInfo")]
        public string Priority 
        {
            get
            {
                return priority;
            }
            set
            {
                base.SetValue("Priority", ref priority, value);
            }
        }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastEditTime { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public string LastEditUser { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PropertyValueVM)
            {
                var o = obj as PropertyValueVM;
                if (o.SysNo == SysNo)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
