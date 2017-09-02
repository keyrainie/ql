using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public class GlobalSearchModel
    {
        private string m_name = null;
        private string m_tip = null;


        public List<CultureItem> NameList { get; set; }

        public List<CultureItem> TipList { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public string Name
        {
            get
            {
                if (m_name != null)
                    return m_name;

                var code = CPApplication.Current.LanguageCode;
                var name = NameList.FirstOrDefault(item => string.Equals(item.LanguageCode, code, StringComparison.OrdinalIgnoreCase));

                if (name != null)
                    m_name = name.Value;
                else
                {
                    var en = NameList.FirstOrDefault(item => string.Equals(item.LanguageCode, "en-us", StringComparison.OrdinalIgnoreCase));
                    if (en != null)
                        m_name = en.Value;
                }

                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string Tip
        {
            get
            {
                if (m_tip != null)
                    return m_tip;

                var code = CPApplication.Current.LanguageCode;
                var tip = TipList.FirstOrDefault(item => string.Equals(item.LanguageCode, code, StringComparison.OrdinalIgnoreCase));

                if (tip != null)
                    m_tip = tip.Value;
                else
                {
                    var en = TipList.FirstOrDefault(item => string.Equals(item.LanguageCode, "en-us", StringComparison.OrdinalIgnoreCase));
                    if (en != null)
                        m_tip = en.Value;
                }

                return m_tip;
            }
            set
            {
                m_tip = value;
            }
        }

        public string QuickKey { get; set; }

        public string UrlTemplate { get; set; }
        
        public string BaseUrl
        {
            get
            {
                var baseUrl = UrlTemplate;

                if (baseUrl != null)
                {
                    baseUrl = Regex.Replace(baseUrl, @"^(?<url>/[\w-\.]+/[\w-\.]+)?.*$", @"${url}", RegexOptions.IgnoreCase);
                }

                return baseUrl;
            }
        }
    }

    public class CultureItem
    {
        public string Value { get; set; }

        [System.Xml.Serialization.XmlAttribute()]
        public string LanguageCode { get; set; }
    }

}
