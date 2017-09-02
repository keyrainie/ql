using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;


namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductPriceDA))]
    public class ProductPriceDA : IProductPriceDA
    {
        public void UpdateProductBasicPrice(int productSysNo, ProductPriceInfo productPriceInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductBasicPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@BasicPrice", productPriceInfo.BasicPrice);
            cmd.SetParameterValue("@PointType", productPriceInfo.PayType);
            cmd.SetParameterValue("@MaxPerOrder", productPriceInfo.MaxCountPerDay);
            cmd.SetParameterValue("@MinCountPerOrder", productPriceInfo.MinCountPerOrder);
            cmd.SetParameterValue("@MinCommission", productPriceInfo.MinCommission);
            cmd.SetParameterValue("@Discount",
                                  productPriceInfo.BasicPrice != 0
                                      ? (productPriceInfo.CurrentPrice/productPriceInfo.BasicPrice).Round(2)
                                      : 0m);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductPrice(int productSysNo, ProductPriceInfo productPriceInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@BasicPrice", productPriceInfo.BasicPrice);
            cmd.SetParameterValue("@CurrentPrice", productPriceInfo.CurrentPrice);
            cmd.SetParameterValue("@CashRebate", productPriceInfo.CashRebate);
            cmd.SetParameterValue("@Point", productPriceInfo.Point);
            cmd.SetParameterValue("@IsWholeSale", productPriceInfo.ProductWholeSalePriceInfo.Any(p=>p.Qty!=null) ? 1 : 0);
            cmd.SetParameterValue("@Discount", productPriceInfo.BasicPrice==0?0:(productPriceInfo.CurrentPrice / productPriceInfo.BasicPrice).Round(2));
            cmd.SetParameterValue("@IsExistRankPrice", productPriceInfo.ProductRankPrice.Any(rankPrice => rankPrice.Status == ProductRankPriceStatus.Active) ? 1 : 0);
            cmd.SetParameterValue("@MinCommission", productPriceInfo.MinCommission);
            for (int i = 0; i < Enum.GetValues(typeof(WholeSaleLevelType)).Cast<WholeSaleLevelType>().Count(); i++)
            {
                if (productPriceInfo.ProductWholeSalePriceInfo.Count > i)
                {
                    cmd.SetParameterValue("@Q" + (i + 1), productPriceInfo.ProductWholeSalePriceInfo[i].Qty);
                    cmd.SetParameterValue("@P" + (i + 1), productPriceInfo.ProductWholeSalePriceInfo[i].Price);
                }
                else
                {
                    cmd.SetParameterValue("@Q" + (i + 1), null);
                    cmd.SetParameterValue("@P" + (i + 1), null);
                }
            }

            cmd.ExecuteNonQuery();
            DeleteProductRankPrice(productSysNo);
            InsertProductRankPrice(productSysNo, productPriceInfo);
            UpdateProductVipPrice(productSysNo, productPriceInfo);
        }

        private void UpdateProductVipPrice(int productSysNo, ProductPriceInfo productPriceInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductVipPrice");

            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@IsUseAlipayVipPrice", productPriceInfo.IsUseAlipayVipPrice);
            cmd.SetParameterValue("@AlipayVipPrice", productPriceInfo.AlipayVipPrice);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductVirtualPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@OldVirtualPrice", originalVirtualPrice);
            cmd.SetParameterValue("@NewVirtualPrice", newVirtualPrice);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductBasicPriceOnly(int productSysNo, decimal newPrice)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductBasicPriceOnly");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@BasicPrice", newPrice);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductCurrentPriceOnly(int productSysNo, decimal newPrice)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductCurrentPriceOnly");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CurrentPrice", newPrice);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProductSyncShopPrice(int productSysNo, IsSyncShopPrice isSyncShopPrice)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductSyncShopPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@IsSyncShopPrice", (int)isSyncShopPrice);
            cmd.ExecuteNonQuery();
        }

        private void DeleteProductRankPrice(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteProductRankPrice");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
        }

        private void InsertProductRankPrice(int productSysNo, ProductPriceInfo productPriceInfo)
        {
            foreach (var rankPrice in productPriceInfo.ProductRankPrice.Where(rankPrice => rankPrice.RankPrice.HasValue && rankPrice.Status.HasValue))
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductRankPrice");
                cmd.SetParameterValue("@ProductSysNo", productSysNo);
                cmd.SetParameterValue("@CustomerRank", rankPrice.Rank);
                cmd.SetParameterValue("@RankPrice", rankPrice.RankPrice);
                cmd.SetParameterValue("@Status", rankPrice.Status);
                cmd.SetParameterValue("@EditUserSysNo", ServiceContext.Current.UserSysNo);
                cmd.ExecuteNonQuery();
            }
        }

        public List<ProductRankPriceInfo> GetProductRankPriceBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRankPriceBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = cmd.ExecuteEntityList<ProductRankPriceInfo>();

            var customerRankList = Enum.GetValues(typeof(CustomerRank)).Cast<CustomerRank>().ToList();
            customerRankList.Remove(CustomerRank.Ferrum);
            var rresult = customerRankList.GroupJoin(sourceEntity,
                rank => rank,
                productPriceCustomerRank => productPriceCustomerRank.Rank,
                (rank, rresultList) => new { rank, rresultList }).SelectMany(
                @r => @r.rresultList.DefaultIfEmpty(), (@t, r) => new
                {
                    CustomerRank = @t.rank,
                    RankPrice = (r == null) ? (decimal?)null : r.RankPrice,
                    Status = (r == null) ? (ProductRankPriceStatus?)null : r.Status
                });

            var resultProductRankPriceList = rresult.Select(rankPrice =>
                new ProductRankPriceInfo
                {
                    Rank = rankPrice.CustomerRank,
                    RankPrice = rankPrice.RankPrice,
                    Status = rankPrice.Status
                }).ToList();
            resultProductRankPriceList = resultProductRankPriceList.OrderBy(p => p.Rank).ToList();
            return resultProductRankPriceList;
        }

        public List<ProductWholeSalePriceInfo> GetWholeSalePriceInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetWholeSalePriceInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var priceArray = new List<ProductWholeSalePriceInfo>();
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    var price = new ProductWholeSalePriceInfo();
                    var value = reader.GetInt32(0);
                    WholeSaleLevelType level;
                    Enum.TryParse(value.ToString(CultureInfo.InvariantCulture), out level);
                    price.Level = level;
                    if (!reader.IsDBNull(1) && !reader.IsDBNull(2))
                    {
                        price.Qty = reader.GetInt32(1);
                        price.Price = reader.GetDecimal(2);
                        priceArray.Add(price);
                    }
                }
            }

            var volumePriceList = Enum.GetValues(typeof(WholeSaleLevelType)).Cast<WholeSaleLevelType>().ToList();
            var vresult = volumePriceList.GroupJoin(priceArray,
                volume => volume,
                productVolumeLevelType => productVolumeLevelType.Level,
                (volume, vresultList) => new { volume, vresultList }).SelectMany(
                @v => @v.vresultList.DefaultIfEmpty(), (@v, o) => new
                {
                    Level = @v.volume,
                    Qty = (o == null) ? (int?)null : o.Qty,
                    Price = (o == null) ? (decimal?)null : o.Price
                });

            var resultProductVolumePriceList = vresult.Select(volumePrice =>
                new ProductWholeSalePriceInfo
                {
                    Level = volumePrice.Level,
                    Qty = volumePrice.Qty,
                    Price = volumePrice.Price
                }).ToList();
            resultProductVolumePriceList = resultProductVolumePriceList.OrderBy(p => p.Level).ToList();
            return resultProductVolumePriceList;
        }

        public PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPriceChangeLogInfoByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StartTime", startTime);
            cmd.SetParameterValue("@EndTime", endTime);
            var sourceEntity = cmd.ExecuteEntity<PriceChangeLogInfo>();
            return sourceEntity;
        }
    }
}
