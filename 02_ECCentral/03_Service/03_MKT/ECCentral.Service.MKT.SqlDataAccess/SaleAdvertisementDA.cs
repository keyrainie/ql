using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ISaleAdvertisementDA))]
    public class SaleAdvertisementDA : ISaleAdvertisementDA
    {
        #region SaleAdvertisement
        public SaleAdvertisement CreateSaleAdv(SaleAdvertisement entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSaleAdv");

            command.SetParameterValue(entity);

            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            return entity;
        }

        public void UpdateSaleAdv(SaleAdvertisement entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("MaintainSaleAdv");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();            
        }

        /// <summary>
        /// 修改模板锁定状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void UpdateIsHoldSaleAdvBySysNo(SaleAdvertisement entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateIsHoldSaleAdvertisementBySysNo");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@IsHold", entity.IsHold);

            cmd.ExecuteNonQuery();
        }

        public SaleAdvertisement LoadBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSaleAdvBySysNo");

            command.SetParameterValue("@SysNo", sysNo);           
 
            return command.ExecuteEntity<SaleAdvertisement>();            
        }
        #endregion

        #region SaleAdvertisementItem
        public List<SaleAdvertisementItem> GetSaleAdvItems(int saleAdvSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSaleAdvItemsBySaleAdvSysNo");
            command.SetParameterValue("@SaleAdvSysNo", saleAdvSysNo);            

            List<SaleAdvertisementItem> result = command.ExecuteEntityList<SaleAdvertisementItem>();
            return result;
        }

        public SaleAdvertisementItem LoadSaleAdvItemBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSaleAdvItemBySysNo");            
            command.SetParameterValue("@SysNo", sysNo);
            
            return command.ExecuteEntity<SaleAdvertisementItem>();
        }
       
        public SaleAdvertisementItem CreateItem(SaleAdvertisementItem entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateSaleAdvItem");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));

            return entity;
        }

        public void UpdateItem(SaleAdvertisementItem entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("MaintainSaleAdvItem");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();                        
        }

        public void UpdateItemStatus(SaleAdvertisementItem entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdataSaleAdvertisementItemStatusManagement");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();            
        }


        public void DeleteItem(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("DeleteSaleAdvItem");

            dc.SetParameterValue("@SysNo", sysNo);
            
            dc.ExecuteNonQuery();            
        }

        public void CreateSaleAdvItemLog(SaleAdvertisementItem entity, string action)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("CreateSaleAdvertisementItem_Log");

            dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            dc.SetParameterValue("@SaleAdvSysNo", entity.SaleAdvSysNo);
            dc.SetParameterValue("@OperationType", action);
            dc.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            dc.SetParameterValue("@StoreCompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]

            dc.ExecuteNonQuery();            
        }

        public bool CheckSaleAdvItemDuplicate(SaleAdvertisementItem entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSaleAdvItemCheckRepeate");

            command.SetParameterValue(entity);
           
            List<SaleAdvertisementItem> result = command.ExecuteEntityList<SaleAdvertisementItem>();

            return result != null && result.Count > 0;
        }
        #endregion

        #region SaleAdvGroup
        public SaleAdvertisementGroup LoadSaleAdvGroupBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSaleAdvGroupBySysNo");

            command.SetParameterValue("@SysNo",sysNo);

            return command.ExecuteEntity<SaleAdvertisementGroup>();                        
        }

        public SaleAdvertisementGroup CreateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSaleAdvGroup");

            command.SetParameterValue(entity);            

            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            return entity;            
        }

        public void UpdateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSaleAdvGroup");

            command.SetParameterValue(entity);

            command.ExecuteNonQuery();            
        }

        public List<SaleAdvertisementGroup> LoadSaleAdvGroupsBySaleAdvSysNo(int saleAdvSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSaleAdvGroupsBySaleAdvSysNo");

            command.SetParameterValue("@SaleAdvSysNo",saleAdvSysNo);

            return command.ExecuteEntityList<SaleAdvertisementGroup>();
        }

        public void DeleteSaleAdvGroup(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSaleAdvGroup");

            command.SetParameterValue("@SysNo", sysNo);

            command.ExecuteNonQuery();
        }
        #endregion
    }
}
