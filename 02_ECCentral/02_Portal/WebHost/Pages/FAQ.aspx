<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Newegg.Oversea.Silverlight.ControlPanel.WebHost" %>
<%@ Import Namespace="Newegg.Oversea.Framework.Log" %>
<script runat="server">
    protected override void OnInit(EventArgs e) 
    {
        string successfulMsg = "<!--Newegg-->";
        string errorMsg = "<!--Error-->";
        string dbInstanceName = System.Net.Dns.GetHostName();
        var serviceList = new List<String>();

        var serviceSettings = FAQDetector.GetServiceSetting();
        if (serviceSettings != null)
        {
            foreach (FAQServiceModel serviceSetting in serviceSettings)
            {
                serviceList.Add(serviceSetting.URL);
            }
        }
        
        var dbList = new List<FAQDBModel>();
        var dbSettings = FAQDetector.GetDBSetting();
        if (dbSettings != null)
        {
            foreach (FAQDBModel dbSetting in dbSettings)
            {
                if (dbSetting.OverrideDBInstanceName)
                {
                    dbSetting.DBInstanceName = dbInstanceName;
                }
                dbList.Add(dbSetting);
            }
        }
        
        var serviceFlag = FAQDetector.DetectServiceStatus(serviceList).Flag;
        var dbFlag = FAQDetector.DetectDBStatus(dbList).Flag;
        if (serviceFlag && dbFlag)
        {
            this.Response.Write(successfulMsg);
        }
        else 
        {
            this.Response.Write(errorMsg);
            Logger.WriteLog("Service or database crash, please see the detail information in FAQView.aspx page.", "ExceptionLog");
        }
    }
</script>