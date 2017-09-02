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
    /// 转换单 - AppService
    /// </summary>
    [VersionExport(typeof(ConvertRequestAppService))]
    public class ConvertRequestAppService
    {
        private ConvertRequestProcessor convertRequestProcessor = ObjectFactory<ConvertRequestProcessor>.Instance;
        #region 转换单维护

        /// <summary>
        /// 根据转换单的SysNo获取转换单的全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetConvertRequestInfoBySysNo(int requestSysNo)
        {
            ConvertRequestInfo requestInfo = convertRequestProcessor.GetConvertRequestInfoBySysNo(requestSysNo);
            LoadRequestDetailInfo(requestInfo);
            return requestInfo;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetProductLineInfo(int ProductsysNo)
        {
            ConvertRequestInfo requestInfo = convertRequestProcessor.GetProductLineInfo(ProductsysNo);

            return requestInfo;
        }

        /// <summary>
        /// 创建转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CreateRequest(ConvertRequestInfo entityToCreate)
        {
            LoadRequestProductInfo(entityToCreate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.CreateRequest(entityToCreate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 更新转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo UpdateRequest(ConvertRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.UpdateRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 作废转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo AbandonRequest(ConvertRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.AbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消作废转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CancelAbandonRequest(ConvertRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.CancelAbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 审核转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo VerifyRequest(ConvertRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.VerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消审核转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CancelVerifyRequest(ConvertRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.CancelVerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
            
        }

        /// <summary>
        /// 转换出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo OutStockRequest(ConvertRequestInfo entityToUpdate)
        {   
            LoadRequestProductInfo(entityToUpdate);
            ConvertRequestInfo resultRequest = convertRequestProcessor.OutStockRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }
 

        #endregion

        #region 私有方法

        private void LoadRequestDetailInfo(ConvertRequestInfo requestInfo)
        {
            LoadRequestProductInfo(requestInfo);
            LoadRequestUserInfo(requestInfo);
            //batch info
            if(requestInfo.SysNo.HasValue)
            LoadRequestProductBatchInfo(requestInfo);
        }
        

        private void LoadRequestUserInfo(ConvertRequestInfo requestInfo)
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

     
        private void LoadRequestProductInfo(ConvertRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.ConvertItemInfoList == null || requestInfo.ConvertItemInfoList.Count == 0)
            {
                return;
            }            

            foreach (var item in requestInfo.ConvertItemInfoList)
            {
                if (item.ConvertProduct != null && item.ConvertProduct.SysNo > 0)
                {
                    item.ConvertProduct = ExternalDomainBroker.GetProductInfoByProductSysNo(item.ConvertProduct.SysNo);
                }
            }
        }

        private void LoadRequestProductBatchInfo(ConvertRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.ConvertItemInfoList == null || requestInfo.ConvertItemInfoList.Count == 0)
            {
                return;
            }
            ProductInventoryProcessor proInventory = ObjectFactory<ProductInventoryProcessor>.Instance;

            foreach (var item in requestInfo.ConvertItemInfoList)
            {
                item.BatchDetailsInfoList = proInventory.GetProdcutBatchesInfo("TR", requestInfo.SysNo.Value, item.ConvertProduct.SysNo);
            }
        }

        #endregion 私有方法
    }
}
