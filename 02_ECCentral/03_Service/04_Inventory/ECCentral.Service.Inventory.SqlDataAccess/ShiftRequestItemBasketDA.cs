using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IShiftRequestItemBasketDA))]
    public class ShiftRequestItemBasketDA : IShiftRequestItemBasketDA
    {
        #region IShiftRequestItemBasketDA Members

        public int CreateShiftBasket(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertShiftBasket");
            dc.SetParameterValue("@SysNo", item.SysNo);
            dc.SetParameterValue("@ProductSysNo", item.ShiftProduct.SysNo);
            dc.SetParameterValue("@StockSysNoA", item.SourceStock.SysNo);
            dc.SetParameterValue("@StockSysNoB", item.TargetStock.SysNo);
            dc.SetParameterValue("@ShiftQty", item.ShiftQuantity);
            dc.SetParameterValue("@InDate", DateTime.Now);
            dc.SetParameterValueAsCurrentUserAcct("@InUser");
            dc.SetParameterValue("@EditDate", DateTime.Now);
            dc.SetParameterValueAsCurrentUserAcct("@EditUser");
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);

            int result = dc.ExecuteNonQuery();
            item.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return result;
        }

        public bool IsExistSourceAndTargetStockInBasket(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            object o = null;
            DataCommand dc = DataCommandManager.GetDataCommand("IsExistPAB");
            dc.SetParameterValue("@ProductSysNo", item.ShiftProduct.SysNo);
            dc.SetParameterValue("@StockSysNoA", item.SourceStock.SysNo);
            dc.SetParameterValue("@StockSysNoB", item.TargetStock.SysNo);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            o = dc.ExecuteScalar();
            return (o == null || o == DBNull.Value || Convert.ToInt32(o) == 0) ? false : true;
        }

        public int GetNowShiftQtyGroupByProductSysNo(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            object o = null;

            DataCommand dc = DataCommandManager.GetDataCommand("GetNowShiftQtyGroupByProductSysNo");
            dc.SetParameterValue("@SysNo", item.SysNo);
            dc.SetParameterValue("@StockSysNoA", item.SourceStock.SysNo);
            dc.SetParameterValue("@ProductSysNo", item.ShiftProduct.SysNo);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);
            o = dc.ExecuteScalar();
            if (o == null || o == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(o);
        }

        public int DeleteShiftBasket(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteShiftBasket");
            dc.SetParameterValue("@SysNo", item.SysNo);
            int result = dc.ExecuteNonQuery();
            return result;
        }

        public int GetStockAvailableQtyGroupByProductSysNo(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetStockAvailableQtyGroupByProductSysNo");

            dc.SetParameterValue("@StockSysNo", item.SourceStock.SysNo);
            dc.SetParameterValue("@ProductSysNo", item.ShiftProduct.SysNo);
            dc.SetParameterValue("@CompanyCode", item.CompanyCode);

            object o = dc.ExecuteScalar();
            if (o == null || o == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(o);
        }

        public int UpdateShiftBasket(BizEntity.Inventory.ShiftRequestItemInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateShiftBasket");
            dc.SetParameterValue("@SysNo", item.SysNo);
            dc.SetParameterValue("@ShiftQty", item.ShiftQuantity);
            dc.SetParameterValue("@EditDate", DateTime.Now);
            dc.SetParameterValueAsCurrentUserAcct("@EditUser");
            return dc.ExecuteNonQuery();
        }

        #endregion
    }
}
