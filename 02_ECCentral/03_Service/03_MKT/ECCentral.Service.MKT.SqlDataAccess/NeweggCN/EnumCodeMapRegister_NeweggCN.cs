using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    public class EnumCodeMapRegister_NeweggCN : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<AmbassadorStatus>(new Dictionary<AmbassadorStatus, int>
            {
                {AmbassadorStatus.UnActive,1},
                {AmbassadorStatus.Active,2},
                {AmbassadorStatus.Canceled,3}                
            });

            EnumCodeMapper.AddMap<UserType>(new Dictionary<UserType, int>
            {
                {UserType.NeweggAmbassador,0},
                {UserType.NeweggEmployee,4}           
            });
        }
    }
}
