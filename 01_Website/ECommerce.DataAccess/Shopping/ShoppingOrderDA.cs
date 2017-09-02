using System.Data;
using System.Collections.Generic;

using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Order;
using ECommerce.Entity.Shopping;
using ECommerce.Enums;
using ECommerce.Entity.Payment;
using ECommerce.Entity;

namespace ECommerce.DataAccess.Shopping
{
    public class ShoppingOrderDA
    {
        /// <summary>
        /// 根据购物车ID获取订单简单信息列表
        /// </summary>
        /// <param name="shoppingCartID">购物车ID</param>
        /// <returns></returns>
        public static List<ThankyouOrderInfo> GetOrderListByShoppingCartID(int shoppingCartID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Shopping_GetOrderListByShoppingCartID");
            cmd.SetParameterValue("@ShoppingCartID", shoppingCartID);
            return cmd.ExecuteEntityList<ThankyouOrderInfo>();
        }

        /// <summary>
        /// 支付时获取SO详细信息，优先从QueryDB读取，若QueryDB不存在，则从CenterDB读取
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns>订单详细信息</returns>
        public static PayOrderInfo PayGetOrderInfoBySOSysNo(int SOSysNo)
        {
            //优先从QueryDB读取，若QueryDB不存在，则从CenterDB读取
            PayOrderInfo result = PayGetQueryDBOrderInfoBySOSysNo(SOSysNo);
            if (result == null)
                result = PayGetCenterDBOrderInfoBySOSysNo(SOSysNo);
            return result;
        }
        /// <summary>
        /// 支付时获取SO详细信息，从QueryDB获取
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns>订单详细信息</returns>
        private static PayOrderInfo PayGetQueryDBOrderInfoBySOSysNo(int SOSysNo)
        {
            PayOrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("Order_PayGetOrderInfoBySOSysNo");
            command.SetParameterValue("@SOID", SOSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<PayOrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<PaySOItemInfo, List<PaySOItemInfo>>(itemTable.Rows);
                    }
                }
            }

            return orderInfo;
        }
        /// <summary>
        /// 支付时获取SO详细信息，从CenterDB获取
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns>订单详细信息</returns>
        public static PayOrderInfo PayGetCenterDBOrderInfoBySOSysNo(int SOSysNo)
        {
            PayOrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("Order_PayGetCenterDBOrderInfoBySOSysNo");
            command.SetParameterValue("@SOID", SOSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<PayOrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<PaySOItemInfo, List<PaySOItemInfo>>(itemTable.Rows);
                    }
                }
            }

            return orderInfo;
        }

        /// <summary>
        /// 根据订单号查询Netpay
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        public static NetpayInfo GetCenterDBNetpayBySOSysNo(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetCenterDBNetpayBySOSysNo");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            return command.ExecuteEntity<NetpayInfo>();
        }
        /// <summary>
        /// 创建Netpay
        /// </summary>
        /// <param name="entity">Netpay</param>
        public static bool CreateNetpay(NetpayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_CreateNetpay");
            command.SetParameterValue<NetpayInfo>(entity);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 根据订单号查询虚拟Netpay
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        public static NetpayInfo GetCenterDBVirualNetpayBySOSysNo(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetCenterDBVirualNetpayBySOSysNo");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            return command.ExecuteEntity<NetpayInfo>();
        }
        /// <summary>
        /// 创建虚拟Netpay
        /// </summary>
        /// <param name="entity">Netpay</param>
        public static void CreateVirtualNetpay(NetpayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_CreateVirtualNetpay");
            command.SetParameterValue<NetpayInfo>(entity);
            command.ExecuteNonQuery();
        }

        #region 虚拟团购相关

        public static VirualGroupBuyTicketInfo GetVirualGroupBuyTicketInfo(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVirualGroupBuyTicketInfo");
            command.SetParameterValue("@OrderSysNo", SOSysNo);
            return command.ExecuteEntity<VirualGroupBuyTicketInfo>();
        }

        public static string CreateGroupBuyingTicketSequence()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateGroupBuyingTicketSequence");
            DataTable dt = command.ExecuteDataTable();
            return dt.Rows[0][0].ToString();
        }

        public static List<int> GetVirualGroupBuyTicketSysNoBySOSysNo(int SOSysNo)
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetVirualGroupBuyTicketSysNoBySOSysNo");
            command.SetParameterValue("@OrderSysNo", SOSysNo);
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(int.Parse(dr[0].ToString()));
                }
            }
            return result;
        }

        public static void UpdateGroupBuyingTicketID(int sysNo, string ticketID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingTicketID");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@TicketID", ticketID);
            command.ExecuteNonQuery();
        }

        #endregion


        /// <summary>
        /// 网关退款回调
        /// </summary>
        /// <param name="externalKey">退款流水号</param>
        /// <param name="isTrue">是否成功</param>
        /// <returns></returns>
        public static void Refund(string externalKey, SOIncomeStatus Status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_UpdateSOIncomeStatus");
            command.SetParameterValue("@ExternalKey", externalKey);
            command.SetParameterValue("@Status", Status);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新订单申报记录状态
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <param name="status">状态，10-申报成功；-10-申报失败</param>
        public static void UpdateDeclareRecordsStatus(int SOSysNo, int status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_UpdateDeclareRecordsStatus");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.SetParameterValue("@Status", status);
            command.ExecuteNonQuery();
        }

        public static VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_LoadVendorCustomsInfo");
            command.SetParameterValue("@SOID", soSysNo);
            return command.ExecuteEntity<VendorCustomsInfo>();
        }

        public static VendorCustomsInfo LoadVendorCustomsInfoByProduct(string productID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_LoadVendorCustomsInfoByProduct");
            command.SetParameterValue("@ProductID", productID);
            return command.ExecuteEntity<VendorCustomsInfo>();
        }
        /// <summary>
        /// 加载订单贸易类型
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public static TradeType GetOrderTradeType(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetOrderTradeType");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<TradeType>();
        }

    }
}
