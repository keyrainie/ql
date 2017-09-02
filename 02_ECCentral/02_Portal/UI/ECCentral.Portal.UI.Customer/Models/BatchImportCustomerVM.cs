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
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class BatchImportCustomerVM : ModelBase
    {
        public BatchImportCustomerVM()
        {
            this.TemplateTypeList = EnumConverter.GetKeyValuePairs<TemplateType>(EnumConverter.EnumAppendItemType.Select);
            CompanyCode = CPApplication.Current.CompanyCode;
        }
        public string CompanyCode { get; set; }

        private string m_customerSource;
        public string CustomerSource
        {
            get
            {
                return m_customerSource;
            }
            set
            {
                base.SetValue("CustomerSource", ref m_customerSource, value);
            }
        }

        private string m_ImportFilePath;
        public string ImportFilePath
        {
            get
            {
                return m_ImportFilePath;
            }
            set
            {
                base.SetValue("ImportFilePath", ref m_ImportFilePath, value);
            }
        }

        private TemplateType? m_templateType;
        [Validate(ValidateType.Required, ErrorMessageResourceName = "Message_UploadExcel_NoSelFileType", ErrorMessageResourceType = typeof(ResBatchImportCustomer))]
        public TemplateType? TemplateType
        {
            get
            {
                return m_templateType;
            }
            set
            {
                base.SetValue("TemplateType", ref m_templateType, value);
            }
        }

        private string _FromLinkSource;

        public string FromLinkSource
        {
            get { return _FromLinkSource; }
            set
            {
                base.SetValue("FromLinkSource", ref _FromLinkSource, value);
            }
        }

        public List<KeyValuePair<TemplateType?, string>> TemplateTypeList
        {
            get;
            set;
        }
    }
}