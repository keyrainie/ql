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
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Models
{
    public class TariffInfoQueryFilterVM : ModelBase
    {
        public TariffInfoQueryFilterVM()
        {

            this.StatusList = EnumConverter.GetKeyValuePairs<TariffStatus>(EnumConverter.EnumAppendItemType.All);

        }

        private string _SysNo;
        [Validate(ValidateType.Interger)]
        public string SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }


        private string _ItemCategoryName;
        public string ItemCategoryName
        {
            get { return _ItemCategoryName; }
            set { base.SetValue("ItemCategoryName", ref _ItemCategoryName, value); }
        }

        private string _TariffCode;
        public string TariffCode
        {
            get { return _TariffCode; }
            set { base.SetValue("TariffCode", ref _TariffCode, value); }
        }


        private string _TariffRate;
        public string TariffRate
        {
            get { return _TariffRate; }
            set { base.SetValue("TariffRate", ref _TariffRate, value); }
        }

        public int? ParentSysNo { get; set; }

        public TariffStatus? Status { get; set; }

        public List<KeyValuePair<TariffStatus?, string>> StatusList { get; set; }

        /// <summary>
        /// 等于0  查所有税率信息，不等于0  查询有效的税率信息
        /// </summary>
        public int? Code { get; set; }
    }
}
