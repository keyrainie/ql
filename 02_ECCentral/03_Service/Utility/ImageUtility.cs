using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace ECCentral.Service.Utility
{
    public class ImageUtility
    {

        private const int JPG = 255216;//图片
        private const int FLV = 707686;//视频
        private const int ImageFor360 = 678783;//360图片
        private const int ImageFor360Other = 708783;//360图片

        /// <summary>
        /// 中间文件名
        /// </summary>
        public string MidFilepath { get; set; }

        /// <summary>
        /// 图片名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }

        private event UploadEvent MUpload;
        public  event UploadEvent Upload
        {
            add
            {
                MUpload += value;
            }
            remove
            {
                MUpload -= value;
            }
        }

        private void OnUpload(UploadEventArgs args)
        {
            UploadEvent handler = MUpload;
            if (handler != null)
            {
                handler(this,args);
            }
        }

        private static Bitmap ImgWatermark
        {
            get
            {
                string folder = AppSettingManager.GetSetting("IM", "ProductImage_WatermarkPath");
                var waterMarkPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, folder);
                return new Bitmap(waterMarkPath);
            }
        }

        #region 等比缩放

        /// <summary>   
        /// 图片等比缩放   
        /// </summary>   
        /// <param name="postedFile">原图HttpPostedFile对象</param>
        /// <param name="targetWidth">指定的最大宽度</param>   
        /// <param name="targetHeight">指定的最大高度</param>   
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>   
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>   
        public  void ZoomAuto(byte[] postedFile, int targetWidth, int targetHeight, string watermarkText, string watermarkImage)
        {
            if (postedFile == null) return;

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息） 
            var stream = new MemoryStream(postedFile);
            Image initImage = Image.FromStream(stream, true);
           
            //原图宽高均小于模版，不作处理，直接保存   
            if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
            {
                ////生成新图   
                ////新建一个bmp图片   
                //initImage = new Bitmap(initImage.Width, initImage.Height);
                ////新建一个画板   
                //Graphics newG = Graphics.FromImage(initImage);

                ////设置质量   
                //newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                ////置背景色   
                //newG.Clear(Color.White);
                ////画图   
                //newG.DrawImage(initImage, new Rectangle(0, 0, initImage.Width, initImage.Height), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
                //生成新图   
                //新建一个bmp图片  

                Image newImage = new Bitmap(initImage.Width, initImage.Height);
                //新建一个画板   
                Graphics newG = Graphics.FromImage(newImage);

                //设置质量   
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //置背景色   
                newG.Clear(Color.White);
                //画图   
                newG.DrawImage(initImage, new Rectangle(0, 0, initImage.Width, initImage.Height),
                    new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);



                //文字水印   
                if (watermarkText != "")
                {
                    using (Graphics gWater = Graphics.FromImage(initImage))
                    {
                        var fontWater = new Font("黑体", 10);
                        Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印   
                if (watermarkImage != "")
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片   
                        using (Image wrImage = GetWatermark(initImage.Width, initImage.Height))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片   
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(initImage);

                                //透明属性   
                                var imgAttributes = new ImageAttributes();
                                //var colorMap = new ColorMap
                                //                   {
                                //                       OldColor = Color.FromArgb(0, 0, 0, 0),
                                //                       NewColor = Color.FromArgb(0, 0, 0, 0)
                                //                   };
                                //ColorMap[] remapTable = { colorMap };
                                //imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
												new[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},       
												new[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},        
												new[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},        
												new[] {0.0f,  0.0f,  0.0f,  0.1f, 0.0f},        
												new[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
                                var wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }
                var args = new UploadEventArgs { UploadImage = initImage, FileName = FileName, MidFilepath = MidFilepath };
                OnUpload(args);
                //保存  
                SavePath = args.SaveFileName;
                if (!string.IsNullOrWhiteSpace(SavePath))
                {
                    if (File.Exists(SavePath))
                    {
                        File.Delete(SavePath);
                    }
                    initImage.Save(SavePath, ImageFormat.Jpeg);
                }
                   
            }
            else
            {
                //缩略图宽、高计算   
                int newWidth = initImage.Width;
                int newHeight = initImage.Height;

                //宽大于高或宽等于高（横图或正方）   
                if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
                {
                    //如果宽大于模版   
                    if (initImage.Width > targetWidth)
                    {
                        //宽按模版，高按比例缩放   
                        newWidth = targetWidth;
                       var  height = initImage.Height * (targetWidth / Convert.ToDouble(initImage.Width));
                        newHeight = (int) height;
                    }
                }
                //高大于宽（竖图）   
                else
                {
                    //如果高大于模版   
                    if (initImage.Height > targetHeight)
                    {
                        //高按模版，宽按比例缩放   
                        newHeight = targetHeight;
                        var width = initImage.Width * (targetHeight / initImage.Height);
                        newWidth = width;
                    }
                }

                //生成新图   
                //新建一个bmp图片   
                Image newImage = new Bitmap(newWidth, newHeight);
                //新建一个画板   
                Graphics newG = Graphics.FromImage(newImage);

                //设置质量   
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //置背景色   
                newG.Clear(Color.White);
                //画图   
                newG.DrawImage(initImage, new Rectangle(0, 0, newImage.Width, newImage.Height), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);

                //文字水印   
                if (watermarkText != "")
                {
                    using (var gWater = Graphics.FromImage(newImage))
                    {
                        var fontWater = new Font("宋体", 10);
                        var brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印   
                if (watermarkImage != "")
                {
                    if (File.Exists(watermarkImage))
                    {
                        //获取水印图片   
                        using (Image wrImage = GetWatermark(newWidth, newHeight))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片   
                            if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(newImage);

                                //透明属性   
                                var imgAttributes = new ImageAttributes();
                                //ColorMap colorMap = new ColorMap();
                                //colorMap.OldColor = Color.FromArgb(0, 0, 0, 0);
                                //colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                //ColorMap[] remapTable = { colorMap };
                                //imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);


                                float[][] colorMatrixElements = { 
												new[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},       
												new[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},        
												new[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},        
												new[] {0.0f,  0.0f,  0.0f,  0.1f, 0.0f},        
												new[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};

                                var wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }
                stream.Close();
                //保存缩略图
                var args = new UploadEventArgs {UploadImage = newImage, FileName = FileName, MidFilepath = MidFilepath};
                OnUpload(args);
                SavePath = args.SaveFileName;
                if (!string.IsNullOrWhiteSpace(SavePath))
                {
                    if (File.Exists(SavePath))
                    {
                        File.Delete(SavePath);
                    }
                    newImage.Save(SavePath, ImageFormat.Jpeg);
                }
                   
                //释放资源   
                newG.Dispose();
                newImage.Dispose();
                initImage.Dispose();
               
               
            }
        }

        /// <summary>
        /// 无水印等比率缩放
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="isWaterMark"></param>
        public void ZoomAuto(byte[] postedFile, int targetWidth, int targetHeight, bool isWaterMark=false)
        {
            if (!isWaterMark)
            {
                ZoomAuto(postedFile, targetWidth, targetHeight, "", "");
            }
            else
            {
                string folder = AppSettingManager.GetSetting("IM", "ProductImage_WatermarkPath");
                var waterMarkPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, folder);
                ZoomAuto(postedFile, targetWidth, targetHeight, "", waterMarkPath);
            }

        }


        #endregion

        #region 图片水印
        /// <summary>
        /// 根据所给的宽、高得到相应比例的水印
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        private static Bitmap GetWatermark(int imageWidth, int imageHeight)
        {
            var precent = GetWatermarkPercent(imageWidth, imageHeight);
            var resizedWidth = (int)(ImgWatermark.Width * precent);
            var resizedHeight = (int)(ImgWatermark.Height * precent);

            var result = ImgWatermark.GetThumbnailImage(resizedWidth, resizedHeight, () => false, IntPtr.Zero);

            return (Bitmap)result;
        }

        private const int DefaultWidth = 1024;
        private const int DefaultHeight = 768;
        private static float GetWatermarkPercent(int imageWidth, int imageHeight)
        {
            return Math.Min(imageWidth / (float)DefaultWidth, imageHeight / (float)DefaultHeight);
        }
        #endregion

        /// <summary>
        ///是否为图片
        /// </summary>
        /// <param name="fileIdentity"></param>
        public static bool IsImages(string fileIdentity)
        {
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return false;
            var curentByte = FileUploadManager.GetFileData(fileIdentity, 0, 3);
            if (curentByte.Length < 3) return false;

            //读取文件前三个字节确定文件后缀
            var fileType = curentByte[0].ToString() + curentByte[1].ToString() + curentByte[2].ToString();
            return fileType.Contains(JPG.ToString());
        }

        /// <summary>
        ///是否为视频
        /// </summary>
        /// <param name="fileIdentity"></param>
        public static bool IsFLV(string fileIdentity)
        {
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return false;
            var curentByte = FileUploadManager.GetFileData(fileIdentity, 0, 3);
            if (curentByte.Length < 3) return false;

            //读取文件前三个字节确定文件后缀
            var fileType = curentByte[0].ToString() + curentByte[1].ToString() + curentByte[2].ToString();
            return fileType.Contains(FLV.ToString());
        }

        /// <summary>
        ///是否为视频
        /// </summary>
        /// <param name="fileIdentity"></param>
        public static bool Is360Image(string fileIdentity)
        {
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return false;
            var curentByte = FileUploadManager.GetFileData(fileIdentity, 0, 3);
            if (curentByte.Length < 3) return false;

            //读取文件前三个字节确定文件后缀
            var fileType = curentByte[0].ToString() + curentByte[1].ToString() + curentByte[2].ToString();
            return fileType.Contains(ImageFor360.ToString()) || fileType.Contains(ImageFor360Other.ToString());
        }

        /// <summary>
        /// 检查上传的图片文件的像素是否符合
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <returns></returns>
        public static bool CheckImagePixels(Image uploadImage)
        {

            //标准图片：1280X1024、800X600、640X480
            //if (uploadImage.Width == 1280 && uploadImage.Height == 1024)
            //{
            //    return true;
            //}

            //if (uploadImage.Width == 800 && uploadImage.Height == 600)
            //{
            //    return true;
            //}

            //if (uploadImage.Width == 640 && uploadImage.Height == 480)
            //{
            //    return true;
            //}

            return true;

        }
       
    }

    public delegate void UploadEvent(object sender, UploadEventArgs args);

    public class UploadEventArgs:EventArgs
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        public Image UploadImage { get; set; }

        /// <summary>
        /// 中间文件名
        /// </summary>
        public string MidFilepath { get; set; }

        /// <summary>
        /// 图片名
        /// </summary>
        public string FileName { get; set; }

        public string SaveFileName { get; set; }

    }
}
