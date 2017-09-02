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
using IPP.CN.IM.Service.Common.Utility;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductPriceRequestDA))]
    public class ProductPriceRequestDA : IProductPriceRequestDA
    {
        /// <summary>
        /// 更新商品价格审核申请状态
        /// </summary>
        /// <param name="productPriceRequest"></param>
        public void UpdateProductPriceRequestStatus(ProductPriceRequestInfo productPriceRequest)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductPriceRequestStatus");
            cmd.SetParameterValue("@ProductPriceRequestSysNo", productPriceRequest.SysNo);
            cmd.SetParameterValue("@ProductPriceRequestStatus", productPriceRequest.RequestStatus != null ? productPriceRequest.RequestStatus.Value : 0);
            cmd.SetParameterValue("@PMDMemo", productPriceRequest.PMDMemo);
            cmd.SetParameterValue("@TLMemo", productPriceRequest.TLMemo);
            cmd.SetParameterValue("@AuditUserSysNo", productPriceRequest.AuditUser == null ? null : productPriceRequest.AuditUser.SysNo);
            cmd.SetParameterValue("@FinalAuditUserSysNo", productPriceRequest.FinalAuditUser == null ? null : productPriceRequest.FinalAuditUser.SysNo);
            cmd.ExecuteNonQuery();
            if (productPriceRequest.RequestStatus == ProductPriceRequestStatus.Approved)
            {
                if (productPriceRequest.SysNo != null)
                {
                    UpdateProductPriceByRequestInfoSysNo(productPriceRequest);
                }
            }
        }


        /// <summary>
        /// 根据PriceRequestSysNo获取商品价格变动信息
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        public ProductPriceRequestInfo GetProductPriceRequestInfoBySysNo(int productPriceRequestSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductPriceRequestInfoBySysNo");
            cmd.SetParameterValue("@SysNo", productPriceRequestSysNo);
            var entity = cmd.ExecuteEntity<ProductPriceRequestInfo>();
            if (entity != null)
            {
                entity.ProductRankPrice = GetProductRankPriceRequestBySysNo(productPriceRequestSysNo);
                entity.ProductWholeSalePriceInfo = GetWholeSalePriceRequestInfoBySysNo(productPriceRequestSysNo);
                var productSysNo = GetProductSysNoBySysNo(productPriceRequestSysNo);
                if (productSysNo > 0)
                {
                    var productPriceDA = ObjectFactory<IProductPriceDA>.Instance;
                    entity.OldPrice.ProductRankPrice = productPriceDA.GetProductRankPriceBySysNo(productSysNo);
                    entity.OldPrice.ProductWholeSalePriceInfo = productPriceDA.GetWholeSalePriceInfoBySysNo(productSysNo);
                }
            }
            return entity;
        }

        /// <summary>
        /// 获取当前商品会员价格更改请求
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductRankPriceInfo> GetProductRequestRankPriceBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRequestRankPriceBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = cmd.ExecuteEntityList<ProductRankPriceInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 获取当前商品批发价格更改请求
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductWholeSalePriceInfo> GetProductRequestWholeSalePriceInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRequestWholeSalePriceInfoBySysNo");
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

            return resultProductVolumePriceList;
        }

        public ProductPriceRequestInfo GetProductLastProductPriceRequestInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductLastProductPriceRequestInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            var sourceEntity = cmd.ExecuteEntity<ProductPriceRequestInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 根据PriceRequestSysNo获取商品编号
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        public int GetProductSysNoBySysNo(int productPriceRequestSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductSysNoBySysNo");
            cmd.SetParameterValue("@SysNo", productPriceRequestSysNo);
            var entity = cmd.ExecuteScalar();
            return entity is DBNull ? 0 : Convert.ToInt32(entity);
        }

        /// <summary>
        /// 根据PriceRequestSysNo获取商品价格变动信息
        /// </summary>
        /// <param name="productPriceRequest"></param>
        /// <returns></returns>
        private void UpdateProductPriceByRequestInfoSysNo(ProductPriceRequestInfo productPriceRequest)
        {
            if (productPriceRequest == null || productPriceRequest.SysNo == null || productPriceRequest.SysNo <= 0) return;
            InitProductPriceRequestInfo(productPriceRequest);
            var productSysNo = GetProductSysNoBySysNo(productPriceRequest.SysNo.Value);
            var productPrice = new ProductPriceInfo();
            EntityCopy.CopyProperties(productPriceRequest, productPrice);
            productPrice.ProductWholeSalePriceInfo = productPriceRequest.ProductWholeSalePriceInfo;
            productPrice.ProductRankPrice = productPriceRequest.ProductRankPrice;
            var productPriceDA = ObjectFactory<IProductPriceDA>.Instance;
            productPriceDA.UpdateProductPrice(productSysNo, productPrice);
        }

        public void UpdateProductPriceRequestStatus(int productPriceRequestSysNo, ProductPriceRequestStatus status)
        {
            var productPriceRequest = new ProductPriceRequestInfo
                                                              {
                                                                  RequestStatus = status,
                                                                  SysNo =
                                                                      productPriceRequestSysNo
                                                              };
            UpdateProductPriceRequestStatus(productPriceRequest);
        }

        public void InsertProductPriceRequest(int productSysNo, ProductPriceRequestInfo productPriceRequestInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductPriceRequest");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@BasicPrice", productPriceRequestInfo.BasicPrice);
            cmd.SetParameterValue("@CurrentPrice", productPriceRequestInfo.CurrentPrice);
            cmd.SetParameterValue("@CashRebate", productPriceRequestInfo.CashRebate);
            cmd.SetParameterValue("@Point", productPriceRequestInfo.Point);
            cmd.SetParameterValue("@Status", ProductPriceRequestStatus.Origin);
            cmd.SetParameterValue("@Type", productPriceRequestInfo.AuditType);
            cmd.SetParameterValue("@IsWholeSale", productPriceRequestInfo.ProductWholeSalePriceInfo.Any() ? 1 : 0);


            for (int i = 0; i < Enum.GetValues(typeof(WholeSaleLevelType)).Cast<WholeSaleLevelType>().Count(); i++)
            {
                if (productPriceRequestInfo.ProductWholeSalePriceInfo.Count > i)
                {
                    cmd.SetParameterValue("@Q" + (i + 1), productPriceRequestInfo.ProductWholeSalePriceInfo[i].Qty);
                    cmd.SetParameterValue("@P" + (i + 1), productPriceRequestInfo.ProductWholeSalePriceInfo[i].Price);
                }
                else
                {
                    cmd.SetParameterValue("@Q" + (i + 1), null);
                    cmd.SetParameterValue("@P" + (i + 1), null);
                }
            }
            cmd.SetParameterValue("@IsUseAlipayVipPrice", productPriceRequestInfo.IsUseAlipayVipPrice);
            cmd.SetParameterValue("@AlipayVipPrice", productPriceRequestInfo.AlipayVipPrice);
            cmd.SetParameterValue("@PMMemo", productPriceRequestInfo.PMMemo);
            cmd.SetParameterValue("@CreateUserSysNo", productPriceRequestInfo.CreateUser.SysNo);
            cmd.ExecuteNonQuery();
            productPriceRequestInfo.SysNo = (int)cmd.GetParameterValue("@SysNo");
            InsertRequestRankPrice(productSysNo, productPriceRequestInfo);
        }

        private void InsertRequestRankPrice(int productSysNo, ProductPriceRequestInfo productPriceRequestInfo)
        {
            foreach (var rankPrice in productPriceRequestInfo.ProductRankPrice.Where(rankPrice => rankPrice.RankPrice.HasValue && rankPrice.Status.HasValue))
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("InsertRequestRankPrice");
                cmd.SetParameterValue("@PriceRequestSysNo", productPriceRequestInfo.SysNo);
                cmd.SetParameterValue("@ProductSysNo", productSysNo);
                cmd.SetParameterValue("@CustomerRank", rankPrice.Rank);
                cmd.SetParameterValue("@RankPrice", rankPrice.RankPrice);
                cmd.SetParameterValue("@Status", rankPrice.Status);
                cmd.SetParameterValue("@Type", productPriceRequestInfo.AuditType);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取会员等级价格--价格申请表
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        private List<ProductRankPriceInfo> GetProductRankPriceRequestBySysNo(int productPriceRequestSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRankPriceRequestBySysNo");
            cmd.SetParameterValue("@PriceRequestSysNo", productPriceRequestSysNo);
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

        /// <summary>
        /// 获取批发价格--价格申请表
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        private List<ProductWholeSalePriceInfo> GetWholeSalePriceRequestInfoBySysNo(int productPriceRequestSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetWholeSalePriceRequestInfoBySysNo");
            cmd.SetParameterValue("@PriceRequestSysNo", productPriceRequestSysNo);
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

        /// <summary>
        /// 初始化价格申请表数据
        /// </summary>
        /// <param name="productPriceRequest"></param>
        private void InitProductPriceRequestInfo(ProductPriceRequestInfo productPriceRequest)
        {
            if (productPriceRequest.BasicPrice < 0)
                productPriceRequest.BasicPrice = productPriceRequest.OldPrice.BasicPrice;
            if (productPriceRequest.CurrentPrice == null || productPriceRequest.CurrentPrice < 0)
                productPriceRequest.CurrentPrice = productPriceRequest.OldPrice.CurrentPrice;
            if (productPriceRequest.CashRebate == null || productPriceRequest.CashRebate < 0)
                productPriceRequest.CashRebate = productPriceRequest.OldPrice.CashRebate;
            if (productPriceRequest.Point == null || productPriceRequest.Point < 0)
                productPriceRequest.Point = productPriceRequest.OldPrice.Point;
           
                //productPriceRequest.Point = productPriceRequest.OldPrice.Point;
           
            productPriceRequest.IsExistRankPrice = productPriceRequest.ProductRankPrice != null && productPriceRequest.ProductRankPrice.Any(p=>p.RankPrice>0)?0:1; 
            if (productPriceRequest.ProductWholeSalePriceInfo == null || productPriceRequest.ProductWholeSalePriceInfo.Count == 0)
            {
                productPriceRequest.IsWholeSale = 0;
            }
            else
            {
                productPriceRequest.ProductWholeSalePriceInfo.ForEach(v =>
                                    {
                                        var entity = productPriceRequest.OldPrice
                                                .ProductWholeSalePriceInfo
                                                .Where(k => k.Level == v.Level).FirstOrDefault();
                                        if (v.Qty != null && v.Qty < 0)
                                        {
                                            v.Qty = entity != null ? entity.Qty : null;
                                        }
                                        if (v.Price != null && v.Price < 0)
                                        {
                                            v.Price = entity != null ? entity.Price : null;
                                        }
                                    });
                productPriceRequest.IsWholeSale =productPriceRequest.ProductWholeSalePriceInfo.Any(k=>k.Price>0)?1:0;
            }
        }
    }
}
