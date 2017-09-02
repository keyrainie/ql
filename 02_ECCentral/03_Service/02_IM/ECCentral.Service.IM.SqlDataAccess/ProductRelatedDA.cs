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
    [VersionExport(typeof(IProductRelatedDA))]
   public  class ProductRelatedDA:IProductRelatedDA
    {
       /// <summary>
       /// 根据query 得到相关商品信息
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
        public DataTable GetProductRelatedByQuery(ProductRelatedQueryFilter query, out int totalCount)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetItemRelatedList");
            dc.SetParameterValue("@ProductSysNo", query.ProductSysNo);
            dc.SetParameterValue("@RelatedProductSysNo", query.RelatedProductSysNo);
            dc.SetParameterValue("@PMSysNo", query.PMUserSysNo);
            dc.SetParameterValue("@SortField", query.PageInfo.SortBy);
            dc.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            dc.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            DataTable dt = new DataTable();
            dt = dc.ExecuteDataTable();
            totalCount =Convert.ToInt32(dc.GetParameterValue("@TotalCount"));
            return dt;
            

        }


        public int CreateProductRelate(ProductRelatedInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateItemRelated");

            dc.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            dc.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            dc.SetParameterValue("@RelatedProductSysNo", info.RelatedProductSysNo);
            dc.SetParameterValue("@IsMutual", info.IsMutual);
            dc.SetParameterValue("@CompanyCode", info.CompanyCode);
            dc.SetParameterValue("@Priority", info.Priority);
            dc.SetParameterValue("@LanguageCode", info.LanguageCode);
            dc.ExecuteNonQuery();
            int flag = Convert.ToInt32(dc.GetParameterValue("@Flag"));
            return flag;
        }





        public void DeleteProductRelate(string sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemoveItemRelated");
            dc.SetParameterValue("@SysNos", sysNo);
            dc.ExecuteNonQuery();
        }


        public void UpdateProductRelatePriority(ProductRelatedInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductRelatePriority");
            dc.SetParameterValue("@Priority",info.Priority);
            dc.SetParameterValue("@SysNo", info.SysNo);
            dc.ExecuteNonQuery();
        }
    }
}
