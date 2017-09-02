using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IPriceChangeDA))]
    public class PriceChangeDA : IPriceChangeDA
    {
        public int SavePriceChangeMaster(PriceChangeMaster item) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_SavePriceChangeMaster");
            cmd.SetParameterValue<PriceChangeMaster>(item);

            return cmd.ExecuteScalar<int>();
        }

        public void SavePriceChangeItem(PriceChangeItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_SavePriceChangeItem");
            cmd.SetParameterValue<PriceChangeItem>(item);

            cmd.ExecuteNonQuery();
        }

        public void DeletePriceChangeItemByMasterSysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_DeletePriceChangeItem");
            cmd.SetParameterValue("@MasterSysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        public PriceChangeMaster UpdatePriceChangeMaster(PriceChangeMaster item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_UpdatePriceChangeMaster");
            cmd.SetParameterValue<PriceChangeMaster>(item);

            cmd.ExecuteNonQuery();

            return item;
        }

        public PriceChangeMaster GetPriceChangeBySysNo(int sysno) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_GetByPriceChangeBySysNo");
            cmd.SetParameterValue("@SysNo", sysno);

            DataSet ds = cmd.ExecuteDataSet();
            PriceChangeMaster master = new PriceChangeMaster();

            if(ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                master = DataMapper.GetEntity<PriceChangeMaster>(row);

                master.ItemList = new List<PriceChangeItem>();

                master.ItemList = DataMapper.GetEntityList<PriceChangeItem,List<PriceChangeItem>>(ds.Tables[1].Rows);
            }

            return master;
        }

        public List<PriceChangeMaster> GetPriceChangeByStatus(RequestPriceStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_GetByPriceChangeByStatus");
            cmd.SetParameterValue("@Status", status);

            DataSet ds = cmd.ExecuteDataSet();

            List<PriceChangeMaster> masterLst = new List<PriceChangeMaster>();
            List<PriceChangeItem> itemList = new List<PriceChangeItem>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                masterLst = DataMapper.GetEntityList<PriceChangeMaster, List<PriceChangeMaster>>(ds.Tables[0].Rows);

                itemList = DataMapper.GetEntityList<PriceChangeItem, List<PriceChangeItem>>(ds.Tables[1].Rows);

                masterLst.ForEach(p => 
                {
                    p.ItemList = itemList.Where(item => item.MasterSysNo == p.SysNo).ToList();
                });
            }

            return masterLst;
        }

        public void UpdatePriceChangeStatus(PriceChangeMaster item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_ChangePriceChangeStatus");
            cmd.SetParameterValue<PriceChangeMaster>(item);

            cmd.ExecuteNonQuery();
        }

        public bool IsExistsAuditedOrRuningProduct(PriceChangeItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_IsExistsAuditOrRun");
            cmd.SetParameterValue("@ProductSysNo", item.ProductsysNo);

            return cmd.ExecuteScalar<int>() > 0 ? true : false;
        }

        public void UpdateRealBeginDate(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_UpdateRealBeginDate");
            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 新单压就单，使旧单的状态为不可用状态
        /// </summary>
        /// <param name="itmeSysNo"></param>
        public void DisableOldChangeItemStatusByNewItemSysNo(int itmeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_DisableOldChangeItemStatusByNewItemSysNo");
            cmd.SetParameterValue("@SysNo", itmeSysNo);
            cmd.SetParameterValue("@CurentStatus", PriceChangeItemStatus.Enable);
            cmd.SetParameterValue("@Status", PriceChangeItemStatus.Disable);
            cmd.SetParameterValue("@EnableStatus", PriceChangeItemStatus.Enable);
            cmd.ExecuteNonQuery();
        }
    }
}
