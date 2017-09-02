//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动单据
// 子系统名		        商品价格变动单据逻辑实现
// 作成者				Tom
// 改版日				2012.4.26
// 改版内容				新建
//************************************************************************
using System;
using System.Data;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductPriceRequestProcessor))]
    public class ProductPriceRequestProcessor
    {
        private readonly IProductPriceRequestDA _productPriceRequestDA = ObjectFactory<IProductPriceRequestDA>.Instance;
        /// <summary>
        /// 待审核状态
        /// </summary>
        private static readonly ProductPriceRequestStatus[] PendingList = new[] { ProductPriceRequestStatus.Origin, ProductPriceRequestStatus.NeedSeniorApprove };

        private static readonly PromotionType[] PromotionTypeList = new[] { PromotionType.SaleGift, PromotionType.Coupons };

        /// <summary>
        /// 修改商品价格变动状态
        /// </summary>
        /// <param name="productPriceRequest"></param>
        [ProductInfoChange]
        //[SendEmailAttribute(SendEmailType.ApprovePriceRequest)]
        public void AuditProductPriceRequest(ProductPriceRequestInfo productPriceRequest)
        {
            CheckProductPriceRequestProcessor.CheckProductPriceReques(productPriceRequest);
            CheckProductPriceRequestProcessor.CheckProductPriceRequestSysNo(productPriceRequest.SysNo);
            CheckProductPriceRequestProcessor.CheckProductPriceRequestStatus(productPriceRequest.RequestStatus);
            var status = productPriceRequest.RequestStatus;
            var tlMemo = productPriceRequest.TLMemo ?? "";
            var pmdMemo = productPriceRequest.PMDMemo ?? "";
            var hasAdvancedAuditPricePermission = productPriceRequest.HasAdvancedAuditPricePermission;
            var hasPrimaryAuditPricePermission = productPriceRequest.HasPrimaryAuditPricePermission;
            var isOnePass = productPriceRequest.IsOnePass;
            if (productPriceRequest.SysNo != null)
            {
                productPriceRequest = _productPriceRequestDA.GetProductPriceRequestInfoBySysNo(productPriceRequest.SysNo.Value);
            }
            productPriceRequest.TLMemo = tlMemo;
            productPriceRequest.PMDMemo = pmdMemo;
            productPriceRequest.HasAdvancedAuditPricePermission = hasAdvancedAuditPricePermission;
            productPriceRequest.HasPrimaryAuditPricePermission = hasPrimaryAuditPricePermission;
            var productID = "";
            int proSysNo = 0;
            CheckProductPriceRequestProcessor.CheckProductPriceRequestStatus(productPriceRequest, status, ref productID, ref proSysNo);
            if (!isOnePass)
            {
                CheckProductPriceRequestProcessor.CheckModifyStatusPermission(productPriceRequest, status, ref productID);
            }
            CheckProductPriceRequestProcessor.CheckProductPriceRequestCreateSysNo(productPriceRequest, productID);
            SetAuditUser(productPriceRequest);
            using (var tran = new TransactionScope())
            {
                productPriceRequest.RequestStatus = status;
                _productPriceRequestDA.UpdateProductPriceRequestStatus(productPriceRequest);
                #region Check当前商品调价后所在销售规则中差价
                ExternalDomainBroker.CheckComboPriceAndSetStatus(proSysNo);
                #endregion

                //状态更新成功之后发送消息
                switch (productPriceRequest.RequestStatus)
                {
                    //审核通过
                    case ProductPriceRequestStatus.Approved:
                        EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ProductPriceAuditMessage>(new ECCentral.Service.EventMessage.IM.ProductPriceAuditMessage()
                        {
                            AuditUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = productPriceRequest != null && productPriceRequest.SysNo.HasValue ? productPriceRequest.SysNo.Value : 0
                        });
                        break;
                    //审核拒绝
                    case ProductPriceRequestStatus.Deny:
                        EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ProductPriceRejectMessage>(new ECCentral.Service.EventMessage.IM.ProductPriceRejectMessage()
                        {
                            RejectUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = productPriceRequest != null && productPriceRequest.SysNo.HasValue ? productPriceRequest.SysNo.Value : 0
                        });
                        break;
                }

                tran.Complete();
            }
            //SetSendEmailAddress(proSysNo, productPriceRequest);
        }

        /// <summary>
        /// 修改商品价格变动状态
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        public ProductPriceRequestInfo GetProductPriceRequestInfoBySysNo(int auditProductPriceSysNo)
        {
            CheckProductPriceRequestProcessor.CheckProductPriceRequestSysNo(auditProductPriceSysNo);
            var result = _productPriceRequestDA.GetProductPriceRequestInfoBySysNo(auditProductPriceSysNo);
            
            if (result != null && result.Category != null && result.Category.SysNo != null)
            {
                CalcProductPrice(result);
                var productSysNo = _productPriceRequestDA.GetProductSysNoBySysNo(auditProductPriceSysNo);
                if (productSysNo > 0)
                {
                    result.Category.CategorySetting = ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingBySysNo(result.Category.SysNo.Value, productSysNo);
                    int giftsysno;
                    int couponsysno;
                    ExternalDomainBroker.GetGiftSNAndCouponSNByProductSysNo(productSysNo,out giftsysno,out couponsysno);
                    result.GiftSysNo = giftsysno;
                    result.CouponSysNo = couponsysno;
                }
            }
            return result;
        }

        /// <summary>
        /// 设置审核人和审核时间
        /// </summary>
        /// <param name="productPriceRequest"></param>
        private void SetAuditUser(ProductPriceRequestInfo productPriceRequest)
        {
            if (productPriceRequest.AuditType == ProductPriceRequestAuditType.Audit)
            {
                //有初审
                productPriceRequest.AuditTime = DateTime.Now;
                productPriceRequest.AuditUser = new UserInfo { SysNo = ServiceContext.Current.UserSysNo };
                //有终审
                productPriceRequest.FinalAuditTime = DateTime.Now;
                productPriceRequest.FinalAuditUser = new UserInfo { SysNo = ServiceContext.Current.UserSysNo };
            }
            else if (productPriceRequest.RequestStatus == ProductPriceRequestStatus.Origin)
            {
                productPriceRequest.AuditTime = DateTime.Now;
                productPriceRequest.AuditUser = new UserInfo { SysNo = ServiceContext.Current.UserSysNo };
            }
            else if (productPriceRequest.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove)
            {
                productPriceRequest.FinalAuditTime = DateTime.Now;
                productPriceRequest.FinalAuditUser = new UserInfo { SysNo = ServiceContext.Current.UserSysNo };
            }
        }


        /// <summary>
        /// 获取商品编号
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        public int GetProductSysNoByAuditProductPriceSysNo(int auditProductPriceSysNo)
        {
            if (auditProductPriceSysNo <= 0) return 0;
            var productSysNo = _productPriceRequestDA.GetProductSysNoBySysNo(auditProductPriceSysNo);
            return productSysNo;
        }


        #region 商品价格审核添加其他信息

        public virtual void AddOtherData(DataTable productPriceRequest)
        {
            if (productPriceRequest == null || productPriceRequest.Rows.Count == 0) return;
            AddotherColumns(productPriceRequest);
            var rows = (from e in productPriceRequest.AsEnumerable() select e).ToList();
            rows.ForEach(v =>
            {
                var value = MarginUtility.GetOldMarginDesc(v.Field<decimal>("NewPrice"),
                    v.Field<int?>("Point") ?? 0,
                    v.Field<decimal?>("CostPrice") ?? 0);
                v.SetField("NewMarginString", value);
                var oldValue = MarginUtility.GetOldMarginDesc(v.Field<decimal>("OldPrice"),
                   v.Field<int?>("OldPoint") ?? 0,
                   v.Field<decimal?>("CostPrice") ?? 0);
                v.SetField("MarginString", oldValue);
                var auditStatus = GetAuditStatus(v.Field<ProductPriceRequestAuditType>("AuditType"),
                                                 v.Field<ProductPriceRequestStatus>("Status"));

                v.SetField("StatusString", auditStatus);
            });
        }

        /// <summary>
        /// 获取审核状态
        /// </summary>
        /// <param name="auditType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetAuditStatus(ProductPriceRequestAuditType auditType, ProductPriceRequestStatus status)
        {
            if (auditType == ProductPriceRequestAuditType.Audit)
            {
                return ResouceManager.GetMessageString("IM.ProductPrice", "GetAuditStatus1");
            }
            if (auditType == ProductPriceRequestAuditType.SeniorAudit && status == ProductPriceRequestStatus.Origin)
            {
                return ResouceManager.GetMessageString("IM.ProductPrice", "GetAuditStatus2");
            }
            if (auditType == ProductPriceRequestAuditType.SeniorAudit && status == ProductPriceRequestStatus.NeedSeniorApprove)
            {
                return ResouceManager.GetMessageString("IM.ProductPrice", "GetAuditStatus3");
            }
            return "";
        }

        private void AddotherColumns(DataTable productPriceRequest)
        {
            productPriceRequest.Columns.Add("NewMarginString").DefaultValue = "";
            productPriceRequest.Columns.Add("MarginString").DefaultValue = "";
            productPriceRequest.Columns.Add("StatusString").DefaultValue = "";
        }


        #endregion

        #region 检查商品价格变动单据逻辑
        private static class CheckProductPriceRequestProcessor
        {
            /// <summary>
            /// 检查商品价格变动单据编号
            /// </summary>
            /// <param name="productPriceRequest"></param>
            public static void CheckProductPriceReques(ProductPriceRequestInfo productPriceRequest)
            {

                if (productPriceRequest == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestIsNull"));
                }

            }

            /// <summary>
            /// 检查商品价格变动单据编号
            /// </summary>
            /// <param name="auditProductPriceSysNo"></param>
            public static void CheckProductPriceRequestSysNo(int? auditProductPriceSysNo)
            {

                if (auditProductPriceSysNo == null || auditProductPriceSysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestSysNOIsNull"));
                }
            }


            /// <summary>
            /// 检查商品价格变动单据状态
            /// </summary>
            /// <param name="status"></param>
            public static void CheckProductPriceRequestStatus(ProductPriceRequestStatus? status)
            {

                if (status == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestStatusIsNull"));
                }
            }

            /// <summary>
            /// 检查商品价格变动单据审核单据
            /// </summary>
            /// <param name="productPriceRequest"></param>
            /// <param name="status"></param>
            /// <param name="productID"></param>
            /// <returns></returns>
            public static void CheckProductPriceRequestStatus(ProductPriceRequestInfo productPriceRequest, ProductPriceRequestStatus? status, ref string productID,ref int proSysNo)
            {
                CheckProductPriceReques(productPriceRequest);
                if (productPriceRequest.AuditType == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestTypeIsNull"));
                }
                var productPriceRequestDA = ObjectFactory<IProductPriceRequestDA>.Instance;
                if (productPriceRequest.SysNo == null) return;
                var productSysNo = productPriceRequestDA.GetProductSysNoBySysNo(productPriceRequest.SysNo.Value);
                proSysNo = productSysNo;
                if (productSysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductSysNoIsNull"));
                }
                var productDA = ObjectFactory<IProductDA>.Instance;
                var product = productDA.GetProductInfoBySysNo(productSysNo);
                productID = product.ProductID;
                if (product == null || String.IsNullOrEmpty(product.ProductID))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductIsNull"));
                }
                if (productPriceRequest.RequestStatus == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestStatusIsNull"));
                }
                if (!PendingList.Contains(productPriceRequest.RequestStatus.Value))
                {
                    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                   "AuditProductPriceRequestForPending");
                    throw new BizException(String.Format(errorStr, product.ProductID));
                }
                if (status != null && productPriceRequest.AuditType.Value == ProductPriceRequestAuditType.SeniorAudit
                                       && productPriceRequest.RequestStatus.Value == ProductPriceRequestStatus.NeedSeniorApprove
                                       && status.Value == ProductPriceRequestStatus.Origin)
                {
                    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                  "AuditProductPriceRequestForPending");
                    throw new BizException(String.Format(errorStr, product.ProductID));
                }
                if (status != null && productPriceRequest.AuditType.Value == ProductPriceRequestAuditType.Audit
                    && status.Value == ProductPriceRequestStatus.NeedSeniorApprove)
                {
                    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                  "TLAuditProductPriceRequest");
                    throw new BizException(String.Format(errorStr, product.ProductID));
                }
                if (productPriceRequest.AuditType.Value == ProductPriceRequestAuditType.SeniorAudit)
                {
                    if (String.IsNullOrEmpty(productPriceRequest.TLMemo.Trim())
                        && productPriceRequest.RequestStatus.Value == ProductPriceRequestStatus.Origin)
                    {
                        var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                "TLMemoInvalid");
                        throw new BizException(String.Format(errorStr, product.ProductID));
                    }
                    if (String.IsNullOrEmpty(productPriceRequest.PMDMemo.Trim())
                       && productPriceRequest.RequestStatus.Value == ProductPriceRequestStatus.NeedSeniorApprove)
                    {
                        var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                "PMDMemoInvalid");
                        throw new BizException(String.Format(errorStr, product.ProductID));
                    }
                }
                var result = ExternalDomainBroker.CheckMarketIsActivity(productSysNo);
                if (result && status == ProductPriceRequestStatus.Approved)
                {
                    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                   "ProductPriceRequestStatusInvalid");
                    throw new BizException(String.Format(errorStr, product.ProductID));
                }
                ExternalDomainBroker.CheckComboPriceAndSetStatus(productSysNo);

            }

            /// <summary>
            /// 检查商品价格变动单据审核单据
            /// </summary>
            /// <param name="productPriceRequest"></param>
            /// <param name="status"></param>
            /// <param name="productID"></param>
            /// <returns></returns>
            public static void CheckModifyStatusPermission(ProductPriceRequestInfo productPriceRequest, ProductPriceRequestStatus? status, ref string productID)
            {
                CheckProductPriceReques(productPriceRequest);
                if (productPriceRequest.AuditType == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestTypeIsNull"));
                }
                if (productPriceRequest.AuditType == ProductPriceRequestAuditType.Audit
                    || (productPriceRequest.AuditType == ProductPriceRequestAuditType.SeniorAudit
                    && productPriceRequest.RequestStatus == ProductPriceRequestStatus.Origin))
                {
                    if (!productPriceRequest.HasPrimaryAuditPricePermission)
                    {
                        var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                               "TLAuditPermission");
                        throw new BizException(String.Format(errorStr, productID));
                    }
                }

                if (productPriceRequest.AuditType == ProductPriceRequestAuditType.SeniorAudit
                    && productPriceRequest.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove)
                {
                    if (!productPriceRequest.HasAdvancedAuditPricePermission)
                    {
                        var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                               "PMDAuditPermission");
                        throw new BizException(String.Format(errorStr, productID));
                    }
                }

            }

            /// <summary>
            /// 检查审核人
            /// </summary>
            /// <param name="productPriceRequest"></param>
            /// <param name="productID"></param>
            public static void CheckProductPriceRequestCreateSysNo(ProductPriceRequestInfo productPriceRequest, string productID)
            {
                CheckProductPriceReques(productPriceRequest);
                if (productPriceRequest.CreateUser == null
                    || productPriceRequest.CreateUser.SysNo == null
                    || productPriceRequest.CreateUser.SysNo <= 0)
                {
                    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                                                                  "ExistProductPriceRequestForCreateSysNo");
                    throw new BizException(String.Format(errorStr, productID));
                }
                //if (productPriceRequest.CreateUser.SysNo == ServiceContext.Current.UserSysNo)
                //{
                //    var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                //                                                 "ProductPriceRequestForCreateSysNoIsInvalid");
                //    throw new BizException(String.Format(errorStr, productID));
                //}
                //if (productPriceRequest.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove)
                //{
                //    if (productPriceRequest.AuditUser.SysNo == ServiceContext.Current.UserSysNo)
                //    {
                //        var errorStr = ResouceManager.GetMessageString("IM.ProductPriceRequest",
                //                                               "ProductPriceRequestForCreateSysNoIsInvalid");
                //        throw new BizException(String.Format(errorStr, productID));
                //    }
                //}
            }
        }
        #endregion

        #region 毛利率计算
        private void CalcProductPrice(ProductPriceRequestInfo requestInfo)
        {
            if (requestInfo == null) return;
            if (requestInfo.SysNo == null) return;
            CalcMargin(requestInfo);
            CalcMarginAmount(requestInfo);
            var productSysNo = GetProductSysNoByAuditProductPriceSysNo(requestInfo.SysNo.Value);
            var source = ExternalDomainBroker.GetProductPromotionDiscountInfoList(productSysNo);
            requestInfo.NewMargin = requestInfo.Margin;
            requestInfo.OldPrice.NewMargin = requestInfo.OldPrice.Margin;
            requestInfo.NewMarginAmount = requestInfo.MarginAmount;
            requestInfo.OldPrice.NewMarginAmount = requestInfo.OldPrice.NewMarginAmount;
            if (source == null || source.Count == 0)
            {
                return;
            }
            var targetSource = source.Where(e => PromotionTypeList.Contains(e.PromotionType)).ToList();
            if (targetSource.Count == 0) return;
            var targetSources =
                  (from p in targetSource
                   group p by p.PromotionType into g
                   let maxPrice = g.Max(p => p.Discount)
                   select g.Where(p => p.Discount == maxPrice).First()).ToList();

            var amount = targetSources.Sum(p => p.Discount);
            amount = requestInfo.CurrentPrice ?? 0 - amount;
            var oldAmount = requestInfo.OldPrice.CurrentPrice ?? 0 - amount;
            var margin = ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(amount, requestInfo.Point ?? 0, requestInfo.UnitCost);
            var oldMargin = ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(oldAmount, requestInfo.OldPrice.Point ?? 0, requestInfo.OldPrice.UnitCost);
            var marginAmount = ObjectFactory<ProductPriceProcessor>.Instance.GetMarginAmount(amount, requestInfo.Point ?? 0, requestInfo.UnitCost);
            var oldMarginAmount = ObjectFactory<ProductPriceProcessor>.Instance.GetMarginAmount(oldAmount, requestInfo.OldPrice.Point ?? 0, requestInfo.OldPrice.UnitCost);
            margin = Math.Round(margin, 4);
            oldMargin = Math.Round(oldMargin, 4);
            marginAmount = Math.Round(marginAmount, 4);
            oldMarginAmount = Math.Round(oldMarginAmount, 4);
            requestInfo.NewMargin = margin;
            requestInfo.OldPrice.NewMargin = oldMargin;
            requestInfo.MarginAmount = marginAmount;
            requestInfo.OldPrice.MarginAmount = oldMarginAmount;
        }

        /// <summary>
        /// 计算毛利率
        /// </summary>
        /// <param name="requestInfo"></param>
        private void CalcMargin(ProductPriceRequestInfo requestInfo)
        {
            var margin = ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(requestInfo.CurrentPrice ?? 0, requestInfo.Point ?? 0, requestInfo.UnitCost);
            margin = Math.Round(margin, 4);
            requestInfo.Margin = margin;
            if (requestInfo.OldPrice != null)
            {
                margin = ObjectFactory<ProductPriceProcessor>.Instance.GetMargin(requestInfo.OldPrice.CurrentPrice ?? 0, requestInfo.OldPrice.Point ?? 0, requestInfo.OldPrice.UnitCost);
                margin = Math.Round(margin, 4);
                requestInfo.OldPrice.Margin = margin;
            }
        }

        /// <summary>
        /// 计算毛利
        /// </summary>
        /// <param name="requestInfo"></param>
        private void CalcMarginAmount(ProductPriceRequestInfo requestInfo)
        {
            var margin = ObjectFactory<ProductPriceProcessor>.Instance.GetMarginAmount(requestInfo.CurrentPrice ?? 0, requestInfo.Point ?? 0, requestInfo.UnitCost);
            margin = Math.Round(margin, 4);
            requestInfo.MarginAmount = margin;
            if (requestInfo.OldPrice != null)
            {
                margin = ObjectFactory<ProductPriceProcessor>.Instance.GetMarginAmount(requestInfo.OldPrice.CurrentPrice ?? 0, requestInfo.OldPrice.Point ?? 0, requestInfo.OldPrice.UnitCost);
                margin = Math.Round(margin, 4);
                requestInfo.OldPrice.MarginAmount = margin;
            }
        }


        #endregion

        //private bool SendPriceRequestEmail(int requestSysNo, int productSysNo, EmailType emailType, string companyCode, string reasons)
        //{
        //    //try
        //    //{
        //    //    EmailHelper.SendEmailByTemplate(customer.BasicInfo.Email, "MKT_ProductReviewMailContent", replaceVariables, false);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    TxtFileLogger logger = LoggerManager.GetLogger("EmailNotify.log");
        //    //    logger.WriteLog(ex);
        //    //    return false;
        //    //}
        //    return true;
        //}
        //[SendEmailAttribute(SendEmailType.CreatePriceRequest)]
        public void InsertProductPriceRequest(int productSysNo, ProductPriceRequestInfo productPriceRequestInfo) 
        {
            _productPriceRequestDA.InsertProductPriceRequest(productSysNo, productPriceRequestInfo);
            //SetSendEmailAddress(productSysNo, productPriceRequestInfo);
        }

        //[SendEmailAttribute(SendEmailType.CancelPriceRequest)]
        public void CancelAuditProductPriceRequest(ProductPriceRequestInfo productPriceRequestInfo, ProductPriceRequestStatus status) 
        {
            _productPriceRequestDA.UpdateProductPriceRequestStatus(productPriceRequestInfo.SysNo.Value, status);
            //SetSendEmailAddress(_productPriceRequestDA.GetProductSysNoBySysNo(productPriceRequestInfo.SysNo.Value), productPriceRequestInfo);
        }

        private void SetSendEmailAddress(int productsysno,ProductPriceRequestInfo entity) 
        {
            ProductLineInfo plentity = ObjectFactory<IProductLineDA>.Instance.GetPMByProductSysNo(productsysno);
            if (plentity!=null)
            {
                entity.PMUserEmailAddress = ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(plentity.PMUserSysNo).UserInfo.EmailAddress;
                entity.BackupPMUserEmailAddress = GetBackupPMUserEmailAddress(plentity.BackupPMSysNoList);
                entity.CreateUserEmailAddress = ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(entity.CreateUser.SysNo.Value).UserInfo.EmailAddress;
                entity.CurrentUserEmailAddress = ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(ServiceContext.Current.UserSysNo).UserInfo.EmailAddress;
            }           
        }

        private string GetBackupPMUserEmailAddress(string bakPMUserSysNos)
        {
            List<string> bakpmemaillist = new List<string>();

            List<string> bakpmlist = bakPMUserSysNos.Split(new string[]{";"},StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            foreach (var item in bakpmlist)
            {
                ProductManagerInfo pm = ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(int.Parse(item));
                if (pm != null)
                {
                    bakpmemaillist.Add(pm.UserInfo.EmailAddress);
                }
            }
            return bakpmemaillist.Join(";");
        }
    }
}
