using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.ThirdPart.Interface
{
    public interface ITaoBaoAPI
    {
        string GetIncrementOrderIDs(string startTime, string endTime, string status, string pageNo, string pageSize, string useHasNext);

        string GetOrderDetails(string orderID);

        string OfflineShip(string tID, string OutSID, string companyCode, string senderID, string cancelID);
    }
}
