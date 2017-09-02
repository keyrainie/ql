using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.EventMessage.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    #region 商品价格审核待办事项

    /// <summary>
    /// 提交商品价格审核待办事项
    /// </summary>
    public class ProductPriceAuditSubmitTask : WPMessageCreator<ProductPriceAuditSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 100;
        }

        protected override string GetBizSysNo(ProductPriceAuditSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(ProductPriceAuditSubmitMessage msg)
        {
            return null;
        }

        protected override string GetMemo(ProductPriceAuditSubmitMessage msg)
        {
            return "提交商品价格审核";
        }

        protected override int GetCurrentUserSysNo(ProductPriceAuditSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
        protected override bool NeedProcess(ProductPriceAuditSubmitMessage msg)
        {
            throw new NotImplementedException();
            //int productSysNo = msg != null && msg.ProductSysNo.HasValue ? (int)msg.ProductSysNo : 0;
            //int requestSysNo = msg != null ? msg.RequestSysNo : 0;
            //if (msg.SalesPolicyType == ECCentral.Service.EventMessage.IM.SalesPolicyType.Normal)
            //{
            //    var product = ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
            //    if (product == null
            //        || (product.ProductPriceRequest == null
            //        || product.ProductPriceRequest.RequestStatus == ProductPriceRequestStatus.Approved
            //        || product.ProductPriceRequest.RequestStatus == ProductPriceRequestStatus.Deny
            //        || product.ProductPriceRequest.RequestStatus == ProductPriceRequestStatus.Canceled
            //        || product.ProductPriceRequest.RequestStatus == ProductPriceRequestStatus.NeedSeniorApprove))
            //        return false;
            //    return true;
            //}
            //if (msg.SalesPolicyType == ECCentral.Service.EventMessage.IM.SalesPolicyType.TimeRelated)
            //{
            //    var rule = ObjectFactory<IIMBizInteract>.Instance.LoadChannelTimePriceRuleBySysNo(requestSysNo);
            //    if (rule == null
            //        || (rule.RequestStatus == ProductPriceRuleStatus.Approved
            //        || rule.RequestStatus == ProductPriceRuleStatus.Deny
            //        || rule.RequestStatus == ProductPriceRuleStatus.Void))
            //        return false;
            //    return true;
            //}
            //return true;
        }
    }
    /// <summary>
    /// 通过商品价格审核待办事项
    /// </summary>
    public class ProductPriceAuditTask : WPMessageCompleter<ProductPriceAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 100;
        }

        protected override string GetBizSysNo(ProductPriceAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(ProductPriceAuditMessage msg)
        {
            return "通过商品价格审核";
        }

        protected override int GetCurrentUserSysNo(ProductPriceAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(ProductPriceAuditMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 拒绝商品价格审核待办事项
    /// </summary>
    public class ProductPriceRejectTask : WPMessageCompleter<ProductPriceRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 100;
        }

        protected override string GetBizSysNo(ProductPriceRejectMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(ProductPriceRejectMessage msg)
        {
            return "拒绝商品价格审核";
        }

        protected override int GetCurrentUserSysNo(ProductPriceRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
        protected override bool NeedProcess(ProductPriceRejectMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 取消商品价格审核待办事项
    /// </summary>
    public class CanceledUpdateProductPriceRequest_CompleteUpdateProductPriceRequestTask : WPMessageCompleter<CanceledUpdateProductPriceRequestMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 100;
        }

        protected override string GetBizSysNo(CanceledUpdateProductPriceRequestMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override int GetCurrentUserSysNo(CanceledUpdateProductPriceRequestMessage msg)
        {
            return msg.CancelUserSysNo;
        }
    }

    #endregion

    #region 生产商审核待办事项

    /// <summary>
    /// 提交生产商审核待办事项
    /// </summary>
    public class ManufacturerAuditSubmitTask : WPMessageCreator<ManufacturerAuditSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 101;
        }

        protected override string GetBizSysNo(ManufacturerAuditSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(ManufacturerAuditSubmitMessage msg)
        {
            return null;
        }

        protected override string GetMemo(ManufacturerAuditSubmitMessage msg)
        {
            return "提交生产商审核";
        }

        protected override int GetCurrentUserSysNo(ManufacturerAuditSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
        protected override bool NeedProcess(ManufacturerAuditSubmitMessage msg)
        {
            throw new NotImplementedException();
            //int sysNo = msg != null ? msg.RequestSysNo : 0;
            //var request = ObjectFactory<IIMBizInteract>.Instance.GetManufacturerRequest(sysNo);
            //if (request == null
            //    || (request.Status == 1 || request.Status == -1 || request.Status == -2))
            //    return false;
            //return true;
        }
    }
    /// <summary>
    /// 通过生产商审核待办事项
    /// </summary>
    public class ManufacturerAuditTask : WPMessageCompleter<ManufacturerAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 101;
        }

        protected override string GetBizSysNo(ManufacturerAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(ManufacturerAuditMessage msg)
        {
            return "通过生产商审核";
        }

        protected override int GetCurrentUserSysNo(ManufacturerAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(ManufacturerAuditMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 拒绝生产商审核待办事项
    /// </summary>
    public class ManufacturerRejectTask : WPMessageCompleter<ManufacturerRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 101;
        }

        protected override string GetBizSysNo(ManufacturerRejectMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(ManufacturerRejectMessage msg)
        {
            return "拒绝生产商审核";
        }

        protected override int GetCurrentUserSysNo(ManufacturerRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
        protected override bool NeedProcess(ManufacturerRejectMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 取消生产商审核待办事项
    /// </summary>
    public class ManufacturerCancelTask : WPMessageCompleter<ManufacturerCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 101;
        }

        protected override string GetBizSysNo(ManufacturerCancelMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(ManufacturerCancelMessage msg)
        {
            return "取消生产商审核";
        }

        protected override int GetCurrentUserSysNo(ManufacturerCancelMessage msg)
        {
            return msg.CancelUserSysNo;
        }
        protected override bool NeedProcess(ManufacturerCancelMessage msg)
        {
            return true;
        }
    }

    #endregion

    #region 品牌审核待办事项

    /// <summary>
    /// 提交品牌审核待办事项
    /// </summary>
    public class BrandAuditSubmitTask : WPMessageCreator<BrandAuditSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 102;
        }

        protected override string GetBizSysNo(BrandAuditSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(BrandAuditSubmitMessage msg)
        {
            return null;
        }

        protected override string GetMemo(BrandAuditSubmitMessage msg)
        {
            return "提交品牌审核";
        }

        protected override int GetCurrentUserSysNo(BrandAuditSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
        protected override bool NeedProcess(BrandAuditSubmitMessage msg)
        {
            throw new NotImplementedException();
            //int sysNo = msg != null ? msg.RequestSysNo : 0;
            //var request = ObjectFactory<IIMBizInteract>.Instance.GetBrandRequestBySysNo(sysNo);
            //if (request == null || (request.Status == BrandValidStatus.Active || request.Status == BrandValidStatus.DeActive))
            //    return false;
            //return true;
        }
    }
    /// <summary>
    /// 通过品牌审核待办事项
    /// </summary>
    public class BrandAuditTask : WPMessageCompleter<BrandAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 102;
        }

        protected override string GetBizSysNo(BrandAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(BrandAuditMessage msg)
        {
            return "通过品牌审核";
        }

        protected override int GetCurrentUserSysNo(BrandAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(BrandAuditMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 拒绝品牌审核待办事项
    /// </summary>
    public class BrandRejectTask : WPMessageCompleter<BrandRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 102;
        }

        protected override string GetBizSysNo(BrandRejectMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(BrandRejectMessage msg)
        {
            return "拒绝品牌审核";
        }

        protected override int GetCurrentUserSysNo(BrandRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
        protected override bool NeedProcess(BrandRejectMessage msg)
        {
            return true;
        }
    }

    #endregion

    #region 商品类别审核待办事项

    /// <summary>
    /// 提交商品类别审核待办事项
    /// </summary>
    public class CategoryAuditSubmitTask : WPMessageCreator<CategoryAuditSubmitMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 103;
        }

        protected override string GetBizSysNo(CategoryAuditSubmitMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetUrlParameter(CategoryAuditSubmitMessage msg)
        {
            return null;
        }

        protected override string GetMemo(CategoryAuditSubmitMessage msg)
        {
            return "提交商品类别审核";
        }

        protected override int GetCurrentUserSysNo(CategoryAuditSubmitMessage msg)
        {
            return msg.SubmitUserSysNo;
        }
        protected override bool NeedProcess(CategoryAuditSubmitMessage msg)
        {
            throw new NotImplementedException();
            //int sysNo = msg != null ? msg.RequestSysNo : 0;
            //var request = ObjectFactory<IIMBizInteract>.Instance.GetCategoryRequestBySysNo(sysNo);
            //if (request == null || (request.Status == 1 || request.Status == -1 || request.Status == -2))
            //    return false;
            //return true;
        }
    }
    /// <summary>
    /// 通过商品类别审核待办事项
    /// </summary>
    public class CategoryAuditTask : WPMessageCompleter<CategoryAuditMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 103;
        }

        protected override string GetBizSysNo(CategoryAuditMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(CategoryAuditMessage msg)
        {
            return "通过商品类别审核";
        }

        protected override int GetCurrentUserSysNo(CategoryAuditMessage msg)
        {
            return msg.AuditUserSysNo;
        }
        protected override bool NeedProcess(CategoryAuditMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 拒绝商品类别审核待办事项
    /// </summary>
    public class CategoryRejectTask : WPMessageCompleter<CategoryRejectMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 103;
        }

        protected override string GetBizSysNo(CategoryRejectMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(CategoryRejectMessage msg)
        {
            return "拒绝商品类别审核";
        }

        protected override int GetCurrentUserSysNo(CategoryRejectMessage msg)
        {
            return msg.RejectUserSysNo;
        }
        protected override bool NeedProcess(CategoryRejectMessage msg)
        {
            return true;
        }
    }
    /// <summary>
    /// 取消商品类别审核待办事项
    /// </summary>
    public class CategoryCancelTask : WPMessageCompleter<CategoryCancelMessage>
    {
        protected override int GetCategorySysNo()
        {
            return 103;
        }

        protected override string GetBizSysNo(CategoryCancelMessage msg)
        {
            return msg.RequestSysNo.ToString();
        }

        protected override string GetMemo(CategoryCancelMessage msg)
        {
            return "取消商品类别审核";
        }

        protected override int GetCurrentUserSysNo(CategoryCancelMessage msg)
        {
            return msg.CancelUserSysNo;
        }
        protected override bool NeedProcess(CategoryCancelMessage msg)
        {
            return true;
        }
    }

    #endregion
}
