using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using PostSharp.Laos;

namespace ECCentral.Service.IM.BizProcessor.IMAOP
{
    [Serializable]
    public sealed class ProductInfoChangeAttribute : OnMethodBoundaryAspect
    {
        private static readonly Dictionary<string, MethodLog> MethodDic = new Dictionary<string, MethodLog>();

        private MethodLog _operationLog;
        private static readonly string[] GetOldValueMethod = new[] { "UpdateProductBasicInfo", "UpdateProductImageInfo", 
                                                              "UpdateProductDescriptionInfo","UpdateProductPriceInfo",
                                                              "UpdateProductPropertyInfo","UpdateProductDimensionInfo",
                                                              "ProductOnSale","ProductOnShow","ProductUnShow",
                                                              "ProductInvalid" };

        private static readonly string[] SpecialMethod = new[] { "ProductCopyProperty" };

        private static readonly string[] GetReturnValueMethod = new[] { "CreatetCategoryExtendWarranty", "CreatetCategoryExtendWarrantyDisuseBrand" };

        public override void OnEntry(MethodExecutionEventArgs eventArgs)
        {
            if (MethodDic.ContainsKey(eventArgs.Method.Name))
            {
                _operationLog = MethodDic[eventArgs.Method.Name];
            }
            else
            {
                InitMethodLog(eventArgs.Method.Name);
            }
            _operationLog.MethodName = eventArgs.Method.Name;
            for (var i = 0; i < eventArgs.GetReadOnlyArgumentArray().Count(); i++)
            {
                var value = eventArgs.GetReadOnlyArgumentArray()[i];
                if (value == null) continue;
                if (i == 0 || value is ProductInfo)
                {
                    _operationLog.ParameterType = value.GetType();
                    _operationLog.ParameterValue = value;
                }
                if (SpecialMethod.Contains(_operationLog.MethodName) && i == 1)
                {
                    _operationLog.ParameterType = value.GetType();
                    _operationLog.ParameterValue = value;
                }
            }
            if (GetOldValueMethod.Contains(eventArgs.Method.Name))
            {
                _operationLog.OldValue = GetOldValue(_operationLog.ParameterValue);
            }
        }

        public override void OnSuccess(MethodExecutionEventArgs eventArgs)
        {
            //eventArgs.GetReadOnlyArgumentArray().ToList().ForEach(o =>
            //{
            //    if (o is ProductInfo)
            //    {
            //        //var product = o as ProductInfo;
            //        //var content = _productInfo.ProductBasicInfo.LongDescription.Content + "至" + product.ProductBasicInfo.LongDescription.Content;
            //        //Utility.ObjectFactory<IBizInteract.ICommonBizInteract>.Instance.CreateOperationLog(content,
            //        //                                                                                   _bizLogType,
            //        //                                                                                   product.SysNo,
            //        //                                                                                   "8601");
            //        //比较product新---_productInfo旧区别并写日志
            //    }
            //});
            if (GetReturnValueMethod.Contains(_operationLog.MethodName))
            {
                _operationLog.ParameterType = eventArgs.ReturnValue.GetType();
                _operationLog.ParameterValue = eventArgs.ReturnValue;
            }
            _operationLog.AddLogEntityList();
            //ObjectFactory<IBizInteract.ICommonBizInteract>.Instance.CreateOperationLog(_productInfo.ToString(),
            //                                                                                   _bizLogType,
            //                                                                                   _productInfo.SysNo,
            //                                                                                   "8601");
        }

        /// <summary>
        /// 退出方法时写日志
        /// </summary>
        /// <param name="eventArgs"></param>
        public override void OnExit(MethodExecutionEventArgs eventArgs)
        {
            if (_operationLog != null)
                _operationLog.WriteLogEntityList();
        }

