<%@ Page Language="C#" %>
<%= System.Environment.MachineName %><br>
<%= System.DateTime.Now.ToString() %><br>
<%= HttpContext.Current.Request.Url.ToString() %>