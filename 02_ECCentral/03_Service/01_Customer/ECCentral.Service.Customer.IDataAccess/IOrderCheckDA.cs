using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IOrderCheckMasterDA
    {
        OrderCheckMaster Creat(OrderCheckMaster entity);
        void UpdateOrderCheckMasterAllDisable(OrderCheckMaster entity);
        List<OrderCheckMaster> GetCurrentOrderCheckMasterList(string companyCode);
        void UpdateOrderCheckMaster(OrderCheckMaster entity);

    }
    public interface IOrderCheckItemDA
    {
        OrderCheckItem Creat(OrderCheckItem entity);
        void UpdateOrderCheckItem(OrderCheckItem entity);
        int DeleteOrderCheckItem(OrderCheckItem entity);
        int GetSACount(OrderCheckItem entity);
        List<OrderCheckItem> GetOrderCheckItemByQuery(OrderCheckItem queryCriteriaEntity);
        List<OrderCheckItem> GetOrderCheckItem(string checkType, string companyCode);
    }
}
