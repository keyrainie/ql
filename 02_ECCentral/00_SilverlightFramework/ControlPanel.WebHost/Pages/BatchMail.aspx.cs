using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost.Pages
{
    public partial class BatchMail : System.Web.UI.Page
    {
        private string m_languageCode;

        public string LanguageCode
        {
            get
            {
                if (this.m_languageCode.Trim().ToUpper() == "EN-US")
                {
                    return "en";
                }
                else if (this.m_languageCode.Trim().ToUpper() == "ZH-CN")
                {
                    return "zh_CN";
                }
                else if (this.m_languageCode.Trim().ToUpper() == "ZH-TW")
                {
                    return "zh_TW";
                }
                else if (this.m_languageCode.Trim().ToUpper() == "JA-JP")
                {
                    return "ja-JP";
                }
                return "en";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void InitializeCulture()
        {
            var languageCode = Request.QueryString["LanguageCode"];

            if (string.IsNullOrEmpty(languageCode))
            {
                m_languageCode = "en-US";
            }
            else
            {
                this.m_languageCode = languageCode;
            }

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(languageCode);

            base.InitializeCulture();
        }
    }
}