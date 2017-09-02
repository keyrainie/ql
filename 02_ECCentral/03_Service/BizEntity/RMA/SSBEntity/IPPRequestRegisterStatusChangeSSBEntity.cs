using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// IPPRequestRegisterStatusChangeSSBEntity
    /// </summary>
    [XmlRoot("RequestInfo")]
    //public class IPPRequestRegisterStatusChangeSSBEntity : IPP.Framework.SSB.SSBEntityBaseV3
    public class IPPRequestRegisterStatusChangeSSBEntity
    {
        /// <summary>
        /// RequestSysNo
        /// </summary>
        [XmlElement("RequestSysNo")]
        public int RequestSysNo { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [XmlElement("Status")]
        public string Status { get; set; }

        /// <summary>
        /// RegisterInfo
        /// </summary>
        [XmlElement("RegisterList")]
        public List<RegisterInfo> RegisterInfoList { get; set; }
    }

    /// <summary>
    /// RegisterInfo
    /// </summary>
    public class RegisterInfo
    {
        /// <summary>
        /// RegisterSysNo
        /// </summary>
        [XmlElement("RegisterSysNo")]
        public int RegisterSysNo { get; set; }

    }
}