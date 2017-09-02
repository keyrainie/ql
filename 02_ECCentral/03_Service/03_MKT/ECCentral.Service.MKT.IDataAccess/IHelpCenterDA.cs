using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IHelpCenterDA
    {
        void Create(HelpTopic entity);

        void Update(HelpTopic entity);

        HelpTopic Load(int sysNo);

        bool CheckTitleExists(int excludeSysNo, string title, string companyCode,string channelID);
    }
}
