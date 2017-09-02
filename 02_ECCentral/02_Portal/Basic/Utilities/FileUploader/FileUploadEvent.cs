using System;
using System.IO;

namespace ECCentral.Portal.Basic.Utilities
{
    public class UploadProgressChangedEventArgs
    {
        public FileInfo FileInfo
        {
            get;
            internal set;
        }

        public long TotalUploadedDataLength
        {
            get;
            internal set;
        }

        public string FileIdentity
        {
            get;
            internal set;
        }
    }

    public delegate void UploadProgressChangedEvent(object sender, UploadProgressChangedEventArgs args);

    public class UploadErrorOccuredEventEventArgs
    {
        public FileInfo FileInfo
        {
            get;
            internal set;
        }

        public string FileIdentity
        {
            get;
            internal set;
        }

        public Exception Exception
        {
            get;
            internal set;
        }

        public long TotalUploadedDataLength
        {
            get;
            internal set;
        }
    }

    public delegate void UploadErrorOccuredEvent(object sender, UploadErrorOccuredEventEventArgs args);

    public class UploadCompletedEventArgs
    {
        public FileInfo FileInfo
        {
            get;
            internal set;
        }

        public string FileIdentity
        {
            get;
            internal set;
        }

        public long TotalUploadedDataLength
        {
            get;
            internal set;
        }

        public string ServerFilePath { get; set; }
    }

    public delegate void UploadCompletedEvent(object sender, UploadCompletedEventArgs args);

    public class UploadSuspendedEventArgs
    {
        public FileInfo FileInfo
        {
            get;
            internal set;
        }

        public string FileIdentity
        {
            get;
            internal set;
        }

        public long TotalUploadedDataLength
        {
            get;
            internal set;
        }
    }

    public delegate void UploadSuspendedEvent(object sender, UploadSuspendedEventArgs args);

    public class UploadCanceledEventArgs
    {
        public FileInfo FileInfo
        {
            get;
            internal set;
        }

        public string FileIdentity
        {
            get;
            internal set;
        }

        public long TotalUploadedDataLength
        {
            get;
            internal set;
        }
    }

    public delegate void UploadCanceledEvent(object sender, UploadCanceledEventArgs args);
}