        /// <summary>
        /// 初始化写日志方法
        /// </summary>
        /// <param name="methodName"></param>
        private void InitMethodLog(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName)) return;
            lock (MethodDic)
            {
                if (!MethodDic.ContainsKey(methodName))
                {
                    switch (methodName)
                    {
                        case "ProductCreate":
                            _operationLog = ObjectFactory<ProductCreateLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "ProductOnSale":
                            _operationLog = ObjectFactory<ProductOnSaleLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "ProductOnShow":
                            _operationLog = ObjectFactory<ProductOnShowLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "CategoryRequestAuditPass":
                        case "CategoryRequestAuditNotPass":
                        case "CategoryRequestCanel":
                            _operationLog = ObjectFactory<ActiveCategoryRequestLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "ProductUnShow":
                            _operationLog = ObjectFactory<ProductUnShowLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "ProductInvalid":
                            _operationLog = ObjectFactory<ProductInvalidLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductBasicInfo":
                            _operationLog = ObjectFactory<UpdateProductBasicInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductImageInfo":
                            _operationLog = ObjectFactory<UpdateProductImageInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductDimensionInfo":
                            _operationLog = ObjectFactory<UpdateProductDimensionInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductDescriptionInfo":
                            _operationLog = ObjectFactory<UpdateProductDescriptionInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductPriceInfo":
                            _operationLog = ObjectFactory<UpdateProductPriceInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductWarrantyInfo":
                            _operationLog = ObjectFactory<UpdateProductWarrantyInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "ProductCopyProperty":
                            _operationLog = ObjectFactory<ProductCopyPropertyLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateProductPropertyInfo":
                            _operationLog = ObjectFactory<UpdateProductPropertyInfoLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "CreateCategoryRequest":
                            _operationLog = ObjectFactory<CreateCategoryRequestLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "CreatetCategoryExtendWarranty":
                            _operationLog = ObjectFactory<CreatetCategoryExtendWarrantyLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "AuditProductPriceRequest":
                            _operationLog = ObjectFactory<AuditProductPriceRequestLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateCategoryExtendWarranty":
                            _operationLog = ObjectFactory<UpdateCategoryExtendWarrantyLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "CreatetCategoryExtendWarrantyDisuseBrand":
                            _operationLog = ObjectFactory<CreatetCategoryExtendWarrantyDisuseBrandLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "UpdateCategoryExtendWarrantyDisuseBrand":
                            _operationLog = ObjectFactory<UpdateCategoryExtendWarrantyDisuseBrandLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "AuditRequestProductPrice":
                            _operationLog = ObjectFactory<AuditRequestProductPriceLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                        case "CancelAuditProductPriceRequest":
                            _operationLog = ObjectFactory<CancelAuditProductPriceRequestLog>.Instance;
                            MethodDic.Add(methodName, _operationLog);
                            break;
                    }
                }
            }
        }

        private object GetOldValue(object parameterValue)
        {
            object oldValue = null;
            var productSysNo = 0;
            if (parameterValue != null && parameterValue is ProductInfo)
            {
                var product = (ProductInfo)parameterValue;
                productSysNo = product.SysNo;
            }
            else if(parameterValue != null && parameterValue is int)
            {
                productSysNo = (int) parameterValue;
            }
            var productProcessor = ObjectFactory<ProductProcessor>.Instance;
            if (productProcessor != null && productSysNo > 0)
            {
                oldValue = productProcessor.GetProductInfo(productSysNo);
            }

            return oldValue;
        }
    }

    /// <summary>
    /// 方法日志
    /// </summary>
    [Serializable]
    public abstract class MethodLog
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object ParameterValue { get; set; }

        /// <summary>
        /// 旧数据
        /// </summary>
        public object OldValue { get; set; }

        /// <summary>
        /// 添加日志队列
        /// </summary>
        public abstract void AddLogEntityList();

        /// <summary>
        /// 写日志
        /// </summary>
        public virtual void WriteLogEntityList()
        {
            foreach (var log in LogEntity.PopAll())
            {
                ExternalDomainBroker.CreateOperationLog(log.OperationNote, log.LogType, log.TicketSysNo, "8601");
            }
        }
    }

    /// <summary>
    /// 日志队列
    /// </summary>
    public class LogEntity
    {
        [ThreadStatic]
        private static List<LogEntity> _list;

        private LogEntity()
        {
        }

        public static void Init()
        {
            if (_list == null)
                _list = new List<LogEntity>();
            else
                _list.Clear();
        }


        public static void Push(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            if (_list == null) _list = new List<LogEntity>();
            _list.Add(new LogEntity
                          {
                              OperationNote = note,
                              LogType = logType,
                              TicketSysNo = ticketSysNo,
                              CompanyCode = companyCode
                          });
        }


        public static IEnumerable<LogEntity> PopAll()
        {
            if (_list == null)
            {
                return new List<LogEntity>().ToArray();
            }
            var result = _list.ToArray();
            _list.Clear();
            return result;
        }

        public string OperationNote
        {
            get;
            private set;
        }

        public int TicketSysNo
        {
            get;
            private set;
        }

        public string LogCategory
        {
            get;
            private set;
        }

        public BizLogType LogType
        {
            get;
            private set;
        }

        public string CompanyCode
        {
            get;
            private set;
        }


    }

    /// <summary>
    /// 添加商品日志
    /// </summary>
    [VersionExport(typeof(ProductCreateLog))]
    public class ProductCreateLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is ProductGroup)
            {
                var value = (ProductGroup)ParameterValue;
                if (value.ProductList != null)
                {
                    foreach (var item in value.ProductList)
                    {
                        if (item.SysNo <= 0 || string.IsNullOrWhiteSpace(item.ProductID)) continue;
                        var operationNote = "添加商品：" + item.ProductID;
                        LogEntity.Push(operationNote, BizLogType.Basic_Product_Add, item.SysNo, "8601");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 商品上架日志
    /// </summary>
    [VersionExport(typeof(ProductOnSaleLog))]
    public class ProductOnSaleLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is int && OldValue != null && OldValue is ProductInfo)
            {
                var item = (ProductInfo) OldValue;
                if (!string.IsNullOrWhiteSpace(item.ProductID))
                {
                    var operationNote = "更新商品状态" + EnumHelper.GetDescription(item.ProductStatus) + "-->上架(商品ID:" + item.ProductID + ")";
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Status_Update, item.SysNo, "8601");
                }
            }
        }
    }

    /// <summary>
    /// 商品仅展示日志
    /// </summary>
    [VersionExport(typeof(ProductOnShowLog))]
    public class ProductOnShowLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is int && OldValue != null && OldValue is ProductInfo)
            {
                var item = (ProductInfo)OldValue;
                if (!string.IsNullOrWhiteSpace(item.ProductID))
                {
                    var operationNote = "更新商品状态" + EnumHelper.GetDescription(item.ProductStatus) + "-->仅展示(商品ID:" + item.ProductID + ")";
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Status_Update, item.SysNo, "8601");
                }
            }
        }
    }

    /// <summary>
    /// 商品不展示日志
    /// </summary>
    [VersionExport(typeof(ProductUnShowLog))]
    public class ProductUnShowLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is int && OldValue != null && OldValue is ProductInfo)
            {
                var item = (ProductInfo)OldValue;
                if (!string.IsNullOrWhiteSpace(item.ProductID))
                {
                    var operationNote = "更新商品状态" + EnumHelper.GetDescription(item.ProductStatus) + "-->不展示(商品ID:" + item.ProductID + ")";
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Status_Update, item.SysNo, "8601");
                }
            }
        }
    }

    /// <summary>
    /// 商品作废日志
    /// </summary>
    [VersionExport(typeof(ProductInvalidLog))]
    public class ProductInvalidLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is int && OldValue != null && OldValue is ProductInfo)
            {
                var item = (ProductInfo)OldValue;
                if (!string.IsNullOrWhiteSpace(item.ProductID))
                {
                    var operationNote = "更新商品状态" + EnumHelper.GetDescription(item.ProductStatus) + "-->作废(商品ID:" + item.ProductID + ")";
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Status_Update, item.SysNo, "8601");
                }
            }
        }
    }

    /// <summary>
    /// 修改商品基本信息日志
    /// </summary>
    [VersionExport(typeof(UpdateProductBasicInfoLog))]
    public class UpdateProductBasicInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateProductPromotionTitle();
            UpdateProductNoteLog();
        }

        /// <summary>
        /// 更新商品标题和商品促销信息的日志
        /// </summary>
        private void UpdateProductPromotionTitle()
        {
            var operationNote = "";
            if (OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var productProcessor = ObjectFactory<ProductProcessor>.Instance;
                if (productProcessor != null)
                {
                    var item = productProcessor.GetProductInfo(product.SysNo);
                    if (item == null) return;
                    var result = IsWriteTitleLog(product, item);
                    if (result)
                    {
                        operationNote = "修改商品标题为：" + item.ProductBasicInfo.ProductTitle.Content;
                    }
                    var message = GetPromotionTitle(item);
                    item.PromotionTitle.Content = message;
                    result = IsWritePromotionTitleLog(product, item);
                    if (result)
                    {
                        operationNote = (string.IsNullOrWhiteSpace(operationNote) ? "" : operationNote + "&") + "修改商品促销信息为：" + item.PromotionTitle.Content;
                    }
                    if (!string.IsNullOrWhiteSpace(operationNote))
                        LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, item.SysNo, "8601");
                }
            }

        }

        /// <summary>
        /// 更新商品备注的日志
        /// </summary>
        private void UpdateProductNoteLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteProductNoteLog(product, item);
                if (result)
                {
                    operationNote = string.Format(@"修改商品备注信息为：{0}", item.ProductBasicInfo.Note);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品促销信息
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWritePromotionTitleLog(ProductInfo oldValue, ProductInfo newValue)
        {
            if (oldValue == null || newValue == null) return false;
            if (oldValue.PromotionTitle == null && newValue.PromotionTitle != null)
            {
                return true;
            }
            if (oldValue.PromotionTitle != null && newValue.PromotionTitle == null)
            {
                return true;
            }
            if (oldValue.PromotionTitle != null && newValue.PromotionTitle != null)
            {
                if (oldValue.PromotionTitle.Content != newValue.PromotionTitle.Content)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private string GetPromotionTitle(ProductInfo newValue)
        {
            if (newValue.ProductTimelyPromotionTitle == null || newValue.ProductTimelyPromotionTitle.Count == 0)
                return "";
            var message = "";
            foreach (var item in newValue.ProductTimelyPromotionTitle)
            {
                if(item.BeginDate==null)
                {
                    message = item.PromotionTitle.Content;
                }
                else if(item.EndDate.HasValue)
                {
                    var time = DateTime.Now;
                    if(time>item.BeginDate.Value&&time<=item.EndDate.Value)
                    {
                        message = item.PromotionTitle.Content;
                    }
                }
            }
            return message;
        }
        /// <summary>
        /// 是否更改了商品标题信息
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteTitleLog(ProductInfo oldValue, ProductInfo newValue)
        {
            if (oldValue == null || newValue == null) return false;
            if (oldValue.ProductBasicInfo.ProductTitle == null && newValue.ProductBasicInfo.ProductTitle != null)
            {
                return true;
            }
            if (oldValue.ProductBasicInfo.ProductTitle != null && newValue.ProductBasicInfo.ProductTitle == null)
            {
                return true;
            }
            if (oldValue.ProductBasicInfo.ProductTitle != null && newValue.ProductBasicInfo.ProductTitle != null)
            {
                if (oldValue.ProductBasicInfo.ProductTitle.Content != newValue.ProductBasicInfo.ProductTitle.Content)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否更改了商品备注
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteProductNoteLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductBasicInfo.Note != newValue.ProductBasicInfo.Note;
        }

    }

    /// <summary>
    /// 修改商品图片信息日志
    /// </summary>
    [VersionExport(typeof(UpdateProductImageInfoLog))]
    public class UpdateProductImageInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateVirtualPicStatus();
        }

        /// <summary>
        /// 更新商品标题和商品促销信息的日志
        /// </summary>
        private void UpdateVirtualPicStatus()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteIsVirtualPicLog(product, item);
                if (result)
                {
                    operationNote = "" + (item.ProductBasicInfo.IsVirtualPic == ProductIsVirtualPic.Yes ? "勾选虚库图片" : "取消勾选虚库图片");
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Pic_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品促销信息
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteIsVirtualPicLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductBasicInfo.IsVirtualPic != newValue.ProductBasicInfo.IsVirtualPic;
        }

       
    }


    /// <summary>
    /// 修改商品重量日志
    /// </summary>
    [VersionExport(typeof(UpdateProductDimensionInfoLog))]
    public class UpdateProductDimensionInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateWeightLog();
            UpdateDimensionLog();
        }

        /// <summary>
        /// 更新商品重量的日志
        /// </summary>
        private void UpdateDimensionLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteDimensionInfoLog(product, item);
                if (result)
                {
                    operationNote = String.Format("修改商品体积（长宽高）为：{0}，{1}，{2}", item.ProductBasicInfo.ProductDimensionInfo.Length
                                    , item.ProductBasicInfo.ProductDimensionInfo.Width, item.ProductBasicInfo.ProductDimensionInfo.Height);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Weight_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 更新商品重量的日志
        /// </summary>
        private void UpdateWeightLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteWeightLog(product, item);
                if (result)
                {
                    operationNote = "修改商品重量为:" + item.ProductBasicInfo.ProductDimensionInfo.Weight;
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Weight_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品重量
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteWeightLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductBasicInfo.ProductDimensionInfo.Weight != newValue.ProductBasicInfo.ProductDimensionInfo.Weight;
        }

        /// <summary>
        /// 是否更改了商品体积
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteDimensionInfoLog(ProductInfo oldValue, ProductInfo newValue)
        {
            var oldwidth= oldValue.ProductBasicInfo.ProductDimensionInfo.Width;
            var newwidth = newValue.ProductBasicInfo.ProductDimensionInfo.Weight;
            var oldLength = oldValue.ProductBasicInfo.ProductDimensionInfo.Length;
            var newLength = newValue.ProductBasicInfo.ProductDimensionInfo.Length;
            var oldHeight = oldValue.ProductBasicInfo.ProductDimensionInfo.Height;
            var newHeight = newValue.ProductBasicInfo.ProductDimensionInfo.Height;
            if(oldwidth!=newwidth||oldLength!=newLength||oldHeight!=newHeight)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 修改商品详细描述日志
    /// </summary>
    [VersionExport(typeof(UpdateProductDescriptionInfoLog))]
    public class UpdateProductDescriptionInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateProductDescriptionLog();
        }

        /// <summary>
        /// 修改商品详细描述日志
        /// </summary>
        private void UpdateProductDescriptionLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                 && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteProductDescriptionInfoLog(product, item);
                if (result)
                {
                    operationNote = "修改商品详细描述";
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品详细
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteProductDescriptionInfoLog(ProductInfo oldValue, ProductInfo newValue)
        {
            if (oldValue == null || newValue == null) return false;
            if (oldValue.ProductBasicInfo.LongDescription == null && newValue.ProductBasicInfo.LongDescription != null)
            {
                return true;
            }
            if (oldValue.ProductBasicInfo.LongDescription != null && newValue.ProductBasicInfo.LongDescription == null)
            {
                return true;
            }
            if (oldValue.ProductBasicInfo.LongDescription != null && newValue.ProductBasicInfo.LongDescription != null)
            {
                if (oldValue.ProductBasicInfo.LongDescription.Content != newValue.ProductBasicInfo.LongDescription.Content)
                {
                    return true;
                }
            }
            return false;
        }



    }

    /// <summary>
    /// 修改商品价格日志
    /// </summary>
    [VersionExport(typeof(UpdateProductPriceInfoLog))]
    public class UpdateProductPriceInfoLog : MethodLog
    {
        private ProductInfo _item;
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateProductPriceNoteLog();
            UpdateProductBasicPriceLog();
            UpdateProductPriceLog();
            MinCountPerOrderLog();
            MinCommissionLog();
        }

        /// <summary>
        /// 更新商品备注的日志
        /// </summary>
        private void UpdateProductPriceNoteLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var result = IsWriteProductPriceNoteLog(product, item);
                if (result)
                {
                    operationNote = string.Format(@"修改商品备注信息为：{0}", item.ProductBasicInfo.Note);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 更改了商品最小订购量日志
        /// </summary>
        private void MinCountPerOrderLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var result = IsWriteMinCountPerOrderLog(product, _item);
                if (result)
                {
                    operationNote = string.Format("修改商品最小订购量为：{0}", _item.ProductPriceInfo.MinCountPerOrder);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, _item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 修改商品最低佣金限额日志
        /// </summary>
        private void MinCommissionLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var result = IsWriteMinCommissionLog(product, _item);
                if (result)
                {
                    operationNote = string.Format("修改商品最低佣金限额为：{0}", _item.ProductPriceInfo.MinCommission);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, _item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 修改对比价的日志
        /// </summary>
        private void UpdateProductBasicPriceLog()
        {
            var operationNote = "";
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                _item = null;
                var productProcessor = ObjectFactory<ProductProcessor>.Instance;
                if (productProcessor != null && product.SysNo > 0)
                {
                    _item = productProcessor.GetProductInfo(product.SysNo);
                }
                if (_item == null) return;
                var result = IsWriteProductBasicPriceLog(product, _item);
                if (result)
                {
                    operationNote = string.Format("修改商品【{0}】对比价记录：{1}修改为{2}", _item.ProductID, product.ProductPriceInfo.BasicPrice, _item.ProductPriceInfo.BasicPrice);
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, _item.SysNo, "8601");
            }

        }


        /// <summary>
        /// 修改价格的日志
        /// </summary>
        private void UpdateProductPriceLog()
        {
            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                if (_item == null) return;
                var productWholeSalePriceInfo1 =
                    _item.ProductPriceInfo.ProductWholeSalePriceInfo.Where(p => p.Level == WholeSaleLevelType.L1).
                        FirstOrDefault() ?? new ProductWholeSalePriceInfo();
                var productWholeSalePriceInfo2 =
                   _item.ProductPriceInfo.ProductWholeSalePriceInfo.Where(p => p.Level == WholeSaleLevelType.L2).
                       FirstOrDefault() ?? new ProductWholeSalePriceInfo();
                var productWholeSalePriceInfo3 =
                   _item.ProductPriceInfo.ProductWholeSalePriceInfo.Where(p => p.Level == WholeSaleLevelType.L3).
                       FirstOrDefault() ?? new ProductWholeSalePriceInfo();
                var operationNote = string.Format("更新价格:ProductSysNo-{0},BasicPrice-{1},CurrentPrice-{2},Discount-{3},UnitCost-{4},CashRebate-{5},Point-{6},MaxPreOrder-{7},Q1-{8},P1-{9},Q2-{10},P2-{11},Q3-{12},P3-{13},CreateTime-{14},CreateUserSysNo-{15}",
                _item.SysNo, _item.ProductPriceInfo.BasicPrice, _item.ProductPriceInfo.CurrentPrice, _item.ProductPriceInfo.DiscountAmount, _item.ProductPriceInfo.UnitCost, _item.ProductPriceInfo.CashRebate, _item.ProductPriceInfo.Point, _item.ProductPriceInfo.MaxCountPerDay,
                productWholeSalePriceInfo1.Qty, productWholeSalePriceInfo1.Price, productWholeSalePriceInfo2.Qty, productWholeSalePriceInfo2.Price, productWholeSalePriceInfo3.Qty, productWholeSalePriceInfo3.Price, DateTime.Now, ServiceContext.Current.UserSysNo);
                LogEntity.Push(operationNote, BizLogType.Basic_Product_Basic_Update, _item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品备注
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteProductPriceNoteLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductBasicInfo.Note != newValue.ProductBasicInfo.Note;
        }

        /// <summary>
        /// 是否更改了商品对比价
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteProductBasicPriceLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductPriceInfo.BasicPrice != newValue.ProductPriceInfo.BasicPrice;
        }

        /// <summary>
        /// 是否更改了商品最小订购量
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteMinCountPerOrderLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductPriceInfo.MinCountPerOrder != newValue.ProductPriceInfo.MinCountPerOrder;
        }

        /// <summary>
        /// 是否修改商品最低佣金限额
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsWriteMinCommissionLog(ProductInfo oldValue, ProductInfo newValue)
        {
            return oldValue.ProductPriceInfo.MinCommission != newValue.ProductPriceInfo.MinCommission;
        }
    }

    /// <summary>
    /// 修改商品质保日志
    /// </summary>
    [VersionExport(typeof(UpdateProductWarrantyInfoLog))]
    public class UpdateProductWarrantyInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateProductWarrantyLog();
        }

        /// <summary>
        /// 更新商品质保的日志
        /// </summary>
        private void UpdateProductWarrantyLog()
        {
            const string operationNote = "更新质保信息";
            if (ParameterValue != null && ParameterValue is ProductInfo)
            {
                var item = (ProductInfo)ParameterValue;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_PM_Update, item.SysNo, "8601");

            }
        }
    }

    /// <summary>
    /// 克隆商品规格参数
    /// </summary>
    [VersionExport(typeof(ProductCopyPropertyLog))]
    public class ProductCopyPropertyLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            if (ParameterValue != null && ParameterValue is int)
            {

                var productProcessor = ObjectFactory<ProductProcessor>.Instance;
                if (productProcessor != null)
                {
                    var sysNo = (int)ParameterValue;
                    var item = productProcessor.GetSimpleProductInfo(sysNo);
                    if (!string.IsNullOrWhiteSpace(item.ProductID))
                    {
                        var operationNote = "商品信息复制：" + item.ProductID;
                        LogEntity.Push(operationNote, BizLogType.Basic_Product_Copy, item.SysNo, "8601");
                    }
                }
            }
        }


    }

    /// <summary>
    /// 修改商品属性日志
    /// </summary>
    [VersionExport(typeof(UpdateProductPropertyInfoLog))]
    public class UpdateProductPropertyInfoLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {

            if (ParameterValue != null && ParameterValue is ProductInfo
                && OldValue != null && OldValue is ProductInfo)
            {
                var product = (ProductInfo)OldValue;
                var item = (ProductInfo)ParameterValue;
                var source = GetWriteProductPriceLog(product, item);
                var operationNote = source.Join(",");
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, BizLogType.Basic_Product_Attribute_Update, item.SysNo, "8601");
            }

        }

        /// <summary>
        /// 是否更改了商品属性
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private IEnumerable<string> GetWriteProductPriceLog(ProductInfo oldValue, ProductInfo newValue)
        {
            var oldCount = oldValue.ProductBasicInfo.ProductProperties == null
                            ? 0
                            : oldValue.ProductBasicInfo.ProductProperties.Count;
            var newCount = newValue.ProductBasicInfo.ProductProperties == null
                            ? 0
                            : newValue.ProductBasicInfo.ProductProperties.Count;
            var getProductPropertyChange = new List<string>();
            if (oldCount == 0 && newCount == 0)
            {
                return getProductPropertyChange;
            }
            if (oldCount > 0 && newCount > 0 && newValue.ProductBasicInfo.ProductProperties != null)
            {
                newValue.ProductBasicInfo.ProductProperties.ForEach(v =>
                        {
                            var oldProductPropertie = oldValue.ProductBasicInfo.ProductProperties.Where(
                                               p => p.Property.PropertyInfo.SysNo == v.Property.SysNo).FirstOrDefault();
                            var propertyName = v.Property.PropertyInfo.PropertyName.Content;
                            var valueDescription = v.Property.ValueDescription == null
                                                          ? ""
                                                          : v.Property.ValueDescription.Content;
                            var personalizedValue = v.PersonalizedValue == null
                                                         ? ""
                                                         : v.PersonalizedValue.Content;
                            if (oldProductPropertie != null)
                            {
                                var oldPersonalizedValue = oldProductPropertie.PersonalizedValue == null
                                                         ? ""
                                                         : oldProductPropertie.PersonalizedValue.Content;
                                if (personalizedValue == oldPersonalizedValue)
                                {
                                    personalizedValue = "";
                                }
                                var oldValueDescription = oldProductPropertie.Property.ValueDescription == null
                                                          ? ""
                                                          : oldProductPropertie.Property.ValueDescription.Content;
                                if (oldValueDescription == valueDescription)
                                {
                                    valueDescription = "";
                                }
                            }
                            var note = "";
                            if (!string.IsNullOrWhiteSpace(valueDescription) || !string.IsNullOrWhiteSpace(personalizedValue))
                            {
                                personalizedValue = string.IsNullOrWhiteSpace(personalizedValue)
                                                        ? ""
                                                        : "\"" + personalizedValue + "\"";
                                valueDescription = string.IsNullOrWhiteSpace(valueDescription)
                                                       ? ""
                                                       : "\"" + valueDescription + "\"";
                                note = String.Format("{0}:{1}{2}", propertyName, valueDescription, personalizedValue);
                            }
                            if (!string.IsNullOrEmpty(note))
                            {
                                getProductPropertyChange.Add(note);
                            }
                        });
            }
            return getProductPropertyChange;
        }
    }


    /// <summary>
    /// 类别创建日志
    /// </summary>
    [VersionExport(typeof(CreateCategoryRequestLog))]
    public class CreateCategoryRequestLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            CreateCategoryRequestInfoLog();
        }

        /// <summary>
        ///类别创建的日志
        /// </summary>
        private void CreateCategoryRequestInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryRequestApprovalInfo)
            {
                var item = (CategoryRequestApprovalInfo)ParameterValue;
                var operationNote = string.Format("提交审核：{0}", item.CategoryName);
                var logType = BizLogType.Basic_Category1_Add;
                switch (item.CategoryType)
                {
                    case CategoryType.CategoryType1:
                        logType = BizLogType.Basic_Category1_Add;
                        break;
                    case CategoryType.CategoryType2:
                        logType = BizLogType.Basic_Category2_Add;
                        break;
                    case CategoryType.CategoryType3:
                        logType = BizLogType.Basic_Category3_Add;
                        break;
                }
                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, logType, -999999, "8601");
            }

        }
    }

    /// <summary>
    /// 类别审核日志
    /// </summary>
    [VersionExport(typeof(ActiveCategoryRequestLog))]
    public class ActiveCategoryRequestLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            ActiveCategoryRequestInfoLog();
        }

        /// <summary>
        ///类别审核的日志
        /// </summary>
        private void ActiveCategoryRequestInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryRequestApprovalInfo)
            {
                var item = (CategoryRequestApprovalInfo)ParameterValue;
                var operationNote = "";
                var logType = BizLogType.Basic_Category1_Add;
                //大类审核日志
                if (item.CategoryType == CategoryType.CategoryType1)
                {
                    switch (item.Status)
                    {
                        case CategoryAuditStatus.CategoryAuditCanel:
                            operationNote = string.Format("大类取消审核：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category1_CancelAudit;
                            break;
                        case CategoryAuditStatus.CategoryAuditPass:
                            operationNote = string.Format("大类审核通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category1_Approved;
                            break;
                        case CategoryAuditStatus.CategoryAuditNotPass:
                            operationNote = string.Format("大类审核未通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category1_Declined;
                            break;
                    }    
                }

                //中类审核日志
                if (item.CategoryType == CategoryType.CategoryType2)
                {
                    switch (item.Status)
                    {
                        case CategoryAuditStatus.CategoryAuditCanel:
                            operationNote = string.Format("中类取消审核：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category2_CancelAudit;
                            break;
                        case CategoryAuditStatus.CategoryAuditPass:
                            operationNote = string.Format("中类审核通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category2_Approved;
                            break;
                        case CategoryAuditStatus.CategoryAuditNotPass:
                            operationNote = string.Format("中类审核未通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category2_Declined;
                            break;
                    }
                }

                //小类审核日志
                if (item.CategoryType == CategoryType.CategoryType3)
                {
                    switch (item.Status)
                    {
                        case CategoryAuditStatus.CategoryAuditCanel:
                            operationNote = string.Format("小类取消审核：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category3_CancelAudit;
                            break;
                        case CategoryAuditStatus.CategoryAuditPass:
                            operationNote = string.Format("小类审核通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category3_Approved;
                            break;
                        case CategoryAuditStatus.CategoryAuditNotPass:
                            operationNote = string.Format("小类审核未通过：{0}", item.CategoryName);
                            logType = BizLogType.Basic_Category3_Declined;
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(operationNote))
                    LogEntity.Push(operationNote, logType, -999999, "8601");
            }

        }
    }

    /// <summary>
    /// 添加类别延保日志
    /// </summary>
    [VersionExport(typeof(CreatetCategoryExtendWarrantyLog))]
    public class CreatetCategoryExtendWarrantyLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            CreatetCategoryExtendWarrantyInfoLog();
        }

        /// <summary>
        /// 类别延保的日志
        /// </summary>
        private void CreatetCategoryExtendWarrantyInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryExtendWarranty)
            {
                var item = (CategoryExtendWarranty)ParameterValue;
                var categoryName = item.CategoryInfo.CategoryName.Content;
                if (String.IsNullOrEmpty(categoryName))
                {
                    var categoryProcessor = ObjectFactory<CategoryProcessor>.Instance;
                    if (item.SysNo > 0 && item.CategoryInfo.SysNo > 0)
                    {
                        var category = categoryProcessor.GetCategory3BySysNo(item.CategoryInfo.SysNo.Value);
                        categoryName = category.CategoryName.Content;
                    }
                }
                var operationNote = "添加类别延保：" + categoryName + " " + item.Brand.BrandNameLocal.Content;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_CategoryExtendWarranty, item.SysNo != null ? item.SysNo.Value : 0, "8601");

            }
        }
    }

    /// <summary>
    /// 修改类别延保日志
    /// </summary>
    [VersionExport(typeof(UpdateCategoryExtendWarrantyLog))]
    public class UpdateCategoryExtendWarrantyLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateCategoryExtendWarrantyInfoLog();
        }

        /// <summary>
        /// 修改类别延保的日志
        /// </summary>
        private void UpdateCategoryExtendWarrantyInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryExtendWarranty)
            {
                var item = (CategoryExtendWarranty)ParameterValue;
                var operationNote = "更新类别延保：" + item.CategoryInfo.CategoryName.Content + " " + item.Brand.BrandNameLocal.Content;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_CategoryExtendWarranty, item.SysNo != null ? item.SysNo.Value : 0, "8601");

            }
        }
    }

    /// <summary>
    /// 创建不参加类别延保的品牌
    /// </summary>
    [VersionExport(typeof(CreatetCategoryExtendWarrantyDisuseBrandLog))]
    public class CreatetCategoryExtendWarrantyDisuseBrandLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            CreatetCategoryExtendWarrantyDisuseBrandInfoLog();
        }

        /// <summary>
        /// 类别延保的日志
        /// </summary>
        private void CreatetCategoryExtendWarrantyDisuseBrandInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryExtendWarrantyDisuseBrand)
            {
                var item = (CategoryExtendWarrantyDisuseBrand)ParameterValue;
                var categoryName = item.CategoryInfo.CategoryName.Content;
                if (String.IsNullOrEmpty(categoryName))
                {
                    var categoryProcessor = ObjectFactory<CategoryProcessor>.Instance;
                    if (item.SysNo > 0 && item.CategoryInfo.SysNo > 0)
                    {
                        var category = categoryProcessor.GetCategory3BySysNo(item.CategoryInfo.SysNo.Value);
                        categoryName = category.CategoryName.Content;
                    }
                }
                var operationNote = "添加不参加类别延保的品牌：" + categoryName + " " + item.Brand.BrandNameLocal.Content;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_CategoryExtendWarranty, item.SysNo != null ? item.SysNo.Value : 0, "8601");

            }
        }
    }

    /// <summary>
    /// 更新不参加类别延保的品牌
    /// </summary>
    [VersionExport(typeof(UpdateCategoryExtendWarrantyDisuseBrandLog))]
    public class UpdateCategoryExtendWarrantyDisuseBrandLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            UpdateCategoryExtendWarrantyDisuseBrandInfoLog();
        }

        /// <summary>
        /// 类别延保的日志
        /// </summary>
        private void UpdateCategoryExtendWarrantyDisuseBrandInfoLog()
        {
            if (ParameterValue != null && ParameterValue is CategoryExtendWarrantyDisuseBrand)
            {
                var item = (CategoryExtendWarrantyDisuseBrand)ParameterValue;
                var operationNote = "更新不参加类别延保的品牌：" + item.CategoryInfo.CategoryName.Content + " " +
                                    item.Brand.BrandNameLocal.Content;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_CategoryExtendWarranty, item.SysNo != null ? item.SysNo.Value : 0, "8601");

            }
        }
    }

    /// <summary>
    /// 商品价格审核日志
    /// </summary>
    [VersionExport(typeof(AuditProductPriceRequestLog))]
    public class AuditProductPriceRequestLog : MethodLog
    {
        private int _productSysNo;
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            AuditProductPriceRequestInfoLog();
            AuditProductPriceLiog();
        }

        /// <summary>
        /// 商品价格审核的日志
        /// </summary>
        private void AuditProductPriceRequestInfoLog()
        {
            if (ParameterValue != null && ParameterValue is ProductPriceRequestInfo)
            {
                var item = (ProductPriceRequestInfo)ParameterValue;
                var productPriceRequest = ObjectFactory<ProductPriceRequestProcessor>.Instance;
                BizLogType logType = BizLogType.Basic_Product_Price_PMRequestMemo;
                if (item.SysNo > 0)
                {
                    _productSysNo = productPriceRequest.GetProductSysNoByAuditProductPriceSysNo(item.SysNo.Value);
                }
                string operationNote;
                if (!string.IsNullOrEmpty(item.PMDMemo))
                {
                    operationNote = "PMD审核：" + item.PMDMemo;
                    logType = BizLogType.Basic_Product_Price_TLRequestMemo;
                }
                else
                {
                    operationNote = "TL审核：" + item.TLMemo;
                }
                LogEntity.Push(operationNote, logType, _productSysNo, "8601");
            }
        }

        /// <summary>
        /// 审核时记价格日志
        /// </summary>
        private void AuditProductPriceLiog()
        {
            if (ParameterValue != null && ParameterValue is ProductPriceRequestInfo)
            {
                var item = (ProductPriceRequestInfo)ParameterValue;
                var operationNote = "";
                BizLogType logType = BizLogType.Basic_Product_Price_PMRequestMemo;
                if (item.RequestStatus == ProductPriceRequestStatus.Approved)
                {
                    operationNote = "审核通过：";
                    logType = BizLogType.Basic_Product_Price_Update_VerifyOK;
                }
                else if (item.RequestStatus == ProductPriceRequestStatus.Deny)
                {
                    operationNote = "审核未通过：";
                    logType = BizLogType.Basic_Product_Price_Update_VerifyCancel;
                }
                if (!string.IsNullOrEmpty(operationNote))
                {
                    var productProcessor = ObjectFactory<ProductProcessor>.Instance;
                    if (productProcessor != null)
                    {
                        var entity = productProcessor.GetProductInfo(_productSysNo);
                        operationNote = operationNote + GetLogContext(entity);
                    }
                  
                    LogEntity.Push(operationNote, logType, _productSysNo, "8601");
                }
            }

        }

        /// <summary>
        /// 获取价格日志描述
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetLogContext(ProductInfo entity)
        {
            if (entity == null) return "";
            if (entity.OperateUser == null)
            {
                entity.OperateUser = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo);
            }
            string logContent = string.Format("ProductSysNo-{0},BasicPrice-{1},CurrentPrice-{2},CashRebate-{3},Point-{4},AuditTime-{5},CreateUserSysNo-{6}",
             _productSysNo, entity.ProductPriceInfo.BasicPrice, entity.ProductPriceInfo.CurrentPrice, entity.ProductPriceInfo.CashRebate, entity.ProductPriceInfo.Point, DateTime.Now.ToString(), entity.OperateUser.UserDisplayName);
            return logContent;
        }
    }

    /// <summary>
    /// 商品价格审核日志
    /// </summary>
    [VersionExport(typeof(AuditRequestProductPriceLog))]
    public class AuditRequestProductPriceLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            AuditRequestProductPriceInfoLog();
        }

        /// <summary>
        /// 商品价格提交审核的日志
        /// </summary>
        private void AuditRequestProductPriceInfoLog()
        {
            if (ParameterValue != null && ParameterValue is ProductInfo)
            {
                var item = (ProductInfo)ParameterValue;
                string logContent = string.Format("ProductSysNo-{0},BasicPrice-{1},CurrentPrice-{2},CashRebate-{3},Point-{4},CreateTime-{5},CreateUserSysNo-{6}",
               item.SysNo, item.ProductPriceRequest.BasicPrice, item.ProductPriceRequest.CurrentPrice, item.ProductPriceRequest.CashRebate, item.ProductPriceRequest.Point, DateTime.Now.ToString(), item.ProductPriceRequest.CreateUser.UserDisplayName);
                string operationNote = "提交审核:" + logContent;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_Price_Update_Verify, item.SysNo, "8601");
                operationNote = "PM申请：" + item.ProductPriceRequest.PMMemo;
                LogEntity.Push(operationNote, BizLogType.Basic_Product_Price_Update_Verify, item.SysNo, "8601");
            }
        }
    }

    /// <summary>
    /// 商品价格撤销审核日志
    /// </summary>
    [VersionExport(typeof(CancelAuditProductPriceRequestLog))]
    public class CancelAuditProductPriceRequestLog : MethodLog
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        public override void AddLogEntityList()
        {
            CancelAuditProductPriceRequesInfotLog();
        }

        /// <summary>
        /// 商品价格提交审核的日志
        /// </summary>
        private void CancelAuditProductPriceRequesInfotLog()
        {
            if (ParameterValue != null && ParameterValue is ProductInfo)
            {
                var item = (ProductInfo)ParameterValue;
                var productProcessor = ObjectFactory<ProductProcessor>.Instance;
                if (productProcessor != null)
                {
                    item = productProcessor.GetProductInfo(item.SysNo);
                    if (item != null)
                    {
                        if (item.OperateUser==null)
                        {
                             item.OperateUser = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo);
                        }
                        string logContent = string.Format("ProductSysNo-{0},BasicPrice-{1},CurrentPrice-{2},CashRebate-{3},Point-{4},CreateTime-{5},CreateUserSysNo-{6}",
                                             item.SysNo, item.ProductPriceInfo.BasicPrice, item.ProductPriceInfo.CurrentPrice, item.ProductPriceInfo.CashRebate, item.ProductPriceInfo.Point, DateTime.Now.ToString(), item.OperateUser.UserDisplayName);
                        string operationNote = "撤销审核:" + logContent;
                        LogEntity.Push(operationNote, BizLogType.Basic_Product_Price_Update_Verify, item.SysNo, "8601");
                    }
                }

            }
        }
    }
}
