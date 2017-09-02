using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PropertyVM : ModelBase
    {

        public List<KeyValuePair<int, string>> PropertyStatusList { get; set; }

        public PropertyVM()
        {
            List<KeyValuePair<int, string>> statusList = new List<KeyValuePair<int, string>>();

            statusList.Add(new KeyValuePair<int, string>(1, ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<int, string>(0, ResCategoryKPIMaintain.SelectTextInvalid));

            this.PropertyStatusList = statusList;
        }

        private int _sysNo;

        /// <summary>
        /// 属性SysNo
        /// </summary>
        public int SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        /// <summary>
        /// 属性编号
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        private string propertyName;

        public string PropertyName 
        {
            get
            {
                return propertyName;
            }
            set
            {
                base.SetValue("PropertyName", ref propertyName, value);
            }
        }

        /// <summary>
        /// 属性状态
        /// </summary>
        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                int statusValue = 0;
                if (Int32.TryParse(value, out statusValue))
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
                else
                {
                    var tempEnum = (int)Enum.Parse(typeof(PropertyStatus), value, false);
                    _status = tempEnum.ToString();
                }

            }
        }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public string LastEditUser { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastEditTime { get; set; }

    }
}
