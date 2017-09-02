using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shipping
{
    public class ShipTypeAndAreaUnInfo
    {
        #region [ fields ]
        private int id;
        private int shippingTypeID;
        private int areaID;
        #endregion

        #region [ properties ]


        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public int ShippingTypeID
        {
            get { return this.shippingTypeID; }
            set { this.shippingTypeID = value; }
        }

        public int AreaID
        {
            get { return this.areaID; }
            set { this.areaID = value; }
        }

        #endregion
    }
}
