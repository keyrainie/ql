using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newegg.Oversea.Silverlight.FileUploadHandler;
using System.IO;
using Newegg.DFIS.Uploader;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost.HttpHandler
{
    /// <summary>
    /// Summary description for DFISUploadHandler
    /// </summary>
    public class DFISUploadHandler : FileUploadHandlerBase
    {
        public override FileUploadProcessResult ProcessUploadedFile(object sender, FileUploadCompletedEventArgs args)
        {
            if (!args.UploadParams.ContainsKey("DFISUploadURL"))
            {
                throw new ArgumentException("Please Specify UploadParams[\"DFISUploadURL\"] ");
            }

            if (!args.UploadParams.ContainsKey("DFISGroup"))
            {
                throw new ArgumentException("Please Specify UploadParams[\"DFISGroup\"] ");
            }

            if (!args.UploadParams.ContainsKey("DFISType"))
            {
                throw new ArgumentException("Please Specify UploadParams[\"DFISType\"] ");
            }

            if (!args.UploadParams.ContainsKey("DFISUserName"))
            {
                throw new ArgumentException("Please Specify UploadParams[\"DFISUserName\"] ");
            }

            if (!args.UploadParams.ContainsKey("DFISFileName"))
            {
                throw new ArgumentException("Please Specify UploadParams[\"DFISFileName\"] ");
            }  

            string url = args.UploadParams["DFISUploadURL"];
            string fileGroup = args.UploadParams["DFISGroup"]; 
            string fileType = args.UploadParams["DFISType"];
            string userName = args.UploadParams["DFISUserName"];
            string fileName = args.UploadParams["DFISFileName"];

            using (FileStream fileStream = new FileStream(args.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                HttpUploader.UploadFile(fileStream, fileName, url, fileGroup, fileType, string.Empty, userName, UploadMethod.Update);
            }


            FileUploadProcessResult result = new FileUploadProcessResult();
            result.Status = Newegg.Oversea.Silverlight.FileUploadHandler.ProcessStatus.Success;
            result.ResponseMsg = fileName;
            return result;
        }

    }
}