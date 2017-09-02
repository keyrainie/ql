using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.FileUploader
{
    public delegate void UploadStartedEvent(object sender, EventArgs args);

    public delegate void UploadCompletedEvent(object sender, UploadCompletedEventArgs args);

    public delegate void EachFilePreStartUploadEvent(object sender, UploadPreStartEventArgs args);

    public delegate void AllUploadCompletedEvent(object sender, AllUploadCompletedEventArgs args);

    public delegate void CheckUploadFileEvent(object sender, CheckUploadFileEventArgs args);

    public delegate void UploadCanceledEvent(object sender, UploadCanceledEventArgs args);


    public class CheckUploadFileEventArgs : EventArgs
    {
        public List<FileInfo> Files { get; private set; }

        public bool CheckResult { get; set; }

        public CheckUploadFileEventArgs(List<FileInfo> files)
        {
            CheckResult = true;
            Files = files;
        }
    }


    public class UploadPreStartEventArgs : EventArgs
    {
        public string FileName { get; private set; }

        public FileInfo File { get; private set; }

        public UploadPreStartEventArgs(string fileName, FileInfo file)
        {
            FileName = fileName;
            File = file;
        }
    }

    public class AllUploadCompletedEventArgs : EventArgs
    {
        public List<UploadCompletedEventArgs> UploadInfo { get; set; }

        public AllUploadCompletedEventArgs()
        {
            UploadInfo = new List<UploadCompletedEventArgs>();
        }

        public AllUploadCompletedEventArgs(List<UploadCompletedEventArgs> uploadInfo)
        {
            UploadInfo = uploadInfo;
        }
    }

    public class UploadCompletedEventArgs : EventArgs
    {
        public string FileName { get; private set; }

        public string ResponseMsg { get; private set; }

        public UploadResult UploadResult { get; private set; }


        public UploadCompletedEventArgs(UploadResult uploadResult, string fileName, string responseMsg)
            : base()
        {
            this.UploadResult = uploadResult;
            this.FileName = fileName;
            this.ResponseMsg = responseMsg;
        }
    }

    public enum UploadResult
    {
        Success,
        Failed
    }

    public class UploadCanceledEventArgs : EventArgs
    {
        public List<FileInfo> CanceledFiles { get; private set; }

        public UploadCanceledEventArgs(List<FileInfo> canceledFiles)
        {
            CanceledFiles = canceledFiles;
        }
    }
}
