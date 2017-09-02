using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.Restful.RequestMsg;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class PurchaseOrderFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public PurchaseOrderFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询采购单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrders(PurchaseOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询PO单历史
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrderHistory(PurchaseOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.CurrentUserName = CPApplication.Current.LoginUser.LoginName;
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderHistory";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelForPurchaseOrders(PurchaseOrderQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 按采购单状态统计
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void CountPurchaseOrders(PurchaseOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.CurrentUserName = CPApplication.Current.LoginUser.LoginName;
            string relativeUrl = "POService/PurchaseOrder/CountPurchaseOrderList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询商品的最后一次PO入库的价格
        /// </summary>
        /// <param name="itemSysNo"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrderLastPrice(string itemSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/QueryPurchaseOrderLastPrice/{0}", itemSysNo);
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        /// <summary>
        /// 查询采购单 RMA List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrdersRMAList(PurchaseOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderRMAList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询PO单退货批次信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrderBatchNumberList(PurchaseOrderBatchNumberQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderBatchNumberList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 根据供应商编号和PM NAME，获取可用返点
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryPurchaseOrderEIMSInvoiceInfo(PurchaseOrderQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/QueryPurchaseOrderEIMSInvoiceInfo";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 加载采购单信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="callback"></param>
        public void LoadPurchaseOrderInfo(string poSysNo, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/LoadPurchaseOrderInfo/{0}", poSysNo);
            restClient.Query<PurchaseOrderInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 加载单个采购单ITEM信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="callback"></param>
        public void LoadPurchaseOrderItemInfo(string itemSysNo, EventHandler<RestClientEventArgs<PurchaseOrderItemInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/LoadPurchaseOrderItemInfo/{0}", itemSysNo);
            restClient.Query<PurchaseOrderItemInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 检查PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CheckPurchaseOrderInfo(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/CheckPurchaseOrderInfo";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 更新PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callbadk"></param>
        public void UpdatePurchaseOrderInfo(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/UpdatePurchaseOrderInfo";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateMailAddressAndHasSentMail(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/UpdateMailAddressAndHasSentMail";
            restClient.Update<object>(relativeUrl, info, callback);
        }

        public void SendMailWhenAuditPurchaseOrder(PurchaseOrderSendMailReq request, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/SendMailWhenAuditPurchaseOrder";
            restClient.Update<int>(relativeUrl, request, callback);
        }


        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreatePurchaseOrder(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }
            if (null == info.POItems)
            {
                info.POItems = new List<PurchaseOrderItemInfo>();
            }
            info.POItems.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CurrencyCode = info.PurchaseOrderBasicInfo.CurrencyCode;
                x.BatchInfo = x.BatchInfo == null ? "" : x.BatchInfo;

            });
            info.PurchaseOrderBasicInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;

            string relativeUrl = "POService/PurchaseOrder/CreatePurchaseOrderInfo";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 提交审核PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SubmitAuditPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/SubmitAuditPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }
        /// <summary>
        /// 确认PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void VerifyPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/VerifyPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 取消确认PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelVerifyPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/CancelVerifyPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }
        /// <summary>
        /// 拒绝并退回PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void RefusePO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }
            string relativeUrl = "POService/PurchaseOrder/RefusePO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 作废PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void AbandonPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/AbandonPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 取消作废PO单信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelAbandonPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/CancelAbandonPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// PM与供应商确认
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void PMConfirmWithVendorPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/PMConfirmWithVendorPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 更新入库备注和到付运费金额
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateInStockMemoPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/UpdateInStockMemoPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// PO单中止入库
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void StopInStockPO(PurchaseOrderInfo info, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == info.PurchaseOrderBasicInfo)
            {
                info.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/StopInStockPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, info, callback);
        }

        #region [EIMS相关:]

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNo"></param>
        /// <param name="callback"></param>
        public void GetEIMSRuleInfoBySysNo(string ruleNo, EventHandler<RestClientEventArgs<PurchaseOrderEIMSRuleInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/GetEIMSRuleInfoBySysNo/{0}", ruleNo);
            restClient.Query<PurchaseOrderEIMSRuleInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public void GetEIMSRuleInfoByAssignedCode(string id, EventHandler<RestClientEventArgs<PurchaseOrderEIMSRuleInfo>> callback)
        {
            string relativeUrl = string.Format("POService/PurchaseOrder/GetEIMSRuleInfoByAssignedCode/{0}", id);
            restClient.Query<PurchaseOrderEIMSRuleInfo>(relativeUrl, callback);
        }

        /// <summary>
        ///  审核通过  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void PassETAInfo(PurchaseOrderETATimeInfo info, EventHandler<RestClientEventArgs<PurchaseOrderETATimeInfo>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/PassPurchaseOrderETAInfo";
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            restClient.Update<PurchaseOrderETATimeInfo>(relativeUrl, info, callback);
        }
        /// <summary>
        /// 提交审核  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void SubmitETAInfo(PurchaseOrderETATimeInfo info, EventHandler<RestClientEventArgs<PurchaseOrderETATimeInfo>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/SubmitPurchaseOrderETAInfo";
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            restClient.Update<PurchaseOrderETATimeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        /// 取消审核  -采购单预计到货时间
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CancelETAInfo(PurchaseOrderETATimeInfo info, EventHandler<RestClientEventArgs<PurchaseOrderETATimeInfo>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/CancelPurchaseOrderETAInfo";
            info.EditUser = CPApplication.Current.LoginUser.DisplayName;
            restClient.Update<PurchaseOrderETATimeInfo>(relativeUrl, info, callback);
        }

        /// <summary>
        ///  确认 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <param name="poInfo"></param>
        /// <param name="callback"></param>
        public void ConfirmVendorPortalPurchaseOrder(PurchaseOrderInfo poInfo, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            poInfo.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == poInfo.PurchaseOrderBasicInfo)
            {
                poInfo.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }

            string relativeUrl = "POService/PurchaseOrder/ConfirmVendorPortalPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, poInfo, callback);
        }

        /// <summary>
        ///  审核 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <param name="poInfo"></param>
        /// <param name="callback"></param>
        public void AuditVendorPortalPurchaseOrder(PurchaseOrderInfo poInfo, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            poInfo.CompanyCode = CPApplication.Current.CompanyCode;
            poInfo.PurchaseOrderBasicInfo.AuditUserSysNo = CPApplication.Current.LoginUser.UserSysNo.Value;
            if (null == poInfo.PurchaseOrderBasicInfo)
            {
                poInfo.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }
            poInfo.PurchaseOrderBasicInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/PurchaseOrder/AuditVendorPortalPO";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, poInfo, callback);
        }

        /// <summary>
        ///  补充创建PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <param name="callback"></param>
        public void RenewCreatePurchaseOrder(PurchaseOrderInfo poInfo, EventHandler<RestClientEventArgs<PurchaseOrderInfo>> callback)
        {
            poInfo.CompanyCode = CPApplication.Current.CompanyCode;
            if (null == poInfo.PurchaseOrderBasicInfo)
            {
                poInfo.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            }
            poInfo.PurchaseOrderBasicInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/PurchaseOrder/RenewCreatePurchaseOrder";
            restClient.Update<PurchaseOrderInfo>(relativeUrl, poInfo, callback);
        }

        /// <summary>
        /// 退回 VendorPortal 创建的PO单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void RetreatVendorPortalPurchaseOrder(PurchaseOrderRetreatReq request, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/RetreatVendorPortalPurchaseOrder";
            restClient.Update<object>(relativeUrl, request, callback);
        }


        /// <summary>
        /// 新添加PO商品，
        /// </summary>
        /// <param name="productInfo"></param>
        /// <param name="callback"></param>
        public void AddNewPurchaseOrderItem(PurchaseOrderItemProductInfo productInfo, EventHandler<RestClientEventArgs<PurchaseOrderItemInfo>> callback)
        {
            productInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/AddNewPurchaseOrderItem";
            restClient.Query<PurchaseOrderItemInfo>(relativeUrl, productInfo, callback);
        }



        /// <summary>
        /// （批量）新添加PO商品，
        /// </summary>
        /// <param name="productInfo"></param>
        /// <param name="callback"></param>
        public void BatchAddNewPurchaseOrderItem(List<PurchaseOrderItemProductInfo> productInfoList, EventHandler<RestClientEventArgs<List<PurchaseOrderItemInfo>>> callback)
        {
            productInfoList.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;

            });
            string relativeUrl = "POService/PurchaseOrder/BatchAddPurchaseOrderItems";
            restClient.Query<List<PurchaseOrderItemInfo>>(relativeUrl, productInfoList, callback);
        }

        /// <summary>
        /// 获取采购单赠品信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <param name="callback"></param>
        public void GetPurchaseOrderGiftInfo(List<int> productSysNoList, EventHandler<RestClientEventArgs<List<PurchaseOrderItemProductInfo>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetPurchaseOrderGiftInfo";
            restClient.Query<List<PurchaseOrderItemProductInfo>>(relativeUrl, productSysNoList, callback);
        }

        /// <summary>
        /// 获取采购单附件信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <param name="callback"></param>
        public void GetPurchaseOrderAccessoriesInfo(List<int> productSysNoList, EventHandler<RestClientEventArgs<List<PurchaseOrderItemProductInfo>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetPurchaseOrderAccessoriesInfo";
            restClient.Query<List<PurchaseOrderItemProductInfo>>(relativeUrl, productSysNoList, callback);
        }

        #endregion

        /// <summary>
        /// 获取所有货币类型
        /// </summary>
        /// <param name="callback"></param>
        public void GetCurrencyList(EventHandler<RestClientEventArgs<List<CurrencyInfo>>> callback)
        {
            string relativeUrl = "CommonService/CurrencyType/GetAll";
            restClient.Query<List<CurrencyInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新PO单ITEM的批次信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdatePurchaseOrderBatchInfo(PurchaseOrderItemInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/PurchaseOrder/UpdatePurchaseOrderBatchInfo";
            restClient.Update(relativeUrl, info, callback);

        }

        public void GetPurchaseOrderWarehouseList(EventHandler<RestClientEventArgs<List<WarehouseInfo>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetPurchaseOrderWarehouseList";
            restClient.Query<List<WarehouseInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        /// <summary>
        /// 根据传入的ProductSysno 检测与当前PM是否匹配
        /// </summary>
        public void CheckOperateRightForCurrentUser(int productSysNo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            ProductPMLine tPro = new ProductPMLine()
            {
                ProductSysNo = productSysNo,
                PMSysNo = CPApplication.Current.LoginUser.UserSysNo.Value
            };
            string relativeUrl = "POService/PurchaseOrder/CheckOperateRightForCurrentUser";
            restClient.Update(relativeUrl, tPro, callback);
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        public void GetProductLineSysNoByProductList(int[] productSysNo, EventHandler<RestClientEventArgs<List<ProductPMLine>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetProductLineSysNoByProductList";
            restClient.Query<List<ProductPMLine>>(relativeUrl, productSysNo, callback);
        }
        /// <summary>
        /// 根据PM，获取其全部产品线
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        public void GetProductLineInfoByPM(int pmSysNo, EventHandler<RestClientEventArgs<List<ProductPMLine>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetProductLineInfoByPM";
            restClient.Query<List<ProductPMLine>>(relativeUrl, pmSysNo, callback);
        }
    }
}
