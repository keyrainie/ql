using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerchantCommissionSettle.Entities
{
    internal sealed class Constants
    {
        public class CommissionLogType
        {
            public const string SaleCommission = "SAC";

            public const string OrderCommission = "SOC";

            public const string DeliveryFee = "DEF";
        }

        public class CommissionMasterStatus
        {
            public const string Original = "ORG";

            public const string Settled = "SET";

            public const string Closed = "CLS";
        }

        public class OrderType
        {
            public const string SO = "SO";

            public const string RMA = "RMA";
        }
    }
}
