using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.IO;

namespace Newegg.Oversea.Silverlight.FileUploadHandler
{
    public abstract class FileUploadHandlerBase : IHttpHandler, IReadOnlySessionState
    {
        public FileUploadHandlerBase()
        {    
            //如果为true,执行完FileUploadCompleted事件后，会自动删除临时文件。
            this.DeleteFileAfterUpload = true;
        }

        #region IHttpHandler Members

        private HttpContext httpContext = null;
        protected HttpContext HttpContext
        {
            get
            {
                return httpContext;
            }
        }

        public bool IsReusable
        {
            get { return false; }
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
            httpContext = context;

            FileUploadProcess fileUpload = new FileUploadProcess();
            fileUpload.DeleteFileAfterUpload = this.DeleteFileAfterUpload;
            fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(fileUpload_FileUploadCompleted);
            fileUpload.ProcessRequest(context, UploadPath);
        }

        FileUploadProcessResult fileUpload_FileUploadCompleted(object sender, FileUploadCompletedEventArgs args)
        {
            var result = new FileUploadProcessResult();
            try
            {
                result = ProcessUploadedFile(sender, args);
            }
            catch (Exception ex)
            {
                result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Failed;
                result.ResponseMsg = ex.ToString();
                if (File.Exists(args.FilePath))
                {
                    File.Delete(args.FilePath);
                }
            }

            return result;
        }

        #endregion

        public bool DeleteFileAfterUpload { get; set; }

        public abstract FileUploadProcessResult ProcessUploadedFile(object sender, FileUploadCompletedEventArgs args);
    }
}