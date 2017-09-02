
using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Collections.Generic;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public class CreateInvoiceSSBProcessor : IConsumer<CreateInvoiceSSBMessage>
    {
        public void HandleEvent(CreateInvoiceSSBMessage eventMessage)
        {
            string body = String.Format(@"
<Root xmlns=""http://ShippingMessageProcessSchemas.WMSShippingSchema"">
    <toService>http://soa.newegg.com/SOA/CN/InfrastructureService/V10/Warehouse01/ShippingSyncSNService</toService>
    <Node>
        <MessageHead>
            <Action>InvoiceLog</Action>
            <Comment>No Comment</Comment>
            <Sender />
            <Language>CN</Language>
            <Namespace />
            <CompanyCode>{0}</CompanyCode>
        </MessageHead>
        <Body>
            <DropShipMaster ReferenceSONumber=""{1}"" ShippingDateTime=""{2}"" InvoiceNumber=""{3}"">
              <DropShipTransaction  WarehouseNumber=""{4}"">
                <DropShipSerialnumber/>
              </DropShipTransaction>
            </DropShipMaster>
            <ShippingCarrier/>
        </Body>
    </Node>
</Root>", eventMessage.CompanyCode, eventMessage.SOSysNo, DateTime.Now, eventMessage.InvoiceNo, eventMessage.StockSysNo);
            SSBSender.SendV2(ConstValue.SSB_From_Warehouse51FromService, ConstValue.SSB_To_PubSubServic, "Shipping", body, ConstValue.DataBaseName_SSB);
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}