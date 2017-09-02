using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nesoft.ECWeb.DataAccess.Consultation;
using Nesoft.ECWeb.Entity.Consultation;

namespace Nesoft.ECWeb.Facade.Consultation
{
    public class ConsultationFacade
    {
        public ConsultationInfo CheckProductConsultInfo(int customerSysNo)
        {
            return ConsultationDA.CheckProductConsultInfo(customerSysNo);
        }

    }
}
