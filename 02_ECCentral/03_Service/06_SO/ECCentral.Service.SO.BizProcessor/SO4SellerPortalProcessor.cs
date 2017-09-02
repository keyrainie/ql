using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SO4SellerPortalProcessor))]
    public class SO4SellerPortalProcessor
    {
        ISO4SellerPortalDA m_da = ObjectFactory<ISO4SellerPortalDA>.Instance;

        public void UpdateReceiveStatusAndSOStatus(string message)
        {
            SOReceiveStatusEntity entity = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<SOReceiveStatusEntity>(message);
            string companyCode = entity.Node.RequestRoot.MessageHeader.CompanyCode;
            if (entity.Node != null
                && entity.Node.RequestRoot != null
                && entity.Node.RequestRoot.Body != null
                && entity.Node.RequestRoot.Body.Msg != null
                && entity.Node.RequestRoot.Body.Msg.SalesOrderStatusList != null)
            {
                List<SalesOrderStatusEntity> solist = entity.Node.RequestRoot.Body.Msg.SalesOrderStatusList;
                foreach (SalesOrderStatusEntity soEntity in solist)
                {
                    m_da.UpdateSOSellerStatus(soEntity, companyCode);
                }
            }
            else
            {
                //BusinessProcessHelper.ThrowMessage("","消息错误");
            }

        }

        public void UpdateSOMasterInvoiceNo(string message)
        {
            SOInvoicePrintedInfoEnity entity = ECCentral.Service.Utility.SerializationUtility.XmlDeserialize<SOInvoicePrintedInfoEnity>(message);

            //string companyCode 

            if (entity.Node != null
                && entity.Node.RequestRoot != null
                && entity.Node.RequestRoot.Body != null
                && entity.Node.RequestRoot.Body.SalesOrder != null)
            {
                string companyCode = "";
                if (entity.Node.RequestRoot.MessageHeader != null)
                    companyCode = entity.Node.RequestRoot.MessageHeader.CompanyCode;

                m_da.UpdateSOMasterInvoiceNo(entity.Node.RequestRoot.Body.SalesOrder, companyCode);
            }
        }
    }
}
