using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Product;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Shipping;
using ECommerce.Enums;
using ECommerce.Facade.Product.Models;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;


namespace ECommerce.DataAccess.Product
{
    public class ProductDA
    {
        public static List<FrontProductCategoryInfo> GetFrontProductCategory(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFrontProductCategory");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteEntityList<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 获取商品基本信息(商品详情页专用)
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductBasicInfo GetProductBasicInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductBasicInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductBasicInfo>();
        }

        /// <summary>
        /// 商品简要信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductItemInfo GetProductShortInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductShortInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductItemInfo>();
        }

        /// <summary>
        /// 获取商品销售信息(库存以及销售价格)
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductSalesInfo GetProductSalesInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductSalesInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductSalesInfo>();
        }

        /// <summary>
        /// 商品配送方式
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ShipTypeInfo GetProductShippingInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductShippingInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ShipTypeInfo>();
        }

        /// <summary>
        /// 获取所有商品组图片
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <returns></returns>
        public static List<ProductImage> GetProductGroupImageList(ProductInfoFilter filter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Product_GetProductGroupImageList");
            command.SetParameterValue("@ProductCommonInfoSysNo", filter.ProductCommonInfoSysNo);
            command.SetParameterValue("@LanguageCode", filter.LaguageCode);
            command.SetParameterValue("@CompanyCode", filter.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);

            return command.ExecuteEntityList<ProductImage>();

        }

        /// <summary>
        /// 获取商品组属性
        /// </summary>
        /// <param name="productCommonSysNo"></param>
        /// <returns></returns>
        public static List<ProductPropertyInfo> GetProductPropety(ProductInfoFilter filter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Product_GetProductProperty");
            command.SetParameterValue("@ProductCommonSysNo", filter.ProductCommonInfoSysNo);
            command.SetParameterValue("@LanguageCode", filter.LaguageCode);
            command.SetParameterValue("@CompanyCode", filter.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);
            return command.ExecuteEntityList<ProductPropertyInfo>();

        }

        /// <summary>
        /// 商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductAttachmentList(ProductInfoFilter filter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Product_GetProductAttachmentList");
            command.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
            command.SetParameterValue("@LanguageCode", filter.LaguageCode);
            command.SetParameterValue("@CompanyCode", filter.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);

            return command.ExecuteEntityList<ProductItemInfo>();

        }


        /// <summary>
        /// 商品浏览记录
        /// </summary>
        /// <param name="sysNos"></param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductBrowseHistoryInfoBySysNos(List<string> sysNos)
        {

            string nos = string.Join(",", sysNos.ToArray());

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Product_GetProductBrowseHistoryInfoBySysNos");

            cmd.CommandText = cmd.CommandText.Replace("@ProductSysNosWhere", string.IsNullOrEmpty(nos) ? nos : string.Format("Product.SysNo IN({0}) AND", nos));

            return cmd.ExecuteEntityList<ProductItemInfo>();


        }

        /// <summary>
        /// buy also buy
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static string GetProductBuyAlsoBuy(ProductInfoFilter filter)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductBuyAlsoBuy");
            cmd.SetParameterValue("@ProductID", filter.ProductSysNo);
            cmd.SetParameterValue("@LanguageCode", filter.LaguageCode);
            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);
            return cmd.ExecuteScalar<string>();
        }

        /// <summary>
        /// browse also buy
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static string GetProductBrowseAlsoBuy(ProductInfoFilter filter)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductBrowseAlsoBuy");
            cmd.SetParameterValue("@ProductID", filter.ProductID);
            cmd.SetParameterValue("@LanguageCode", filter.LaguageCode);
            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);
            return cmd.ExecuteScalar<string>();
        }

        /// <summary>
        /// 根据商品ID获取商品
        /// </summary>
        /// <param name="productIDs">productIDs</param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductCellInfoListByIDs(List<string> productIDs)
        {
            string nos = string.Join(",", productIDs.ToArray());
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Product_GetProductCellInfoListByIDs");

            cmd.CommandText = cmd.CommandText.Replace("#StrWhere#", string.IsNullOrEmpty(nos) ? nos : string.Format("Product.SysNo IN({0}) AND", nos));
            //cmd.SetParameterValue("@ProductIDs",  );
            return cmd.ExecuteEntityList<ProductItemInfo>();
        }

        /// <summary>
        /// 商品相关品牌
        /// </summary>
        /// <param name="prodcutSysNo"></param>
        /// <returns></returns>
        public static List<BrandInfo> GetProductRelatedBrandInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductRelatedBrandInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteEntityList<BrandInfo>();

        }

        /// <summary>
        /// 获取C1的推荐品牌
        /// </summary>
        /// <param name="ECCategory1Id"></param>
        /// <returns></returns>
        public static List<BrandInfo> GetRecommendBrandForECC1(int ECCategory1Id)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_RecommendBrandForECC1");
            cmd.SetParameterValue("@ECC1ID", ECCategory1Id);
            return cmd.ExecuteEntityList<BrandInfo>();

        }



        public static bool IsProductWished(int productSysNo, int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_IsProductWished");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);

            if (cmd.ExecuteScalar<int>() > 0)
            {
                return true;
            }
            return false;
        }

        public static List<BrandInfoExt> GetAllBrands()
        {
            var cmd = DataCommandManager.GetDataCommand("Product_GetAllBrands");
            return cmd.ExecuteEntityList<BrandInfoExt>();
        }


        public static BrandInfo GetBrandBySysNo(int Sysno)
        {
            var cmd = DataCommandManager.GetDataCommand("Product_GetBrandBySysNo");
            cmd.SetParameterValue("@SysNo", Sysno);
            return cmd.ExecuteEntity<BrandInfo>();
        }


        public static List<ProductCompareInfo> GetProductCompareList(List<string> itemList, string companCode, string languageCode, string storeCompanyCode)
        {
            //DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductCompareList");
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Product_GetProductCompareList");

            // cmd.SetParameterValue("@ItemList", string.Join(",", itemList.ToArray()));
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companCode);
            cmd.SetParameterValue("@StoreCompanyCode", storeCompanyCode);
            //string sqlWhere = string.Empty;
            //for (int i=0;i< itemList.Count; i++)
            //{
            //    sqlWhere += string.Format("'{0}'", itemList[i]) + (i == itemList.Count-1 ? " " : ",");
            //}
            cmd.CommandText = cmd.CommandText.Replace("#StrInWhere#", string.Format(" Product.SysNo IN({0}) ", string.Join(",", itemList.ToArray())));

            return cmd.ExecuteEntityList<ProductCompareInfo>();
        }


        /// <summary>
        /// 商品配件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductAccessories> GetProductAccessoriesList(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductAccessoriesList");
            cmd.SetParameterValue("ProductSysNo", productSysNo);
            return cmd.ExecuteEntityList<ProductAccessories>();

        }

        /// <summary>
        /// 促销模板
        /// </summary>
        /// <param name="sysNo">促销模板编号</param>
        /// <returns></returns>
        public static SaleAdvertisement GetSaleAdvertisementInfo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_SaleAdvertisement");
            cmd.SetParameterValue("@sysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            SaleAdvertisement saleAdvertisementInfo = new SaleAdvertisement();
            DataTable dtSaleAdvertisement = ds.Tables[0];
            //DataTable dtSaleAdvertisementGroup = ds.Tables[1];
            //DataTable dtSaleAdvertisementItem = ds.Tables[2];

            if (dtSaleAdvertisement != null && dtSaleAdvertisement.Rows.Count > 0)
            {
                //促销模板
                saleAdvertisementInfo = DataMapper.GetEntity<SaleAdvertisement>(dtSaleAdvertisement.Rows[0]);
                //if (dtSaleAdvertisementGroup != null && dtSaleAdvertisementGroup.Rows.Count > 0)
                //{
                //    //促销模板组
                //    saleAdvertisementInfo.SaleAdvertisementGroupList = DataMapper.GetEntityList<SaleAdvertisementGroup, List<SaleAdvertisementGroup>>(dtSaleAdvertisementGroup.Rows);
                //    if (dtSaleAdvertisementItem != null && dtSaleAdvertisementItem.Rows.Count > 0)
                //    {
                //        List<SaleAdvertisementItem> productList = DataMapper.GetEntityList<SaleAdvertisementItem, List<SaleAdvertisementItem>>(dtSaleAdvertisementItem.Rows, (row, entity) => {
                //            entity.IsHaveValidGift = row["IsHaveValidGift"] == null ? false : (row["IsHaveValidGift"].ToString() == "1" ? true : false);
                //        });

                //        saleAdvertisementInfo.SaleAdvertisementGroupList.ForEach(f =>
                //        {
                //            //促销模板商品
                //            f.SaleAdvertisementItemList = productList.FindAll(item => item.GroupSysNo == f.SysNo);
                //        });
                //    }
                //}
            }
            return saleAdvertisementInfo;
        }

        /// <summary>
        /// 商品类别模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static string GetProductCategoryTemplateStr(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductCategoryTemplateStr");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteScalar<string>();
        }

        /// <summary>
        /// 获取商品所有属性
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static List<ProductPropertyInfo> GetProductCategoryTemplatePropertys(int sysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Product_GetProductPropertys");
            cmd.SetParameterValue("@ProductSysNo", sysNo);
            string tempSysNos = GetProductCategoryTemplateStr(sysNo);
            if (!string.IsNullOrWhiteSpace(tempSysNos))
            {
                cmd.CommandText = cmd.CommandText.Replace("#TempSysNos#", tempSysNos);
                return cmd.ExecuteEntityList<ProductPropertyInfo>();
            }
            return null;
        }

        /// <summary>
        /// 商品会员价
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductCustomerRankPrice GetProductCustomerRankPrice(int customerSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductCustomerRankPrice");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductCustomerRankPrice>();

        }

        /// <summary>
        /// 获取商品销售区域省份信息集合
        /// Author:     Ausra
        /// CreateDate: 2015-07-16
        /// </summary>
        /// <param name="productSysID">商品编号</param>
        /// <returns></returns>
        public static List<Area> GetProductAreas(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetRestrictedAreaProvices");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            List<Area> coll = cmd.ExecuteEntityList<Area>();
            return coll;
        }
        /// <summary>
        /// 获取商品销售区域指定省份下的市级信息集合
        /// Author:     Ausra
        /// CreateDate: 2015-07-16
        /// </summary>
        /// <param name="productSysID">商品编号</param>
        /// <param name="proviceSysNo">省份编号</param>
        /// <returns></returns>
        public static List<Area> GetProductCitys(int productSysNo, int proviceSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetRestrictedAreaCitys");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@ProvinceSysNo", proviceSysNo);
            List<Area> coll = cmd.ExecuteEntityList<Area>();
            return coll;
        }

        /// <summary>
        /// 获取商品运费
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="areaSysNo">地区编号</param>
        /// <returns></returns>
        public static List<ProductShippingPrice> GetProductShippingPrice(int productSysNo, int areaSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Product_GetProductShippingPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@AreaSysNo", areaSysNo);
            List<ProductShippingPrice> result = cmd.ExecuteEntityList<ProductShippingPrice>();
            return result;
        }
    }
}
