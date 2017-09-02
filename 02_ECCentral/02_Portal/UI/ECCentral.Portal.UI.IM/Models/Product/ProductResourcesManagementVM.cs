using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductResourcesManagementVM : ModelBase
    {
       
        private List<ProductResourceForNewegg> _productResources;
        /// <summary>
        /// 商品资源文件
        /// </summary>
        public List<ProductResourceForNewegg> ProductResources
        {
            get { return _productResources; }
            set {
                #region 2012-12-19 update Jack.G.Tang
                /*
                 *图片的优先级排序 
                 */
                #endregion
                int index=1;
                if (value != null)
                {
                    List<ProductResourceForNewegg> list = value as List<ProductResourceForNewegg>;
                    foreach (var item in list)
                    {
                        item.Resource.Priority = index;
                        index++;
                    }
                }
                SetValue("ProductResources", ref _productResources, value);
                
            
            }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品促销标题
        /// </summary>
        public String PromotionTitle { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public String ProductModel { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        public ProductResourcesCollection ResourceCollection
        {
            get;
            set;
        }


        /// <summary>
        /// 删除商品资源文件
        /// </summary>
        public List<ProductResourceForNewegg> DeleteProductResources { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }


        /// <summary>
        /// 商品CommonInfoSysNo
        /// </summary>
        public int ProductCommonInfoSysNo { get; set; }

        /// <summary>
        /// 商品CommonInfoSysNo
        /// </summary>
        public string CommonSKUNumber { get; set; }

        private int _productGroupCount;

        /// <summary>
        /// 商品组数量
        /// </summary>
        public int ProductGroupCount {
            get { return _productGroupCount; }
            set { SetValue("ProductGroupCount", ref _productGroupCount, value); }
        }

        private int _commonSKUCount;
        /// <summary>
        /// CommonSKU数量
        /// </summary>
        public int CommonSKUCount {  
            get { return _commonSKUCount; }
            set { SetValue("CommonSKUCount", ref _commonSKUCount, value); }
        }

        private int _sucessCount;
        /// <summary>
        /// 上传成功数量
        /// </summary>
        public int SucessCount
        {
            get { return _sucessCount; }
            set { SetValue("SucessCount", ref _sucessCount, value); }
        }

        private int _faileCount;
        /// <summary>
        /// 上传失败数量
        /// </summary>
        public int FaileCount
        {
            get { return _faileCount; }
            set { SetValue("FaileCount", ref _faileCount, value); }
        }

        private bool _isNeedWatermark;
        /// <summary>
        /// 是否添加水印
        /// </summary>
        public bool IsNeedWatermark
        {
            get { return _isNeedWatermark; }
            set { SetValue("IsNeedWatermark", ref _isNeedWatermark, value); }
        }
    }

    public class ProductResourcesCollection : ObservableCollection<ProductResourcesVM>
    {
        public long FileSize
        {
            get
            {
                long size = 0;

                size = this.Items.Sum(p => p.ImageSize);

                return size;
            }
        }

        public int FileCount
        {
            get
            {
                return this.Items.Count;
            }
        }

        public ProductResourcesVM this[Guid guid]
        {
            get
            {
                var entity = this.Items.Where(p => p.FileGuid == guid).SingleOrDefault();

                return entity;
            }
        }

        //public void Sort()
        //{
        //    this.Sort();
        //}

        /// <summary>
        /// 获取某个状态的个数
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int GetCountByStatus(FileUploadProcessStates status)
        {
            if (this.Items == null || this.Items.Count == 0) return 0;
            var count = this.Items.Where(p => p.FileUploadProcessStates == status).Count();
            return count;
        }

        /// <summary>
        /// 删除没有上传成功的
        /// </summary>
        /// <returns></returns>
        public void Remove()
        {
            if (this.Items == null || this.Items.Count == 0) return;
            var source =
                (from e in this.Items where e.FileUploadProcessStates < FileUploadProcessStates.Uploaded select e).
                    ToList();

            if (source == null) return;
            source.ForEach(v => this.Items.Remove(v));

        }

    }

    public class ProductResourcesVM : PropertyChangedBase
    {
        public Guid FileGuid
        {
            get;
            set;
        }

        /// <summary>
        /// 当前文件
        /// </summary>
        public FileInfo File { get; set; }

        private string _fileName;
        public string FileName
        {
            get
            {
                return File == null ? "" : File.Name;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public ResourcesType FileType { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage Thumbnail { get; set; }

        /// <summary>
        /// 预览视图
        /// </summary>
        public BitmapImage PreviewImage { get; set; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public long ImageSize { get; set; }

        /// <summary>
        /// 图片大小
        /// </summary>
        public string ImageSizeDesc
        {
            get { 
                var value = GetText(ImageSize);
                return value;
            }
        }

        /// <summary>
        /// 进度
        /// </summary>
        private float _uploadedPercentage;
        public float UploadedPercentage
        {
            get
            {
                return _uploadedPercentage;
            }
            set
            {
                _uploadedPercentage = value;
                this.NotifyPropertyChanged(p => p.UploadedPercentage);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private FileUploadProcessStates _fileUploadProcessStates;
        public FileUploadProcessStates FileUploadProcessStates
        {
            get
            {
                return _fileUploadProcessStates;
            }
            set
            {
                _fileUploadProcessStates = value;
                FieldInfo field = typeof(FileUploadProcessStates).GetField(value.ToString());
                object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = "";
                if (attrs != null && attrs.Length > 0)
                {
                    var desc = attrs[0] as DescriptionAttribute;
                    if (desc != null) description = desc.Description;
                }
                FileUploadProcessStatesDesc = description;
                this.NotifyPropertyChanged(p => p.FileUploadProcessStates);
            }
        }

        private string _fileUploadProcessStatesDesc;
        /// <summary>
        /// 状态描述
        /// </summary>
        public string FileUploadProcessStatesDesc
        {
            get
            {
                return _fileUploadProcessStatesDesc;
            }
            set
            {
                _fileUploadProcessStatesDesc = value;
                this.NotifyPropertyChanged(p => p.FileUploadProcessStatesDesc);
            }
        }

        /// <summary>
        /// 是否显示预览图片
        /// </summary>
        private Visibility _previewImageVisibility;
        public Visibility PreviewImageVisibility
        {
            get
            {
                return _previewImageVisibility;
            }
            set
            {
                _previewImageVisibility = value;
                this.NotifyPropertyChanged(p => p.PreviewImageVisibility);
            }
        }

        private string _remark;
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                this.NotifyPropertyChanged(p => p.Remark);
            }
        }

        /// <summary>
        /// 文件标志
        /// </summary>
        public string FileIdentity { get; set; }

        /// <summary>
        /// 资源编号
        /// </summary>
        public int ResourceSysNo { get; set; }

        public int? ProductGroupSysNo { get; set; }

        /// <summary>
        /// 商品CommonInfoSysNo
        /// </summary>
        public string CommonSKUNumber { get; set; }

        private string GetText(double size)
        {
            if (size < 1024)
                return string.Format("{0} B", size.ToString());
            else if (size < 1024 * 1024)
                return string.Format("{0} KB", (size / 1024.0f).ToString("0.0"));
            else
                return string.Format("{0} MB", (size / (1024.0f * 1024.0f)).ToString("0.0"));
        }

        public int ProductCommonInfoSysNo { get; set; }
    }

    public static class StaticConfiguration
    {
        public static int Decimal = 1024;
        public static int BLimits = 1;
        public static int KBLimits = Decimal * BLimits;
        public static int MBLimits = Decimal * KBLimits;
        public static int GBLimits = Decimal * MBLimits;

        public static int ChunckSize = 10 * KBLimits;

        //public static int ProductGroupSysNo;

        //public static UploadTargetType UploadTargetType;

        public static string ProductGroupSysNoParameterName = "pgs";

        public static string FileFilter = "JPG Files (*.jpg)|*.jpg|SWF Files (*.swf)|*.swf|FLV Files (*.flv)|*.flv";

        public static string ExcelFileFilter = "Excel Files (*.xlsx)|*.xlsx|Excel Files (*.xls)|*.xls";

        public static int UploadThreadMaxCount = 1;
    }

  
}
