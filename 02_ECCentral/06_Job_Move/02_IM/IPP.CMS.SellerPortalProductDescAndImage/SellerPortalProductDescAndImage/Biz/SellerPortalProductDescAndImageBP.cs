using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.Contract;
using System.Transactions;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.Utilities;
using IPP.CN.IM.Core.Interface.Item;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.DA;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.Biz
{
    public class SellerPortalProductDescAndImageBP
    {
        public static JobContext jobContext = null;
        public static string InUser = ConfigurationManager.AppSettings["InUser"];
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static string LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
        private static string ProductDescLongImageUrl = ConfigurationManager.AppSettings["ProductDescLongImageUrl"];
        public static string ImageServicePath = ConfigurationManager.AppSettings["ImageServicePath"];
        public static string DefaultDelay = ConfigurationManager.AppSettings["DefaultDelay"];
        public static string FriendlyDelay = ConfigurationManager.AppSettings["FriendlyDelay"];
        public static WebProxy proxy;

        public static string _IsTest = ConfigurationManager.AppSettings["IsTest"];
        public static string ByProxy = ConfigurationManager.AppSettings["ByProxy"];
        public static string ByProxyHost = ConfigurationManager.AppSettings["ByProxyHost"];
        public static string ByProxyPort = ConfigurationManager.AppSettings["ByProxyPort"];
        public static string ByProxyUser = ConfigurationManager.AppSettings["ByProxyUser"];
        public static string ByProxyPassword = ConfigurationManager.AppSettings["ByProxyPassword"];

        /// <summary>
        /// Seller Portal处理新产品图片
        /// </summary>
        public static void SellerPortalProductRequestForNewProduct()
        {

            WriteLog("\r\n" + DateTime.Now + "------------------- Begin-------------------------");
            WriteLog("\r\n" + DateTime.Now + "对Seller新建商品导入job开始运行......");

            #region 处理商品描述外链(新品创建)

            WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息描述Start......");
            List<ItemVendorProductRequestEntity> productRequestDescLongList = SellerPortalProductDescAndImageDA.GetSellerPortalProductRequestDescLongList();

            //获取ExInfoStatus为P的，详细信息描述列表
            //insert into ProductRequest_ProcessLog，记录原来的详细描述值
            //update ProductRequest_Ex表
            //update ipp3.dbo.Product
            //update OverseaContentManagement.dbo.ProductCommonInfo_Ex
            foreach (ItemVendorProductRequestEntity item in productRequestDescLongList)
            {
                string mailbody = string.Empty;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息:" + item.SysNo.ToString() + " Start......");
                    if (string.IsNullOrEmpty(item.ProductDescLong))
                    {
                        //如果详细信息描述无img Src处理，则把ExInfoStatus修改为"F"
                        SellerPortalProductDescAndImageDA.UpdateSellerPortalProductRequestExInfoStatus(item.SysNo);
                    }
                    else
                    {
                        //string productDescLong = GetProductDescLong(item.ProductDescLong, item.SysNo);
                        string productDescLong = item.ProductDescLong;
                        bool isError = false;
                        List<string> imageSrcList = new List<string>();
                        List<string> imageUrlList = new List<string>();
                        if (item.ProcessCount == 0)//处理新商品的描述信息
                        {
                            //过滤html,js,css代码
                            productDescLong = RemoveHtmlStr(item.ProductDescLong);

                            //读取Img标签的SRC
                            imageSrcList = GetImageSrc(productDescLong);
                            //读取URL标签中的图片地址
                            imageUrlList = GetImageUrl(productDescLong);

                            imageSrcList.AddRange(imageUrlList);
                            imageSrcList = imageSrcList.Distinct().ToList();
                        }
                        else
                        {
                            //获取处理成功的图片信息
                            SellerPortalProductDescAndImageDA.GetProductRequestImageListLog(item.SysNo, "F").ForEach(p =>
                            {
                                productDescLong.Replace(p.ImageUrl, Path.Combine(ProductDescLongImageUrl, p.ImageName));
                            });
                            //获取处理失败的图片信息
                            imageSrcList = SellerPortalProductDescAndImageDA.GetProductRequestImageListLog(item.SysNo, "E").Select(p =>
                            {
                                return p.ImageUrl;
                            }).ToList();

                            //获取未处理的图片信息
                            List<string> notBeginList = SellerPortalProductDescAndImageDA.GetProductRequestImageListLog(item.SysNo, "O").Select(p =>
                            {
                                return p.ImageUrl;
                            }).ToList();
                            imageSrcList.AddRange(notBeginList);
                        }
                        //有需要处理的img图片链接
                        if (imageSrcList.Count > 0)
                        {
                            #region Process Each ImageScr
                            foreach (string imageSrc in imageSrcList)
                            {
                                if (imageSrc.Equals(""))
                                {
                                    continue;
                                }
                                string status = "O";
                                string memo = "未下载";
                                string fileName = string.Empty;
                                if (!isError)
                                {
                                    WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息图片:" + imageSrc + "Start......");

                                    if (WhiteList_Check_IsExists(imageSrc))
                                    {
                                        int delay = 100;
                                        if (!int.TryParse(FriendlyDelay, out delay))
                                        {
                                            delay = 100;
                                        }
                                        Thread.Sleep(delay);
                                    }
                                    else
                                    {
                                        int delay = 500;
                                        if (!int.TryParse(DefaultDelay, out delay))
                                        {
                                            delay = 500;
                                        }
                                        Thread.Sleep(delay);
                                    }

                                    try
                                    {
                                        WriteLog("\r\n" + DateTime.Now + "正在验证图片请求地址的可访问性......");
                                        HttpWebRequest req = HttpWebRequest.Create(imageSrc) as HttpWebRequest;
                                        req.Method = "HEAD";
                                        req.Timeout = 5000;
                                        //--------------------发布要取消代理-----------------------
                                        if (ByProxy == "Y")
                                        {
                                            proxy = new WebProxy(ByProxyHost, string.IsNullOrEmpty(ByProxyPort) ? 8080 : int.Parse(ByProxyPort));
                                            proxy.Credentials = new NetworkCredential(ByProxyUser, ByProxyPassword);
                                            req.Proxy = proxy;
                                        }
                                        WriteLog("\r\n\t\t" + "请求超时时间：" + req.Timeout.ToString() + "ms");
                                        //--------------------发布要取消代理-----------------------
                                        HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                                        int statuscode = (int)res.StatusCode;
                                        WriteLog("\r\n\t\t" + "请求响应状态：" + statuscode.ToString() + "\t" + "详细描述：" + res.StatusDescription);
                                        if (res.StatusCode == HttpStatusCode.OK)
                                        {
                                            string fileType = "";
                                            switch (res.ContentType)
                                            {
                                                case "image/jpeg":
                                                    fileType = ".jpg";
                                                    break;
                                                case "image/bmp":
                                                    fileType = ".bmp";
                                                    break;
                                                case "image/gif":
                                                    fileType = ".gif";
                                                    break;
                                                case "image/png":
                                                    fileType = ".png";
                                                    break;
                                            }
                                            //新文件名
                                            fileName = Guid.NewGuid().ToString() + fileType;
                                            WebDownload myWebClient = new WebDownload();
                                            myWebClient.Timeout = req.Timeout;

                                            //本地测试需要设置代理
                                            if (ByProxy == "Y")
                                            {
                                                myWebClient.Proxy = proxy;
                                            }
                                            Image uploadImage = Image.FromStream(new MemoryStream(myWebClient.DownloadData(imageSrc)));
                                            string dFilePath = ImageServicePath;
                                            if (!Directory.Exists(dFilePath))
                                            {
                                                Directory.CreateDirectory(dFilePath);
                                            }
                                            uploadImage.Save(dFilePath + fileName);

                                            //替换原始文件名称
                                            productDescLong = productDescLong.Replace(imageSrc, ProductDescLongImageUrl + fileName);
                                        }
                                        else
                                        {
                                            isError = true;
                                            memo = res.StatusDescription;
                                        }
                                        res.Close();

                                    }
                                    catch (Exception ex)
                                    {
                                        isError = true;
                                        WriteLog("\r\n" + DateTime.Now + " 商品详细描述图片获取失败!" + imageSrc + ex.Message + ex.StackTrace);
                                        memo = ex.Message;
                                    }

                                    if (isError)
                                    {
                                        //ProductRequest_Ex count + 1
                                        SellerPortalProductDescAndImageDA.SetProductRequestExCount(item.SysNo);

                                        status = "E";
                                        if (item.ProcessCount >= 5)
                                        {
                                            mailbody = string.Format(mailbodyTemplate, item.C3Name, item.ProductID, item.ProductName, memo, item.PMName);
                                        }
                                        fileName = string.Empty;
                                    }
                                    else
                                    {
                                        status = "F";
                                        memo = "";
                                    }

                                    WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息图片:" + imageSrc + ".End.....");
                                }
                                SellerPortalProductDescAndImageDA.InsertProductRequestImageProcessLog(imageSrc, fileName, item.SysNo, status, memo);
                            }
                            #endregion Process Each ImageScr
                        }
                        if (!isError)
                        {
                            SellerPortalProductDescAndImageDA.ApproveProductRequest_Ex(item.SysNo, productDescLong);
                        }
                        else if (item.ProcessCount >= 5)
                        {
                            SellerPortalProductDescAndImageDA.SendMailIMGProcFail("(Info)商家商品详细描述信息处理失败[JOB]", mailbody);
                        }
                    }
                    scope.Complete();
                    WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息:" + item.SysNo.ToString() + "End......");
                }
            }


            WriteLog("\r\n" + DateTime.Now + "正在处理商品详细信息描述End......");
            #endregion

            #region 存在未处理的新品图片
            List<ItemVendorProductRequestFileEntity> requestImageList = SellerPortalProductDescAndImageDA.GetSellerPortalProductRequestImageList();
            if (requestImageList.Count > 0)
            {

                foreach (ItemVendorProductRequestFileEntity item in requestImageList)
                {
                    try
                    {

                        WriteLog("\r\n" + DateTime.Now + "正在处理品商:" + item.CommonSKUNumber.ToString() + "......");
                        DefaultDataContract rtn = new DefaultDataContract();
                        ProductDFISImage service = new ProductDFISImage();

                        EntityHeader header = new EntityHeader();
                        header.CompanyCode = CompanyCode;
                        header.Language = LanguageCode;
                        header.OriginalGUID = Guid.NewGuid().ToString();
                        header.OperationUser = new OperationUserEntity();
                        header.OperationUser.FullName = InUser;
                        header.OperationUser.CompanyCode = "8601";
                        header.OperationUser.LogUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
                        header.OperationUser.SourceDirectoryKey = "bitkoo";
                        header.OperationUser.SourceUserName = InUser;
                        header.OperationUser.UniqueUserName = "IPPSystemAdmin\\bitkoo\\IPPSystemAdmin[8601]";
                        rtn = service.UploadSendorPortalImageList(item.SysNo, item.GroupSysNo, item.comskuSysNo, item.CommonSKUNumber, header);

                        if (rtn.Faults != null && rtn.Faults.Count > 0)
                        {
                            WriteLog("\r\n" + DateTime.Now + "商品:" + item.CommonSKUNumber.ToString() + " 处理失败......");
                            WriteLog("\r\n" + rtn.Faults[0].ErrorDescription);
                        }
                        else
                        {
                            WriteLog("\r\n" + DateTime.Now + "商品:" + item.CommonSKUNumber.ToString() + " 处理成功......");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("\r\n" + DateTime.Now + "商品:" + item.CommonSKUNumber.ToString() + " 处理失败......");
                        WriteLog("\r\n" + ex.Message);
                    }
                }

            }
            else
            {
                WriteLog("\r\n" + DateTime.Now + " 无商品图片处理......");
            }
            #endregion

            using (TransactionScope scope = new TransactionScope())
            {
                #region count>5，删除ProductRequest_Files表记录，同时在表ProductRequest_ProcessLog增加log日志
                List<ItemVendorProductRequestFileEntity> exceedFiveCountList = SellerPortalProductDescAndImageDA.GetExceedFiveCountList();
                if (exceedFiveCountList.Count > 0)
                {
                    foreach (ItemVendorProductRequestFileEntity li in exceedFiveCountList)
                    {
                        WriteLog("\r\n" + DateTime.Now + "正在处理失败>5的商品:" + li.SysNo.ToString() + "开始......");
                        SellerPortalProductDescAndImageDA.DeleteSellerPortalProductRequestImageFiles(li.SysNo);
                        ItemVendorProductRequestEntity item = SellerPortalProductDescAndImageDA.GetSellerPortalProductRequestBySysNo(li.ProductRequestSysno);
                        string mailbody = string.Format(mailbodyTemplate, item.C3Name, item.ProductID, item.ProductName, "", item.PMName);
                        SellerPortalProductDescAndImageDA.SendMailIMGProcFail("(Info)商家商品图片信息处理失败[JOB]", mailbody);
                        WriteLog("\r\n" + DateTime.Now + "处理失败>5的商品:" + li.SysNo.ToString() + "成功！......");
                    }
                }
                #endregion

                #region 查询详细描述图片每个ProductRequestSysno都为"F"的list，然后处理其ExInfoStatus为F
                List<ProductRequestImage> longDescFStatusList = SellerPortalProductDescAndImageDA.GetSellerPortalProductLongDescFStatusList();
                if (longDescFStatusList.Count > 0)
                {
                    foreach (ProductRequestImage li in longDescFStatusList)
                    {
                        //WriteLog("\r\n" + DateTime.Now + "正在处理商品:" + li.ProductRequestSysno.ToString() + "的FileStatus状态为F......");
                        SellerPortalProductDescAndImageDA.UpdateSellerPortalProductRequestExInfoStatus(li.ProductRequestSysNo);
                        SellerPortalProductDescAndImageDA.InsertProductDescProductRequest_ProcessLog(li.ProductRequestSysNo);
                        //WriteLog("\r\n" + DateTime.Now + "商品:" + li.ProductRequestSysno.ToString() + "的FileStatus状态成功修改为F......");
                    }
                }
                #endregion

                #region 查询图片每个ProductRequestSysno都为"F"的list，然后处理其FileStatus为F
                List<ItemVendorProductRequestFileEntity> imageFilesFStatusList = SellerPortalProductDescAndImageDA.GetSellerPortalImageFFilesFStatusList();
                if (imageFilesFStatusList.Count > 0)
                {
                    foreach (ItemVendorProductRequestFileEntity li in imageFilesFStatusList)
                    {
                        //WriteLog("\r\n" + DateTime.Now + "正在处理商品:" + li.ProductRequestSysno.ToString() + "的FileStatus状态为F......");
                        SellerPortalProductDescAndImageDA.UpdateSellerPortalProductRequestFileStatus(li.ProductRequestSysno);
                        //WriteLog("\r\n" + DateTime.Now + "商品:" + li.ProductRequestSysno.ToString() + "的FileStatus状态成功修改为F......");
                    }
                }
                #endregion

                #region 处理三个状态都 BasicInfoStatus = "F",FileStatus = "F",ExInfoStatus = "F"，则Status = "F"
                List<ItemVendorProductRequestFileEntity> threeFStatusList = SellerPortalProductDescAndImageDA.GetSellerPortalThreeFStatusList();
                if (threeFStatusList.Count > 0)
                {
                    foreach (ItemVendorProductRequestFileEntity li in threeFStatusList)
                    {
                        WriteLog("\r\n" + DateTime.Now + "正在处理商品:" + li.ProductRequestSysno.ToString() + "的Status状态为F......");
                        SellerPortalProductDescAndImageDA.UpdateSellerPortalProductRequestStatus(li.ProductRequestSysno, "F");

                        //同步SellPortal表状态
                        SellerPortalProductDescAndImageDA.CallExternalSP(li.ProductRequestSysno);

                        WriteLog("\r\n" + DateTime.Now + "商品:" + li.ProductRequestSysno.ToString() + "的Status状态成功修改为F......");
                    }
                }
                #endregion


                scope.Complete();
            }

            WriteLog("\r\n" + DateTime.Now + "Job.......END......");

        }

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                SellerPortalProductDescAndImageDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }



        /// <summary>
        /// 过滤html,js,css代码
        /// </summary>
        /// <param name="html">参数传入</param>
        /// <returns></returns>
        private static string RemoveHtmlStr(string html)
        {
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+?</script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"<a.*?>|</a>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@"(<[^>]*)\son[\s\S]*?=([^>]*>)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+?</iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+?</frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"<!--[\s\S]*?-->", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"<form.*?>|</form>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            html = regex1.Replace(html, ""); //过滤<script></script>标记
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性
            html = regex3.Replace(html, "$1$2"); //过滤其它控件的on...事件
            html = regex4.Replace(html, ""); //过滤iframe
            html = regex5.Replace(html, ""); //过滤frameset
            html = regex6.Replace(html, ""); //过滤注释
            html = regex7.Replace(html, ""); //过滤From

            return html;
        }

        /// <summary>
        /// 读取Img标签的SRC
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<string> GetImageSrc(string str)
        {
            string regStr = @"<img[^>]*src=[""']?([^""'\s]+)[""']?[^>]*>";
            string cont1 = string.Empty; //图片的src
            Regex reg = new Regex(regStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            string picSrc = "";
            while (match.Success)
            {
                picSrc = match.Groups[1].Value;
                cont1 += string.Format("{0}", picSrc + ",");
                match = match.NextMatch();
            }

            if (cont1.Contains(","))
            {
                cont1 = cont1.Substring(0, cont1.LastIndexOf(','));
            }

            string[] strList = cont1.Split(',');
            return strList.ToList();
        }

        /// <summary>
        /// 读取URL标签中的图片地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<string> GetImageUrl(string str)
        {
            string regStr = @"url\(['""]?([^'""\)]+)['""]?\)";
            string cont1 = string.Empty; //图片的src
            Regex reg = new Regex(regStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            string picSrc = "";
            while (match.Success)
            {
                picSrc = match.Groups[1].Value;
                cont1 += string.Format("{0}", picSrc + ",");
                match = match.NextMatch();
            }

            if (cont1.Contains(","))
            {
                cont1 = cont1.Substring(0, cont1.LastIndexOf(','));
            }

            string[] strList = cont1.Split(',');
            return strList.ToList();
        }

        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }


        public static List<string> WhiteList;
        public static bool WhiteList_Check_IsExists(string url)
        {
            if (WhiteList == null)
            {
                WhiteList = new List<string>();
                string whitelistfile = "whitelist.xml";
                if (File.Exists(whitelistfile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(whitelistfile);
                    foreach (XmlNode item in doc.DocumentElement.ChildNodes)
                    {
                        WhiteList.Add(item.InnerText);
                    }
                }
            }
            bool isExists = false;
            //Regex reg = new Regex(@"^(http|https|ftp):\/\/(([A-Z0-9][A-Z0-9_-]*)(\.[A-Z0-9][A-Z0-9_-]*)+)(:(\d+))?\/?/i]");
            WhiteList.Contains(url);
            WhiteList.ForEach(p =>
            {
                if (url.Contains(p))
                {
                    isExists = true;
                    return;
                }
            });
            return isExists;
        }


        private static string mailbodyTemplate = @"<table border=0 cellspacing=0 cellpadding=0><tr><td width=100 style='width:75.0pt;background:#B7DEE8;padding:0cm 0cm 0cm 0cm'><p class=MsoNormal>商品类别<span lang=EN-US><o:p></o:p></span></p></td><td width=100 style='width:75.0pt;background:#B7DEE8;padding:0cm 0cm 0cm 0cm'><p class=MsoNormal>商品号<span lang=EN-US><o:p></o:p></span></p></td><td width=150 style='width:112.5pt;background:#B7DEE8;padding:0cm 0cm 0cm 0cm'><p class=MsoNormal>商品名称<span lang=EN-US><o:p></o:p></span></p></td><td width=200 style='width:150.0pt;background:#B7DEE8;padding:0cm 0cm 0cm 0cm'><p class=MsoNormal>失败原因<span lang=EN-US><o:p></o:p></span></p></td><td width=100 style='width:75.0pt;background:#B7DEE8;padding:0cm 0cm 0cm 0cm'><p class=MsoNormal>所属<span lang=EN-US>PM<o:p></o:p></span></p></td></tr><tr><td style='padding:0cm 0cm 0cm 0cm'><p class=MsoNormal style='word-break:break-all'><span lang=EN-US></span>{0}<span lang=EN-US><o:p></o:p></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class=MsoNormal style='word-break:break-all'><span lang=EN-US>{1}<o:p></o:p></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class=MsoNormal style='word-break:break-all'>{2}</p></td><td style='padding:0cm 0cm 0cm 0cm'><p class=MsoNormal style='word-break:break-all'>{3}<span lang=EN-US><o:p></o:p></span></p></td><td style='padding:0cm 0cm 0cm 0cm'><p class=MsoNormal style='word-break:break-all'><span lang=EN-US>{4}<o:p></o:p></span></p></td></tr></table>";
    }
}
