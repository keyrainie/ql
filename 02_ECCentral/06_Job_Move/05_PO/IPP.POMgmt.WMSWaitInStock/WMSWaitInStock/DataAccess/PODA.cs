using System.Collections.Generic;
using Newegg.Oversea.Framework.DataAccess;
using IPP.Oversea.CN.POASNMgmt.BusinessEntities;


namespace IPP.OrderMgmt.JobV31.DataAccess
{
    public class PODA
    {
        /// <summary>
        /// 获取待入库的采购单列表
        /// </summary>
        /// <returns></returns>
        public static List<POEntity> GetPOList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOList");
            return command.ExecuteEntityList<POEntity>();
        }

        /// <summary>
        /// 获取采购单子项
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public static List<POItemEntity> GetPOItemListByPOSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOWithItems");
            command.SetParameterValue("@SysNo", poSysNo);
            return command.ExecuteEntityList<POItemEntity>();
        }
    }
}
