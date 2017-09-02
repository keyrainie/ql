using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentMgmt.GiftCardPoolInterface.DA;
using ContentMgmt.GiftCardPoolInterface.Entities;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    public class GiftCardPoolDATest
    {
        [TestMethod]
        public void TestSendMailInfo()
        {
            GiftCardPoolDA.SendMailInfo("testSubject", "testBody");
        }

        [TestMethod]
        public void TestGiftCardPoolDA()
        {
            GiftCardPoolEntity entity = new GiftCardPoolEntity();
            GiftCardPoolDA.Insert(entity);
            Assert.AreNotEqual<int>(0, entity.SysNo);
            Debug.WriteLine(entity.SysNo.ToString());
            GiftCardPoolDA.Update(entity);
            GiftCardPoolDA.Delete(entity.SysNo);
        }
    }
}
