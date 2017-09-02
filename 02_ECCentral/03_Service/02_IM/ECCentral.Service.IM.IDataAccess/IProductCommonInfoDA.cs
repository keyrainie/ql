using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductCommonInfoDA
    {
        void InsertProductCommonInfo(int? productGroupSysNo, ProductInfo productCommonInfo);

        void UpdateProductCommonInfoBasicInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoPMInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoGroupSysNo(int productGroupSysNo, ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoDescription(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoWarrantyInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoDimensionInfo(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoNote(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoPerformance(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoIsAccessoryShow(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        void UpdateProductCommonInfoIsVirtualPic(ProductCommonInfo productCommonInfo, UserInfo operationUser);

        int GetProductCommonInfoByCommonInfoSkuNumber(string commonSKUNumber);

        #region ProductCommonInfoRelationObjectDataAccess

        void InsertProductCommonInfoResource(int productCommonInfoSysNo, ProductResourceForNewegg productAccessory);

        void DeleteProductCommonInfoResourceImage(int productCommonInfoSysNo);

        int InsertProductCommonInfoProperty(int productCommonInfoSysNo, int? categorySysNo, ProductProperty productProperty);

        void DeleteProductCommonInfoProperty(int productCommonInfoSysNo);

        void UpdateProductCommonInfoDesc(ProductCommonInfo productCommonInfo, UserInfo operationUser);
        #endregion


    }
}
