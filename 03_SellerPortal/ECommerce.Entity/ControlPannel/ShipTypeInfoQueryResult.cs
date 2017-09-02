using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.ControlPannel
{
    public class ShipTypeInfoQueryResult : ShipTypeInfo
    {
        public string UIStoreType
        {
            get
            {
                return this.StoreType.GetDescription();
            }
        }
        public string UIShipTypeEnum
        {
            get
            {
                return this.ShipTypeEnum.GetDescription();
            }
        }
        public string UIIsOnlineShow
        {
            get
            {
                return this.IsOnlineShow.GetDescription();
            }
        }
        public string UIIsWithPackFee
        {
            get
            {
                return this.IsWithPackFee.GetDescription();
            }
        }
        public string UIPackStyle
        {
            get
            {
                return this.PackStyle.GetDescription();
            }
        }
        public string UIDeliveryType
        {
            get
            {
                return this.DeliveryType.GetDescription();
            }
        }
        public string UIDeliveryPromise
        {
            get
            {
                return this.DeliveryPromise.GetDescription();
            }
        }
        public string ShipTypeNameforDrp
        {
            get;
            set;
        }
    }
}
