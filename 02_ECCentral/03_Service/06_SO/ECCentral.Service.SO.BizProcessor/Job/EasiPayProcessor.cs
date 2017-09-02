using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ICSharpCode.SharpZipLib.BZip2;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(EasiPayProcessor))]
    public class EasiPayProcessor
    {
        private static string s_EasiPayID = AppSettingManager.GetSetting("SO", "EasiPayDeclarePaymentConfigEasiPayID");
        private ISODA SODA = ObjectFactory<ISODA>.Instance;

        /// <summary>
        /// 获取待申报的订单
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareSO> GetWaitDeclareSO()
        {
            return SODA.GetWaitDeclareSO();
        }

        /// <summary>
        /// 申报订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool DeclareSO(WaitDeclareSO entity)
        {
            bool result = false;

            #region 0.先进行支付申报，申报成功再执行订单申报
            if (!this.DeclarePayment(entity))
            {
                return result;
            }
            #endregion

            #region 1.单笔值计算
            DeclareOrderInfo declareOrderInfo = SODA.DeclareGetOrderInfoBySOSysNo(entity.SOSysNo);
            VendorCustomsInfo customsInfo = SODA.LoadVendorCustomsInfo(entity.SOSysNo);
            PayType payType = ObjectFactory<ICommonBizInteract>.Instance.GetPayTypeByID(declareOrderInfo.PayTypeID);
            if (declareOrderInfo == null || declareOrderInfo.SOItemList == null || declareOrderInfo.SOItemList.Count == 0)
                return result;
            decimal allCargoTotalPrice = 0m;
            string cargoDescript = "";
            decimal totalTariffAmount = 0m;
            decimal otherPrice = Math.Abs(declareOrderInfo.Amount.PrepayAmt) * -1;
            List<EasiPayRequestDeclareOrderCargoes> cargoesList = new List<EasiPayRequestDeclareOrderCargoes>();

            foreach (var item in declareOrderInfo.SOItemList)
            {
                cargoDescript += (string.IsNullOrEmpty(cargoDescript) ? "" : "；") + (item.ProductName.Replace("#", "").Replace("%", "").Replace("&", "").Replace("+", "") + "描述");
                totalTariffAmount += item.TariffAmt * item.Quantity;
                item.ProductName = item.ProductName.Replace("#", "").Replace("%", "").Replace("&", "").Replace("+", "");
                //折扣除不尽时，把多余的作为OtherPrice上送
                decimal currOtherPrice = Math.Abs(item.DiscountAmt) % item.Quantity;
                otherPrice += currOtherPrice * -1;
                //Item上是商品本身的价格，需要排除折扣
                decimal unitPrice = item.OriginalPrice - ((Math.Abs(item.DiscountAmt) - currOtherPrice) / item.Quantity);
                unitPrice = decimal.Parse(unitPrice.ToString("F2"));
                otherPrice += Math.Abs(item.PromotionDiscount * item.Quantity) * -1;
                allCargoTotalPrice += unitPrice * item.Quantity;
                cargoesList.Add(new EasiPayRequestDeclareOrderCargoes()
                {
                    cargoName = item.ProductName,
                    cargoCode = item.EntryCode,
                    cargoNum = item.Quantity,
                    cargoUnitPrice = decimal.Parse(unitPrice.ToString("F2")),
                    cargoTotalPrice = decimal.Parse((unitPrice * item.Quantity).ToString("F2")),
                    cargoTotalTax = decimal.Parse((item.TariffAmt * item.Quantity).ToString("F2"))
                });
            }
            //积分支付作为其他金额报关
            otherPrice += Math.Abs(declareOrderInfo.Amount.PointPay * 1.00m / 100.00m) * -1;
            #endregion

            #region 2.构造请求业务数据
            int serialNumber = SODA.CreateSODeclareRecordsSysNo();
            EasiPayRequestDeclareOrder requestInfo = new EasiPayRequestDeclareOrder();
            requestInfo.version = AppSettingManager.GetSetting("SO", "EasiPayDeclareOrderConfigVersion");
            requestInfo.commitTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            requestInfo.coName = customsInfo.CBTMerchantName;
            requestInfo.coCode = customsInfo.CBTSRC_NCode;
            requestInfo.MerchantSecretKey = customsInfo.CBTSODeclareSecretKey;
            requestInfo.serialNumber = serialNumber.ToString();
            requestInfo.merchantOrderId = entity.SOSysNo.ToString();
            requestInfo.assBillNo = entity.TrackingNumber;
            requestInfo.orderCommitTime = declareOrderInfo.OrderDate.ToString("yyyyMMddHHmmss");
            //发件信息，从Appsetting配置中取(改为从仓库表中读取 by cavin 2014-12-02)
            //仓库编号
            //string warehouseNumber = declareOrderInfo.SOItemList[0].WarehouseNumber.Trim();
            requestInfo.senderName = declareOrderInfo.senderName;//AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderName", warehouseNumber));
            requestInfo.senderTel = declareOrderInfo.senderTel;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderTel", warehouseNumber));
            requestInfo.senderCompanyName = declareOrderInfo.senderCompanyName;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderCompanyName", warehouseNumber));
            requestInfo.senderAddr = declareOrderInfo.senderAddr;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderAddr", warehouseNumber));
            requestInfo.senderZip = declareOrderInfo.senderZip;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderZip", warehouseNumber));
            requestInfo.senderCity = declareOrderInfo.senderCity;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderCity", warehouseNumber));
            requestInfo.senderProvince = declareOrderInfo.senderProvince;// AppSettingManager.GetSetting("SO", string.Format("EasiPayDeclareOrderConfig_Sender_{0}_SenderProvince", warehouseNumber));
            //发件地国家，需要适配三位国家代码
            requestInfo.senderCountry = declareOrderInfo.SOItemList.FirstOrDefault().CountryCode;
            requestInfo.cargoDescript = string.IsNullOrEmpty(cargoDescript) ? "无商品信息简述" : cargoDescript;
            requestInfo.allCargoTotalPrice = decimal.Parse(allCargoTotalPrice.ToString("F2"));
            requestInfo.allCargoTotalTax = decimal.Parse(totalTariffAmount.ToString("F2"));
            requestInfo.expressPrice = decimal.Parse(declareOrderInfo.Amount.ShipPrice.ToString("F2"));
            requestInfo.otherPrice = decimal.Parse(otherPrice.ToString("F2"));
            requestInfo.recPerson = string.IsNullOrWhiteSpace(declareOrderInfo.ReceiveName) ? "无" : declareOrderInfo.ReceiveName;
            requestInfo.recPhone = string.IsNullOrWhiteSpace(declareOrderInfo.ReceiveCellPhone) ? "无" : declareOrderInfo.ReceiveCellPhone;
            requestInfo.recCountry = "中国";
            requestInfo.recProvince = string.IsNullOrWhiteSpace(declareOrderInfo.ReceiveProvinceName) ? "无" : declareOrderInfo.ReceiveProvinceName;
            requestInfo.recCity = string.IsNullOrWhiteSpace(declareOrderInfo.ReceiveCityName) ? "无" : declareOrderInfo.ReceiveCityName;
            requestInfo.recAddress = string.IsNullOrWhiteSpace(declareOrderInfo.ReceiveAddress) ? "无" : declareOrderInfo.ReceiveAddress;
            //为空
            requestInfo.recZip = "";
            requestInfo.serverType = declareOrderInfo.StockType == BizEntity.Common.TradeType.FTA ? "S02" : "S01";
            requestInfo.custCode = declareOrderInfo.CustomsCode;
            requestInfo.operationCode = AppSettingManager.GetSetting("SO", "EasiPayDeclareOrderConfigOperationCode");
            //为空
            requestInfo.spt = "";
            requestInfo.cargoes = cargoesList;
            requestInfo.payMethod = AppSettingManager.GetSetting("SO", "EasiPayDeclareOrderConfigPayMethod");
            requestInfo.payMerchantName = payType.CBTMerchantName;
            requestInfo.payMerchantCode = payType.CBTMerchantCode;
            //支付总金额=全部商品合计总价+税费+物流运费+其他金额（其他金额为负数）-余额支付金额
            requestInfo.payAmount = decimal.Parse((allCargoTotalPrice + totalTariffAmount + declareOrderInfo.Amount.ShipPrice + otherPrice).ToString("F2"));
            requestInfo.payCUR = customsInfo.PayCurrencyCode;
            requestInfo.payID = string.IsNullOrWhiteSpace(declareOrderInfo.PayInfo.SerialNumber) ? "" : declareOrderInfo.PayInfo.SerialNumber.Substring(1).Trim();//因为支付有个前缀P
            requestInfo.payTime = declareOrderInfo.PayInfo.PayProcessTime.Trim();
            #endregion

            #region 3.处理请求，解析请求结果
            SOLogInfo soLogInfo = new SOLogInfo()
            {
                UserSysNo = 3025,
                IP = "Delcare SO Job",
                OperationType = ECCentral.BizEntity.Common.BizLogType.Sale_SO_Update,
                SOSysNo = entity.SOSysNo,
                Note = "",
                CompanyCode = "8601"
            };
            string reqContent = BuildPostReqData(requestInfo, requestInfo.MerchantSecretKey);
            string strRequestResult = HttpPostRequest(AppSettingManager.GetSetting("SO", "EasiPayDeclareOrderConfigRequestUrl"), reqContent);
            strRequestResult = CommonUtility.HttpUrlDecode(strRequestResult);
            EasiPayRequestResult requestResult = SerializationUtility.JsonDeserialize<EasiPayRequestResult>(strRequestResult);
            if (requestResult.status.ToLower().Equals("success"))
            {
                result = true;
                soLogInfo.Note = "订单报关申报提交成功";
            }
            else
            {
                soLogInfo.Note = string.Format("订单报关申报提交失败：{0}", requestResult.errorMsg);
            }
            ObjectFactory<ISOLogDA>.Instance.InsertSOLog(soLogInfo);
            #region 写系统Log
            ApplicationEventLog log = new ApplicationEventLog()
            {
                Source = "JOB SODeclare",
                ReferenceIP = ServiceContext.Current.ClientIP,
                EventType = 8,
                HostName = "JOB",
                EventTitle = "SODeclare Request",
                EventMessage = reqContent,
                LanguageCode = "zh-CN",
                CompanyCode = "8601",
                StoreCompanyCode = "8601",
                EventDetail = soLogInfo.Note,
            };
            ObjectFactory<ICommonBizInteract>.Instance.CreateApplicationEventLog(log);
            #endregion
            #endregion

            #region 4.提交申报申请成功，创建申报记录
            if (result)
            {
                SODeclareRecords declareRecord = new SODeclareRecords()
                {
                    SysNo = serialNumber,
                    SOSysNo = entity.SOSysNo,
                    TrackingNumber = entity.TrackingNumber
                };
                SODA.CreateSODeclareRecords(declareRecord);
            }
            #endregion

            return result;
        }

        #region 支付申报
        /// <summary>
        /// 支付申报
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true申报成功，false申报失败</returns>
        public bool DeclarePayment(WaitDeclareSO entity)
        {
            bool result = false;
            #region 获取支付信息
            SOPaymentDeclare data = SODA.DeclareGetPaymentInfoBySOSysNo(entity.SOSysNo);
            #endregion
            //检查是否需要进行支付申报，东方支付不需要进行支付申报
            if (data.PayTypeID == s_EasiPayID)
            {
                //东方支付不需要传，直接更新支付申报状态为申报成功
                SODA.DeclareUpdatePaymentDeclareInfo(entity.SOSysNo, null, 1);
                data = SODA.DeclareGetPaymentInfoBySOSysNo(entity.SOSysNo);
            }
            if (data.DeclareStatus.HasValue && data.DeclareStatus.Value == 1)
            {
                result = true;
            }
            else
            {
                #region 构造请求参数
                VendorCustomsInfo customsInfo = SODA.LoadVendorCustomsInfo(entity.SOSysNo);
                PayType payType = ObjectFactory<ICommonBizInteract>.Instance.GetPayType(data.PayTypeSysNo);
                EasiPayRequestDeclarePayment declarePayment = EntityConverter<SOPaymentDeclare, EasiPayRequestDeclarePayment>.Convert(data);
                declarePayment.Version = "v1.0";
                declarePayment.CommitTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                declarePayment.CoCode = payType.CBTMerchantCode;
                declarePayment.CoName = payType.CBTMerchantName;
                declarePayment.MerchantCode = customsInfo.CBTMerchantCode;
                declarePayment.MerchantName = customsInfo.CBTMerchantName;
                declarePayment.SerialNumber = this.CreateSerialNumber(declarePayment.CoCode);
                declarePayment.IdentifyType = data.IDCardType == 0 ? "30" : "";//固定值30 身份证
                declarePayment.Birthday = data.Birthday.ToString("yyyyMMdd");
                declarePayment.Sex = data.Gender == 0 ? "F" : data.Gender == 1 ? "M" : "";
                declarePayment.PayID = data.PayTransNumber.Substring(1).Trim();//因为支付有个前缀P
                #endregion
                #region 处理请求，解析请求结果
                string note = string.Empty;
                string reqContent = BuildPostReqData(declarePayment, payType.CBTSODeclarePaymentSecretKey);
                string strRequestResult = HttpPostRequest(AppSettingManager.GetSetting("SO", "EasiPayDeclarePaymentConfigRequestUrl"), reqContent);
                strRequestResult = CommonUtility.HttpUrlDecode(strRequestResult);
                EasiPayRequestResult requestResult = SerializationUtility.JsonDeserialize<EasiPayRequestResult>(strRequestResult);
                if (requestResult.status.ToLower().Equals("success"))
                {
                    result = true;
                    note = "支付报关申请成功";
                }
                else
                {
                    note = "支付报关申请失败:" + requestResult.msg;
                }
                #region 写系统Log
                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "JOB SODeclarePayment",
                    ReferenceIP = ServiceContext.Current.ClientIP,
                    EventType = 9,
                    HostName = "JOB",
                    EventTitle = "SODeclarePayment Request",
                    EventMessage = reqContent,
                    LanguageCode = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601",
                    EventDetail = note,
                };
                ObjectFactory<ICommonBizInteract>.Instance.CreateApplicationEventLog(log);
                #endregion
                #endregion
                #region 更新支付状态
                SODA.DeclareUpdatePaymentDeclareInfo(entity.SOSysNo, declarePayment.SerialNumber, result ? 1 : 0);
                #endregion
            }
            return result;
        }
        #endregion

        #region 商检商品
        /// <summary>
        /// 获取商检状态为审核通过的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetEntryAuditSucess()
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProduct(ProductEntryStatus.AuditSucess, null);
        }
        /// <summary>
        /// 标记商品商检状态为商检中
        /// </summary>
        /// <param name="products"></param>
        public void MarkInInspection(List<WaitDeclareProduct> products)
        {
            //检查数据是否满足要求
            if (products.Count > 10)
            {
                throw new BizException("不能超过10个商品");
            }
            ObjectFactory<IIMBizInteract>.Instance.ProductBatchEntry(products.Select(t => t.ProductSysNo).ToList(), "", ProductEntryStatus.Entry, ProductEntryStatusEx.Inspection);
        }
        /// <summary>
        /// 获取商品商检状态为商检中的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetInInspectionProduct()
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProduct(ProductEntryStatus.Entry, ProductEntryStatusEx.Inspection);
        }
        /// <summary>
        /// 标记商品商检状态为商检通过，待报关
        /// </summary>
        /// <param name="products"></param>
        public void MarkSuccessInspection(List<WaitDeclareProduct> products)
        {
            //检查数据是否满足要求
            if (products.Count > 10)
            {
                throw new BizException("不能超过10个商品");
            }
            ObjectFactory<IIMBizInteract>.Instance.ProductBatchEntry(products.Select(t => t.ProductSysNo).ToList(), "", ProductEntryStatus.Entry, ProductEntryStatusEx.InspectionSucess);
        }
        #endregion

        #region 申报商品
        /// <summary>
        /// 获取待申报的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetWaitDeclareProduct()
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProduct(ProductEntryStatus.Entry, ProductEntryStatusEx.InspectionSucess);
        }
        /// <summary>
        /// 申报商品
        /// </summary>
        /// <param name="decalreList">待申报商品列表，必须是同一个商家，不能超过10个商品</param>
        /// <returns></returns>
        public DeclareProductResult DeclareProduct(List<WaitDeclareProduct> decalreList)
        {
            DeclareProductResult result = new DeclareProductResult();
            //检查是否有数据
            if (decalreList == null || decalreList.Count <= 0)
            {
                return result;
            }
            var group = decalreList.GroupBy(d => d.MerchantSysNo);
            //检查数据是否满足要求
            if (group.Count() > 1 || decalreList.Count > 10)
            {
                throw new BizException("待申报商品列表，必须是同一个商家，不能超过10个商品");
            }
            //获取商家取关务对接相关信息
            VendorCustomsInfo customsInfo = SODA.LoadVendorCustomsInfoByMerchant(decalreList.FirstOrDefault().MerchantSysNo);
            //获取待申报商品详细信息
            List<ProductDeclare> dataList = ObjectFactory<IIMBizInteract>.Instance.DeclareGetProduct(decalreList);
            #region 构造请求参数
            EasiPayRequestDeclareProduct requestInfo = new EasiPayRequestDeclareProduct();
            requestInfo.version = AppSettingManager.GetSetting("SO", "EasiPayDeclareProductConfigVersion");
            requestInfo.commitTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            requestInfo.coName = customsInfo.CBTMerchantName;
            requestInfo.coCode = customsInfo.CBTSRC_NCode;
            requestInfo.serialNumber = this.CreateSerialNumber(requestInfo.coCode);
            requestInfo.Cargoes = EntityConverter<ProductDeclare, EasiPayRequestDeclareProductDetail>.Convert(dataList, (s, t) =>
            {
                t.cargoCode = s.ProductID;
                t.cargoBrand = s.BrandName;
                t.cargoNameCh = s.ProductName;
                t.cargoNameEh = s.ProductNameEN;
                t.cargoModel = s.ProductMode;
                t.cargoSpec = s.Specifications;
                t.cargoPlace = s.CountryName;
                t.cargoFunction = s.Functions;
                t.cargoPurpose = s.Purpose;
                t.cargoIngredient = s.Component;
                t.cargoFactoryDate = s.ManufactureDate.ToString("yyyyMMdd");
                t.cargoUnit = s.TaxUnit;
                t.cargoUnitNum = s.TaxQty.ToString();
                t.cargoPrice = s.CurrentPrice.ToString("F2");
                t.cargoGrossWT = s.GrossWeight;
                t.cargoNetWT = s.SuttleWeight;
                t.serverType = s.BizType == TradeType.FTA ? "S02" : "S01";
                t.customsCode = s.CustomsCode;
                t.copGNo = s.ProductSKUNO;
                t.materialID = s.SuppliesSerialNo;
                t.beianUnit = s.ApplyUnit;
                t.beianQty = s.ApplyQty.ToString();
            });
            requestInfo.operationCode = "1";
            requestInfo.spt = "";
            #endregion
            #region 3.处理请求，解析请求结果
            string reqContent = BuildPostReqData(requestInfo, customsInfo.CBTProductDeclareSecretKey);
            string note = string.Empty;
            string strRequestResult = HttpPostRequest(AppSettingManager.GetSetting("SO", "EasiPayDeclareProductConfigRequestUrl"), reqContent);
            strRequestResult = CommonUtility.HttpUrlDecode(strRequestResult);
            EasiPayRequestResult requestResult = SerializationUtility.JsonDeserialize<EasiPayRequestResult>(strRequestResult);
            if (requestResult.status.ToLower().Equals("success"))
            {
                result.Success = true;
                try
                {
                    ObjectFactory<IIMBizInteract>.Instance.ProductBatchEntry(decalreList.Select(t => t.ProductSysNo).ToList(), "", ProductEntryStatus.Entry, ProductEntryStatusEx.Customs);
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
                note = "商品报关申请成功";
            }
            else
            {
                result.Message = requestResult.msg;
                note = "商品报关申请失败:" + requestResult.msg;
            }
            #region 写系统Log
            ApplicationEventLog log = new ApplicationEventLog()
            {
                Source = "JOB ProductDeclare",
                ReferenceIP = ServiceContext.Current.ClientIP,
                EventType = 8,
                HostName = "JOB",
                EventTitle = "ProductDeclare Request",
                EventMessage = reqContent,
                LanguageCode = "zh-CN",
                CompanyCode = "8601",
                StoreCompanyCode = "8601",
                EventDetail = note
            };
            ObjectFactory<ICommonBizInteract>.Instance.CreateApplicationEventLog(log);
            #endregion
            #endregion
            return result;
        }
        /// <summary>
        /// 申报商品结果处理
        /// </summary>
        /// <param name="callbackString"></param>
        /// <returns></returns>
        public bool DeclareProductCallBack(string callbackString)
        {
            List<EasiPayProductDeclareBackItemInfo> itemList = SerializationUtility.JsonDeserialize<List<EasiPayProductDeclareBackItemInfo>>(callbackString);
            foreach (var item in itemList)
            {
                if (item.status == "1")
                {
                    //商品申报成功
                    ObjectFactory<IIMBizInteract>.Instance.ProductCustomsSuccess(new ProductEntryInfo()
                    {
                        ProductID = item.cargoCode,
                        EntryCode = item.cargoCode,
                        TariffCode = item.cargoCodeTS,
                        TariffRate = item.cargoRate,
                        AuditNote = item.statusMsg,
                    });
                }
                else if (item.status == "2")
                {
                    //商品申报失败
                    ObjectFactory<IIMBizInteract>.Instance.ProductCustomsFail(new ProductEntryInfo()
                    {
                        ProductID = item.cargoCode,
                        AuditNote = item.statusMsg,
                    });
                }
                else
                {
                    throw new BizException("非法审核状态");
                }
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 生成唯一流水号
        /// </summary>
        /// <param name="merchantCode">CBT平台分配的唯一企业代码（7位字符）</param>
        /// <returns></returns>
        private string CreateSerialNumber(string merchantCode)
        {
            return merchantCode + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000, 99999);
        }

        /// <summary>
        /// 构造请求的协议数据
        /// </summary>
        /// <param name="reqXmlValue"></param>
        /// <returns></returns>
        private string BuildPostReqData(object requestInfo, string secretKey)
        {
            string requestValues = SerializationUtility.JsonSerialize(requestInfo);
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("EData={0}", requestValues);
            postData.AppendFormat("&SignMsg={0}", SignData(requestValues, secretKey));
            return postData.ToString();
        }
        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="reqValue">请求的业务值</param>
        /// <returns>签名数据</returns>
        private string SignData(string reqValue, string secretKey)
        {
            string sourceSignValue = "{0}{1}";
            sourceSignValue = string.Format(sourceSignValue, reqValue, secretKey);
            return Hash_MD5.GetMD5(sourceSignValue).ToUpper();
        }

        #region HTTP POST DATA

        private static string HttpPostRequest(string url, string reqContent)
        {
            string encoding = "UTF-8";
            HttpWebResponse response = CreatePostHttpResponse(url, reqContent, encoding, null);
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding)))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>  
        /// 创建POST方式的HTTP请求
        /// </summary>  
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        private static HttpWebResponse CreatePostHttpResponse(string url, string reqContent, string encoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (string.IsNullOrWhiteSpace(encoding))
                encoding = "UTF-8";
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //如果是发送HTTPS请求  
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Timeout = 600000;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            if (!string.IsNullOrWhiteSpace(reqContent))
            {
                byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(reqContent);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //总是接受  
            return true;
        }

        #endregion
    }

    #region 接口对象类

    /// <summary>
    /// 请求结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestResult
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string errorCode { get; set; }
        [DataMember]
        public string errorMsg { get; set; }
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string msg { get; set; }
    }

    /// <summary>
    /// 协议参数
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequest
    {
        /// <summary>
        /// 网关版本
        /// </summary>
        [DataMember]
        public string version { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public string commitTime { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [DataMember]
        public string coName { get; set; }
        /// <summary>
        /// 企业代码
        /// </summary>
        [DataMember]
        public string coCode { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [DataMember]
        public string serialNumber { get; set; }
    }

    /// <summary>
    /// 申报订单
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestDeclareOrder : EasiPayRequest
    {
        [DataMember]
        public string merchantOrderId { get; set; }
        [DataMember]
        public string assBillNo { get; set; }
        [DataMember]
        public string orderCommitTime { get; set; }
        [DataMember]
        public string senderName { get; set; }
        [DataMember]
        public string senderTel { get; set; }
        [DataMember]
        public string senderCompanyName { get; set; }
        [DataMember]
        public string senderAddr { get; set; }
        [DataMember]
        public string senderZip { get; set; }
        [DataMember]
        public string senderCity { get; set; }
        [DataMember]
        public string senderProvince { get; set; }
        [DataMember]
        public string senderCountry { get; set; }
        [DataMember]
        public string cargoDescript { get; set; }
        [DataMember]
        public decimal allCargoTotalPrice { get; set; }
        [DataMember]
        public decimal allCargoTotalTax { get; set; }
        [DataMember]
        public decimal expressPrice { get; set; }
        [DataMember]
        public decimal otherPrice { get; set; }
        [DataMember]
        public string recPerson { get; set; }
        [DataMember]
        public string recPhone { get; set; }
        [DataMember]
        public string recCountry { get; set; }
        [DataMember]
        public string recProvince { get; set; }
        [DataMember]
        public string recCity { get; set; }
        [DataMember]
        public string recAddress { get; set; }
        [DataMember]
        public string recZip { get; set; }
        [DataMember]
        public string serverType { get; set; }
        [DataMember]
        public string custCode { get; set; }
        [DataMember]
        public string operationCode { get; set; }
        [DataMember]
        public string spt { get; set; }
        [DataMember]
        public List<EasiPayRequestDeclareOrderCargoes> cargoes { get; set; }
        [DataMember]
        public string payMethod { get; set; }
        [DataMember]
        public string payMerchantName { get; set; }
        [DataMember]
        public string payMerchantCode { get; set; }
        [DataMember]
        public decimal payAmount { get; set; }
        [DataMember]
        public string payCUR { get; set; }
        [DataMember]
        public string payID { get; set; }
        [DataMember]
        public string payTime { get; set; }
        /// <summary>
        /// 签名SecretKey 不传输此字段
        /// </summary>
        public string MerchantSecretKey { get; set; }
    }
    /// <summary>
    /// 申报订单明细
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestDeclareOrderCargoes
    {
        [DataMember]
        public string cargoName { get; set; }
        [DataMember]
        public string cargoCode { get; set; }
        [DataMember]
        public int cargoNum { get; set; }
        [DataMember]
        public decimal cargoUnitPrice { get; set; }
        [DataMember]
        public decimal cargoTotalPrice { get; set; }
        [DataMember]
        public decimal cargoTotalTax { get; set; }
    }

    /// <summary>
    /// 申报支付
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestDeclarePayment
    {
        #region 协议参数
        /// <summary>
        /// 网关版本
        /// </summary>
        [DataMember]
        public string Version { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public string CommitTime { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [DataMember]
        public string CoName { get; set; }
        /// <summary>
        /// 企业代码
        /// </summary>
        [DataMember]
        public string CoCode { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [DataMember]
        public string SerialNumber { get; set; }
        #endregion
        #region 业务参数：支付主体信息
        /// <summary>
        /// 商户订单号
        /// </summary>
        [DataMember]
        public string MerchantOrderId { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        [DataMember]
        public string MerchantName { get; set; }
        /// <summary>
        /// 商户编号
        /// </summary>
        [DataMember]
        public string MerchantCode { get; set; }
        /// <summary>
        /// 支付总金额
        /// </summary>
        [DataMember]
        public string PayAmount { get; set; }
        /// <summary>
        /// 付款币种
        /// </summary>
        [DataMember]
        public string PayCUR { get; set; }
        /// <summary>
        /// 支付交易号
        /// </summary>
        [DataMember]
        public string PayID { get; set; }
        /// <summary>
        /// 支付交易时间
        /// </summary>
        [DataMember]
        public string PayTime { get; set; }
        /// <summary>
        /// 付款银行名称
        /// </summary>
        [DataMember]
        public string BankPayName { get; set; }
        /// <summary>
        /// 付款银行编号
        /// </summary>
        [DataMember]
        public string BankPayCode { get; set; }
        /// <summary>
        /// 付款银行交易号
        /// </summary>
        [DataMember]
        public string BankSerialNo { get; set; }
        #endregion
        #region 业务参数：个人实名信息
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string PersonName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        [DataMember]
        public string IdentifyType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [DataMember]
        public string IdentifyCode { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }
        /// <summary>
        /// 所属国家代码
        /// </summary>
        [DataMember]
        public string CountryCode { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public string Sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [DataMember]
        public string Birthday { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        [DataMember]
        public string Spt { get; set; }
        #endregion
    }
    /// <summary>
    /// 申报商品
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestDeclareProduct : EasiPayRequest
    {
        /// <summary>
        /// 业务参数：Cargoes集合信息，一次最多10条商品信息
        /// </summary>
        [DataMember]
        public List<EasiPayRequestDeclareProductDetail> Cargoes { get; set; }
        /// <summary>
        /// 操作编码
        /// </summary>
        [DataMember]
        public string operationCode { get; set; }
        /// <summary>
        /// spt
        /// </summary>
        [DataMember]
        public string spt { get; set; }
    }
    /// <summary>
    /// 申报商品明细
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayRequestDeclareProductDetail
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string cargoCode { get; set; }
        /// <summary>
        /// 品牌（中文）
        /// </summary>
        [DataMember]
        public string cargoBrand { get; set; }
        /// <summary>
        /// 商品名称（中文）
        /// </summary>
        [DataMember]
        public string cargoNameCh { get; set; }
        /// <summary>
        /// 商品名称（英文）
        /// </summary>
        [DataMember]
        public string cargoNameEh { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        [DataMember]
        public string cargoModel { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [DataMember]
        public string cargoSpec { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        [DataMember]
        public string cargoPlace { get; set; }
        /// <summary>
        /// 功能
        /// </summary>
        [DataMember]
        public string cargoFunction { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        [DataMember]
        public string cargoPurpose { get; set; }
        /// <summary>
        /// 成份
        /// </summary>
        [DataMember]
        public string cargoIngredient { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        [DataMember]
        public string cargoFactoryDate { get; set; }
        /// <summary>
        /// 单位（计税单位）
        /// </summary>
        [DataMember]
        public string cargoUnit { get; set; }
        /// <summary>
        /// 单位数量
        /// </summary>
        [DataMember]
        public string cargoUnitNum { get; set; }
        /// <summary>
        /// 单价（人民币）
        /// </summary>
        [DataMember]
        public string cargoPrice { get; set; }
        /// <summary>
        /// 毛重（KG）
        /// </summary>
        [DataMember]
        public string cargoGrossWT { get; set; }
        /// <summary>
        /// 净重（KG）
        /// </summary>
        [DataMember]
        public string cargoNetWT { get; set; }
        /// <summary>
        /// 贸易类型
        /// </summary>
        [DataMember]
        public string serverType { get; set; }
        /// <summary>
        /// 货号
        /// </summary>
        [DataMember]
        public string copGNo { get; set; }
        /// <summary>
        /// 物资序号
        /// </summary>
        [DataMember]
        public string materialID { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        [DataMember]
        public string beianUnit { get; set; }
        /// <summary>
        /// 申报数量
        /// </summary>
        [DataMember]
        public string beianQty { get; set; }
        /// <summary>
        /// 关区代码
        /// </summary>
        [DataMember]
        public string customsCode { get; set; }
    }
    /// <summary>
    /// 商品备案结果详细信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class EasiPayProductDeclareBackItemInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string cargoCode { get; set; }
        /// <summary>
        /// 申报备案号
        /// </summary>
        [DataMember]
        public string declaraNo { get; set; }
        /// <summary>
        /// 税则号
        /// </summary>
        [DataMember]
        public string cargoCodeTS { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal cargoRate { get; set; }
        /// <summary>
        /// 审核状态
        /// 1：成功
        /// 2：失败
        /// </summary>
        [DataMember]
        public string status { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        [DataMember]
        public string statusMsg { get; set; }
        [DataMember]
        public string effectStartTime { get; set; }
        [DataMember]
        public string effectEndTime { get; set; }
    }
    #endregion
}
