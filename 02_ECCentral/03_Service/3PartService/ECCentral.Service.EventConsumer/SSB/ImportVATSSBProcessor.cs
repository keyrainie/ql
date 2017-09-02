using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventConsumer;
using ECCentral.Service.EventMessage;

namespace ECCentral.Service.EventConsumer
{
    public class ImportVATSSBProcessor : IConsumer<ImportVATSSBMessage>
    {
        public void HandleEvent(ImportVATSSBMessage eventMessage)
        {
            string body = String.Format(@"
<MessageHead>
    <Type>ImportInvoice</Type>
    <Comment>Send so message to local warehouse.</Comment>
</MessageHead>
<Body>
    <InvoiceNodes>
        <InvoiceNode>
            <OrderSysNo>{0}</OrderSysNo>
            <OrderType>{1}</OrderType>
            <WarehouseNumber>{2}</WarehouseNumber>
            <ComputerNo>0</ComputerNo>
        </InvoiceNode>
    </InvoiceNodes> 
</Body>", eventMessage.SOSysNo, eventMessage.OrderType.ToString(), eventMessage.StockSysNo);
            SSBSender.SendV2(ConstValue.SSB_From_WMSShippingProcessDownLoad, ConstValue.SSB_To_PubSubServic, "DC01WMSShippingProcessDownLoad_51", body, ConstValue.DataBaseName_SSB);

        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
