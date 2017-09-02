<%@ Page Language="C#" %>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        String hostName = System.Net.Dns.GetHostName();

        Response.Write(hostName);
        Response.Write("<br/>");

        System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(hostName);

        foreach (System.Net.IPAddress ipAddress in ipHostEntry.AddressList)
        {
            if (ipAddress.AddressFamily.ToString().ToUpper().Trim() == "InterNetwork".ToUpper().Trim())
            {
                Response.Write(ipAddress.ToString() + "<br/>");
            }
        }

        Response.Write(System.DateTime.Now.ToString());
        Response.Write("<br/>");
        Response.Write(HttpContext.Current.Request.Url.ToString());

        base.OnPreRender(e);
    }

</script>