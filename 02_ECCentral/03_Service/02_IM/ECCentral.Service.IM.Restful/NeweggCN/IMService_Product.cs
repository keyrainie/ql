using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Linq;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    partial class IMService
    {
        #region QueryProduct

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/QueryProductEx", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductEx(NeweggProductQueryFilter request)
        {
            int totalCount;
            int maxTotalCnt = 500;
            var dataTable = ObjectFactory<IProductQueryDA>.Instance.QueryProductEx(request, out totalCount);
            ObjectFactory<ProductAppService>.Instance.ClearProduct(dataTable, ref totalCount);
            //分区处理导出全部的数据
            if (dataTable.Rows.Count > maxTotalCnt)
            {
                int index = dataTable.Rows.Count / maxTotalCnt;
                index += (dataTable.Rows.Count % maxTotalCnt > 0 ? 1 : 0);
                System.Data.DataTable dtTmp = new System.Data.DataTable();
                System.Data.DataTable dtResult = new System.Data.DataTable();
                dtTmp = dataTable.Clone();
                for (int i = 0; i < index; i++)
                {
                    dtTmp.Rows.Clear();
                    for (int j = maxTotalCnt * i; j < maxTotalCnt * (i + 1); j++)
                    {
                        if (j >= dataTable.Rows.Count)
                        {
                            break;
                        }
                        dtTmp.ImportRow(dataTable.Rows[j]);
                    }
                    ObjectFactory<ProductAppService>.Instance.GetInventoryInfoByStock(dtTmp);
                    if (i == 0)
                    {
                        ObjectFactory<ProductAppService>.Instance.AddOtherData(dtTmp);
                        dtResult = dtTmp.Clone();
                    }
                    foreach (System.Data.DataRow r in dtTmp.Rows)
                    {
                        dtResult.ImportRow(r);
                    }
                }
                return new QueryResult
                {
                    Data = dtResult,
                    TotalCount = totalCount
                };
            }
            else
            {
                ObjectFactory<ProductAppService>.Instance.GetInventoryInfoByStock(dataTable);
                ObjectFactory<ProductAppService>.Instance.AddOtherData(dataTable);
                return new QueryResult
                {
                    Data = dataTable,
                    TotalCount = totalCount
                };
            }
        }

        /// <summary>
        /// 分仓类表
        /// </summary>
        /// <param name="webChannelID"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetStockList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ObjectFactory<ProductAppService>.Instance.GetWarehouseList(companyCode);
        }

        /// <summary>
        /// 导出商检文件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ExportInspection", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportInspection(List<int> productSysnos)
        {
            int totalCount = 0;
            var dataTable = ObjectFactory<IProductQueryDA>.Instance.QueryExporterEntryFile(productSysnos);
            if (dataTable != null && dataTable.Rows != null)
            {
                totalCount = dataTable.Rows.Count;
            }
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }


        /// <summary>
        /// 导出报关文件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/ExportTariffApply", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportTariffApply(List<int> productSysnos)
        {
            int totalCount = 0;
            var dataTable = ObjectFactory<IProductQueryDA>.Instance.QueryExporterEntryFile(productSysnos);
            if (dataTable != null && dataTable.Rows != null)
            {
                totalCount = dataTable.Rows.Count;
            }
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion
    }
}

