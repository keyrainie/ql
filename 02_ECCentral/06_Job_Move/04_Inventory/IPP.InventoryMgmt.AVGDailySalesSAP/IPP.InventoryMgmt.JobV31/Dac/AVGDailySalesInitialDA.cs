using System;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.InventoryMgmt.JobV31.Dac
{
    public class AVGDailySalesInitialDA
    {
        /// <summary>
        ///  获取需要初始化的数据量（有销售记录的商品）
        /// </summary>
        /// <returns></returns>
        public static int GetNeedAVGDailySalesInitialData()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNeedAVGDailySalesInitialData");
            return command.ExecuteScalar() == null ? 0 : Convert.ToInt32(command.ExecuteScalar()); 
        } 


         /// <summary>
        ///  获取需要初始化的数据量（没有销售记录的商品）
        /// </summary>
        /// <returns></returns>
        public static int GetNeedAVGDailySalesInitialDataOfNotSaledRecord()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNeedAVGDailySalesInitialDataOfNotSaledRecord");
            return command.ExecuteScalar() == null ? 0 : Convert.ToInt32(command.ExecuteScalar()); 
        } 


        /// <summary>
        /// 执行初始化 有销售记录 但是是 上架销售 和上架展示的商品 计算化日均销量 和可卖天数
        /// </summary>
        public static void AVGDailySalesInitial(int NeedInitialProductCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AVGDailySalesSync");
            command.SetParameterValue("@NeedInitialProductCount", NeedInitialProductCount);
            command.CommandTimeout = 15000;
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 执行初始化 没有销售记录 但是是 上架销售 和上架展示的商品 计算化日均销量 和可卖天数
        /// </summary>
        public static void AVGDailySalesInitialOfNotSaledRecord(int NeedInitialProductCountOfNotSaledRecord)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AVGDailySalesSyncOfNotSaledRecord");
            command.SetParameterValue("@NeedInitialProductCountOfNotSaledRecord", NeedInitialProductCountOfNotSaledRecord);
            command.CommandTimeout = 15000;
            command.ExecuteNonQuery();
        }
    }
}
