//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品信息管理
// 子系统名		        渠道商品信息管理业务实现
// 作成者				Kevin
// 改版日				2012.6.4
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Text;
using ECCentral.BizEntity;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(ProductChannelInfoAppService))]
    public class ProductChannelInfoAppService
    {
        /// <summary>
        /// 根据SysNO获取渠道商品信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductChannelInfo GetProductChannelInfoBySysNo(int sysNo)
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.GetProductChannelInfoBySysNo(sysNo);
            return result;
        }


        /// <summary>
        /// 创建渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void CreatetProductChannelInfo(List<ProductChannelInfo> entityList)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (var entity in entityList)
            {
                try
                {
                    ObjectFactory<ProductChannelInfoProcessor>.Instance.CreatetProductChannelInfo(entity);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductID");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, entity.ProductID, ex.Message));
                    errorCount++;
                }
            }
            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "AddSuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "AddFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());


        }

        /// <summary>
        /// 批量修改渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchUpdateProductChannelInfo(List<ProductChannelInfo> entityList)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (var entity in entityList)
            {
                try
                {
                    var item = ObjectFactory<ProductChannelInfoProcessor>.Instance.GetProductChannelInfoBySysNo(entity.SysNo.Value);

                    item.SafeInventoryQty = entity.SafeInventoryQty;
                    item.ChannelPricePercent = entity.ChannelPricePercent;
                    item.IsUsePromotionPrice = entity.IsUsePromotionPrice;
                    item.InventoryPercent = entity.InventoryPercent;
                    item.EditUser = entity.EditUser;

                    ObjectFactory<ProductChannelInfoProcessor>.Instance.UpdateProductChannelInfo(item, true);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductID");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, entity.ProductID, ex.Message));
                    errorCount++;
                }
            }

            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "ModifySuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "ModifyFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());


        }


        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelInfo UpdateProductChannelInfo(ProductChannelInfo entity)
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.UpdateProductChannelInfo(entity, false);
            return result;
        }

        /// <summary>
        /// 根据SysNO获取渠道商品价格信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice GetProductChannelPeriodPriceBySysNo(int sysNo)
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.GetProductChannelPeriodPriceBySysNo(sysNo);
            return result;
        }


        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice CreatetProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.CreatetProductChannelPeriodPrice(entity);
            return result;
        }


        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice UpdateProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.UpdateProductChannelPeriodPrice(entity);
            return result;
        }

        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        public List<ChannelInfo> GetChannelInfoList()
        {
            var result = ObjectFactory<ProductChannelInfoProcessor>.Instance.GetChannelInfoList();
            return result;
        }

        /// <summary>
        /// 批量设置多渠道商品记录状态
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchUpdateChannelProductInfoStatus(ProductChannelInfo entity)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;



            if (entity == null)
            {
                throw new BizException("BatchUpdateChannelProductInfoStatus param entity is null");
            }
            if (entity.SysNoList == null || entity.SysNoList.Count < 1)
            {
                throw new BizException("BatchUpdateChannelProductInfoStatus param entity.SysNoList is null");
            }

            foreach (var sysNo in entity.SysNoList)
            {
                var channelProduct = ObjectFactory<ProductChannelInfoProcessor>.Instance.GetProductChannelInfoBySysNo(sysNo);
                if (channelProduct != null)
                {
                    if (channelProduct.Status != entity.Status)
                    {
                        channelProduct.Status = entity.Status;
                        channelProduct.EditUser = entity.EditUser;


                        if (entity.Status == ProductChannelInfoStatus.DeActive)
                        {
                            channelProduct.ChannelSellCount = 0;
                            channelProduct.IsAppointInventory = BooleanEnum.No;
                        }

                        try
                        {
                            ObjectFactory<ProductChannelInfoProcessor>.Instance.UpdateProductChannelInfo(channelProduct, false);
                            successCount++;
                        }
                        catch (BizException ex)
                        {
                            string message = ResouceManager.GetMessageString("IM.Category", "ProductID");
                            message += "：{0}，";
                            message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                            message += "：{1}";
                            result.AppendLine(string.Format(message, channelProduct.ProductID, ex.Message));
                            errorCount++;
                        }
                    }
                }
            }
            string resMessage = ResouceManager.GetMessageString("IM.ProductChannelInfo", "ModifySuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.ProductChannelInfo", "ModifyFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());
        }
    }
}
