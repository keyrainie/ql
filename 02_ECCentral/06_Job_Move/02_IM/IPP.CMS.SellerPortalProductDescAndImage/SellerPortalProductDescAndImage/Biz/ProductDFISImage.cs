using IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.DA;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.Utilities;
using Newegg.Oversea.Framework.ExceptionBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newegg.Oversea.Framework.Contract;
using System.Configuration;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.Biz
{
    public class ProductDFISImage
    {
        private readonly List<string> LocalPathList = ConfigurationManager.AppSettings["UploadFilePath"].Split(Char.Parse("|")).ToList();
        private readonly string TemporaryPath = string.Format(ConfigurationManager.AppSettings["UploadFileTemporaryPath"] + "{0}\\", DateTime.Now.ToString("yyyyMMdd"));
        private readonly string FilePathTitle = ConfigurationManager.AppSettings["UploadFilePathTitle"];
        private readonly bool IsWaterMark = Convert.ToBoolean(ConfigurationManager.AppSettings["IsWaterMark"]);

        /// <summary>
        /// 处理sendor portal上传的图片，包括新品创建和图片更新
        /// </summary>
        /// 
        public DefaultDataContract UploadSendorPortalImageList(int ProductRequestSysno, int productGroupSysNo, int comskuSysNo, string comsku, EntityHeader header)
        {
            DefaultDataContract returnValue = new DefaultDataContract();
            string faults = string.Empty;
            List<ItemVendorProductRequestFileEntity> imageList = SellerPortalProductDescAndImageDA.GetProductRequestImageList(ProductRequestSysno, "O");

            if (imageList.Count <= 0)
            {
                return returnValue;
            }

            //取商品图片数
            int currentPicNumber = SellerPortalProductDescAndImageDA.GetCommonInfoPicNumber(comskuSysNo) + 1;

            string photoFolderPathOnServer = ConfigurationManager.AppSettings["PhotoFolderPathOnServer"].Trim();

            ProductGroupInfoMediaEntity productgroupinfomediaentity = new ProductGroupInfoMediaEntity();
            productgroupinfomediaentity.Header = header;
            //图片类型
            productgroupinfomediaentity.FileType = "I";

            foreach (ItemVendorProductRequestFileEntity image in imageList)
            {
                //图片路径
                productgroupinfomediaentity.FileWebPath = image.Path.ToLower().Contains("http://") ? image.Path : photoFolderPathOnServer + image.Path;

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(productgroupinfomediaentity.FileWebPath));
                    ((HttpWebResponse)request.GetResponse()).Close();

                    //文件名
                    if (currentPicNumber == 1)
                    {
                        productgroupinfomediaentity.FileName = comsku + ".jpg";
                    }
                    else
                    {
                        productgroupinfomediaentity.FileName = comsku + "-" + currentPicNumber.ToString().PadLeft(2, '0') + ".jpg";
                    }

                    UploadWebFile(productgroupinfomediaentity, comsku);
                }
                catch (BusinessException ex)
                {
                    faults += image.ImageName + "上传失败,失败原因：" + ex.Message;
                    SellerPortalProductDescAndImageBP.WriteLog("\r\n" + DateTime.Now + image.ImageName + "上传失败,失败原因：" + ex.Message + " 处理失败......");

                    //如果失败，Count + 1
                    SellerPortalProductDescAndImageDA.SetProductRequestImageCount(image);
                    continue;
                }

                //保证图片编号连续性，待改进为直接取最大编号。
                currentPicNumber++;

                //插入图片关联
                //imageBp.InsertCommonInfoImage(productGroupSysNo, comskuSysNo, header);
                //上传成功更新产品图片状态
                image.Status = "F";
                SellerPortalProductDescAndImageDA.SetProductRequestImageStatus(image);

                if (!faults.Equals(string.Empty))
                {
                    returnValue.Faults = new MessageFaultCollection();
                    returnValue.Faults.Add(new MessageFault());
                    returnValue.Faults[0].ErrorDescription = faults;
                }
            }

            return returnValue;
        }


        /// <summary>
        /// 上传Web文件
        /// </summary>
        /// <param name="productGroupInfoMedia"></param>
        /// <returns></returns>
        public ProductGroupInfoMediaEntity UploadWebFile(ProductGroupInfoMediaEntity productGroupInfoMedia, string strCommonSKUNumber)
        {
            try
            {
                //根据文件路径下载文件到Byte[]  
                WebClient myWebClient = new WebClient();
                //WebProxy proxy = new WebProxy("ssproxy", 8080);

                //proxy.BypassProxyOnLocal = true;
                //proxy.UseDefaultCredentials = true;

                //myWebClient.Proxy = proxy;

                productGroupInfoMedia.ChunckData = myWebClient.DownloadData(productGroupInfoMedia.FileWebPath);
                productGroupInfoMedia.IsFirstChunck = true;
                productGroupInfoMedia.IsLastChunck = true;
                productGroupInfoMedia.FileGuid = Guid.NewGuid();

                //上传到临时文件
                productGroupInfoMedia = UploadFile(productGroupInfoMedia, strCommonSKUNumber);

                //保存文件并生成套图
                productGroupInfoMedia = SaveFile(productGroupInfoMedia, strCommonSKUNumber);

                return productGroupInfoMedia;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="productGroupInfoImage"></param>
        /// <returns></returns>
        public ProductGroupInfoMediaEntity UploadFile(ProductGroupInfoMediaEntity productGroupInfoMedia, string commonSKUNumber)
        {

            //文件后缀
            string fileFix = "";
            FileStream fs = null;

            try
            {
                //判断上传文件是否为第一次上传
                if (productGroupInfoMedia.IsFirstChunck)
                {
                    //取得文件名中CommonInfoSkuNumber
                    //string commonSKUNumber = "";

                    //var re = new Regex(@"^([^\._]+)(_(([0-9]+)|(0[1-9]?)))?\..+?$", RegexOptions.IgnoreCase);
                    //var re = new Regex(@"(\w+[/])+\w{2,5}(-\w{2,5})+(_\w{2,5})*\.jpg", RegexOptions.IgnoreCase);
                    //var match = re.Match(productGroupInfoMedia.FileName);

                    //Check上传文件名是否合法
                    //if (match.Success)
                    //{
                    //    commonSKUNumber = match.Groups[1].Value;
                    //}
                    //else
                    //{
                    //    throw new BusinessException("上传文件名不合法，请检查上传文件名称。");
                    //}

                    //根据取得商品组信息
                    ProductGroupInfoEntity groupInfo = SellerPortalProductDescAndImageDA.GetProductGroupInfoByCommonSku(commonSKUNumber, productGroupInfoMedia.Header.CompanyCode);

                    if (groupInfo == null)
                    {
                        throw new BusinessException("上传文件名不属于任何商品组，请检查上传文件名称。");
                    }


                    //判断ProductGroupSysno如果大于0，则需要Check文件名是否属于该组
                    if (productGroupInfoMedia.ProductGroupSysno > 0)
                    {
                        //Check文件名是否属于该组
                        if (productGroupInfoMedia.ProductGroupSysno != groupInfo.SysNo)
                        {
                            throw new BusinessException("上传文件不属于该组。");
                        }

                    }

                    productGroupInfoMedia.ProductGroupSysno = groupInfo.SysNo;

                    //如果为第一段文件需要判断文件类型是否合法                
                    fileFix = GetFileFix(productGroupInfoMedia.ChunckData);

                    //合法创建文件，否则抛出异常
                    if (!fileFix.Equals(GetFileFixByType(productGroupInfoMedia.FileType)))
                    {
                        throw new BusinessException("上传文件类型不合法，请检查上传文件类型。");
                    }

                    //判断文件夹是否存在
                    if (!Directory.Exists(TemporaryPath))
                    {
                        Directory.CreateDirectory(TemporaryPath);
                    }

                    //创建临时文件
                    fs = File.Create(TemporaryPath + productGroupInfoMedia.FileGuid.ToString());

                }
                else
                {
                    //读取临时文件
                    fs = File.Open(TemporaryPath + productGroupInfoMedia.FileGuid.ToString(), FileMode.Append);

                }

                //循环将数据流写入临时文件进行保存
                foreach (byte chunck in productGroupInfoMedia.ChunckData)
                {
                    fs.WriteByte(chunck);
                }

                fs.Close();
                fs.Dispose();

                productGroupInfoMedia.ChunckData = null;

                return productGroupInfoMedia;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

        }

        /// <summary>
        /// 文件保存到相对路径，并重命名
        /// </summary>
        /// <param name="productGroupInfoMedia"></param>
        public ProductGroupInfoMediaEntity SaveFile(ProductGroupInfoMediaEntity productGroupInfoMedia, string commonSKUNumber)
        {
            try
            {
                string savePath = "";

                //读取图片根据GroupSysNo分段存储信息
                string fileFix = GetFileFixByType(productGroupInfoMedia.FileType);
                
                //读取文件相对存储路径
                savePath = GetFileSavePath(fileFix);

                //临时文件名
                string sfileName = productGroupInfoMedia.FileGuid.ToString();
                string dfileName = productGroupInfoMedia.FileName;

                //int fileNameNo = 0;

                //fileNameNo = ProductImageDA.GetGroupInfoMaxImage(productGroupInfoMedia.ProductGroupSysno,productGroupInfoMedia.FileType, productGroupInfoMedia.Header.CompanyCode);
                //dfileName = string.Format("NeweggShowPic{0}-{1}{2}", productGroupInfoMedia.ProductGroupSysno.ToString(), fileNameNo.ToString(), fileFix);

                savePath += GetSubFolderName(dfileName);

                //创建图片与商品组关联信息以及图片与商品关联信息
                ProductGroupInfoImageEntity productImage = new ProductGroupInfoImageEntity();
                //productImage.IsShow = "Y";
                productImage.ProductGroupSysno = productGroupInfoMedia.ProductGroupSysno;

                productImage.ResourceUrl = dfileName;

                productImage.Status = "A";
                productImage.Type = productGroupInfoMedia.FileType;
                productImage.Header = productGroupInfoMedia.Header;

                //取得文件名中CommonInfoSkuNumber
                //string commonSKUNumber = "";
                //var re = new Regex(@"^([^\._]+)(_(([0-9]+)|(0[1-9]?)))?\..+?$", RegexOptions.IgnoreCase);
                //var match = re.Match(productGroupInfoMedia.FileName);
                //if (match.Success) commonSKUNumber = match.Groups[1].Value;

                SellerPortalProductDescAndImageDA.CreateProductGroupInfoImage(productImage);

                //创建商品与图片关联信息
                SellerPortalProductDescAndImageDA.CreateProductCommonInfoImage(productImage, commonSKUNumber);

                //配置存储路径
                foreach (string localPath in LocalPathList)
                {

                    string filePath = localPath + savePath.Replace("/", "\\");


                    //判断文件夹是否存在
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    //判断上传的文件是否为图片，如果为图片需要进行套图处理
                    if (fileFix == ".jpg")
                    {
                        using (Image uploadImage = Image.FromFile(TemporaryPath + sfileName))
                        {
                            //判断是否为标准图片
                            if (CheckImagePixels(uploadImage))
                            {
                                //重命名文件
                                FileCopy(TemporaryPath + sfileName, dfileName, filePath);
                            }
                            else
                            {
                                //非标准图片需将临时文件压缩为标准图片
                                SaveFixedImage(uploadImage, 800, 800, filePath, "Original", dfileName, true);
                            }
                        }
                        
                        using (Image uploadImage = Image.FromFile(filePath + dfileName))
                        {
                            SaveFixedImageCase(uploadImage, filePath, dfileName);
                        }
                    }
                    else
                    {
                        //重命名文件
                        FileCopy(TemporaryPath + sfileName, dfileName, filePath);
                    }

                }

                //删除临时文件
                File.Delete(TemporaryPath + sfileName);

                return productGroupInfoMedia;

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }


        /// <summary>
        /// 根据文件头获取文件后缀
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        private string GetFileFix(byte[] uploadFile)
        {
            if (uploadFile.Length < 3) return "";

            //读取文件前三个字节确定文件后缀
            string fileType = uploadFile[0].ToString() + uploadFile[1].ToString() + uploadFile[2].ToString();

            return GetFileFix(fileType);
        }

        /// <summary>
        ///  根据文件头获取文件后缀
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private string GetFileFix(string fileType)
        {
            //文件扩展名说明
            //二个字节确定
            //*7173        gif 
            //*255 216      jpg
            //*13780       png
            //*6677        bmp
            //*239187      txt,aspx,asp,sql
            //*208207      xls.doc.ppt
            //*6063        xml
            //*6033        htm,html
            //*4742        js
            //*8075        xlsx,zip,pptx,mmap,zip
            //*8297        rar   
            //*01          accdb,mdb
            //*7790        exe,dll           
            //*5666        psd 
            //*255254      rdp 
            //*10056       bt种子 
            //*64101       bat  
            //三个字节确定
            //7087 83      swf
            //6787 83      swf
            //70 76 86     flv

            Dictionary<string, string> typeDictionary = new Dictionary<string, string>();

            typeDictionary.Add("255216", ".jpg");
            typeDictionary.Add("708783", ".swf");
            typeDictionary.Add("678783", ".swf");
            typeDictionary.Add("707686", ".flv");

            foreach (string key in typeDictionary.Keys)
            {
                if (fileType.Contains(key))
                {
                    return typeDictionary[key];
                }
            }

            return "";
        }

        /// <summary>
        ///  根据后缀获取文件类型
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private string GetFileFixByType(string fileType)
        {
            string fileFix = "";

            switch (fileType)
            {
                case "I":
                    fileFix = ".jpg";
                    break;
                case "D":
                    fileFix = ".swf";
                    break;
                case "V":
                    fileFix = ".flv";
                    break;
            }

            return fileFix;
        }

        /// <summary>
        /// 检查上传的图片文件的像素是否符合
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <returns></returns>
        private bool CheckImagePixels(Image uploadImage)
        {
            //标准图片：800X800
            if (uploadImage.Width == 800 && uploadImage.Height == 800)
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 生成图片套图
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <param name="fileName"></param>
        private void SaveFixedImageCase(Image uploadImage, string fileName, string userName)
        {
            //添加水印图片
            if (IsWaterMark)
            {
                uploadImage = ImageResizingManagerBP.InsertWatermark(uploadImage);
            }
            
            //保存图片缩略图
            SaveFixedImage(uploadImage, 800, 800, "P800", fileName, false, userName);
            SaveFixedImage(uploadImage, 450, 450, "P450", fileName, false, userName);
            SaveFixedImage(uploadImage, 240, 240, "P240", fileName, false, userName);
            SaveFixedImage(uploadImage, 200, 200, "P200", fileName, false, userName);
            SaveFixedImage(uploadImage, 160, 160, "P160", fileName, false, userName);
            SaveFixedImage(uploadImage, 120, 120, "P120", fileName, false, userName);
            SaveFixedImage(uploadImage, 100, 100, "P100", fileName, false, userName);
            SaveFixedImage(uploadImage, 60, 60, "P60", fileName, false, userName);
        }

        /// <summary>
        /// 保存图片缩略图
        /// </summary>
        /// <param name="uploadImage">图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="fileKey">分组文件夹</param>
        /// <param name="midFilepath">中间文件夹</param>
        /// <param name="fileName">文件名</param>
        /// <param name="isOriginal">是否生成原图</param>
        private void SaveFixedImage(Image uploadImage, int width, int height, string filePath, string midFilepath, string fileName, bool isOriginal)
        {
            Image resizedImage = null;

            string savePath = filePath.Replace("Original", midFilepath);

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            //判断源图片大小小于等于被压缩的图片格式并且非原图压缩
            //则无需压缩直接保存到目标地址
            if (!isOriginal && (uploadImage.Height <= height || uploadImage.Width <= width))
            {
                uploadImage.Save(savePath + fileName, ImageFormat.Jpeg);
            }
            else
            {
                resizedImage = ImageResizingManagerBP.FixedSize(uploadImage, width, height);
                resizedImage.Save(savePath + fileName, ImageFormat.Jpeg);
                resizedImage.Dispose();
            }
        }

        /// <summary>
        /// 保存图片缩略图
        /// </summary>
        /// <param name="uploadImage">图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="fileKey">分组文件夹</param>
        /// <param name="midFilepath">中间文件夹</param>
        /// <param name="filePath">文件名</param>
        /// <param name="isOriginal">是否生成原图</param>
        private void SaveFixedImage(Image uploadImage, int width, int height, string midFilepath, string filePath, bool isOriginal, string fileName)
        {
            //判断源图片大小小于等于被压缩的图片格式并且非原图压缩
            //则无需压缩直接保存到目标地址

            if (!isOriginal && (uploadImage.Height <= height || uploadImage.Width <= width))
            {
                SaveFixedImage(uploadImage, width, height, filePath, midFilepath, fileName, false);
            }
            else
            {
                Image resizedImage = ImageResizingManagerBP.FixedSize(uploadImage, width, height);
                SaveFixedImage(resizedImage, width, height, filePath, midFilepath, fileName, false);
                resizedImage.Dispose();
            }
        }

        /// <summary>
        /// 读取文件保存路径
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public string GetFileSavePath(string fileType)
        {
            string savePath = "";
            switch (fileType)
            {
                case ".jpg":
                    savePath = FilePathTitle + "Original/";
                    break;
                case ".swf":
                case ".flv":
                    savePath = FilePathTitle + "360/";
                    break;
                default:
                    break;
            }

            return savePath;

        }

        /// <summary>
        /// 支持多服务器保存
        /// </summary>
        /// <param name="sFileName">临时文件完整路径</param>
        /// <param name="dFileName">目标文件名</param>
        private void FileCopy(string sFileName, string dFileName, string filePath)
        {
            //重命名文件
            File.Copy(sFileName, filePath + dFileName, true);
        }

        #region 获取文件存储物理随机路径
        private static string GetSubFolderName(string fileName)
        {
            string SavePath = "{0}/{1}/{2}/";
            fileName = fileName.ToLower();
            return string.Format(SavePath, GetSingleCharacter(fileName), GetCoupleInteger(fileName), GetCoupleHex(fileName));
        }

        private static string GetSingleCharacter(string key)
        {
            //Format to A-Z
            return ((char)('A' + GetCharToIntSum(key, false) % 26)).ToString();
        }

        private static string GetCoupleInteger(string key)
        {
            //Format to 00-99
            return (GetCharToIntSum(key, true) % 100).ToString("00");
        }

        private static string GetCoupleHex(string key)
        {
            //Format to 00-FF
            return (GetCharToIntSum(key, true) % 256).ToString("X").PadLeft(2, '0');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="isOdd">是否只取奇数</param>
        /// <returns></returns>
        private static int GetCharToIntSum(string chars, bool isOdd)
        {
            int sumAsciiValue = 0;
            if (!string.IsNullOrEmpty(chars))
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    char fileNameChar = chars[i];
                    if (isOdd)
                    {
                        if (i % 2 != 0) sumAsciiValue += fileNameChar * i;
                    }
                    else
                    {
                        sumAsciiValue += fileNameChar;
                    }
                }
            }

            return sumAsciiValue;
        }

        #endregion

    }
}
