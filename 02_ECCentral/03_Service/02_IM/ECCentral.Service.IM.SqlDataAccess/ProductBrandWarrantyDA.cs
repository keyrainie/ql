using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.IDataAccess
{
    [VersionExport(typeof(IProductBrandWarrantyDA))]
    public class ProductBrandWarrantyDA : IProductBrandWarrantyDA
    {
        #region IProductBrandWarrantyDA
        //获取品牌维护信息
        public DataTable GetProductBrandWarrantyByQuery(ProductBrandWarrantyQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandWarrantyInfo");
            cmd.SetParameterValue("@C1SysNo", query.C1SysNo);
            cmd.SetParameterValue("@C2SysNo", query.C2SysNo);
            cmd.SetParameterValue("@C3SysNo", query.C3SysNo);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@BrandSysNo", query.BrandSysNo);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);
            DataTable dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }
        //插入品牌维护
        public int InsertBrandWarrantyInfo(ProductBrandWarrantyInfo productBrandWarranty)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertBrandWarrantyInfo");
            cmd.SetParameterValue("@BrandSysNo", productBrandWarranty.BrandSysNo);
            cmd.SetParameterValue("@C3SysNo", productBrandWarranty.C3SysNo);
            cmd.SetParameterValue("@WarrantyDay", productBrandWarranty.WarrantyDay);
            cmd.SetParameterValue("@WarrantyDesc", productBrandWarranty.WarrantyDesc);
            cmd.SetParameterValue("@InUser", productBrandWarranty.CreateUser.UserDisplayName);
            cmd.SetParameterValue("@LanguageCode", "Zh-Cn");
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@SysNo");
        }
        //更新品牌维护通过品牌和三级类别
        public void UpdateBrandWarrantyInfoByBrandSysNoAndC3SysNo(ProductBrandWarrantyInfo productBrandWarranty)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandWarrantyInfoByBrandSysNoAndC3SysNo");
            cmd.SetParameterValue("@BrandSysNo", productBrandWarranty.BrandSysNo);
            cmd.SetParameterValue("@C3SysNo", productBrandWarranty.C3SysNo);
            cmd.SetParameterValue("@WarrantyDay", productBrandWarranty.WarrantyDay);
            cmd.SetParameterValue("@WarrantyDesc", productBrandWarranty.WarrantyDesc);
            cmd.SetParameterValue("@EditUser", productBrandWarranty.EditUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }
        //更新品牌維護
        public void UpdateBrandWarrantyInfoBySysNo(ProductBrandWarrantyInfo productBrandWarranty)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandWarrantyInfoBySysNo");
            cmd.SetParameterValue("@SysNo", productBrandWarranty.SysNo);
            cmd.SetParameterValue("@WarrantyDay", productBrandWarranty.WarrantyDay);
            cmd.SetParameterValue("@WarrantyDesc", productBrandWarranty.WarrantyDesc);
            cmd.SetParameterValue("@EditUser", productBrandWarranty.EditUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }
        //查询三级别类别
        public List<ProductBrandWarrantyInfo> GetC3SysNo(ProductBrandWarrantyInfo productBrandWarranty)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandWarrantyC3SysNo");
            cmd.SetParameterValue("@C1SysNo", productBrandWarranty.C1SysNo);
            cmd.SetParameterValue("@C2SysNo", productBrandWarranty.C2SysNo);
            cmd.SetParameterValue("@C3SysNo", productBrandWarranty.C3SysNo);
            return cmd.ExecuteEntityList<ProductBrandWarrantyInfo>();
        }
        //获取所有品牌保修
        public List<ProductBrandWarrantyInfo> GetBrandWarrantyInfoByAll()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandWarrantyInfoByAll");
            return cmd.ExecuteEntityList<ProductBrandWarrantyInfo>();
        }
        //删除品牌保修
        public void DelBrandWarrantyInfoBySysNo(Int32 SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelBrandWarrantyInfoBySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据C3和品牌获取品牌保修
        /// </summary>
        /// <param name="c3sysno"></param>
        /// <param name="brandsysno"></param>
        /// <returns></returns>
        public ProductBrandWarrantyInfo GetBrandWarranty(int c3sysno, int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandWarranty");
            cmd.SetParameterValue("@C3SysNo", c3sysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteEntity<ProductBrandWarrantyInfo>();
        }

        /// <summary>
        ///  允许删除品牌保修
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ProductBrandWarrantyInfo GetAllowDeleteBrandWarranty(int c3sysno, int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllowDeleteBrandWarranty");
            cmd.SetParameterValue("@C3SysNo", c3sysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteEntity<ProductBrandWarrantyInfo>();
        }
        #endregion
    }
}