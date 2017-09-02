using System;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductCommonInfoDA))]
    public class ProductCommonInfoDA : IProductCommonInfoDA
    {
        public void InsertProductCommonInfo(int? productGroupSysNo, ProductInfo productCommonInfo)
        {
            InsertProductCommonInfo(productGroupSysNo.HasValue ? productGroupSysNo.Value : 0, productCommonInfo);
            InsertProductCommonInfoEx(productCommonInfo);
            InsertProductCommonInfoStatus(productCommonInfo);
            if (productCommonInfo.ProductBasicInfo.ProductProperties != null &&
                productCommonInfo.ProductCommonInfoSysNo.HasValue)
            {
                productCommonInfo.ProductBasicInfo.ProductProperties.ForEach(
                    property =>
                    {
                        if (productCommonInfo.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                        {
                            if (property.OperationUser == null)
                                property.OperationUser = productCommonInfo.OperateUser;
                            InsertProductCommonInfoProperty(productCommonInfo.ProductCommonInfoSysNo.Value,
                                                           productCommonInfo.ProductBasicInfo.ProductCategoryInfo.
                                                               SysNo
                                                               .Value, property);
                        }

                    });
            }
        }

        private void InsertProductCommonInfo(int productGroupSysNo, ProductInfo productCommonInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductCommonInfo");
            cmd.SetParameterValue("@CommonSKUNumber", productCommonInfo.ProductBasicInfo.CommonSkuNumber);
            cmd.SetParameterValue("@ProductGroupSysno", productGroupSysNo);
            cmd.SetParameterValue("@UPCCode", productCommonInfo.ProductBasicInfo.UPCCode);
            cmd.SetParameterValue("@BMCode", productCommonInfo.ProductBasicInfo.BMCode);
            cmd.SetParameterValue("@ProductName", productCommonInfo.ProductBasicInfo.ProductTitle.Content);
            cmd.SetParameterValue("@BriefName", productCommonInfo.ProductBasicInfo.ProductBriefName);
            cmd.SetParameterValue("@Keywords", productCommonInfo.ProductBasicInfo.Keywords.Content);
            cmd.SetParameterValue("@PMUserSysNo", productCommonInfo.ProductBasicInfo.ProductManager.SysNo);
            cmd.SetParameterValue("@ProductModel", productCommonInfo.ProductBasicInfo.ProductModel.Content);
            cmd.SetParameterValue("@ProductType", productCommonInfo.ProductBasicInfo.ProductType);
            cmd.SetParameterValue("@IsTakePictures", productCommonInfo.ProductBasicInfo.IsTakePicture);
            cmd.SetParameterValue("@PackageList", productCommonInfo.ProductBasicInfo.PackageList.Content);
            cmd.SetParameterValue("@ProductLink", productCommonInfo.ProductBasicInfo.ProductLink);
            cmd.SetParameterValue("@Attention", productCommonInfo.ProductBasicInfo.Attention.Content);
            cmd.SetParameterValue("@Note", productCommonInfo.ProductBasicInfo.Note);
            cmd.SetParameterValue("@CompanyBelongs", "N");//Newegg产品
            cmd.SetParameterValue("@OnlyForRank", 0);
            cmd.SetParameterValue("@PicNumber", 0);
            cmd.SetParameterValue("@HostWarrantyDay", productCommonInfo.ProductWarrantyInfo.HostWarrantyDay);
            cmd.SetParameterValue("@PartWarrantyDay", productCommonInfo.ProductWarrantyInfo.PartWarrantyDay);
            cmd.SetParameterValue("@Warranty", productCommonInfo.ProductWarrantyInfo.Warranty.Content);
            cmd.SetParameterValue("@ServicePhone", productCommonInfo.ProductWarrantyInfo.ServicePhone);
            cmd.SetParameterValue("@ServiceInfo", productCommonInfo.ProductWarrantyInfo.ServiceInfo);
            cmd.SetParameterValue("@IsOfferInvoice", productCommonInfo.ProductWarrantyInfo.OfferVATInvoice);
            cmd.SetParameterValue("@Weight", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Weight);
            cmd.SetParameterValue("@IsLarge", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.LargeFlag);
            cmd.SetParameterValue("@Length", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Length);
            cmd.SetParameterValue("@Width", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Width);
            cmd.SetParameterValue("@Height", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Height);
            cmd.SetParameterValue("@MinPackNumber", productCommonInfo.ProductPOInfo.MinPackNumber);
            cmd.SetParameterValue("@InUser", productCommonInfo.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", productCommonInfo.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", productCommonInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", productCommonInfo.LanguageCode);
            cmd.SetParameterValue("@ProductDesc", productCommonInfo.ProductBasicInfo.ShortDescription.Content);
            cmd.ExecuteNonQuery();
            productCommonInfo.ProductCommonInfoSysNo = (int)cmd.GetParameterValue("@SysNo");
        }

        private void InsertProductCommonInfoEx(ProductCommonInfo productCommonInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductCommonInfoEx");
            cmd.SetParameterValue("@SysNo", productCommonInfo.ProductCommonInfoSysNo);
            cmd.SetParameterValue("@Performance", productCommonInfo.ProductBasicInfo.Performance);
            cmd.SetParameterValue("@ProductDescLong", productCommonInfo.ProductBasicInfo.LongDescription.Content);
            cmd.SetParameterValue("@ProductPhotoDesc", productCommonInfo.ProductBasicInfo.PhotoDescription.Content);
            cmd.ExecuteNonQuery();
        }

        private void InsertProductCommonInfoStatus(ProductCommonInfo productCommonInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductCommonInfoStatus");
            cmd.SetParameterValue("@CommonSKUNumber", productCommonInfo.ProductBasicInfo.CommonSkuNumber);
            cmd.SetParameterValue("@IsChangeStyleWithTemplate", "Y");
            cmd.SetParameterValue("@IsExtendWarrantyDisuse", "N");
            cmd.SetParameterValue("@IsBasicInformationCorrect", "Y");
            cmd.SetParameterValue("@IsAccessoriesShow", productCommonInfo.ProductBasicInfo.IsAccessoryShow);
            cmd.SetParameterValue("@IsAccessoriesCorrect", "Y");
            cmd.SetParameterValue("@IsVirtualPic", productCommonInfo.ProductBasicInfo.IsVirtualPic);
            cmd.SetParameterValue("@IsPictureCorrect", "Y");
            cmd.SetParameterValue("@IsWarrantyCorrect", "Y");
            cmd.SetParameterValue("@IsWarrantyShow", productCommonInfo.ProductWarrantyInfo.WarrantyShow);
            cmd.SetParameterValue("@IsWeightCorrect", "Y");
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoBasicInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoBasicInfo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@ProductTitle", productCommonInfo.ProductBasicInfo.ProductTitle.Content);
            dc.SetParameterValue("@ProductBriefName", productCommonInfo.ProductBasicInfo.ProductBriefName);
            dc.SetParameterValue("@Keywords", productCommonInfo.ProductBasicInfo.Keywords.Content);
            dc.SetParameterValue("@ProductModel", productCommonInfo.ProductBasicInfo.ProductModel.Content);
            dc.SetParameterValue("@ProductType", productCommonInfo.ProductBasicInfo.ProductType);
            dc.SetParameterValue("@PMUserSysNo", productCommonInfo.ProductBasicInfo.ProductManager.SysNo);
            dc.SetParameterValue("@ProductDescription", productCommonInfo.ProductBasicInfo.ShortDescription.Content);
            dc.SetParameterValue("@PackageList", productCommonInfo.ProductBasicInfo.PackageList.Content);
            dc.SetParameterValue("@ProductLink", productCommonInfo.ProductBasicInfo.ProductLink);
            dc.SetParameterValue("@Attention", productCommonInfo.ProductBasicInfo.Attention.Content);
            dc.SetParameterValue("@IsTakePicture", productCommonInfo.ProductBasicInfo.IsTakePicture);
            dc.SetParameterValue("@UPCCode", productCommonInfo.ProductBasicInfo.UPCCode);
            dc.SetParameterValue("@BMCode", productCommonInfo.ProductBasicInfo.BMCode);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.SetParameterValue("@EditUserSysNo", operationUser.SysNo);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoPMInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoPMInfo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@PMUserSysNo", productCommonInfo.ProductBasicInfo.ProductManager.SysNo);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoGroupSysNo(int productGroupSysNo, ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductCommonInfoGroupSysNo");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            cmd.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoDescription(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoDescription");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@ProductDescLong", productCommonInfo.ProductBasicInfo.LongDescription.Content);
            dc.SetParameterValue("@ProductPhotoDesc", productCommonInfo.ProductBasicInfo.PhotoDescription.Content);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoWarrantyInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoWarrantyInfo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@HostWarrantyDay", productCommonInfo.ProductWarrantyInfo.HostWarrantyDay);
            dc.SetParameterValue("@PartWarrantyDay", productCommonInfo.ProductWarrantyInfo.PartWarrantyDay);
            dc.SetParameterValue("@Warranty", productCommonInfo.ProductWarrantyInfo.Warranty.Content);
            dc.SetParameterValue("@ServicePhone", productCommonInfo.ProductWarrantyInfo.ServicePhone);
            dc.SetParameterValue("@ServiceInfo", productCommonInfo.ProductWarrantyInfo.ServiceInfo);
            dc.SetParameterValue("@IsOfferInvoice", productCommonInfo.ProductWarrantyInfo.OfferVATInvoice);
            dc.SetParameterValue("@IsWarrantyShow", productCommonInfo.ProductWarrantyInfo.WarrantyShow);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.SetParameterValue("@EditId", operationUser.SysNo);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoDimensionInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoDimensionInfo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@Weight", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Weight);
            dc.SetParameterValue("@IsLarge", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.LargeFlag);
            dc.SetParameterValue("@Length", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Length);
            dc.SetParameterValue("@Width", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Width);
            dc.SetParameterValue("@Height", productCommonInfo.ProductBasicInfo.ProductDimensionInfo.Height);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoNote(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoNote");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@Note", productCommonInfo.ProductBasicInfo.Note);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoPerformance(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoPerformance");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@Performance", productCommonInfo.ProductBasicInfo.Performance);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoIsAccessoryShow(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoIsAccessoryShow");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@IsAccessoryShow", productCommonInfo.ProductBasicInfo.IsAccessoryShow);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void UpdateProductCommonInfoIsVirtualPic(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductCommonInfoIsVirtualPic");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            dc.SetParameterValue("@IsVirtualPic", productCommonInfo.ProductBasicInfo.IsVirtualPic);
            dc.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            dc.ExecuteNonQuery();
        }

        public void InsertProductCommonInfoResource(int productCommonInfoSysNo, ProductResourceForNewegg productResource)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductCommonInfoResource");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            dc.SetParameterValue("@ResourceSysNo", productResource.Resource.ResourceSysNo);
            dc.SetParameterValue("@Status", productResource.IsShow);
            dc.SetParameterValue("@InUser", productResource.OperateUser.UserDisplayName);
            dc.SetParameterValue("@CompanyCode", productResource.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productResource.LanguageCode);
            dc.SetParameterValue("@StoreCompanyCode", productResource.CompanyCode);
            dc.ExecuteNonQuery();
        }

        public void DeleteProductCommonInfoResourceImage(int productCommonInfoSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductCommonInfoResourceImage");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            dc.ExecuteNonQuery();
        }

        public int InsertProductCommonInfoProperty(int productCommonInfoSysNo, int? categorySysNo, ProductProperty productProperty)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertProductCommonInfoProperty");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            int propertyGroupSysNo = 0;
            if (productProperty.PropertyGroup == null || !productProperty.PropertyGroup.SysNo.HasValue)
            {
                //添加Similar Product时无法取到ProductPropertyGroup信息，因此这里重新取
                if (productProperty.Property.PropertyInfo.SysNo.HasValue)
                {
                    propertyGroupSysNo =
                        ObjectFactory<IPropertyDA>.Instance
                        .GetPropertyGroupSysNoByPropertySysNo(categorySysNo, productProperty.Property.PropertyInfo.SysNo.Value);
                }
            }
            else
            {
                propertyGroupSysNo = productProperty.PropertyGroup.SysNo.Value;
            }
            dc.SetParameterValue("@GroupSysNo", propertyGroupSysNo);
            dc.SetParameterValue("@PropertySysNo", productProperty.Property.PropertyInfo.SysNo);
            dc.SetParameterValue("@ValueSysNo", productProperty.Property.SysNo.HasValue ? productProperty.Property.SysNo.Value : 0);
            dc.SetParameterValue("@ManualInput", productProperty.PersonalizedValue.Content);
            dc.SetParameterValue("@InUser", productProperty.OperationUser.UserDisplayName);
            dc.SetParameterValue("@InUserSysNo", productProperty.OperationUser.SysNo);
            dc.SetParameterValue("@CompanyCode", productProperty.CompanyCode);
            dc.SetParameterValue("@LanguageCode", productProperty.LanguageCode);
            return dc.ExecuteScalar<int>();
        }

        public void DeleteProductCommonInfoProperty(int productCommonInfoSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductCommonInfoProperty");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 是否存在某个commonSKUNumber码
        /// </summary>
        /// <param name="commonSKUNumber"></param>
        /// <returns></returns>
        public int GetProductCommonInfoByCommonInfoSkuNumber(string commonSKUNumber)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCommonInfoByCommonInfoSkuNumber");
            cmd.SetParameterValue("@CommonSKUNumber", commonSKUNumber);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return -1;
            }
            var sysNo = Convert.ToInt32(value);
            return sysNo;
        }

        #region IProductCommonInfoDA Members

        /// <summary>
        /// 修改商品描述
        /// </summary>
        /// <param name="productCommonInfo"></param>
        /// <param name="operationUser"></param>
        public void UpdateProductCommonInfoDesc(ProductCommonInfo productCommonInfo, UserInfo operationUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductCommonInfoDesc");
            cmd.SetParameterValue("@Description", productCommonInfo.ProductBasicInfo.ShortDescription.Content);
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfo.ProductCommonInfoSysNo);
            cmd.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            cmd.ExecuteNonQuery();

        }

        #endregion
    }
}
