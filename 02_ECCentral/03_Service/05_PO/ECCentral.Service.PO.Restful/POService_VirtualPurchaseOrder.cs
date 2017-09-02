using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 查询虚库采购单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/QueryVirtualPurchaseOrderList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualPurchaseOrderList(VirtualPurchaseOrderQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IVirtualPurchaseOrderQueryDA>.Instance.QueryVirtualPurchaseOrderList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }
        /// <summary>
        /// 加载单个虚库采购单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/LoadVirtualPurchaseOrder/{sysNo}", Method = "GET")]
        public VirtualStockPurchaseOrderInfo GetVirtualPurchaseOrderBySysNo(string sysNo)
        {
            return ObjectFactory<VirtualPurchaseOrderAppService>.Instance.LoadVirtualPurchaseOrderInfoBySysNo(Convert.ToInt32(sysNo));
        }

        /// <summary>
        /// 更新虚库采购单信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdateVirtualPurchaseOrder", Method = "PUT")]
        public void UpdateVirtualPurchaseOrderInfo(VirtualStockPurchaseOrderInfo info)
        {
            ObjectFactory<VirtualPurchaseOrderAppService>.Instance.UpdateVirtualPurchaseInfo(info);
        }

        /// <summary>
        /// 更新虚库采购单CS备注
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurchaseOrder/UpdateVirtualPurchaseOrderCSMemo", Method = "PUT")]
        public void UpdateVirtualPurchaseOrderInfoCSMemo(VirtualStockPurchaseOrderInfo info)
        {
            ObjectFactory<VirtualPurchaseOrderAppService>.Instance.UpdateVirtualPurchaseInfoCSMemo(info);
        }

        /// <summary>
        /// 作废虚库采购单
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/PurchaseOrder/AbandonVirtualPurchaseOrder", Method = "PUT")]
        public void AbandonVirtualPurchaseOrder(VirtualStockPurchaseOrderInfo info)
        {
            ObjectFactory<VirtualPurchaseOrderAppService>.Instance.AbandonVirtualPurchaseInfo(info);
        }

        /// <summary>
        /// 计算和加载虚库采购单(SO新建)
        /// </summary>
        /// <param name="SOItemSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/LoadVirtualPurchaseInfoBySOItemSysNo/{soSysNoAndProductSysNo}", Method = "GET")]
        public VirtualStockPurchaseOrderInfo LoadVirtualPurchaseInfoBySOItemSysNo(string soSysNoAndProductSysNo)
        {
            int getSOSysNo = 0;
            int getProductSysNo = 0;
            int.TryParse(soSysNoAndProductSysNo.Split('-')[0].Trim(), out getSOSysNo);
            int.TryParse(soSysNoAndProductSysNo.Split('-')[1].Trim(), out getProductSysNo);
            VirtualStockPurchaseOrderInfo returnEntity = ObjectFactory<VirtualPurchaseOrderAppService>.Instance.LoadVirtualPurchaseInfoBySOItemSysNo(getSOSysNo, getProductSysNo);
            return returnEntity;
        }

        /// <summary>
        /// 创建虚库采购单(从SO链接)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PurchaseOrder/CreateVSPO", Method = "PUT")]
        public VirtualStockPurchaseOrderInfo CreateVSPO(VirtualStockPurchaseOrderInfo info)
        {
            return ObjectFactory<VirtualPurchaseOrderAppService>.Instance.CreateVSPO(info);
        }

        [WebInvoke(UriTemplate = "/PurchaseOrder/IsVSPOItemPriceLimited/{soSysNoAndProductSysNo}", Method = "GET")]
        public bool IsVSPOItemPriceLimited(string soSysNoAndProductSysNo)
        {
            int getSOSysNo = 0;
            int getProductSysNo = 0;
            int getPurchaseQty = 0;
            int.TryParse(soSysNoAndProductSysNo.Split('-')[0].Trim(), out getSOSysNo);
            int.TryParse(soSysNoAndProductSysNo.Split('-')[1].Trim(), out getProductSysNo);
            int.TryParse(soSysNoAndProductSysNo.Split('-')[2].Trim(), out getPurchaseQty);
            return ObjectFactory<VirtualPurchaseOrderAppService>.Instance.IsVSPOItemPriceLimited(getSOSysNo, getProductSysNo, getPurchaseQty);
        }
    }
}
