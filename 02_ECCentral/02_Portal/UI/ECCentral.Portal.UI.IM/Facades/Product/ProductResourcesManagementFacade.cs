//************************************************************************
// 用户名				泰隆优选
// 系统名				商品图片管理
// 子系统名		        商品图片Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM.Resource;
using ECCentral.Service.IM.Restful;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Facades.Product
{
    public class ProductResourcesManagementFacade
    {
        #region 属性以及构造函数
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/IMService/ProductResource/CreateProductResource";
        const string UPdateRelativeUrl = "/IMService/ProductResource/ModifyProductResources";
        const string DeleteRelativeUrl = "/IMService/ProductResource/DeleteProductResource";
        const string CreateImagesRelativeUrl = "/IMService/ProductResource/CreateImages";
        const string QueryFileNameUrl = "/IMService/ProductResource/IsExistFileName";
        const string QueryUrl = "/IMService/ProductResource/QueryResourceList";

        public delegate void BindHadle(int args);
        public delegate void SortHadle();

        public BindHadle OnBind { get; set; }

        public SortHadle OnSort { get; set; }

        private ProductResourcesManagementVM _productResources;
        public ProductResourcesManagementVM ProductResources
        {
            get { return _productResources; }
            set { _productResources = value; }
        }

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductResourcesManagementFacade(ProductResourcesManagementVM entity)
        {
            _productResources = entity;
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductResourcesManagementFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductResourcesManagementFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 创建资源文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateResourcesManagement(ProductResourceRequestMsg data, EventHandler<RestClientEventArgs<IList<ProductResourceForNewegg>>> callback)
        {
            if (data == null) return;
            _restClient.Create(CreateRelativeUrl, data, callback);
        }

        /// <summary>
        /// 创建资源文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="callback"></param>
        public void IsExistFileName(List<string> fileName, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            if (fileName == null || fileName.Count==0) return;
            _restClient.Query(QueryFileNameUrl, fileName, callback);
        }

        /// <summary>
        /// 创建资源文件
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void CreateImages(string fileIdentity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (string.IsNullOrEmpty(fileIdentity)) return;
            _restClient.Create(CreateImagesRelativeUrl, _productResources.ProductResources, callback);
        }

        /// <summary>
        /// 修改资源文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateResourcesManagement(ProductResourceRequestMsg data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (data == null) return;
            _restClient.Update(UPdateRelativeUrl, data, callback);
        }

        /// <summary>
        /// 删除资源文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void DeleteResourcesManagement(ProductResourceRequestMsg data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (data == null) return;
            _restClient.Delete(DeleteRelativeUrl, data, callback);
        }

        /// <summary>
        /// 查询商品组信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryResources(ProductResourcesQueryVM model, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var filter=new ResourceQueryFilter();
            filter.CommonSKUNumberList = model.CommonSKUNumberList;

            filter.PagingInfo = new PagingInfo
            {
                SortBy = sortField
            };


            _restClient.QueryDynamicData(QueryUrl, filter,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        callback(obj, args);
                    }
                );
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        public void SelectFiles()
        {
            var openFileDialog = new OpenFileDialog { Multiselect = true };

            if (!string.IsNullOrEmpty(StaticConfiguration.FileFilter))
            {
                openFileDialog.Filter = StaticConfiguration.FileFilter;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (FileInfo fileInfo in openFileDialog.Files)
                {

                    var tempEntity = GetProductResourcesVM(fileInfo);
                    if (tempEntity.ImageSize == 0) continue;
                    if (_productResources == null)
                    {
                        _productResources = new ProductResourcesManagementVM { ResourceCollection = new ProductResourcesCollection() };
                    }
                    _productResources.ResourceCollection.Add(tempEntity);

                }
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public void Upload()
        {
            if (_productResources == null) return;
            if (_productResources.ResourceCollection == null) return;
            if (_productResources.ProductResources == null)
                _productResources.ProductResources = new List<ProductResourceForNewegg>();
            var count = _productResources.ResourceCollection.GetCountByStatus(FileUploadProcessStates.WaitingToUpload);
            if (count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(ResProductResourcesManagement.CurrNotUploadFile, MessageBoxType.Warning);
                return;
            }
            var productResources = new List<ProductResourceForNewegg>();
            _productResources.ResourceCollection.ForEach(v => Deployment.Current.Dispatcher.BeginInvoke(() => UpLoad(v, productResources, count)));

        }


        /// <summary>
        /// 清理文件
        /// </summary>
        public void Clear()
        {
            if (_productResources == null) return;
            if (_productResources.ResourceCollection == null) return;
            _productResources.ResourceCollection.Remove();
            //_productResources.ResourceCollection.ForEach(v => DeleteFileFromServer(v.FileIdentity));
            var productResources = new List<ProductResourceForNewegg>();
            _productResources.ResourceCollection.ForEach(v =>
            {
                if (v.FileUploadProcessStates == FileUploadProcessStates.Finished)
                {

                    var resource = new ProductResourceForNewegg
                    {
                        Resource = new ResourceForNewegg()
                    };
                    resource.Resource.ResourceURL = v.File.Name;
                    resource.Resource.Type = v.FileType;
                    resource.Resource.ResourceSysNo = v.ResourceSysNo;
                    resource.Resource.OperateUser = new UserInfo
                                                        {
                                                            SysNo = CPApplication.Current.LoginUser.userSysNo,
                                                            UserDisplayName =
                                                                CPApplication.Current.LoginUser.DisplayName
                                                        };
                    productResources.Add(resource);
                }
            });
            var prarm = new ProductResourceRequestMsg { ProductResources = productResources };
            DeleteResourcesManagement(prarm, (obj, arg) =>
                                                           {
                                                               if (arg.FaultsHandle())
                                                               {
                                                                   return;
                                                               }
                                                               _productResources.ResourceCollection.Clear();
                                                               if (OnBind != null)
                                                               {
                                                                   OnBind(_productResources.ProductSysNo);
                                                               }
                                                           });

        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            if (_productResources == null) return;
            if (_productResources.ProductResources == null || _productResources.ProductResources.Count == 0) return;
            var prarm = new ProductResourceRequestMsg { ProductResources = _productResources.ProductResources };
            UpdateResourcesManagement(prarm, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
            });
            if (_productResources.DeleteProductResources == null || _productResources.DeleteProductResources.Count == 0) return;
            prarm = new ProductResourceRequestMsg { ProductResources = _productResources.DeleteProductResources };
            DeleteResourcesManagement(prarm, (obj, arg) =>
           {
               if (arg.FaultsHandle())
               {
                   return;
               }
               CPApplication.Current.CurrentPage.Context.Window.Alert(ResProductResourcesManagement.OperationSuccess);
           });
        }

        /// <summary>
        /// 置顶
        /// </summary>
        public void MoveTop(int sysNo)
        {
            if (_productResources == null || _productResources.ProductResources == null || _productResources.ProductResources.Count == 0
                || sysNo <= 0 || _productResources.ProductResources.Count == 1) return;
            var single = _productResources.ProductResources.Where(p => p.Resource.ResourceSysNo == sysNo).SingleOrDefault();
            if (single == null) return;
            if (single.Resource.ResourceSysNo == _productResources.ProductResources[0].Resource.ResourceSysNo) return;
            var firstPriority = _productResources.ProductResources[0].Resource.Priority;
            var arrys = _productResources.ProductResources.ToArray();
            if (firstPriority != 1)
            {
                single.Resource.Priority = 1;
            }
            else
            {
                for (int i = 0; i < arrys.Count(); i++)
                {
                    if (arrys[i].Resource.ResourceSysNo == single.Resource.ResourceSysNo) break;
                    arrys[i].Resource.Priority = arrys[i].Resource.Priority + 1;
                }
                single.Resource.Priority = firstPriority;
            }
            if (OnSort != null)
            {
                OnSort();
            }
        }

        /// <summary>
        /// 上移
        /// </summary>
        public void MoveUp(int sysNo)
        {
            if (_productResources == null || _productResources.ProductResources == null || _productResources.ProductResources.Count == 0
                || sysNo <= 0 || _productResources.ProductResources.Count == 1) return;
            var single = _productResources.ProductResources.Where(p => p.Resource.ResourceSysNo == sysNo).SingleOrDefault();
            if (single == null) return;
            if (single.Resource.ResourceSysNo == _productResources.ProductResources[0].Resource.ResourceSysNo) return;
            var arrays = _productResources.ProductResources.ToArray();
            for (int i = 0; i < arrays.Count(); i++)
            {
                if (arrays[i].Resource.ResourceSysNo != single.Resource.ResourceSysNo) continue;
                if (i == 0) break;
                var index = arrays[i].Resource.Priority;
                arrays[i].Resource.Priority = arrays[i - 1].Resource.Priority;
                arrays[i - 1].Resource.Priority = index;
            }
            if (OnSort != null)
            {
                OnSort();
            }
        }

        /// <summary>
        /// 下移
        /// </summary>
        public void MoveDown(int sysNo)
        {
            if (_productResources == null || _productResources.ProductResources == null || _productResources.ProductResources.Count == 0
                || sysNo <= 0 || _productResources.ProductResources.Count == 1) return;
            var single = _productResources.ProductResources.Where(p => p.Resource.ResourceSysNo == sysNo).SingleOrDefault();
            if (single == null) return;
            var tempIndex = _productResources.ProductResources.Count - 1;
            if (single.Resource.ResourceSysNo == _productResources.ProductResources[tempIndex].Resource.ResourceSysNo) return;
            var arrays = _productResources.ProductResources.ToArray();
            for (int i = 0; i < arrays.Count(); i++)
            {
                if (arrays[i].Resource.ResourceSysNo != single.Resource.ResourceSysNo) continue;
                var index = arrays[i].Resource.Priority;
                arrays[i].Resource.Priority = arrays[i + 1].Resource.Priority;
                arrays[i + 1].Resource.Priority = index;
                break;
            }
            if (OnSort != null)
            {
                OnSort();
            }
        }

        /// <summary>
        /// 置底
        /// </summary>
        public void MoveBottom(int sysNo)
        {
            if (_productResources == null || _productResources.ProductResources == null || _productResources.ProductResources.Count == 0
                || sysNo <= 0 || _productResources.ProductResources.Count == 1) return;
            var index = _productResources.ProductResources.Count - 1;
            var lastPriority = _productResources.ProductResources[index].Resource.Priority;
            var single = _productResources.ProductResources.Where(p => p.Resource.ResourceSysNo == sysNo).SingleOrDefault();
            if (single == null) return;
            if (single.Resource.ResourceSysNo == _productResources.ProductResources[index].Resource.ResourceSysNo) return;
            single.Resource.Priority = lastPriority + 1;
            if (OnSort != null)
            {
                OnSort();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sysNo"></param>
        public void Delete(int sysNo)
        {
            if (_productResources == null || _productResources.ProductResources == null || _productResources.ProductResources.Count == 0
                || sysNo <= 0) return;
            var item = _productResources.ProductResources.Where(p => p.Resource.ResourceSysNo == sysNo).SingleOrDefault();
            if (item == null) return;
            if (_productResources.DeleteProductResources == null) _productResources.DeleteProductResources = new List<ProductResourceForNewegg>();
            _productResources.DeleteProductResources.Add(item);
            _productResources.ProductResources.Remove(item);
            if (OnSort != null)
            {
                OnSort();
            }
        }

        /// <summary>
        /// 清理文件
        /// </summary>
        public void ClearWaitUploadFiles()
        {
            if (_productResources == null) return;
            if (_productResources.ResourceCollection == null) return;
            _productResources.ResourceCollection.Remove();
            _productResources.ResourceCollection.Clear();
            if (OnBind != null)
            {
                OnBind(_productResources.ProductSysNo);
            }
        }

        #region 私有方法
        private ProductResourcesVM GetProductResourcesVM(FileInfo fileInfo)
        {
            if (fileInfo == null) return new ProductResourcesVM();
            var source = new ProductResourcesVM { FileGuid = Guid.NewGuid(), File = fileInfo };
            GetFileEntity(fileInfo, source);
            source.ProductCommonInfoSysNo = _productResources.ProductCommonInfoSysNo;
            if (source.FileUploadProcessStates != FileUploadProcessStates.CheckFailed)
            {
                CheckFileEntity(fileInfo, source);
            }
            return source;
        }


       
        private ProductResourceForNewegg GetProductResource(ProductResourcesVM entity)
        {
            if (entity == null) return null;
            var resource = new ProductResourceForNewegg
            {
                Resource = new ResourceForNewegg()
            };
            resource.ProductCommonInfoSysNo = entity.ProductCommonInfoSysNo;
            resource.Resource.TemporaryName = entity.FileIdentity;
            resource.Resource.Type = entity.FileType;
            resource.LanguageCode = CPApplication.Current.LanguageCode;
            resource.CompanyCode = CPApplication.Current.CompanyCode;
            resource.Resource.OperateUser = new UserInfo
            {
                SysNo = CPApplication.Current.LoginUser.userSysNo,
                UserDisplayName =
                    CPApplication.Current.LoginUser.DisplayName
            };
            return resource;
        }

        /// <summary>
        /// 获取文件实体
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="entity"></param>
        private void GetFileEntity(FileInfo fileInfo, ProductResourcesVM entity)
        {
            if (entity == null) entity = new ProductResourcesVM();
            var myStream = fileInfo.OpenRead();
            var fileHead = new byte[3];
            myStream.Position = 0;
            myStream.Read(fileHead, 0, 3);
            string fileHeadInfo = fileHead[0].ToString() + fileHead[1].ToString() + fileHead[2].ToString();
            var fileType = Convert.ToInt32(fileHeadInfo);
            string fileTypedesc = fileType.ToString().Length > 6 ? fileType.ToString().Substring(0, 6) : fileType.ToString();
            var result = Enum.IsDefined(typeof(FileExtension), Convert.ToInt32(fileTypedesc));
            if (!result)
            {
                entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
                entity.Remark = "文件损坏";
                entity.PreviewImageVisibility = Visibility.Collapsed;
                entity.FileType = ResourcesType.NotSupported;
            }
            else
            {

                var filetype = (FileExtension)Enum.Parse(typeof(FileExtension), fileTypedesc, false);
                switch (filetype)
                {
                    case FileExtension.JPG:
                        entity.FileType = ResourcesType.Image;
                        entity.PreviewImageVisibility = Visibility.Visible;
                        entity.PreviewImage = new BitmapImage();
                        entity.PreviewImage.SetSource(myStream);
                        entity.FileType = ResourcesType.Image;
                        entity.Thumbnail = new BitmapImage();
                        entity.Thumbnail.SetSource(myStream);
                        CheckJPGType(entity);
                        break;
                    case FileExtension.SWF:
                        entity.FileType = ResourcesType.Image360;
                        entity.PreviewImageVisibility = Visibility.Collapsed;
                        CheckSWFType(entity);
                        break;
                    case FileExtension.SWF1:
                        entity.PreviewImageVisibility = Visibility.Collapsed;
                        entity.FileType = ResourcesType.Image360;
                        CheckSWFType(entity);
                        break;
                    case FileExtension.FLV:
                        entity.FileType = ResourcesType.Video;
                        entity.PreviewImageVisibility = Visibility.Collapsed;
                        CheckFLVType(entity);
                        break;
                }

            }
            entity.ImageSize = fileInfo.Length;

            entity.UploadedPercentage = 0;
        }

        /// <summary>
        /// 获取文件实体
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="entity"></param>
        private void CheckFileEntity(FileInfo fileInfo, ProductResourcesVM entity)
        {
            if (entity == null) entity = new ProductResourcesVM();
            var message = "";
            var commonSKUNumber = CheckFileName(fileInfo.Name, ref message);
            if (string.IsNullOrWhiteSpace(commonSKUNumber))
            {
                entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
                entity.Remark = message;
                entity.PreviewImageVisibility = Visibility.Collapsed;
                entity.FileType = ResourcesType.NotSupported;
                return;
            }
            //if (_productResources != null
            //            && !string.IsNullOrWhiteSpace(_productResources.CommonSKUNumber)
            //            && _productResources.CommonSKUNumber != commonSKUNumber)
            //{
            //    entity.Remark = "资源文件" + fileInfo.Name + "不在此商品下！";
            //    entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            //    entity.PreviewImageVisibility = Visibility.Collapsed;
            //    //entity.FileType = ResourcesType.NotSupported;
            //    return;
            //}
            entity.CommonSKUNumber = commonSKUNumber;
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="relativeUriString"></param>
        /// <returns></returns>
        private BitmapImage LoadImage(string relativeUriString)
        {
            // Get the image stream at the specified URI that
            // is relative to the application package root.
            Uri uri = new Uri(relativeUriString, UriKind.Relative);
            StreamResourceInfo sri = Application.GetResourceStream(uri);

            // Convert the stream to an Image object.
            BitmapImage bi = new BitmapImage();
            bi.SetSource(sri.Stream);

            return bi;
        }

        /// <summary>
        /// 检查图片
        /// </summary>
        /// <param name="entity"></param>
        private void CheckJPGType(ProductResourcesVM entity)
        {
            //if (entity.Thumbnail.PixelWidth == 1280 && entity.Thumbnail.PixelHeight == 1024)
            //{
            //    if (entity.ImageSize > 3 * StaticConfiguration.MBLimits)
            //    {
            //        entity.Remark = "分辨率为1280*1024的图片体积应不大于3兆。";
            //        entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            //    }
            //}
            //else if (entity.Thumbnail.PixelWidth == 800 && entity.Thumbnail.PixelHeight == 600)
            //{
            //    if (entity.ImageSize > 2 * StaticConfiguration.MBLimits)
            //    {
            //        entity.Remark = "分辨率为800*600的图片体积应不大于2兆。";
            //        entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            //    }
            //}
            //else if (entity.Thumbnail.PixelWidth == 640 && entity.Thumbnail.PixelHeight == 480)
            //{
            //    if (entity.ImageSize > 2 * StaticConfiguration.MBLimits)
            //    {
            //        entity.Remark = "分辨率为640*480的图片体积应不大于2兆。";
            //        entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            //    }
            //}
            //else
            //{
            //    entity.Remark = "请选择分辨率为1280*1024/800*600/640*480的图片。";
            //    entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            //}
        }

        /// <summary>
        /// 检查360图片
        /// </summary>
        /// <param name="entity"></param>
        private void CheckSWFType(ProductResourcesVM entity)
        {
            entity.Thumbnail = LoadImage(@"/ECCentral.Portal.UI.IM;Component/Resources/360p.png");

            if (entity.ImageSize > 5 * StaticConfiguration.MBLimits)
            {
                entity.Remark = "360图片体积应不大于5兆。";
                entity.FileUploadProcessStates = FileUploadProcessStates.CheckFailed;
            }

        }

        private void CheckFLVType(ProductResourcesVM entity)
        {
            entity.Thumbnail = LoadImage(@"/ECCentral.Portal.UI.IM;Component/Resources/videop.png");
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="productResources"></param>
        /// <param name="count"></param>
        private void UpLoad(ProductResourcesVM entity, List<ProductResourceForNewegg> productResources, int count)
        {
            if (productResources == null) productResources = new List<ProductResourceForNewegg>();
            if (entity.FileUploadProcessStates != FileUploadProcessStates.WaitingToUpload)
            {
                entity.FileIdentity = "-1";
                return;
            }
            var client = new FileUploadClient(ConstValue.DomainName_IM, entity.File, 1);
            //if (entity.FileType == ResourcesType.Image)
            //{
            //    client.AppName = "product";
            //}
            //if (entity.FileType == ResourcesType.Image360)
            //{
            //    client.AppName = "ProductImage360";
            //}

            //if (entity.FileType == ResourcesType.Video)
            //{
            //    client.AppName = "ProductVideo";
            //}
            client.AppName = "product";
            entity.FileUploadProcessStates = FileUploadProcessStates.Uploading;
            client.UploadProgressChanged += (se, args) =>
            {
                long t = args.TotalUploadedDataLength;  // 已经上传的数据大小
                entity.UploadedPercentage = (float)t / entity.ImageSize;   // 上传数据的百分比
            };
            client.UploadErrorOccured += (se, args) =>
            {
                entity.FileUploadProcessStates = FileUploadProcessStates.UploadFailed;
                entity.Remark = args.Exception.Message;
                CPApplication.Current.CurrentPage.Context.Window.Alert(args.Exception.Message);
                entity.FileIdentity = "-1";
            };
            client.UploadCompleted += (se, args) =>
            {
                entity.FileUploadProcessStates = FileUploadProcessStates.Uploaded;
                entity.Remark = "";
                entity.FileIdentity = args.FileIdentity;
                var source = GetProductResource(entity);
                source.Resource.ResourceURL = args.ServerFilePath;
                source.IsNeedWatermark = _productResources.IsNeedWatermark;
                productResources.Add(source);
                if (count == productResources.Count)
                {
                    _productResources.ResourceCollection.ForEach(v =>
                                                                     {
                                                                         if (v.FileUploadProcessStates == FileUploadProcessStates.Uploaded)
                                                                         {
                                                                             v.FileUploadProcessStates =
                                                                                 FileUploadProcessStates.Processing;
                                                                         }
                                                                     });
                    CreateProductResources(productResources);
                }
            };
            client.Upload();

        }

        /// <summary>
        /// 生成图片资源
        /// </summary>
        /// <param name="productResources"></param>
        private void CreateProductResources(List<ProductResourceForNewegg> productResources)
        {
            if (productResources == null || productResources.Count == 0) return;
            var source = productResources.Where(p => p.Resource.TemporaryName != "-1").ToList();
            if (source != null && source.Count > 0)
            {
                var prarm = new ProductResourceRequestMsg { ProductResources = source };
                CreateResourcesManagement(prarm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        _productResources.ResourceCollection.ForEach(v =>
                        {
                            if (v.FileUploadProcessStates == FileUploadProcessStates.Processing)
                            {
                                v.FileUploadProcessStates =
                                    FileUploadProcessStates.ProcessFailed;
                            }
                        });
                        return;
                    }
                    var index = 0;
                    _productResources.ResourceCollection.ForEach(v =>
                    {
                        if (v.FileUploadProcessStates == FileUploadProcessStates.Processing)
                        {
                            v.FileUploadProcessStates =
                                FileUploadProcessStates.Finished;
                            if (args.Result[index] != null && args.Result[index] != null)
                            {
                                var productResource = args.Result[index];
                                if (productResource != null)
                                    if (productResource.Resource.ResourceSysNo != null)
                                        v.ResourceSysNo = productResource.Resource.ResourceSysNo.Value;
                            }
                            index++;
                        }
                    });
                    if (OnBind != null)
                    {
                        OnBind(_productResources.ProductSysNo);
                    }
                    
                });
            }
        }

        private string CheckFileName(string fileName, ref string message)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                message = "文件名不能为空！";
                return "";
            }
            var re = new Regex(@"^([^\.\-]+)(\-(([1-9][0-9])|(0[1-9])))?\..+?$", RegexOptions.IgnoreCase);
            var match = re.Match(fileName);

            //Check上传文件名是否合法
            if (match.Success)
            {
                var commonSKUNumber = match.Groups[1].Value;
                return commonSKUNumber;
            }
            message = "文件" + fileName + "命名不规范！";
            return "";
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="relativeUriString"></param>
        /// <returns></returns>
        private string LoadModel(string relativeUriString)
        {
            // Get the image stream at the specified URI that
            // is relative to the application package root.
            var uri = new Uri(relativeUriString, UriKind.Relative);
            StreamResourceInfo sri = Application.GetResourceStream(uri);

            // Convert the stream to an Image object.
            var reader = new StreamReader(sri.Stream);

            var htmlContext=reader.ReadToEnd();


            return htmlContext;
        }

        #endregion

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum FileUploadProcessStates
    {
        [Description("验证文件")]
        Checking,
        [Description("检查失败")]
        CheckFailed,
        [Description("等待上传")]
        WaitingToUpload,
        [Description("上传中")]
        Uploading,
        [Description("上传失败")]
        UploadFailed,
        [Description("上传成功")]
        Uploaded,
        [Description("处理中")]
        Processing,
        [Description("处理失败")]
        ProcessFailed,
        [Description("完成")]
        Finished
    }

    public enum FileExtension
    {
        [Description("JPG图片")]
        JPG = 255216,
        [Description("SWF文件")]
        SWF = 678783,
        [Description("SWF文件")]
        SWF1 = 708783,
        [Description("视频文件")]
        FLV = 707686

    }
}
