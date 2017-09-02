<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ECCentral.Service.WebHost.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

        a
        {
            color:blue;
            text-decoration:none;
        }
        a:hover {color: red; background-color:yellow; } 
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>服务列表
     <ul>
      <%  var list = ECCentral.Service.Utility.WCF.ServiceConfig.GetAllService();
           list = list.OrderBy(p => p.Path).ToList();
           var host=Request.ServerVariables["HTTP_HOST"];
           foreach (ECCentral.Service.Utility.WCF.ServiceData item in list)
           {
              %>
             <li><a href="http://<%=host+"/"+item.Path+"/help"%>"><%=item.Path%></a></li>
         <%
           }           
         %>
        </ul>
    </div>
    </form>
</body>
</html>
