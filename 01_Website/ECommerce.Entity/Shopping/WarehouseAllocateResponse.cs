using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shopping
{
    public class WarehouseAllocateResponse
    {
        #region [ fields ]

        private List<AllocatedItemInventoryInfo> allocateItemInventoryInfoList;
        private int allocateResult;

        #endregion

        #region [ properties ]

        public List<AllocatedItemInventoryInfo> AllocateItemInventoryInfoList
        {
            get { return allocateItemInventoryInfoList; }
            set { allocateItemInventoryInfoList = value; }
        }

        public int AllocateResult
        {
            get { return allocateResult; }
            set { allocateResult = value; }
        }

        #endregion
    }
}
