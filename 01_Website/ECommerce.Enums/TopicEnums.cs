using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
 

    public enum EFileType
    {
        [Description("图片")]
        Image=1,
        [Description("视频")]
        Vedio=100,
        [Description("下载文件")]
        DownloadFile=200
    }

    public enum EImageSize
    {
        [Description("极小")]
        Icon = 100,
        [Description("小")]
        Small = 200,
        [Description("中")]
        Middle = 300,
        [Description("大")]
        Big = 600,
        [Description("原图")]
        Source = 1000 
    }
     

}
