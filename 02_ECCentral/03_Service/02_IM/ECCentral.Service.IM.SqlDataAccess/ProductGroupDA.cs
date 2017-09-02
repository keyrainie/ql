using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductGroupDA))]
    public class ProductGroupDA : IProductGroupDA
    {

        #region 获取商品组信息

        public ProductGroup GetProductGroupInfoBySysNo(int productGroupSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductGroupInfoBySysNo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            var entity = dc.ExecuteEntity<ProductGroup>();
            entity.ProductGroupSettings = GetProductGroupSettings(productGroupSysNo);
            entity.ProductList = ObjectFactory<IProductDA>.Instance.GetProductListByProductGroupSysNo(productGroupSysNo);
            return entity;
        }

        public ProductGroup GetProductGroup(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductGroupByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntity<ProductGroup>();
            if (sourceEntity != null && sourceEntity.SysNo != null)
            {
                sourceEntity.ProductGroupSettings = (sourceEntity.SysNo != null
                                                         ? GetProductGroupSettings(sourceEntity.SysNo.Value)
                                                         : new List<ProductGroupSettings>()) ??
                                                    new List<ProductGroupSettings>();
                sourceEntity.ProductList =
                    ObjectFactory<IProductDA>.Instance.GetProductListByProductGroupSysNo(sourceEntity.SysNo.Value);
            }
            return sourceEntity;
        }

        public List<int> GetProductSysNoListByGroupProductSysNo(int productSysNo)
        {
            var result = new List<int>();
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductGroupByProductSysNo");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = dc.ExecuteEntity<ProductGroup>();
            if (sourceEntity != null && sourceEntity.SysNo != null)
            {
                result =
                    ObjectFactory<IProductDA>.Instance.GetProductSysNoListByProductGroupSysNo(sourceEntity.SysNo.Value).ToList();
            }
            return result;
        }

        public ProductGroup GetProductGroupInfoByGroupName(int sysNo, string groupName)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductGroupInfoByGroupName");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@ProductGroupName", groupName);
            var entity = dc.ExecuteEntity<ProductGroup>();
            return entity;
        }

        private IList<ProductGroupSettings> GetProductGroupSettings(int productGroupSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductGroupSettingsByProductGroupSysNo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductGroupSettings>();
            return sourceEntity;
        }

        #endregion

        /// <summary>
        /// 创建商品组信息
        /// </summary>
        /// <param name="productGroup"></param>
        public void CreateProductGroupInfo(ProductGroup productGroup)
        {
            var product = productGroup.ProductList.First();
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductGroupInfo");
            cmd.SetParameterValue("@ProductName", productGroup.ProductGroupName.Content);
            if (product.ProductBasicInfo.ProductBrandInfo.SysNo.HasValue)
            {
                cmd.SetParameterValue("@BrandSysno", product.ProductBasicInfo.ProductBrandInfo.SysNo.Value);
            }
            if (product.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
            {
                cmd.SetParameterValue("@C3SysNo", product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value);
            }
            cmd.SetParameterValue("@ProductModel", productGroup.ProductGroupModel.Content);
            cmd.SetParameterValue("@Type", "C");
            cmd.SetParameterValue("@InUser", productGroup.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", productGroup.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", productGroup.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", productGroup.LanguageCode);
            cmd.ExecuteNonQuery();
            productGroup.SysNo = (int)cmd.GetParameterValue("@SysNo");
        }

        /// <summary>
        /// 编辑商品组信息
        /// </summary>
        /// <param name="productGroup"></param>
        public void UpdateProductGroupInfo(ProductGroup productGroup)
        {
            if (productGroup.SysNo.HasValue)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductGroupInfo");
                cmd.SetParameterValue("@SysNo", productGroup.SysNo.Value);
                cmd.SetParameterValue("@ProductName", productGroup.ProductGroupName.Content);
                cmd.SetParameterValue("@ProductModel", productGroup.ProductGroupModel.Content);
                cmd.SetParameterValue("@EditUser", productGroup.OperateUser.UserDisplayName);
                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveProductCommonInfoGroupSysNo(int productGroupSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemoveProductCommonInfoGroupSysNo");
            dc.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建商品组分组属性
        /// </summary>
        /// <param name="productGroup"></param>
        public void CreateGroupPropertySetting(ProductGroup productGroup)
        {
            productGroup.ProductGroupSettings.ForEach(setting =>
            {
                if (productGroup.SysNo.HasValue && setting.ProductGroupProperty.SysNo.HasValue)
                {
                    DataCommand cmd = DataCommandManager.GetDataCommand("CreateGroupPropertySetting");
                    cmd.SetParameterValue("@ProductGroupSysno", productGroup.SysNo.Value);
                    cmd.SetParameterValue("@PropertySysno", setting.ProductGroupProperty.SysNo.Value);
                    cmd.SetParameterValue("@IsPolymeric", setting.Polymeric);
                    cmd.SetParameterValue("@IsDisplayPic", setting.ImageShow);
                    cmd.SetParameterValue("@ShowName", setting.PropertyBriefName.Content);
                    cmd.SetParameterValue("@InUser", productGroup.OperateUser.UserDisplayName);
                    cmd.SetParameterValue("@CompanyCode", productGroup.CompanyCode);
                    cmd.SetParameterValue("@StoreCompanyCode", productGroup.CompanyCode);
                    cmd.SetParameterValue("@LanguageCode", productGroup.LanguageCode);
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// 删除商品组所有分组属性
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        public void DeleteGroupPropertySetting(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAllGroupPropertySetting");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<string> GetProductGroupIDSFromProductID(string productID)
        {
            var list = new List<string>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupIDSFromProductID");
            cmd.SetParameterValue("@ProductID", productID);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.ToString());
                }
            }
            return list;
        }
    }
}
