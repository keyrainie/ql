using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Invoice
{
    [TestClass]
    public class PayTest
    {
        [TestMethod]
        public void Test_LockByVendor()
        {
            int vendorSysNo = 2347;
            ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendor(vendorSysNo, true);
        }

        [TestMethod]
        public void Test_UnLockByVendor()
        {
            int vendorSysNo = 2347;
            ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendor(vendorSysNo, false);
        }

        [TestMethod]
        public void Test_LockByVendorPM()
        {
            int vendorSysNo = 243;
            List<int> holdPMSysNoList = new List<int>()
            {
                3311
            };
            ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendorPM(vendorSysNo, true, holdPMSysNoList, null);
        }

        [TestMethod]
        public void Test_UnLockByVendorPM()
        {
            int vendorSysNo = 243;
            List<int> unholdPMSysNoList = new List<int>();
            ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendorPM(vendorSysNo, false, null, unholdPMSysNoList);
        }

        [TestMethod]
        public void Test_ObjectFactory()
        {
            var BL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
        }
    }
}