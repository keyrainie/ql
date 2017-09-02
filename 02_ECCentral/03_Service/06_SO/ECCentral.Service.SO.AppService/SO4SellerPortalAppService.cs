using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Text;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.BizEntity.IM;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace ECCentral.Service.SO.AppService
{
    /// <summary>
    /// 订单服务
    /// </summary>
    [VersionExport(typeof(SO4SellerPortalAppService))]
    public class SO4SellerPortalAppService
    {
        public void ProcessSellerPortalMessage(RequestMessage reqMsg)
        {
            switch (reqMsg.ActionType)
            {
                case "ChangeSOStatus":
                    ObjectFactory<SO4SellerPortalProcessor>.Instance.UpdateReceiveStatusAndSOStatus(reqMsg.Message);
                    break;
                case "ShipOrder":
                    OutStock(reqMsg.Message);
                    break;
                case "SOInvoicePrinted":
                    ObjectFactory<SO4SellerPortalProcessor>.Instance.UpdateSOMasterInvoiceNo(reqMsg.Message);
                    break;
                default:
                    break;
            }
        }

        private void OutStock(string ssbMessage)
        {
            SOShippedEntity shippedInfo = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<SOShippedEntity>(ssbMessage);
            if (shippedInfo == null || shippedInfo.Node == null || shippedInfo.Node.RequestRoot == null ||
                shippedInfo.Node.RequestRoot.Body == null || shippedInfo.Node.RequestRoot.Body.ShipOrderMsg == null ||
                shippedInfo.Node.RequestRoot.Body.ShipOrderMsg.SalesOrder == null)
            {
                return;
            }

            SOProcessor processor = ObjectFactory<SOProcessor>.Instance;

            foreach (SalesOrderInfo soMsg in shippedInfo.Node.RequestRoot.Body.ShipOrderMsg.SalesOrder)
            {
                SOInfo soInfo = processor.GetSOBySOSysNo(soMsg.SONumber);
                soMsg.InUser = shippedInfo.Node.RequestRoot.Body.ShipOrderMsg.InUser;
                soMsg.CompanyCode = shippedInfo.Node.RequestRoot.MessageHead.CompanyCode;
                processor.ProcessSO(new SOAction.SOCommandInfo
                 {
                     SOInfo = soInfo,
                     Command = SOAction.SOCommand.OutStock,
                     Parameter = new object[] { soMsg }
                 });
            }
        }
    }
}
