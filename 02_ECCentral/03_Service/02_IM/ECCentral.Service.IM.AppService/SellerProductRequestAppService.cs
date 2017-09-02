//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理业务实现
// 作成者				Kevin
// 改版日				2012.6.7
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using ECCentral.BizEntity;
using System.Text;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(SellerProductRequestAppService))]
    public class SellerProductRequestAppService
    {
        /// <summary>
        /// 根据SysNO获取商家商品请求信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SellerProductRequestInfo GetSellerProductRequestInfoBySysNo(int sysNo)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.GetSellerProductRequestInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据ProductID获取商家商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SellerProductRequestInfo GetSellerProductInfoByProductID(string productID)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.GetSellerProductInfoByProductID(productID);
            return result;
        }

        /// <summary>
        /// 审核通过新品创建请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo ApproveProductRequest(SellerProductRequestInfo entity)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.ApproveProductRequest(entity);
            return result;
        }

        /// <summary>
        /// 退回新品创建请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo DenyProductRequest(SellerProductRequestInfo entity)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.DenyProductRequest(entity);
            return result;
        }

        /// <summary>
        /// 更新请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo UpdateProductRequest(SellerProductRequestInfo entity)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.UpdateProductRequest(entity);
            return result;
        }

        
        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo CreateItemIDForNewProductRequest(SellerProductRequestInfo entity)
        {
            var result = ObjectFactory<SellerProductRequestProcessor>.Instance.CreateItemIDForNewProductRequest(entity);
            return result;
        }

        
        /// <summary>
        /// 审核通过请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchApproveProductRequest(List<SellerProductRequestInfo> entityList)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (var entity in entityList)
            {
                try
                {
                    ObjectFactory<SellerProductRequestProcessor>.Instance.ApproveProductRequest(entity);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductName");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, entity.ProductName, ex.Message));
                    errorCount++;
                }
            }

            string resMessage = ResouceManager.GetMessageString("IM.ProductPriceRequest", "AuditSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductPriceRequest", "AuditFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());        
        }

        /// <summary>
        /// 批量退回请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchDenyProductRequest(List<SellerProductRequestInfo> entityList)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (var entity in entityList)
            {
                try
                {
                    ObjectFactory<SellerProductRequestProcessor>.Instance.DenyProductRequest(entity);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductName");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, entity.ProductName, ex.Message));
                    errorCount++;
                }
            }

            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "BackSuccess");
            resMessage += "：{0}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "，";
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "BackFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());
        }

        /// <summary>
        /// 批量创建商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchCreateItemIDForNewProductRequest(List<SellerProductRequestInfo> entityList)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (var entity in entityList)
            {
                try
                {
                    var request = ObjectFactory<SellerProductRequestProcessor>.Instance.GetSellerProductRequestInfoBySysNo(entity.SysNo.Value);
                    request.EditUser = entity.EditUser;
                    ObjectFactory<SellerProductRequestProcessor>.Instance.CreateItemIDForNewProductRequest(request);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductName");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, entity.ProductName, ex.Message));
                    errorCount++;
                }
            }

            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "CreateSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "CreateFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());

        }

    }
}
