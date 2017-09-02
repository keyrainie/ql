using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

namespace IPP.InventoryMgmt.JobV31.Common
{
    //public class InventoryQtyArgs : EventArgs
    //{
    //    public List<ProductEntity> ProductEntityList { get; set; }

    //    public List<InventoryQtyEntity> InventoryQtyEntityList { get; set; }
    //}

    //public delegate void SynInventoryQtyRunning(object sender, InventoryQtyArgs args);


    public class ThirdPartInventoryArgs : EventArgs
    {
        public List<ThirdPartInventoryEntity> ThirdPartInventoryList { get; set; }
    }

    public class ThirdPartInventoryErrorArgs : ThirdPartInventoryArgs
    {
        public Exception Exception { get; set; }
    }

    public delegate void ThirdPartInventoryRunning(object sender,ThirdPartInventoryArgs args);

    public delegate void ThirdPartInventoryRunningError(object sender,ThirdPartInventoryErrorArgs args);

}
