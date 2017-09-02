using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Newegg.Oversea.Silverlight.FileUploadHandler
{
    public delegate FileUploadProcessResult FileUploadCompletedEvent(object sender, FileUploadCompletedEventArgs args);

    public class FileUploadCompletedEventArgs
    {
        public string ClientFileName { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public Dictionary<string, string> UploadParams { get; private set; }
        private static Regex s_Regex = new Regex(@"\?([^|]*)\|([^?]*)");

        public FileUploadCompletedEventArgs() { }

        public FileUploadCompletedEventArgs(string clientFileName,string fileName, string filePath,string param)
        {
            ClientFileName = clientFileName;
            FileName = fileName;
            FilePath = filePath;
            UploadParams = ParseUploadParam(param);
        }

        private Dictionary<string, string> ParseUploadParam(string param)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(param))
            {
                param = HttpUtility.HtmlDecode(param);

                foreach (Match match in s_Regex.Matches(param))
                {
                    result.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }

            return result;
        }
    }

    public class FileUploadProcess
    {
        public event FileUploadCompletedEvent FileUploadCompleted;      

        public bool DeleteFileAfterUpload { get; set; }              

        public FileUploadProcess()
        {
        }

        public void ProcessRequest(HttpContext context, string uploadPath)
        {
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            context.RewritePath(context.Server.UrlDecode(context.Request.Url.PathAndQuery));
            
            string filename = context.Request.QueryString["Filename"];            
            string clientFileName = filename;
            string guid = context.Request.QueryString["Guid"];
            string uploadParam = context.Request.QueryString["Param"];
            var index = filename.LastIndexOf(".");
            var suffix = filename.Substring(index);

            bool complete = string.IsNullOrEmpty(context.Request.QueryString["Complete"]) ? true : bool.Parse(context.Request.QueryString["Complete"]);
            //bool getBytes = string.IsNullOrEmpty(context.Request.QueryString["GetBytes"]) ? false : bool.Parse(context.Request.QueryString["GetBytes"]);
            long startByte = string.IsNullOrEmpty(context.Request.QueryString["StartByte"]) ? 0 : long.Parse(context.Request.QueryString["StartByte"]); ;
            if (startByte == 0)
            {
                guid = Guid.NewGuid().ToString();
            }

            filename = guid + suffix;

            string filePath = Path.Combine(uploadPath, filename);

            if (startByte > 0 && File.Exists(filePath))
            {
                using (FileStream fs = File.Open(filePath, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);                    
                }
            }
            else
            {
                using (FileStream fs = File.Create(filePath))
                {
                    SaveFile(context.Request.InputStream, fs);                    
                }
            }

            FileUploadProcessResult processResult = new FileUploadProcessResult();
            if (complete)
            {
                if (FileUploadCompleted != null)
                {
                    FileUploadCompletedEventArgs args = new FileUploadCompletedEventArgs(clientFileName, filename, filePath, uploadParam);
                    processResult = FileUploadCompleted(this, args);
                }
                else
                {
                    processResult.Status = ProcessStatus.Success;
                }

                if (DeleteFileAfterUpload)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            var responseString = string.Format("NeweggFileUploaderResponse[^v^]{0}[^v^]{1}[^v^]{2}[^v^]{3}", 
                guid, context.Request.InputStream.Length, processResult.ResponseMsg, processResult.Status);

            context.Response.Write(responseString);
            context.Response.Flush();
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
    }
}
