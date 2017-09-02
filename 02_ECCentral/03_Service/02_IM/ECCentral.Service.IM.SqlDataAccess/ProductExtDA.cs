using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.IM;
using System.Xml;
using System.Data;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductExtDA))]
    public class ProductExtDA : IProductExtDA
    {

        public DataTable GetProductExtByQuery(ProductExtQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetItemExtList");
            dc.SetParameterValue("@ProductID", query.ProductID);
            dc.SetParameterValue("@ProductStatus", query.ProductStatus);

            if (query.ProductType != null)
            {
                dc.SetParameterValue("@ProductType", LegacyEnumMapper.ConvertProductType(query.ProductType.Value));
            }
            else
            {
                dc.SetParameterValue("@ProductType", null);
            }
            dc.SetParameterValue("@Manufacturer", query.Manufacturer);
            dc.SetParameterValue("@ProductPrice", query.ProductPrice);
            dc.SetParameterValue("@IsPermitRefund", query.IsPermitRefund);
            dc.SetParameterValue("@Category1", query.Category1);
            dc.SetParameterValue("@Category2", query.Category2);
            dc.SetParameterValue("@Category3", query.Category3);
            dc.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            dc.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(ProductStatus) },
                    { "IsPermitRefund", typeof(IsDefault) }
                   
                };
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable(enumList);
            totalCount = Convert.ToInt32(dc.GetParameterValue("@TotalCount"));
            return dt;
        }

        /// <summary>
        /// 更新是否可以退货
        /// </summary>
        /// <param name="info"></param>
        public void UpdatePermitRefund(ProductExtInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdatePermitRefund");
            dc.SetParameterValue("@IsPermitRefund", info.IsPermitRefund);
            dc.SetParameterValue("@SysNo", info.SysNo);
            dc.SetParameterValue("@EditUserSysNo", ServiceContext.Current.UserSysNo);
            dc.ExecuteNonQuery();

        }


        /// <summary>
        /// 修改商品关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="keywords"></param>
        /// <param name="keywords0"></param>
        /// <param name="editUserSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0, int editUserSysNo, string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductExKeyKeywords");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@Keywords", keywords);
            dc.SetParameterValue("@Keywords0", keywords0);
            dc.SetParameterValue("@EditUserSysNo", editUserSysNo);
            dc.SetParameterValue("@CompanyCode", companyCode);

            return dc.ExecuteNonQuery();
        }

        public void UpdateIsBatch(ProductBatchManagementInfo batchManagementInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductBatchManagementInfo");

            cmd.SetParameterValue(batchManagementInfo);
            cmd.SetParameterValue("@IsBatch", batchManagementInfo.IsBatch.Value ? "Y" : "N");
            cmd.SetParameterValue("@IsCollectBatchNo", batchManagementInfo.IsCollectBatchNo.Value ? "Y" : "N");
            cmd.SetParameterValueAsCurrentUserAcct("@UserAcct");
            //cmd.SetParameterValue("@CompanyCode","8601");
            //cmd.SetParameterValue("@LanguageCode","zh-CN");
            //cmd.SetParameterValue("@StoreCompanyCode","8601");
            cmd.ExecuteNonQuery();
        }

        public ProductBatchManagementInfo GetProductBatchManagementInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductBatchManagementInfo");

            cmd.SetParameterValue("@ProductSysNo",productSysNo);
            var result = cmd.ExecuteEntity<ProductBatchManagementInfo>();
            if (result != null)
            {
                result.Logs = GetProductBatchManagementLogByBatchManagementSysNo(result.SysNo.Value);
            }
            return result;
        }

        public void InsertProductBatchManagementLog(ProductBatchManagementInfoLog entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertProductBatchManagementLog");
            command.SetParameterValue("@BatchManagementSysNo", entity.BatchManagementSysNo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValueAsCurrentUserAcct("@InUser");
            command.ExecuteNonQuery();            
        }

        public List<ProductBatchManagementInfoLog> GetProductBatchManagementLogByBatchManagementSysNo(int batchManagementSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductBatchManagementLogByBatchManagementSysNo");
            cmd.SetParameterValue("@BatchManagementSysNo", batchManagementSysNo);
            return cmd.ExecuteEntityList<ProductBatchManagementInfoLog>();            
        }
    }
}
