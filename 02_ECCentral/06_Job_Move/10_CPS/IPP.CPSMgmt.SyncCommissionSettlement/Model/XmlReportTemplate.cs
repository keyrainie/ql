using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model
{
    /// <summary>
    /// 财务应付对账 Root 节点
    /// </summary>
    [XmlRoot("root")]
    public class FincialPayReportRoot
    {
        [XmlArray("Reports")]
        [XmlArrayItem("Report")]
        public List<ReportItem> Reports { get; set; }
    }

    /// <summary>
    /// 报表元素 Report 节点
    /// </summary>
    [XmlRoot("Report")]
    public class ReportItem
    {
        [XmlAttribute("ID")]
        public string ID { get; set; }

        [XmlElement("Template")]
        public ReportTemplate Template { get; set; }
    }

    /// <summary>
    /// 报表模版 Template 节点
    /// </summary>
    [XmlRoot("Template")]
    public class ReportTemplate
    {
        [XmlElement("Style")]
        public string Style { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Header")]
        public string Header { get; set; }

        [XmlElement("Body")]
        public string Body { get; set; }

        [XmlElement("Footer")]
        public string Footer { get; set; }
    }
    
}
