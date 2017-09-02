using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<RMAOwnBy>(new Dictionary<RMAOwnBy, int>{
                { RMAOwnBy.Origin, 0 },
                { RMAOwnBy.Customer, 1 },
                { RMAOwnBy.Self, 2 },
            });

            EnumCodeMapper.AddMap<RMALocation>(new Dictionary<RMALocation, int>
                {
                    {RMALocation.Origin,0},
                    {RMALocation.Self,1},
                    {RMALocation.Vendor,2}
                });

            EnumCodeMapper.AddMap<SellersType>(new Dictionary<SellersType, string>
                {
                    {SellersType.Self,"NEG"},
                    {SellersType.Seller,"MET"}
                });
        }
    }
}
