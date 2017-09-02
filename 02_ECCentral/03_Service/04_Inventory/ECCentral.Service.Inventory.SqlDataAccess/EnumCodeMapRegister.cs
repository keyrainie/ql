using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<AdjustRequestStatus>(new Dictionary<AdjustRequestStatus, int>{
                { AdjustRequestStatus.Origin, 1 },
                { AdjustRequestStatus.Verified, 2 },
                { AdjustRequestStatus.OutStock, 3 },
                { AdjustRequestStatus.Abandon, -1 }
            });

            EnumCodeMapper.AddMap<ConvertRequestStatus>(new Dictionary<ConvertRequestStatus, int>{
                { ConvertRequestStatus.Origin, 1 },
                { ConvertRequestStatus.Verified, 2 },
                { ConvertRequestStatus.OutStock, 3 },
                { ConvertRequestStatus.Abandon, -1 }
            });

            EnumCodeMapper.AddMap<LendRequestStatus>(new Dictionary<LendRequestStatus, int>{
                { LendRequestStatus.Origin, 1 },
                { LendRequestStatus.Verified, 2 },
                { LendRequestStatus.OutStock, 3 },
                { LendRequestStatus.Abandon, -1 },
                { LendRequestStatus.ReturnPartly, 4 },
                { LendRequestStatus.ReturnAll, 5 }
            });

            EnumCodeMapper.AddMap<ShiftRequestStatus>(new Dictionary<ShiftRequestStatus, int>{
                { ShiftRequestStatus.Origin, 1 },
                { ShiftRequestStatus.Verified, 2 },
                { ShiftRequestStatus.OutStock, 3 },
                { ShiftRequestStatus.Abandon, -1 },
                { ShiftRequestStatus.InStock, 4 },
                { ShiftRequestStatus.PartlyInStock, 5 }
            });

            EnumCodeMapper.AddMap<ConvertProductType>(new Dictionary<ConvertProductType, int>{
                { ConvertProductType.Source, 1 },
                { ConvertProductType.Target, 2 }
            });

            EnumCodeMapper.AddMap<VirtualRequestActionType>(new Dictionary<VirtualRequestActionType, string>{
                { VirtualRequestActionType.Run, "R" },
                { VirtualRequestActionType.Close, "C" }
            });

            EnumCodeMapper.AddMap<SpecialShiftRequestType>(new Dictionary<SpecialShiftRequestType, int>{
                { SpecialShiftRequestType.Default, 0 },
                { SpecialShiftRequestType.DelayOutStock, 1 },
                { SpecialShiftRequestType.UrgencyDelayInStock, 2 },
                { SpecialShiftRequestType.NormalDelayInStock, 3 },
                { SpecialShiftRequestType.HandWork, 4 }
            });

            EnumCodeMapper.AddMap<WarehouseType>(new Dictionary<WarehouseType, int>{
                { WarehouseType.Real, 1 },
                { WarehouseType.Virtual, 2 }
            });

            EnumCodeMapper.AddMap<WarehouseOwnerType>(new Dictionary<WarehouseOwnerType, int>{
                { WarehouseOwnerType.Self, 1 },
                { WarehouseOwnerType.ThirdParty, 2 }
            });

            EnumCodeMapper.AddMap<ValidStatus>(new Dictionary<ValidStatus, int>{
                { ValidStatus.Valid, 0 },
                { ValidStatus.InValid, -1}
            });

            EnumCodeMapper.AddMap<CustomsCodeMode>(new Dictionary<CustomsCodeMode, string>{
                { CustomsCodeMode.DirectImportMode, "2244" },
                { CustomsCodeMode.PudongAirportTradeMode , "2216" },
                { CustomsCodeMode.YangshanTradeMode , "2249" },
                { CustomsCodeMode.WaigaoqiaoTradeMode , "2218" }
            });
        }
    }
}
