using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductEntryInfoDA
    {
        //bool InsertEntryInfo(ProductEntryInfo entity);


        bool UpdateEntryInfo(ProductEntryInfo entity);


        List<ProductEntryInfo> GetProductEntryInfoList(List<int> productSysNoList);

        bool AuditSucess(ProductEntryInfo info);

        bool AuditFail(ProductEntryInfo info);

        bool ToInspection(ProductEntryInfo info);

        bool InspectionSucess(ProductEntryInfo info);

        bool InspectionFail(ProductEntryInfo info);

        bool ToCustoms(ProductEntryInfo info);

        bool CustomsSuccess(ProductEntryInfo info);

        bool CustomsFail(ProductEntryInfo info);

        bool checkInspectionNum(string InspectionNum);

        #region 申报商品
        /// <summary>
        /// 申报时，获取不同状态下的商品信息
        /// </summary>
        /// <param name="entryStatus">商品备案状态</param>
        /// <param name="entryStatusEx">商品备案扩展状态</param>
        /// <returns></returns>
        List<WaitDeclareProduct> GetProduct(ProductEntryStatus entryStatus, ProductEntryStatusEx? entryStatusEx);
        /// <summary>
        /// 申报时获取申报商品详细信息
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        List<ProductDeclare> DeclareGetProduct(List<WaitDeclareProduct> products);
        /// <summary>
        /// 申报商品成功（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        bool ProductCustomsSuccess(ProductEntryInfo entryInfo);
        /// <summary>
        /// 申报商品失败（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        bool ProductCustomsFail(ProductEntryInfo entryInfo);
        #endregion
    }
}
