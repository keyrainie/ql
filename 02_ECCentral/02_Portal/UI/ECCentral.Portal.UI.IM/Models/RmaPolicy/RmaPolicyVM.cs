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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class RmaPolicyVM:ModelBase
    {

        public RmaPolicyVM()
        {
            RmaPolicyTypeList = EnumConverter.GetKeyValuePairs<RmaPolicyType>(EnumConverter.EnumAppendItemType.Select);
        }

        public int? SysNo { get; set; }
        private string rmaPolicyName;
         [Validate(ValidateType.Required)]
        public string RMAPolicyName 
        {
            get { return rmaPolicyName; }
            set { SetValue("RMAPolicyName", ref rmaPolicyName, value); }
        }
        private string ecDisplayName;
         [Validate(ValidateType.Required)]
        public string ECDisplayName
        {
            get { return ecDisplayName; }
            set { SetValue("ECDisplayName", ref ecDisplayName, value); }
        }
        private string priority;
          [Validate(ValidateType.Interger)]
          [Validate(ValidateType.Required)]
        public string Priority
        {
            get { return priority; }
            set { SetValue("Priority", ref priority, value); }
        }
        private string returnDate;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public string ReturnDate 
        {
            get { return returnDate; }
            set { SetValue("ReturnDate", ref returnDate, value); }
        }
        private string changeDate;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public string ChangeDate
        {
            get { return changeDate; }
            set { SetValue("ChangeDate", ref changeDate, value); }
        }
        private string ecDisplayDesc;
        public string ECDisplayDesc
        {
            get { return ecDisplayDesc; }
            set { SetValue("ECDisplayDesc", ref ecDisplayDesc, value); }
        }
        private string ecDisplayMoreDesc;
        public string ECDisplayMoreDesc
        {
            get { return ecDisplayMoreDesc; }
            set { SetValue("ECDisplayMoreDesc", ref ecDisplayMoreDesc, value); }
        }
        private RmaPolicyType? rmaType;
          [Validate(ValidateType.Required)]
        public RmaPolicyType? RmaType
        {
            get { return rmaType; }
            set { SetValue("RmaType", ref rmaType, value); }
        }

          private bool isRequst;
          public bool IsRequest 
          {
              get { return isRequst; }
              set { SetValue("IsRequest", ref isRequst, value); }
          }

        /// <summary>
        ///控制页面，没有实际作用
        /// </summary>
          public Visibility IsDisPlay { get; set; }
          public RmaPolicyStatus? Status { get; set; }
        public List<KeyValuePair<RmaPolicyType?, string>> RmaPolicyTypeList { get; set; }
        

    }

    public class RmaPolicyComBoxVM : ModelBase
    {
        public RmaPolicyComBoxVM()
        {
            Data = new List<RmaPolicyVM>();
            RmaPolicy = new RmaPolicyVM();
        }

        private RmaPolicyVM rmaPolicy;
        public RmaPolicyVM RmaPolicy 
        {
            get { return rmaPolicy; }
            set { SetValue("RmaPolicy", ref rmaPolicy, value); }
        }

     
        private List<RmaPolicyVM> data;
        public List<RmaPolicyVM> Data 
        {
            get { return data; }
            set { SetValue("Data", ref data, value); }
        }
    }
}
