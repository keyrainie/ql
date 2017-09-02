using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductResourceProcessor))]
    public class ProductResourceProcessor
    {
        private readonly IProductResourceDA _productResourceDA = ObjectFactory<IProductResourceDA>.Instance;

        public virtual bool IsExistFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "FileNameIsNull"));
            }
            var result = _productResourceDA.GetProductGroupInfoImageSysNoByFileName(fileName);
            return result > 0;
        }
        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteProductResource(ProductResourceForNewegg entity)
        {
            CheckProductResourceProcessor.CheckProductResource(entity);
            CheckProductResourceProcessor.CheckProductResourceSysNo(entity.Resource.ResourceSysNo);
            var productCommonInfoSysNo = entity.ProductCommonInfoSysNo;
            CheckProductResourceProcessor.CheckProductCommonInfoSysNo(productCommonInfoSysNo);
            if (entity.Resource.ResourceSysNo != null)
                _productResourceDA.DeleteProductResource(entity.Resource.ResourceSysNo.Value, productCommonInfoSysNo);
        }

        /// <summary>
        /// 修改商品图片
        /// </summary>
        /// <param name="entity"></param>
        public virtual void ModifyProductResources(ProductResourceForNewegg entity)
        {
            CheckProductResourceProcessor.CheckProductResource(entity);
            CheckProductResourceProcessor.CheckProductResourceSysNo(entity.Resource.ResourceSysNo);
            var productCommonInfoSysNo = entity.ProductCommonInfoSysNo;
            CheckProductResourceProcessor.CheckProductCommonInfoSysNo(productCommonInfoSysNo);
            entity.Resource.ResourceURL = entity.Resource.ResourceURL.ToLower().Contains("http") ? 
                                          Path.GetFileName(entity.Resource.ResourceURL) : 
                                          entity.Resource.ResourceURL;
            _productResourceDA.UpdateProductResource(entity, productCommonInfoSysNo);
        }

        /// <summary>
        /// 创建商品图片
        /// </summary>
        /// <param name="entity"></param>
        public virtual List<ProductResourceForNewegg> CreateProductResource(List<ProductResourceForNewegg> entity)
        {
            if (entity == null || entity.Count == 0) return entity;
            foreach (var item in entity)
            {
                /*
                if (ImageUtility.IsImages(item.Resource.TemporaryName))
                {
                    CreateImages(item.Resource.TemporaryName, item.Resource.ResourceURL, item.IsNeedWatermark);
                }
                else if (ImageUtility.IsFLV(item.Resource.TemporaryName))
                {
                    FlvOp.Save(item.Resource.TemporaryName, item.Resource.ResourceURL);
                }
                else
                {
                    ImageOPFor360.Save(item.Resource.TemporaryName, item.Resource.ResourceURL);
                }
                */
                CheckProductResourceProcessor.CheckProductResource(item);
                var productCommonInfoSysNo = item.ProductCommonInfoSysNo ; 
                CheckProductResourceProcessor.CheckProductCommonInfoSysNo(productCommonInfoSysNo);
                var result = _productResourceDA.GetProductGroupInfoImageSysNoByFileName(item.Resource.ResourceURL);
                if (result <= 0)
                {
                    _productResourceDA.InsertProductResource(item, productCommonInfoSysNo);
                }
                else
                {
                    item.Resource.ResourceSysNo = result;
                }
               
            }

            return entity;
        }

        /// <summary>
        /// 生成套图
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="fileName"></param>
        private void CreateImages(string fileIdentity, string fileName, bool isNeedWatermark)
        {
            if (ImageUtility.IsImages(fileIdentity))
            {
                ImageOp.OriginalImageSave(fileIdentity, fileName);
                ImageOp.CreateImagesByNewegg(fileIdentity, fileName, isNeedWatermark);
            }
        }


        /// <summary>
        /// 业务逻辑判断
        /// </summary>
        private static class CheckProductResourceProcessor
        {
            /// <summary>
            /// 检查图片资源实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductResource(ProductResourceForNewegg entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "ProductResourceInvalid"));
                }
                if (entity.Resource == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "ProductResourceIsNull"));
                }
            }

            /// <summary>
            /// 检查图片资源编号
            /// </summary>
            /// <param name="resourceSysNo"></param>
            public static void CheckProductResourceSysNo(int? resourceSysNo)
            {

                if (resourceSysNo == null || resourceSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "ResourceSysNoIsNull"));
                }
            }

            /// <summary>
            /// 检查商品productCommonInfoSysNo
            /// </summary>
            /// <param name="productCommonInfoSysNo"></param>
            public static void CheckProductCommonInfoSysNo(int productCommonInfoSysNo)
            {
                if (productCommonInfoSysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "ProductCommonInfoSysNoIsNull"));
                }
            }

            public static int CheckFileName(string fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "FileNameIsNull"));
                }
                if(fileName.ToLower().Contains("http"))
                fileName = Path.GetFileName(fileName);

                var re = new Regex(@"^([^\.\-]+)(\-(([1-9][0-9])|(0[1-9])))?\..+?$", RegexOptions.IgnoreCase);

                if (fileName != null)
                {
                    var match = re.Match(fileName);

                    //Check上传文件名是否合法
                    if (match.Success)
                    {
                        string commonSKUNumber = match.Groups[1].Value;
                        var productCommonInfoDA = ObjectFactory<IProductCommonInfoDA>.Instance;
                        var result = productCommonInfoDA.GetProductCommonInfoByCommonInfoSkuNumber(commonSKUNumber);
                        if (result < 0)
                        {
                            throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "IsExistsFileName"));
                        }
                        return result;
                    }
                }
                throw new BizException(ResouceManager.GetMessageString("IM.ProductResource", "FileNameInvalid"));
            }


        }

    }


    /// <summary>
    /// 图片操作
    /// </summary>
    internal class ImageOp
    {
        private enum Graph
        {
            //泰隆优选项目新增图片规格
            //32*32
            //P32,
            //50*50
            //P50,
            //60*60
           
            //90*90
            //P90,
            //100*100
            //P100,
            //120*120
            
            //140*140
            //P140,
            //150*150
            //P150,
            //160*160
            
            //200*200
            
            //218*218
            //P218,
            //450*450
            
            //830*830
            //P830,

            //泰隆优选项目ECC使用的图片尺寸：
            //125*125
            //P125,
            ////144*144
            //P144,
            ////泰隆优选项目SellerPortal使用的图片尺寸：
            ////80*80
            //P80,

            ////泰隆优选项目手机端使用的图片尺寸：
            ////40*40
            //P40,
            ////45*45
            //P45,
            ////68*68
            //P68,
            ////112*112
            //P112,
            ////155*155
            //P155,
            ////176*176
            //P176,
            ////192*192
            //P192,
            ////216*216
            //P216,
            ////220*220
            //P220,
            ////240*240
            //P240,
            ////246*246
            //P246,
            ////270*270
            //P270,
            ////296*296
            //P296,
            ////330*330
            //P330,
            ////350*350
            //P350,
            ////400*400
            //P400,
            ////480*480
            //P480,
            ////540*540
            //P540,
            //800*800
            P60,
            P120,
            P160,
            P200,
            P240,
            P450,
            P800
        }

        private readonly static Dictionary<Graph, ImageSize> ImageList = new Dictionary<Graph, ImageSize>();
        private static readonly string[] ImageFileExtensionName = new[] { ".JPG" };//文件后缀名
        /// <summary>
        /// 保存文件标题
        /// </summary>
        //private readonly static string SaveTitle = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePathTitle");

        private readonly static string UploadURL = AppSettingManager.GetSetting("IM", "ProductImage_DFISUploadURL");

        private readonly static string FileGroup = AppSettingManager.GetSetting("IM", "ProductImage_DFISFileGroup");

        private static readonly string UserName;

        /// <summary>
        /// 保存文件地址
        /// </summary>
        private readonly static string[] LocalPathList = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePath").Split(Char.Parse("|"));

        private class ImageSize
        {
            public Double Height { get; set; }

            public Double Width { get; set; }
        }

        static ImageOp()
        {
            //泰隆优选项目新增图片规格
            //32*32
            //50*50
            //60*60
            //90*90
            //100*100
            //120*120
            //140*140
            //150*150
            //200*200
            //218*218
            //450*450
            //830*830
            //ImageList.Add(Graph.P32, new ImageSize { Height = 32, Width = 32 });
            //ImageList.Add(Graph.P50, new ImageSize { Height = 50, Width = 50 });
            //ImageList.Add(Graph.P60, new ImageSize { Height = 60, Width = 60 });
            //ImageList.Add(Graph.P90, new ImageSize { Height = 90, Width = 90 });
            //ImageList.Add(Graph.P100, new ImageSize { Height = 100, Width = 100 });
            //ImageList.Add(Graph.P120, new ImageSize { Height = 120, Width = 120 });
            //ImageList.Add(Graph.P140, new ImageSize { Height = 140, Width = 140 });
            //ImageList.Add(Graph.P150, new ImageSize { Height = 150, Width = 150 });
            //ImageList.Add(Graph.P200, new ImageSize { Height = 200, Width = 200 });
            //ImageList.Add(Graph.P218, new ImageSize { Height = 218, Width = 218 });
            //ImageList.Add(Graph.P450, new ImageSize { Height = 450, Width = 450 });
            //ImageList.Add(Graph.P830, new ImageSize { Height = 830, Width = 830 });
            //泰隆优选项目ECC使用的图片尺寸：
            //125*125
            //144*144
            //ImageList.Add(Graph.P125, new ImageSize { Height = 125, Width = 125 });
            //ImageList.Add(Graph.P144, new ImageSize { Height = 144, Width = 144 });
            ////泰隆优选项目SellerPortal使用的图片尺寸：
            ////80*80
            //ImageList.Add(Graph.P80, new ImageSize { Height = 80, Width = 80 });
            ////泰隆优选项目手机端使用的图片尺寸：
            ////40*40
            ////45*45
            ////68*68
            ////112*112
            ////155*155
            ////176*176
            ////192*192
            ////216*216
            ////220*220
            ////240*240
            ////246*246
            ////270*270
            ////296*296
            ////330*330
            ////350*350
            ////400*400
            ////480*480
            ////540*540
            ////800*800
            //ImageList.Add(Graph.P40, new ImageSize { Height = 40, Width = 40 });
            //ImageList.Add(Graph.P45, new ImageSize { Height = 45, Width = 45 });
            //ImageList.Add(Graph.P68, new ImageSize { Height = 68, Width = 68 });
            //ImageList.Add(Graph.P112, new ImageSize { Height = 112, Width = 112 });
            //ImageList.Add(Graph.P155, new ImageSize { Height = 155, Width = 155 });
            //ImageList.Add(Graph.P176, new ImageSize { Height = 176, Width = 176 });
            //ImageList.Add(Graph.P192, new ImageSize { Height = 192, Width = 192 });
            //ImageList.Add(Graph.P216, new ImageSize { Height = 216, Width = 216 });
            //ImageList.Add(Graph.P220, new ImageSize { Height = 220, Width = 220 });
            //ImageList.Add(Graph.P240, new ImageSize { Height = 240, Width = 240 });
            //ImageList.Add(Graph.P246, new ImageSize { Height = 246, Width = 246 });
            //ImageList.Add(Graph.P270, new ImageSize { Height = 270, Width = 270 });
            //ImageList.Add(Graph.P296, new ImageSize { Height = 296, Width = 296 });
            //ImageList.Add(Graph.P330, new ImageSize { Height = 330, Width = 330 });
            //ImageList.Add(Graph.P350, new ImageSize { Height = 350, Width = 350 });
            //ImageList.Add(Graph.P400, new ImageSize { Height = 400, Width = 400 });
            //ImageList.Add(Graph.P480, new ImageSize { Height = 480, Width = 480 });
            //ImageList.Add(Graph.P540, new ImageSize { Height = 540, Width = 540 });
            //ImageList.Add(Graph.P800, new ImageSize { Height = 800, Width = 800 });


            ImageList.Add(Graph.P60, new ImageSize { Height = 60, Width = 60 });
            ImageList.Add(Graph.P120, new ImageSize { Height = 120, Width = 120 });
            ImageList.Add(Graph.P160, new ImageSize { Height = 160, Width = 160 });
            ImageList.Add(Graph.P200, new ImageSize { Height = 200, Width = 200 });
            ImageList.Add(Graph.P240, new ImageSize { Height = 240, Width = 240 });
            ImageList.Add(Graph.P450, new ImageSize { Height = 450, Width = 450 });
            ImageList.Add(Graph.P800, new ImageSize { Height = 800, Width = 800 });

            UserName = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
        }       

        public static void CreateImagesByNewegg(string fileIdentity, string newFileName, bool isNeedWatermark)
        {
            if (ImageList.Count == 0) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return;
            var bytes = FileUploadManager.GetFileData(fileIdentity);
            ImageList.ForEach(v => ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        var imageHelp = new ImageUtility { FileName = newFileName, MidFilepath = v.Key.ToString() };
                        imageHelp.Upload += Upload;
                        imageHelp.ZoomAuto(bytes, (int)v.Value.Width, (int)v.Value.Height, isNeedWatermark);
                    }));
        }

        /// <summary>
        /// 生成一组套图
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="newFileName"></param>
        public static void CreateImages(string fileIdentity, string newFileName)
        {
            if (ImageList.Count == 0) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return;
            var bytes = FileUploadManager.GetFileData(fileIdentity);
            ImageList.ForEach(v => ThreadPool.QueueUserWorkItem(
                delegate
                        {
                            var imgageHelp = new ImageUtility();
                            imgageHelp.ZoomAuto(bytes, (int)v.Value.Width, (int)v.Value.Height, true);
                        }));
        }

        /// <summary>
        /// 生成标准图片
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="newFileName"></param>
        public static void CreateStandardImages(string fileIdentity, string newFileName)
        {
            if (ImageList.Count == 0) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (!result) return;
            var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
            Image uploadImage = Image.FromFile(filePath);         
                //判断是否为标准图片
            if (!ImageUtility.CheckImagePixels(uploadImage))
            {
                uploadImage.Dispose();//后面的zoomauto会删除的，资源要提前释放
                //重命名文件
                var bytes = FileUploadManager.GetFileData(fileIdentity);
                var imageHelp = new ImageUtility {SavePath = filePath};
                imageHelp.ZoomAuto(bytes, 640, 480);
            }
        }

        /// <summary>
        /// 删除一组套图
        /// </summary>
        /// <param name="fileIdentity"></param>
        public static void DeleteImages(string fileIdentity)
        {
            if (ImageList.Count == 0) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result) return;
            ImageList.ForEach(v =>
            {
                var folderPath = Path.Combine(FileUploadManager.BaseFolder, v.Key.ToString());
                if (!Directory.Exists(folderPath))
                {
                    return;
                }
                var filePath = Path.Combine(folderPath, fileIdentity);
                if (File.Exists(filePath))
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        File.Delete(filePath);
                    });
                }
            });

        }

        /// <summary>
        /// 原始图片保存
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="destFileName"></param>
        public static void OriginalImageSave(string fileIdentity, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(fileIdentity) || string.IsNullOrWhiteSpace(destFileName)) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result)
            {
                var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
                CreateStandardImages(fileIdentity, destFileName);

                var fileExtensionName = FileUploadManager.GetFileExtensionName(fileIdentity);
                var savePath = FilePathHelp.GetFileSavePath(fileExtensionName.ToLower());
                OriginalImageSave(fileIdentity, destFileName, savePath);
            }
        }

        public static void OriginalImageSaveByDFIS(string fileIdentity, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(fileIdentity) || string.IsNullOrWhiteSpace(destFileName)) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result)
            {
                var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
                CreateStandardImages(fileIdentity, destFileName);

                HttpUploader.UploadFile(UploadURL, FileGroup, "Original", filePath, destFileName, "", UserName, UploadMethod.Update);
            }
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="destFileName"></param>
        /// <param name="savePath"></param>
        private static void OriginalImageSave(string fileIdentity, string destFileName, string savePath)
        {
            if (string.IsNullOrWhiteSpace(fileIdentity) || string.IsNullOrWhiteSpace(destFileName)
                || string.IsNullOrWhiteSpace(savePath)) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result)
            {
                var fileExtensionName = FileUploadManager.GetFileExtensionName(fileIdentity).ToUpper();
                var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
                if (ImageFileExtensionName.Contains(fileExtensionName))
                {
                    if (destFileName.IndexOf(".") == -1)
                    {
                        destFileName = destFileName + "." + fileExtensionName;
                    }
                    savePath += FilePathHelp.GetSubFolderName(destFileName);
                    foreach (string localPath in LocalPathList)
                    {
                        string path = localPath + savePath.Replace("/", "\\");

                        //判断文件夹是否存在
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        destFileName = path + destFileName;
                        //FLV文件重命名操作,不进行DFIS处理
                        File.Copy(filePath, destFileName, true);
                    }

                }
            }
        }

        private static void Upload(object sender, UploadEventArgs args)
        {
            args.SaveFileName = GetSaveFilePath(args.FileName, FileGroup, args.MidFilepath);
            //var filePath = GetSaveFilePath(args.FileName, FileGroup, args.MidFilepath);
            //if (!string.IsNullOrWhiteSpace(filePath))
            //    args.UploadImage.Save(filePath);
        }

        private static void UploadByDFIS(object sender, UploadEventArgs args)
        {

            using (var ms = new MemoryStream())
            {
                args.UploadImage.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);

                HttpUploader.UploadFile(ms, args.FileName, UploadURL, FileGroup, args.MidFilepath, "", UserName, UploadMethod.Update);
            }            
        }

        /// <summary>
        /// 得到图片保存路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileGroup"></param>
        /// <param name="midFilepath"></param>
        /// <returns></returns>
        private static string GetSaveFilePath(string fileName, string fileGroup, string midFilepath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileGroup)
               || string.IsNullOrWhiteSpace(midFilepath)) return "";
            if (LocalPathList == null || LocalPathList.Count() == 0) return "";
            var diretory = FilePathHelp.GetSubFolderName(fileName).Replace("/", "\\");
            var saveDirectory = String.Format(@"{0}{1}\{2}\{3}", LocalPathList[0], fileGroup, midFilepath, diretory);

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            var filePath = saveDirectory + fileName;
            return filePath;
        }
    }

    /// <summary>
    /// 视频处理
    /// </summary>
    internal class FlvOp
    {
        private static readonly string[] FLVFileExtensionName = new[] { ".FLV" };//文件后缀名
        /// <summary>
        /// 保存文件地址
        /// </summary>
        private readonly static string[] LocalPathList = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePath").Split(Char.Parse("|"));

        /// <summary>
        /// 保存文件标题
        /// </summary>
       // private readonly static string SaveTitle = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePathTitle");

        /// <summary>
        /// 保存视频文件
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="destFileName"></param>
        public static void Save(string fileIdentity, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(fileIdentity) || string.IsNullOrWhiteSpace(destFileName)) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result)
            {
                var fileExtensionName = FileUploadManager.GetFileExtensionName(fileIdentity).ToUpper();
                var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
                if (FLVFileExtensionName.Contains(fileExtensionName))
                {
                    if (destFileName.IndexOf(".") == -1)
                    {
                        destFileName = destFileName + "." + fileExtensionName;
                    }
                    var savePath = FilePathHelp.GetFileSavePath(fileExtensionName.ToLower());
                    savePath += FilePathHelp.GetSubFolderName(destFileName);
                    foreach (string localPath in LocalPathList)
                    {
                        string path = localPath + savePath.Replace("/", "\\");

                        //判断文件夹是否存在
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        destFileName = path + destFileName;
                        //FLV文件重命名操作,不进行DFIS处理
                        File.Copy(filePath, destFileName, true);
                    }

                }
            }
        }
    }

    /// <summary>
    /// 360图片
    /// </summary>
    internal class ImageOPFor360
    {
        /// <summary>
        /// 保存文件地址
        /// </summary>
        private readonly static string SaveTitle = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePathTitle");

        private readonly static string UploadURL = AppSettingManager.GetSetting("IM", "ProductImage_DFISUploadURL");

        private static readonly string[] Image360FileExtensionName = new[] { ".SWF" };//文件后缀名

        /// <summary>
        /// 保存文件地址
        /// </summary>
        private readonly static string[] LocalPathList = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePath").Split(Char.Parse("|"));

        /// <summary>
        /// 支持多服务器保存
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="newFileName"></param>
        /// <param name="fileSize"></param>
        /// <param name="userName"></param>
        public static void Save(string filePath, string newFileName, string fileSize, string userName)
        {

            //重命名文件
            HttpUploader.UploadFile(UploadURL, SaveTitle, fileSize, filePath, newFileName, "", userName, UploadMethod.Update);
        }

        /// <summary>
        /// 保存视频文件
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="destFileName"></param>
        public static void Save(string fileIdentity, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(fileIdentity) || string.IsNullOrWhiteSpace(destFileName)) return;
            var result = FileUploadManager.FileExists(fileIdentity);
            if (result)
            {
                var fileExtensionName = FileUploadManager.GetFileExtensionName(fileIdentity).ToUpper();
                var filePath = FileUploadManager.GetFilePhysicalFullPath(fileIdentity);
                if (Image360FileExtensionName.Contains(fileExtensionName))
                {
                    if (destFileName.IndexOf(".") == -1)
                    {
                        destFileName = destFileName + "." + fileExtensionName;
                    }
                    var savePath = FilePathHelp.GetFileSavePath(fileExtensionName.ToLower());
                    savePath += FilePathHelp.GetSubFolderName(destFileName);
                    foreach (string localPath in LocalPathList)
                    {
                        string path = localPath + savePath.Replace("/", "\\");

                        //判断文件夹是否存在
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        destFileName = path + destFileName;
                        //FLV文件重命名操作,不进行DFIS处理
                        File.Copy(filePath, destFileName, true);
                    }

                }
            }
        }
    }


}
