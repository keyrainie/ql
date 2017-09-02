<%@ WebHandler Language="C#" Class="imageUp" %>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.IO;

public class imageUp : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //\\10.1.220.195\cms
        //上传配置                                          //保存路径
        string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };          //文件允许格式
        int size = 1024;                                                          //文件大小限制，单位KB

        //文件上传状态,初始默认成功，可选参数{"SUCCESS","ERROR","SIZE","TYPE"}
        String state = "SUCCESS";

        String title = String.Empty;
        String oriName = String.Empty;
        String filename = String.Empty;
        String url = String.Empty;
        String currentType = String.Empty;
        String uploadpath = String.Empty;
        String tempPath = String.Empty;
        String watermarkImagePath = String.Empty;

        tempPath = "~/UploadFiles/Temp_CMS"; 
        uploadpath = "~/UploadFiles/CMS";

        watermarkImagePath = "";

        string fixedImagePath = String.Empty;

        try
        {
            HttpPostedFile uploadFile = context.Request.Files[0];
            title = uploadFile.FileName;


            //格式验证
            string[] temp = uploadFile.FileName.Split('.');
            currentType = "." + temp[temp.Length - 1].ToLower();
            if (Array.IndexOf(filetype, currentType) == -1)
            {
                state = "TYPE";
            }

            //大小验证
            if (uploadFile.ContentLength / 1024 > size)
            {
                state = "SIZE";
            }

            var now = DateTime.Now;
            //保存图片
            if (state == "SUCCESS")
            {
                #region 设置压缩质量
                ImageCodecInfo myImageCodecInfo;
                Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;

                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 100L);//0-100
                myEncoderParameters.Param[0] = myEncoderParameter;
                #endregion

                tempPath = Path.Combine(tempPath, now.ToString("yyyy-MM"), now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(context.Server.MapPath(tempPath)))
                {
                    Directory.CreateDirectory(context.Server.MapPath(tempPath));
                }
                tempPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + currentType);
                tempPath = context.Server.MapPath(tempPath);
                uploadFile.SaveAs(tempPath);
                #region 加水印
                Image initImage = Image.FromFile(tempPath);
                watermarkImagePath = Path.Combine(context.Server.MapPath("~/"), watermarkImagePath);
                if (File.Exists(watermarkImagePath))
                {
                    using (Image wrImage = Image.FromFile(watermarkImagePath))
                    {
                        //水印绘制条件：原始图片宽高均大于或等于水印图片   
                        if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                        {
                            Graphics gWater = Graphics.FromImage(initImage);

                            //透明属性   
                            ImageAttributes imgAttributes = new ImageAttributes();
                            ColorMap colorMap = new ColorMap();
                            colorMap.OldColor = Color.FromArgb(0, 0, 0, 0);
                            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                            ColorMap[] remapTable = { colorMap };
                            imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                            float[][] colorMatrixElements = {  
                                   new float[] {1f,  0f,  0f,  0f, 0f},  
                                   new float[] {0f,  1f,  0f,  0f, 0f},  
                                   new float[] {0f,  0f,  1f,  0f, 0f},  
                                   new float[] {0f,  0f,  0f,  5f, 0f},  
                                   new float[] {0f,  0f,  0f,  0f, 1f}  
                                };

                            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                            imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                            gWater.Dispose();
                        }
                        wrImage.Dispose();
                    }
                }
                fixedImagePath =context.Server.MapPath(uploadpath);
                uploadpath = Path.Combine(fixedImagePath, now.ToString("yyyy-MM"), now.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                filename = Guid.NewGuid().ToString() + currentType;
                url = Path.Combine(uploadpath, filename);
                initImage.Save(url, myImageCodecInfo, myEncoderParameters);

                #endregion

                initImage.Dispose();
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);//删除没上水印的老图
                }
            }
        }
        catch (Exception e)
        {
            state = "ERROR";
        }

        //获取图片描述
        if (context.Request.Form["pictitle"] != null)
        {
            if (!String.IsNullOrEmpty(context.Request.Form["pictitle"]))
            {
                title = context.Request.Form["pictitle"];
            }
        }
        //获取原始文件名
        if (context.Request.Form["fileName"] != null)
        {
            if (!String.IsNullOrEmpty(context.Request.Form["fileName"]))
            {
                oriName = context.Request.Form["fileName"].Split(',')[1];
            }
        }

        //向浏览器返回数据json数据
        HttpContext.Current.Response.Write("{'url':'" + url.Replace(fixedImagePath, "CMS").Replace("\\", "/") + "','title':'" + title + "','original':'" + oriName + "','state':'" + state + "'}");
    }
    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}