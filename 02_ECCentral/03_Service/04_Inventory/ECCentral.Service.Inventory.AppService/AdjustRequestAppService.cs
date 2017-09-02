using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    /// <summary>
    /// 损益单 - AppService
    /// </summary>
    [VersionExport(typeof(AdjustRequestAppService))]
    public class AdjustRequestAppService
    {
        private AdjustRequestProcessor adjustRequestProcessor = ObjectFactory<AdjustRequestProcessor>.Instance;

        #region 损益单维护

        /// <summary>
        /// 根据损益单的SysNo获取损益单的全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetAdjustRequestInfoBySysNo(int requestSysNo)
        {
            AdjustRequestInfo requestInfo = adjustRequestProcessor.GetAdjustRequestInfoBySysNo(requestSysNo);
            LoadRequestDetailInfo(requestInfo);
            return requestInfo;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetProductLineInfo(int ProductsysNo)
        {
            AdjustRequestInfo requestInfo = adjustRequestProcessor.GetProductLineInfo(ProductsysNo);

            return requestInfo;
        }

        /// <summary>
        /// 创建损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<AdjustRequestInfo> CreateRequest(AdjustRequestInfo entityToCreate)
        {            
            LoadRequestDetailInfo(entityToCreate);
            List<AdjustRequestInfo> requestList = adjustRequestProcessor.CreateRequest(entityToCreate);
            foreach (AdjustRequestInfo request in requestList)
            {
                LoadRequestDetailInfo(request);
            }
            return requestList;
        }

        /// <summary>
        /// 更新损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo UpdateRequest(AdjustRequestInfo entityToUpdate)
        {            
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.UpdateRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }


        /// <summary>
        /// 作废损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo AbandonRequest(AdjustRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.AbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消作废损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CancelAbandonRequest(AdjustRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.CancelAbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 审核损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo VerifyRequest(AdjustRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.VerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消审核损益单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CancelVerifyRequest(AdjustRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.CancelVerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 损益出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo OutStockRequest(AdjustRequestInfo entityToUpdate)
        {            
            LoadRequestProductInfo(entityToUpdate);
            AdjustRequestInfo resultRequest = adjustRequestProcessor.OutStockRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 更新损益单发票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo MaintainAdjustInvoiceInfo(AdjustRequestInfo entityToUpdate)
        {  
            AdjustRequestInfo resultRequest = adjustRequestProcessor.MaintainAdjustInvoiceInfo(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        #endregion

        #region 私有方法

        private void LoadRequestDetailInfo(AdjustRequestInfo requestInfo)
        {
            LoadRequestProductInfo(requestInfo);
            LoadRequestUserInfo(requestInfo);
            //batch info
            if (requestInfo.AdjustItemInfoList != null && requestInfo.AdjustItemInfoList.Count > 0&&requestInfo.SysNo.HasValue)
            {
                LoadRequestProductBatchInfo(requestInfo);
            }
        }

        private void LoadRequestUserInfo(AdjustRequestInfo requestInfo)
        {
            if (requestInfo.CreateUser != null && requestInfo.CreateUser.SysNo != null)
            {
                requestInfo.CreateUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.CreateUser.SysNo);
            }

            if (requestInfo.EditUser != null && requestInfo.EditUser.SysNo != null)
            {
                requestInfo.EditUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.CreateUser.SysNo);
            }

            if (requestInfo.AuditUser != null && requestInfo.AuditUser.SysNo != null)
            {
                requestInfo.AuditUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.CreateUser.SysNo);
            }

            if (requestInfo.OutStockUser != null && requestInfo.OutStockUser.SysNo != null)
            {
                requestInfo.OutStockUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.CreateUser.SysNo);
            }

        }       
        
        private void LoadRequestProductInfo(AdjustRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.AdjustItemInfoList == null || requestInfo.AdjustItemInfoList.Count == 0)
            {
                return;
            }

            IIMBizInteract IMService = ObjectFactory<IIMBizInteract>.Instance;

            foreach (var item in requestInfo.AdjustItemInfoList)
            {
                if (item.AdjustProduct != null && item.AdjustProduct.SysNo > 0)
                {
                    item.AdjustProduct = IMService.GetProductInfo(item.AdjustProduct.SysNo);
                }
            }
        }

        private void LoadRequestProductBatchInfo(AdjustRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.AdjustItemInfoList == null || requestInfo.AdjustItemInfoList.Count == 0)
            {
                return;
            }
            ProductInventoryProcessor proInventory = ObjectFactory<ProductInventoryProcessor>.Instance;

            foreach (var item in requestInfo.AdjustItemInfoList)
            {
                item.BatchDetailsInfoList = proInventory.GetProdcutBatchesInfo("AD", requestInfo.SysNo.Value, item.AdjustProduct.SysNo);
            }
        }

        #endregion 私有方法
    }
}
