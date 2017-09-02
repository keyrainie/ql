using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Newegg.Oversea.Framework.ExceptionHandler;
using System.Reflection;
using System.IO;
using System.Globalization;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public partial class Report : System.Web.UI.Page
    {
        private static ReportConfiguration m_Config = (ReportConfiguration)ConfigurationManager.GetSection("ReportConfigruation");
        object source;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Unload += new EventHandler(Report_Unload);
        }

        void Report_Unload(object sender, EventArgs e)
        {
            try
            {
                ReportClass rc = source as ReportClass;
                if (rc != null)
                {
                    rc.Dispose();
                }
                this.OVSReportViewer.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string reportKey = "";
            
            try
            {
                //1.获取需要加载的Report实例
                reportKey = Request.QueryString["ReportKey"];

                if (!string.IsNullOrEmpty(reportKey))
                {
                    var report = m_Config.Reports[reportKey];

                    if (report != null)
                    {
                        var typeName = report.ComponentType.Split(',')[0];
                        var assemblyName = report.ComponentType.Split(',')[1];

                        var assembly = Assembly.Load(assemblyName);
                        source = assembly.CreateInstance(typeName);

                        this.Title = report.Description;
                        this.OVSReportViewer.ReportSource = source;
                    }
                    else
                    {
                        throw new Exception(string.Format("Report key [{0}] is not found in configuration file.", reportKey));

                    }
                }
                else
                {
                    throw new Exception("Can not found ReportKey in URL.");
                }
            }
            catch (Exception ex)
            {         
                ExceptionHelper.HandleException(ex, "Init Report page error.","ReportPage", new object[] { "ReportKey:" + reportKey });
                Response.Write("Exception Message:<br />" + ex.ToString());

            }
        }
    }
}