using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.EventConsumer;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.SO.BizProcessor.SO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.SO
{
    [TestClass]
    public class SOProcessorTest
    {
        [TestMethod]
        public void SendEmail()
        {
            new SOSendMessageProcessor().SOAuditedSendEmailToCustomer(new SOProcessor().GetSOBySOSysNo(10039652));
        }

        [TestMethod]
        public void Test_SO_Calculate()
        {
            //SOInfo soInfo = GetTestSO();

            SOCaculator processor = new SOCaculator();

            //mock接口方法
            //processor.CommonBiz = new mock_CommonBiz();
            //processor.CustomerBiz = new mock_CustomerBiz();

            //processor.Calculate(soInfo);
        }

        /// <summary>
        /// 取得订单信息
        /// </summary>
        [TestMethod]
        public void GetSOInfoBySOSysNo_VendorGift()
        {
            new SOProcessor().GetSOBySOSysNo(34728751);// 附件Mapping测试
        }

        /// <summary>
        /// 取得订单信息
        /// </summary>
        [TestMethod]
        public void GetSOInfoBySOSysNo_Accessory()
        {
            new SOProcessor().GetSOBySOSysNo(26468272);// 附件Mapping测试
        }

        /// <summary>
        /// 取得订单信息
        /// </summary>
        [TestMethod]
        public void GetSOInfoBySOSysNo_SelfGift()
        {
            new SOProcessor().GetSOBySOSysNo(34458514);//赠品规则Mapping 测试
        }

        /// <summary>
        /// 取得订单信息
        /// </summary>
        [TestMethod]
        public void GetSOInfoBySOSysNo_Combo()
        {
            new SOProcessor().GetSOBySOSysNo(34363058);//组合销售Mapping测试
        }

        [TestMethod]
        public void WriteSOLog()
        {
            SOInfo soInfo = new SOProcessor().GetSOBySOSysNo(34363058);//组合销售Mapping测试

            new SOLogProcessor().WriteSOLog(
                  ECCentral.BizEntity.Common.BizLogType.Sale_SO_Audit,
                  "订单写日志测试", soInfo);
        }

        [TestMethod]
        public void GetSOBaseInfoList()
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.AddRange(new int[] { 26468272, 34458514, 34363058, 34728751 });
            List<SOBaseInfo> infoList = new SOProcessor().GetSOBaseInfoBySOSysNoList(soSysNoList);
        }

        [TestMethod]
        public void GetSOInfoList()
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.AddRange(new int[] { 26468272, 34458514, 34363058, 34728751 });
            List<SOInfo> infoList = new SOProcessor().GetSOBySOSysNoList(soSysNoList);
        }

        /// <summary>
        /// 订单审核测试
        /// </summary>
        [TestMethod]
        public void SOAudit()
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.AddRange(new int[] { 26468272, 34458514, 34363058, 34728751 });
            //  new SOAppService().AuditSO(soSysNoList, false, false, false);
        }

        /// <summary>
        /// 订单主管审核测试
        /// </summary>
        [TestMethod]
        public void SOManagerAudit()
        {
            List<int> soSysNoList = new List<int>();
            soSysNoList.AddRange(new int[] { 26468272, 34458514, 34363058, 34728751 });
            // new SOAppService().AuditSO(soSysNoList, false, true, false);
        }

        #region 金额去分算法

        [TestMethod]
        public void TruncMoney()
        {
            decimal d = TruncMoney(100.91M);//100.90
            d = TruncMoney(100.99M);//100.90
        }

        private decimal TruncMoney(decimal amount)
        {
            int tempAmt = (int)(amount * 10);
            return tempAmt / 10M;
        }

        #endregion 金额去分算法

        [TestMethod]
        public void SOAuditedSendEmailToCustomer()
        {
            new ECCentral.Service.SO.BizProcessor.SOSendMessageProcessor().SOAuditedSendEmailToCustomer(new SOProcessor().GetSOBySOSysNo(34458514));

            new ECCentral.Service.SO.BizProcessor.SOSendMessageProcessor().SOAuditedSendEmailToCustomer(new SOProcessor().GetSOBySOSysNo(34363058));
        }

        [TestMethod]
        public void InserSOPrice()
        {
            ECCentral.Service.SO.SqlDataAccess.SOPriceDA da = new Service.SO.SqlDataAccess.SOPriceDA();
            da.InsertSOPrice(new SOPriceMasterInfo(), "zh-cn");
        }

        [TestMethod]
        public void GetAppSetting()
        {
            System.Xml.Linq.XElement xe = ECCentral.Service.SO.BizProcessor.AppSettingHelper.OrderBizConfig;
        }

        [TestMethod]
        public void WMSSendSSB()
        {
            new WMSSSBMessageProcessor().HandleEvent(new Service.EventMessage.WMSSOActionRequestMessage());
        }
    }
}