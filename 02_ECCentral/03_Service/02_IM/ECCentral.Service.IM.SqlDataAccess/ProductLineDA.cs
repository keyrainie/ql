using System.Collections.Generic;
using System.Data;
using System.Linq;

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductLineDA))]
    public class ProductLineDA : IProductLineDA
    {

        #region IProductLineDA Members
        public ProductLineInfo GetProductLine(int sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetProductLine");
            cmd.SetParameterValue("@SysNo", sysno);
            return cmd.ExecuteEntity<ProductLineInfo>();
        }

        public List<ProductLineInfo> GetProductLineByPMSysNo(int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetByPMUserSysNo");
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            return cmd.ExecuteEntityList<ProductLineInfo>();
        }

        public List<ProductLineInfo> GetByBakPMUserSysNo(int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetByBakPMUserSysNo");
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            return cmd.ExecuteEntityList<ProductLineInfo>();
        }

        public ProductLineInfo Create(ProductLineInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_Create");
            cmd.SetParameterValue("@BrandSysNo",entity.BrandSysNo);
            cmd.SetParameterValue("@C2SysNo", entity.C2SysNo);
            cmd.SetParameterValue("@PMSysNo", entity.PMUserSysNo);
            cmd.SetParameterValue("@BackupPMSysNoList", entity.BackupPMSysNoList);
            cmd.SetParameterValue("@MerchandiserSysNo",entity.MerchandiserSysNo);
            cmd.SetParameterValue("@InUser",entity.InUser);
            cmd.SetParameterValue("@CompanyCode",entity.CompanyCode);
            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return entity;
        }

        public ProductLineInfo Update(ProductLineInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_UpdateProductLine");
            cmd.SetParameterValue("@SysNo",entity.SysNo);
            cmd.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            cmd.SetParameterValue("@C2SysNo", entity.C2SysNo);
            cmd.SetParameterValue("@PMSysNo", entity.PMUserSysNo);
            cmd.SetParameterValue("@BackupPMSysNoList", entity.BackupPMSysNoList);
            cmd.SetParameterValue("@MerchandiserSysNo", entity.MerchandiserSysNo);
            cmd.SetParameterValue("@EditUser", entity.EditUser);
            cmd.ExecuteNonQuery();
            return entity;
        }

        public void CreateChangePool(int linesysno, int? c2sysno, int? brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_CreateChangePool");
            cmd.SetParameterValue("@ProductLineSysNo", linesysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            cmd.SetParameterValue("@C2SysNo", c2sysno);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_DeleteBySysNo");
            cmd.SetParameterValue("@ProductLineSysNo", sysNo);
            cmd.ExecuteNonQuery();
        }

        public void DeleteByPMUserSysNo(int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_DeleteByPMUserSysNo");
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            cmd.ExecuteNonQuery();
        }

        public bool IsExists(int c2sysno, int? brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_IsExists");
            cmd.SetParameterValue("@C2SysNo",c2sysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool HasSameChange(int linesysno, int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_HasSameChange");
            cmd.SetParameterValue("@ProductLineSysNo", linesysno);
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool BrandIsUsing(int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_BrandIsUsing");
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool ManufacturerIsUsing(int manufacturersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_ManufacturerIsUsing");
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturersysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool Category2IsUsing(int csysno, int categorytype)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_Category2IsUsing");
            cmd.SetParameterValue("@CategorySysNo", csysno);
            cmd.SetParameterValue("@CategoryType", categorytype);
            return cmd.ExecuteScalar<bool>();
        }

        public bool PMIsUsing(int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_PMIsUsing");
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            return cmd.ExecuteScalar<bool>();
        }

        public void DeleteByBrand(int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_DeleteByBrand");
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            cmd.ExecuteNonQuery();
        }

        public void DeleteByManufacturer(int manufacturersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_DeleteByManufacturer");
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturersysno);
            cmd.ExecuteNonQuery();
        }

        public void DeleteByCategory(int csysno, int categorytype)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_DeleteByCategory");
            cmd.SetParameterValue("@CategorySysNo", csysno);
            cmd.SetParameterValue("@CategoryType", categorytype);
            cmd.ExecuteNonQuery();
        }

        public bool IsProductLosePM(int c2sysno,int? brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_IsProductLosePM");
            cmd.SetParameterValue("@C2SysNo", c2sysno);
            cmd.SetParameterValue("@BrandSysNo", c2sysno);
            return cmd.ExecuteScalar<bool>();
        }

        #region 关键数据有效性验证
        public bool C2IsValid(int c2sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_C2IsValid");
            cmd.SetParameterValue("@C2SysNo", c2sysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool BrandIsValid(int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_BrandIsValid");
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteScalar<bool>();
        }

        public bool PMIsValid(int pmusersysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_PMIsValid");
            cmd.SetParameterValue("@PMUserSysNo", pmusersysno);
            return cmd.ExecuteScalar<bool>();
        }
        
        #endregion

        public ProductManagerInfo GetPMByC3(int c3sysno, int brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetPMByC3");
            cmd.SetParameterValue("@C3SysNo", c3sysno);
            var pllist = cmd.ExecuteEntityList<ProductLineInfo>();
            int? pmSysNo = default(int?);
            string pmname = string.Empty;
            var productline = pllist.FirstOrDefault(p => p.BrandSysNo == brandsysno);
            if (productline != null)
            {
                pmSysNo = productline.PMUserSysNo;
                pmname = productline.PMUserName;
            }
            else
            {
                /*
                * 如果只配类别，品牌不配置的话，那么就代表这个类别和所有品牌全都配置这个PM
                */
                productline = pllist.FirstOrDefault(p => p.BrandSysNo == null);
                if (productline != null)
                {
                    pmSysNo = productline.PMUserSysNo;
                    pmname = productline.PMUserName;
                }
            }
            if (pmSysNo != null)
            {
                return ObjectFactory<IProductManagerDA>.Instance.GetProductManagerInfoByUserSysNo(pmSysNo.Value);
            }
            return null;
        }

        public ProductLineInfo GetPMByProductSysNo(int productsysno) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_GetPMByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productsysno);
            return cmd.ExecuteEntity<ProductLineInfo>();
        }

        public bool HasRightByPMUser(ProductLineInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_HasRightByPMUser");
            cmd.SetParameterValue("@PMUserSysNo", entity.PMUserSysNo);
            cmd.SetParameterValue("@C3SysNo", entity.C3SysNo);
            cmd.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            return cmd.ExecuteScalar<bool>();
        }

        public bool HasProduct(int? c2sysno, int? brandsysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductLine_HasProduct");
            cmd.SetParameterValue("@C2SysNo", c2sysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);

            return cmd.ExecuteScalar<bool>();
        }
        #endregion
    }
}
