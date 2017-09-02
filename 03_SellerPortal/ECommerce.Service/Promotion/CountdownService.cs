using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using System.IO;
using System.Data;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Common;
using ECommerce.DataAccess.Promotion;
using ECommerce.Enums.Promotion;
using ECommerce.Utility;
using ECommerce.Service.Product;
using ECommerce.WebFramework;
using ECommerce.Enums;

namespace ECommerce.Service.Promotion
{
    public class CountdownService
    {
        private CountdownDA m_CountdownDA = new CountdownDA();

        public void Create(CountdownInfo entity)
        {
            ValidateEntity(entity);
            m_CountdownDA.CreateCountdown(entity);
        }

        public void Update(CountdownInfo entity)
        {
            ValidateEntity(entity);
            m_CountdownDA.UpdateCountdown(entity);
        }

        public void Submit(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    var entity = m_CountdownDA.Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    //验证记录是否可提交审核
                    if (entity.CanSubmit == false)
                    {
                        sb.AppendLine(string.Format("活动#{0}的状态不是初始态，不可执行提交审核操作！", id));
                        continue;
                    }
                    entity.Status = CountdownStatus.WaitForPrimaryVerify;
                    entity.EditUserName = opUserName;
                    m_CountdownDA.MaintainCountdownStatus(entity);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        public void Void(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    var entity = m_CountdownDA.Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    //验证记录是否可作废
                    if (entity.CanVoid == false)
                    {
                        sb.AppendLine(string.Format("活动#{0}作废失败，只有初始态、待审核、就绪或审核未通过的活动可执行作废操作！", id));
                        continue;
                    }
                    entity.Status = CountdownStatus.Abandon;
                    entity.EditUserName = opUserName;
                    m_CountdownDA.MaintainCountdownStatus(entity);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        public void Stop(int sellerSysNo, string opUserName, params int[] sysNos)
        {
            if (sysNos != null && sysNos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var id in sysNos)
                {
                    var entity = m_CountdownDA.Load(id);
                    if (entity.SellerSysNo != sellerSysNo)
                    {
                        sb.AppendLine(string.Format(LanguageHelper.GetText("活动#{0}:您无权操作此单据！"), id));
                        continue;
                    }
                    //验证记录是否可终止
                    if (entity.CanStop == false)
                    {
                        sb.AppendLine(string.Format("活动#{0}终止失败，只有运行状态的活动才能终止！", id));
                        continue;
                    }
                    entity.EndTime = DateTime.Now;
                    entity.EditUserName = opUserName;
                    m_CountdownDA.MaintainCountdownEndTime(entity);
                }
                if (sb.Length > 0)
                {
                    throw new BusinessException("操作已完成。<br/>" + sb.ToString());
                }
            }
        }

        public CountdownQueryResult Load(int id)
        {
            return m_CountdownDA.Load(id);
        }

        public QueryResult<CountdownQueryResult> Query(CountdownQueryFilter filter)
        {
            if (filter.CountdownToStartTime.HasValue)
            {
                filter.CountdownToStartTime = filter.CountdownToStartTime.Value.AddDays(1);
            }
            if (filter.CountdownToEndTime.HasValue)
            {
                filter.CountdownToEndTime = filter.CountdownToEndTime.Value.AddDays(1);
            }
            return m_CountdownDA.Query(filter);
        }

        /// <summary>
        /// 团购排重：验证商品在一段时间内是否存在有效的团购活动
        /// </summary>
        public bool CheckGroupBuyAndCountDownConflict(List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            return m_CountdownDA.CheckConflict(0,productSysNos, beginDate, endDate);
        }

        private void ValidateEntity(CountdownInfo entity)
        {
            if (entity.SysNo > 0)
            {
                var item = Load(entity.SysNo);
                if (item == null)
                {
                    throw new BusinessException(LanguageHelper.GetText("单据已不存在。"));
                }
                if (item.SellerSysNo != entity.SellerSysNo)
                {
                    throw new BusinessException(LanguageHelper.GetText("您无权操作此单据。"));
                }
                if (item.Status != CountdownStatus.Init)
                {
                    throw new BusinessException(LanguageHelper.GetText("此单据的状态不是初始态，无法执行编辑或提交审核操作。"));
                }
            }
            //验证商品是否存在
            var product = ProductService.GetProduct(entity.SellerSysNo.Value, entity.ProductID);
            if (product == null)
            {
                throw new BusinessException(LanguageHelper.GetText("活动商品不存在！"));
            }
            if (product.Status != ProductStatus.Active)
            {
                throw new BusinessException(LanguageHelper.GetText("活动商品为未上架状态,不能设置活动！"));
            }
            entity.ProductSysNo = product.SysNo;
            if (product.CurrentPrice <= entity.CountDownCurrentPrice)
            {
                throw new BusinessException(LanguageHelper.GetText("活动价格不能大于等于商品卖价！"));
            }

            if (entity.StartTime >= entity.EndTime)
            {
                throw new BusinessException("限时抢购的开始时间必须小于结束时间。");
            }

            if (entity.EndTime <= DateTime.Now)
            {
                throw new BusinessException("限时抢购的结束时间必须大于当前时间。");
            }
            if (entity.CountDownQty <= 0)
            {
                throw new BusinessException("数量上限必须大于0。");
            }


            //验证商品是否存在有效的限时抢购活动
            List<int> productSysNos = new List<int>();
            productSysNos.Add(entity.ProductSysNo);
            if (m_CountdownDA.CheckConflict(entity.SysNo, productSysNos, entity.StartTime, entity.EndTime))
            {
                throw new BusinessException(LanguageHelper.GetText("该商品已经存在一个时间冲突的促销活动！"));
            }
            //验证商品是否存在有效的团购活动
            GroupBuyingService groupBuyService = new GroupBuyingService();
            if (groupBuyService.CheckGroupBuyConflict(entity.ProductSysNo, entity.StartTime, entity.EndTime))
            {
                throw new BusinessException("该商品已存在相冲突时间段内相关的团购活动，不能重复添加促销活动。");
            }
        }
    }
}
