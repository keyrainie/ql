using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using System.IO;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using System.Data;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        [WebInvoke(UriTemplate = "/Basket/QueryPurchaseOrderBasketList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPurchaseOrderBasketList(PurchaseOrderBasketQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IPurchaseOrderBasketQueryDA>.Instance.QueryBasketList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 批量创建采购单
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/Basket/BatchCreatePurchaseOrder", Method = "PUT")]
        public BatchCreateBasketResultInfo BatchCreatePurchaseOrder(List<BasketItemsInfo> list)
        {
            return ObjectFactory<PurchaseOrderBasketAppService>.Instance.BatchCreatePurchaseOrder(list);
        }

        /// <summary>
        /// 根据编号加载采购单商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Basket/LoadBasketItem/{sysNo}", Method = "GET")]
        public BasketItemsInfo LoadBasketItem(string sysNo)
        {
            return ObjectFactory<PurchaseOrderBasketAppService>.Instance.LoadBasketItem(Convert.ToInt32(sysNo));
        }


        /// <summary>
        /// 批量添加赠品操作
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Basket/BatchAddGift", Method = "PUT")]
        public void BatchAddGiftForBasket(List<BasketItemsInfo> list)
        {
            ObjectFactory<PurchaseOrderBasketAppService>.Instance.BatchCreateGiftForBasket(list);
        }

        /// <summary>
        /// 批量更新采购篮商品
        /// </summary>
        /// <param name="itemList"></param>
        [WebInvoke(UriTemplate = "/Basket/BatchUpdateBasketItems", Method = "PUT")]
        public void BatchUpdateBasketItems(List<BasketItemsInfo> itemList)
        {
            ObjectFactory<PurchaseOrderBasketAppService>.Instance.BatchUpdateBasketItems(itemList);
        }

        /// <summary>
        /// 批量删除采购篮商品
        /// </summary>
        /// <param name="itemList"></param>
        [WebInvoke(UriTemplate = "/Basket/BatchDeleteBasketItems", Method = "PUT")]
        public void BatchDeleteBasketItems(List<BasketItemsInfo> itemList)
        {
            ObjectFactory<PurchaseOrderBasketAppService>.Instance.BatchDeleteBasketItems(itemList);
        }

        /// <summary>
        /// 解析上传的采购篮模板，并转换为List:
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Basket/ConvertBasketTemplateFileToEntityList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList ConvertBasketTemplateFileToEntityList(string fileIdentity)
        {
            int sucCount = 0;
            int failCount = 0;
            string errorMessage = string.Empty;
            List<BasketItemsInfo> failedList = new List<BasketItemsInfo>();
            ObjectFactory<PurchaseOrderBasketAppService>.Instance.ConvertBasketTemplateFileToEntityList(fileIdentity, out sucCount, out failCount, out errorMessage, out failedList);
            //errorMessage = string.Format("导入成功{0}条，导入失败{1}条", sucCount, failCount);
            DataTable dt = new DataTable();

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("successCount");
            dtResult.Columns.Add("failedCount");
            var newResultRow = dtResult.NewRow();
            newResultRow["successCount"] = sucCount;
            newResultRow["failedCount"] = failCount;
            dtResult.Rows.Add(newResultRow);

            var propertys = typeof(BasketItemsInfo).GetProperties();
            foreach (var property in propertys)
            {
                dt.Columns.Add(property.Name);
            }

            foreach (var data in failedList)
            {
                var newRow = dt.NewRow();
                foreach (var property in propertys)
                {
                    newRow[property.Name] = property.GetValue(data, null);
                }
                dt.Rows.Add(newRow);
            }

            return new QueryResultList()
            {
                new QueryResult()
                {
                    TotalCount = failCount,
                    Data = dt
                },
                new QueryResult()
                {
                    TotalCount = 1,
                    Data = dtResult
                }
                
            };
        }

        [WebInvoke(UriTemplate = "/Basket/QueryBasketTargetWarehouseList", Method = "POST")]
        public List<WarehouseInfo> QueryBasketTargetWarehouseList(string companyCode)
        {
            return ObjectFactory<PurchaseOrderBasketAppService>.Instance.QueryBasketTargetWarehouseList(companyCode);
        }

        [WebInvoke(UriTemplate = "/Basket/GetGiftBasketItems", Method = "POST")]
        public List<BasketItemsInfo> GetGiftBasketItems(List<int> productSysNoList)
        {
            return ObjectFactory<PurchaseOrderBasketAppService>.Instance.GetGiftBasketItems(productSysNoList);
        }
    }
}
