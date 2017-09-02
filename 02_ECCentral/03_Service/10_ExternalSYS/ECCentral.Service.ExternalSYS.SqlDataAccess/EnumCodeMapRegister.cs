using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        #region IStartup Members

        public void Start()
        {
            #region EIMS
            EnumCodeMapper.AddMap<InvoiceStatus>(new Dictionary<InvoiceStatus, string>
                {
                    {InvoiceStatus.AutoClose,"F"},
                    {InvoiceStatus.Open,"O"},
                    {InvoiceStatus.Lock,"E"}
                });
            #endregion

            #region VendorPortal

            EnumCodeMapper.AddMap<ValidStatus>(new Dictionary<ValidStatus, string>{
                { ValidStatus.Active, "A" },
                { ValidStatus.DeActive, "D" }
            });

            #endregion

            #region CPS

            EnumCodeMapper.AddMap<FinanceStatus>(new Dictionary<FinanceStatus, string>{
                { FinanceStatus.Paid, "P" },
                { FinanceStatus.Settled, "S" },
                { FinanceStatus.Unsettled, "U" },
                {FinanceStatus.UnRequest, "R"},
                { FinanceStatus.Abandon, "V" }
            });

            EnumCodeMapper.AddMap<CPSOrderType>(new Dictionary<CPSOrderType, string>{
                { CPSOrderType.SO, "SO" },
                { CPSOrderType.RMA, "RMA" }
            });

            EnumCodeMapper.AddMap<ToCashStatus>(new Dictionary<ToCashStatus, string>{
                { ToCashStatus.Paid, "P" },
                { ToCashStatus.Requested, "R" },
                { ToCashStatus.Confirmed, "C" }
            });

            EnumCodeMapper.AddMap<AdvertisingType>(new Dictionary<AdvertisingType, string>{
                { AdvertisingType.Custom, "C" },
                { AdvertisingType.IMG, "I" },
                { AdvertisingType.TEXT, "T" }
            });
            EnumCodeMapper.AddMap<AuditStatus>(new Dictionary<AuditStatus, string>{
                { AuditStatus.AuditClearance, "A" },
                { AuditStatus.AuditNoClearance, "D" },
                { AuditStatus.AuditReady, "O" }
            });
            EnumCodeMapper.AddMap<UserType>(new Dictionary<UserType, string>{
                { UserType.Personal, "P" },
                { UserType.Enterprise, "E" }
                 //{ UserType.Other, "O" }
            });
            EnumCodeMapper.AddMap<IsActive>(new Dictionary<IsActive, string>{
                { IsActive.DeActive, "0" },
                { IsActive.Active, "1" },
            });
            EnumCodeMapper.AddMap<IsLock>(new Dictionary<IsLock, string>{
                { IsLock.Lock, "L" },
                { IsLock.UnLock, "U" },
            });

            EnumCodeMapper.AddMap<CanProvideInvoice>(new Dictionary<CanProvideInvoice, string>{
                { CanProvideInvoice.Yes, "Y" },
                { CanProvideInvoice.No, "N" }
            });

            #endregion
        }

        #endregion
    }
}
