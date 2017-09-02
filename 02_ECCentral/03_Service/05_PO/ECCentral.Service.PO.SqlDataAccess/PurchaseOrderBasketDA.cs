using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IPurchaseOrderBasketDA))]
    public class PurchaseOrderBasketDA : IPurchaseOrderBasketDA
    {
        public BizEntity.PO.BasketItemsInfo CreateBasketItem(BizEntity.PO.BasketItemsInfo basketItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateBasket");

            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@OrderPrice", basketItemInfo.OrderPrice);
            command.SetParameterValue("@ProductSysNo", basketItemInfo.ProductSysNo);
            command.SetParameterValue("@Quantity", basketItemInfo.Quantity);
            command.SetParameterValue("@ReadyQuantity", basketItemInfo.ReadyQuantity);
            command.SetParameterValue("@CompanyCode", basketItemInfo.CompanyCode);

            basketItemInfo.ItemSysNo = System.Convert.ToInt32(command.ExecuteScalar());

            return basketItemInfo;
        }

        public BizEntity.PO.BasketItemsInfo CreateBasketItemForPrepare(BizEntity.PO.BasketItemsInfo prepareInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateBasketForPrepare");

            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@OrderPrice", prepareInfo.OrderPrice);
            command.SetParameterValue("@ProductSysNo", prepareInfo.ProductSysNo);
            command.SetParameterValue("@Quantity", prepareInfo.Quantity);
            command.SetParameterValue("@StockSysNo", prepareInfo.StockSysNo);
            command.SetParameterValue("@IsTransfer", prepareInfo.IsTransfer);
            command.SetParameterValue("@LastVendorSysNo", prepareInfo.LastVendorSysNo);
            command.SetParameterValue("@ReadyQuantity", prepareInfo.ReadyQuantity);
            command.SetParameterValue("@CompanyCode", prepareInfo.CompanyCode);

            prepareInfo.ItemSysNo = System.Convert.ToInt32(command.ExecuteScalar());
            return prepareInfo;
        }

        public BizEntity.PO.BasketItemsInfo UpdateBasketItem(BizEntity.PO.BasketItemsInfo basketItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBasket");

            command.SetParameterValue("@SysNo", basketItemInfo.ItemSysNo);
            command.SetParameterValue("@OrderPrice", basketItemInfo.OrderPrice);
            command.SetParameterValue("@Quantity", basketItemInfo.Quantity);
            command.SetParameterValue("@LastVendorSysNo", basketItemInfo.VendorSysNo);
            command.SetParameterValue("@StockSysNo", basketItemInfo.StockSysNo);
            command.SetParameterValue("@IsTransfer", basketItemInfo.IsTransfer);
            command.ExecuteNonQuery();

            return basketItemInfo;
        }

        public BizEntity.PO.BasketItemsInfo DeleteBasketItem(BizEntity.PO.BasketItemsInfo basketItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteBasket");

            command.SetParameterValue("@SysNo", basketItemInfo.ItemSysNo);
            command.ExecuteNonQuery();

            return basketItemInfo;
        }

        public BizEntity.PO.BasketItemsInfo UpdateBasketItemForGift(BizEntity.PO.BasketItemsInfo basketItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBasketForGift");
            command.SetParameterValue("@SysNo", basketItemInfo.ItemSysNo);
            command.SetParameterValue("@Quantity", basketItemInfo.Quantity);
            command.ExecuteNonQuery();
            return basketItemInfo;
        }

        public bool CheckProductHasExistInBasket(BizEntity.PO.BasketItemsInfo basketItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductFormBasketCount");

            command.SetParameterValue("@ProductSysoNo", basketItemInfo.ProductSysNo);
            command.SetParameterValueAsCurrentUserSysNo("@UserSysNo");
            command.SetParameterValue("@StockSysNo", basketItemInfo.StockSysNo);
            command.SetParameterValue("@LastVendorSysNo", basketItemInfo.LastVendorSysNo);

            object result = command.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }

        public int DeleteBasket(int basketSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteBasket");

            command.SetParameterValue("@SysNo", basketSysNo);
            return command.ExecuteNonQuery();
        }

        public BizEntity.PO.BasketItemsInfo LoadBasketItemBySysNo(int? basketSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetBasketItemBySysNo");
            command.SetParameterValue("@SysNo", basketSysNo);
            return command.ExecuteEntity<BasketItemsInfo>();
        }


        #region IPurchaseOrderBasketDA Members


        public List<BasketItemsInfo> LoadGiftItemByBasketItem(List<int> productSysNoList)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySaleGiftByBasketItem");
            command.CommandText = command.CommandText.Replace("@ItemList", productSysNoList.ToListString());
            return command.ExecuteEntityList<BasketItemsInfo>();
        }

        public int CheckGiftInBasket(BasketItemsInfo basketInfo, int giftSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductFormBasket");

            command.SetParameterValue("@ProductSysoNo", giftSysNo);
            command.SetParameterValue("@StockSysNo", basketInfo.StockSysNo);
            command.SetParameterValue("@IsTransfer", basketInfo.IsTransfer);
            command.SetParameterValue("@VendorSysNo", basketInfo.VendorSysNo);
            object result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public BasketItemsInfo LoadBasketGiftInfo(int productSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGiftInfo");
            command.CommandText = command.CommandText.Replace("@ProductSysNo", productSysNo.ToString());
            return command.ExecuteEntity<BasketItemsInfo>();
        }

        public int GetItemSysNoByItemID(string itemID, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductSysNoByID");

            command.SetParameterValue("@ProductID", itemID);
            command.SetParameterValue("@CompanyCode", companyCode);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        #endregion

        public int GetStockSysNoByName(string stockName, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetStockSysNoByName");

            command.SetParameterValue("@StockName", stockName);
            command.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        #region IPurchaseOrderBasketDA Members


        public int? AvailableQtyByProductSysNO(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Basket_V_INM_Inventory");
            command.SetParameterValue("@ItemSysNumber", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["AvailableQty"] != null)
                {
                    return (int)dt.Rows[0]["AvailableQty"];
                }
            }
            return null;
        }

        public decimal? JDPriceByProductSysNO(int productSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetProduct");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "v_ici.SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v_ici.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal, productSysNo);

                command.CommandText = builder.BuildQuerySql();
                DataTable dt = command.ExecuteDataSet().Tables[0];
                if (dt.Rows.Count > 0 && dt.Rows[0]["JDPrice"] != null && dt.Rows[0]["JDPrice"].ToString() != "")
                {
                    return Convert.ToDecimal(dt.Rows[0]["JDPrice"]);
                }
                else
                {
                    return null;
                }
            }
        }

        public int? M1ByProductSysNO(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductInforByProductSysNo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["m1"] != null)
                {
                    return (int)dt.Rows[0]["m1"];
                }
            }
            return 0;
        }

        public BasketItemsInfo QueryGiftInfo(int productSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGiftInfo");
            command.CommandText = command.CommandText.Replace("@ProductSysNo", productSysNo.ToString());
            return command.ExecuteEntity<BasketItemsInfo>();
        }

        #endregion

        public int GetVendorSysNoByProductNoAndStockSysNo(int productSysNo,int stockSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorByProductNoAndStockSysNo");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@StockSysNo", stockSysNo);

            return System.Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
