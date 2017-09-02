using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;


namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(StockProcessor))]
    public class StockProcessor
    {
        private readonly IStockDA _stockDA = ObjectFactory<IStockDA>.Instance;
        private readonly IWarehouseDA _warehouseDA = ObjectFactory<IWarehouseDA>.Instance;
        private readonly IWarehouseOwnerDA _warehouseOwnerDA = ObjectFactory<IWarehouseOwnerDA>.Instance;

        #region 渠道仓库(Stock)相业务处理方法

        /// <summary>
        /// 创建渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public virtual StockInfo CreateStock(StockInfo stockInfo)
        {
            string checkResult = PreCheckStockInfoForCreate(stockInfo);
            StockInfo result = null;
            if (string.IsNullOrEmpty(checkResult))
            {    
                result = _stockDA.CreateStock(stockInfo);
            }
            return result;
        }

        /// <summary>
        /// 更新渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public virtual StockInfo UpdateStock(StockInfo stockInfo)
        {
            string checkResult = PreCheckStockInfoForUpdate(stockInfo);
            if (string.IsNullOrEmpty(checkResult))
            {
                return _stockDA.UpdateStock(stockInfo);
            }
            return null;
        }

        /// <summary>
        /// 获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public virtual StockInfo GetStockInfo(int stockSysNo)
        {
            return _stockDA.GetStockInfoBySysNo(stockSysNo);
        }

        /// <summary>
        /// 获取渠道仓库列表
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public virtual List<StockInfo> GetStockList(string webChannelID)
        {
            return _stockDA.QueryStockListByWebChannelID(webChannelID);
        }

        #region 渠道仓库信息检查逻辑

        private string PreCheckStockInfoForCreate(StockInfo stockInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckStockInfo(stockInfo));
            //TODO: Special check points for Create action

            return result.ToString();
        }

        private string PreCheckStockInfoForUpdate(StockInfo stockInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckStockInfo(stockInfo));
            //TODO: Special check points for Update action

            return result.ToString();
        }

        private string PreCheckStockInfo(StockInfo stockInfo)
        {
            StringBuilder result = new StringBuilder();
            if (String.IsNullOrEmpty((stockInfo.StockID)))
            {
                result.Append("渠道仓库的编号不能为空!");
            }
            if (String.IsNullOrEmpty((stockInfo.StockName)))
            {
                result.Append("渠道仓库的名称不能为空!");
            }
            if (stockInfo.WarehouseInfo == null || !stockInfo.WarehouseInfo.SysNo.HasValue)
            {
                result.Append("渠道仓库的所属仓库不能为空!");
            }
            if (stockInfo.WebChannel == null || String.IsNullOrEmpty(stockInfo.WebChannel.ChannelID))
            {
                result.Append("渠道仓库的销售渠道不能为空!");
            }

            //TODO: StockID Duplicated Check
            return result.ToString();
        }

        #endregion

        #endregion

        #region 仓库(Warehouse)业务处理方法

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo CreateWarehouse(WarehouseInfo warehouseInfo)
        {
            string checkResult = PreCheckWarehouseInfoForCreate(warehouseInfo);
            WarehouseInfo result = null;
            if (string.IsNullOrEmpty(checkResult))
            {
                result = _warehouseDA.CreateWarehouse(warehouseInfo);                
            }
            return result;
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo UpdateWarehouse(WarehouseInfo warehouseInfo)
        {
            string checkResult = PreCheckWarehouseInfoForUpdate(warehouseInfo);
            if (string.IsNullOrEmpty(checkResult))
            {
                return _warehouseDA.UpdateWarehouse(warehouseInfo);
            }
            return null;
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo GetWarehouseInfo(int warehouseSysNo)
        {
            return _warehouseDA.GetWarehouseInfoBySysNo(warehouseSysNo);
        }

        /// <summary>
        /// 获取仓库列表
        /// </summary>      
        /// <param name="companyCode">公司编号</param>  
        /// <returns>仓库列表</returns>
        public virtual List<WarehouseInfo> GetWarehouseListByCompanyCode(string companyCode)
        {
            return _warehouseDA.GetWarehouseListByCompanyCode(companyCode);
        }

        #region 仓库业务检查逻辑

        private string PreCheckWarehouseInfoForCreate(WarehouseInfo warehouseInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckWarehouseInfo(warehouseInfo));
            //TODO: Special check points for Create action

            return result.ToString();
        }

        private string PreCheckWarehouseInfoForUpdate(WarehouseInfo warehouseInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckWarehouseInfo(warehouseInfo));
            //TODO: Special check points for Update action

            return result.ToString();
        }

        private string PreCheckWarehouseInfo(WarehouseInfo warehouseInfo)
        {
            StringBuilder result = new StringBuilder();
            if (String.IsNullOrEmpty((warehouseInfo.WarehouseID)))
            {
                result.Append("仓库编号不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.WarehouseName)))
            {
                result.Append("仓库名称不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.Address)))
            {
                result.Append("仓库地址不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.Contact)))
            {
                result.Append("联系人不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.PhoneNumber)))
            {
                result.Append("联系电话不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.ReceiveAddress)))
            {
                result.Append("收货地址不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.ReceiveContact)))
            {
                result.Append("收货联系人不能为空!");
            }
            if (String.IsNullOrEmpty((warehouseInfo.ReceiveContactPhoneNumber)))
            {
                result.Append("收货联系电话不能为空!");
            }
            if (warehouseInfo.TransferRate < 0)
            {
                result.Append("移仓分仓系数不能小于零!");
            }

            return result.ToString();
        }

        #endregion

        #endregion

        #region 仓库所有者(WarehouseOwner)业务处理方法

        /// <summary>
        /// 创建仓库所有者
        /// </summary>
        /// <param name="warehouseOwnerInfo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo CreateWarehouseOwner(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            string checkResult = PreCheckWarehouseOwnerInfoForCreate(warehouseOwnerInfo);
            if (string.IsNullOrEmpty(checkResult))
            {
                warehouseOwnerInfo.CreateDate = DateTime.Now;
                _warehouseOwnerDA.CreateWarehouseOwner(warehouseOwnerInfo);
            }
            return null;
        }

        /// <summary>
        /// 更新仓库所有者信息
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo UpdateWarehouseOwner(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            string checkResult = PreCheckWarehouseOwnerInfoForUpdate(warehouseOwnerInfo);
            if (string.IsNullOrEmpty(checkResult))
            {
                warehouseOwnerInfo.EditDate = DateTime.Now;
                return _warehouseOwnerDA.UpdateWarehouseOwner(warehouseOwnerInfo);
            }
            return null;
        }

        /// <summary>
        /// 获取仓库所有者信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo GetWarehouseOwnerInfo(int warehouseOwnerSysNo)
        {
            return _warehouseOwnerDA.GetWarehouseOwnerInfoBySysNo(warehouseOwnerSysNo);
        }
        /// <summary>
        /// 根据公司编号取得仓库所有者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<WarehouseOwnerInfo> GetWarehouseOwnerInfoByCompanyCode(string companyCode)
        {
            return _warehouseOwnerDA.GetWarehouseOwnerInfoByCompanyCode(companyCode);
        }

        #region 仓库所有者业务检查逻辑
        private string PreCheckWarehouseOwnerInfoForCreate(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckWarehouseOwnerInfo(warehouseOwnerInfo));
            //TODO: Special check points for Create action

            return result.ToString();
        }

        private string PreCheckWarehouseOwnerInfoForUpdate(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(PreCheckWarehouseOwnerInfo(warehouseOwnerInfo));
            //TODO: Special check points for Update action

            return result.ToString();
        }

        private string PreCheckWarehouseOwnerInfo(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            StringBuilder result = new StringBuilder();
            if (String.IsNullOrEmpty((warehouseOwnerInfo.OwnerID)))
            {
                result.Append("仓库所有者的编号不能为空!");
            }

            if (String.IsNullOrEmpty((warehouseOwnerInfo.OwnerName)))
            {
                result.Append("仓库所有者的名称不能为空!");
            }
            return result.ToString();
        }

        #endregion

        #endregion

        /// <summary>
        /// 获取本地仓库编码
        /// </summary>
        /// <param name="areaSysNo">地区编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>本地仓库编码</returns>
        public virtual string GetlocalWH(int areaSysNo, string companyCode)
        {
            return _warehouseDA.GetlocalWHByAreaSysNo(areaSysNo, companyCode);
        }

    }
}
