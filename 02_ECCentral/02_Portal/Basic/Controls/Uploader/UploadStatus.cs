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

namespace ECCentral.Portal.Basic.Controls.Uploader
{
    public enum UploadStatus
    {
        Pending,
        Uploading,
        Complete,
        Error,
        Canceling,//正在取消上传，因为是异步的，所以加上这个状态
        Canceled,//已经取消上传
        Removed,
    }
}
