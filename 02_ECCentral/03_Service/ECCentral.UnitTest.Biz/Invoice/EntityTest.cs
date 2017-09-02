using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Invoice
{
    [TestClass]
    public class EntityTest
    {
        [TestMethod]
        public void Test_Invoice_NetPayEntityClone()
        {
            var now = DateTime.Now;

            NetPayInfo netpayInfo = new NetPayInfo();
            netpayInfo.ApproveTime = now;
            netpayInfo.ApproveUserSysNo = 1;
            netpayInfo.ChannelID = "channel";
            netpayInfo.CompanyCode = "8601";
            netpayInfo.ExternalKey = "externalkey";
            netpayInfo.GiftCardPayAmt = 1000;
            netpayInfo.MasterSoSysNo = 100000;
            netpayInfo.Note = "note";
            netpayInfo.OrderAmt = 1000;
            netpayInfo.PayAmount = 1000;
            netpayInfo.PayTypeSysNo = 1;
            netpayInfo.PointPay = 1000;
            netpayInfo.PrePayAmt = 1000;
            netpayInfo.RelatedSoSysNo = 100000;
            netpayInfo.ReviewedTime = now;
            netpayInfo.ReviewedUserSysNo = 1;
            netpayInfo.SOSysNo = 100000;
            netpayInfo.Source = NetPaySource.Bank;
            netpayInfo.Status = NetPayStatus.Abandon;
            netpayInfo.SysNo = 1000;

            var cloneNetPayInfo = SerializationUtility.DeepClone<NetPayInfo>(netpayInfo);
            Assert.AreEqual(now, cloneNetPayInfo.ApproveTime);
            Assert.AreEqual(1, cloneNetPayInfo.ApproveUserSysNo);
            Assert.AreEqual("channel", cloneNetPayInfo.ChannelID);
            Assert.AreEqual("8601", cloneNetPayInfo.CompanyCode);
            Assert.AreEqual("externalkey", cloneNetPayInfo.ExternalKey);
            Assert.AreEqual(1000, cloneNetPayInfo.GiftCardPayAmt);
            Assert.AreEqual(100000, cloneNetPayInfo.MasterSoSysNo);
            Assert.AreEqual("note", cloneNetPayInfo.Note);
            Assert.AreEqual(1000, cloneNetPayInfo.OrderAmt);
            Assert.AreEqual(1000, cloneNetPayInfo.PayAmount);
            Assert.AreEqual(1, cloneNetPayInfo.PayTypeSysNo);
            Assert.AreEqual(1000, cloneNetPayInfo.PointPay);
            Assert.AreEqual(1000, cloneNetPayInfo.PrePayAmt);
            Assert.AreEqual(100000, cloneNetPayInfo.RelatedSoSysNo);
            Assert.AreEqual(now, cloneNetPayInfo.ReviewedTime);
            Assert.AreEqual(1, cloneNetPayInfo.ReviewedUserSysNo);
            Assert.AreEqual(100000, cloneNetPayInfo.SOSysNo);
            Assert.AreEqual(NetPaySource.Bank, cloneNetPayInfo.Source);
            Assert.AreEqual(NetPayStatus.Abandon, cloneNetPayInfo.Status);
            Assert.AreEqual(1000, cloneNetPayInfo.SysNo);
        }

        [TestMethod]
        public void Test_Invoice_PostPayEntityClone()
        {
            var now = DateTime.Now;

            PostPayInfo postpayInfo = new PostPayInfo();
            postpayInfo.ChannelID = "channel";
            postpayInfo.CompanyCode = "8601";
            postpayInfo.ConfirmedSOSysNo = 100000;
            postpayInfo.GiftCardPay = 1000;
            postpayInfo.MasterSoSysNo = 100000;
            postpayInfo.Note = "note";
            postpayInfo.OrderAmt = 1000;
            postpayInfo.PayAmount = 1000;
            postpayInfo.PayTypeSysNo = 1000;
            postpayInfo.PointPay = 1000;
            postpayInfo.PrepayAmt = 1000;
            postpayInfo.RemainAmt = 1000;
            postpayInfo.SOSysNo = 100000;
            postpayInfo.Status = PostPayStatus.Yes;
            postpayInfo.SysNo = 1000;

            var clonePostPayInfo = SerializationUtility.DeepClone<PostPayInfo>(postpayInfo);
            Assert.AreEqual("channel", clonePostPayInfo.ChannelID);
            Assert.AreEqual("8601", clonePostPayInfo.CompanyCode);
            Assert.AreEqual(100000, clonePostPayInfo.ConfirmedSOSysNo);
            Assert.AreEqual(1000, clonePostPayInfo.GiftCardPay);
            Assert.AreEqual(100000, clonePostPayInfo.MasterSoSysNo);
            Assert.AreEqual("note", clonePostPayInfo.Note);
            Assert.AreEqual(1000, clonePostPayInfo.OrderAmt);
            Assert.AreEqual(1000, clonePostPayInfo.PayAmount);
            Assert.AreEqual(1000, clonePostPayInfo.PayTypeSysNo);
            Assert.AreEqual(1000, clonePostPayInfo.PointPay);
            Assert.AreEqual(1000, clonePostPayInfo.PrepayAmt);
            Assert.AreEqual(1000, clonePostPayInfo.RemainAmt);
            Assert.AreEqual(100000, clonePostPayInfo.SOSysNo);
            Assert.AreEqual(PostPayStatus.Yes, clonePostPayInfo.Status);
            Assert.AreEqual(1000, clonePostPayInfo.SysNo);
        }
    }
}