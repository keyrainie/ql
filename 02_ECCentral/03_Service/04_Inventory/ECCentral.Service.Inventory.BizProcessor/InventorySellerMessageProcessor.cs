using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using System.Xml;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(InventorySellerMessageProcessor))]
    public class InventorySellerMessageProcessor
    {
        private ILendRequestDA lendRequestDA = ObjectFactory<ILendRequestDA>.Instance;

        public void AdjustSellerInventory(string message)
        {
            SellerAdjustEntity sellerAdjustEntity = SerializationUtility.XmlDeserialize<SellerAdjustEntity>(message);
            if (sellerAdjustEntity != null)
            {
                InventoryAdjustContractInfo inventoryAdjustContractInfo = new BizEntity.Inventory.InventoryAdjustContractInfo();
                inventoryAdjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Seller_Inventory;
                inventoryAdjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Update;
                inventoryAdjustContractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                List<SellerAdjustItemInventory> sellerAdjustEntityList = sellerAdjustEntity.Node.RequestRoot.Body.AdjustInventoryMsg.ItemInventoryList;
                foreach (var adjustItem in sellerAdjustEntityList)
                {
                    inventoryAdjustContractInfo.AdjustItemList.Add(
                        new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = adjustItem.Quantity,
                            ProductSysNo = adjustItem.ProductSysNo,
                            StockSysNo = adjustItem.WareHouseNumber
                        }
                            );
                }

                string result = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContractInfo);
                if (!string.IsNullOrEmpty(result))
                {
                    throw new BizException(result);
                }
            }
        }

        public void CreateWMSCheck(string message)
        {
            string ssbInfo1 = message;
            string ssbInfo2 = message;
            string ssbInfo3 = message;
            string ssbInfo4 = message;
            string[] ssbInfoAray1 = new string[3];
            string inputssbInfo = string.Empty;
            if (ssbInfo1.Contains("<Node>"))
            {
                ssbInfoAray1 = ssbInfo1.Replace("<Node>", "</Node>").Split(new string[] { "</Node>" }, StringSplitOptions.None);
                inputssbInfo = ssbInfoAray1[1];
            }
            string[] ssbInfoAray2 = new string[3];
            string OriginalGUID = string.Empty;
            if (ssbInfo2.Contains("<OriginalGUID>"))
            {
                ssbInfoAray2 = ssbInfo2.Replace("<OriginalGUID>", "</OriginalGUID>").Split(new string[] { "</OriginalGUID>" }, StringSplitOptions.None);
                OriginalGUID = ssbInfoAray2[1];
            }
            string[] ssbInfoAray3 = new string[3];
            string SendEmailAddress = string.Empty;
            if (ssbInfo3.Contains("<SendEmailAddress>"))
            {
                ssbInfoAray3 = ssbInfo3.Replace("<SendEmailAddress>", "</SendEmailAddress>").Split(new string[] { "</SendEmailAddress>" }, StringSplitOptions.None);
                SendEmailAddress = ssbInfoAray3[1];
            }
            string[] ssbInfoAray4 = new string[3];
            string StockSysNo = string.Empty;
            if (ssbInfo4.Contains("<StockSysNo>"))
            {
                ssbInfoAray4 = ssbInfo4.Replace("<StockSysNo>", "</StockSysNo>").Split(new string[] { "</StockSysNo>" }, StringSplitOptions.None);
                StockSysNo = ssbInfoAray4[1];
            }
            if (!string.IsNullOrEmpty(inputssbInfo))
            {
                inputssbInfo = "<Node>" + inputssbInfo + "</Node>";
                using (TransactionScope scope = new TransactionScope())
                {
                    //执行SP 创建损益单 作废借货单 调整对应库存
                    ObjectFactory<IInventorySellerDA>.Instance.CreateAdjust4WMSCheck(inputssbInfo);
                    //发送邮件
                    //AdjustDAL.SendReciveWMSSSBCreateAdjustEmail(StockSysNo, OriginalGUID, SendEmailAddress);
                    scope.Complete();
                }
            }     
        }
    }
}
