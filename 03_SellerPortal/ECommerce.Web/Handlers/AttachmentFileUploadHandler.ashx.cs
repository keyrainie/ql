using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ECommerce.Web.Handlers
{
    /// <summary>
    /// Summary description for AttachmentFileUploadHandler
    /// </summary>
    public class AttachmentFileUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            context.Response.ContentType = "text/plain";
            string filePath = string.Empty;


            string dateTimeFolderPath = string.Format("{0}/{1}", DateTime.Now.ToString("yyyy-MM"), DateTime.Now.ToString("yyyy-MM-dd"));

            HttpPostedFile imageUploadFile = context.Request.Files[0];
            int fileLength = imageUploadFile.ContentLength;
            byte[] imageFileBytes = new byte[fileLength];
            imageUploadFile.InputStream.Read(imageFileBytes, 0, fileLength);

            if (null != imageUploadFile)
            {
                string imageGuid = Guid.NewGuid().ToString();
                string uploadFileDirectory = context.Server.MapPath(string.Format("~/UploadFiles/{0}/", dateTimeFolderPath));
                if (!Directory.Exists(uploadFileDirectory))
                {
                    Directory.CreateDirectory(uploadFileDirectory);
                }
                string fileName = imageGuid + Path.GetExtension(imageUploadFile.FileName);
                string uploadProductImagePath = Path.Combine(uploadFileDirectory, imageGuid + Path.GetExtension(imageUploadFile.FileName));
                imageUploadFile.SaveAs(uploadProductImagePath);
                filePath = Path.Combine(dateTimeFolderPath, fileName).Replace('\\', '/');

            }
            context.Response.Write(filePath);
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