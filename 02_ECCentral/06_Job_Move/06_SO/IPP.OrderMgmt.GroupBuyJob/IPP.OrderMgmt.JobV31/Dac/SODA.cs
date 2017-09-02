using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using System.Data;

namespace IPP.OrderMgmt.JobV31.Dac
{
    public class SODA
    {

        /// <summary>
        /// 获取团购信息
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ProductGroupBuyingEntity> GetGroupBuyNeedProcess(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyNeedProcess");

            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<ProductGroupBuyingEntity>();
        }

        /// <summary>
        /// 根据团购的编号获取需要处理的订单
        /// </summary>
        /// <param name="referenceSysno"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<int> GetSOSysNoListByReferenceSysno(int referenceSysno, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOSysNoListByReferenceSysno");

            command.SetParameterValue("@ReferenceSysno", referenceSysno);
            command.SetParameterValue("@CompanyCode", companyCode);
            List<int> result = new List<int>();

            IDataReader dr = command.ExecuteDataReader();
            while (dr.Read())
            {
                result.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
            return result;
            
        }




        public static SOEntity GetSOEntity(int soSysNo, string companyCode)
        {
            SOEntity soEntity = new SOEntity();

            //soEntity.SOCheckShipping = GetSOCheckShippingBySOSysno(soSysNo, companyCode);
            //soEntity.SOItemList = GetSOItemInfoListBySysNo(soSysNo, companyCode);
            soEntity.SOMaster = GetSOMasterInfoBySysNo(soSysNo, companyCode);

            return soEntity;
 
        }

        /// <summary>
        /// 根据订单系统编号获取订单主项信息
        /// </summary>GetSOMasterInfoBySysNo
        /// <param name="sysNo">订单系统编号</param>
        /// <returns></returns>
        public static SOMasterEntity GetSOMasterInfoBySysNo(int sysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOMasterInfoBySysNo");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            SOMasterEntity somaster = command.ExecuteEntity<SOMasterEntity>();
            return somaster;
        }


        /// <summary>
        /// 根据订单系统编号获取订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public static List<SOItemEntity> GetSOItemInfoListBySysNo(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOItemInfoListBySOSysNo");

            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);

            List<SOItemEntity> list = command.ExecuteEntityList<SOItemEntity>();
            return list;
        }

        /// <summary>
        /// 根据 订单系统编号 获取运费检查表项
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public static SOCheckShippingEntity GetSOCheckShippingBySOSysno(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOCheckShippingBySOSysno");

            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);

            SOCheckShippingEntity entity = command.ExecuteEntity<SOCheckShippingEntity>();
            return entity;
        }

        public static int UpdateSOGroupBuyStatus(int soSysNo,string status, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOGroupBuyStatus");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@SettlementStatus", status);
            return command.ExecuteNonQuery();

        }


        //获取一小时内没有支付的团购订单
        public static List<int> GetGroupBuySONotPayInTime(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuySONotPayInTime");
            command.SetParameterValue("@CompanyCode", companyCode);

            List<int> result = new List<int>();

            IDataReader dr = command.ExecuteDataReader();
            while (dr.Read())
            {
                result.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
            return result;
        }


        public static List<int> GetGroupBuySONotPayInTimeV2(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuySONotPayInTimeV2");
            command.SetParameterValue("@CompanyCode", companyCode);

            List<int> result = new List<int>();

            IDataReader dr = command.ExecuteDataReader();
            while (dr.Read())
            {
                result.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
            return result;

        }

    }
}
