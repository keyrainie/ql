using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(CustomerContactProcessor))]
    public class CustomerContactProcessor
    {
        private ICustomerContactDA da = ObjectFactory<ICustomerContactDA>.Instance;

        public virtual CustomerContactInfo LoadByRequestSysNo(int sysNo)
        {
            CustomerContactInfo entity = null;
            entity = da.Load(sysNo);
            if (entity == null)
            {
                RMARequestInfo request = ObjectFactory<IRequestDA>.Instance.LoadBySysNo(sysNo);
                if (request != null)
                {
                    entity = new CustomerContactInfo();
                    entity.ReceiveAddress = request.Address;
                    entity.ReceiveAreaSysNo = request.AreaSysNo;
                    entity.ReceiveContact = request.Contact;
                    entity.ReceiveName = request.Contact;
                    entity.ReceivePhone = request.Phone;
                    if (request.SOSysNo.HasValue)
                    {
                        SOInfo so = ExternalDomainBroker.GetSOInfo(request.SOSysNo.Value);                        
                        if (so != null)
                        {
                            entity.ReceiveCellPhone = so.ReceiverInfo.MobilePhone;
                            entity.ReceiveZip = so.ReceiverInfo.Zip;
                        }
                    }
                }
            }
            return entity;
        }

        public virtual CustomerContactInfo LoadOriginByRequestSysNo(int sysNo)
        {
            CustomerContactInfo entity = null;
            entity = da.LoadOrigin(sysNo);

            return entity;
        }

        public virtual CustomerContactInfo Create(CustomerContactInfo entity)
        {
            return da.Insert(entity);            
        }

        public virtual void Update(CustomerContactInfo entity)
        {
            da.Update(entity);
        }
    }
}
