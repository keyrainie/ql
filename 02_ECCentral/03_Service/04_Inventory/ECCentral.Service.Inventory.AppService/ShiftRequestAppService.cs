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
    /// 移仓单
    /// </summary>
    [VersionExport(typeof(ShiftRequestAppService))]
    public class ShiftRequestAppService
    {
        private ShiftRequestProcessor shiftRequestProcessor = ObjectFactory<ShiftRequestProcessor>.Instance;
        private ShiftRequestMemoProcessor shiftRequestMemoProcessor = ObjectFactory<ShiftRequestMemoProcessor>.Instance;

        #region 移仓单维护

        /// <summary>
        /// 根据移仓单的SysNo获取移仓单的全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetShiftRequestInfoBySysNo(int requestSysNo)
        {
            ShiftRequestInfo requestInfo = shiftRequestProcessor.GetShiftRequestInfoBySysNo(requestSysNo);
            LoadRequestDetailInfo(requestInfo);
            return requestInfo;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetProductLineInfo(int ProductsysNo)
        {
            ShiftRequestInfo requestInfo = shiftRequestProcessor.GetProductLineInfo(ProductsysNo);
           
            return requestInfo;
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestInfo> CreateRequest(ShiftRequestInfo entityToCreate)
        {
            LoadRequestProductInfo(entityToCreate);            
            List<ShiftRequestInfo> requestList = shiftRequestProcessor.CreateRequest(entityToCreate);
            foreach (ShiftRequestInfo request in requestList)
            {
                LoadRequestDetailInfo(request);
            }
            return requestList;
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo UpdateRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.UpdateRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo AbandonRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.AbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo AbandonRequestForPO(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.AbandonRequestForPO(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }


        /// <summary>
        /// 取消作废移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CancelAbandonRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.CancelAbandonRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo VerifyRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.VerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 取消审核移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CancelVerifyRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.CancelVerifyRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;

        }

        /// <summary>
        /// 移仓单出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo OutStockRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.OutStockRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 移仓单入库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo InStockRequest(ShiftRequestInfo entityToUpdate)
        {
            LoadRequestProductInfo(entityToUpdate);
            ShiftRequestInfo resultRequest = shiftRequestProcessor.InStockRequest(entityToUpdate);
            LoadRequestDetailInfo(resultRequest);
            return resultRequest;
        }

        /// <summary>
        /// 移仓单特殊状态更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestInfo> UpdateSpecialShiftTypeBatch(List<ShiftRequestInfo> entityListToUpdate)
        {
            List<ShiftRequestInfo> requestList = shiftRequestProcessor.UpdateSpecialShiftTypeBatch(entityListToUpdate);
            foreach (ShiftRequestInfo request in requestList)
            {
                LoadRequestDetailInfo(request);
            }
            return requestList;
        }
        #endregion

        #region 移仓单跟进日志维护

        /// <summary>
        /// 根据日志SysNo获取日志信息
        /// </summary>
        /// <param name="memoSysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestMemoInfo GetShiftRequestMemoInfoBySysNo(int memoSysNo)
        {
            return shiftRequestMemoProcessor.GetShiftRequestMemoInfoBySysNo(memoSysNo);
        }

        /// <summary>
        /// 根据移仓单的SysNo获取日志列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> GetShiftRequestMemoListByRequestSysNo(int requestSysNo)
        {
            return shiftRequestMemoProcessor.GetShiftRequestMemoInfoListByRequestSysNo(requestSysNo);
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> CreateShiftRequestMemo(List<ShiftRequestMemoInfo> entityToCreate)
        {
            return shiftRequestMemoProcessor.CreateShiftRequestMemo(entityToCreate);
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> UpdateShiftRequestMemoList(List<ShiftRequestMemoInfo> entityListToUpdate)
        {
            return shiftRequestMemoProcessor.UpdateShiftRequestMemoList(entityListToUpdate);
        }

        #endregion 移仓单跟进日志维护

        #region 私有方法

        private void LoadRequestDetailInfo(ShiftRequestInfo requestInfo)
        {
            LoadRequestProductInfo(requestInfo);
            LoadRequestUserInfo(requestInfo);
        }

        private void LoadRequestUserInfo(ShiftRequestInfo requestInfo)
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

            if (requestInfo.InStockUser != null && requestInfo.InStockUser.SysNo != null)
            {
                requestInfo.InStockUser = ExternalDomainBroker.GetUserInfo((int)requestInfo.InStockUser.SysNo);
            }

        }

        private void LoadRequestProductInfo(ShiftRequestInfo requestInfo)
        {
            if (requestInfo == null || requestInfo.ShiftItemInfoList == null || requestInfo.ShiftItemInfoList.Count == 0)
            {
                return;
            }

            IIMBizInteract IMService = ObjectFactory<IIMBizInteract>.Instance;

            foreach (var item in requestInfo.ShiftItemInfoList)
            {
                if (item.ShiftProduct != null && item.ShiftProduct.SysNo > 0)
                {
                    item.ShiftProduct = IMService.GetProductInfo(item.ShiftProduct.SysNo);
                }
            }
        }

        #endregion 私有方法


        #region 仓库移仓配置

        public StockShiftConfigInfo CreateStockShiftConfig(StockShiftConfigInfo info)
        {
            return shiftRequestProcessor.CreateStockShiftConfig(info);
        }
        /// <summary>
        /// 返回true表示修改成功，否则修改失败
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void UpdateStockShiftConfig(StockShiftConfigInfo info)
        {
            shiftRequestProcessor.UpdateStockShiftConfig(info);
        }
        public StockShiftConfigInfo GetStockShiftConfigBySysNo(int sysNo)
        {
            return shiftRequestProcessor.GetStockShiftConfigBySysNo(sysNo);
        }

        #endregion
    }
}
