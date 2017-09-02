using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(OrderCheckMasterProcessor))]
    public class OrderCheckMasterProcessor
    {
        private IOrderCheckMasterDA OrderCheckMasterDA = ObjectFactory<IOrderCheckMasterDA>.Instance;
        /// <summary>
        /// 批量更新OrderCheckMaster状态
        /// </summary>
        public virtual void BatchUpdateOrderCheckMasterStatus(OrderCheckMaster msg, List<int> SysNoList)
        {
            ///未选择sysno则将所有的都更新为无效状态
            if (SysNoList.Count == 0)
            {
                OrderCheckMasterDA.UpdateOrderCheckMasterAllDisable(msg);
            }
            else
            {
                ///获取现有的自动审单列表
                List<OrderCheckMaster> currentList = OrderCheckMasterDA.GetCurrentOrderCheckMasterList(msg.CompanyCode);
                foreach (var entity in currentList)
                {
                    if (SysNoList.Exists(x => x == entity.SysNo))//更新为有效状态
                    {
                        if (entity.Status != OrderCheckStatus.Valid)
                        {
                            entity.Status = OrderCheckStatus.Valid;
                            OrderCheckMasterDA.UpdateOrderCheckMaster(entity);
                        }
                    }
                    else//更新为无效状态
                    {
                        if (entity.Status != OrderCheckStatus.Invalid)
                        {
                            entity.Status = OrderCheckStatus.Invalid;
                            OrderCheckMasterDA.UpdateOrderCheckMaster(entity);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建OrderCheckMaster
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual OrderCheckMaster CreateOrderCheckMaster(OrderCheckMaster msg)
        {
            return ObjectFactory<IOrderCheckMasterDA>.Instance.Creat(msg);
        }

        public List<OrderCheckMaster> GetOrderCheckList(string companyCode)
        {
            List<OrderCheckMaster> list = ObjectFactory<IOrderCheckMasterDA>.Instance.GetCurrentOrderCheckMasterList(companyCode);
            foreach (var item in list)
            {
                item.OrderCheckItemList = ObjectFactory<OrderCheckItemProcessor>.Instance.GetOrderCheckItemList(item);
            }
            return list;
        }
    }

    [VersionExport(typeof(OrderCheckItemProcessor))]
    public class OrderCheckItemProcessor
    {
        private IOrderCheckItemDA OrderCheckItemDA = ObjectFactory<IOrderCheckItemDA>.Instance;
        private IOrderCheckMasterDA OrderCheckMasterDA = ObjectFactory<IOrderCheckMasterDA>.Instance;
        private static string NeedDeleteKeys = "ST,PT,FP,CT";//保存前清除数据的KEY
        private static string DeliveryTimeKeys = "'DT11','DT12'";//配送时间的KEY
        private static string KeywordKeys = "'CA','CP','CN'";//关键字KEY
        private static string AMTKeys = "'1AL','2AL','3AL','4AL','5AL'";//订单金额KEY

        /// <summary>
        /// 创建OrderCheckItem
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual OrderCheckItem CreateOrderCheckItem(OrderCheckItem msg)
        {
            if (msg.ReferenceType == "SA")
            {
                DateTime start = DateTime.Parse(msg.ReferenceContent);
                DateTime end = DateTime.Parse(msg.Description);
                if (end < start)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "DateTimeCheck"));
                }
                int i = OrderCheckItemDA.GetSACount(msg);
                if (i > 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "TimeIntervalCheck"));
                }
            }
            else
            {
                OrderCheckItem queryCriteriaEntity = new OrderCheckItem();
                queryCriteriaEntity.ReferenceType = msg.ReferenceType;
                queryCriteriaEntity.ReferenceContent = msg.ReferenceContent;
                queryCriteriaEntity.CompanyCode = msg.CompanyCode;
                if (msg.ReferenceType == "PC3")
                {
                    queryCriteriaEntity.Description = msg.Description;
                }
                if (msg.ReferenceType.EndsWith("AL"))
                {
                    queryCriteriaEntity.ReferenceContent = null;
                    queryCriteriaEntity.Status = msg.Status;
                }
                else if (msg.ReferenceType.StartsWith("DT"))
                {
                    queryCriteriaEntity.ReferenceType = null;
                    queryCriteriaEntity.ReferenceTypeIn = DeliveryTimeKeys;
                }
                List<OrderCheckItem> result = OrderCheckItemDA.GetOrderCheckItemByQuery(queryCriteriaEntity);
                if (result != null && result.Count > 0)
                {
                    if (KeywordKeys.IndexOf(msg.ReferenceType) != -1)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "ExistsKeyword"));
                    }
                    else if (msg.ReferenceType.StartsWith("DT"))
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "ExistsServiceObject"));
                    }
                    else if (msg.ReferenceType == "PC3")
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "ExistsProductType"));
                    }
                    else if (msg.ReferenceType == "PID")
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "ExistsProductSysNo"));
                    }
                    else if (AMTKeys.IndexOf(msg.ReferenceType) != -1 && msg.Status == OrderCheckStatus.Valid)
                    {
                        foreach (var item in result)
                        {
                            item.Status = OrderCheckStatus.Invalid;
                            OrderCheckItemDA.UpdateOrderCheckItem(item);
                        }
                    }
                }
            }
            return OrderCheckItemDA.Creat(msg);
        }

        /// <summary>
        /// 批量创建OrderCheckItem
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual List<OrderCheckItem> BatchCreateOrderCheckItem(List<OrderCheckItem> msgs, string ReferenceType)
        {
            List<OrderCheckItem> list = new List<OrderCheckItem>();

            if (msgs.Count > 0)
            {
                if (NeedDeleteKeys.IndexOf(ReferenceType) != -1)
                {
                    OrderCheckItemDA.DeleteOrderCheckItem(msgs[0]);
                }
                foreach (OrderCheckItem item in msgs)
                {
                    OrderCheckItem orderCheckItem = CreateOrderCheckItem(item);
                    list.Add(orderCheckItem);
                }
            }
            return list;
        }
        /// <summary>
        /// 更新OrderCheckItem状态
        /// </summary>
        public virtual void UpdateOrderCheckItem(OrderCheckItem msg)
        {
            if (!string.IsNullOrEmpty(msg.ReferenceType) && msg.ReferenceType.StartsWith("DT"))
            {
                OrderCheckItem queryCriteriaEntity = new OrderCheckItem();
                queryCriteriaEntity.ReferenceContent = msg.ReferenceContent;
                queryCriteriaEntity.ReferenceType = msg.ReferenceType;
                List<OrderCheckItem> result = OrderCheckItemDA.GetOrderCheckItemByQuery(queryCriteriaEntity);
                if (result != null && result.Count > 0 && result[0].SysNo != msg.SysNo)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "ExistsOrderCheckItem"));
                }
                OrderCheckItemDA.UpdateOrderCheckItem(msg);
            }
            else if (!string.IsNullOrEmpty(msg.ReferenceType) && msg.ReferenceType == "PC")
            {
                OrderCheckItem queryCriteriaEntity = new OrderCheckItem();
                queryCriteriaEntity.ReferenceType = msg.ReferenceType;
                List<OrderCheckItem> oldList = OrderCheckItemDA.GetOrderCheckItemByQuery(queryCriteriaEntity);
                if (string.IsNullOrEmpty(msg.SysNos))  //disable all
                {
                    CheckOrderMaster(msg, false);
                    foreach (var oldEntity in oldList)
                    {
                        if (oldEntity.Status != OrderCheckStatus.Invalid)
                        {
                            oldEntity.SysNo = oldEntity.SysNo;
                            oldEntity.Status = OrderCheckStatus.Invalid;                         
                            OrderCheckItemDA.UpdateOrderCheckItem(oldEntity);
                        }
                    }
                }
                else
                {
                    CheckOrderMaster(msg, true);
                    string[] sysNoArray = msg.SysNos.Split(',');
                    List<int> sysNoList = new List<int>();
                    foreach (var item in sysNoArray)
                    {
                        sysNoList.Add(int.Parse(item));
                    }

                    foreach (var oldEntity in oldList)
                    {
                        if (sysNoList.Exists(x => x == oldEntity.SysNo))//active
                        {
                            if (oldEntity.Status != OrderCheckStatus.Valid)
                            {
                                oldEntity.SysNo = oldEntity.SysNo;
                                oldEntity.Status = OrderCheckStatus.Valid;                             
                                OrderCheckItemDA.UpdateOrderCheckItem(oldEntity);
                            }
                        }
                        else//disable
                        {
                            if (oldEntity.Status != OrderCheckStatus.Invalid)
                            {
                                oldEntity.SysNo = oldEntity.SysNo;
                                oldEntity.Status = OrderCheckStatus.Invalid;                              
                                OrderCheckItemDA.UpdateOrderCheckItem(oldEntity);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(msg.ReferenceType) && AMTKeys.IndexOf(msg.ReferenceType) != -1)
                {
                    OrderCheckItem queryCriteriaEntity = new OrderCheckItem();
                    queryCriteriaEntity.ReferenceType = msg.ReferenceType;
                    queryCriteriaEntity.Status = msg.Status;
                    List<OrderCheckItem> result = OrderCheckItemDA.GetOrderCheckItemByQuery(queryCriteriaEntity);
                    if (result != null && result.Count > 0 && msg.Status == OrderCheckStatus.Valid)
                    {
                        foreach (var item in result)
                        {
                            item.Status = OrderCheckStatus.Invalid;
                            OrderCheckItemDA.UpdateOrderCheckItem(item);
                        }
                    }
                }
                OrderCheckItemDA.UpdateOrderCheckItem(msg);
            }
        }

        public List<OrderCheckItem> GetOrderCheckItemList(OrderCheckMaster masterInfo)
        {
            return OrderCheckItemDA.GetOrderCheckItem(masterInfo.CheckType, masterInfo.CompanyCode);
        }

        public virtual void CheckOrderMaster(OrderCheckItem entity, bool status)
        {          
            List<OrderCheckMaster> result = new List<OrderCheckMaster>();
            result = OrderCheckMasterDA.GetCurrentOrderCheckMasterList(entity.CompanyCode);
            if (status)
            {
                foreach (OrderCheckMaster master in result)
                {
                    if (master.CheckType.Trim() == entity.ReferenceType && master.Status == OrderCheckStatus.Invalid)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.OrderCheck", "SavePCItemError"));
                    }
                }               
            }
            //else
            //{
            //    if (!result.Exists(x => x.CheckType == entity.ReferenceType && x.Status == 1))
            //    {
            //        throw new BusinessException("只有取消勾选“订单中使用优惠券、积分、余额和礼品卡”并保存后再全部取消勾选这一级选项，才能生效。");
            //    }
            //}
        }

    }
}
