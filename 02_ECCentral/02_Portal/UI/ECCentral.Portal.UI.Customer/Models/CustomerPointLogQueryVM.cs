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
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerPointLogQueryVM : ModelBase
    {
        public CustomerPointLogQueryVM()
        {
            //this.PointLogTypes = EnumConverter.GetKeyValuePairs<AdjustPointType>(EnumConverter.EnumAppendItemType.All);
            this.YNList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            IsUseCreateDate = true;
        }


        private bool isUseCreateDate;
        public bool IsUseCreateDate
        {
            get
            {
                return isUseCreateDate;
            }
            set
            {
                base.SetValue("IsUseCreateDate", ref isUseCreateDate, value);
            }
        }

        private DateTime? createTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get
            {
                return createTimeFrom;
            }
            set
            {
                base.SetValue("CreateTimeFrom", ref createTimeFrom, value);
            }
        }

        private DateTime? createTimeTo;
        public DateTime? CreateTimeTo
        {
            get
            {
                return createTimeTo;
            }
            set
            {
                base.SetValue("CreateTimeTo", ref createTimeTo, value);
            }
        }

        private string customerSysNo;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }

        private YNStatus? isCashPoint;
        public YNStatus? IsCashPoint
        {
            get
            {
                return isCashPoint;
            }
            set
            {
                base.SetValue("IsCashPoint", ref isCashPoint, value);
            }
        }

        private string orderSysNo;
        [Validate(ValidateType.Interger)]
        public string OrderSysNo
        {
            get
            {
                return orderSysNo;
            }
            set
            {
                base.SetValue("OrderSysNo", ref orderSysNo, value);
            }
        }

        private int? pointType;
        public int? PointType
        {
            get
            {
                return pointType;
            }
            set
            {
                base.SetValue("PointType", ref pointType, value);
            }
        }

        private int? resultType;
        /// <summary>
        /// -1:消费记录；1:获取记录
        /// </summary>
        public int? ResultType
        {
            get
            {
                return resultType;
            }
            set
            {
                base.SetValue("ResultType", ref resultType, value);
            }
        }



        public List<CodeNamePair> PointLogTypes { get; set; }
        public List<KeyValuePair<YNStatus?, string>> YNList { get; set; }
    }
}
