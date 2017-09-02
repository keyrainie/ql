using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ECommerce.Entity;
using ECommerce.WebFramework;

namespace ECommerce.SOPipeline.Impl
{
    public class CPSPersister : IPersist
    {
        private const string CookieName_AdvEffectMonitor_cm_mmc = "adveffectmonitor.cm_mmc";
        private string cm_mmc;
        private string cmp1;
        private string cmp2;
        private string cmp3;
        private string cmp4;
        private AdvEffectMonitorInfo advEffectMonitor;

        public void Persist(OrderInfo order)
        {
            // 检查是否来自CMP
            if (!IsFromCMP())
                return;
            
            // 解析CMP代码
            AnalyseCMPCode();

            // 保存广告跟踪数据
            foreach (var subOrder in order.SubOrderList.Values)
            {
                BuildAdvEffectMonitorInfo(subOrder);
                PipelineDA.CreateAdvEffectMonitor(advEffectMonitor);
            }
        }

        /// <summary>
        /// 判断订单的成效是否来源广告活动
        /// </summary>
        /// <returns></returns>
        private bool IsFromCMP()
        {
            cm_mmc = CookieHelper.GetCookie<string>(CookieName_AdvEffectMonitor_cm_mmc);

            if (string.IsNullOrWhiteSpace(cm_mmc))
                return false;

            cm_mmc = HttpContext.Current.Server.UrlDecode(cm_mmc);
            return true;
        }

        /// <summary>
        /// 分析广告活动代码
        /// </summary>
        private void AnalyseCMPCode()
        {
            if (cm_mmc == null)
                return;

            string[] cm_mmc_words = Regex.Split(cm_mmc, "-_-");
            if (cm_mmc_words != null && cm_mmc_words.Length > 0)
            {
                if (cm_mmc_words.Length >= 1) cmp1 = cm_mmc_words[0];
                if (cm_mmc_words.Length >= 2) cmp2 = cm_mmc_words[1];
                if (cm_mmc_words.Length >= 3) cmp3 = cm_mmc_words[2];
                if (cm_mmc_words.Length >= 4) cmp4 = cm_mmc_words[3];
            }
        }

        /// <summary>
        /// 构建广告活动数据信息，作为存储到数据库所用。
        /// </summary>
        /// <param name="order"></param>
        private void BuildAdvEffectMonitorInfo(OrderInfo order)
        {
            advEffectMonitor = new AdvEffectMonitorInfo
            {
                CMP = cm_mmc,
                OperationType = "生成订单PlaceOrder",
                CustomerSysNo = order.Customer.SysNo,
                ReferenceSysNo = order.ID,
                CreateDate = DateTime.Now,
                CompanyCode = ConstValue.CompanyCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode,
                LanguageCode = order.LanguageCode,
                CMP1 = cmp1,
                CMP2 = cmp2,
                CMP3 = cmp3,
                CMP4 = cmp4
            };
        }
    }
}