using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer.VendorPortal
{
    public class VendorPortalSSBSender
    {
        #region Properties

        public string MessageHeaderXml
        {
            get;
            private set;
        }

        public string MessageBodyXml
        {
            get;
            private set;
        }

        public VendorPortalMessageBodyBase MessageBody
        {
            get;
            private set;
        }

        public VendorPortalMessageHeader MessageHeader
        {
            get;
            private set;
        }

        private string FromService
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_FromService");
            }
        }

        private string[] ToService
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_ToService").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private string ArticleCategory
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_ArticleCategory");
            }
        }

        private string ArticleType1
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_ArticleType1");
            }
        }

        private string ArticleType2
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_ArticleType2");
            }
        }

        private string DataBaseName
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_DataBaseName");
            }
        }

        private string LanguageCode
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_LanguageCode");
            }
        }

        private string Sender
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Sender");
            }
        }

        private string CompanyCode
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_CompanyCode");
            }
        }

        private string Namespace
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Namespace");
            }
        }

        private string Version
        {
            get
            {
                return AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_Version");
            }
        }

        #endregion Properties

        #region Constructor

        public VendorPortalSSBSender(VendorPortalMessageBodyBase body)
        {
            this.MessageBody = body;
        }

        #endregion Constructor

        #region Public Methods

        public void Send()
        {
            CheckArguments();

            string xmlData = this.BuildSendMessageXml();

            SSBSender.SendV3(this.FromService, this.ToService, this.ArticleCategory, this.ArticleType1, this.ArticleType2, xmlData, this.DataBaseName);
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckArguments()
        {
            if (this.MessageBody == null)
            {
                throw new ArgumentNullException("Message body is null");
            }
            if (string.IsNullOrWhiteSpace(this.MessageBody.MsgType))
            {
                throw new ArgumentException("MsgType is required");
            }
            if (string.IsNullOrWhiteSpace(this.Version))
            {
                throw new ArgumentException("Version is required");
            }
        }

        private string BuildSendMessageXml()
        {
            this.BuildMessageHeaderXml();
            this.BuildMessageBodyXml();
            return string.Format("<RequestRoot>{0}</RequestRoot>", (this.MessageHeaderXml + string.Format("<Body>{0}</Body>", this.MessageBodyXml)));
        }

        private void BuildMessageHeaderXml()
        {
            VendorPortalMessageHeader header = new VendorPortalMessageHeader()
            {
                Version = this.Version,
                Type = MessageBody.MsgType,
                Comment = MessageBody.Comment,
                Tag = MessageBody.Tag,
                OriginalGUID = Guid.NewGuid().ToString(),
                CompanyCode = this.CompanyCode,
                Language = this.LanguageCode,
                Sender = this.Sender,
                Namespace = this.Namespace
            };
            string headerXmlData = header.ToXmlString();

            this.MessageHeader = header;
            this.MessageHeaderXml = this.RemoveNameSpace(headerXmlData);
        }

        private void BuildMessageBodyXml()
        {
            string bodyXmlData = MessageBody.ToXmlString();
            this.MessageBodyXml = this.RemoveNameSpace(bodyXmlData);
        }

        private string RemoveNameSpace(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            foreach (XElement element in document.Root.DescendantsAndSelf())
            {
                if (element.Name.Namespace != XNamespace.None)
                {
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);
                }
                if (element.Attributes().Where<XAttribute>(delegate(XAttribute a)
                {
                    if (!a.IsNamespaceDeclaration)
                    {
                        return (a.Name.Namespace != XNamespace.None);
                    }
                    return true;
                }).Any<XAttribute>())
                {
                    element.ReplaceAttributes(element.Attributes().Select<XAttribute, XAttribute>(delegate(XAttribute a)
                    {
                        if (a.IsNamespaceDeclaration)
                        {
                            return null;
                        }
                        if (!(a.Name.Namespace != XNamespace.None))
                        {
                            return a;
                        }
                        return new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value);
                    }));
                }
            }
            return document.ToString();
        }

        #endregion Private Methods
    }
}