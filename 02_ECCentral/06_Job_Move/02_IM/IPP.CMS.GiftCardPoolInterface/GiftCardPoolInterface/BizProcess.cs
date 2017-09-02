using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ContentMgmt.GiftCardPoolInterface.DA;
using ContentMgmt.GiftCardPoolInterface.Entities;
using Newegg.Oversea.ContentMgmt.Utility;

namespace ContentMgmt.GiftCardPoolInterface
{
    public class BizProcess
    {
        GiftCardPoolEntity entity = new GiftCardPoolEntity();
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;

        public void Process()
        {
            try
            {
                WriteLog("开始循环礼品卡Item");
                List<GCItemEntity> gclist = GiftCardPoolDA.GetGCItemList();
                foreach (GCItemEntity item in gclist)
                {
                    ProcessEach(item);
                }
            }
            catch (Exception er)
            {
                WriteLog("Exception:" + er.Message);
            }
        }

        private void ProcessEach(GCItemEntity gcItem)
        {
            DateTime beginDate = DateTime.Now;
            string barcodeParam = gcItem.BarPrefix + "%";
            int activeCount = GiftCardPoolDA.GetActiveCount(barcodeParam,gcItem.Amount);

            if (activeCount < ConstValues.AvailableCount)
            {
                //get identity, Initiate entity.SysNo
                if (entity.SysNo == 0)
                {
                    entity.IsStatusActive = false;
                    GiftCardPoolDA.Insert(entity);
                    GiftCardPoolDA.Delete(entity.SysNo);
                }
                //get barcode number
                string maxBarcode = GiftCardPoolDA.GetMaxBarcode(barcodeParam);
                long barNumber = long.Parse(maxBarcode.Substring(4));

                int i = activeCount;
                entity.IsStatusActive = true;
                entity.AmountType = (int)gcItem.Amount;
                string msg = string.Empty;
                while (i < ConstValues.AvailableCount)
                {
                    ++entity.SysNo;
                    entity.Code = GetGCCode(entity.SysNo);
                    entity.Barcode = string.Format("{0}{1}", gcItem.BarPrefix
                        , (++barNumber).ToString("0000000000"));
                    entity.Password = GetPassword();
                    GiftCardPoolDA.Insert(entity);
                    msg = string.Format("BarPrefix={0},AvailableCount={1},i={2}", gcItem.BarPrefix, ConstValues.AvailableCount, ++i);
                    WriteLog(msg);
                }
            }
            int logCount = ConstValues.AvailableCount - activeCount;
            if (logCount < 0)
            {
                logCount = 0;
            }
            string message = string.Format("{2} - 新增{0}条数据，花费{1}秒时间。", logCount.ToString(),
                    (DateTime.Now - beginDate).TotalSeconds.ToString(),
                    gcItem.BarPrefix);
            //LogHelper.WriteOverseaLog<GiftCardPoolEntity>(entity,
            //    string.Empty, string.Empty, "GiftCardPoolInterface",
            //    message, entity.SysNo.ToString());
            WriteLog(message);
        }

        private static StringBuilder builder = new StringBuilder();

        private static Random random = new Random();

        private static string GetGCCode(int identity)
        {
            WriteLog("GetGCCode");
            const int identityLength = 8;
            const int randomLength = 6;
            builder.Length = 0;
            builder.Append(MyUtility.GenerateCode(identity + MyUtility.InitialCodeValue, identityLength));
            int maxValue = ConstValues.CodeDimension - 1;
            for (int i = 0; i < randomLength; i++)
            {
                builder.Append(ConstValues.AvailableCode[random.Next(maxValue)]);
            }
            return builder.ToString();
        }

        private static string GetPassword()
        {
            WriteLog("GetPassword");
            builder.Length = 0;
            int maxValue = ConstValues.PasswordDimension - 1;
            for (int i = 0; i < 6; i++)
            {
                builder.Append(ConstValues.AvailablePassword[random.Next(maxValue)]);
            }
            return DESCryptographer.Encrypt(builder.ToString());
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
        }
    }
}
