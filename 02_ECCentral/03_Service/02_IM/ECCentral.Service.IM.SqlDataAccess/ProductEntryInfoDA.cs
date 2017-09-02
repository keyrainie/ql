using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductEntryInfoDA))]
    public class ProductEntryInfoDA : IProductEntryInfoDA
    {
        /// <summary>
        /// 创建商品备案信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public bool InsertEntryInfo(ProductEntryInfo entity)
        //{
        //    DataCommand cmd = DataCommandManager.GetDataCommand("InsertEntryInfo");
        //    ///如果业务类型是直邮中国商品(一般进口)，则申报关区为直邮中国商品（2244）
        //    ///如果是自贸专区商品（保税进口),则申报关区为自贸专区商品（2216）
        //    if (entity.BizType == EntryBizType.NormalImport)
        //    {
        //        entity.ApplyDistrict = 2244;
        //    }

        //    if (entity.BizType == EntryBizType.BondedImport)
        //    {
        //        entity.ApplyDistrict = 2216;
        //    }

        //    cmd.SetParameterValue<ProductEntryInfo>(entity);
        //    return cmd.ExecuteNonQuery() > 0;
        //}


        /// <summary>
        /// 更新商品备案信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateEntryInfo(ProductEntryInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateEntryInfo");
            if (entity.BizType == EntryBizType.NormalImport)
            {
                entity.ApplyDistrict = 2244;
            }

            if (entity.BizType == EntryBizType.BondedImport)
            {
                entity.ApplyDistrict = 2216;
            }
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@ProductName_EN", entity.ProductName_EN);
            cmd.SetParameterValue("@Specifications", entity.Specifications);
            cmd.SetParameterValue("@Functions", entity.Functions);
            cmd.SetParameterValue("@Component", entity.Component);
            cmd.SetParameterValue("@Origin", entity.Origin);
            cmd.SetParameterValue("@Purpose", entity.Purpose);
            cmd.SetParameterValue("@TaxQty", entity.TaxQty);
            cmd.SetParameterValue("@TaxUnit", entity.TaxUnit);
            cmd.SetParameterValue("@ApplyUnit", entity.ApplyUnit);
            cmd.SetParameterValue("@GrossWeight", entity.GrossWeight);
            cmd.SetParameterValue("@BizType", entity.BizType);
            cmd.SetParameterValue("@ApplyDistrict", entity.ApplyDistrict);
            cmd.SetParameterValue("@Product_SKUNO", entity.Product_SKUNO);
            cmd.SetParameterValue("@Supplies_Serial_No", entity.Supplies_Serial_No);
            cmd.SetParameterValue("@ApplyQty", entity.ApplyQty);
            cmd.SetParameterValue("@SuttleWeight", entity.SuttleWeight);
            cmd.SetParameterValue("@Note", entity.Note);
            cmd.SetParameterValue("@Supplies_Serial_No_1", entity.Supplies_Serial_No_1);
            cmd.SetParameterValue("@TariffRate", entity.TariffRate);
            cmd.SetParameterValue("@TariffCode", entity.TariffCode);
            cmd.SetParameterValue("@StoreType", entity.StoreType);
            cmd.SetParameterValue("@EntryCode", entity.EntryCode);
            cmd.SetParameterValue("@Remark1", entity.Remark1);
            cmd.SetParameterValue("@Remark2", entity.Remark2);
            cmd.SetParameterValue("@Remark3", entity.Remark3);
            cmd.SetParameterValue("@Remark4", entity.Remark4);
            cmd.SetParameterValue("@ProductOthterName", entity.ProductOthterName);
            cmd.SetParameterValue("@ManufactureDate", entity.ManufactureDate);
            cmd.SetParameterValue("@DefaultLeadTimeDays", entity.DefaultLeadTimeDays);
            cmd.SetParameterValue("@NeedValid", entity.NeedValid);
            cmd.SetParameterValue("@NeedLabel", entity.NeedLabel);
            cmd.SetParameterValue("@HSCode", entity.HSCode);
            cmd.SetParameterValue("@NotProhibitedEntry", entity.NotProhibitedEntry);
            cmd.SetParameterValue("@NotInNotice1712", entity.NotInNotice1712);
            cmd.SetParameterValue("@NotTransgenic", entity.NotTransgenic);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<ProductEntryInfo> GetProductEntryInfoList(List<int> productSysNoList)
        {
            List<ProductEntryInfo> result = new List<ProductEntryInfo>();
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductEntryInfo");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd, "EntryInfo.[SysNo]"))
            {
                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "EntryInfo.[ProductSysNo]", DbType.Int32, productSysNoList);
                cmd.CommandText = builder.BuildQuerySql();

                result = cmd.ExecuteEntityList<ProductEntryInfo>();
            }
            return result;
        }

        public bool AuditSucess(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryAuditSucess");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            cmd.SetParameterValue("@InspectionNum", info.InspectionNum);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool AuditFail(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryAuditFail");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ToInspection(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryToInspection");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool InspectionSucess(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryInspectionSucess");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool InspectionFail(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryInspectionFail");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ToCustoms(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryToCustoms");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool CustomsSuccess(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryCustomsSuccess");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool CustomsFail(ProductEntryInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("EntryCustomsFail");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo.Value);
            cmd.SetParameterValue("@AuditNote", info.AuditNote);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool checkInspectionNum(string InspectionNum)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("checkInspectionNum");
            cmd.SetParameterValue("@InspectionNum", InspectionNum);
            return cmd.ExecuteNonQuery() > 0;
        }

        #region 申报商品
        /// <summary>
        /// 申报时，获取不同状态下的商品信息
        /// </summary>
        /// <param name="entryStatus">商品备案状态</param>
        /// <param name="entryStatusEx">商品备案扩展状态</param>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetProduct(ProductEntryStatus entryStatus, ProductEntryStatusEx? entryStatusEx)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductEntry_Declare_GetProduct");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd, "pei.SysNo"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pei.EntryStatus", DbType.Int32, "@EntryStatus", QueryConditionOperatorType.Equal, entryStatus);
                if (entryStatusEx.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pei.EntryStatusEx", DbType.Int32, "@EntryStatusEx", QueryConditionOperatorType.Equal, entryStatusEx);
                }
                cmd.CommandText = builder.BuildQuerySql();
            }
            return cmd.ExecuteEntityList<WaitDeclareProduct>();
        }
        /// <summary>
        /// 申报时获取申报商品详细信息
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public List<ProductDeclare> DeclareGetProduct(List<WaitDeclareProduct> products)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductEntry_Declare_GetProductDetail");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd, "pei.SysNo"))
            {
                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, @"pei.ProductSysNo", DbType.Int32, products.Select(t => t.ProductSysNo).ToList());
                cmd.CommandText = builder.BuildQuerySql();
            }
            return cmd.ExecuteEntityList<ProductDeclare>();
        }
        /// <summary>
        /// 申报商品成功（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        public bool ProductCustomsSuccess(ProductEntryInfo entryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductEntry_ProductCustomsSuccess");
            entryInfo.EntryStatus = ProductEntryStatus.EntrySuccess;
            entryInfo.EntryStatusEx = ProductEntryStatusEx.CustomsSuccess;
            cmd.SetParameterValue<ProductEntryInfo>(entryInfo);
            return cmd.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 申报商品失败（用于第三方回调处理）
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        public bool ProductCustomsFail(ProductEntryInfo entryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductEntry_ProductCustomsFail");
            entryInfo.EntryStatus = ProductEntryStatus.EntryFail;
            entryInfo.EntryStatusEx = ProductEntryStatusEx.CustomsFail;
            cmd.SetParameterValue<ProductEntryInfo>(entryInfo);
            return cmd.ExecuteNonQuery() > 0;
        }
        #endregion
    }

}
