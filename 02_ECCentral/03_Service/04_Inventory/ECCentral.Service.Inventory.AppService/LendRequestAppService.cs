using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
//using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(LendRequestAppService))]
    public class LendRequestAppService
    {
        private LendRequestProcessor lendRequestProcessor = ObjectFactory<LendRequestProcessor>.Instance;

        #region 借货单维护

        /// <summary>
        /// 根据借货单的SysNo获取借货单的全部信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetLendRequestInfoBySysNo(int requestSysNo)
        {
            LendRequestInfo requestInfo = lendRequestProcessor.GetLendRequestInfoBySysNo(requestSysNo);
            LoadRequestDetailInfo(requestInfo);
            return requestInfo;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetProductLineInfo(int ProductsysNo)
        {
            LendRequestInfo requestInfo = lendRequestProcessor.GetProductLineInfo(ProductsysNo);

            return requestInfo;
        }

        /// <summary>
        /// 创建借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CreateRequest(LendRequestInfo entityToCreate)
        {
            LoadRequestProductInfo(entityToCreate);
            LendRequestInfo resultRequest = lendRequestProcessor.CreateRequest(entityToCreate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 更新借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo UpdateRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.UpdateRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }
       
        /// <summary>
        /// 作废借货单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo AbandonRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.AbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消作废借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CancelAbandonRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.CancelAbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 审核借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo VerifyRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.VerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消审核借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CancelVerifyRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.CancelVerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }
        
        /// <summary>
        /// 借货出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo OutStockRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.OutStockRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 借货归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo ReturnRequest(LendRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            LendRequestInfo resultRequest = lendRequestProcessor.ReturnRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        #endregion

        #region 私有方法

        private void LoadRequestDetailInfo(LendRequestInfo requestInfo)
        {
            LoadRequestProductInfo(requestInfo);
            LoadRequestUserInfo(requestInfo);
            // batch info
            if(requestInfo.SysNo.HasValue)
            LoadRequestProductBatchInfo(requestInfo);
        }

        private void LoadRequestUserInfo(LendRequestInfo requestInfo)
        {
            if (requestInfo.CreateUser != null && requestInfo.CreateUser.SysNo != null)
            {
                requestInfo.CreateUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.CreateUser.SysNo);
            }

            if (requestInfo.EditUser != null && requestInfo.EditUser.SysNo != null)
            {
                requestInfo.EditUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.EditUser.SysNo);
            }

            if (requestInfo.AuditUser != null && requestInfo.AuditUser.SysNo != null)
            {
                requestInfo.AuditUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.AuditUser.SysNo);
            }

            if (requestInfo.OutStockUser != null && requestInfo.OutStockUser.SysNo != null)
            {
                requestInfo.OutStockUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.OutStockUser.SysNo);
            }

        }

        private void LoadRequestProductInfo(LendRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.LendItemInfoList == null || requestInfo.LendItemInfoList.Count == 0)
            {
                return;
            }

            IIMBizInteract IMService = ObjectFactory<IIMBizInteract>.Instance;

            foreach (var item in requestInfo.LendItemInfoList)
            {
                if (item.LendProduct != null && item.LendProduct.SysNo > 0)
                {
                    item.LendProduct = IMService.GetProductInfo(item.LendProduct.SysNo);
                }
            }
        }

        private void LoadRequestProductBatchInfo(LendRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.LendItemInfoList == null || requestInfo.LendItemInfoList.Count == 0)
            {
                return;
            }
            ProductInventoryProcessor proInventory = ObjectFactory<ProductInventoryProcessor>.Instance;

            foreach (var item in requestInfo.LendItemInfoList)
            {
                item.BatchDetailsInfoList = proInventory.GetProdcutBatchesInfo("LD", requestInfo.SysNo.Value, item.LendProduct.SysNo);
            }
        }
        #endregion 私有方法
    }
}
