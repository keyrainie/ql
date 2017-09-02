using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace GiveERPCustomerPoint.Entities
{
    [Serializable]
    [XmlRoot("CRMQZJConfig")]
    public class CRMQZJConfig
    {
        [XmlElement("CRMIP")]
        public string CRMIP;
        [XmlElement("CRMPORT")]
        public string CRMPORT;
        [XmlElement("CRMPOSTYPE")]
        public string CRMPOSTYPE;
        [XmlElement("CRMPOSNO")]
        public string CRMPOSNO;
        [XmlElement("CRMSHDM")]
        public string CRMSHDM;
        [XmlElement("CRMMDDM")]
        public string CRMMDDM;
        [XmlElement("CRM_LOCALTION_CODE")]
        public string CRM_LOCALTION_CODE;
        [XmlElement("CRMHYKTYPE")]
        public string CRMHYKTYPE;
        [XmlElement("B2CVIPTYPE")]
        public string B2CVIPTYPE;
        [XmlElement("CRM_BFCRM_USER")]
        public string CRM_BFCRM_USER;
        [XmlElement("CRM_PASSWORD")]
        public string CRM_PASSWORD;
    }

}
