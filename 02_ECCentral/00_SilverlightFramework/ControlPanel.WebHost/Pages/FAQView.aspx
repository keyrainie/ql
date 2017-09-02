<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Newegg.Oversea.Silverlight.ControlPanel.WebHost" %>
<script runat="server">
    protected FAQServiceResult ServiceResult { get; set; }
    protected FAQDBResult DBResult { get; set; }
    protected long MaxResponseTime 
    { 
        get 
        { 
            return 500;
        } 
    }
    
    protected override void OnInit(EventArgs e)
    {
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
        
        ServiceResult = FAQDetector.DetectServiceStatus(serviceList);
        DBResult = FAQDetector.DetectDBStatus(dbList);
    }
</script>

<html>
<head>
<title>FAQ View Page</title>
<style type="text/css">
html, body
{
	padding: 0;
	margin: 1;
	width: 100%;
	font-size: 12px;
	font-family: Verdana,Times New Roman;
}
.normal
{
	background-color: #8EC939;
}
.error
{
	background-color: #DF4214;
}
.slow
{
	background-color: #FAF969;
}
</style>
</head>
<body>
	<div><h1>Service Status</h1></div>
	<table id="Service" border="1" cellspacing="0" cellpadding="1">
		<tr style="background-color:#91E8FB">
			<td>Service Url</td>
			<td>Success Flag</td>
			<td>Response Time(ms)</td>
			<td>Http Status Code</td>
			<td>Http Status Description</td>
			<td>Exception Msg</td>
		</tr>
        <% foreach (var serviceModel in ServiceResult.Detail)
             {
                 if (serviceModel.IsSuccessful == true)
                 { 
                     if (serviceModel.ResponseTime > MaxResponseTime)
                     { %>
                     <tr class="slow">
                     <%}
                     else
                     {%>
                     <tr class="normal">
                     <%}
                %>
                    <td><%= serviceModel.URL%></td>
                    <td><%= serviceModel.IsSuccessful.ToString()%></td>
                    <td><%= serviceModel.ResponseTime.ToString()%></td>
                    <td><%= serviceModel.StatusCode%></td>
                    <td><%= serviceModel.StatusDescrption%></td>
                    <td>N/A</td>
            <%}
                 else
                 {%>
                        <tr class="error"> 
                        <td><%= serviceModel.URL%></td>
                        <td><%= serviceModel.IsSuccessful.ToString()%></td>
                        <td><%= serviceModel.ResponseTime.ToString()%></td>
                        <td><%= serviceModel.StatusCode%></td>
                        <td><%= serviceModel.StatusDescrption%></td>
                        <td><%= serviceModel.ExceptionMessage%></td>     
              <%}%>
         <%}%>
		</tr>
	</table>
	<div><h1>DB Status</h1></div>
	<table id="DB" border="1" cellspacing="0" cellpadding="1">
		<tr style="background-color:#91E8FB">
			<td>DB Instance</td>
			<td>DB Name</td>
			<td>Exceute SQL</td>
			<td>Response Time(ms)</td>
			<td>Exception Msg</td>
		</tr>
		<% foreach (var dbModel in DBResult.Detail)
             {
                 if (dbModel.IsSuccessful == true)
                 {
                     if (dbModel.ResponseTime > MaxResponseTime)
                     { %>
                     <tr class="slow">
                     <%}
                     else
                     {%>
                     <tr class="normal">
                     <%}
                %>
                    <td><%= dbModel.DBInstanceName%></td>
                    <td><%= dbModel.DBName%></td>
                    <td><%= dbModel.SqlScript%></td>
                    <td><%= dbModel.ResponseTime%></td>
                    <td>N/A</td>
            <%}
                 else
                 {%>
                        <tr class="error"> 
                        <td><%= dbModel.DBInstanceName%></td>
                        <td><%= dbModel.DBName%></td>
                        <td><%= dbModel.SqlScript%></td>
                        <td><%= dbModel.ResponseTime%></td>
                        <td><%= dbModel.ExceptionMessage%></td>   
              <%}%>
         <%}%>
		</tr>
	</table>
</body>
</html>