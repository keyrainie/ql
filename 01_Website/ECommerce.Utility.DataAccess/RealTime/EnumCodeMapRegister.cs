using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<BusinessDataType>(new Dictionary<BusinessDataType, Int32>{
                { BusinessDataType.SO, 0 },
                { BusinessDataType.RMA, 1 },
                { BusinessDataType.CustomerMaster, 2 }                
            });

            EnumCodeMapper.AddMap<ChangeType>(new Dictionary<ChangeType, string>{
                { ChangeType.Add, "A" },
                { ChangeType.Update, "U" },
                { ChangeType.Delete, "D" }                
            });
        }
    }
}
