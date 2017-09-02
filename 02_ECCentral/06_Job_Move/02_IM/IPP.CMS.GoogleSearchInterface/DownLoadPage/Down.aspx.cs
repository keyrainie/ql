using System;
using System.IO;
using System.Text;

using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string downLoadPath = ConfigurationManager.AppSettings["DownLoadPath"].ToString();

        if (String.IsNullOrEmpty(downLoadPath) || !File.Exists(downLoadPath))
        {
            Response.Write("DownLoad Files is not Exists!!!");
            return;
        }

        FileInfo DownloadFile = new FileInfo(downLoadPath);
        Response.Clear();
        Response.ClearHeaders();
        Response.Buffer = false;
        Response.ContentType = "application/octet-stream";
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(DownloadFile.Name, System.Text.Encoding.UTF8));
        Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
        Response.WriteFile(DownloadFile.FullName);
        Response.Flush();
        Response.End();
    }
}