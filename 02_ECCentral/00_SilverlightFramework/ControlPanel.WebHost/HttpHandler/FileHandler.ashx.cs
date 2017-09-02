using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost.HttpHandler
{
    /// <summary>
    /// Uploader Http Handler
    /// </summary>
    public class FileHandler : IHttpHandler
    {

        private HttpContext httpContext = null;
        protected HttpContext HttpContext
        {
            get
            {
                return httpContext;
            }
        }

        public string UploadPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["UploadFilePath"] != null)
                {
                    return httpContext.Server.MapPath(ConfigurationManager.AppSettings["UploadFilePath"]);
                }
                else
                {
                    return httpContext.Server.MapPath("~/UploadFile");
                }
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            this.httpContext = context;

            var subDomain = context.Request.Params["SubDomain"];
            var uploadPath = Path.Combine(this.UploadPath, subDomain);

            var count = context.Request.Files.Count;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var file = context.Request.Files[i];

                    if (file.ContentLength > 0)
                    {
                        var fileName = GetFileName(file.FileName);

                        var filePath = Path.Combine(uploadPath, fileName);
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        using (FileStream fs = File.Create(filePath))
                        {
                            SaveFile(file.InputStream, fs);
                        }
                    }

                }
                context.Response.Write("Y");
            }
            catch
            {
                var files = Directory.GetFiles(uploadPath);

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                context.Response.Write("N");
            }
            finally
            {
                context.Response.Flush();
            }
        }

        private string GetFileName(string filePath)
        {
            var paths = filePath.Split('\\');

            return paths[paths.Length - 1];
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}