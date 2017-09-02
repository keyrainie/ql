using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ECCentral.Job.Utility;

namespace ECCentral.Job.Inventory.AutoSendMessageInventory
{
    public class AutoSendMessageInventory
    {
        public static void SendMessageInventory()
        {
            Console.WriteLine("开始导出月底库存");
            string baseUrl = ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string address = ConfigurationManager.AppSettings["ToAddress"];
            RestClient client = new RestClient(baseUrl);
            RestServiceError error;
            client.Update("InventoryService/Inventory/Job/SendInventoryEmailEndOfMonth", address, out error);

            if (error!=null&&error.Faults.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in error.Faults)
                {
                    sb.Append(item.ErrorDescription);
                    Console.WriteLine(item.ErrorDescription);
                }

                string msg = sb.ToString();
            }
            else
            {
                Console.WriteLine("月底库存导出成功");
            }

            System.Environment.Exit(0);
        }

    }
}
