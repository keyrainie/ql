using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.SqlDataAccess
{
     [VersionExport(typeof(IProductSalesAreaBatchDA))]
    public class ProductSalesAreaBatchDA : IProductSalesAreaBatchDA
    {
         /// <summary>
         /// 得到商品信息
         /// </summary>
         /// <param name="query"></param>
         /// <param name="totalCount"></param>
         /// <returns></returns>
        public DataTable GetProductByQuery(ProductSalesAreaBatchQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductSalesAreaBatch");
            dc.SetParameterValue("@category1",query.Category1SysNo);
            dc.SetParameterValue("@category2", query.Category2SysNo);
            dc.SetParameterValue("@category3", query.Category3SysNo);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageIndex", query.PageInfo.PageIndex);
            dc.SetParameterValue("@ProductStatus", query.ProductStatus);
            dc.SetParameterValue("@VendorName", query.VendorName);
            dc.SetParameterValue("@ManufacturerName", query.ManufacturerName);
            dc.SetParameterValue("@productName", query.ProductName);
            dc.SetParameterValue("@shortField", query.PageInfo.SortBy);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable(7,typeof(ProductStatus));
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;

        }

         /// <summary>
         /// 得到所有省份
         /// </summary>
         /// <returns></returns>
        public DataTable GetAllProvince()
        {
             DataCommand dc = DataCommandManager.GetDataCommand("GetAllProvince");
             return dc.ExecuteDataTable();
        }

         /// <summary>
         /// 商品销售区域设置查询
         /// </summary>
         /// <param name="query"></param>
         /// <param name="totalCount"></param>
         /// <returns></returns>
        public DataTable GetProductSalesAreaBatchList(ProductSalesAreaBatchQueryFilter query, out int totalCount)
        {
            DataCommand dc;
            if (query.IsSearchProduct) //查询商品
            {
                dc = DataCommandManager.GetDataCommand("GetItemSalesAreaListBatchByProduct");
            }
            else
            {
                dc = DataCommandManager.GetDataCommand("GetItemSalesAreaListBatch");
            }
            dc.SetParameterValue("@Category1SysNo",query.Category1SysNo);
            dc.SetParameterValue("@Category2SysNo", query.Category2SysNo);
            dc.SetParameterValue("@Category3SysNo", query.Category3SysNo);
            dc.SetParameterValue("@ManufacturerName",query.ManufacturerName==null?null:string.Format("%{0}%",query.ManufacturerName));
            dc.SetParameterValue("@ProductName", query.ProductName==null?null:string.Format("%{0}%", query.ProductName));
            dc.SetParameterValue("@ProvinceSysNo", query.ProvinceSysNos);
            dc.SetParameterValue("@StockSysNo", query.StockSysNos);
            dc.SetParameterValue("@SortField", query.PageInfo.SortBy);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable();
            totalCount = (int)dc.GetParameterValue("@TotalCount");
            return dt;
           }

         /// <summary>
         /// 移除销售区域
         /// </summary>
         /// <param name="Info"></param>
        public void RemoveItemSalesAreaListBatch(ProductSalesAreaBatchInfo Info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemoveItemSalesAreaListBatch");
            dc.SetParameterValue("@ProductSysNo",Info.ProductSysNo);
            dc.SetParameterValue("@StockSysNo", Info.StockSysNo);
            dc.ExecuteNonQuery();

        }

         /// <summary>
         /// 移除省份
         /// </summary>
         /// <param name="info"></param>
        public void RemoveProvinceByProductSysNo(ProductSalesAreaBatchInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemoveProvinceByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            dc.SetParameterValue("@ProvinceSysNo", info.ProvinceSysNo);
            dc.SetParameterValue("@StockSysNo", info.StockSysNo);
            dc.ExecuteNonQuery();
        }
    }
}
