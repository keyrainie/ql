using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.EventMessage;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(OPCProcessor))]
    public class OPCProcessor
    {

        IOPCDA OPCDA = ObjectFactory<IOPCDA>.Instance;

        private WMSActionType WMSActionToAction(WMSAction action)
        {
            switch (action)
            {
                case WMSAction.Abandon:
                    return WMSActionType.Abandon;
                case WMSAction.CancelAuditHold:
                    return WMSActionType.CancelAuditHold;
                case WMSAction.AbandonHold:
                    return WMSActionType.AbandonHold;
                case WMSAction.Hold:
                    return WMSActionType.Hold;
                case WMSAction.UnHold:
                    return WMSActionType.UnHold;
            }
            return WMSActionType.Hold;
        }
        public void SendMessageToWMS(SOInfo soInfo, WMSAction action, OPCCallBackType callBackType)
        {
            object mark = new object();
            List<int> stockSysNoList = (from item in soInfo.Items
                                        where item.StockSysNo.HasValue && item.ProductType != SOProductType.Coupon && item.ProductType != SOProductType.ExtendWarranty
                                        select item.StockSysNo.Value).Distinct().ToList();
            lock (mark)
            {
                int masterID = this.SaveOPCMaster(soInfo, action, callBackType);
                foreach (int stockSysNo in stockSysNoList)
                {
                    WarehouseInfo wareInfo = ExternalDomainBroker.GetWarehouseInfo(stockSysNo);
                    string stockID = wareInfo == null ? "" : wareInfo.WarehouseID;
                    if (String.IsNullOrEmpty(stockID))
                    {
                        BizExceptionHelper.Throw("SO_Audit_StockIsNotExist", stockSysNo.ToString());
                    }

                    int transactionID = this.SaveOPCTransaction(soInfo.SysNo.Value, stockSysNo, masterID, soInfo.CompanyCode);
                    WMSSOActionRequestMessage requestEntity = new WMSSOActionRequestMessage
                    {
                        SOSysNo = soInfo.SysNo.Value,
                        CompanyCode = soInfo.CompanyCode,
                        ActionDate = DateTime.Now,
                        ActionReason = string.Format("{0} SO", action.ToDisplayText()),
                        StockSysNo = stockSysNo,
                        TransactionSysNo = transactionID,
                        StockID = stockID,
                        Language = soInfo.BaseInfo.LanguageCode,
                        ActionType = WMSActionToAction(action),
                        ActionUser = ServiceContext.Current.UserSysNo
                    };
                    this.SaveOPCMessage(soInfo, transactionID, requestEntity);
                    EventPublisher.Publish<WMSSOActionRequestMessage>(requestEntity);
                }
            }
        }

        private int SaveOPCMessage(SOInfo soInfo, int transactionID, WMSSOActionRequestMessage msEntity)
        {
            OPCOfflineMessageInfo message = new OPCOfflineMessageInfo();
            message.SOSysNo = soInfo.SysNo.Value;
            message.RequestMessage = SerializationUtility.XmlSerialize(msEntity);
            message.OPCTransactionSysNo = transactionID;
            return OPCDA.InsertOPCOfflineMessage(message);
        }

        private int SaveOPCTransaction(int soSysNo, int stockSysNo, int OPCOfflineMasterID, string companyCode)
        {
            OPCOfflineTransactionInfo info = new OPCOfflineTransactionInfo();
            info.StockSysNo = stockSysNo;
            info.Status = OPCTransStatus.Origin;
            info.SOSysNo = soSysNo;
            info.InUser = string.Empty;
            info.OperationFlag = string.Empty;
            info.MasterID = OPCOfflineMasterID;
            info.InDate = DateTime.Now;
            info.CompanyCode = companyCode;
            return OPCDA.InsertOPCOfflineTransaction(info);
        }

        private int SaveOPCMaster(SOInfo soInfo, WMSAction action, OPCCallBackType callBackType)
        {
            OPCOfflineInfo offlineInfo = new OPCOfflineInfo();
            offlineInfo.FromSystem = "IPP_Order_System";
            offlineInfo.ActionType = action;
            offlineInfo.CompanyCode = soInfo.CompanyCode;
            offlineInfo.CustomerSysNo = soInfo.BaseInfo.CustomerSysNo.Value;
            offlineInfo.Indate = DateTime.Now;

            offlineInfo.NeedResponse = true;
            offlineInfo.SentDate = DateTime.Now;
            offlineInfo.SOSysNo = soInfo.SysNo.Value;

            offlineInfo.Status = OPCStatus.Open;
            offlineInfo.CallBackService = callBackType;
            offlineInfo.Body = SerializationUtility.XmlSerialize(soInfo);

            return OPCDA.InsertOPCOfflineInfo(offlineInfo);
        }

        /// <summary>
        /// 根据MasterID获取OPCTransaction集合
        /// </summary>
        /// <param name="masterID">主ID</param>
        /// <returns>集合数据</returns>
        public List<OPCOfflineTransactionInfo> GetTransactionsByMasterID(int masterID)
        {
            var result = OPCDA.GetOPCOfflineTransactionByMasterID(masterID);
            if (result != null)
            {
                foreach (var item in result)
                {
                    var stock = ExternalDomainBroker.GetWarehouseInfo(item.StockSysNo);
                    if (stock != null)
                    {
                        item.StockName = stock.WarehouseName;
                    }
                }
            }
            return result;
        }
    }
}
